using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Events;

namespace Ballpoint.SpeechBubble {

    [RequireComponent(typeof(InkEventDispatcher))]
    [DisallowMultipleComponent]
    [HelpURL(HelpURL)]
	[AddComponentMenu("Ballpoint/Dialog Box Manager")]
    public class DialogBoxManager : MonoBehaviour {

        public const string HelpURL = InkManager.HelpURL + "#dialog-box-manager";
        private InkManager ink;
        private InkEventDispatcher inkEvents;

        [HideInInspector]
        public string speaker = "";

        [Header("Dialog Box Components")]
        public GameObject dialogBox;
        public Text speakerDisplay;
        public Text textDisplay;
        public GameObject nextButtonVisibilityContainer;
        public GameObject choicesParent;

        [Header("Prefabs")]
        public Button choicePrefab;

        [SerializeField]
        [HideInInspector]
        private bool _InitiallyWiredEvents = false;

        private void OnValidate() {
            ink = ink ?? InkManager.FindAny();
            inkEvents = inkEvents ?? InkEventDispatcher.FindAny();

            if (_InitiallyWiredEvents == false) {
                _InitiallyWiredEvents = true;
                WireEvents();
            }
        }
        
		[ContextMenu("Re-Add Events to Dispatcher")] 
        private void WireEvents() { 
            if (inkEvents == null) Debug.LogError("InkEvents is null");

            // Hook up the events so they show up in the inspector
            UnityEventTools.AddPersistentListener(inkEvents.StoryUpdate, this.StoryUpdate);
            UnityEventTools.AddPersistentListener(inkEvents.StoryEnded, this.HideDialogBox);

            var watcher = inkEvents.GetOrAddTagChangeWatcher(InkTagState.StartsWith, "said:"); 

            if (watcher == null) Debug.LogError("'Said' watcher is null");

            UnityEventTools.AddPersistentListener(watcher.tagEvent, SetSpeaker);
        }
        

        // Start is called before the first frame update
        void Start() {
            ink.Initialize();
            SetSpeaker(ink.story.variablesState["lastKnownSpeaker"] as string ?? "");
            ink.BeginStory();

        }

        public void HideDialogBox() => dialogBox?.SetActive(false);

        private void SetSpeaker(string value) {
            speaker = value == "None" ? "" : value;
            ink.story.variablesState["lastKnownSpeaker"] = speaker;
            speakerDisplay.text = speaker;

        }

        // Update is called once per frame
        void Update() {

        }

        private void StoryUpdate(Ballpoint.StoryUpdate update) {
            textDisplay.text = update.text;
            var hasChoices = update.choices.Count > 0;
            choicesParent.SetActive(hasChoices);
            nextButtonVisibilityContainer.SetActive(!hasChoices);
            if (hasChoices) {
                var buttons = choicesParent.GetComponentsInChildren<Button>();
                foreach (var button in buttons) button.gameObject.SetActive(false);

                for (var i = 0; i < update.choices.Count; i++) {
                    var buttonExists = i < buttons.Length;
                    Button button;
                    if (buttonExists) button = buttons[i];
                    else {
                        button = Instantiate(choicePrefab, choicesParent.transform);
                        var currentI = i; // if we don't do this, it uses the final value of i
                        button.onClick.AddListener(() => ink.Continue(currentI));
                    }
                    button.GetComponentInChildren<UnityEngine.UI.Text>().text = update.choices[i];
                    button.gameObject.SetActive(true);
                }
            }
        }
    }
}

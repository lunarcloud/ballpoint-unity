using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Ballpoint.SpeechBubble {

    [RequireComponent(typeof(InkManager))]
    [DisallowMultipleComponent]
    [HelpURL(HelpURL)]
    public class DialogBoxManager : MonoBehaviour {

        public const string HelpURL = InkManager.HelpURL + "#dialog-box-manager";
        private InkManager ink;

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

        private void OnValidate() {
            ink = ink ?? GetComponent<InkManager>();
        }

        // Start is called before the first frame update
        void Start() {
            ink.AddTagListener("said", SetSpeaker);
            ink.storyUpdate.AddListener(StoryUpdate);
            ink.storyEnded.AddListener(() => dialogBox?.SetActive(false));
            ink.Initialize();
            SetSpeaker(ink.story.variablesState["lastKnownSpeaker"] as string ?? "");
            ink.BeginStory();

        }
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

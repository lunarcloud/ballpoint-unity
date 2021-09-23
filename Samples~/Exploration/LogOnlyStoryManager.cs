using System.Collections;
using UnityEngine;

namespace Ballpoint.Sample.Exploration {

    [RequireComponent(typeof(InkEventDispatcher))]
    [DisallowMultipleComponent]
    [HelpURL(InkManager.HelpURL)]
    public class LogOnlyStoryManager : MonoBehaviour {
        private InkManager ink;
        private InkEventDispatcher inkEvents;

        public bool autoContinue = true;

        [SerializeField]
        [HideInInspector]
        private bool _InitiallyWiredEvents = false;
        
        private void OnValidate() {
            ink = ink ?? InkManager.FindAny();
            inkEvents = inkEvents ?? InkEventDispatcher.FindAny();

            // Hook up the events so they show up in the inspector
            if (_InitiallyWiredEvents == false) {
                _InitiallyWiredEvents = true;
                WireEvents();
            }
        }
        
		[ContextMenu("Re-Add Events to Dispatcher")] 
        private void WireEvents() {       
            UnityEventTools.AddPersistentListener(inkEvents.StoryUpdate, StoryUpdate);
        }

        void Start() {
            ink.Initialize();
            ink.BeginStory();
        }

        public void StoryUpdate(StoryUpdate update) {
            Debug.Log(update);
            StartCoroutine(AutoContinue(update));
        }

        // must async auto-continue, or new messages happen immediately, 
        // in the middle of the old message, leading to out-of-order issues.
        private IEnumerator AutoContinue(StoryUpdate update) {
            if (autoContinue && (ink.story.canContinue && update.choices.Count == 0)) {
                yield return new WaitForSeconds(0);
                ink.Continue();
            }
            if (autoContinue && update.atEnd) {
                Quit();
            }
        }

        private void Quit() {
            Debug.Log("Auto-Continue triggered quit.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

    }
}

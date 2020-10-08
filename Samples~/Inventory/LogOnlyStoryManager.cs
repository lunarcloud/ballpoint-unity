using System.Collections;
using UnityEngine;

namespace Ballpoint.Sample.Inventory {

    [RequireComponent(typeof(InkManager))]
    [DisallowMultipleComponent]
    [HelpURL(InkManager.HelpURL)]
    public class LogOnlyStoryManager : MonoBehaviour {
        private InkManager ink;

        public bool autoContinue = true;

        private void OnValidate() => ink = ink ?? GetComponent<InkManager>();

        void Start() {
            ink.storyUpdate.AddListener(StoryUpdate);
            ink.Initialize();
            ink.BeginStory();
        }

        private void StoryUpdate(StoryUpdate update) {
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

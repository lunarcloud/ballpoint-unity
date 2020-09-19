using UnityEngine;
using UnityEngine.Events;

namespace Ballpoint.SpeechBubble {

    [DisallowMultipleComponent]
    [HelpURL(InkManager.HelpURL)]
    public class Talkable : MonoBehaviour {
        public SpeechBubble speechBubble;

        public UnityEvent<bool> OnSpeakingChange = new UnityEvent<bool>();

        private void OnValidate() => speechBubble = speechBubble ?? GetComponentInChildren<SpeechBubble>();

        public void SetAsSpeaker(bool value) {
            if (!value) speechBubble.SetActive(false);
            OnSpeakingChange?.Invoke(value);
        }

        public void StoryUpdate(StoryUpdate u) => speechBubble?.StoryUpdate(u);
    }
}

using UnityEngine;

namespace Ballpoint.SpeechBubble {

    [DisallowMultipleComponent]
    [HelpURL(InkManager.HelpURL)]
    public class Talkable : MonoBehaviour {
        public SpeechBubble speechBubble;

        private void OnValidate() => speechBubble = speechBubble ?? GetComponentInChildren<SpeechBubble>();
    }
}

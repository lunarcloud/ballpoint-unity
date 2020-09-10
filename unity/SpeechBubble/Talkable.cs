using UnityEngine;

namespace InkWrapper.SpeechBubble {

    [DisallowMultipleComponent]
    [HelpURL("https://github.com/lunarcloud/InkWrapper")]
    public class Talkable : MonoBehaviour {
        public SpeechBubble speechBubble;

        private void OnValidate() => speechBubble = speechBubble ?? GetComponentInChildren<SpeechBubble>();
    }
}

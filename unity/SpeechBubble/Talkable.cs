using UnityEngine;

namespace InkPlusPlus.SpeechBubble
{
    public class Talkable : MonoBehaviour
    {
        public SpeechBubble speechBubble;

        private void OnValidate() => speechBubble = speechBubble ?? GetComponentInChildren<SpeechBubble>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ballpoint.SpeechBubble.Sample {
    public class Portrait : MonoBehaviour {
        public string speakerName = "None";

        public void ShowIfSpeaker(string value) => gameObject.SetActive(value == speakerName);
    }
}

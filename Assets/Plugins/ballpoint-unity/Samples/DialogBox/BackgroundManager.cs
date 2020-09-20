using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ballpoint.SpeechBubble.Sample {
    public class BackgroundManager : MonoBehaviour {
        public GameObject currentBackground;
        public void BackgroundChanged(string n) {
            if (string.IsNullOrEmpty(n)) return;

            currentBackground?.SetActive(false);
            currentBackground = transform.Find(n)?.gameObject;
            currentBackground?.SetActive(true);
        }
    }
}

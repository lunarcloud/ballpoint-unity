using UnityEngine;

namespace Ballpoint.SpeechBubble.Sample {
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour {
        public void MusicChanged(string value) {
            Debug.Log($"Music is now {value}");
        }
    }
}

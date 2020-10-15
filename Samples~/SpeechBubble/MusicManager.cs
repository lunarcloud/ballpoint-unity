using System.Linq;
using UnityEngine;

namespace Ballpoint.Sample.SpeechBubble {
    public class MusicManager : MonoBehaviour {
        
        public bool logSongChange = true;
        private AudioSource currentSong;

        private AudioSource[] audioSources;

        void Awake() => audioSources = GetComponentsInChildren<AudioSource>();

        public void MusicChanged(string value) {
            currentSong?.Stop();
            currentSong = audioSources?.SingleOrDefault(a => a.name == value);
            currentSong?.Play();
            if (logSongChange) Debug.Log($"Song is now {currentSong.clip.name}");
        }
    }
}

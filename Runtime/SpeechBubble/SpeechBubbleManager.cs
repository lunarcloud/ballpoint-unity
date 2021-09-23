using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Events;

namespace Ballpoint.SpeechBubble {

    [RequireComponent(typeof(InkEventDispatcher))]
    [DisallowMultipleComponent]
    [HelpURL(HelpURL)]
	[AddComponentMenu("Ballpoint/Speech Bubble/Speech Bubble Manager")]
    public class SpeechBubbleManager : MonoBehaviour {

        public const string HelpURL = InkManager.HelpURL + "#speech-bubble-manager";
        private InkManager ink;
        private InkEventDispatcher inkEvents;

        [SerializeField]
        [ContextMenuItem("Auto Detect All", "AutodetectTalkables")]
        public List<Talkable> talkables = new List<Talkable>();

        [SerializeField]
        [Tooltip("If left unset, will attempt to set to \"None\".")]
        public Talkable speaker;


        [SerializeField]
        [HideInInspector]
        private bool _InitiallyWiredEvents;

        private void OnValidate() {
            ink = ink ?? InkManager.FindAny();
            inkEvents = inkEvents ?? InkEventDispatcher.FindAny();
            
            if (_InitiallyWiredEvents == false) {
                _InitiallyWiredEvents = true;
                WireEvents();
            }
        }
        
		[ContextMenu("Re-Add Events to Dispatcher")] 
        private void WireEvents() { 
            if (inkEvents == null) Debug.LogError("InkEvents is null");

            // Hook up the events so they show up in the inspector
            UnityEventTools.AddPersistentListener(inkEvents.StoryUpdate, StoryUpdate);
            UnityEventTools.AddPersistentListener(inkEvents.StoryEnded, HideAllSpeechBubbles);

            var watcher = inkEvents.GetOrAddTagChangeWatcher(InkTagState.StartsWith, "said:"); 
            if (watcher == null) Debug.LogError("'Said' watcher is null");  
            UnityEventTools.AddPersistentListener(watcher.tagEvent, SetSpeaker);
        }

        void Start() {
            HideAllSpeechBubbles();
            ink.Initialize();
            SetSpeaker(speaker?.name ?? ink.story.variablesState["lastKnownSpeaker"] as string ?? "None");
            ink.BeginStory();
        }

        private void HideAllSpeechBubbles() => talkables?.ForEach(o => o?.SetAsSpeaker(false));

        private void SetSpeaker(string value) {
            //Debug.Log($"speaker is {value}");
            speaker?.SetAsSpeaker(false);
            speaker = talkables.Find(o => o?.name == value); // yes it can be set to null, aka hidden
            speaker?.SetAsSpeaker(true);

            ink.story.variablesState["lastKnownSpeaker"] = value;
        }

        private void StoryUpdate(Ballpoint.StoryUpdate update) => speaker?.StoryUpdate(update);

        [ContextMenu("Autodetect All Talkables")]
        private void AutodetectTalkables() => talkables = (FindObjectsOfType(typeof(Talkable)) as Talkable[])?.ToList<Talkable>();
    }
}

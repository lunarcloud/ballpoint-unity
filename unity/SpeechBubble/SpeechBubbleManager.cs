using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InkPlusPlus.SpeechBubble
{

    [DisallowMultipleComponent]
    [HelpURL("https://github.com/lunarcloud/InkWrapper")]
    public class SpeechBubbleManager : MonoBehaviour
    {

        [SerializeField]
        [ContextMenuItem("Auto Detect", "AutodetectInkManager")]
        public InkManager ink;

        [SerializeField]
        [ContextMenuItem("Auto Detect All", "AutodetectTalkables")]
        public List<Talkable> talkables = new List<Talkable>();

        [SerializeField]
        [Tooltip("If left unset, will attempt to set to \"None\".")]
        public Talkable speaker;

        void Start()
        {
            hideAllSpeechBubbles();
            ink.storyUpdate.AddListener(storyUpdate);
            ink.AddTagListener("speaker", setSpeaker);
            setSpeaker(speaker?.name ?? "None");
            ink.StartStory();
        }

        private void hideAllSpeechBubbles() => talkables.ForEach(o => o?.speechBubble.SetActive(false));

        private void setSpeaker(string value)
        {
            speaker?.speechBubble.SetActive(false);
            speaker = talkables.Find(o => o?.name == value); // yes it can be set to null, aka hidden
        }

        private void storyUpdate(InkPlusPlus.StoryUpdate update)
        {
            speaker?.speechBubble.SetText(update.text);
            speaker?.speechBubble.SetActive(true);
            speaker?.speechBubble.SetContinueButtonActive(update.choices.Count == 0);
        }

        [ContextMenu("Autodetect Ink Manager")]
        private void AutodetectInkManager() => ink = FindObjectOfType(typeof(InkManager)) as InkManager;

        [ContextMenu("Autodetect All Talkables")]
        private void AutodetectTalkables() => talkables = (FindObjectsOfType(typeof(Talkable)) as Talkable[])?.ToList<Talkable>();
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InkPlusPlus.SpeechBubble
{
    public class DialogTestSceneStoryManager : MonoBehaviour
    {

        [SerializeField]
        public InkManager ink;

        [ContextMenuItem("Auto Detect All", "AutodetectTalkables")]
        [SerializeField]
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

        private void setSpeaker(string value) {
            speaker?.speechBubble.SetActive(false);
            speaker = talkables.Find(o => o?.name == value); // yes it can be set to null, aka hidden
        }

        private void storyUpdate(InkPlusPlus.StoryUpdate update)
        {
            speaker?.speechBubble.SetText(update.text);
            speaker?.speechBubble.SetActive(true);
            speaker?.speechBubble.SetContinueButtonActive(update.choices.Count == 0);
        }

        [ContextMenu("Autodetect All Talkables")]
        private void AutodetectTalkables() => talkables = (FindObjectsOfType(typeof(Talkable)) as Talkable[])?.ToList<Talkable>();
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace InkWrapper.SpeechBubble {

    [DisallowMultipleComponent]
    [HelpURL("https://github.com/lunarcloud/InkWrapper")]
    public abstract class SpeechBubble : MonoBehaviour {

        [SerializeField]
        UnityEngine.UI.Text text;

        [SerializeField]
        UnityEngine.UI.Button choiceButton;

        [SerializeField]
        public UnityEngine.Events.UnityEvent ContinueClicked;

        [SerializeField]
        public UnityEngine.Events.UnityEvent<int> ChoiceChosen;

        private void OnValidate() {
            text = text ?? GetComponent("Body") as UnityEngine.UI.Text;
            choiceButton = choiceButton ?? GetComponent("ContinueBtn") as UnityEngine.UI.Button;
        }

        internal virtual void SetActive(bool v) => gameObject.SetActive(v);

        public void Continue() => ContinueClicked?.Invoke();

        public void Choose(int choiceIndex) => ChoiceChosen?.Invoke(choiceIndex);

        internal void SetText(string value) => text.text = value;

        // This will be different based on how you present choices
        internal abstract void SetChoices(List<string> choices);

        public void StoryUpdate(StoryUpdate update) {
            SetActive(false);
            SetText(update.text);
            SetChoices(update.choices);
            SetActive(true);
        }

    }
}

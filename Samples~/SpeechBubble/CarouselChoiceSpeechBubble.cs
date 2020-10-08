using System.Collections.Generic;
using UnityEngine;

namespace Ballpoint.Sample.SpeechBubble {

    [DisallowMultipleComponent]
    [HelpURL(InkManager.HelpURL)]
    public class CarouselChoiceSpeechBubble : Ballpoint.SpeechBubble.SpeechBubble {

        [SerializeField]
        GameObject previousIndicator = null;

        [SerializeField]
        GameObject nextIndicator = null;

        private List<string> choices = new List<string>();

        private int choiceSelected;

        private bool showingChoices = false;

        private bool holdingHorizontal = false; // debouncer
        private const double horizontalThreshold = 0.3;

        public void Update() {
            if (!isActiveAndEnabled) return;

            if (Input.GetButtonUp("Submit")) ContinueOrShowChoices();
            else {
                var hor = Input.GetAxis("Horizontal");
                if (!holdingHorizontal) {
                    if (hor > horizontalThreshold) NextChoice();
                    else if (hor < -horizontalThreshold) PreviousChoice();
                }
                holdingHorizontal = Mathf.Abs(hor) > horizontalThreshold;
            }
        }

        public void ContinueOrShowChoices() {
            if (choices.Count == 0) {
                Continue();
            } else if (showingChoices) {
                previousIndicator?.SetActive(false);
                nextIndicator?.SetActive(false);
                Choose(choiceSelected);
                showingChoices = false;
            } else {
                previousIndicator?.SetActive(true);
                nextIndicator?.SetActive(true);
                SetText(choices[0]);
                choiceSelected = 0;
                showingChoices = true;
            }
        }

        public void PreviousChoice() {
            if (!showingChoices || choices.Count == 0) return;
            choiceSelected--;
            choiceSelected = choiceSelected < 0 ? choices.Count - 1 : choiceSelected;
            SetText(choices[choiceSelected]);
        }

        public void NextChoice() {
            if (!showingChoices || choices.Count == 0) return;
            choiceSelected++;
            choiceSelected = choiceSelected >= choices.Count ? 0 : choiceSelected;
            SetText(choices[choiceSelected]);
        }

        public override void SetChoices(List<string> choices) {
            this.choices = choices;
            showingChoices = false;
        }
    }
}

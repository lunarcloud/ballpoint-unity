using UnityEngine;

namespace InkWrapper.SpeechBubble {

    [DisallowMultipleComponent]
    [HelpURL("https://github.com/lunarcloud/InkWrapper")]
    public class SpeechBubble : MonoBehaviour {

        [SerializeField]
        UnityEngine.UI.Text text;

        [SerializeField]
        UnityEngine.UI.Button choiceButton;

        [SerializeField]
        public UnityEngine.Events.UnityEvent ContinueClicked;

        private void OnValidate() {
            text = text ?? GetComponent("Body") as UnityEngine.UI.Text;
            choiceButton = choiceButton ?? GetComponent("ContinueBtn") as UnityEngine.UI.Button;
        }

        public void Continue() => ContinueClicked.Invoke();

        public void SetText(string value) => text.text = value;

        public void SetActive(bool value) => gameObject.SetActive(value);

        public void SetContinueButtonActive(bool v) => choiceButton.gameObject.SetActive(v);
    }
}

using UnityEngine;

namespace Ballpoint.Sample.Exploration {

    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    [HelpURL(InkManager.HelpURL)]
    public class TalkListenSpriteChange : MonoBehaviour {
        public Sprite talking;
        public Sprite listening;

        public void IsTalking(bool value) {
            if (value) UseTalking();
            else UseListening();
        }

        public void UseTalking() {
            var renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = talking;
        }
        public void UseListening() {
            var renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = listening;
        }
    }
}

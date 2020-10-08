using System.Collections.Generic;
using UnityEngine;

namespace Ballpoint.Sample.Exploration {
    public class Player : MonoBehaviour {

        public InkManager ink;
        public Vector2 speed = new Vector2(8f, 4f);
        public Inkeractable interactTarget;

        private Vector3 scale;
        private float x_scale;
        private List<string> choices = new List<string>();

        void Start() {
            if (ink?.lastStoryUpdate != null) StoryUpdate(ink.lastStoryUpdate);
            ink.storyUpdate.AddListener(StoryUpdate);
            scale = transform.localScale;
            x_scale = scale.x;
        }

        void Update() {
            float x = Input.GetAxis("Horizontal");
            if (x != 0) scale.x = x < 0 ? -x_scale : x_scale; // Flip the Character
            transform.localScale = scale;

            if (Input.GetButtonUp("Submit") && interactTarget != null) {
                interactTarget.Interact();
                var choiceIndex = choices.IndexOf(interactTarget.name);
                if (choiceIndex < 0) Debug.LogWarning($"No such choice at this time: {interactTarget.name}");
                else ink.Continue(choiceIndex);
            }
        }

        void FixedUpdate() {
            transform.Translate(
                Input.GetAxis("Horizontal") * speed.x * Time.deltaTime,
                Input.GetAxis("Vertical") * speed.y * Time.deltaTime,
                0f);
        }

        public void StoryUpdate(StoryUpdate update) => choices = update.choices;

    }
}

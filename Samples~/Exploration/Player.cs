using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Events;

namespace Ballpoint.Sample.Exploration {
    public class Player : MonoBehaviour {

        private InkManager ink;
        private InkEventDispatcher inkEvents;

        public Vector2 speed = new Vector2(8f, 4f);
        public Inkeractable interactTarget;

        private Vector3 scale;
        private float x_scale;
        private List<string> choices = new List<string>();
        
        [SerializeField]
        [HideInInspector]
        private bool _InitiallyWiredEvents = false;

        void OnValidate() {
            ink = ink ?? InkManager.FindAny();
            inkEvents = inkEvents ?? InkEventDispatcher.FindAny();
            
            // Hook up the events so they show up in the inspector
            if (_InitiallyWiredEvents == false) {
                _InitiallyWiredEvents = true;
                WireEvents();
            }
        }
        
        
		[ContextMenu("Re-Add Events to Dispatcher")] 
        private void WireEvents() {       
            UnityEventTools.AddPersistentListener(inkEvents.StoryUpdate, StoryUpdate);
        }


        void Start() {
            if (ink?.lastStoryUpdate != null) StoryUpdate(ink.lastStoryUpdate);
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

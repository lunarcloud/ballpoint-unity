using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ballpoint.Sample.Exploration {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class Inkeractable : MonoBehaviour {

        void OnTriggerEnter2D(Collider2D other) {
            var maybeInkeractor = other.GetComponent<Player>();
            if (maybeInkeractor != null) {
                maybeInkeractor.interactTarget = this;
                Debug.Log($"Nearby inkeractable: {name}");
            }
        }

        void OnTriggerExit2D(Collider2D other) {
            var maybeInkeractor = other.GetComponent<Player>();
            if (maybeInkeractor.interactTarget == this) {
                Debug.Log($"Leaving inkeractable: {name}");
                maybeInkeractor.interactTarget = null;
            }
        }

        public virtual void Interact() {
            Debug.Log($"Interacted with {name}"); // perform generic animations or whatnot
        }
    }
}

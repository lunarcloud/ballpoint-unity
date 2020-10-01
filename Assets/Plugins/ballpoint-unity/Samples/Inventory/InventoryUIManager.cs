using Ballpoint.Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace Ballpoint.Sample.Inventory {
    public class InventoryUIManager : MonoBehaviour {

        public GameObject SlotPrefab;

        public UnityEvent<InventoryItem, InventoryItem> Combine;

        void Start() {
            foreach (Transform item in transform) {
                item.GetComponent<InventorySlot>().Clear();
            }
        }

        public void OnInventoryUpdate(InventoryItem[] items) {
            // create missing slots
            for (var i = transform.childCount; i < items.Length; i++) {
                var created = Instantiate(SlotPrefab, transform);
                created.SetActive(true);
            }
            // hide extra slots
            for (var i = items.Length; i < transform.childCount; i++) {
                var slot = transform.GetChild(i);
                slot.GetComponent<InventorySlot>().Clear();
            }
            // update data
            for (var i = 0; i < items.Length; i++) {
                var slot = transform.GetChild(i);
                slot.GetComponent<InventorySlot>().Set(items[i]);
            }
        }

        public void CombineSelected() {

            InventorySlot slotA = null;
            InventorySlot slotB = null;
            foreach (Transform child in transform) {
                if (slotA != null && slotB != null) break;
                var slot = child.GetComponent<InventorySlot>();
                if (!slot.selected) continue;

                if (slotA == null) slotA = slot;
                else slotB = slot;
            }
            if (slotA != null && slotB != null)
                Combine?.Invoke(slotA.item, slotB.item);
        }
    }
}

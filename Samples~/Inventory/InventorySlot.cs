using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Ballpoint.Inventory;

namespace Ballpoint.Sample.Inventory {
    public class InventorySlot : MonoBehaviour {
        [Header("Button State Images")]
        public Sprite DeselectedButton;
        public Sprite SelectedButton;

        [Header("Slot Sub-Components")]
        public Image ButtonImage;
        public Image Icon;

        public InventoryItem item { get; private set; }

        private bool _selected = false;
        public bool selected {
            get => _selected;
            set { SetSelected(value); }
        }

        [Header("Events")]
        public UnityEvent<bool> OnSelectChange;

        public void Set(InventoryItem newItem) {
            item = newItem;
            Icon.sprite = item.icon;
            gameObject.SetActive(true);
            selected = false; // trigger event, change (hidden) image
        }

        public void Clear() {
            item = null;
            Icon.sprite = null;
            _selected = false; // do not trigger event, do not change (hidden) image
            gameObject.SetActive(false);
        }

        public void ToggleSelect() {
            SetSelected(!_selected);
        }

        public void SetSelected(bool value) {
            var change = _selected == value;
            _selected = value;
            ButtonImage.sprite = value ? SelectedButton : DeselectedButton;
            if (change) OnSelectChange?.Invoke(value);
        }
    }
}

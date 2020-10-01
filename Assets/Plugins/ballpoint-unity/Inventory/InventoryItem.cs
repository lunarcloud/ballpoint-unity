using UnityEngine;

namespace Ballpoint.Inventory {
    [CreateAssetMenu(fileName = "New Item", menuName = "Ballpoint/Inventory Item")]
    public class InventoryItem : ScriptableObject {
        public string listDefinitionName = "inventory";
        public new string name;
        public string fullName { get => $"{listDefinitionName}.{name}"; }
        public Sprite icon;
        public Ink.Runtime.InkList value = new Ink.Runtime.InkList();
    }
}

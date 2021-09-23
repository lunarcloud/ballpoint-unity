using System;
using System.Linq;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor.Events;

namespace Ballpoint.Inventory {

    [RequireComponent(typeof(InkEventDispatcher))]
    [DisallowMultipleComponent]
    [HelpURL(HelpURL)]
	[AddComponentMenu("Ballpoint/Inventory Manager")]
    public class InventoryManager : MonoBehaviour {

        public const string HelpURL = InkManager.HelpURL + "#inventory-manager";

        private InkManager ink;
        private InkEventDispatcher inkEvents;
        
        public string inkListName = "inventory";

        public string inkCombineFunctionName = "combine";

        public string inkPickupFunctionName = "pickup";

        public InventoryItem[] items;

        public InkList list;

        public bool logInventoryChanges = false;

        public UnityEvent<InventoryItem[]> InventoryChanged;

        private bool hasCombineFunction = false;
        private bool hasPickupFunction = false;
        private bool inventoryItemsSetup = false;
        
        [SerializeField]
        [HideInInspector]
        private bool _InitiallyWiredEvents = false;

        private void OnValidate() {
            ink = ink ?? InkManager.FindAny();
            inkEvents = inkEvents ?? InkEventDispatcher.FindAny();
            
            if (_InitiallyWiredEvents == false) {
                _InitiallyWiredEvents = true;
                WireEvents();
            }
        }
        
        
		[ContextMenu("Re-Add Events to Dispatcher")] 
        private void WireEvents() { 
            if (inkEvents == null) Debug.LogError("InkEvents is null");

            // Hook up the events so they show up in the inspector            
            var watcher = inkEvents.GetOrAddInkVariableWatcher(inkListName, HandleTypeEnum.List);

            if (watcher == null) Debug.LogError($"'{inkListName}' watcher is null");
            
            UnityEventTools.AddPersistentListener(watcher.changedAsList, OnInventoryChanged);
        }

        public void OnInventoryChanged(Ink.Runtime.InkList newList) {
            SetupInventoryItems(newList.all);
            list = newList;
            if (logInventoryChanges) Debug.Log($"Inventory update: {newList}");
            InventoryChanged?.Invoke(items.Where(i => newList.Contains(i.value)).ToArray());
        }

        private void SetupInventoryItems(InkList all)
        {
            if (inventoryItemsSetup) return;
            foreach (var item in items) {
                item.value.Clear();
                var found = all.Where(i => i.Key.fullName == item.fullName);
                if (found.Count() != 1) {
                    throw new Exception($"Cannot resolve {item.fullName} to a single item within {all}");
                }
                var listItem = found.Single();
                item.value.Add(listItem.Key, listItem.Value);
            }
            inventoryItemsSetup = true;
        }

        public void Combine(InventoryItem a, InventoryItem b) {
            hasCombineFunction = hasCombineFunction || ink.story.HasFunction(inkCombineFunctionName);
            if (!hasCombineFunction) {
                throw new System.Exception($"Story doesn't include inventory combining function ({inkCombineFunctionName})!");
            }
            if (!list.ContainsItemNamed(a.name) && !list.ContainsItemNamed(b.name)) {
                Debug.LogError($"Inventory doesn't currently include both of those items: {a}, {b}");
                return;
            }
            var result = ink.story.EvaluateFunction(inkCombineFunctionName, a.value.Union(b.value));
        }
        
        public void Pickup(InventoryItem a) {
            hasPickupFunction = hasPickupFunction || ink.story.HasFunction(inkPickupFunctionName);
            if (!hasPickupFunction) {
                throw new System.Exception($"Story doesn't include inventory pickup function ({inkPickupFunctionName})!");
            }
            if (list.ContainsItemNamed(a.name)) {
                Debug.LogError($"Inventory already includes this item: {a}");
                return;
            }
            if (a.value.Count == 0) {
                Debug.LogError($"WTF! {a} isn't setup!");
                return;
            }
            var result = ink.story.EvaluateFunction(inkPickupFunctionName, a.value);
        }
    }
}

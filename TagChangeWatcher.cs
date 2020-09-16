using UnityEngine.Events;
using Ink.Runtime;

namespace InkWrapper {
    [System.Serializable]
    public class TagChangeWatcher {
        public string name;
        public UnityEvent<string> changed = new UnityEvent<string>();

        public TagChangeWatcher(string name) => this.name = name;

        public void Invoke(string value) => changed?.Invoke(value);
    }
}

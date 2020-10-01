using Ink.Runtime;
using UnityEngine.Events;

namespace Ballpoint {
    [System.Serializable]
    public class TagChangeWatcher {
        public string name;
        public UnityEvent<string> changed = new UnityEvent<string>();

        public TagChangeWatcher(string name) => this.name = name;

        public void Invoke(string value) => changed?.Invoke(value);
    }
}

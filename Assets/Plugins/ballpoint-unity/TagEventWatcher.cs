using Ink.Runtime;
using UnityEngine.Events;

namespace Ballpoint {
    [System.Serializable]
    public class TagEventWatcher {
        public string name;
        public UnityEvent<string> tagEvent = new UnityEvent<string>();

        public TagEventWatcher(string name) => this.name = name;

        public void Invoke(string value) => tagEvent?.Invoke(value);
    }
}

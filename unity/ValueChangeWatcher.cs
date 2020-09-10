namespace InkWrapper {
    [System.Serializable]
    public class ValueChangeWatcher<T> {
        public string name;

        public UnityEngine.Events.UnityEvent<T> changed = new UnityEngine.Events.UnityEvent<T>();

        public ValueChangeWatcher(string name) => this.name = name;

        public void Invoke(T value) => changed?.Invoke(value);
    }
}

namespace InkPlusPlus
{
    [System.Serializable]
    public class InkTagProcessor
    {
        public string key;

        public UnityEngine.Events.UnityEvent<string> handler = new UnityEngine.Events.UnityEvent<string>();

        public InkTagProcessor(string keyToHandle) => key = keyToHandle;

        public void Invoke(string value) => handler?.Invoke(value);
    }
}
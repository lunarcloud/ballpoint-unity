using System;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace InkWrapper {

    [System.Flags]
    public enum HandleTypeEnum {
        String = 1,
        Integer = 2,
        Float = 4,
        List = 8,
        Object = 16
    }

    [System.Serializable]
    public class InkVariableWatcher {

        public string name;

        [SerializeField]
        public HandleTypeEnum handleAsType;

        [ShowIf(nameof(handleAsType), HandleTypeEnum.Integer)]
        public UnityEvent<int> changedAsInt = new UnityEvent<int>();

        [ShowIf(nameof(handleAsType), HandleTypeEnum.Float)]
        public UnityEvent<float> changedAsFloat = new UnityEvent<float>();

        [ShowIf(nameof(handleAsType), HandleTypeEnum.String)]
        public UnityEvent<string> changedAsString = new UnityEvent<string>();

        [Tooltip("Will pass in a null if this isn't a list value")]

        [ShowIf(nameof(handleAsType), HandleTypeEnum.List)]
        public UnityEvent<InkList> changedAsList = new UnityEvent<InkList>();

        [ShowIf(nameof(handleAsType), HandleTypeEnum.Object)]
        public UnityEvent<object> changedAsObject = new UnityEvent<object>();

        public InkVariableWatcher(string name) {
            this.name = name;
        }

        public void Invoke(object value) {
            if (handleAsType.HasFlag(HandleTypeEnum.Object)) changedAsObject?.Invoke(value);
            if (handleAsType.HasFlag(HandleTypeEnum.String)) changedAsString?.Invoke(Convert.ToString(value));
            if (handleAsType.HasFlag(HandleTypeEnum.List)) changedAsList?.Invoke(value as InkList);
            if (handleAsType.HasFlag(HandleTypeEnum.Integer)) changedAsInt?.Invoke(Convert.ToInt32(value));
            if (handleAsType.HasFlag(HandleTypeEnum.Float)) changedAsFloat?.Invoke(Convert.ToSingle(value));
        }
    }
}

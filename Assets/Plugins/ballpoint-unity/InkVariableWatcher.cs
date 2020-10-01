using System;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace Ballpoint {

    [System.Flags]
    public enum HandleTypeEnum {
        String = 1,
        Integer = 2,
        Float = 4,
        Bool = 8,
        List = 16,
        Path = 32,
        Object = 64
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

        [ShowIf(nameof(handleAsType), HandleTypeEnum.Bool)]
        public UnityEvent<bool> changedAsBool = new UnityEvent<bool>();

        [ShowIf(nameof(handleAsType), HandleTypeEnum.String)]
        public UnityEvent<string> changedAsString = new UnityEvent<string>();

        [Tooltip("Will pass in a null if this isn't a list value")]

        [ShowIf(nameof(handleAsType), HandleTypeEnum.List)]
        public UnityEvent<InkList> changedAsList = new UnityEvent<InkList>();

        [Tooltip("Will pass in a null if this isn't a path (divert target) value")]

        [ShowIf(nameof(handleAsType), HandleTypeEnum.Path)]
        public UnityEvent<Path> changedAsPath = new UnityEvent<Path>();

        [ShowIf(nameof(handleAsType), HandleTypeEnum.Object)]
        public UnityEvent<object> changedAsObject = new UnityEvent<object>();

        public InkVariableWatcher(string name, HandleTypeEnum types) {
            this.name = name;
            this.handleAsType |= types;
        }

        public void Invoke(object value) {
            if (handleAsType.HasFlag(HandleTypeEnum.Object)) changedAsObject?.Invoke(value);
            if (handleAsType.HasFlag(HandleTypeEnum.List)) changedAsList?.Invoke(value as InkList);
            if (handleAsType.HasFlag(HandleTypeEnum.Path)) changedAsPath?.Invoke(value as Path);
            if (handleAsType.HasFlag(HandleTypeEnum.String)) changedAsString?.Invoke(Convert.ToString(value));
            if (handleAsType.HasFlag(HandleTypeEnum.Integer)) changedAsInt?.Invoke(Convert.ToInt32(value));
            if (handleAsType.HasFlag(HandleTypeEnum.Float)) changedAsFloat?.Invoke(Convert.ToSingle(value));
            if (handleAsType.HasFlag(HandleTypeEnum.Bool)) changedAsBool?.Invoke(Convert.ToBoolean(value));
        }
    }
}

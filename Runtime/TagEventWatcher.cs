using System.Text.RegularExpressions;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace Ballpoint {
    /// <summary>
    /// Choices for ink boolean states
    /// </summary>
    public enum InkTagState
    {
        [Tooltip("When the tag fires with a value equals to 'pattern'.")]
        Equals,

        [Tooltip("When the tag fires with a value starting with 'pattern'.")]
        StartsWith,

        [Tooltip("When the tag fires with a value matching the regex specified in 'pattern'.")]
        MatchesRegex,
    }

    [System.Serializable]
    public class TagEventWatcher {

        [Tooltip("State to trigger actions")]
        public InkTagState state;

        [Tooltip("Optional value used for comparison")]
        public string pattern;

        public UnityEvent<string> tagEvent = new UnityEvent<string>();

        public TagEventWatcher(InkTagState state, string pattern)  {
            this.state = state;
            this.pattern = pattern;
        }

        public bool IsMatch(string value) {
            return (state == InkTagState.Equals && value.Equals(pattern))
                || (state == InkTagState.StartsWith && value.StartsWith(pattern))
                || (state == InkTagState.MatchesRegex && Regex.IsMatch(value, pattern));
        }

        public void Invoke(string value) => tagEvent?.Invoke(value);
        
        public void InvokeIfMatch(string value) { 
            if (IsMatch(value)) tagEvent?.Invoke(value);
        }
    }
}

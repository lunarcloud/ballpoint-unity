using System.Collections.Generic;

namespace Ballpoint {
    [System.Serializable]
    public class StoryUpdate {
        public string text;
        public List<string> choices;
        public Dictionary<string, string> tags;
        public bool atEnd = false;

        public StoryUpdate(string text, List<string> choices, Dictionary<string, string> tags, bool end) {
            this.text = text;
            this.choices = choices;
            this.tags = tags;
            atEnd = end;
        }

        public override string ToString() {
            var endString = atEnd ? ", at end" : "";
            return $@"Story Update:
            {choices.Count} choice(s), {tags.Keys.Count} tag(s){endString}, text = {text}";
        }
    }
}

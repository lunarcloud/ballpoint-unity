using System.Collections.Generic;

namespace InkPlusPlus
{
    [System.Serializable]
    public class StoryUpdate
    {
        public string text;

        public List<string> choices;
        public Dictionary<string, string> tags;

        public bool atEnd = false;

        public StoryUpdate(string tx, List<string> ch, Dictionary<string, string> tg, bool end)
        {
            text = tx;
            choices = ch;
            tags = tg;
            atEnd = end;
        }
    }
}
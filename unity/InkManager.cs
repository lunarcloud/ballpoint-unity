using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ink.Runtime;
using UnityEngine;

namespace InkPlusPlus
{
	public class InkManager : MonoBehaviour
	{

		[SerializeField]
		public TextAsset InkJsonAsset;

		private string savePath;

		[SerializeField]
		[Tooltip("JSON to load as initial state, for debugging")]
		public TextAsset startState;

		public Story story
		{
			get;
			private set;
		}

		[Header("Tags Handling")]
		[SerializeField]
		public char tagValueSplitter = ':';

		[SerializeField]
		public List<InkTagProcessor> tagProcessors = new List<InkTagProcessor>();

		[Header("Story Events")]
		[SerializeField]
		public UnityEngine.Events.UnityEvent<StoryUpdate> storyUpdate;

		[SerializeField]
		public UnityEngine.Events.UnityEvent<bool> choiceRequiredToContinue;

		[SerializeField]
		public UnityEngine.Events.UnityEvent atStoryEnd;

		public string State
		{
			get => this.story.state.ToJson();
			set => this.story.state.LoadJson(value);
		}

		private void Awake()
		{
			savePath = $"{Application.persistentDataPath}/save.json";
		}

		public void Save(string path = null) => File.WriteAllText(path ?? savePath, State);

		public void Load(string path = null) => State = File.ReadAllText(path ?? savePath);

		public void LoadOrCreate(string path = null)
		{
			if (File.Exists(path)) Load(path);
			else Save(path);
		}

		public void StartStory()
		{
			story = new Story(InkJsonAsset.text);

			if (startState == null) LoadOrCreate();
			else State = startState.text;

			Continue();
		}

		public void Continue()
		{
			var text = story.Continue().Trim();
			var tags = ProcessTags(story.currentTags);
			var choices = story.currentChoices.Select(c => c.text).ToList<string>();
			storyUpdate?.Invoke(new StoryUpdate(text, choices, tags, isAtEnd()));
			choiceRequiredToContinue?.Invoke(story.canContinue);
			if (isAtEnd()) atStoryEnd?.Invoke();
		}

		public void Continue(int choiceIndex)
		{
			story.ChooseChoiceIndex(choiceIndex);
			Continue();
		}

		public bool isAtEnd() => this.story.currentChoices.Count == 0 && !this.story.canContinue;

		private InkTagProcessor GetOrAddProcessor(string key)
		{
			tagProcessors = tagProcessors ?? new List<InkTagProcessor>();
			var processor = tagProcessors.Find(o => o.key == key);
			if (processor == null)
			{
				processor = new InkTagProcessor(key);
				tagProcessors.Add(processor);
			}
			return processor;
		}

		public void AddTagListener(string key, UnityEngine.Events.UnityAction<string> call) => GetOrAddProcessor(key).handler.AddListener(call);

		public void RemoveTagListener(string key, UnityEngine.Events.UnityAction<string> call) => GetOrAddProcessor(key).handler.RemoveListener(call);

		public Dictionary<string, string> ProcessTags(List<string> tags)
		{
			var tagMap = new Dictionary<string, string>();
			if (tags != null)
				foreach (string tag in tags)
				{
					var key = tag.Split(tagValueSplitter) [0];
					var value = tag.Split(tagValueSplitter) [1];
					tagMap.Add(key, value);
					// Trigger Event
					var inkTagProcessor = tagProcessors?.Find(o => o.key == key);
					inkTagProcessor?.Invoke(value);
				}
			return tagMap;
		}
	}
}

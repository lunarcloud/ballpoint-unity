using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace InkWrapper {
	[DisallowMultipleComponent]
	[HelpURL("https://github.com/lunarcloud/InkWrapper")]
	public class InkManager : MonoBehaviour {

		[SerializeField]
		public TextAsset InkJsonAsset;
		public Story story {
			get;
			private set;
		}

		[HideInInspector]
		public string standardSavePath {
			get;
			private set;
		}

		[Header("Start-Up")]

		[SerializeField]
		[Tooltip("Save File to load at start, for debugging.")]
		[ContextMenuItem("Reset to this state", "LoadStartState")]
		public TextAsset debugState;

		[SerializeField]
		[Tooltip("Unless debug state is set.")]
		public bool loadSaveFileOnStart = false;

		public bool skipInitialBlankLine = true;

		[Header("Processing")]

		[Tooltip("Not including empty text with choices")]
		public bool skipBlankLines = true;

		[SerializeField]
		public UnityEvent<StoryUpdate> storyUpdate;

		[SerializeField]
		public UnityEvent storyEnded;

		[Header("Tags Handling")]
		[SerializeField]
		public char tagValueSplitter = ':';

		[SerializeField]
		public List<TagChangeWatcher> tagEvents = new List<TagChangeWatcher>();

		[Header("Variable Observation")]

		[SerializeField]
		public List<InkVariableWatcher> variableChangedEvents = new List<InkVariableWatcher>();

		public string State {
			get => this.story.state.ToJson();
			set => this.story.state.LoadJson(value);
		}

		private void Awake() {
			// Application.persistentDataPath is not available at compile-time, must assign this here
			standardSavePath = $"{Application.persistentDataPath}/save.json";
		}

		private string pathOrStandardSavePath(string path) => string.IsNullOrEmpty(path) ? standardSavePath : path;

		public void Save(string path = null) => File.WriteAllText(pathOrStandardSavePath(path), State);

		public void Load(string path = null) => State = File.ReadAllText(pathOrStandardSavePath(path));

		public bool TryLoad(string path = null) {
			if (!File.Exists(pathOrStandardSavePath(path))) return false;

			Load(path);
			return true;
		}

		public void Initialize() {
			story = new Story(InkJsonAsset.text);
			variableChangedEvents.ForEach(watcher => story.ObserveVariable(watcher.name, (k, v) => watcher.Invoke(v)));

			// Load a state or standard save (if configured to)
			if (debugState != null) LoadDebugState();
			else if (loadSaveFileOnStart) TryLoad();
		}

		public void LoadDebugState() => State = debugState.text;

		public void BeginStory() {
			// Send out Variable initial values
			variableChangedEvents.ForEach(watcher => watcher.Invoke(story.variablesState[watcher.name]));
			// Skip the first line if blank (if configured to)
			if (skipInitialBlankLine && IsBlankLineWithoutChoices()) story.Continue();
			if (IsAtEnd()) storyEnded?.Invoke();
			// Initial Story Update
			SendStoryUpdate();
		}

		public void Continue() {
			if (IsAtEnd()) {
				storyEnded?.Invoke();
				return;
			}
			// Continue at least once
			if (story.canContinue) story.Continue();
			while (skipBlankLines && IsBlankLineWithoutChoices() && !IsAtEnd()) {
				story.Continue();
			}
			SendStoryUpdate();
		}
		private void SendStoryUpdate() {
			var text = story.currentText.Trim();
			var tags = ProcessTags(story.currentTags);
			var choices = story.currentChoices.Select(c => c.text).ToList<string>();
			storyUpdate?.Invoke(new StoryUpdate(text, choices, tags, IsAtEnd()));
		}

		public void Continue(int choiceIndex) {
			story.ChooseChoiceIndex(choiceIndex);
			Continue();
		}

		private bool IsBlankLineWithoutChoices() => story.currentText.Trim() == string.Empty && story.currentChoices.Count == 0;

		public bool IsAtEnd() => this.story.currentChoices.Count == 0 && !this.story.canContinue;

		public Dictionary<string, string> ProcessTags(List<string> tags) {
			var tagMap = new Dictionary<string, string>();
			if (tags != null)
				foreach (string tag in tags) {
					var split = tag.Split(tagValueSplitter);
					var key = split.Length > 0 ? split[0] : tag;
					var value = split.Length > 1 ? split[1] : key;
					tagMap.Add(key, value);
					// Trigger Event
					var inkTagProcessor = tagEvents?.Find(o => o.name == key);
					inkTagProcessor?.Invoke(value);
				}
			return tagMap;
		}

		// Tag Event functions
		internal TagChangeWatcher GetOrAddTagChangeWatcher(string name) {
			tagEvents = tagEvents ?? new List<TagChangeWatcher>();
			var watcher = tagEvents.Find(o => o.name == name);
			if (watcher == null) {
				watcher = new TagChangeWatcher(name);
				tagEvents.Add(watcher);
			}
			return watcher;
		}

		public void AddTagListener(string key, UnityAction<string> call) => GetOrAddTagChangeWatcher(key).changed.AddListener(call);

		public void RemoveTagListener(string key, UnityAction<string> call) => GetOrAddTagChangeWatcher(key).changed.RemoveListener(call);

		// Variable Event functions
		public InkVariableWatcher GetOrAddInkVariableWatcher(string name) {
			variableChangedEvents = variableChangedEvents ?? new List<InkVariableWatcher>();
			var watcher = variableChangedEvents.Find(o => o.name == name);
			if (watcher == null) {
				watcher = new InkVariableWatcher(name);
				variableChangedEvents.Add(watcher);
				// Setup actual watcher with ink
				story.ObserveVariable(name, (k, v) => watcher.Invoke(v));
				watcher.Invoke(story.variablesState[name]);
			}
			return watcher;
		}
	}
}

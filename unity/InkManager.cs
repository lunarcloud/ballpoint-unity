using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ink.Runtime;
using UnityEngine;

namespace InkWrapper {
	[DisallowMultipleComponent]
	[HelpURL("https://github.com/lunarcloud/InkWrapper")]
	public class InkManager : MonoBehaviour {

		[SerializeField]
		public TextAsset InkJsonAsset;

		[HideInInspector]
		public string standardSavePath {
			get;
			private set;
		}

		[SerializeField]
		[Tooltip("Save File to load at start, for debugging.")]
		[ContextMenuItem("Reset to this state", "LoadStartState")]
		public TextAsset debugState;

		[SerializeField]
		[Tooltip("Unless debug state is set.")]
		public bool loadSaveFileOnStart = false;

		[Header("Blank Lines")]

		public bool skipInitialBlankLine = true;

		[Tooltip("Not including empty text with choices")]
		public bool skipBlankLines = true;

		public Story story {
			get;
			private set;
		}

		[Header("Tags Handling")]
		[SerializeField]
		public char tagValueSplitter = ':';

		[SerializeField]
		public List<ValueChangeWatcher<string>> tagEvents = new List<ValueChangeWatcher<string>>();

		[Header("Variable Observers")]
		[SerializeField]
		public List<ValueChangeWatcher<object>> variableChangedEvents = new List<ValueChangeWatcher<object>>();

		[Header("Story Events")]
		[SerializeField]
		public UnityEngine.Events.UnityEvent<StoryUpdate> storyUpdate;

		[SerializeField]
		public UnityEngine.Events.UnityEvent atStoryEnd;

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
			variableChangedEvents.ForEach(watcher => story.ObserveVariable(watcher.name, (_name, newValue) => watcher.Invoke(newValue)));

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
			if (IsAtEnd()) atStoryEnd?.Invoke();
			// Initial Story Update
			SendStoryUpdate();
		}

		public void Continue() {
			// Continue at least once
			do {
				story.Continue();
			} while (skipBlankLines && IsBlankLineWithoutChoices() && !IsAtEnd());

			if (IsAtEnd()) atStoryEnd?.Invoke();
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
					var key = tag.Split(tagValueSplitter) [0];
					var value = tag.Split(tagValueSplitter) [1];
					tagMap.Add(key, value);
					// Trigger Event
					var inkTagProcessor = tagEvents?.Find(o => o.name == key);
					inkTagProcessor?.Invoke(value);
				}
			return tagMap;
		}

		// Tag Event functions
		internal ValueChangeWatcher<string> GetOrAddTagEvent(string name) {
			tagEvents = tagEvents ?? new List<ValueChangeWatcher<string>>();
			var watcher = tagEvents.Find(o => o.name == name);
			if (watcher == null) {
				watcher = new ValueChangeWatcher<string>(name);
				tagEvents.Add(watcher);
			}
			return watcher;
		}

		public void AddTagListener(string key, UnityEngine.Events.UnityAction<string> call) => GetOrAddTagEvent(key).changed.AddListener(call);

		public void RemoveTagListener(string key, UnityEngine.Events.UnityAction<string> call) => GetOrAddTagEvent(key).changed.RemoveListener(call);

		// Variable Event functions
		internal ValueChangeWatcher<object> GetOrAddVariableEvent(string name) {
			variableChangedEvents = variableChangedEvents ?? new List<ValueChangeWatcher<object>>();
			var watcher = variableChangedEvents.Find(o => o.name == name);
			if (watcher == null) {
				watcher = new ValueChangeWatcher<object>(name);
				variableChangedEvents.Add(watcher);
				// Setup actual watcher with ink
				story.ObserveVariable(name, (_name, newValue) => watcher.Invoke(newValue));
				watcher.Invoke(story.variablesState[name]); // send initial value
			}
			return watcher;
		}

		public void AddVariableListener(string key, UnityEngine.Events.UnityAction<object> call) => GetOrAddVariableEvent(key).changed.AddListener(call);

		public void RemoveVariableListener(string key, UnityEngine.Events.UnityAction<object> call) => GetOrAddVariableEvent(key).changed.RemoveListener(call);

	}
}

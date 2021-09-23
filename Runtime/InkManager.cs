using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace Ballpoint {

	[DisallowMultipleComponent]
	[HelpURL(HelpURL + "#ink-manager")]
	[AddComponentMenu("Ballpoint/Ink Manager")]
	public class InkManager : MonoBehaviour {

		public const string HelpURL = "https://github.com/lunarcloud/ballpoint-unity/tree/master/Documentation~/Ballpoint.md";

		private InkEventDispatcher eventDispatcher;

		[SerializeField]
		public TextAsset InkJsonAsset;
		public Story story { get; private set; }

		[HideInInspector]
		public string standardSavePath { get; private set; }

		[Header("Start-Up")]

		[SerializeField]
		[Tooltip("Save File to load at start, for debugging.")]
		[ContextMenuItem("Reset to this state", "LoadStartState")]
		public TextAsset debugState;

		[SerializeField]
		[Tooltip("Unless debug state is set.")]
		public bool loadSaveFileOnStart = false;

		[Tooltip("Process tags and auto-continue if initial line has no text.")]
		public bool AutoSkipInitialBlankLine = true;

		[Tooltip("Process tags and auto-continue for empty text without choices.")]
		public bool AutoSkipBlankLines = true;
		protected bool _AutoSkipBlankLines; // actually used
		
		public StoryUpdate lastStoryUpdate { get; private set; }

		public string State {
			get => this.story.state.ToJson();
			set => this.story.state.LoadJson(value);
		}
		
		public static InkManager FindAny()
		{
				var maybeInkManager = FindObjectsOfType<InkManager>();
				if (maybeInkManager.Length > 1) {
					Debug.LogWarning("Multiple Ink Managers found in scene, using first found");
				} else if (maybeInkManager.Length < 1) {
					Debug.LogError("Ink Manager could not be found in the scene!");
					return null;
				}
				
				return maybeInkManager[0];
		}

		private void Awake() {
			eventDispatcher = eventDispatcher ?? InkEventDispatcher.FindAny();

			// Application.persistentDataPath is not available at compile-time, must assign this here
			standardSavePath = $"{Application.persistentDataPath}/save.json";
		}

		private string pathOrStandardSavePath(string path) => string.IsNullOrEmpty(path) ? standardSavePath : path;

		public void Save(string path = null) => File.WriteAllText(pathOrStandardSavePath(path), State);

		private void Load(string path = null) => State = File.ReadAllText(pathOrStandardSavePath(path));

		public bool TryLoad(string path = null) {
			if (!File.Exists(pathOrStandardSavePath(path))) return false;

			Load(path);
			return true;
		}

		public void Initialize() {
			story = new Story(InkJsonAsset.text);
			eventDispatcher?.InitializeVariableObservation();
			
			// Load a state or standard save (if configured to)
			if (debugState != null) LoadDebugState();
			else if (loadSaveFileOnStart) TryLoad();

			eventDispatcher?.StoryReady?.Invoke();
		}

		public void LoadDebugState() => State = debugState.text;

		public void BeginStory() {
			// Send out Variable initial values
			eventDispatcher?.InvokeInitialVariableObservationValues();
			
			// AutoSkip the first line if blank (if configured to)
			_AutoSkipBlankLines = AutoSkipInitialBlankLine;
			Continue();
			_AutoSkipBlankLines = AutoSkipBlankLines;
		}

		public void Continue() {
			if (IsAtEnd()) {
				eventDispatcher?.StoryEnded?.Invoke();
				return;
			}
			if (!story.canContinue) return;
			// Continue at least once
			do {
				story.Continue();
				SendStoryUpdate();
			} while (_AutoSkipBlankLines && IsBlankLineWithoutChoices() && !IsAtEnd());
		}
		private void SendStoryUpdate() {
			var text = story.currentText.Trim();
			var choices = story.currentChoices.Select(c => c.text).ToList<string>();
			eventDispatcher?.ProcessTags(story.currentTags); // Process Tags first, then send story update
			lastStoryUpdate = new StoryUpdate(text, choices, story.currentTags, IsAtEnd());
			eventDispatcher?.StoryUpdate?.Invoke(lastStoryUpdate);
		}

		public void Continue(int choiceIndex) {
			story.ChooseChoiceIndex(choiceIndex);
			Continue();
		}

		private bool IsBlankLineWithoutChoices() => story.currentText.Trim() == string.Empty && story.currentChoices.Count == 0;

		public bool IsAtEnd() => this.story.currentChoices.Count == 0 && !this.story.canContinue;
	}

}

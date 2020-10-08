How to Use Ballpoint 
==================

One should be familiar with the process of writing with Ink before using this plug-in (in combination with the Ink Unity Integration plug-in) to create a game from an Ink story, see https://github.com/inkle/ink/blob/master/Documentation/WritingWithInk.md .

Once you've created your project and scene, create an empty object to be used as the Story Manager. To it, add either:

* DialogBoxManager
* SpeechBubbleManager
* InkManager and your own custom script (such as Samples\Exploration\LogOnlyStoryManager.cs)

The sample stories may contain a 'music' and 'scene' variable, but they are not required. Your story should, for dialog and speech bubble managers, at contain:

* `VAR lastKnownSpeaker= "?"`
* uses of `#said:?` to change the speaker
* At least one line of dialog

Ink Manager
---------------------------------

Your Ink story should be automatically compiled by the Ink Unity Integration plug-in. Wire the compiled json as the "Ink Json Asset". 

### Saving and Loading

There is an option to attempt to load the save file when the Ink Manager is Initialized. Loading can also be done by calling 'TryLoad'. Saving can be done by calling 'Save'. Both APIs take optional path arguments, to specify the save file location, but if none is provided, it will use "save.json" within `Application.persistentDataPath`.

### Tags 

Tags in Ink are text strings that start with a '#' and end when there is whitespace. They are attached to a line of dialog and should not typically contain anything that needs to be in a save file. Ballpoint adds an additional expectation, that many tags will be in the form of key-value pairs such as `#said:Will` or `#expression:confused`.

You can change the key-value splitter character, which is a colon ':' by default.

With the Ink Manager you can set up "Tag Events", which are Unity Events that are called when the tag appears in the current story moment, and provide the handler with the (string) value of the tag.

### Variables

Variables ("Global Variables" in Ink) are more robust, permanent affairs compared with tags. They store data of many different types, and are restored when a save file is loaded.

With the Ink Manager you can set up "Variable Changed Events", Unity Events that call when the variable is changed. You must tell the Ink Manager what type(s) to handle the variable as.

Dialog Box Manager
---------------------------------

Please see Samples\DialogBox\DialogBoxDemo.unity for example.

Create a central UI for all dialog. It should contain and be wired up to the Dialog Box Manager:

* Dialog Box - container, for showing/hiding
* Speaker Display - name of character speaking
* Text Display - ui for the dialog text 
* Next Button Visibility Container - element to be shown/hidden when 'next' is or isn't a valid option
* Choices Parent - to be shown and hidden when choices exist / are absent. Parent container for instances of ...
* Choice Prefab - choice UI to be added to the Choices Parent when there are choices to be displayed

Additionally, your Next Button should have its OnClick configured to trigger InkManager.Continue. You can setup more, custom actions, such as character portraits, music, backgrounds, etc directly with the Ink Manager.

Speech Bubble Manager
-------------------------------------------

Please see Samples\SpeechBubble\Speech Bubble Scene.unity for example.

Create a prefab UI for your speech bubbles, and attach a script which extends SpeechBubble\SpeechBubble.cs - these will be custom, as the way your bubbles work and present choices may vary greatly between projects.

Add your bubble UI as children of sprites in your scene and give those sprites the SpeechBubble\Talkable.cs script.

Then, go to the story manager, right click on the Speech Bubble Manager (Script) and select "Autodetect all Talkables" to populate the Talkables list. "Speaker" should remain unset unless you need the speech bubble to be shown as the scene is loaded.

Exploration & Interaction
--------------------------------

Please see Samples\Exploration\Exploration.unity for example. Only a sample is provided for this, as a guide.

Inventory Manager
--------------------------------

Please see Samples\Inventory\Inventory.unity for example.

Add Inventory\InventoryManager.cs to your Story Manager. Create and provide the script with the names of:

* An inventory list - `LIST inventory = ...`
* A function for obtaining inventory `== function pickup(item) ==`
* A function for combining inventory `== function combine(items) ==`

You must create instances of the ScriptableObject 'Inventory\InventoryItem.cs' or extended versions of it and add them to the Inventory Manager's "Items" list.

An example of how to create a UI that displays this inventory is found in the sample.
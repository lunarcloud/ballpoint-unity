using System;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace Ballpoint.Reaction {
  
	[HelpURL(HelpURL + "#ink-manager")]
	[AddComponentMenu("Ballpoint/Tag Observer")]
	public class InkTagObserver : MonoBehaviour {
		public const string HelpURL = "https://github.com/lunarcloud/ballpoint-unity/tree/master/Documentation~/Ballpoint.md";
    public TagEventWatcher watcher;
  }

}
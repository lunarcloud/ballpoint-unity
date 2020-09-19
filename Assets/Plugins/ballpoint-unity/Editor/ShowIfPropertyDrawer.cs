using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Ballpoint {
	/// <summary>
	/// Based on: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
	/// </summary>
	[CustomPropertyDrawer(typeof(ShowIfAttribute))]
	public class ShowIfPropertyDrawer : PropertyDrawer {
		#region Fields

		// Reference to the attribute on the property.
		ShowIfAttribute drawIf;

		// Field that is being compared.
		SerializedProperty comparedField;

		PropertyDrawer propertyDrawer;

		#endregion

		/// <summary>
		/// Errors default to showing the property.
		/// </summary>
		private bool ShowMe(SerializedProperty property) {
			drawIf = attribute as ShowIfAttribute;
			string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, drawIf.comparedPropertyName) : drawIf.comparedPropertyName;
			comparedField = property.serializedObject.FindProperty(path);

			if (comparedField == null) {
				Debug.LogError("Cannot find property with name: " + path);
				return true;
			}

			switch (comparedField.type) {
				case "bool":
					return comparedField.boolValue.Equals(drawIf.comparedValue);
				case "Enum":
					return (comparedField.intValue & (int) drawIf.comparedValue) != 0; // Enum HasFlag Equivalent
				default:
					Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
					return true;
			}
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			propertyDrawer = GetPropertyDrawer(property);
			if (ShowMe(property)) {
				Draw(position, property, label);
			} else if (drawIf.disablingType == ShowIfAttribute.DisablingType.ReadOnly) {
				GUI.enabled = false;
				Draw(position, property, label);
				GUI.enabled = true;
			}
		}

		public void Draw(Rect position, SerializedProperty property, GUIContent label) {
			if (propertyDrawer == null) EditorGUI.PropertyField(position, property);
			else propertyDrawer.OnGUI(position, property, label);
		}
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			if (!ShowMe(property)) return 0f;
			return propertyDrawer == null ?
				base.GetPropertyHeight(property, label) :
				propertyDrawer.GetPropertyHeight(property, label);
		}

		public PropertyDrawer GetPropertyDrawer(SerializedProperty property) {
			Type fieldType = fieldInfo.FieldType;

			Type propertyDrawerType = (Type) Type.GetType("UnityEditor.ScriptAttributeUtility,UnityEditor")
				.GetMethod("GetDrawerTypeForType", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
				.Invoke(null, new object[] {
					fieldType
				});

			PropertyDrawer propertyDrawer = null;
			if (typeof(PropertyDrawer).IsAssignableFrom(propertyDrawerType))
				propertyDrawer = (PropertyDrawer) Activator.CreateInstance(propertyDrawerType, false);

			return propertyDrawer;
		}

	}
}

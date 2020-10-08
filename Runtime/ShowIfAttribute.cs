using System;
using UnityEngine;

namespace Ballpoint {
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public class ShowIfAttribute : PropertyAttribute {
			#region Fields

			public string comparedPropertyName {
					get;
					private set;
			}
			public object comparedValue {
					get;
					private set;
			}
			public DisablingType disablingType {
					get;
					private set;
			}

			/// <summary>
			/// Types of comperisons.
			/// </summary>
			public enum DisablingType {
					ReadOnly = 2,
					DontDraw = 3
			}

			#endregion

			/// <summary>
			/// Only draws the field only if a condition is met. Supports enum and bools.
			/// </summary>
			/// <param name="comparedPropertyName">The name of the property that is being compared (case sensitive).</param>
			/// <param name="comparedValue">The value the property is being compared to.</param>
			/// <param name="disablingType">The type of disabling that should happen if the condition is NOT met. Defaulted to DisablingType.DontDraw.</param>
			public ShowIfAttribute(string comparedPropertyName, object comparedValue, DisablingType disablingType = DisablingType.DontDraw) {
					this.comparedPropertyName = comparedPropertyName;
					this.comparedValue = comparedValue;
					this.disablingType = disablingType;
			}
	}
}

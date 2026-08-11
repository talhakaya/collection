using System;
using UnityEngine;

namespace Collection.Controls
{
	/// Greys out (doesn't hide) this field in the Inspector unless the named sibling bool
	/// field on the same object is true. Unity has no built-in equivalent to Unreal's
	/// EditCondition - this is the minimal version of one, paired with ConditionalFieldDrawer
	/// in Assets/Editor.
	[AttributeUsage(AttributeTargets.Field)]
	public class ConditionalFieldAttribute : PropertyAttribute
	{
		public readonly string conditionFieldName;

		public ConditionalFieldAttribute(string conditionFieldName)
		{
			this.conditionFieldName = conditionFieldName;
		}
	}
}

using Collection.Controls;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label, true);
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var conditional = (ConditionalFieldAttribute)attribute;
		SerializedProperty condition = FindSibling(property, conditional.conditionFieldName);

		// Fields whose condition can't be resolved (typo'd name, non-bool sibling) stay
		// enabled rather than silently locking the Inspector.
		bool enabled = condition == null || condition.propertyType != SerializedPropertyType.Boolean || condition.boolValue;

		using (new EditorGUI.DisabledScope(!enabled))
		{
			EditorGUI.PropertyField(position, property, label, true);
		}
	}

	private static SerializedProperty FindSibling(SerializedProperty property, string name)
	{
		string path = property.propertyPath;
		int lastDot = path.LastIndexOf('.');
		string siblingPath = lastDot >= 0 ? path.Substring(0, lastDot + 1) + name : name;
		return property.serializedObject.FindProperty(siblingPath);
	}
}

using Collection.Story;
using UnityEditor;
using UnityEngine;

namespace Collection.EditorTools
{
	/// A button for each emote under the ScreenFace's own fields, to try them while playing.
	[CustomEditor(typeof(ScreenFace))]
	public class ScreenFaceEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			var face = (ScreenFace)target;
			EditorGUILayout.Space();

			if (!Application.isPlaying)
			{
				EditorGUILayout.HelpBox("Press play to try the emotes from here.", MessageType.None);
				return;
			}

			EditorGUILayout.LabelField("Showing", string.IsNullOrEmpty(face.Showing) ? "nothing yet" : face.Showing);

			using (new EditorGUI.DisabledScope(face.idle == null))
			{
				if (GUILayout.Button(ScreenFace.IdleName)) face.ShowIdle();
			}

			foreach (ScreenFace.Emote emote in face.emotes)
			{
				using (new EditorGUI.DisabledScope(emote.clip == null))
				{
					string label = emote.clip == null ? emote.name + " (no video)" : emote.name;
					if (GUILayout.Button(label)) face.Show(emote.name);
				}
			}

			// The "Showing" line changes without anything being clicked.
			Repaint();
		}
	}
}

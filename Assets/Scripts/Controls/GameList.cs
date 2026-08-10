using System;
using UnityEngine;

namespace Collection.Controls
{
	/// <summary>
	/// Per-game metadata: one entry per Assets/games/&lt;Name&gt; folder, keyed by gameName
	/// (matches the folder name / input action map name). GameImportWindow creates a blank
	/// entry automatically on import; fields are then filled in / tweaked by hand in the
	/// Inspector. Consumers (e.g. GamePhysicsManager) look entries up by name and fall back
	/// to a sensible default when a game has no entry yet.
	/// </summary>
	[CreateAssetMenu(fileName = "GameList", menuName = "Collection/Game List")]
	public class GameList : ScriptableObject
	{
		[Serializable]
		public class Entry
		{
			public string gameName;
			[TextArea] public string description;
			public Vector2 gravity;

			[Tooltip("For mouse-only games with no gamepad support of their own: left stick moves a virtual cursor and GlobalInputManager's MouseEmulate(Left/Right)Click actions stand in for mouse buttons.")]
			public bool enableMouseEmulation;
			[Tooltip("Screen pixels/sec the emulated cursor moves at full stick deflection.")]
			[ConditionalField(nameof(enableMouseEmulation))]
			public float mouseEmulationSpeed = 1000f;

#if UNITY_EDITOR
			// Editor-only scene picker, synced into entryScenePath (below) by OnValidate.
			// SceneAsset lives in UnityEditor and can't be referenced from runtime code -
			// this field is stripped from player builds entirely, leaving entryScenePath
			// (a plain string, safe in both Editor and builds) as what actually ships.
			public UnityEditor.SceneAsset entryScene;
#endif

			// Which scene the main menu launches for this game. GameImportWindow guesses a
			// starting value (a scene named "main" if there is one) - explicit and editable
			// here (via the entryScene picker above) rather than re-guessed by
			// MainMenuController every time, since a wrong guess (e.g. an auxiliary editing
			// scene mistaken for the real entry point) has no way to be corrected other than
			// renaming scene files.
			[HideInInspector] public string entryScenePath;
		}

		public Entry[] entries = Array.Empty<Entry>();

		public bool TryGetEntry(string gameName, out Entry entry)
		{
			foreach (Entry candidate in entries)
			{
				if (string.Equals(candidate.gameName, gameName, StringComparison.OrdinalIgnoreCase))
				{
					entry = candidate;
					return true;
				}
			}

			entry = default;
			return false;
		}

		// Unity's array-insert serialization zero-values a freshly-added Inspector element -
		// C# field initializers aren't run for it (true for classes and structs alike). A
		// blank gameName is the only reliable "this entry hasn't been configured yet" signal
		// (a real entry always needs one to be looked up by TryGetEntry), so it's safe to
		// backfill gravity here without risking a deliberately-set (0, 0) on a named entry.
		private void OnValidate()
		{
			foreach (Entry entry in entries)
			{
				if (string.IsNullOrEmpty(entry.gameName) && entry.gravity == Vector2.zero)
				{
					entry.gravity = new Vector2(0f, -9.81f);
				}

				if (string.IsNullOrEmpty(entry.gameName) && entry.mouseEmulationSpeed == 0f)
				{
					entry.mouseEmulationSpeed = 1000f;
				}

#if UNITY_EDITOR
				// Scene picker is the source of truth once set; entries created before this
				// field existed (a plain string path already set, no picked object yet) get
				// the object reference filled in for display instead of losing their value.
				if (entry.entryScene != null)
				{
					entry.entryScenePath = UnityEditor.AssetDatabase.GetAssetPath(entry.entryScene);
				}
				else if (!string.IsNullOrEmpty(entry.entryScenePath))
				{
					entry.entryScene = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(entry.entryScenePath);
				}
#endif
			}
		}
	}
}

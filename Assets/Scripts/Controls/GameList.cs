using System;
using UnityEngine;

namespace Collection.Controls
{
	/// The shape a game's picture is kept to (see GameList.Entry.aspect).
	public enum GameAspect
	{
		Any,
		FourByThree,
		SixteenByNine
	}

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

			[Tooltip("Leaves the game out of the main menu's list of games. Off for every game unless ticked. The game's scenes and files are still part of the build; this only takes away the way in.")]
			public bool hidden;

			[TextArea] public string description;
			public Vector2 gravity;

			[Tooltip("For mouse-only games with no gamepad support of their own: left stick moves a virtual cursor and GlobalInputManager's MouseEmulate(Left/Right)Click actions stand in for mouse buttons.")]
			public bool enableMouseEmulation;
			[Tooltip("Screen pixels/sec the emulated cursor moves at full stick deflection.")]
			[ConditionalField(nameof(enableMouseEmulation))]
			public float mouseEmulationSpeed = 1000f;

			[Tooltip("Cursor shown while this game is running, in place of the collection's default one - for mouse and gamepad alike. Leave empty for the default. Drawn at its own pixel size on a 1080p screen and scaled with the resolution. For a game that draws its own pointer, assign a fully transparent texture so nothing is drawn over it.")]
			public Texture2D cursorTexture;
			[Tooltip("The click point within cursorTexture, in pixels from its top-left corner.")]
			public Vector2 cursorHotspot;

			[Tooltip("The shape the game has to be shown at. On a screen of another shape it gets black bars: a 4:3 game on the left and right, a 16:9 game above and below on a 16:10 screen. Any leaves the game to fill whatever screen there is.")]
			public GameAspect aspect;

			[Tooltip("The music volume of the settings applies to sources playing music, which is guessed from the clip: a long one, or a looping one of some length. Clips the guess gets wrong are named here - a short loop that is music.")]
			public string[] musicClips = Array.Empty<string>();
			[Tooltip("And the other way: a long or looping clip that is not music (wind, an engine, speech).")]
			public string[] soundClips = Array.Empty<string>();

			/// The aspect ratio asked for, or zero for any.
			public float AspectRatio
			{
				get
				{
					switch (aspect)
					{
						case GameAspect.FourByThree: return 4f / 3f;
						case GameAspect.SixteenByNine: return 16f / 9f;
						default: return 0f;
					}
				}
			}

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

		[Header("Global cursor behavior")]
		[Tooltip("Hide the mouse cursor whenever a gamepad is the active input device, across every game/menu - not just ones with mouse emulation enabled. Turn off to keep showing it at the mouse's position regardless, e.g. for debugging; gamepad mouse emulation is then off too.")]
		public bool hideCursorWhenUsingGamepad = true;

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

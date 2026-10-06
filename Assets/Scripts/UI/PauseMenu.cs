using Collection.Controls;
using Collection.Saving;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Collection.UI
{
	/// <summary>
	/// The pause screen, on Escape and Start in every game: Continue, Settings, Exit to
	/// main menu, and how long ago the game last saved.
	///
	/// Pausing stops time (Time.timeScale), sound (AudioListener.pause) and the game's
	/// input (the menus see to that). That is everything for a game that runs on Unity's
	/// time. One that steps itself by the frame, or on real time, has to look at
	/// PauseMenu.Paused and stand still.
	/// </summary>
	public static class PauseMenu
	{
		private const string AssetResourcePath = "Input/CollectionInput";

		private static MenuScreen screen;
		private static float timeScaleBefore = 1f;
		private static bool listenerPausedBefore;

		public static bool Paused { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			Paused = false;
			screen = null;

			InputActionAsset asset = Resources.Load<InputActionAsset>(AssetResourcePath);
			InputAction pause = asset != null ? asset.FindActionMap("Global")?.FindAction("Pause") : null;
			if (pause == null)
			{
				Debug.LogError($"PauseMenu: no Global/Pause action in '{AssetResourcePath}'.");
				return;
			}

			pause.performed -= OnPausePressed;
			pause.performed += OnPausePressed;
			pause.Enable();

			SceneManager.sceneLoaded -= OnSceneLoaded;
			SceneManager.sceneLoaded += OnSceneLoaded;

			var go = new GameObject(nameof(PauseMenu));
			Object.DontDestroyOnLoad(go);
			go.AddComponent<Runner>();
		}

		// From the input callback, so before any game script has run for the frame: the
		// game never sees the press that paused it.
		private static void OnPausePressed(InputAction.CallbackContext context)
		{
			if (Paused)
			{
				// Start again continues. (Escape is Cancel too, which the screen handles.)
				if (Menus.Top == screen && !(context.control.device is Keyboard))
				{
					Resume();
				}

				return;
			}

			if (Menus.IsOpen) return;
			if (!GameContext.IsPlayed(SceneManager.GetActiveScene().path)) return;

			// In "Just the games", Shift+Escape and Select+Start are the quick way out
			// (GlobalInputManager), not a pause.
			// In a game played from the story they are the cheat for its artifact, and not a
			// pause either.
			if (!SaveManager.IsStoryMode || (BuildSettings.Cheats && Collection.Story.StoryGames.Playing))
			{
				Keyboard keyboard = Keyboard.current;
				Gamepad pad = context.control.device as Gamepad;
				if (keyboard != null && keyboard.shiftKey.isPressed && context.control.device is Keyboard) return;
				if (pad != null && pad.selectButton.isPressed) return;
			}

			Pause();
		}

		public static void Pause()
		{
			if (Paused) return;

			Paused = true;
			SaveManager.Paused = true;
			timeScaleBefore = Time.timeScale;
			Time.timeScale = 0f;
			listenerPausedBefore = AudioListener.pause;
			AudioListener.pause = true;

			screen = new MenuScreen { title = "Paused" };
			screen.Button("Continue", Resume);
			screen.Button("Settings", SettingsScreen.Open);
			// A game started from inside the story is left for the story (ReturnToMainMenu
			// sees to that): without its artifact, unless it has given it already.
			// For testing the story without playing every game through: the game's artifact as
			// if the game had been played to where it gives it.
			// Only in a Debug build (BuildSettings).
			if (BuildSettings.Cheats && Collection.Story.StoryGames.Playing && !Collection.Story.StoryGames.Gathered)
			{
				screen.Button("Cheat: take the artifact", () =>
				{
					Resume();
					Collection.Story.StoryGames.Finish();
				});
			}

			string leave = !Collection.Story.StoryGames.Playing ? "Exit to main menu"
				: Collection.Story.StoryGames.Gathered ? "Back to the story" : "Abandon the artifact for now";
			screen.Button(leave, () =>
			{
				Resume();
				GlobalInputManager.ReturnToMainMenu();
			});
			screen.cancel = Resume;
			screen.footer = LastSaved;
			Menus.Push(screen);
		}

		public static void Resume()
		{
			if (!Paused) return;

			Paused = false;
			SaveManager.Paused = false;
			Time.timeScale = timeScaleBefore;
			AudioListener.pause = listenerPausedBefore;
			Menus.CloseAll();
			screen = null;
		}

		private static string LastSaved()
		{
			float seconds = SaveManager.SecondsSinceGameSaved;
			if (seconds < 0f)
			{
				// Nothing saved since the game was started: nothing to say.
				return "";
			}

			return "Last saved " + Ago(seconds) + ".";
		}

		private static string Ago(float seconds)
		{
			if (seconds < 10f) return "just now";
			if (seconds < 60f) return Mathf.FloorToInt(seconds) + " seconds ago";

			int minutes = Mathf.FloorToInt(seconds / 60f);
			if (minutes < 60) return minutes == 1 ? "a minute ago" : minutes + " minutes ago";

			int hours = minutes / 60;
			return hours == 1 ? "an hour ago" : hours + " hours ago";
		}

		// Leaving the game by any other way (the quick exit, a game's own way out) while
		// paused must not leave time stopped in the menu.
		private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (GameContext.IsPlayed(scene.path)) return;

			if (Paused)
			{
				Resume();
			}

			// Nor whatever speed the game had time running at.
			Time.timeScale = 1f;
		}

		private class Runner : MonoBehaviour
		{
			// A game that sets the time scale itself while paused (a slow-motion effect
			// running on real time) is setting what it wants after the pause, not ending it.
			private void LateUpdate()
			{
				if (Paused && Time.timeScale != 0f)
				{
					timeScaleBefore = Time.timeScale;
					Time.timeScale = 0f;
				}
			}
		}
	}
}

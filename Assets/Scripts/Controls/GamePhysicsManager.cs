using UnityEngine;
using UnityEngine.SceneManagement;

namespace Collection.Controls
{
	/// <summary>
	/// Applies the active game's GameList gravity entry on scene load, same scene-based
	/// context switching as TaloketoInputManager. Games without an entry (or whose entry
	/// hasn't been filled in) keep the project-wide gravity captured at startup.
	/// </summary>
	public static class GamePhysicsManager
	{
		private const string AssetResourcePath = "Games/GameList";

		private static GameList gameList;
		private static Vector2 defaultGravity;
		private static string currentGameName;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Bootstrap()
		{
			gameList = Resources.Load<GameList>(AssetResourcePath);
			defaultGravity = Physics2D.gravity;

			SceneManager.sceneLoaded += (scene, mode) => Apply(GameContext.FromScenePath(scene.path));
			Apply(GameContext.FromScenePath(SceneManager.GetActiveScene().path));
		}

		private static void Apply(string gameName)
		{
			if (gameName == currentGameName)
			{
				return;
			}

			currentGameName = gameName;

			Vector2 gravity = defaultGravity;
			if (gameList != null && gameName != null && gameList.TryGetEntry(gameName, out GameList.Entry entry))
			{
				gravity = entry.gravity;
			}

			Physics2D.gravity = gravity;
		}
	}
}

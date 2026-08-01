using UnityEngine;
using UnityEngine.SceneManagement;

namespace Collection.Controls
{
	/// <summary>
	/// Applies GamePhysicsSettings' per-game Physics2D.gravity override on scene load, same
	/// scene-based context switching as TaloketoInputManager. Games without an override keep
	/// the project-wide gravity captured at startup, so this is zero-config for any game that
	/// doesn't need one.
	/// </summary>
	public static class GamePhysicsManager
	{
		private const string AssetResourcePath = "Physics/GamePhysicsSettings";

		private static GamePhysicsSettings settings;
		private static Vector2 defaultGravity;
		private static string currentGameName;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Bootstrap()
		{
			settings = Resources.Load<GamePhysicsSettings>(AssetResourcePath);
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
			if (settings != null && gameName != null && settings.TryGetGravity(gameName, out Vector2 overrideGravity))
			{
				gravity = overrideGravity;
			}

			Physics2D.gravity = gravity;
		}
	}
}

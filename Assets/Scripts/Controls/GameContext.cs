namespace Collection.Controls
{
	/// <summary>
	/// Shared "which game does this scene belong to" logic - a scene under
	/// Assets/games/&lt;Name&gt;/... belongs to &lt;Name&gt;, everything else (main menu,
	/// isolated editing scenes outside that folder) has no game context.
	/// Used by anything that needs to auto-switch per-game state on scene load
	/// (TaloketoInputManager, GamePhysicsManager, MainMenuController).
	/// </summary>
	public static class GameContext
	{
		public const string GamesRootFolder = "Assets/games/";

		/// The story mode's scene. Not a game of the collection, but played like one: it can
		/// be paused and its time counts as play time.
		public const string StoryScenePath = "Assets/Scenes/story.unity";

		/// Whether a scene is one that is played - a game's, or the story's - rather than a menu.
		public static bool IsPlayed(string scenePath)
		{
			return FromScenePath(scenePath) != null || scenePath == StoryScenePath;
		}

		/// The scene a game starts in: the one its GameList entry names, or else the first of
		/// its scenes in the build. Null when there is none.
		public static string EntryScenePath(string gameName)
		{
			GameList list = UnityEngine.Resources.Load<GameList>("Games/GameList");
			if (list != null && list.TryGetEntry(gameName, out GameList.Entry entry) && !string.IsNullOrEmpty(entry.entryScenePath))
			{
				return entry.entryScenePath;
			}

			for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
			{
				string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
				if (string.Equals(FromScenePath(path), gameName, System.StringComparison.OrdinalIgnoreCase))
				{
					return path;
				}
			}

			return null;
		}

		public static string FromScenePath(string scenePath)
		{
			if (string.IsNullOrEmpty(scenePath) || !scenePath.StartsWith(GamesRootFolder))
			{
				return null;
			}

			string remainder = scenePath.Substring(GamesRootFolder.Length);
			return remainder.Split('/')[0];
		}
	}
}

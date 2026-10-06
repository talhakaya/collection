using Collection.Controls;
using Collection.Saving;
using Collection.UI;
using UnityEngine.SceneManagement;

namespace Collection.Story
{
    // The story's levels: one scene each, played in order (GameContext.StoryLevelScenes). Which one a save slot
    // is at is kept in the slot. There is no going back to an earlier one.
    public static class StoryLevels
    {
        public static int Count => GameContext.StoryLevelScenes.Length;

        // The level the slot being played is at.
        public static int Current
        {
            get
            {
                int level = SaveManager.Slot.story.level;
                return level < 0 ? 0 : level >= Count ? Count - 1 : level;
            }
        }

        public static string CurrentName => GameContext.StoryLevelNames[Current];

        // The scene of the slot's level. A level that has no scene in the build yet falls back to the first.
        public static string CurrentScene
        {
            get
            {
                string scene = GameContext.StoryLevelScenes[Current];
                return SceneUtility.GetBuildIndexByScenePath(scene) >= 0 ? scene : GameContext.StoryLevelScenes[0];
            }
        }

        // Loads the slot's level: on starting or continuing a slot, and on coming back from a game.
        public static void Load()
        {
            LoadingScreen.Load(CurrentScene);
        }

        // On to the next level, for the end of the cutscene between two. The character starts the new level where
        // it stands in that level's scene. Does nothing on the last level.
        public static void Advance()
        {
            StorySave story = SaveManager.Slot.story;
            if (Current >= Count - 1)
                return;
            story.level = Current + 1;
            story.placeSaved = false;
            story.bikeSaved = false;
            SaveManager.Save();
            Load();
        }
    }
}

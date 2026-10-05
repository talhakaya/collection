using System.Collections;
using Collection.Controls;
using Collection.Saving;
using Collection.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Collection.Story
{
    // Games of the collection played from inside the story, each for an artifact.
    //
    //   Play (an Artifact, in the story scene): the game's first scene is loaded in place of the story's. It saves
    //   into the story slot being played, as any game does in story mode.
    //
    //   Finish (the game, where it ends): the artifact is won and kept in the slot, and the story scene comes back.
    //   Started any other way, the game is told so (false) and does what it always did at its end.
    //
    //   Leave (the pause screen's way out, or anything else that would go to the main menu): the story scene comes
    //   back with nothing won.
    //
    // The story scene is loaded afresh each time and sets itself up from the slot (StoryDirector).
    public static class StoryGames
    {
        // The artifact the game being played is for. Null when no game is being played from the story.
        static string reward;
        static bool finishing;
        static Runner runner;

        public static bool Playing => reward != null && SaveManager.IsStoryMode;

        // The artifact won by the game just come back from, for the story scene to make something of as it starts.
        // Null once the scene has taken it.
        public static string JustWon { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset()
        {
            reward = null;
            finishing = false;
            runner = null;
            JustWon = null;
        }

        public static string TakeJustWon()
        {
            string won = JustWon;
            JustWon = null;
            return won;
        }

        public static void Play(string game, string artifact)
        {
            string scene = GameContext.EntryScenePath(game);
            if (scene == null)
            {
                Debug.LogError($"StoryGames: there is no game called \"{game}\" to play for the artifact \"{artifact}\".");
                return;
            }

            reward = artifact;
            finishing = false;
            JustWon = null;
            SaveManager.Save();
            SceneManager.LoadScene(scene);
        }

        // For a game to call where it ends. `after` is how long the end stays on the screen first (s, real time).
        // False when the game was not started from the story.
        public static bool Finish(float after = 0f)
        {
            if (!Playing)
                return false;
            if (finishing)
                return true;

            finishing = true;
            if (after > 0f)
            {
                if (runner == null)
                {
                    var go = new GameObject(nameof(StoryGames));
                    Object.DontDestroyOnLoad(go);
                    runner = go.AddComponent<Runner>();
                }
                runner.StartCoroutine(FinishAfter(after));
            }
            else
            {
                Win();
            }
            return true;
        }

        static IEnumerator FinishAfter(float seconds)
        {
            // The pause screen stops the wait as it stops the game.
            for (float waited = 0f; waited < seconds; waited += PauseMenu.Paused ? 0f : Time.unscaledDeltaTime)
                yield return null;

            // Left in the meantime.
            if (reward != null && finishing)
                Win();
        }

        static void Win()
        {
            StorySave story = SaveManager.Slot.story;
            if (!story.artifacts.Contains(reward))
                story.artifacts.Add(reward);
            JustWon = reward;
            BackToTheStory();
        }

        // True when a game was being played from the story, and the story is what comes next.
        public static bool Leave()
        {
            if (!Playing)
                return false;
            BackToTheStory();
            return true;
        }

        static void BackToTheStory()
        {
            reward = null;
            finishing = false;
            SaveManager.Save();
            if (PauseMenu.Paused)
                PauseMenu.Resume();

            // Whatever speed the game left time running at.
            Time.timeScale = 1f;
            SceneManager.LoadScene(GameContext.StoryScenePath);
        }

        class Runner : MonoBehaviour
        {
        }
    }
}

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
    //   Ask (an Artifact, in the story scene): a screen says what the game is - its name, its year, how long it
    //   takes to give its artifact - and offers to play it or go back.
    //
    //   Play: the game's first scene is loaded in place of the story's. It saves into the story slot being played,
    //   as any game does in story mode.
    //
    //   Finish (the game, where it gives its artifact - its end, for most): the artifact is won and kept in the
    //   slot, and a screen says so. From there it is back to the story, to talk to the artifact; or, for a game
    //   whose GameList entry has continueAfterArtifact, on with the game. Started any other way, the game is told
    //   so (false) and does what it always did.
    //
    //   Leave (the pause screen's way out, or anything else that would go to the main menu): the story scene comes
    //   back, with the artifact if it was won before leaving and without it if not.
    //
    // The story scene is loaded afresh each time and sets itself up from the slot (StoryDirector).
    public static class StoryGames
    {
        // The artifact the game being played is for. Null when no game is being played from the story.
        static string reward;
        static string rewardTitle;
        static string game;
        // Finish has been called, and the artifact is on its way (finishing) or won (gathered).
        static bool finishing;
        static bool gathered;
        // On the way back to the story: the game is still there for a frame or two under the loading screen, and
        // is not to take that for being played by itself (and run its own ending, or go to the main menu).
        static bool leaving;
        static float timeScaleBefore = 1f;
        static Runner runner;

        public static bool Playing => reward != null && SaveManager.IsStoryMode;

        static bool Leaving => leaving && LoadingScreen.Loading;

        // The cheat for the artifact can be used: a game is being played for one it has not given yet, in a
        // Debug build (BuildSettings).
        public static bool CanCheat => BuildSettings.Cheats && Playing && !Gathered;

        // The game being played has given its artifact, and is being played on.
        public static bool Gathered => gathered;

        // The artifact won by the game just come back from, for the story scene to make something of as it starts.
        // Null once the scene has taken it.
        public static string JustWon { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset()
        {
            reward = null;
            rewardTitle = null;
            game = null;
            finishing = false;
            gathered = false;
            leaving = false;
            runner = null;
            JustWon = null;
        }

        // For the story scene, as it starts: no game is being played from it, however the last one was left.
        public static void Arrive()
        {
            reward = null;
            rewardTitle = null;
            game = null;
            finishing = false;
            gathered = false;
            leaving = false;
        }

        // The artifact kept in the slot, and remembered as the one just won for the story scene to come.
        static void Win(string artifact)
        {
            StorySave story = SaveManager.Slot.story;
            if (!story.artifacts.Contains(artifact))
                story.artifacts.Add(artifact);
            JustWon = artifact;
            SaveManager.Save();
        }

        public static string TakeJustWon()
        {
            string won = JustWon;
            JustWon = null;
            return won;
        }

        static GameList.Entry Entry(string gameName)
        {
            GameList list = Resources.Load<GameList>("Games/GameList");
            return list != null && list.TryGetEntry(gameName, out GameList.Entry entry) ? entry : null;
        }

        // Before the game: what it is, and whether to play it.
        public static void Ask(string gameName, string artifact, string artifactTitle)
        {
            if (Menus.IsOpen)
                return;

            GameList.Entry entry = Entry(gameName);
            string text = "Game: " + (entry != null ? entry.gameName : gameName);
            if (entry != null && entry.year > 0)
                text += "\nYear: " + entry.year;
            if (entry != null && entry.artifactMinutes > 0)
                text += "\nPlay time for artifact: " + entry.artifactMinutes + (entry.artifactMinutes == 1 ? " minute" : " minutes");
            if (entry != null && !string.IsNullOrWhiteSpace(entry.description))
                text += "\n\n" + entry.description.Trim();

            var screen = new MenuScreen { title = "Enter the game?" };
            screen.body = () => text;
            screen.Button("Play", () =>
            {
                Menus.CloseAll();
                Play(gameName, artifact, artifactTitle);
            });
            screen.Button("Back", Menus.CloseAll);
            // For testing the story without playing every game through. Only in a Debug build (BuildSettings).
            if (BuildSettings.Cheats)
            {
                screen.Button("Cheat: take the artifact", () =>
                {
                    Menus.CloseAll();
                    Cheat(artifact);
                });
            }
            screen.cancel = Menus.CloseAll;
            Menus.Push(screen);
        }

        public static void Play(string gameName, string artifact, string artifactTitle)
        {
            string scene = GameContext.EntryScenePath(gameName);
            if (scene == null)
            {
                Debug.LogError($"StoryGames: there is no game called \"{gameName}\" to play for the artifact \"{artifact}\".");
                return;
            }

            reward = artifact;
            rewardTitle = artifactTitle;
            game = gameName;
            finishing = false;
            gathered = false;
            leaving = false;
            JustWon = null;
            SaveManager.Save();
            LoadingScreen.Load(scene);
        }

        // The artifact without the game, from the story scene: won, kept, and the scene started again as it is on
        // coming back from the game.
        public static void Cheat(string artifact)
        {
            Win(artifact);
            StoryLevels.Load();
        }

        // For a game to call where it gives its artifact. `after` is how long that moment stays on the screen
        // first (s, real time). False when the game was not started from the story. Calling it again, as a game
        // that goes on may, does nothing more.
        public static bool Finish(float after = 0f)
        {
            if (Leaving)
                return true;
            if (!Playing)
                return false;
            if (finishing || gathered)
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
                Gather();
            }
            return true;
        }

        static IEnumerator FinishAfter(float seconds)
        {
            // The pause screen stops the wait as it stops the game.
            for (float waited = 0f; waited < seconds; waited += PauseMenu.Paused ? 0f : Time.unscaledDeltaTime)
                yield return null;
            while (PauseMenu.Paused)
                yield return null;

            // Left in the meantime.
            if (reward != null && finishing)
                Gather();
        }

        // The artifact is won, whatever happens next, and the screen that says so is up.
        static void Gather()
        {
            finishing = false;
            gathered = true;
            Win(reward);

            // The game stands still under the screen.
            timeScaleBefore = Time.timeScale;
            Time.timeScale = 0f;

            GameList.Entry entry = Entry(game);
            bool canContinue = entry != null && entry.continueAfterArtifact;
            string title = rewardTitle;

            var screen = new MenuScreen { title = "Artifact gathered" };
            screen.body = () => title;
            if (canContinue)
                screen.Button("Continue playing", ContinuePlaying);
            screen.Button("Talk to artifact", BackToTheStory);
            // Backing out is carrying on, where that can be done; otherwise there is no way but on.
            screen.cancel = canContinue ? (System.Action)ContinuePlaying : () => { };
            Menus.CloseAll();
            Menus.Push(screen);
        }

        static void ContinuePlaying()
        {
            Menus.CloseAll();
            Time.timeScale = timeScaleBefore;
        }

        // True when a game was being played from the story, and the story is what comes next.
        public static bool Leave()
        {
            if (Leaving)
                return true;
            if (!Playing)
                return false;
            BackToTheStory();
            return true;
        }

        static void BackToTheStory()
        {
            reward = null;
            rewardTitle = null;
            game = null;
            finishing = false;
            gathered = false;
            SaveManager.Save();
            if (PauseMenu.Paused)
                PauseMenu.Resume();
            Menus.CloseAll();

            // Whatever speed the game left time running at.
            Time.timeScale = 1f;
            leaving = true;
            StoryLevels.Load();
        }

        class Runner : MonoBehaviour
        {
        }
    }
}

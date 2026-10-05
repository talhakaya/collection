using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// Ported from SaveManager.as, with the Flash SharedObject swapped for PlayerPrefs as
	/// Golfinity and Nykrig already do. Keys are namespaced, since PlayerPrefs is shared by
	/// the whole collection while a SharedObject was the game's own.
	///
	/// It persists playedBefore and the six scores, and nothing else. In particular it does
	/// not persist SceneManager.replayInputs, even though Scene.checkIfFinished assigns them
	/// alongside the score - so the ghost replays only ever show runs from the current
	/// session. That is the original's behaviour; don't "fix" it without deciding to.
	/// </summary>
	public class SaveManager
	{
		public static SaveManager instance;
		public static bool playedBefore = false;

		public SaveManager()
		{
			instance = this;

			playedBefore = Data.playedBefore;
			if (!playedBefore)
			{
				_save();
			}
			else
			{
				_load();
			}
		}

		public static void save()
		{
			instance._save();
		}

		public static void load()
		{
			instance._load();
		}

		// In the collection: the scores were PlayerPrefs keys. They are the game's part of the
		// collection's save file now.
		private static Collection.Saving.SleepyTimeSave Data
		{
			get { return Collection.Saving.SaveManager.Slot.sleepyTime; }
		}

		public void _save()
		{
			Data.playedBefore = true;
			Data.scores.Clear();
			for (int i = 0; i < 6; i++)
			{
				Data.scores.Add(SceneManager.scores[i]);
			}

			Collection.Saving.SaveManager.MarkDirty();
		}

		public void _load()
		{
			for (int i = 0; i < 6; i++)
			{
				SceneManager.scores[i] = i < Data.scores.Count ? Data.scores[i] : 0;
			}
		}
	}
}

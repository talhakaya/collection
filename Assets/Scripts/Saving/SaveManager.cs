using System;
using System.IO;
using Collection.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Collection.Saving
{
	/// How the game being played was reached.
	public enum SaveMode
	{
		/// From "Just the games": saves go to the free-play area.
		FreePlay,

		/// From a story slot.
		Story
	}

	/// <summary>
	/// The collection's one save file: save.json in the persistent data folder, holding the
	/// three story slots, the free-play area and the settings (SaveData).
	///
	/// It is read - or made, if there is none - before the first scene loads. A game reads
	/// and changes its part of SaveManager.Slot and calls MarkDirty(); the file is written at
	/// the end of that frame, and again on leaving a game and on quitting.
	///
	/// Slot is the story slot being played, or the free-play area. Started straight from a
	/// game's scene in the editor, with no menu to choose, it is the first slot.
	/// </summary>
	public static class SaveManager
	{
		private const string FileName = "save.json";

		private static SaveData data;
		private static int storySlot = -1;
		private static bool dirty;

		public static SaveData Data
		{
			get
			{
				if (data == null) Load();
				return data;
			}
		}

		public static Settings Settings => Data.settings;

		/// Whether the current game was reached from a story slot or from "Just the games".
		public static SaveMode Mode { get; private set; }

		public static bool IsStoryMode => Mode == SaveMode.Story;

		/// The story slot being played, or -1 in free play.
		public static int StorySlotIndex => Mode == SaveMode.Story ? storySlot : -1;

		/// What the current game saves into.
		public static SaveSlotData Slot => Mode == SaveMode.Story ? Data.slots[storySlot] : Data.freePlay;

		/// Whether a game is paused by the collection (the pause screen). Play time does not
		/// run while it is.
		public static bool Paused { get; set; }

		/// Raised after the file has been written.
		public static event Action Saved;

		public static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			Load();

			// Nothing has chosen yet. From the menu, a choice follows before any game loads;
			// a game's scene played directly in the editor keeps this.
#if UNITY_EDITOR
			PlayStory(0);
#else
			PlayFree();
#endif

			var go = new GameObject(nameof(SaveManager));
			UnityEngine.Object.DontDestroyOnLoad(go);
			go.AddComponent<Runner>();
		}

		/// From here on games save into this story slot.
		public static void PlayStory(int slot)
		{
			storySlot = Mathf.Clamp(slot, 0, SaveData.SlotCount - 1);
			Mode = SaveMode.Story;
		}

		/// From here on games save into the free-play area.
		public static void PlayFree()
		{
			Mode = SaveMode.FreePlay;
		}

		/// Empties a story slot, as if it had never been played.
		public static void DeleteSlot(int slot)
		{
			Data.slots[slot] = new SaveSlotData();
			Save();
		}

		/// To be called after changing anything in Slot or Settings. The file is written once
		/// at the end of the frame, however many changes there were.
		public static void MarkDirty()
		{
			dirty = true;
		}

		/// Seconds of play since the current slot was last written.
		public static float SecondsSinceSave => Mathf.Max(0f, Slot.timePlayed - Slot.timePlayedAtLastSave);

		public static void Load()
		{
			data = null;
			string path = FilePath;
			if (File.Exists(path))
			{
				try
				{
					data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
				}
				catch (Exception e)
				{
					Debug.LogError($"SaveManager: could not read {path} ({e.Message}). Starting from an empty save; the unreadable file is kept as {FileName}.bad.");
					try { File.Copy(path, path + ".bad", true); } catch (Exception) { }
				}
			}

			bool created = data == null;
			if (data == null) data = new SaveData();
			if (data.slots == null) data.slots = new System.Collections.Generic.List<SaveSlotData>();
			while (data.slots.Count < SaveData.SlotCount) data.slots.Add(new SaveSlotData());
			if (data.freePlay == null) data.freePlay = new SaveSlotData();
			if (data.settings == null) data.settings = new Settings();

			if (created) Save();
		}

		/// Writes the file now. Through a temporary file, so a crash mid-write leaves the old
		/// save rather than half of a new one.
		public static void Save()
		{
			if (data == null) return;

			dirty = false;
			foreach (SaveSlotData slot in data.slots) slot.timePlayedAtLastSave = slot.timePlayed;
			data.freePlay.timePlayedAtLastSave = data.freePlay.timePlayed;

			string path = FilePath;
			string temp = path + ".tmp";
			try
			{
				File.WriteAllText(temp, JsonUtility.ToJson(data, true));
				if (File.Exists(path)) File.Delete(path);
				File.Move(temp, path);
				Saved?.Invoke();
			}
			catch (Exception e)
			{
				Debug.LogError($"SaveManager: could not write {path} ({e.Message}).");
			}
		}

		/// Counts play time, and writes the file when something has asked for it.
		private class Runner : MonoBehaviour
		{
			private void Awake()
			{
				SceneManager.sceneLoaded += OnSceneLoaded;
			}

			private void OnDestroy()
			{
				SceneManager.sceneLoaded -= OnSceneLoaded;
			}

			// Leaving a game (for the menu or for another game) is a moment to keep the play
			// time that has run up, whether or not the game saved anything itself.
			private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
			{
				if (GameContext.FromScenePath(scene.path) == null)
				{
					Paused = false;
					Save();
				}
			}

			private void Update()
			{
				// Only time spent playing: in a game's scene, not paused, time running.
				if (!Paused && Time.timeScale > 0f && GameContext.FromScenePath(SceneManager.GetActiveScene().path) != null)
				{
					SaveSlotData slot = Slot;
					slot.started = true;
					slot.timePlayed += Time.unscaledDeltaTime;
				}
			}

			private void LateUpdate()
			{
				if (dirty) Save();
			}

			private void OnApplicationQuit()
			{
				Save();
			}
		}
	}
}

using Collection.Controls;
using Collection.Saving;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Collection
{
	/// <summary>
	/// Makes the settings in the save file (SaveManager.Settings) happen: the two volumes,
	/// and the window.
	///
	/// Volumes. The games were made without a mixer, every source playing straight to the
	/// listener. The master volume is the listener's own volume. For the music volume there
	/// is a mixer (Resources/Audio/CollectionMixer) with one group, Music, and sources that
	/// are playing music are moved onto it as they turn up. Nothing in a game has to be set
	/// up for this; what counts as music is a guess (IsMusic). Where it is wrong, the clip
	/// is named in the game's GameList entry, or an AudioKindOverride is put next to the
	/// source.
	///
	/// Window. Full screen is a borderless window at the chosen resolution, or the
	/// display's own. A game whose GameList entry asks for an aspect ratio is given a
	/// resolution of that shape, and Unity puts black bars around it; so the bars need
	/// nothing from the game either. None of this happens in the editor, where the Game
	/// view's size is the editor's business.
	/// </summary>
	public static class CollectionSettings
	{
		private const string MixerResourcePath = "Audio/CollectionMixer";
		private const string MusicVolumeParameter = "MusicVolume";

		// A source is taken to be music when its clip is this long, or loops and is this long.
		private const float MusicSeconds = 45f;
		private const float LoopingMusicSeconds = 15f;

		// How often sources are looked over for new music, in real seconds.
		private const float ScanInterval = 0.1f;

		private static AudioMixer mixer;
		private static AudioMixerGroup musicGroup;
		private static GameList gameList;
		private static GameList.Entry currentEntry;
		private static float gameVolume = 1f;
		private static Vector3Int appliedDisplay;

		/// <summary>
		/// A game's own setting of the listener volume, where a game used to set
		/// AudioListener.volume: that is the master volume now, so a game sets this and the
		/// two are multiplied. Back to 1 when the game is left.
		/// </summary>
		public static float GameVolume
		{
			get { return gameVolume; }
			set
			{
				gameVolume = value;
				ApplyAudio(SaveManager.Settings);
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			mixer = Resources.Load<AudioMixer>(MixerResourcePath);
			if (mixer != null)
			{
				AudioMixerGroup[] groups = mixer.FindMatchingGroups("Music");
				musicGroup = groups.Length > 0 ? groups[0] : null;
			}

			if (musicGroup == null)
			{
				Debug.LogError($"CollectionSettings: no Music group in Resources/{MixerResourcePath}. The music volume will do nothing.");
			}

			gameList = Resources.Load<GameList>("Games/GameList");
			appliedDisplay = Vector3Int.zero;
			gameVolume = 1f;

			var go = new GameObject(nameof(CollectionSettings));
			Object.DontDestroyOnLoad(go);
			go.AddComponent<Runner>();
		}

		/// Everything, as saved.
		public static void Apply()
		{
			ApplyAudio(SaveManager.Settings);
			ApplyDisplay(SaveManager.Settings);
		}

		/// The volumes of the given settings, which need not be the saved ones: the settings
		/// screen plays what it is showing.
		public static void ApplyAudio(Settings settings)
		{
			AudioListener.volume = Mathf.Clamp01(settings.masterVolume) * gameVolume;
			if (mixer != null)
			{
				mixer.SetFloat(MusicVolumeParameter, Decibels(settings.musicVolume));
			}
		}

		private static float Decibels(float volume)
		{
			return volume <= 0.0001f ? -80f : Mathf.Log10(Mathf.Clamp01(volume)) * 20f;
		}

		// ---- Window ----------------------------------------------------------------------

		/// The aspect ratio the game being played has to be shown at, or zero for any.
		public static float CurrentGameAspect()
		{
			string game = GameContext.FromScenePath(SceneManager.GetActiveScene().path);
			if (game != null && gameList != null && gameList.TryGetEntry(game, out GameList.Entry entry))
			{
				return entry.AspectRatio;
			}

			return 0f;
		}

		public static void ApplyDisplay(Settings settings)
		{
#if !UNITY_EDITOR
			int width = settings.resolutionWidth;
			int height = settings.resolutionHeight;
			if (width <= 0 || height <= 0)
			{
				width = Display.main.systemWidth;
				height = Display.main.systemHeight;
				if (!settings.fullscreen && width > 1280 && height > 720)
				{
					// A window the size of the display would not fit on it.
					width = 1280;
					height = 720;
				}
			}

			// The game's shape, as large as fits in that.
			float aspect = CurrentGameAspect();
			if (aspect > 0f)
			{
				if (width > height * aspect) width = Mathf.RoundToInt(height * aspect);
				else height = Mathf.RoundToInt(width / aspect);
			}

			var wanted = new Vector3Int(width, height, settings.fullscreen ? 1 : 2);
			if (wanted == appliedDisplay) return;

			appliedDisplay = wanted;
			Screen.SetResolution(width, height, settings.fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
#endif
		}

		// ---- Music -----------------------------------------------------------------------

		private static bool IsMusic(AudioSource source)
		{
			AudioKindOverride given = source.GetComponent<AudioKindOverride>();
			if (given != null) return given.kind == AudioKind.Music;

			AudioClip clip = source.clip;
			if (clip == null) return false;

			if (currentEntry != null)
			{
				string name = clip.name;
				if (currentEntry.musicClips != null && System.Array.IndexOf(currentEntry.musicClips, name) >= 0) return true;
				if (currentEntry.soundClips != null && System.Array.IndexOf(currentEntry.soundClips, name) >= 0) return false;
			}

			return clip.length >= MusicSeconds || (source.loop && clip.length >= LoopingMusicSeconds);
		}

		private static void RouteMusic()
		{
			if (musicGroup == null) return;

			foreach (AudioSource source in Object.FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None))
			{
				// A source a game has put on a mixer of its own is the game's business.
				AudioMixerGroup group = source.outputAudioMixerGroup;
				if (group != null && group != musicGroup) continue;

				AudioMixerGroup wanted = IsMusic(source) ? musicGroup : null;
				if (group != wanted)
				{
					source.outputAudioMixerGroup = wanted;
				}
			}
		}

		private class Runner : MonoBehaviour
		{
			private float nextScan;

			private void Awake()
			{
				SceneManager.sceneLoaded += OnSceneLoaded;
			}

			private void OnDestroy()
			{
				SceneManager.sceneLoaded -= OnSceneLoaded;
			}

			// A mixer does not keep a value set before the first frame, hence Start.
			private void Start()
			{
				OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
				Apply();
				RouteMusic();
			}

			private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
			{
				string game = GameContext.FromScenePath(scene.path);
				currentEntry = null;
				if (game == null)
				{
					gameVolume = 1f;
				}
				else if (gameList != null && gameList.TryGetEntry(game, out GameList.Entry entry))
				{
					currentEntry = entry;
				}

				Apply();
				RouteMusic();
			}

			private void Update()
			{
				if (Time.unscaledTime >= nextScan)
				{
					nextScan = Time.unscaledTime + ScanInterval;
					RouteMusic();
				}
			}
		}
	}
}

using System.Collections.Generic;
using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// Stands in for openfl.Assets, so ported code keeps the source's literal asset paths:
	/// Assets.getBitmapData("img/scene1/line1.png") becomes
	/// SleepyAssets.GetSprite("img/scene1/line1.png"), character for character.
	///
	/// OpenFL cached behind Assets, and the game leans on it - updateGraphicsTimeline swaps
	/// line1/line2 every frame - so everything is loaded once up front and served from a
	/// dictionary. Resources.Load per call would be a per-frame disk/lookup cost.
	///
	/// Music is imported as compressed-in-memory and not preloaded, so holding a reference
	/// to all six songs here costs almost nothing; the short SFX are decompressed on load.
	/// </summary>
	public static class SleepyAssets
	{
		private const string ResourcesRoot = "SleepyTime/";

		// Resources.LoadAll recurses, so the scene folders are loaded first and everything
		// they returned is excluded from the "img" pass - otherwise all six scenes' line1.png
		// would collide on the key "img/line1" and the real "img/scene1/line1" would never be
		// filled in. Matching is by object reference, not name, so it can't misfire.
		private static readonly string[] ImageSubfolders =
		{
			"img/scene1", "img/scene2", "img/scene3", "img/scene4", "img/scene5", "img/scene6",
		};

		private static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
		private static readonly Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();

		public static bool Loaded { get; private set; }

		public static void Preload()
		{
			if (Loaded)
			{
				return;
			}

			HashSet<Object> claimed = new HashSet<Object>();
			foreach (string folder in ImageSubfolders)
			{
				foreach (Sprite sprite in Resources.LoadAll<Sprite>(ResourcesRoot + folder))
				{
					sprites[folder + "/" + sprite.name] = sprite;
					claimed.Add(sprite);
				}
			}

			foreach (Sprite sprite in Resources.LoadAll<Sprite>(ResourcesRoot + "img"))
			{
				if (!claimed.Contains(sprite))
				{
					sprites["img/" + sprite.name] = sprite;
				}
			}

			foreach (AudioClip clip in Resources.LoadAll<AudioClip>(ResourcesRoot + "snd"))
			{
				sounds["snd/" + clip.name] = clip;
			}

			Loaded = true;
		}

		/// <summary>
		/// Takes the path exactly as the ActionScript wrote it, extension and all - the
		/// extension is stripped here so call sites never have to differ from the source.
		/// </summary>
		public static Sprite GetSprite(string path)
		{
			Preload();

			Sprite sprite;
			if (sprites.TryGetValue(StripExtension(path), out sprite))
			{
				return sprite;
			}

			Debug.LogError("SleepyAssets: no sprite for '" + path + "'.");
			return null;
		}

		public static AudioClip GetSound(string path)
		{
			Preload();

			AudioClip clip;
			if (sounds.TryGetValue(StripExtension(path), out clip))
			{
				return clip;
			}

			Debug.LogError("SleepyAssets: no sound for '" + path + "'.");
			return null;
		}

		private static string StripExtension(string path)
		{
			int dot = path.LastIndexOf('.');
			int slash = path.LastIndexOf('/');
			return dot > slash ? path.Substring(0, dot) : path;
		}
	}
}

using System.Collections.Generic;
using UnityEngine;

namespace Games.WhereLostOnesGo
{
	/// The game's embedded bitmaps and sounds, from Resources/WhereLostOnesGo/img and /snd.
	public static class FlashAssets
	{
		private const string ImageRoot = "WhereLostOnesGo/img/";
		private const string SoundRoot = "WhereLostOnesGo/snd/";

		private static readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

		public static Texture2D GetTexture(string name)
		{
			Texture2D texture;
			if (!textures.TryGetValue(name, out texture) || texture == null)
			{
				texture = Resources.Load<Texture2D>(ImageRoot + name);
				if (texture == null)
				{
					Debug.LogError("WhereLostOnesGo: no image '" + name + "'.");
					texture = Texture2D.whiteTexture;
				}

				textures[name] = texture;
			}

			return texture;
		}

		public static AudioClip GetSound(string name)
		{
			AudioClip clip = Resources.Load<AudioClip>(SoundRoot + name);
			if (clip == null)
			{
				Debug.LogError("WhereLostOnesGo: no sound '" + name + "'.");
			}

			return clip;
		}

		public static void Clear()
		{
			textures.Clear();
		}
	}

	/// flash.media.SoundTransform: volume (and pan, unused).
	public class SoundTransform
	{
		public double volume;
		public double pan;

		public SoundTransform(double volume = 1, double pan = 0)
		{
			this.volume = volume;
			this.pan = pan;
		}
	}

	/// flash.media.SoundChannel: one playing sound, whose volume can be changed.
	public class SoundChannel
	{
		internal AudioSource source;

		public SoundTransform soundTransform
		{
			set
			{
				if (source != null)
				{
					source.volume = Mathf.Clamp01((float)value.volume);
				}
			}
		}
	}

	/// flash.media.Sound, made from an embedded sound's name.
	public class FlashSound
	{
		private static readonly List<AudioSource> sources = new List<AudioSource>();

		private readonly string name;

		public FlashSound(string name)
		{
			this.name = name;
		}

		/// Loops is how many more times to play it; the game only uses 0 and 99, and 99 of
		/// its beach sound outlasts the piece, so anything above 0 loops.
		public SoundChannel play(double startTime = 0, int loops = 0)
		{
			AudioSource source = FlashPlayer.NewAudioSource();
			source.clip = FlashAssets.GetSound(name);
			source.loop = loops > 0;
			source.time = (float)(startTime / 1000);
			source.Play();
			sources.Add(source);
			return new SoundChannel { source = source };
		}

		internal static void StopAll()
		{
			foreach (AudioSource source in sources)
			{
				if (source != null)
				{
					source.Stop();
				}
			}

			sources.Clear();
		}
	}
}

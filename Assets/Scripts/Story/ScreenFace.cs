using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Collection.Story
{
	/// <summary>
	/// The face on the screen: a video for each emote ("smile", "scared", "confused"), and
	/// one it idles on. Show("scared") changes the face; an emote that does not loop plays
	/// once and the face goes back to idling.
	///
	/// The videos are played into the screen's render texture, which is what the Morph
	/// Sphere material shows in the hole. There are two players, so that the next video is
	/// made ready on the spare one while the current one keeps playing, and the face changes
	/// without a blank frame in between.
	///
	/// Every video should be framed alike - the picture is stretched over the whole side of
	/// the cube - and saved the right way up: a phone's video has to be re-encoded first, or
	/// it plays on its side.
	/// </summary>
	public class ScreenFace : MonoBehaviour
	{
		[Serializable]
		public class Emote
		{
			public string name;
			public VideoClip clip;

			[Tooltip("Ticked: stays on this until another is shown. Unticked: plays once, then back to idle.")]
			public bool loop;
		}

		[Tooltip("The render texture the Morph Sphere material has as its screen.")]
		public RenderTexture screen;

		[Tooltip("What the face does when nothing else is asked of it. Loops.")]
		public VideoClip idle;

		public List<Emote> emotes = new List<Emote>
		{
			new Emote { name = "smile" },
			new Emote { name = "scared" },
			new Emote { name = "confused" }
		};

		public const string IdleName = "idle";

		private VideoPlayer current;
		private VideoPlayer next;
		private VideoPlayer retiring;
		private string nextName;
		private bool preparing;

		/// The name of what is on the screen: an emote's, "idle", or empty before anything
		/// has started.
		public string Showing { get; private set; } = "";

		private void Awake()
		{
			current = NewPlayer();
			next = NewPlayer();
		}

		private void Start()
		{
			ShowIdle();
		}

		private VideoPlayer NewPlayer()
		{
			VideoPlayer player = gameObject.AddComponent<VideoPlayer>();
			player.playOnAwake = false;
			player.renderMode = VideoRenderMode.RenderTexture;
			player.targetTexture = screen;
			player.aspectRatio = VideoAspectRatio.Stretch;
			player.audioOutputMode = VideoAudioOutputMode.None;
			player.waitForFirstFrame = true;
			player.loopPointReached += OnEnded;
			return player;
		}

		/// Changes the face to an emote. False when there is none of that name, or it has no
		/// video yet.
		public bool Show(string emoteName)
		{
			foreach (Emote emote in emotes)
			{
				if (!string.Equals(emote.name, emoteName, StringComparison.OrdinalIgnoreCase)) continue;

				if (emote.clip == null)
				{
					Debug.LogWarning($"ScreenFace: the emote '{emote.name}' has no video.", this);
					return false;
				}

				Play(emote.clip, emote.loop, emote.name);
				return true;
			}

			Debug.LogWarning($"ScreenFace: no emote named '{emoteName}'.", this);
			return false;
		}

		public void ShowIdle()
		{
			if (idle != null)
			{
				Play(idle, true, IdleName);
			}
		}

		// Made ready on the spare player; Update puts it on the screen when it is.
		private void Play(VideoClip clip, bool loop, string name)
		{
			// The spare player may still be holding the last frame of what was showing before
			// (see Update). It is taken over here, so it is no longer waiting to be stopped.
			retiring = null;
			next.Stop();
			next.clip = clip;
			next.isLooping = loop;
			nextName = name;
			preparing = true;
			next.Prepare();
		}

		private void Update()
		{
			// The player that was showing before the change is stopped once the new one has
			// drawn, not before: until then its last frame is what is on the screen.
			if (retiring != null && current.isPlaying && current.frame > 0)
			{
				retiring.Stop();
				retiring = null;
			}

			if (!preparing || !next.isPrepared) return;

			preparing = false;
			current.Pause();
			if (retiring != null) retiring.Stop();
			retiring = current;
			current = next;
			next = retiring;
			current.Play();
			Showing = nextName;
		}

		private void OnEnded(VideoPlayer player)
		{
			// An emote that plays once has finished: back to idling, unless something else
			// has been asked for meanwhile.
			if (player == current && !player.isLooping && !preparing)
			{
				ShowIdle();
			}
		}
	}
}

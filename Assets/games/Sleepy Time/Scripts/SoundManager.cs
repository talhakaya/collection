using System;
using System.Collections.Generic;
using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// Ported from SoundManager.as: one music channel plus a queue of sound effects that only
	/// ever fire on a quarter-beat grid.
	///
	/// That quantisation is deliberate - hits land on the beat rather than when the player
	/// pressed - and so is the rule that an explosion is dropped whenever anything else is in
	/// the same batch. Both are the game's sound design, not bugs to tidy up.
	///
	/// This is also where the song clock comes from. GameManager.calculateTime asks
	/// TryGetSyncedTime first and only falls back to accumulating frame deltas when no song
	/// is playing - see the comment there for why.
	/// </summary>
	public class SoundManager
	{
		public static SoundManager instance;

		public static AudioClip music1;
		public static AudioClip music2;
		public static AudioClip music3;
		public static AudioClip music4;
		public static AudioClip music5;
		public static AudioClip music6;
		public static AudioClip musicMenu1;
		public static AudioClip musicMenu2;
		public static AudioClip musicMenu3;
		public static AudioClip rate_perfect;
		public static AudioClip rate_great;
		public static AudioClip rate_good;
		public static AudioClip rate_ok;
		public static AudioClip rate_sad;
		public static AudioClip explosion;

		public static SoundTransform soundTransformMain;
		public static SoundTransform soundTransformReplay;

		public static float MinSoundRhythmRatio = 0.25f;
		public static string currentRate = "";
		public static bool currentExplosion = false;
		public static bool musicOn = true;

		// AudioSource.timeSamples only moves when the audio thread hands over a new buffer, so
		// reading it raw makes the song clock stair-step. It's advanced by frame time and eased
		// back onto the audio clock instead; a jump bigger than the threshold (a seek, a stall,
		// a song change) snaps rather than eases.
		private const float SyncCorrectionRate = 0.1f;
		private const double SyncResyncThresholdMs = 100.0;

		// Enough voices for a batch of hit sounds across the active scene and its ghosts.
		private const int SfxVoices = 12;

		public float soundRhythmCountOld;
		public float soundRhythmCount;
		public List<SoundSpec> soundQueue;
		public double position;
		public bool playing;
		public AudioClip currentMusic;
		public AudioSource channelMusic;

		private AudioSource[] sfxVoices;
		private int nextSfxVoice;

		// Only the six songs drive GameManager.time. The menu tracks must not: they don't loop,
		// so a finished menu track would freeze the clock and with it every animation on screen.
		private bool syncsGameTime;
		private bool syncPrimed;
		private double syncedMs;

		public SoundManager()
		{
			soundQueue = new List<SoundSpec>();
			soundRhythmCount = 0f;
			soundRhythmCountOld = 0f;
			position = 0.0;
			playing = false;
			instance = this;

			music1 = SleepyAssets.GetSound("snd/1.mp3");
			music2 = SleepyAssets.GetSound("snd/2.mp3");
			music3 = SleepyAssets.GetSound("snd/3.mp3");
			music4 = SleepyAssets.GetSound("snd/4.mp3");
			music5 = SleepyAssets.GetSound("snd/5.mp3");
			music6 = SleepyAssets.GetSound("snd/6.mp3");
			musicMenu1 = SleepyAssets.GetSound("snd/menu1.mp3");
			musicMenu2 = SleepyAssets.GetSound("snd/menu2.mp3");
			musicMenu3 = SleepyAssets.GetSound("snd/menu3.mp3");
			rate_perfect = SleepyAssets.GetSound("snd/rate perfect.wav");
			rate_great = SleepyAssets.GetSound("snd/rate great.wav");
			rate_good = SleepyAssets.GetSound("snd/rate good.wav");
			rate_ok = SleepyAssets.GetSound("snd/rate ok.wav");
			rate_sad = SleepyAssets.GetSound("snd/rate sad.wav");
			explosion = SleepyAssets.GetSound("snd/explosion.wav");

			soundTransformMain = new SoundTransform(0.05f, 0f);
			soundTransformReplay = new SoundTransform(0.02f, 0f);

			CreateChannels();
		}

		public static void update()
		{
			instance._update();
		}

		public static void changeMusic(string name = null)
		{
			if (name == null)
			{
				name = "menu1";
			}

			instance._changeMusic(name);
		}

		public static void pauseMusic()
		{
			instance._pauseMusic();
		}

		public static void playMusic()
		{
			instance._playMusic();
		}

		public static void skipToMusic(double position)
		{
			instance._skipToMusic(position);
		}

		public static void playSound(string rate, bool main, float panning = 0f)
		{
			instance._playSound(rate, main, panning);
		}

		/// <summary>
		/// The song clock, in milliseconds from the start of the song, or false when no song is
		/// playing and GameManager should accumulate frame deltas instead.
		/// </summary>
		public static bool TryGetSyncedTime(out int ms)
		{
			if (instance == null)
			{
				ms = 0;
				return false;
			}

			return instance._tryGetSyncedTime(out ms);
		}

		public void _update()
		{
			soundRhythmCount = GameManager.time % (GameManager.rhythm * 0.25f);
			if (soundRhythmCount < GameManager.rhythm * 0.25f / 2f && soundRhythmCountOld > GameManager.rhythm * 0.25f / 2f)
			{
				if (soundQueue.Count > 0)
				{
					// An explosion (a miss) is only heard when nothing else is in the batch -
					// it would otherwise talk over the hit sounds it shares a beat with.
					bool anyNonExplosion = false;
					for (int i = 0; i < soundQueue.Count; i++)
					{
						if (soundQueue[i].sound != explosion)
						{
							anyNonExplosion = true;
							break;
						}
					}

					for (int i = 0; i < soundQueue.Count; i++)
					{
						if (soundQueue[i].sound != explosion || !anyNonExplosion)
						{
							soundQueue[i].soundTransform.pan = soundQueue[i].panning;
							Play(soundQueue[i].sound, soundQueue[i].soundTransform);
						}
					}

					soundQueue.Clear();
				}
			}

			soundRhythmCountOld = soundRhythmCount;
		}

		public void _skipToMusic(double position)
		{
			_pauseMusic();
			this.position = position;
		}

		public void _playSound(string rate, bool main, float panning = 0f)
		{
			SoundTransform transform = main ? soundTransformMain : soundTransformReplay;

			switch (rate)
			{
				case "perfect": soundQueue.Add(new SoundSpec(rate_perfect, transform, panning)); break;
				case "great": soundQueue.Add(new SoundSpec(rate_great, transform, panning)); break;
				case "good": soundQueue.Add(new SoundSpec(rate_good, transform, panning)); break;
				case "ok": soundQueue.Add(new SoundSpec(rate_ok, transform, panning)); break;
				case "sad": soundQueue.Add(new SoundSpec(rate_sad, transform, panning)); break;
				case "explosion": soundQueue.Add(new SoundSpec(explosion, transform, panning)); break;
			}
		}

		public void _playMusic()
		{
			if (currentMusic == null)
			{
				Debug.Log("no music selected");
				return;
			}

			playing = true;
			channelMusic.clip = currentMusic;
			// position is milliseconds (_pauseMusic stores GameManager.time in it);
			// AudioSource.time is seconds, and seeking past the end throws.
			channelMusic.time = Mathf.Clamp((float)(position / 1000.0), 0f, Mathf.Max(0f, currentMusic.length - 0.01f));
			channelMusic.Play();
			syncPrimed = false;
		}

		public void _pauseMusic()
		{
			if (playing && channelMusic != null)
			{
				playing = false;
				position = GameManager.time;
				channelMusic.Stop();
			}
		}

		public void _changeMusic(string name)
		{
			if (playing)
			{
				_pauseMusic();
			}

			syncsGameTime = false;
			switch (name)
			{
				case "menu1":
					currentMusic = musicMenu1;
					GameManager.rhythm = 500;
					break;
				case "menu2":
					currentMusic = musicMenu2;
					GameManager.rhythm = 500;
					break;
				case "menu3":
					currentMusic = musicMenu3;
					GameManager.rhythm = 400;
					break;
				case "1":
					currentMusic = music1;
					GameManager.rhythm = 500;
					Scene.numberOfSections = 12;
					syncsGameTime = true;
					break;
				case "2":
					currentMusic = music2;
					GameManager.rhythm = 556;
					Scene.numberOfSections = 11;
					syncsGameTime = true;
					break;
				case "3":
					currentMusic = music3;
					GameManager.rhythm = 526;
					Scene.numberOfSections = 10;
					syncsGameTime = true;
					break;
				case "4":
					currentMusic = music4;
					GameManager.rhythm = 500;
					Scene.numberOfSections = 10;
					syncsGameTime = true;
					break;
				case "5":
					currentMusic = music5;
					GameManager.rhythm = 577;
					Scene.numberOfSections = 9;
					syncsGameTime = true;
					break;
				case "6":
					currentMusic = music6;
					GameManager.rhythm = 606;
					Scene.numberOfSections = 7;
					syncsGameTime = true;
					break;
			}

			position = 0.0;
			GameManager.time = 0;
			syncPrimed = false;
		}

		public bool _tryGetSyncedTime(out int ms)
		{
			ms = 0;
			if (!syncsGameTime || channelMusic == null || channelMusic.clip == null || !channelMusic.isPlaying)
			{
				syncPrimed = false;
				return false;
			}

			double raw = channelMusic.timeSamples / (double)channelMusic.clip.frequency * 1000.0;
			if (!syncPrimed)
			{
				syncPrimed = true;
				syncedMs = raw;
			}
			else
			{
				syncedMs += Time.unscaledDeltaTime * 1000.0;
				if (Math.Abs(raw - syncedMs) > SyncResyncThresholdMs)
				{
					syncedMs = raw;
				}
				else
				{
					syncedMs += (raw - syncedMs) * SyncCorrectionRate;
				}
			}

			ms = (int)syncedMs;
			return true;
		}

		private void CreateChannels()
		{
			// Under the stage rather than the display-list root: SleepyStage's render pass
			// walks the root, and this has nothing to draw. It dies with the scene, so
			// leaving the game stops the music.
			GameObject host = new GameObject("SoundManager");
			if (SleepyStage.Instance != null)
			{
				host.transform.SetParent(SleepyStage.Instance.transform, false);
			}

			channelMusic = host.AddComponent<AudioSource>();
			channelMusic.playOnAwake = false;
			channelMusic.loop = false;
			channelMusic.spatialBlend = 0f;

			sfxVoices = new AudioSource[SfxVoices];
			for (int i = 0; i < SfxVoices; i++)
			{
				AudioSource voice = host.AddComponent<AudioSource>();
				voice.playOnAwake = false;
				voice.loop = false;
				voice.spatialBlend = 0f;
				sfxVoices[i] = voice;
			}
		}

		/// <summary>
		/// Flash handed every sound its own channel; here a small round-robin of AudioSources
		/// stands in, because panning is a property of the source rather than of the play call.
		/// </summary>
		private void Play(AudioClip clip, SoundTransform transform)
		{
			if (clip == null)
			{
				return;
			}

			AudioSource voice = sfxVoices[nextSfxVoice];
			nextSfxVoice = (nextSfxVoice + 1) % sfxVoices.Length;

			voice.Stop();
			voice.clip = clip;
			voice.volume = transform.volume;
			voice.panStereo = transform.pan;
			voice.Play();
		}
	}
}

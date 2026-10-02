using UnityEngine;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// org.flixel.FlxSound: one playing sound, over a Unity AudioSource.
	///
	/// What the game relies on: volume it can turn down over time (the music fade is a
	/// timer subtracting 0.1), FlxG.volume scaling everything, and survive - music carries
	/// on across a change of level while ordinary sounds are cut off by it.
	/// </summary>
	public class FlxSound
	{
		public bool survive;
		public bool autoDestroy;

		protected double _volume;
		protected bool _looped;

		private AudioSource _source;
		private bool _played;

		public FlxSound()
		{
			_volume = 1.0;
			_looped = false;
			survive = false;
			autoDestroy = false;
		}

		public FlxSound loadEmbedded(string EmbeddedSound, bool Looped = false, bool AutoDestroy = false)
		{
			stop();
			if (_source == null)
			{
				_source = FlxGame.NewAudioSource();
			}

			_source.clip = FlxAssets.GetSound(EmbeddedSound);
			_source.loop = Looped;
			_looped = Looped;
			autoDestroy = AutoDestroy;
			updateTransform();
			return this;
		}

		public void play(bool ForceRestart = false)
		{
			if (_source == null || _source.clip == null)
			{
				return;
			}

			if (_source.isPlaying && !ForceRestart)
			{
				return;
			}

			_source.Play();
			_played = true;
		}

		public void pause()
		{
			if (_source != null)
			{
				_source.Pause();
			}
		}

		public void stop()
		{
			if (_source != null)
			{
				_source.Stop();
			}
		}

		public void destroy()
		{
			if (_source != null)
			{
				_source.Stop();
				Object.Destroy(_source.gameObject);
				_source = null;
			}
		}

		public bool active
		{
			get { return _source != null && _source.isPlaying; }
		}

		/// Whether this sound has anything left to do; a finished one-shot is reclaimed.
		public bool done
		{
			get { return _source == null || (_played && !_looped && !_source.isPlaying); }
		}

		public double volume
		{
			get { return _volume; }
			set
			{
				_volume = value;
				if (_volume < 0)
				{
					_volume = 0;
				}
				else if (_volume > 1)
				{
					_volume = 1;
				}

				updateTransform();
			}
		}

		internal void updateTransform()
		{
			if (_source != null)
			{
				_source.volume = (float)((FlxG.mute ? 0 : 1) * FlxG.volume * _volume);
			}
		}
	}
}

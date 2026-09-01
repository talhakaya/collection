using UnityEngine;

namespace Games.SleepyTime
{
	/// One queued sound effect, waiting for the next quarter-beat. Ported from SoundSpec.as.
	public class SoundSpec
	{
		public AudioClip sound;
		public SoundTransform soundTransform;
		public float panning;

		public SoundSpec(AudioClip sound, SoundTransform soundTransform, float panning = 0f)
		{
			this.sound = sound;
			this.soundTransform = soundTransform;
			this.panning = panning;
		}
	}

	/// <summary>
	/// flash.media.SoundTransform, reduced to what the game uses. Mutable and shared, because
	/// the original is: SoundManager keeps two of these and writes pan into whichever one a
	/// queued sound points at, immediately before playing it.
	/// </summary>
	public class SoundTransform
	{
		public float volume;
		public float pan;

		public SoundTransform(float volume, float pan)
		{
			this.volume = volume;
			this.pan = pan;
		}
	}
}

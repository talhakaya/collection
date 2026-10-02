namespace Games.WhereLostOnesGo
{
	/// <summary>
	/// The two sounds. Ported from SoundManager.as.
	///
	/// The beach plays from the start, fades out over about eight seconds once the zoom
	/// begins, and fades back in when the fall ends. The music plays once, from the jump.
	/// </summary>
	public class SoundManager : Sprite
	{
		private static FlashSound soundBeach;
		private static SoundChannel soundChannelBeach;
		private static SoundTransform soundTransformBeach;
		private static bool soundVolumeUp;
		public static SoundManager instance;
		public static string music = "music";
		public static string beach = "beach";

		public SoundManager()
		{
			instance = this;
		}

		public static void enterFrameHandler()
		{
			soundChannelBeach.soundTransform = soundTransformBeach;
			if (!soundVolumeUp)
			{
				soundTransformBeach.volume -= 0.004;
				if (soundTransformBeach.volume <= 0)
				{
					instance.removeEventListener(Event.ENTER_FRAME, enterFrameHandler);
				}
			}
			else
			{
				soundTransformBeach.volume += 0.004;
				if (soundTransformBeach.volume >= 1)
				{
					instance.removeEventListener(Event.ENTER_FRAME, enterFrameHandler);
				}
			}
		}

		public static void playBeach()
		{
			soundBeach = new FlashSound(beach);
			soundTransformBeach = new SoundTransform(1, 0);
			soundChannelBeach = soundBeach.play(0, 99);
		}

		public static void stopBeach()
		{
			soundVolumeUp = false;
			instance.addEventListener(Event.ENTER_FRAME, enterFrameHandler);
		}

		public static void continueBeach()
		{
			soundVolumeUp = true;
			soundTransformBeach = new SoundTransform(0, 0);
			instance.addEventListener(Event.ENTER_FRAME, enterFrameHandler);
		}

		public static void playMusic()
		{
			var sound = new FlashSound(music);
			sound.play();
		}
	}
}

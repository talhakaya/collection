using System;

namespace Games.SleepyTime
{
	/// <summary>
	/// One note. Ported from Beat.as.
	///
	/// timeInSong and lengthOfPress are milliseconds; timing is how far the player was from
	/// the note when they pressed, signed, and timingMax is the whole hit window - 200 ms
	/// either side, for both the press and the release of a hold.
	///
	/// hit() and hitEnd() are one-shot: once a beat has been hit they return false forever,
	/// which is what stops a single held press from scoring the same note repeatedly.
	/// </summary>
	public class Beat
	{
		public static int timingMax = 200;

		public int timing;
		public int timeInSong;
		public int lengthOfPress;
		public bool isHitEnd;
		public bool isHit;

		public Beat(int timeInSong = 0, int lengthOfPress = 0, bool isHit = false, bool isHitEnd = false, int timing = 1000)
		{
			this.timeInSong = timeInSong;
			this.timing = timing;
			this.isHit = isHit;
			this.lengthOfPress = lengthOfPress;
			this.isHitEnd = isHitEnd;
		}

		public bool hitEnd()
		{
			if (!isHitEnd)
			{
				int now = GameManager.time;
				timing = timeInSong + lengthOfPress - now;
				isHitEnd = Math.Abs(timing) <= timingMax;
				return isHitEnd;
			}

			return false;
		}

		public bool hit()
		{
			if (!isHit)
			{
				int now = GameManager.time;
				timing = timeInSong - now;
				isHit = Math.Abs(timing) <= timingMax;
				return isHit;
			}

			return false;
		}

		public int getRelativePositionToEnd()
		{
			return timeInSong + lengthOfPress - GameManager.time;
		}

		public int getRelativePosition()
		{
			return timeInSong - GameManager.time;
		}
	}
}

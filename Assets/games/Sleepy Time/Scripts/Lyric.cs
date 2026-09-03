namespace Games.SleepyTime
{
	/// <summary>
	/// One line of lyrics. Ported from Lyric.as.
	///
	/// time is in beats, not milliseconds - putInLyrics accumulates per-line deltas into
	/// absolute beats, and updateLyrics compares against GameManager.time / rhythm.
	/// </summary>
	public class Lyric
	{
		public float time;
		public string text;

		public Lyric(float time = 0f, string text = null)
		{
			this.time = time;
			this.text = text;
		}
	}
}

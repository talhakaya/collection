namespace Games.SleepyTime
{
	/// <summary>
	/// Scene.as is the whole gameplay loop and lands with it. What's here is the static state
	/// SoundManager.changeMusic writes on every song change, ported early so that switch can
	/// be transcribed as written.
	///
	/// These are static and shared by every live Scene, including the ghost replays running
	/// alongside the active one - so all of them use the current song's section count. That's
	/// the original's behaviour, and it's what makes the ghosts scroll at the active song's
	/// speed rather than their own.
	/// </summary>
	public class Scene
	{
		public static float lengthOfSection;
		public static float PenisHeightMax;
		public static float LineYRatioToScene = 0.3f;
		public static int numberOfSections = 5;
		public static int finishingScoreTableConst = 3000;
	}
}

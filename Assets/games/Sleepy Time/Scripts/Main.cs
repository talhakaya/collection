namespace Games.SleepyTime
{
	/// <summary>
	/// What survives of Main.as: the stage dimensions the rest of the game measures itself
	/// against. The original read them off the live Flash stage, but the SWF was fixed at
	/// 800x450 and the source assumes it anyway - GameManager.enterFrameHandler divides by
	/// literal 800/450, and GriddyBackground hardcodes (800 - 45 * 9) / 2 - so they're
	/// constants here.
	///
	/// Kept under the original name so call sites transcribe verbatim (Main.stageWidth).
	/// </summary>
	public static class Main
	{
		public const int stageWidth = 800;
		public const int stageHeight = 450;
	}
}

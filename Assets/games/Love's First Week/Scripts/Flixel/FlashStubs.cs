namespace Games.LovesFirstWeek
{
	// The few pieces of Flash itself that the game's code names. They exist so that the
	// transcribed code can stay as it was written; none of them does anything.

	/// flash.display.Stage, as reached through FlxG.stage. The game sets displayState when
	/// toggling full screen - code that is switched off in the source (isFullscreenAvailable
	/// is false) - and reads width once, to size background stripes.
	public class FlashStage
	{
		public string displayState = StageDisplayState.NORMAL;

		public int width
		{
			get { return FlxG.width; }
		}
	}

	/// flash.display.StageDisplayState.
	public static class StageDisplayState
	{
		public const string NORMAL = "normal";
		public const string FULL_SCREEN = "fullScreen";
	}

	/// flash.ui.Mouse. Only called from the source's level editor, which is switched off.
	public static class Mouse
	{
		public static void show()
		{
		}
	}
}

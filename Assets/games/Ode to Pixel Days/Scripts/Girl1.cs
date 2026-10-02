namespace Games.OdeToPixelDays
{
	/// <summary>
	/// One of the two girls talking to the cheerleader in the last scene. Ported from Girl1.as.
	/// </summary>
	public class Girl1 : FlxSprite
	{
		private static string S_ = "Girl1_S_";

		public Girl1(double _x, double _y)
		{
			x = _x;
			y = _y;
			loadGraphic(S_, true, false, 32, 32, false);
			addAnimation("talk", new[] { 1, 1, 1, 1, 0, 2, 0, 2, 0, 0, 2, 0, 2, 2, 0, 1, 1, 1, 1 }, 3, true);
			play("talk");
		}
	}
}

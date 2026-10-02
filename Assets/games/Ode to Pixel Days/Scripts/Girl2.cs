namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The other girl talking to the cheerleader in the last scene. Ported from Girl2.as.
	/// </summary>
	public class Girl2 : FlxSprite
	{
		private static string S_ = "Girl2_S_";

		public Girl2(double _x, double _y)
		{
			x = _x;
			y = _y;
			loadGraphic(S_, true, false, 32, 32, false);
			addAnimation("talk", new[] { 2, 2, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 }, 3, true);
			play("talk");
		}
	}
}

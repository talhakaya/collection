namespace Games.OdeToPixelDays
{
	/// <summary>
	/// A panel of the wire fence in the last scene. Ported from Tel.as.
	/// </summary>
	public class Tel : FlxSprite
	{
		private static string S_ = "Tel_S_";

		public Tel(double _x, double _y, FlxPoint _scale)
		{
			loadGraphic(S_, false, false, 64, 64, false);
			x = _x;
			y = _y;
			scale = _scale;
		}
	}
}

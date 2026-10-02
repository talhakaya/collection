namespace Games.OdeToPixelDays
{
	/// <summary>
	/// A door hanging the wrong way up - decoration. Ported from Door2UpsideDown.as.
	/// </summary>
	public class Door2UpsideDown : FlxSprite
	{
		private static string S_kapi = "Door2UpsideDown_S_kapi";

		public Door2UpsideDown(double _X, double _Y, FlxPoint _scale)
		{
			scale = _scale;
			x = _X;
			y = _Y;
			loadGraphic(S_kapi, true, false, 5, 5, false);
			addAnimation("closed", new[] { 6 }, 1, false);
			play("closed");
			width = 5 * scale.x;
			height = 5 * scale.x;
			centerOffsets();
		}
	}
}

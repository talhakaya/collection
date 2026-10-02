namespace Games.LovesFirstWeek
{
	/// <summary>
	/// The door at the end of a level. Ported from Kapi.as.
	/// </summary>
	public class Kapi : FlxSprite
	{
		private static string img = "Kapi_img";

		public Kapi(double _X, double _Y)
		{
			x = _X;
			y = _Y;
			loadGraphic(img, true, false, 64, 64, false);
			width = 32;
			height = 48;
			offset.x = 16;
			offset.y = 16;
			addAnimation("0", new[] { 0 }, 0, false);
			addAnimation("open", new[] { 1, 2, 3, 4 }, 8, false);
			play("0");
		}

		public void open()
		{
			play("open");
		}
	}
}

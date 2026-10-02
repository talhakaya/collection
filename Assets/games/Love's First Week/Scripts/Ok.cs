namespace Games.LovesFirstWeek
{
	/// <summary>
	/// An arrow tile: whatever passes over it is sent off in its direction. Ported from Ok.as.
	/// </summary>
	public class Ok : FlxSprite
	{
		private static string img = "Ok_img";

		public string dir;

		public Ok(string direction, double _X, double _Y)
		{
			x = _X;
			y = _Y;
			loadGraphic(img, false, false, 16, 16, false);
			addAnimation("up", new[] { 0, 1, 2, 3, 2, 1 }, 12, true);
			addAnimation("right", new[] { 4, 5, 6, 7, 6, 5 }, 12, true);
			addAnimation("down", new[] { 8, 9, 10, 11, 10, 9 }, 12, true);
			addAnimation("left", new[] { 12, 13, 14, 15, 14, 13 }, 12, true);
			play(direction);
			dir = direction;
			width = 1;
			height = 1;
			offset.x = offset.y = 8;
		}
	}
}

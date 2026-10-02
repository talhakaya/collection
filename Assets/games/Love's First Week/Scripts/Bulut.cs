namespace Games.LovesFirstWeek
{
	/// <summary>
	/// A cloud drifting across the background. Ported from Bulut.as.
	/// </summary>
	public class Bulut : FlxSprite
	{
		private static string img = "Bulut_img";

		public Bulut(string no, double _X, double _Y)
		{
			x = _X;
			y = _Y;
			loadGraphic(img, false, false, 32, 32, false);
			addAnimation("0", new[] { 0 }, 0, false);
			addAnimation("1", new[] { 1 }, 0, false);
			addAnimation("2", new[] { 2 }, 0, false);
			addAnimation("3", new[] { 3 }, 0, false);
			play(no);
			scrollFactor.x = scrollFactor.y = 0;
			velocity.x = 10 * FlxG.random();
			scale.x = 4;
			scale.y = 2;
		}

		public override void update()
		{
			base.update();
			if (x > 17 * 32)
			{
				x = -100;
			}
		}
	}
}

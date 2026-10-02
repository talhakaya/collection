namespace Games.LovesFirstWeek
{
	/// <summary>
	/// A tree in the background. Ported from Agac.as.
	/// </summary>
	public class Agac : FlxSprite
	{
		private static string img = "Agac_img";

		public Agac(string no, double _X, double _Y)
		{
			x = _X;
			y = _Y;
			loadGraphic(img, false, false, 50, 125, false);
			addAnimation("0", new[] { 0 }, 0, false);
			addAnimation("1", new[] { 1 }, 0, false);
			addAnimation("2", new[] { 2 }, 0, false);
			addAnimation("3", new[] { 3 }, 0, false);
			addAnimation("4", new[] { 4 }, 0, false);
			play(no);
			scrollFactor.x = 0.2 + 0.2 * FlxG.random();
			scrollFactor.y = 1;
		}
	}
}

namespace Games.LovesFirstWeek
{
	/// <summary>
	/// One of the two faces beside the dialogue box, talking or not. Ported from Kafa.as.
	/// </summary>
	public class Kafa : FlxSprite
	{
		private static string img1 = "Kafa_img1";
		private static string img2 = "Kafa_img2";

		public Kafa(bool isNaz)
		{
			if (isNaz)
			{
				loadGraphic(img1, true, false, 20, 17);
				x = 3 + 18;
				y = 198 + 20;
			}
			else
			{
				loadGraphic(img2, true, false, 20, 17);
				x = 393 + 18;
				y = 198 + 20;
			}

			addAnimation("idle", new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 3, 3, 3, 3, 3, 3, 3, 2, 4, 4, 4, 4, 4, 4, 4, 2, 0, 0, 0, 0, 0, 0, 2 }, 6, true);
			addAnimation("talk", new[] { 1, 1, 0, 0, 1, 1, 2, 3, 3, 1, 1, 4, 4, 2 }, 6, true);
			alpha = 0;
			scale.x = scale.y = 3;
			scrollFactor.x = scrollFactor.y = 0;
			play("idle");
		}
	}
}

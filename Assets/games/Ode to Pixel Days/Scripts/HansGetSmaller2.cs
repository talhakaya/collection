namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The Hans that fades in at the end of the shrinking animation, drawn from the next
	/// size's sheet. Ported from HansGetSmaller2.as.
	/// </summary>
	public class HansGetSmaller2 : FlxSprite
	{
		private static string S_hans = "HansGetSmaller2_S_hans";
		private static string S_hans16 = "HansGetSmaller2_S_hans16";
		private static string S_hans8 = "HansGetSmaller2_S_hans8";
		private static string S_hans4 = "HansGetSmaller2_S_hans4";
		private static string S_hans2 = "HansGetSmaller2_S_hans2";
		private static string S_hans1 = "HansGetSmaller2_S_hans1";

		public bool goToNextLevel;
		private bool animate;
		public bool smaller;
		public int count;
		private int countSlower;
		public FlxPoint initialScale;
		public FlxPoint visualScale;

		public HansGetSmaller2(double _X, double _Y, FlxPoint _scale, bool _smaller)
		{
			x = _X;
			y = _Y;
			if (_smaller)
			{
				visualScale = new FlxPoint(_scale.x * 6, _scale.x * 6);
			}
			else
			{
				visualScale = new FlxPoint(_scale.x * 3, _scale.x * 3);
			}

			scale = visualScale;
			smaller = _smaller;
			initialScale = _scale;
			count = 0;
			countSlower = 0;
			alpha = 0;
			scale = visualScale;
			if (smaller)
			{
				if (initialScale.x == 1)
				{
					loadGraphic(S_hans16, false, true, 10, 16, false);
				}
				else if (initialScale.x == 2)
				{
					loadGraphic(S_hans8, false, true, 5, 8, false);
				}
				else if (initialScale.x == 4)
				{
					loadGraphic(S_hans4, false, true, 4, 4, false);
				}
				else if (initialScale.x == 8)
				{
					loadGraphic(S_hans2, false, true, 1, 2, false);
				}
			}
			else if (initialScale.x == 2)
			{
				loadGraphic(S_hans, false, true, 16, 32, false);
			}
			else if (initialScale.x == 4)
			{
				loadGraphic(S_hans16, false, true, 10, 16, false);
			}
			else if (initialScale.x == 8)
			{
				loadGraphic(S_hans8, false, true, 5, 8, false);
			}
			else if (initialScale.x == 16)
			{
				loadGraphic(S_hans4, false, false, 4, 4, false);
			}

			if (smaller)
			{
				height = 96;
				width = 48;
			}
			else
			{
				height = 192;
				width = 96;
			}

			centerOffsets();
		}

		public override void update()
		{
			base.update();
			if (alpha < 1)
			{
				++countSlower;
				if (countSlower > 1)
				{
					countSlower = 0;
					alpha += 0.05;
				}
			}
		}
	}
}

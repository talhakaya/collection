namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The big Hans of the shrinking (or growing) animation between worlds. Ported from
	/// HansGetSmaller.as.
	///
	/// Over a hundred counts it scales towards the next size, fading out at the end as
	/// HansGetSmaller2 - the same Hans drawn from the next sheet - fades in over it. Enter or
	/// the action button skips the animation.
	/// </summary>
	public class HansGetSmaller : FlxSprite
	{
		private static string S_hans = "HansGetSmaller_S_hans";
		private static string S_hans16 = "HansGetSmaller_S_hans16";
		private static string S_hans8 = "HansGetSmaller_S_hans8";
		private static string S_hans4 = "HansGetSmaller_S_hans4";
		private static string S_hans2 = "HansGetSmaller_S_hans2";
		private static string S_hans1 = "HansGetSmaller_S_hans1";

		public bool goToNextLevel;
		public bool putHansGetSmaller2;
		private bool animate;
		public bool smaller;
		public int count;
		private int countSlower;
		public FlxPoint initialScale;
		public FlxPoint visualScale;

		public HansGetSmaller(double _X, double _Y, FlxPoint _scale, bool _smaller)
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
			if (initialScale.x == 1)
			{
				loadGraphic(S_hans, false, true, 16, 32, false);
			}
			else if (initialScale.x == 2)
			{
				loadGraphic(S_hans16, false, true, 10, 16, false);
			}
			else if (initialScale.x == 4)
			{
				loadGraphic(S_hans8, false, true, 5, 8, false);
			}
			else if (initialScale.x == 8)
			{
				loadGraphic(S_hans4, false, true, 4, 4, false);
			}
			else if (initialScale.x == 16)
			{
				loadGraphic(S_hans2, false, true, 1, 2, false);
			}

			if (smaller)
			{
				height = 192;
				width = 96;
			}
			else
			{
				height = 96;
				width = 48;
			}

			centerOffsets();
		}

		public override void update()
		{
			base.update();
			if (FlxG.keys.justPressed("ENTER"))
			{
				goToNextLevel = true;
			}

			if (FlxG.keys.justPressed("SPACE"))
			{
				goToNextLevel = true;
			}

			if (animate)
			{
				++countSlower;
				if (countSlower > 1)
				{
					++count;
					getSmaller();
					countSlower = 0;
				}
			}
		}

		public void getSmaller()
		{
			if (count < 101)
			{
				if (smaller)
				{
					scale = new FlxPoint(visualScale.x * (1 - count * 0.005), visualScale.y * (1 - count * 0.005));
				}
				else
				{
					scale = new FlxPoint(visualScale.x * (1 + count * 0.005), visualScale.y * (1 + count * 0.005));
				}

				if (count == 90)
				{
					putHansGetSmaller2 = true;
				}
				else if (count > 90)
				{
					alpha -= 0.1;
					putHansGetSmaller2 = false;
				}
			}
			else if (count == 200)
			{
				goToNextLevel = true;
			}
		}

		public void startGettingSmaller()
		{
			animate = true;
		}
	}
}

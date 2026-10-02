namespace Games.OdeToPixelDays
{
	/// <summary>
	/// One 64x64 tile of the wall behind a level. Ported from Bg.as.
	///
	/// Level.addBg tiles the whole level with these, then dresses individual ones up by
	/// calling the method named for the detail it wants there - a torch, a window, the sun
	/// in four quarters. Several details pick one of four variants at random, so no two
	/// visits to a level look quite the same.
	/// </summary>
	public class Bg : FlxSprite
	{
		private static string S_1 = "Bg_S_1";
		private static string S_2 = "Bg_S_2";
		private static string S_3 = "Bg_S_3";
		private static string S_4 = "Bg_S_4";
		private static string S_5 = "Bg_S_5";
		private static string S_6 = "Bg_S_6";
		private static string S_7 = "Bg_S_7";
		private static string S_9 = "Bg_S_9";
		private static string S_10 = "Bg_S_10";
		private static string S_11 = "Bg_S_11";
		private static string S_12 = "Bg_S_12";
		private static string S_13 = "Bg_S_13";
		private static string S_14 = "Bg_S_14";
		private static string S_15 = "Bg_S_15";
		private static string S_16 = "Bg_S_16";
		private static string S_hans = "Bg_S_hans";
		private static string S_17 = "Bg_S_17";

		public bool birdFlied;
		public bool windEsti;
		public FlxTimer timer;
		public bool timerSet;

		public Bg(double _X, double _Y, FlxPoint _scale)
		{
			x = _X;
			y = _Y;
			scale = _scale;
			loadGraphic(S_1, false, false, 64, 64, false);
			timerSet = false;
		}

		public void torch()
		{
			loadGraphic(S_2, true, false, 64, 64, false);
			double random = FlxG.random();
			if (random < 0.25)
			{
				addAnimation("0", new[] { 0, 0, 0, 0, 1, 1, 1, 1 }, 9, true);
				play("0");
			}
			else if (random < 0.5)
			{
				addAnimation("1", new[] { 0, 0, 0, 1, 1, 1, 1, 0 }, 10, true);
				play("1");
			}
			else if (random < 0.75)
			{
				addAnimation("2", new[] { 0, 0, 1, 1, 1, 1, 0, 0 }, 11, true);
				play("2");
			}
			else
			{
				addAnimation("3", new[] { 0, 1, 1, 1, 1, 0, 0, 0 }, 12, true);
				play("3");
			}
		}

		public void windowPalm()
		{
			loadGraphic(S_4, true, false, 64, 64, false);
			addAnimation("4", new[] { 0, 1, 2, 1 }, 1, true);
			play("4");
		}

		public void damaged()
		{
			loadGraphic(S_3, true, false, 64, 64, false);
			oneOfFour();
		}

		public void sun00()
		{
			loadGraphic(S_5, true, false, 64, 64, false);
			addAnimation("00", new[] { 0, 4 }, 0.5, true);
			play("00");
		}

		public void sun01()
		{
			loadGraphic(S_5, true, false, 64, 64, false);
			addAnimation("01", new[] { 2, 6 }, 0.5, true);
			play("01");
		}

		public void sun10()
		{
			loadGraphic(S_5, true, false, 64, 64, false);
			addAnimation("10", new[] { 1, 5 }, 0.5, true);
			play("10");
		}

		public void sun11()
		{
			loadGraphic(S_5, true, false, 64, 64, false);
			addAnimation("11", new[] { 3, 7 }, 0.5, true);
			play("11");
		}

		public void bird()
		{
			loadGraphic(S_6, true, false, 64, 64, false);
			addAnimation("bird", new[] { 0, 1, 2, 3, 4, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7 }, 2, true);
			play("bird");
		}

		public void windowSmall()
		{
			loadGraphic(S_7, true, false, 64, 64, false);
			oneOfFour();
		}

		public void bird2()
		{
			loadGraphic(S_15, true, false, 64, 64, false);
			addAnimation("0", new[] { 0 }, 0, false);
			addAnimation("fly", new[] { 1, 2, 3, 4, 5 }, 3, false);
			birdFlied = false;
			play("0");
		}

		public void windowBig00()
		{
			loadGraphic(S_13, true, false, 64, 64, false);
			addAnimation("00", new[] { 0 }, 0, false);
			play("00");
		}

		public void windowBig02()
		{
			loadGraphic(S_13, true, false, 64, 64, false);
			addAnimation("02", new[] { 2 }, 0, false);
			play("02");
		}

		public void windowBig20()
		{
			loadGraphic(S_13, true, false, 64, 64, false);
			addAnimation("20", new[] { 1 }, 0, false);
			play("20");
		}

		public void windowBig22()
		{
			loadGraphic(S_13, true, false, 64, 64, false);
			addAnimation("22", new[] { 3 }, 0, false);
			play("22");
		}

		public void windowBig10()
		{
			loadGraphic(S_11, true, false, 64, 64, false);
			oneOfFour();
		}

		public void windowBig01()
		{
			loadGraphic(S_10, true, false, 64, 64, false);
			oneOfFour();
		}

		public void windowBig12()
		{
			loadGraphic(S_9, true, false, 64, 64, false);
			oneOfFour();
		}

		public void windowBig21()
		{
			loadGraphic(S_12, true, false, 64, 64, false);
			oneOfFour();
		}

		public void windowBig11()
		{
			loadGraphic(S_14, true, false, 64, 64, false);
			oneOfFour();
		}

		public void greyCloud()
		{
			loadGraphic(S_17, true, false, 64, 64, false);
			oneOfFour();
		}

		public void corner()
		{
			loadGraphic(S_16, true, false, 64, 64, false);
			addAnimation("0", new[] { 0 }, 1, true);
			play("0");
		}

		public void wind()
		{
			if (!timerSet)
			{
				timerSet = true;
				timer = new FlxTimer();
			}

			windEsti = true;
			timer.start(4, 1, windEsmedi);
		}

		private void windEsmedi(FlxTimer a)
		{
			windEsti = false;
		}

		public void hans()
		{
			loadGraphic(S_hans, true, false, 64, 64, false);
			addAnimation("0", new[] { 0 }, 1, true);
			play("0");
		}

		/// The block the source repeats in eight of the methods above: one of the sheet's
		/// first four frames, chosen at random, as a still.
		private void oneOfFour()
		{
			double random = FlxG.random();
			if (random < 0.25)
			{
				addAnimation("0", new[] { 0 }, 0, false);
				play("0");
			}
			else if (random < 0.5)
			{
				addAnimation("1", new[] { 1 }, 0, false);
				play("1");
			}
			else if (random < 0.75)
			{
				addAnimation("2", new[] { 2 }, 0, false);
				play("2");
			}
			else
			{
				addAnimation("3", new[] { 3 }, 0, false);
				play("3");
			}
		}
	}
}

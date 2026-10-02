using System;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The interlude between worlds: Hans, large, shrinking to the next size - or, on the
	/// way back, growing. Ported from levels/AnimationHans.as.
	///
	/// It is a state of its own rather than a Level, and it knows which level follows each
	/// size.
	/// </summary>
	public class AnimationHans : FlxState
	{
		private static string music = "AnimationHans_music";
		private static string music2 = "AnimationHans_music2";

		public HansGetSmaller hans;
		public HansGetSmaller2 hans2;
		public FlxPoint scale;
		public bool smaller;
		public TTimer timer1;
		public TTimer timer2;

		public AnimationHans(FlxPoint _scale, bool _smaller)
		{
			scale = _scale;
			smaller = _smaller;
		}

		public override void create()
		{
			int i = 0;
			int j = 0;
			base.create();
			if (FlxG.music != null)
			{
				FlxG.music.stop();
				FlxG.music = null;
			}

			if (smaller)
			{
				FlxG.play(music);
			}
			else
			{
				FlxG.play(music2);
			}

			FlxGroup bg = new FlxGroup();
			add(bg);
			if (smaller)
			{
				if (scale.x == 1)
				{
					FlxG.bgColor = 4289357414;
				}
				else if (scale.x == 2)
				{
					FlxG.bgColor = 4288900388;
				}
				else if (scale.x == 4)
				{
					FlxG.bgColor = 4285499319;
				}
				else if (scale.x == 8)
				{
					FlxG.bgColor = 4290199552;
				}
			}
			else if (scale.x == 2)
			{
				FlxG.bgColor = 4288900388;
			}
			else if (scale.x == 4)
			{
				FlxG.bgColor = 4285499319;
			}
			else if (scale.x == 8)
			{
				FlxG.bgColor = 4290199552;
			}
			else if (scale.x == 16)
			{
				FlxG.bgColor = 4281241856;
			}

			FlxSprite floor = new FlxSprite(0, 200);
			floor.makeGraphic(320, 80, 4278190080, false);
			add(floor);
			if (smaller)
			{
				hans = new HansGetSmaller(0, 0, scale, smaller);
			}
			else
			{
				hans = new HansGetSmaller(0, 0, scale, smaller);
			}

			add(hans);
			int a = (int)(Math.Ceiling(40 / 8 / hans.visualScale.x) + 1);
			int b = (int)(Math.Ceiling(30 / 8.0 / hans.visualScale.x) + 1);
			for (i = 0; i < a; i++)
			{
				for (j = 0; j < b; j++)
				{
					bg.add(new Bg(64 * hans.visualScale.x * i, 64 * hans.visualScale.x * j, hans.visualScale));
				}
			}

			timer1 = new TTimer();
			add(timer1);
			timer1.start(30);
		}

		public override void update()
		{
			base.update();
			hans.x = 160 - hans.width * 0.5;
			if (hans.count < 100)
			{
				if (smaller)
				{
					hans.y = 200 - hans.height + hans.count * 0.48;
				}
				else
				{
					hans.y = 200 - hans.height - hans.count * 0.24;
				}
			}

			if (hans.putHansGetSmaller2)
			{
				if (smaller)
				{
					hans2 = new HansGetSmaller2(160 - hans.width * 0.25, 200 - hans.height * 0.5, scale, smaller);
				}
				else
				{
					hans2 = new HansGetSmaller2(160 - hans.width, 200 - hans.height * 2, scale, smaller);
				}

				add(hans2);
			}

			if (hans.goToNextLevel)
			{
				if (smaller)
				{
					if (scale.x == 1)
					{
						FlxG.switchState(new Level5());
					}
					else if (scale.x == 2)
					{
						FlxG.switchState(new Level9());
					}
					else if (scale.x == 4)
					{
						FlxG.switchState(new Level13());
					}
					else if (scale.x == 8)
					{
						FlxG.switchState(new Level17());
					}
				}
				else if (scale.x == 16)
				{
					FlxG.switchState(new Level24());
				}
				else if (scale.x == 8)
				{
					FlxG.switchState(new Level31());
				}
				else if (scale.x == 4)
				{
					FlxG.switchState(new Level35());
				}
				else if (scale.x == 2)
				{
					FlxG.switchState(new Level41());
				}
			}

			if (timer1.complete)
			{
				hans.startGettingSmaller();
			}
		}
	}
}

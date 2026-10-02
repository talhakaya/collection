namespace Games.OdeToPixelDays
{
	/// <summary>
	/// Ported from levels/Level42.as.
	/// </summary>
	public class Level42 : Level
	{
		private static string S_tiles = "Level42_S_tiles";
		private static string music = "Level42_music";

		private bool makeClouds;
		private int count;
		private int count2;
		private int period;
		private const int noOfBirds = 50;
		private int birdsSent;
		private int hansSent;
		private const double birdsY = 360;
		private Hans hans2;
		private Hans hans4;
		private Hans hans8;
		private Hans hans16;
		private const double hansDistance = 48;
		private FlxSprite fadeToBlack;
		private bool hansLoaded;
		private bool hansAnimated;
		private bool gecis;

		public override void create()
		{
			noMusic = true;
			gameSave = new FlxSave();
			gameSave.bind("save");
			gameSave.data.level = 45;
			gecis = false;
			levelwidth = 40;
			levelheight = 1200;
			scale = new FlxPoint(1, 1);
			base.create();
			FlxG.bgColor = 4289357414;
			int[] data = new int[]
			{
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				16, 16, 15, 15, 15, 15, 15, 15, 15, 15, 13, 0, 7, 13, 0, 5, 0, 7, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				16, 16, 16, 16, 16, 16, 16, 16, 12, 16, 16, 15, 16, 16, 11, 12, 11, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				12, 12, 12, 12, 16, 12, 16, 14, 0, 8, 16, 16, 12, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 6, 0, 8, 16, 15, 16, 16, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 8, 15, 16, 16, 12, 12, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 4, 16, 12, 10, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 6, 0, 0, 7, 11, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 8, 15, 15, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, 40), S_tiles, 0, 0, FlxTilemap.AUTO);
			add(level);
			bgObjects.add(new Door(26, 24, true, scale));
			player = new Hans(32, 36, scale);
			add(player);
			narrators.add(new Narrator(160, 40, 200, 74, false));
			makeClouds = false;
			hansLoaded = false;
			hansAnimated = false;
			birdsSent = 0;
			hansSent = 0;
			count2 = 0;
			FlxG.camera.setBounds(0, 0, 320, 9600, true);
			FlxG.camera.follow(player, FlxCamera.STYLE_LOCKON);
			fadeToBlack = new FlxSprite(0, 9360);
			fadeToBlack.makeGraphic(320, 240, 4294967295, false);
			fadeToBlack.alpha = 0;
			add(fadeToBlack);
		}

		public override void update()
		{
			base.update();
			if (!gecis && player.y > 320)
			{
				FlxG.playMusic(music, 1);
				gecis = true;
			}

			if (!makeClouds)
			{
				if (player.y > 320 && player.y < 9600)
				{
					makeClouds = true;
					period = 10;
				}
			}
			else if (count == period)
			{
				if (FlxG.random() > 0.3)
				{
					bgObjects.add(new Cloud(player, false));
				}
				else
				{
					add(new Cloud(player, false));
				}

				count = 0;
				if (player.y < 2000)
				{
					period = 4;
					if (birdsSent == 0)
					{
						++birdsSent;
						birds(true);
					}
				}
				else if (player.y < 4000)
				{
					period = 1;
					if (birdsSent == 1)
					{
						++birdsSent;
						birds(false);
					}
				}
				else if (player.y < 6000)
				{
					period = 3;
					if (birdsSent == 2)
					{
						++birdsSent;
						birds(false);
					}
				}
				else if (player.y < 8000)
				{
					period = 2;
					if (birdsSent == 3)
					{
						++birdsSent;
						birds(true);
						birds(true);
					}
				}
				else if (player.y < 9600)
				{
					period = 4;
				}
				else
				{
					makeClouds = false;
				}

				if (player.y > 5000)
				{
					if (hansSent == 0)
					{
						++hansSent;
						hans2 = new Hans(player.x - hansDistance, player.y + 36, new FlxPoint(2, 2));
						hans2.lastLevel = true;
						hans2.jumpEnable = false;
						add(hans2);
					}

					hans2.x = player.x - hansDistance;
				}

				if (player.y > 5500)
				{
					if (hansSent == 1)
					{
						++hansSent;
						hans4 = new Hans(player.x + hansDistance - 8, player.y + 44, new FlxPoint(4, 4));
						hans4.lastLevel = true;
						hans4.jumpEnable = false;
						add(hans4);
					}

					hans4.x = player.x + hansDistance - 8;
				}

				if (player.y > 6500)
				{
					if (hansSent == 2)
					{
						++hansSent;
						hans8 = new Hans(player.x + hansDistance - 16, player.y + 92, new FlxPoint(8, 8));
						hans8.lastLevel = true;
						hans8.jumpEnable = false;
						add(hans8);
					}

					hans8.x = player.x + hansDistance - 16;
				}

				if (player.y > 7000)
				{
					if (hansSent == 3)
					{
						++hansSent;
						hans16 = new Hans(player.x - hansDistance + 20, player.y + 88, new FlxPoint(16, 16));
						hans16.lastLevel = true;
						hans16.jumpEnable = false;
						add(hans16);
					}

					hans16.x = player.x - hansDistance + 20;
				}
			}
			else
			{
				++count;
			}

			if (player.x < 0)
			{
				player.x = 0;
			}
			else if (player.x > 312)
			{
				player.x = 312;
			}

			++count2;
			if (count2 == 3)
			{
				count2 = 0;
			}

			if (!hansAnimated && player.y > 240)
			{
				if (!hansLoaded)
				{
					hansLoaded = true;
					player.gokyuzundenDus();
				}
				else if (player.y < 400)
				{
					player.fall0();
				}
				else if (player.y < 560)
				{
					player.fall1();
				}
				else if (player.y < 720)
				{
					player.fall2();
				}
				else
				{
					player.fall3();
				}
			}

			if (player.y > 9600 && fadeToBlack.alpha < 1)
			{
				fadeToBlack.alpha += 0.005;
				if (FlxG.music != null)
				{
					FlxG.music.volume -= 0.005;
				}
			}

			if (player.y > 11000)
			{
				nextLevel();
			}
		}

		public override void addBgDetail()
		{
			int i = 0;
			int j = 0;
			for (i = 0; i < 5; i++)
			{
				bgDetails[i][0] = "damaged";
				bgDetails[i][1] = "damaged";
				bgDetails[i][2] = "windowBig10";
				bgDetails[i][3] = "windowBig11";
			}

			for (j = 4; j < 151; j++)
			{
				for (i = 0; i < 5; i++)
				{
					bgDetails[i][j] = "windowBig11";
				}
			}

			bgDetails[4][0] = "windowBig01";
			bgDetails[4][1] = "windowBig01";
			bgDetails[4][2] = "corner";
		}

		public override void nextLevel()
		{
			FlxG.switchState(new Level43());
		}

		private void birds(bool onLeft)
		{
			int i = 0;
			if (onLeft)
			{
				for (i = 0; i < noOfBirds; i++)
				{
					if (FlxG.random() > 0.5)
					{
						add(new Bird(-100, player.y + birdsY));
					}
					else
					{
						bgObjects.add(new Bird(-100, player.y + birdsY));
					}
				}
			}
			else
			{
				for (i = 0; i < noOfBirds; i++)
				{
					if (FlxG.random() > 0.5)
					{
						add(new Bird(420, player.y + birdsY));
					}
					else
					{
						bgObjects.add(new Bird(420, player.y + birdsY));
					}
				}
			}
		}
	}
}

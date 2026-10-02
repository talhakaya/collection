namespace Games.OdeToPixelDays
{
	/// <summary>
	/// Ported from levels/Level26.as.
	/// </summary>
	public class Level26 : Level
	{
		private static string S_tiles = "Level26_S_tiles";

		private FlxText greyText;
		private FlxText greyText2;
		private FlxText greyText6;
		private FlxText greyText3;
		private FlxText greyText4;
		private FlxText greyText5;
		private bool textPut;
		private bool textPut2;
		private int countTime;
		private int count;

		public override void create()
		{
			gameSave = new FlxSave();
			gameSave.bind("save");
			gameSave.data.level = 27;
			musicStop();
			levelwidth = 40;
			levelheight = 30;
			scale = new FlxPoint(8, 8);
			base.create();
			FlxG.bgColor = 4290199552;
			int[] data = new int[]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 12, 12, 16, 12,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 6, 0,
				11, 15, 11, 11, 15, 11, 11, 15, 11, 11, 15, 11, 11, 15, 11, 11, 15, 11, 11, 15, 11, 11, 15, 11, 11, 15, 11, 13, 0, 0, 0, 0, 0, 0, 0, 8, 11, 15, 12, 11,
				0, 6, 0, 0, 6, 0, 0, 6, 0, 0, 6, 0, 0, 6, 0, 0, 6, 0, 0, 6, 0, 0, 6, 0, 0, 6, 0, 6, 0, 0, 0, 0, 0, 0, 0, 6, 0, 6, 0, 0,
				15, 12, 11, 15, 12, 11, 15, 12, 11, 15, 12, 11, 15, 12, 11, 15, 12, 11, 15, 12, 11, 15, 12, 11, 15, 12, 11, 14, 0, 0, 0, 0, 0, 0, 0, 8, 11, 12, 15, 11,
				14, 0, 0, 6, 0, 0, 6, 0, 0, 6, 0, 0, 6, 0, 0, 6, 0, 0, 6, 0, 0, 6, 0, 0, 6, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 6, 0,
				16, 11, 11, 12, 11, 11, 12, 11, 11, 12, 11, 11, 12, 11, 11, 12, 11, 11, 12, 11, 11, 12, 11, 11, 12, 11, 11, 10, 0, 0, 0, 0, 0, 0, 0, 8, 11, 15, 12, 11,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 6, 0, 0,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 11, 12, 15, 11,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 6, 0,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 11, 15, 12, 11,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 6, 0, 0,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 11, 12, 15, 11,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 6, 0,
				14, 0, 0, 0, 0, 0, 0, 0, 7, 11, 11, 11, 11, 11, 11, 13, 0, 7, 11, 11, 11, 11, 11, 11, 13, 0, 7, 11, 11, 11, 11, 11, 11, 13, 0, 8, 11, 15, 12, 11,
				14, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 6, 0, 6, 0, 0, 0, 0, 0, 0, 6, 0, 6, 0, 0, 0, 0, 0, 0, 6, 0, 6, 0, 6, 0, 0,
				14, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 6, 0, 6, 0, 0, 0, 0, 0, 0, 6, 0, 6, 0, 0, 0, 0, 0, 0, 6, 0, 8, 11, 12, 15, 11,
				14, 0, 0, 0, 0, 0, 0, 0, 4, 15, 11, 11, 11, 11, 15, 10, 0, 4, 15, 11, 11, 11, 11, 15, 10, 0, 4, 15, 11, 11, 11, 11, 15, 10, 0, 6, 0, 0, 6, 0,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 8, 11, 15, 12, 11,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 6, 0, 6, 0, 0,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 8, 11, 12, 15, 11,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 6, 0, 0, 6, 0,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 8, 11, 15, 12, 11,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 6, 0, 6, 0, 0,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 8, 15, 16, 15, 15,
				16, 15, 11, 15, 15, 11, 15, 15, 11, 16, 15, 11, 15, 15, 12, 15, 15, 11, 16, 15, 11, 15, 15, 12, 15, 15, 11, 16, 15, 11, 15, 15, 12, 15, 15, 12, 16, 16, 12, 16,
				16, 10, 0, 4, 10, 0, 4, 10, 0, 4, 10, 0, 4, 10, 0, 4, 10, 0, 4, 10, 0, 4, 10, 0, 4, 10, 0, 4, 10, 0, 4, 10, 0, 4, 10, 0, 4, 10, 0, 8,
				14, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 7, 16,
				14, 0, 4, 9, 0, 4, 9, 0, 4, 9, 0, 4, 9, 0, 4, 9, 0, 4, 9, 0, 4, 9, 0, 4, 9, 0, 4, 9, 0, 4, 9, 0, 4, 9, 0, 4, 9, 0, 4, 16,
				16, 13, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 0, 5, 0, 8
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, 40), S_tiles, 0, 0, FlxTilemap.AUTO);
			add(level);
			interacts.add(new Door2(16, 160, false, scale));
			countTime = 150;
			count = 0;
			greyText = new FlxText(8, -2, 300, "  HANS                                               WORLD             TIME", true);
			greyText.color = 4286019447;
			add(greyText);
			greyText2 = new FlxText(8, 6, 300, "999999                   0 x 99                 9 - 2                " + countTime, true);
			greyText2.color = 4286019447;
			add(greyText2);
			greyText3 = new FlxText(92, 56, 300, "WELCOME TO WARP ZONE!", true);
			greyText3.color = 4286019447;
			add(greyText3);
			greyText6 = new FlxText(8, 96, 300, "Your way            4                      3                      2", true);
			greyText6.color = 4286019447;
			add(greyText6);
			greyText4 = new FlxText(64, 70, 300, "YOU CAN'T PASS YOUR PROBLEMS BY, HANS!", true);
			greyText4.color = 4294910498;
			greyText4.alpha = 0;
			add(greyText4);
			greyText5 = new FlxText(11, 82, 300, "You thought something cool would happen, didn't you?", true);
			greyText5.color = 4281575987;
			greyText5.alpha = 0;
			add(greyText5);
			textPut = false;
			textPut2 = false;
			player = new Hans(0, -16, scale);
			add(player);
		}

		public override void update()
		{
			base.update();
			++count;
			if (count >= 60)
			{
				count = 0;
				--countTime;
				if (countTime == 0)
				{
					textPut2 = true;
				}
			}

			if ((FlxG.keys.DOWN || FlxG.keys.S) && !textPut)
			{
				textPut = true;
			}

			if (greyText4.alpha < 1 && textPut)
			{
				greyText4.alpha += 0.02;
			}

			if (greyText5.alpha < 1 && textPut2)
			{
				greyText5.alpha += 0.02;
			}

			greyText2.text = "999999                   0 x 99                 9 - 2                " + countTime;
			if (player.x < 0)
			{
				player.x = 0;
			}
			else if (player.x > 256)
			{
				player.x = 256;
			}
		}

		public override void getWell()
		{
			FlxG.bgColor = 4290199552;
		}

		public override void nextLevel()
		{
			FlxG.switchState(new Level27());
		}
	}
}

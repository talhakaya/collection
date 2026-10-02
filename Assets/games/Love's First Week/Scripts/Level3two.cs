namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Ported from Level3two.as.
	/// </summary>
	public class Level3two : State1
	{
		private static string img1 = "Level3two_img1";
		private static string img2 = "Level3two_img2";

		public Level3two(bool _is1player) : base(_is1player)
		{
		}

		public override void create()
		{
			int[] data;
			controlNaz = false;
			bgYogunluk = 3;
			talhaVar = true;
			levelwidth = 15;
			levelheight = 8;
			base.create();
			save.data.level = 3;
			data = new int[]
			{
				14, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 8, 16,
				14, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 8, 16,
				14, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 8, 16,
				16, 11, 11, 9, 0, 0, 0, 8, 11, 11, 9, 0, 0, 8, 16,
				14, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 8, 16,
				14, 0, 0, 0, 0, 0, 7, 14, 0, 0, 0, 0, 7, 16, 16,
				14, 0, 0, 0, 0, 0, 8, 14, 0, 0, 0, 0, 8, 16, 16,
				16, 15, 15, 15, 15, 15, 16, 16, 15, 15, 15, 15, 16, 16, 16
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(talha = new Naz(false, 64, 130 + 64));
			ondekiler.add(naz = new Naz(true, 256, 130 + 64));
			ondekiler.add(new FlxSprite(48, 228, img2));
			ondekiler.add(new FlxSprite(240, 228, img1));
			bulut(15);
			sarmasik(12, 3, 0, 68 + 64);
			agac("0", 100, 100);
			kapi(40, 32);
			kapi2(264, 32);
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Level4two(is1player));
		}
	}
}

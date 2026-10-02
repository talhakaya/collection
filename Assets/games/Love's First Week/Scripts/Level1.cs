namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Ported from Level1.as.
	/// </summary>
	public class Level1 : State1
	{
		private static string img1 = "Level1_img1";
		private static string img2 = "Level1_img2";
		private static string img3 = "Level1_img3";
		private static string img4 = "Level1_img4";

		public Level1(bool _is1player) : base(_is1player)
		{
		}

		public override void create()
		{
			int[] data;
			bgYogunluk = 0;
			levelwidth = 30;
			levelheight = 8;
			base.create();
			save.data.level = 1;
			data = new int[]
			{
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 13, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 14, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 13, 0, 0, 0, 7, 16, 16, 13, 0, 0, 0, 0, 0, 8,
				16, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 16, 16, 15, 15, 15, 16, 16, 16, 16, 15, 15, 15, 15, 15, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(naz = new Naz(true, 50, 162));
			enOndekiler.add(new FlxSprite(16, 200, img1));
			enOndekiler.add(new FlxSprite(64, 200, img2));
			taslar.add(new Tas(208, 164));
			bulut(15);
			sarmasik(30, 2, 16, 100);
			agac("0", 120, 75);
			agac("1", 260, 75);
			agac("2", 300, 75);
			agac("3", 420, 75);
			kapi(844, 128);
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Level2(is1player));
		}
	}
}

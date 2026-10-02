namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Ported from Level2.as.
	/// </summary>
	public class Level2 : State1
	{
		private static string img = "Level2_img";

		public Level2(bool _is1player) : base(_is1player)
		{
		}

		public override void create()
		{
			int[] data;
			bgYogunluk = 0;
			levelwidth = 16;
			levelheight = 8;
			base.create();
			save.data.level = 2;
			data = new int[]
			{
				16, 12, 12, 12, 12, 16, 16, 12, 12, 12, 12, 12, 12, 12, 12, 16,
				14, 0, 0, 0, 0, 8, 14, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 8, 14, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 8, 14, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 4, 10, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				16, 15, 15, 15, 15, 15, 15, 15, 13, 0, 0, 7, 15, 15, 15, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 14, 0, 0, 8, 16, 16, 16, 16
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(naz = new Naz(true, 42, 162));
			taslar.add(new Tas(168, 164));
			bulut(14);
			sarmasik(11, 1, 100, 100);
			agac("2", 50, 75);
			kapi(376, 128);
			enOndekiler.add(new FlxSprite(128, 200, img));
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Level3(is1player));
		}
	}
}

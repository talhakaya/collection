namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Ported from Level5.as.
	/// </summary>
	public class Level5 : State1
	{
		public Level5(bool _is1player) : base(_is1player)
		{
		}

		public override void create()
		{
			bgYogunluk = 2;
			levelwidth = 25;
			levelheight = 8;
			base.create();
			save.data.level = 5;
			int[] data = new int[]
			{
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 10, 0, 0, 0, 0, 0, 0, 0, 0, 7, 15, 15, 15, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 11, 9, 0, 0, 0, 0, 8, 16, 16, 16, 16,
				14, 0, 0, 0, 3, 11, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16, 16, 16, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 7, 15, 13, 0, 0, 0, 0, 0, 0, 0, 0, 7, 16, 16, 16, 16, 16,
				16, 15, 15, 15, 15, 15, 15, 15, 16, 16, 16, 15, 15, 15, 15, 15, 15, 15, 15, 16, 16, 16, 16, 16, 16
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(naz = new Naz(true, 64, 194));
			bulut(11);
			sarmasik(4, 4, 100, 132);
			sarmasik(2, 10, 300, 132);
			agac("2", 100, 107);
			agac("3", 150, 107);
			agac("0", 180, 107);
			agac("1", 220, 107);
			kapi(688, 32);
			ok("up", 18 * 32, 192);
			ok("left", 18 * 32, 0);
			ok("down", 14 * 32, 0);
			ok("up", 32, 192);
			ok("right", 32, 64);
			ok("down", 160, 64);
			tas(5 * 32, 192);
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Level6(is1player));
		}
	}
}

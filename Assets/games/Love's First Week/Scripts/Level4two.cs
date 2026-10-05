namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Ported from Level4two.as.
	/// </summary>
	public class Level4two : State1
	{
		private static string img1 = "Level4two_img1";
		private static string img2 = "Level4two_img2";

		public Level4two(bool _is1player) : base(_is1player)
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
			save.data.level = 4;
			data = new int[]
			{
				14, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 8, 16,
				14, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 8, 16,
				14, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 8, 16,
				16, 11, 11, 11, 13, 0, 0, 8, 11, 15, 9, 0, 0, 8, 16,
				14, 0, 0, 0, 6, 0, 0, 6, 0, 6, 0, 0, 7, 16, 16,
				14, 0, 0, 0, 2, 0, 7, 14, 0, 2, 0, 0, 8, 16, 16,
				14, 0, 0, 0, 0, 0, 8, 14, 0, 0, 0, 0, 8, 16, 16,
				16, 15, 15, 15, 15, 15, 16, 16, 15, 15, 15, 15, 16, 16, 16
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(talha = new Naz(false, 64, 130 + 64));
			ondekiler.add(naz = new Naz(true, 256, 130 + 64));
			// In the collection: the pictures of keys that stood here are prompts in the
			// collection's glyphs, drawn by texts (see FlxText).
			ondekiler.add(new FlxText(158, 225, 60, "{SPACE}").setFormat(null, 12));
			ondekiler.add(new FlxText(296, 225, 60, "{K}").setFormat(null, 12));
			tas(160, 192);
			tas(288, 192);
			bulut(14);
			sarmasik(3, 12, 0, 68 + 64);
			kapi(40, 32);
			kapi2(264, 32);
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Level5two(is1player));
		}
	}
}

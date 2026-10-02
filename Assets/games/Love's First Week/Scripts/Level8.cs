namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Ported from Level8.as.
	/// </summary>
	public class Level8 : State1
	{
		public Level8(bool _is1player) : base(_is1player)
		{
		}

		public override void create()
		{
			int[] data;
			bgYogunluk = 3;
			talhaVar = true;
			diyalogVar = true;
			levelwidth = 16;
			levelheight = 16;
			base.create();
			save.data.level = 8;
			data = new int[]
			{
				16, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 3, 11, 11, 9, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				16, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 9, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 15, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 16, 16, 16,
				16, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 16, 16, 16, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(naz = new Naz(true, 42, 130 + 256));
			ondekiler.add(talha = new Naz(false, 102, 130 + 256));
			talha.facing = FlxObject.LEFT;
			bulut(8);
			sarmasik(5, 8, 0, 68 + 256);
			kapi(48, 128);
			tas(246, 96);
			tas(352, 160);
			for (int u = 0; u < 9; u++)
			{
				ok("right", 32 + 16 * u, 0);
			}

			ok("down", 32 + 32 * 9, 0);
			if (save.data.lang == "tur")
			{
				diyaloglar.Add(new Diyalog(true, "Ikimizin degisik tekmeleriyle biz yenilmez bir takımız."));
				diyaloglar.Add(new Diyalog(false, "Superiz biz!"));
			}
			else
			{
				diyaloglar.Add(new Diyalog(true, "We're invincible with our super cool different kicks!"));
				diyaloglar.Add(new Diyalog(false, "We're awesome!"));
			}
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Level9(is1player));
		}
	}
}

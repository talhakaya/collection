namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Ported from Level13.as.
	/// </summary>
	public class Level13 : State1
	{
		public Level13(bool _is1player) : base(_is1player)
		{
		}

		public override void create()
		{
			int[] data;
			int u = 0;
			bgYogunluk = 14;
			talhaVar = true;
			diyalogVar = true;
			levelwidth = 30;
			levelheight = 8;
			base.create();
			save.data.level = 13;
			data = new int[]
			{
				16, 12, 12, 12, 16, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 16,
				14, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 3, 11, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 16,
				16, 15, 15, 15, 15, 13, 0, 0, 0, 0, 0, 0, 0, 7, 15, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(naz = new Naz(true, 72, 66));
			ondekiler.add(talha = new Naz(false, 104, 66));
			talha.facing = FlxObject.LEFT;
			for (tas(574, 160); u < 10; )
			{
				ok("up", 216 + u * 16, 128);
				u++;
			}

			bulut(13);
			kapi(844, 128);
			agac("2", 0, 100);
			sarmasik(20, 1, 410, 132);
			if (save.data.lang == "tur")
			{
				diyaloglar.Add(new Diyalog(false, "Biliyor musun, ben normalde gözlük takıyorum."));
				diyaloglar.Add(new Diyalog(true, "Aa, peki simdi niye takmıyorsun?"));
				diyaloglar.Add(new Diyalog(false, "Bilmem."));
				diyaloglar.Add(new Diyalog(true, "..."));
			}
			else
			{
				diyaloglar.Add(new Diyalog(false, "Did you know that I wear glasses, in real life?"));
				diyaloglar.Add(new Diyalog(true, "Oh, why don't you wear them in the game?"));
				diyaloglar.Add(new Diyalog(false, "Dunno."));
				diyaloglar.Add(new Diyalog(true, "..."));
			}
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Level14(is1player));
		}
	}
}

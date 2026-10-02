namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Ported from Level11.as.
	/// </summary>
	public class Level11 : State1
	{
		public Level11(bool _is1player) : base(_is1player)
		{
		}

		public override void create()
		{
			int[] data;
			bgYogunluk = 13;
			talhaVar = true;
			diyalogVar = true;
			levelwidth = 15;
			levelheight = 16;
			base.create();
			save.data.level = 11;
			data = new int[]
			{
				16, 16, 12, 12, 12, 12, 12, 12, 12, 12, 12, 16, 16, 16, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16, 16, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16, 16, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16, 16, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16, 16, 16,
				16, 16, 11, 11, 11, 9, 0, 0, 0, 0, 0, 8, 16, 16, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16, 16, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 3, 11, 11, 12, 12, 16, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16,
				16, 14, 0, 0, 3, 11, 9, 0, 0, 0, 0, 0, 0, 8, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16,
				16, 16, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 16, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(naz = new Naz(true, 144, 130 + 256));
			ondekiler.add(talha = new Naz(false, 204, 130 + 256));
			tas(224, 384);
			tas(288, 192);
			talha.facing = FlxObject.LEFT;
			bulut(5);
			sarmasik(3, 12, 0, 68 + 256);
			kapi(96, 96);
			if (save.data.lang == "tur")
			{
				diyaloglar.Add(new Diyalog(true, "Canım sıkıldı."));
				diyaloglar.Add(new Diyalog(false, "?"));
				diyaloglar.Add(new Diyalog(true, "Cuma günü seni terkedicem."));
				diyaloglar.Add(new Diyalog(false, "???????"));
			}
			else
			{
				diyaloglar.Add(new Diyalog(true, "I'm bored"));
				diyaloglar.Add(new Diyalog(false, "?"));
				diyaloglar.Add(new Diyalog(true, "I'm gonna break up with you on Friday."));
				diyaloglar.Add(new Diyalog(false, "???????"));
			}
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Level12(is1player));
		}
	}
}

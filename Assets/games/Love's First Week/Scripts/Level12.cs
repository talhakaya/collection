namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Ported from Level12.as.
	/// </summary>
	public class Level12 : State1
	{
		public Level12(bool _is1player) : base(_is1player)
		{
		}

		public override void create()
		{
			int[] data;
			bgYogunluk = 13;
			talhaVar = true;
			diyalogVar = true;
			levelwidth = 30;
			levelheight = 16;
			base.create();
			save.data.level = 12;
			data = new int[]
			{
				16, 16, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
				16, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 15, 15, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 7, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
				16, 15, 11, 9, 0, 0, 0, 0, 0, 0, 0, 0, 7, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 16,
				16, 10, 0, 0, 0, 0, 0, 0, 7, 15, 15, 15, 16, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 4, 12, 12, 12, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 3, 9, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				16, 15, 15, 15, 15, 15, 15, 13, 0, 0, 8, 16, 13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				16, 16, 16, 16, 16, 16, 16, 16, 15, 15, 16, 16, 16, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(naz = new Naz(true, 50, 386));
			ondekiler.add(talha = new Naz(false, 100, 386));
			talha.facing = FlxObject.LEFT;
			tas(464, 96);
			bulut(4);
			kapi(844, 384);
			agac("2", 0, 75 + 256);
			sarmasik(20, 1, 450, 356);
			if (save.data.lang == "tur")
			{
				diyaloglar.Add(new Diyalog(false, "Beni terk etmemen için ne yapabilirim?"));
				diyaloglar.Add(new Diyalog(true, "Tadelle ve Pınar süt alabilirsin."));
				diyaloglar.Add(new Diyalog(false, "Bu beni terk etmemeni saglar mı?"));
				diyaloglar.Add(new Diyalog(true, "Hayır."));
			}
			else
			{
				diyaloglar.Add(new Diyalog(false, "What can I do to make you not break up with me?"));
				diyaloglar.Add(new Diyalog(true, "Some chocolate and milk would be nice."));
				diyaloglar.Add(new Diyalog(false, "Is that enough not to break up?"));
				diyaloglar.Add(new Diyalog(true, "No."));
			}
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Level13(is1player));
		}
	}
}

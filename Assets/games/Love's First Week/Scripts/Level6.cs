namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Ported from Level6.as.
	/// </summary>
	public class Level6 : State1
	{
		private static string img = "Level6_img";

		public Level6(bool _is1player) : base(_is1player)
		{
		}

		public override void create()
		{
			int[] data;
			bgYogunluk = 2;
			talhaVar = true;
			diyalogVar = true;
			levelwidth = 16;
			levelheight = 8;
			base.create();
			save.data.level = 6;
			data = new int[]
			{
				16, 12, 12, 12, 12, 16, 16, 12, 12, 12, 12, 12, 12, 12, 12, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				16, 15, 15, 15, 15, 15, 15, 15, 13, 1, 1, 7, 15, 15, 15, 16,
				16, 15, 15, 15, 15, 15, 15, 15, 13, 1, 1, 7, 15, 15, 15, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 14, 1, 1, 8, 16, 16, 16, 16
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(naz = new Naz(true, 42, 130));
			ondekiler.add(talha = new Naz(false, 102, 130));
			talha.facing = FlxObject.LEFT;
			bulut(10);
			sarmasik(41, 1, 0, 100);
			agac("2", 50, 43);
			agac("1", 150, 43);
			agac("3", 190, 43);
			agac("4", 250, 43);
			agac("1", 350, 43);
			kapi(400, 96);
			if (is1player)
			{
				enOndekiler.add(new FlxSprite(92, 192, img));
			}

			if (save.data.lang == "tur")
			{
				diyaloglar.Add(new Diyalog(false, "Merabaa"));
				diyaloglar.Add(new Diyalog(true, "Selam, tanısalım mı?"));
				diyaloglar.Add(new Diyalog(false, "Olur, ben Talha."));
				diyaloglar.Add(new Diyalog(true, "Ben de " + NAZIRE + "."));
				diyaloglar.Add(new Diyalog(false, "Hadi su deposuna gidelim."));
				diyaloglar.Add(new Diyalog(true, "Yanlıslıkla sevgili falan olmayalım sonra?"));
				diyaloglar.Add(new Diyalog(false, "Kısmet."));
			}
			else
			{
				diyaloglar.Add(new Diyalog(false, "Hellooo"));
				diyaloglar.Add(new Diyalog(true, "Hey, let's introduce ourselves!"));
				diyaloglar.Add(new Diyalog(false, "Okay, I'm Talha."));
				diyaloglar.Add(new Diyalog(true, "And I'm " + NAZIRE + "."));
				diyaloglar.Add(new Diyalog(false, "Let's go to that place with the nice view!"));
				diyaloglar.Add(new Diyalog(true, "But what if we accidently become lovers?"));
				diyaloglar.Add(new Diyalog(false, "We'll see."));
			}
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Level7(is1player));
		}
	}
}

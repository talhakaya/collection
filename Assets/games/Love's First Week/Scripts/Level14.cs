namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Ported from Level14.as.
	/// </summary>
	public class Level14 : State1
	{
		public Level14(bool _is1player) : base(_is1player)
		{
		}

		public override void create()
		{
			int[] data;
			bgYogunluk = 14;
			talhaVar = true;
			diyalogVar = true;
			levelwidth = 15;
			levelheight = 16;
			base.create();
			save.data.level = 14;
			data = new int[]
			{
				16, 16, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 16, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16,
				16, 14, 0, 0, 0, 3, 11, 11, 11, 9, 0, 0, 0, 8, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16,
				16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16,
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
			talha.facing = FlxObject.LEFT;
			bulut(2);
			sarmasik(6, 6, 0, 58 + 256);
			for (int u = 0; u < 6; u++)
			{
				agac("0", 32 + 64 * u, 350);
				agac("1", 32 + 64 * u + 16, 350);
				agac("2", 32 + 64 * u + 32, 350);
				agac("3", 32 + 64 * u + 48, 350);
			}

			kapi(208, 96);
			ok("up", 64, 384);
			ok("up", 384, 384);
			ok("left", 384, 32);
			ok("right", 64, 32);
			if (save.data.lang == "tur")
			{
				diyaloglar.Add(new Diyalog(false, "Terk etme olayı saka, degil mi?"));
				diyaloglar.Add(new Diyalog(true, "Neden?"));
				diyaloglar.Add(new Diyalog(false, "Cunku tam belli olmuyor saka olup olmadıgı."));
				diyaloglar.Add(new Diyalog(true, "Sence?"));
				diyaloglar.Add(new Diyalog(false, "Saka."));
				diyaloglar.Add(new Diyalog(true, "..."));
				diyaloglar.Add(new Diyalog(false, "Degil."));
				diyaloglar.Add(new Diyalog(true, "..."));
				diyaloglar.Add(new Diyalog(false, "..."));
			}
			else
			{
				diyaloglar.Add(new Diyalog(false, "The breaking up thing.. It's a joke, right?"));
				diyaloglar.Add(new Diyalog(true, "Why?"));
				diyaloglar.Add(new Diyalog(false, "Because I can't tell if it's a joke or not!"));
				diyaloglar.Add(new Diyalog(true, "What do you think?"));
				diyaloglar.Add(new Diyalog(false, "Yes?"));
				diyaloglar.Add(new Diyalog(true, "..."));
				diyaloglar.Add(new Diyalog(false, "No?"));
				diyaloglar.Add(new Diyalog(true, "..."));
				diyaloglar.Add(new Diyalog(false, "..."));
			}
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Level15(is1player));
		}
	}
}

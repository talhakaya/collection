namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Ported from Level15.as.
	/// </summary>
	public class Level15 : State1
	{
		public Level15(bool _is1player) : base(_is1player)
		{
		}

		public override void create()
		{
			int[] data;
			bgYogunluk = 15;
			talhaVar = true;
			diyalogVar = true;
			levelwidth = 30;
			levelheight = 16;
			base.create();
			save.data.level = 15;
			data = new int[]
			{
				16, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 16, 12, 12, 16, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 3, 13, 0, 0, 0, 0, 0, 0, 0, 4, 13, 0, 0, 3, 11, 13, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 11, 11, 11, 11, 9, 0, 0, 0, 6, 0, 0, 0, 0, 6, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 3, 9, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 8, 13, 0, 0, 0, 4, 11, 11, 11, 11, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 3, 11, 16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 7, 11, 13, 0, 0, 3, 15, 14, 0, 0, 0, 0, 0, 0, 0, 0, 4, 12, 13, 0, 0, 0, 0, 0, 0, 0, 8,
				16, 11, 11, 10, 0, 4, 9, 0, 0, 4, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 11, 11, 11, 11, 11, 11, 11, 13, 0, 0, 0, 0, 0, 7, 15, 15, 15, 15, 16,
				14, 0, 0, 0, 0, 3, 11, 9, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 4, 11, 13, 0, 0, 0, 8, 16, 16, 16, 16, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 9, 0, 0, 8, 16, 16, 16, 16, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16, 16, 16, 16, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16, 16, 16, 16, 16,
				16, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 16, 16, 16, 16, 16, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(naz = new Naz(true, 50, 386));
			ondekiler.add(talha = new Naz(false, 100, 386));
			talha.facing = FlxObject.LEFT;
			tas(304, 32);
			ok("right", 352, 224);
			bulut(1);
			kapi(830, 64);
			sarmasik(40, 1, 150, 356);
			if (save.data.lang == "tur")
			{
				diyaloglar.Add(new Diyalog(true, "Sona yaklasıyoruz artık."));
				diyaloglar.Add(new Diyalog(false, "Bana bunu neden yapıyorsun?"));
				diyaloglar.Add(new Diyalog(true, "Çok eglenceli çünkü!"));
				diyaloglar.Add(new Diyalog(false, "..."));
				diyaloglar.Add(new Diyalog(true, "Noldu? Üzülüyor musun? Inanıyor musun gerçekten söylediklerime?"));
				diyaloglar.Add(new Diyalog(false, "..."));
			}
			else
			{
				diyaloglar.Add(new Diyalog(true, "We're close to the end now..."));
				diyaloglar.Add(new Diyalog(false, "Why are you doing this to me?"));
				diyaloglar.Add(new Diyalog(true, "Because it's so much fun!"));
				diyaloglar.Add(new Diyalog(false, "..."));
				diyaloglar.Add(new Diyalog(true, "Hey, are you sad? Do you really believe what I'm saying?"));
				diyaloglar.Add(new Diyalog(false, "..."));
			}
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Level16(is1player));
		}
	}
}

namespace Games.LovesFirstWeek
{
	/// <summary>
	/// The secret room, reached by walking off the right-hand edge of Level 7. Ported from
	/// LevelSecret1.as.
	/// </summary>
	public class LevelSecret1 : State1
	{
		private static string img = "LevelSecret1_img";

		public LevelSecret1(bool _is1player) : base(_is1player)
		{
		}

		public override void create()
		{
			int[] data;
			controlNaz = false;
			diyalogVar = true;
			bgYogunluk = 3;
			talhaVar = true;
			levelwidth = 15;
			levelheight = 8;
			base.create();
			data = new int[]
			{
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 16,
				16, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 16, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(talha = new Naz(false, 64, 66));
			ondekiler.add(naz = new Naz(true, 40, 66));
			kapi(320, 32);
			ondekiler.add(new FlxSprite(148, 102, img));
			if (save.data.lang == "tur")
			{
				diyaloglar.Add(new Diyalog(false, "Oha burası neresi?"));
				diyaloglar.Add(new Diyalog(true, "Uu sanırım gizli bir oda bulduk."));
				diyaloglar.Add(new Diyalog(false, "Yoksa bunlar da bizim gercek halimiz mi?"));
				diyaloglar.Add(new Diyalog(true, "Yok artık!"));
				diyaloglar.Add(new Diyalog(false, "Vay be!"));
			}
			else
			{
				diyaloglar.Add(new Diyalog(false, "What? Where are we?"));
				diyaloglar.Add(new Diyalog(true, "Oh, I think we found a secret room!"));
				diyaloglar.Add(new Diyalog(false, "Are these us in real life?"));
				diyaloglar.Add(new Diyalog(true, "Whoa!"));
				diyaloglar.Add(new Diyalog(false, "Wow!"));
			}
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Level8(is1player));
		}
	}
}

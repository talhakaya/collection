namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Ported from Level10.as.
	/// </summary>
	public class Level10 : State1
	{
		public Level10(bool _is1player) : base(_is1player)
		{
		}

		public override void create()
		{
			int[] data;
			bgYogunluk = 12;
			talhaVar = true;
			diyalogVar = true;
			levelwidth = 30;
			levelheight = 8;
			base.create();
			save.data.level = 10;
			data = new int[]
			{
				16, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 16,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 7, 13, 0, 0, 0, 0, 0, 7, 13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 8, 14, 0, 0, 0, 0, 0, 8, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				16, 15, 15, 15, 15, 16, 14, 0, 0, 0, 0, 0, 8, 16, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 16,
				16, 16, 16, 16, 16, 16, 14, 0, 0, 5, 0, 0, 8, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(naz = new Naz(true, 50, 162));
			ondekiler.add(talha = new Naz(false, 100, 162));
			talha.facing = FlxObject.LEFT;
			tas(574, 160);
			ok("left", 574, 32);
			ok("down", 288, 32);
			bulut(6);
			kapi(844, 128);
			agac("2", 0, 75);
			sarmasik(20, 1, 400, 100);
			if (save.data.lang == "tur")
			{
				diyaloglar.Add(new Diyalog(false, "Yolculugumuzun kalan kısmı da birbirimizi tekmeleyerek geçecekse"));
				diyaloglar.Add(new Diyalog(false, "iliskimizin temelleri çok saglam olacak demektir."));
			}
			else
			{
				diyaloglar.Add(new Diyalog(false, "If we continue kicking each other's butts for the rest of the journey"));
				diyaloglar.Add(new Diyalog(false, "our relationship will be unbreakable."));
			}
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Level11(is1player));
		}
	}
}

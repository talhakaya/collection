namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Ported from Level7.as.
	/// </summary>
	public class Level7 : State1
	{
		public Level7(bool _is1player) : base(_is1player)
		{
		}

		public override void create()
		{
			int[] data;
			bgYogunluk = 3;
			talhaVar = true;
			diyalogVar = true;
			levelwidth = 16;
			levelheight = 8;
			base.create();
			save.data.level = 7;
			data = new int[]
			{
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 8,
				14, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 6, 0, 0, 0, 8,
				16, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 16, 15, 15, 15, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(naz = new Naz(true, 42, 130));
			ondekiler.add(talha = new Naz(false, 102, 130));
			talha.facing = FlxObject.LEFT;
			bulut(9);
			sarmasik(5, 8, 0, 68);
			kapi(400, 96);
			tas(130, 128);
			for (int u = 0; u < 9; u++)
			{
				ok("right", 32 + 16 * u, 0);
			}

			ok("down", 32 + 32 * 9, 0);
			if (save.data.lang == "tur")
			{
				diyaloglar.Add(new Diyalog(true, "Simdi sevgili olduk ama burayı nasıl gecicez?"));
				diyaloglar.Add(new Diyalog(false, "Benim tekmem çok degisik bak izle simdi."));
			}
			else
			{
				diyaloglar.Add(new Diyalog(true, "Now we're lovers! But how will we climb up there?"));
				diyaloglar.Add(new Diyalog(false, "My kick is super cool, just watch!"));
			}
		}

		public override void update()
		{
			base.update();
			if (naz.x > levelwidth * 32 - 17)
			{
				base.nextLevel();
				FlxG.switchState(new LevelSecret1(is1player));
			}
			else if (talha.x > levelwidth * 32 - 17)
			{
				base.nextLevel();
				FlxG.switchState(new LevelSecret1(is1player));
			}
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Level8(is1player));
		}
	}
}

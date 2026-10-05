namespace Games.LovesFirstWeek
{
	/// <summary>
	/// The title screen. Ported from Menu.as.
	///
	/// It is a level like the others - the last one's scene, with the two of them sitting -
	/// under the title and the buttons: one or two players, continue, and the language.
	/// </summary>
	public class Menu : State1
	{
		private static string Asagi = "Menu_Asagi";
		private static string img1 = "Menu_img1";
		private static string img2 = "Menu_img2";
		private static string img3 = "Menu_img3";
		private static string img4 = "Menu_img4";
		private static string music = "Menu_music";

		public FlxText son;
		public FlxSprite asagi1;
		public bool asagiBas;
		public FlxText oyun;
		public FlxText oyun2;
		public FlxText credits;
		public FlxButton playButton;

		public Menu() : base(true)
		{
		}

		public override void create()
		{
			isMenuButton = false;
			FlxG.playMusic(music, 0.4);
			talhaVar = true;
			diyalogVar = true;
			levelwidth = 16;
			levelheight = 8;
			bgYogunluk = 0;
			base.create();
			int[] data = new int[]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 7, 13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				15, 15, 15, 15, 16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				16, 16, 16, 16, 16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(naz = new Naz(true, 166, 130));
			ondekiler.add(talha = new Naz(false, 150, 130));
			naz.sonBolum = true;
			talha.sonBolum = true;
			naz.play("otur1");
			talha.play("otur2");
			sarmasik(4, 3, -10, 100);
			agac("2", 50, 122);
			agac("2", 0, 122);
			agac("0", 20, 122);
			agac("0", -20, 122);
			agac("1", -10, 122);
			agac("1", 40, 122);
			agac("3", 60, 122);
			agac("3", 80, 122);
			agac("2", 175, 187);
			agac("1", 195, 187);
			agac("3", 220, 190);
			agac("0", 250, 230);
			bulut(20);
			kafaoku.x = -8;
			kafaoku.y = -8;
			if (isFullscreenAvailable)
			{
				enOndekiler.add(new FlxSprite((455 - 120) / 2.0, 184, img3));
			}

			// In the collection: the picture of the sound keys stood here, and under the
			// title's buttons the choice of language ("Language / Dil", Turkce, English). Sound
			// is the collection's settings screen and the game is English only for now.
			// Changed for the collection: the title at 60% of the source's size (60, with
			// the second line at y 50), which was too big.
			oyun = new FlxText(0, 0, 455, "");
			oyun.setFormat("NES", 36, 4281017343, "center", 2);
			add(oyun);
			oyun2 = new FlxText(0, 30, 455, "");
			oyun2.setFormat("NES", 36, 4281017343, "center", 2);
			add(oyun2);
			// Left out on purpose: the sponsor's "More Games" button, at (455 - 80) / 2, 183.
			if (save.data.level == null)
			{
				FlxButton onePlayer = enOndekiler.add(playButton = new FlxButton((455 - 80) / 2.0, 120, "1 Player Game", onePlayerNewGame));
				FlxButton twoPlayers = enOndekiler.add(playButton = new FlxButton((455 - 80) / 2.0, 141, "2 Player Game", twoPlayerNewGame));

				// Not in the source: see State1.focusOn.
				focusOn(onePlayer, twoPlayers);
			}
			else
			{
				FlxButton onePlayer = enOndekiler.add(playButton = new FlxButton((455 - 80) / 2.0, 120, "1 Player Game", onePlayerNewGame));
				FlxButton twoPlayers = enOndekiler.add(playButton = new FlxButton((455 - 80) / 2.0, 141, "2 Player Game", twoPlayerNewGame));
				FlxButton continueGame = enOndekiler.add(new FlxButton((455 - 80) / 2.0, 162, "Continue Game", nextLevel2));

				// Not in the source: see State1.focusOn.
				focusOn(onePlayer, twoPlayers, continueGame);
			}

			credits = new FlxText(180, 244, 455, "Made by Talha Kaya");
			add(credits);
		}

		public override void update()
		{
			base.update();
			talha.x = -100;
			talha.y = -100;
			naz.x = -100;
			naz.y = -100;
			if (save.data.lang == "tur")
			{
				oyun.text = "Askın";
				oyun2.text = "Ilk Haftası";
				credits.text = "Yapan: Talha Kaya";
			}
			else
			{
				oyun.text = "Love's";
				oyun2.text = "First Week";
				credits.text = "Made by Talha Kaya";
			}
		}

		public void onePlayerNewGame()
		{
			is1player = true;
			nextLevel();
		}

		public void twoPlayerNewGame()
		{
			is1player = false;
			nextLevel();
		}

		public override void nextLevel()
		{
			base.nextLevel();
			save.data.is1p = is1player;
			if (is1player)
			{
				FlxG.switchState(new Level1(is1player));
			}
			else
			{
				FlxG.switchState(new Level3two(is1player));
			}
		}

		public void nextLevel2()
		{
			base.nextLevel();
			if (save.data.is1p != null)
			{
				is1player = save.data.is1p.Value;
			}

			if (save.data.level == 1)
			{
				if (is1player)
				{
					FlxG.switchState(new Level1(is1player));
				}
				else
				{
					FlxG.switchState(new Level3two(is1player));
				}
			}
			else if (save.data.level == 2)
			{
				if (is1player)
				{
					FlxG.switchState(new Level2(is1player));
				}
				else
				{
					FlxG.switchState(new Level3two(is1player));
				}
			}
			else if (save.data.level == 3)
			{
				if (is1player)
				{
					FlxG.switchState(new Level3(is1player));
				}
				else
				{
					FlxG.switchState(new Level3two(is1player));
				}
			}
			else if (save.data.level == 4)
			{
				if (is1player)
				{
					FlxG.switchState(new Level4(is1player));
				}
				else
				{
					FlxG.switchState(new Level4two(is1player));
				}
			}
			else if (save.data.level == 5)
			{
				if (is1player)
				{
					FlxG.switchState(new Level5(is1player));
				}
				else
				{
					FlxG.switchState(new Level5two(is1player));
				}
			}
			else if (save.data.level == 6)
			{
				FlxG.switchState(new Level6(is1player));
			}
			else if (save.data.level == 7)
			{
				FlxG.switchState(new Level7(is1player));
			}
			else if (save.data.level == 8)
			{
				FlxG.switchState(new Level8(is1player));
			}
			else if (save.data.level == 9)
			{
				FlxG.switchState(new Level9(is1player));
			}
			else if (save.data.level == 10)
			{
				FlxG.switchState(new Level10(is1player));
			}
			else if (save.data.level == 11)
			{
				FlxG.switchState(new Level11(is1player));
			}
			else if (save.data.level == 12)
			{
				FlxG.switchState(new Level12(is1player));
			}
			else if (save.data.level == 13)
			{
				FlxG.switchState(new Level13(is1player));
			}
			else if (save.data.level == 14)
			{
				FlxG.switchState(new Level14(is1player));
			}
			else if (save.data.level == 15)
			{
				FlxG.switchState(new Level15(is1player));
			}
			else if (save.data.level == 16)
			{
				FlxG.switchState(new Level16(is1player));
			}
		}
	}
}

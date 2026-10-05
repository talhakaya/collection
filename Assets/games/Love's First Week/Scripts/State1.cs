using System.Collections.Generic;
using System;

namespace Games.LovesFirstWeek
{
	/// <summary>
	/// What every level has in common, and the game itself. Ported from State1.as.
	///
	/// Two characters and a puzzle of kicks. Naz kicks sideways and Talha kicks upwards;
	/// what they kick - a stone, or each other - flies in a straight line until something
	/// stops it, and arrow tiles turn whatever passes over them. A level ends at its door:
	/// alone in the first levels, and from then on only when both of them stand in it.
	///
	/// One player controls one of the two and swaps with Enter; two players have one each.
	/// The class also carries the dialogue box that opens some levels, the in-game menu
	/// behind the Menu button, and the background - sky, sun, clouds, trees and ivy - whose
	/// colour shifts from blue to pink as the week goes on (bgYogunluk).
	///
	/// A level subclass fills in the tile map, places things with the helpers at the end
	/// (kapi, ok, tas, agac, sarmasik, bulut) and says which level comes next.
	/// </summary>
	public class State1 : FlxState
	{
		public static string Tile = "State1_Tile";
		private static string kafadakiok = "State1_kafadakiok";
		private static string Gunes = "State1_Gunes";
		private static string KapiEsigi = "State1_KapiEsigi";
		private static string DiyalogKutusu = "State1_DiyalogKutusu";
		private static string Asagi = "State1_Asagi";
		private static string talhaJump = "State1_talhaJump";
		private static string talhaFloor = "State1_talhaFloor";
		private static string nazFloor = "State1_nazFloor";
		private static string nazJump = "State1_nazJump";
		private static string nazWalk = "State1_nazWalk";
		private static string talhaWalk = "State1_talhaWalk";
		private static string Dogru = "State1_Dogru";
		private static string menubg = "State1_menubg";
		private static string img1 = "State1_img1";
		private static string img2 = "State1_img2";
		private static string img3 = "State1_img3";
		private static string imgR = "State1_imgR";
		private static string imgSpace = "State1_imgSpace";
		private static string img4 = "State1_img4";

		public string FontNES = "State1_FontNES";
		public FlxText xAndY;
		public FlxSprite camera;
		public double cameraX = 100;
		public double cameraY = 100;
		public bool editorMode;
		public const double CAMERASPEED = 150;
		public FlxTilemap level;
		public int levelwidth = 15;
		public int levelheight = 8;
		public int levelno;
		public double xprev;
		public double yprev;
		public bool controlNaz = true;
		public Naz naz;
		public Naz talha;
		public bool talhaVar = false;
		public FlxGroup tasaTekmeler;
		public FlxGroup enArkalar;
		public FlxGroup bulutlar;
		public FlxGroup sarmasiklar;
		public FlxGroup agaclar;
		public FlxGroup enOndekiler;
		public FlxSprite diyalogKutusu1;
		public FlxSprite diyalogKutusu2;
		public FlxSprite diyalogKutusu3;
		public FlxSprite diyalogKutusu;
		public List<Diyalog> diyaloglar;
		public FlxText diyalogText1;
		public FlxText diyalogText2;
		public FlxSprite asagi;
		public Kafa nazKafa;
		public Kafa talhaKafa;
		public FlxGroup ondekiler;
		public FlxGroup taslar;
		public FlxGroup kapilar;
		public FlxGroup oklar;
		public FlxSprite kafaoku;
		public Kapi kapi1;
		public Kapi kapi2_;
		public int kapiCount = -1;
		public int diyalogCount = -1;
		public bool diyalogVar;
		public int di = -1;
		public int dj = -1;
		public bool nazIsTouching;
		public bool talhaIsTouching;
		public int nazWalkCount = -1;
		public int talhaWalkCount = -1;
		public const int WALKSOUND = 20;
		public FlxSave save;
		public const string NAZIRE = "Naz";
		public bool isMenuOn;
		public bool isFullscreenAvailable = false;
		public bool isMenuButton = true;
		public double bgYogunluk = 16;
		private FlxSprite menuBack;
		private FlxButton menuContinue;
		private FlxButton menuRestart;
		private FlxButton menuMute;
		private FlxButton menuMain;
		private FlxSprite menuFullscreen;
		private FlxSprite menuSound;
		private FlxText menuLanguage;
		private FlxButton menuTurkce;
		private FlxButton menuEnglish;
		private FlxSprite menuWASD;
		private FlxSprite menuDirs;
		private FlxSprite menuR;
		private FlxSprite menuSpace;
		private FlxText menuRText;
		private FlxText menuSpaceText;
		public bool is1player;
		public bool kapi2var;

		public State1(bool _is1player)
		{
			is1player = _is1player;
		}

		public override void create()
		{
			FlxSprite stripe = null;
			FlxButton menuButton = null;
			save = new FlxSave();
			save.bind("0");
			if (save.data.lang == null)
			{
				save.data.lang = "eng";
			}

			editorMode = false;
			FlxG.bgColor = 4294967295;
			enArkalar = new FlxGroup();
			add(enArkalar);
			bulutlar = new FlxGroup();
			add(bulutlar);
			agaclar = new FlxGroup();
			add(agaclar);
			sarmasiklar = new FlxGroup();
			add(sarmasiklar);
			kapilar = new FlxGroup();
			add(kapilar);
			oklar = new FlxGroup();
			add(oklar);
			ondekiler = new FlxGroup();
			add(ondekiler);
			taslar = new FlxGroup();
			ondekiler.add(taslar);
			tasaTekmeler = new FlxGroup();
			add(tasaTekmeler);
			enOndekiler = new FlxGroup();
			add(enOndekiler);
			diyalogKutusu1 = new FlxSprite(0, 192);
			diyalogKutusu1.makeGraphic(64, 64, 4294934399);
			diyalogKutusu1.scrollFactor.x = 0;
			diyalogKutusu1.scrollFactor.y = 0;
			diyalogKutusu1.alpha = 0;
			add(diyalogKutusu1);
			diyalogKutusu2 = new FlxSprite(64, 192);
			diyalogKutusu2.makeGraphic(327, 64, 4290822336);
			diyalogKutusu2.scrollFactor.x = 0;
			diyalogKutusu2.scrollFactor.y = 0;
			diyalogKutusu2.alpha = 0;
			add(diyalogKutusu2);
			diyalogKutusu3 = new FlxSprite(391, 192);
			diyalogKutusu3.makeGraphic(64, 64, 4281017343);
			diyalogKutusu3.scrollFactor.x = 0;
			diyalogKutusu3.scrollFactor.y = 0;
			diyalogKutusu3.alpha = 0;
			add(diyalogKutusu3);
			add(nazKafa = new Kafa(true));
			add(talhaKafa = new Kafa(false));
			diyaloglar = new List<Diyalog>();
			diyalogKutusu = new FlxSprite(0, 192, DiyalogKutusu);
			diyalogKutusu.scrollFactor.x = 0;
			diyalogKutusu.scrollFactor.y = 0;
			diyalogKutusu.alpha = 0;
			add(diyalogKutusu);
			// Changed for the collection: the dialogue is white over its black shadow. The
			// source had it black (4278190080) on the black shadow, which reads as one
			// smudged line.
			diyalogText1 = new FlxText(90, 195, 300, "");
			diyalogText1.setFormat("NES", 12, 0xffffffff, "left", 1);
			diyalogText1.scrollFactor.x = 0;
			diyalogText1.scrollFactor.y = 0;
			add(diyalogText1);
			diyalogText2 = new FlxText(90, 208, 300, "");
			diyalogText2.setFormat("NES", 12, 0xffffffff, "left", 1);
			diyalogText2.scrollFactor.x = 0;
			diyalogText2.scrollFactor.y = 0;
			add(diyalogText2);
			asagi = new FlxSprite(300, 240, Asagi);
			asagi.scrollFactor.x = asagi.scrollFactor.y = 0;
			asagi.alpha = 0;
			add(asagi);
			stripe = new FlxSprite(0, 0);
			stripe.makeGraphic(455, 256, 4286377455);
			stripe.scrollFactor.x = stripe.scrollFactor.y = 0;
			stripe.alpha = (16 - bgYogunluk) / 16;
			enArkalar.add(stripe);
			stripe = new FlxSprite(0, 0);
			stripe.makeGraphic(455, 256, 4294934128);
			stripe.scrollFactor.x = stripe.scrollFactor.y = 0;
			stripe.alpha = bgYogunluk / 16;
			enArkalar.add(stripe);
			for (int i = 0; i < 16; i++)
			{
				stripe = new FlxSprite(0, i * 16);
				stripe.makeGraphic(FlxG.stage.width, 16, 4278190080);
				stripe.alpha = i / 32.0 - 0.1;
				stripe.scrollFactor.x = stripe.scrollFactor.y = 0;
				enArkalar.add(stripe);
			}

			kafaoku = new FlxSprite(6, 6);
			kafaoku.loadGraphic(kafadakiok, true, false, 6, 6, false);
			kafaoku.addAnimation("0", new[] { 0, 1, 2, 1 }, 6, true);
			kafaoku.play("0");
			enOndekiler.add(kafaoku);
			kafaoku.x = -20;
			FlxSprite gunes = new FlxSprite(228 - 32, 128 - 32, Gunes);
			gunes.scrollFactor.x = gunes.scrollFactor.y = 0;
			enArkalar.add(gunes);
			base.create();
			if (editorMode)
			{
				FlxG.mouse.show();
				xAndY = new FlxText(0, 0, 400, "", true);
				xAndY.color = 4294967295;
				add(xAndY);
			}
			else
			{
				FlxG.mouse.show();
			}

			camera = new FlxSprite(cameraX, cameraY);
			camera.alpha = 0;
			add(camera);
			if (isFullscreenAvailable)
			{
				FlxG.stage.displayState = StageDisplayState.FULL_SCREEN;
				FlxCamera.defaultZoom = 3;
			}
			else
			{
				FlxG.stage.displayState = StageDisplayState.NORMAL;
				FlxCamera.defaultZoom = 1.5;
			}

			FlxG.resetCameras(new FlxCamera(0, 0, FlxG.width, FlxG.height));
			FlxG.camera.setBounds(0, 0, levelwidth * 32, levelheight * 32, true);
			FlxG.camera.follow(camera, FlxCamera.STYLE_LOCKON);
			if (isMenuButton)
			{
				enOndekiler.add(menuButton = new FlxButton(455 - 80, 0, "Menu", menuButtonClick));
				menuButton.scrollFactor.x = menuButton.scrollFactor.y = 0;
			}


			// Left out on purpose: the two logoButtons the source adds here - the sponsor's
			// logo and link, on screen in every level.

			// Not in the source: the pads need to know whose keys they are.
			FlxG.keys.twoPlayers = !is1player;
		}

		#region Not in the source: the menus without a mouse

		// The source's buttons are Flixel's, which only answer to the mouse. A menu hands
		// its buttons to focusOn, top to bottom; up and down move the focus between them
		// and the select key presses the one that has it.

		private readonly List<FlxButton> focusButtons = new List<FlxButton>();
		private int focusIndex;

		protected void focusOn(params FlxButton[] buttons)
		{
			focusButtons.Clear();
			focusButtons.AddRange(buttons);
			focusIndex = 0;
		}

		/// Start (or Escape) opens and closes the menu that the "Menu" button opens, and
		/// while it is open the pad and keys work its buttons rather than the characters.
		protected void updateMenuKeys()
		{
			if (isMenuButton && FlxG.keys.justPressed("ESCAPE"))
			{
				menuButtonClick();
			}

			FlxG.keys.gameKeysBlocked = isMenuOn;

			if (focusButtons.Count == 0)
			{
				return;
			}

			// The focus follows the mouse when it is on a button.
			for (int i = 0; i < focusButtons.Count; i++)
			{
				if (focusButtons[i].status != FlxButton.NORMAL)
				{
					focusIndex = i;
				}
			}

			if (FlxG.keys.justPressed("MENU_UP") && focusIndex > 0)
			{
				--focusIndex;
				FlxG.mouse.visible = false;
			}

			if (FlxG.keys.justPressed("MENU_DOWN") && focusIndex < focusButtons.Count - 1)
			{
				++focusIndex;
				FlxG.mouse.visible = false;
			}

			for (int i = 0; i < focusButtons.Count; i++)
			{
				focusButtons[i].focused = i == focusIndex;
			}

			if (FlxG.keys.justPressed("MENU_SELECT"))
			{
				focusButtons[focusIndex].press();
			}
		}

		#endregion

		public void menuButtonClick()
		{
			double sub = double.NaN;
			if (!isMenuOn)
			{
				sub = 16;
				isMenuOn = true;
				enOndekiler.add(menuBack = new FlxSprite((455 - 200) / 2.0, 8, menubg));
				menuBack.alpha = 0.4;
				enOndekiler.add(menuContinue = new FlxButton((455 - 80) / 2.0, 40 - sub, "Continue", menuButtonClick));
				enOndekiler.add(menuRestart = new FlxButton((455 - 80) / 2.0, 64 - sub, "Restart", resetState));
				enOndekiler.add(menuMute = new FlxButton((455 - 80) / 2.0, 88 - sub, "Mute On/Off", muteOnOff));
				enOndekiler.add(menuMain = new FlxButton((455 - 80) / 2.0, 112 - sub, "Main Menu", mainMenu));

				// Left out on purpose: the sponsor's "More Games" button, last in the column
				// at 136 - sub.
				if (isFullscreenAvailable)
				{
					enOndekiler.add(menuFullscreen = new FlxSprite((455 - 120) / 2.0, 130, img3));
					menuFullscreen.scrollFactor.x = menuFullscreen.scrollFactor.y = 0;
				}

				enOndekiler.add(menuSound = new FlxSprite((455 - 120) / 2.0, 142, img4));
				enOndekiler.add(menuLanguage = new FlxText((455 - 120) / 2.0, 160, 130, "Language / Dil:"));
				if (save.data.lang == "tur")
				{
					menuLanguage.text = "Language / Dil: Turkce";
				}
				else
				{
					menuLanguage.text = "Language / Dil: English";
				}

				enOndekiler.add(menuTurkce = new FlxButton(150, 172, "Turkce", turkce));
				enOndekiler.add(menuEnglish = new FlxButton(232, 172, "English", english));
				menuBack.scrollFactor.x = menuBack.scrollFactor.y = 0;
				menuContinue.scrollFactor.x = menuContinue.scrollFactor.y = 0;
				menuEnglish.scrollFactor.x = menuEnglish.scrollFactor.y = 0;
				menuLanguage.scrollFactor.x = menuLanguage.scrollFactor.y = 0;
				menuMute.scrollFactor.x = menuMute.scrollFactor.y = 0;
				menuMain.scrollFactor.x = menuMain.scrollFactor.y = 0;
				menuRestart.scrollFactor.x = menuRestart.scrollFactor.y = 0;
				menuSound.scrollFactor.x = menuSound.scrollFactor.y = 0;
				menuTurkce.scrollFactor.x = menuTurkce.scrollFactor.y = 0;
				// In the collection: the four pictures of keys that stood here (WASD, the
				// arrows, R and the space bar) are prompts in the collection's glyphs, drawn by
				// the texts. The two cluster pictures are texts of their own; the R and space
				// pictures are part of their labels, and their sprites are left empty.
				enOndekiler.add(menuWASD = new FlxText(138, 188, 60, "{<Keyboard>/w|<Keyboard>/a|<Keyboard>/s|<Keyboard>/d}").setFormat(null, 24));
				enOndekiler.add(menuDirs = new FlxText(186, 188, 60, "{<Keyboard>/upArrow|<Keyboard>/downArrow|<Keyboard>/leftArrow|<Keyboard>/rightArrow}").setFormat(null, 24));
				enOndekiler.add(menuR = new FlxSprite(286, 210));
				enOndekiler.add(menuSpace = new FlxSprite(270, 192));
				menuR.visible = false;
				menuSpace.visible = false;
				enOndekiler.add(menuRText = new FlxText(240, 210, 2000, "Restart: <size=150%>{R}</size>"));
				enOndekiler.add(menuSpaceText = new FlxText(240, 192, 2000, "Kick: <size=150%>{SPACE}</size>"));
				menuWASD.scrollFactor.x = menuWASD.scrollFactor.y = 0;
				menuDirs.scrollFactor.x = menuDirs.scrollFactor.y = 0;
				menuR.scrollFactor.x = menuR.scrollFactor.y = 0;
				menuSpace.scrollFactor.x = menuSpace.scrollFactor.y = 0;
				menuRText.scrollFactor.x = menuRText.scrollFactor.y = 0;
				menuSpaceText.scrollFactor.x = menuSpaceText.scrollFactor.y = 0;
				focusOn(menuContinue, menuRestart, menuMute, menuMain, menuTurkce, menuEnglish);
			}
			else
			{
				isMenuOn = false;
				focusOn();
				menuBack.kill();
				menuContinue.kill();
				menuRestart.kill();
				menuMute.kill();
				menuMain.kill();
				if (isFullscreenAvailable)
				{
					menuFullscreen.kill();
				}

				menuSound.kill();
				menuLanguage.kill();
				menuTurkce.kill();
				menuEnglish.kill();
				menuWASD.kill();
				menuDirs.kill();
				menuR.kill();
				menuSpace.kill();
				menuRText.kill();
				menuSpaceText.kill();
			}
		}

		private void turkce()
		{
			if (isMenuOn)
			{
				menuLanguage.text = "Language / Dil: Turkce";
			}

			save.data.lang = "tur";
		}

		private void english()
		{
			if (isMenuOn)
			{
				menuLanguage.text = "Language / Dil: English";
			}

			save.data.lang = "eng";
		}

		public void muteOnOff()
		{
			if (FlxG.volume > 0)
			{
				FlxG.volume = 0;
			}
			else
			{
				FlxG.volume = 0.5;
			}
		}

		public override void update()
		{
			double xx = double.NaN;
			double yy = double.NaN;
			bool mouseMoved = false;
			int tile = 0;
			string s = null;
			int i = 0;
			int j = 0;
			updateMenuKeys();
			base.update();
			if (isMenuOn)
			{
				if (save.data.lang == "tur")
				{
					menuLanguage.text = "Language / Dil: Turkce";
				}
				else
				{
					menuLanguage.text = "Language / Dil: English";
				}
			}

			naz.kapida = false;
			FlxG.overlap<SoyutTekme, Tas>(tasaTekmeler, taslar, overlapTekme);
			FlxG.collide(ondekiler);
			FlxG.collide<Naz, Tas>(naz, taslar, nazTasCollide);
			FlxG.overlap<Naz, SoyutTekme>(naz, tasaTekmeler, nazTekmeOverlap);
			FlxG.overlap<Naz, Kapi>(naz, kapilar, kapiOverlap);
			if (talhaVar)
			{
				talha.kapida = false;
				FlxG.overlap<Naz, SoyutTekme>(talha, tasaTekmeler, nazTekmeOverlap);
				FlxG.overlap<Naz, Ok>(talha, oklar, nazOkOverlap);
				FlxG.collide<Naz, Tas>(talha, taslar, nazTasCollide);
				FlxG.overlap<Naz, Kapi>(talha, kapilar, kapiOverlap);
			}

			FlxG.overlap<Naz, Ok>(naz, oklar, nazOkOverlap);
			if (naz.kapida && talhaVar && talha.kapida)
			{
				opus();
			}

			FlxG.overlap<Tas, Ok>(taslar, oklar, tasOkOverlap);
			if (!controlNaz && naz.movable)
			{
				if (naz.velocity.y > 0)
				{
					naz.play("down");
				}
				else if (naz.velocity.y < 0)
				{
					naz.play("up");
				}
				else
				{
					naz.play("idle");
				}
			}

			if (controlNaz && talhaVar && talha.movable)
			{
				if (talha.velocity.y > 0)
				{
					talha.play("down");
				}
				else if (talha.velocity.y < 0)
				{
					talha.play("up");
				}
				else
				{
					talha.play("idle");
				}
			}

			if (talhaVar)
			{
				if (!talhaIsTouching && talha.isTouching(FlxObject.FLOOR))
				{
					FlxG.play(talhaFloor);
				}
				else if (talhaIsTouching && !talha.isTouching(FlxObject.FLOOR))
				{
					FlxG.play(talhaJump);
				}

				talhaIsTouching = talha.isTouching(FlxObject.FLOOR);
				if (talha.x > levelwidth * 32 - 16)
				{
					talha.x = levelwidth * 32 - 16;
				}
				else if (talha.x < 16)
				{
					talha.x = 16;
				}
			}

			if (!nazIsTouching && naz.isTouching(FlxObject.FLOOR))
			{
				FlxG.play(nazFloor);
			}
			else if (nazIsTouching && !naz.isTouching(FlxObject.FLOOR))
			{
				FlxG.play(nazJump);
			}

			nazIsTouching = naz.isTouching(FlxObject.FLOOR);
			if (naz.x > levelwidth * 32 - 16)
			{
				naz.x = levelwidth * 32 - 16;
			}
			else if (naz.x < 16)
			{
				naz.x = 16;
			}

			if (is1player)
			{
				if (controlNaz && naz.movable)
				{
					if (FlxG.keys.LEFT || FlxG.keys.A)
					{
						naz.facing = FlxObject.LEFT;
						naz.acceleration.x = -naz.maxVelocity.x * 4;
						naz.play("walk");
						if (nazIsTouching)
						{
							if (nazWalkCount == 0)
							{
								nazWalkCount = WALKSOUND;
								FlxG.play(nazWalk);
							}

							--nazWalkCount;
						}
					}
					else if (FlxG.keys.RIGHT || FlxG.keys.D)
					{
						naz.facing = FlxObject.RIGHT;
						naz.acceleration.x = naz.maxVelocity.x * 4;
						naz.play("walk");
						if (nazIsTouching)
						{
							if (nazWalkCount == 0)
							{
								nazWalkCount = WALKSOUND;
								FlxG.play(nazWalk);
							}

							--nazWalkCount;
						}
					}
					else
					{
						naz.acceleration.x = 0;
						naz.play("idle");
						nazWalkCount = WALKSOUND;
					}

					if ((FlxG.keys.UP || FlxG.keys.W) && naz.isTouching(FlxObject.FLOOR))
					{
						naz.jumpThrottle = 0;
						naz.velocity.y = -naz.maxVelocity.y * 0.2;
					}

					if ((FlxG.keys.UP || FlxG.keys.W) && naz.jumpThrottle < naz.jumpThrottleMax && naz.velocity.y < 0)
					{
						++naz.jumpThrottle;
						naz.velocity.y -= naz.maxVelocity.y * 0.043;
					}

					if (naz.velocity.y > 0)
					{
						naz.play("down");
					}
					else if (naz.velocity.y < 0)
					{
						naz.play("up");
					}

					if (FlxG.keys.SPACE && naz.isTouching(FlxObject.FLOOR))
					{
						naz.tekme();
					}
				}
				else if (!controlNaz && talha.movable)
				{
					if (FlxG.keys.LEFT || FlxG.keys.A)
					{
						talha.facing = FlxObject.LEFT;
						talha.acceleration.x = -talha.maxVelocity.x * 4;
						talha.play("walk");
						if (talhaIsTouching)
						{
							if (talhaWalkCount == 0)
							{
								talhaWalkCount = WALKSOUND;
								FlxG.play(talhaWalk);
							}

							--talhaWalkCount;
						}
					}
					else if (FlxG.keys.RIGHT || FlxG.keys.D)
					{
						talha.facing = FlxObject.RIGHT;
						talha.acceleration.x = talha.maxVelocity.x * 4;
						talha.play("walk");
						if (talhaIsTouching)
						{
							if (talhaWalkCount == 0)
							{
								talhaWalkCount = WALKSOUND;
								FlxG.play(talhaWalk);
							}

							--talhaWalkCount;
						}
					}
					else
					{
						talha.acceleration.x = 0;
						talha.play("idle");
						talhaWalkCount = WALKSOUND;
					}

					if ((FlxG.keys.UP || FlxG.keys.W) && talha.isTouching(FlxObject.FLOOR))
					{
						talha.jumpThrottle = 0;
						talha.velocity.y = -talha.maxVelocity.y * 0.2;
					}

					if ((FlxG.keys.UP || FlxG.keys.W) && talha.jumpThrottle < talha.jumpThrottleMax && talha.velocity.y < 0)
					{
						++talha.jumpThrottle;
						talha.velocity.y -= talha.maxVelocity.y * 0.043;
					}

					if (talha.velocity.y > 0)
					{
						talha.play("down");
					}
					else if (talha.velocity.y < 0)
					{
						talha.play("up");
					}

					if (FlxG.keys.SPACE && talha.isTouching(FlxObject.FLOOR))
					{
						talha.tekme();
					}
				}
			}
			else
			{
				if (naz.movable)
				{
					if (FlxG.keys.LEFT)
					{
						naz.facing = FlxObject.LEFT;
						naz.acceleration.x = -naz.maxVelocity.x * 4;
						naz.play("walk");
						if (nazIsTouching)
						{
							if (nazWalkCount == 0)
							{
								nazWalkCount = WALKSOUND;
								FlxG.play(nazWalk);
							}

							--nazWalkCount;
						}
					}
					else if (FlxG.keys.RIGHT)
					{
						naz.facing = FlxObject.RIGHT;
						naz.acceleration.x = naz.maxVelocity.x * 4;
						naz.play("walk");
						if (nazIsTouching)
						{
							if (nazWalkCount == 0)
							{
								nazWalkCount = WALKSOUND;
								FlxG.play(nazWalk);
							}

							--nazWalkCount;
						}
					}
					else
					{
						naz.acceleration.x = 0;
						naz.play("idle");
						nazWalkCount = WALKSOUND;
					}

					if (FlxG.keys.UP && naz.isTouching(FlxObject.FLOOR))
					{
						naz.jumpThrottle = 0;
						naz.velocity.y = -naz.maxVelocity.y * 0.2;
					}

					if (FlxG.keys.UP && naz.jumpThrottle < naz.jumpThrottleMax && naz.velocity.y < 0)
					{
						++naz.jumpThrottle;
						naz.velocity.y -= naz.maxVelocity.y * 0.043;
					}

					if (naz.velocity.y > 0)
					{
						naz.play("down");
					}
					else if (naz.velocity.y < 0)
					{
						naz.play("up");
					}

					if (FlxG.keys.K && naz.isTouching(FlxObject.FLOOR))
					{
						naz.tekme();
					}
				}

				if (talha.movable)
				{
					if (FlxG.keys.A)
					{
						talha.facing = FlxObject.LEFT;
						talha.acceleration.x = -talha.maxVelocity.x * 4;
						talha.play("walk");
						if (talhaIsTouching)
						{
							if (talhaWalkCount == 0)
							{
								talhaWalkCount = WALKSOUND;
								FlxG.play(talhaWalk);
							}

							--talhaWalkCount;
						}
					}
					else if (FlxG.keys.D)
					{
						talha.facing = FlxObject.RIGHT;
						talha.acceleration.x = talha.maxVelocity.x * 4;
						talha.play("walk");
						if (talhaIsTouching)
						{
							if (talhaWalkCount == 0)
							{
								talhaWalkCount = WALKSOUND;
								FlxG.play(talhaWalk);
							}

							--talhaWalkCount;
						}
					}
					else
					{
						talha.acceleration.x = 0;
						talha.play("idle");
						talhaWalkCount = WALKSOUND;
					}

					if (FlxG.keys.W && talha.isTouching(FlxObject.FLOOR))
					{
						talha.jumpThrottle = 0;
						talha.velocity.y = -talha.maxVelocity.y * 0.2;
					}

					if (FlxG.keys.W && talha.jumpThrottle < talha.jumpThrottleMax && talha.velocity.y < 0)
					{
						++talha.jumpThrottle;
						talha.velocity.y -= talha.maxVelocity.y * 0.043;
					}

					if (talha.velocity.y > 0)
					{
						talha.play("down");
					}
					else if (talha.velocity.y < 0)
					{
						talha.play("up");
					}

					if (FlxG.keys.SPACE && talha.isTouching(FlxObject.FLOOR))
					{
						talha.tekme();
					}
				}
			}

			if (FlxG.keys.justPressed("ENTER") && talhaVar)
			{
				controlNaz = !controlNaz;
			}

			if (naz.tekmeAtti)
			{
				naz.tekmeAtti = false;
				tasaTekmeler.add(new SoyutTekme(naz.x - 12, naz.y + 16, true));
			}
			else if (talhaVar && talha.tekmeAtti)
			{
				talha.tekmeAtti = false;
				tasaTekmeler.add(new SoyutTekme(talha.x - 12, talha.y + 16, false));
			}

			if (FlxG.keys.justPressed("R"))
			{
				resetState();
			}

			if (editorMode)
			{
				Mouse.show();
				xx = Math.Floor(FlxG.mouse.x / 32);
				yy = Math.Floor(FlxG.mouse.y / 32);
				mouseMoved = false;
				if (xprev != xx || yprev != yy)
				{
					mouseMoved = true;
				}

				xprev = xx;
				yprev = yy;
				if (FlxG.mouse.justPressed())
				{
					tile = (int)level.getTile((int)xx, (int)yy);
					level.setTile((int)xx, (int)yy, tile > 0 ? 0u : 1u);
				}

				if (FlxG.mouse.pressed())
				{
					if (mouseMoved)
					{
						tile = (int)level.getTile((int)xx, (int)yy);
						level.setTile((int)xx, (int)yy, tile > 0 ? 0u : 1u);
					}
				}

				if (FlxG.keys.justPressed("E"))
				{
					s = "";
					for (i = 0; i < levelheight; i++)
					{
						for (j = 0; j < levelwidth; j++)
						{
							s += level.getTile(j, i) + ", ";
						}

						s += "\n";
					}

				}

				xAndY.x = camera.x - 12;
				xAndY.y = camera.y - 16;
			}

			if (isFullscreenAvailable)
			{
				if (FlxG.keys.justPressed("F"))
				{
					if (FlxG.stage.displayState == StageDisplayState.NORMAL)
					{
						FlxG.stage.displayState = StageDisplayState.FULL_SCREEN;
						FlxCamera.defaultZoom = 3;
						FlxG.resetCameras(new FlxCamera(0, 0, FlxG.width, FlxG.height));
						FlxG.camera.setBounds(0, 0, levelwidth * 32, levelheight * 32, true);
						FlxG.camera.follow(camera, FlxCamera.STYLE_LOCKON);
					}
					else
					{
						FlxG.stage.displayState = StageDisplayState.NORMAL;
						FlxCamera.defaultZoom = 1.5;
						FlxG.resetCameras(new FlxCamera(0, 0, FlxG.width, FlxG.height));
						FlxG.camera.setBounds(0, 0, levelwidth * 32, levelheight * 32, true);
						FlxG.camera.follow(camera, FlxCamera.STYLE_LOCKON);
					}
				}

				if (FlxG.stage.displayState == StageDisplayState.NORMAL && FlxCamera.defaultZoom == 3)
				{
					FlxCamera.defaultZoom = 1.5;
					FlxG.resetCameras(new FlxCamera(0, 0, FlxG.width, FlxG.height));
					FlxG.camera.setBounds(0, 0, levelwidth * 32, levelheight * 32, true);
					FlxG.camera.follow(camera, FlxCamera.STYLE_LOCKON);
				}
			}

			if (is1player)
			{
				if (controlNaz)
				{
					if (naz.x > camera.x + 10)
					{
						camera.velocity.x = CAMERASPEED;
					}
					else if (naz.x < camera.x - 10)
					{
						camera.velocity.x = -CAMERASPEED;
					}
					else
					{
						camera.velocity.x = 0;
					}

					if (naz.y > camera.y + 10)
					{
						camera.velocity.y = CAMERASPEED;
					}
					else if (naz.y < camera.y - 10)
					{
						camera.velocity.y = -CAMERASPEED;
					}
					else
					{
						camera.velocity.y = 0;
					}
				}
				else
				{
					if (talha.x > camera.x + 10)
					{
						camera.velocity.x = CAMERASPEED;
					}
					else if (talha.x < camera.x - 10)
					{
						camera.velocity.x = -CAMERASPEED;
					}
					else
					{
						camera.velocity.x = 0;
					}

					if (talha.y > camera.y + 10)
					{
						camera.velocity.y = CAMERASPEED;
					}
					else if (talha.y < camera.y - 10)
					{
						camera.velocity.y = -CAMERASPEED;
					}
					else
					{
						camera.velocity.y = 0;
					}
				}
			}
			else if (controlNaz)
			{
				camera.x = naz.x;
				camera.y = naz.y;
			}
			else
			{
				camera.x = talha.x;
				camera.y = talha.y;
			}

			cameraX = camera.x;
			cameraY = camera.y;
			if (is1player)
			{
				if (controlNaz)
				{
					kafaoku.x = naz.x + 6;
					kafaoku.y = naz.y - 10;
				}
				else
				{
					kafaoku.x = talha.x + 6;
					kafaoku.y = talha.y - 10;
				}
			}

			if (kapiCount == 50)
			{
				kapi1.open();
				kapi2open();
				FlxG.play(Dogru);
				--kapiCount;
			}
			else if (kapiCount == 0)
			{
				nextLevel();
			}
			else if (kapiCount > 0)
			{
				--kapiCount;
			}

			if (naz.y > levelheight * 32 || talhaVar && talha.y > levelheight * 32)
			{
				death();
			}

			if (diyalogVar && save.data.dialog == true)
			{
				if (di == -1 && dj == -1)
				{
					diyalogBaslat();
					++di;
					diyalogCount = 6;
				}
				else if (di == diyaloglar.Count)
				{
					diyalogKapat();
					diyalogVar = false;
				}
				else if (diyaloglar[di].text.Length != dj)
				{
					if (FlxG.keys.S || FlxG.keys.DOWN || diyalogCount == 0)
					{
						++dj;
						diyalogUpdate();
						diyalogCount = 2;
					}
					else if (diyalogCount > 0)
					{
						--diyalogCount;
					}
				}
				else if (FlxG.keys.justPressed("S") || FlxG.keys.justPressed("DOWN"))
				{
					++di;
					diyalogCount = 6;
					dj = 0;
					diyalogText1.text = "";
					diyalogText2.text = "";
				}
			}
		}

		public void kapi2open()
		{
			if (kapi2var)
			{
				kapi2_.open();
			}
		}

		public void resetState()
		{
			save.data.dialog = false;
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

		public void overlapTekme(SoyutTekme tekme, Tas tas)
		{
			if (tekme.byNaz)
			{
				if (tas.x < naz.x && naz.facing == FlxObject.LEFT)
				{
					tas.tekmelen("left");
				}
				else if (tas.x > naz.x && naz.facing == FlxObject.RIGHT)
				{
					tas.tekmelen("right");
				}
			}
			else if (tas.x < talha.x && talha.facing == FlxObject.LEFT)
			{
				tas.tekmelen("up");
			}
			else if (tas.x > talha.x && talha.facing == FlxObject.RIGHT)
			{
				tas.tekmelen("up");
			}
		}

		public void nazTasCollide(Naz asd, Tas tas)
		{
			if (tas.y < asd.y + 25 && tas.velocity.y > 0)
			{
				death();
			}

			if (!tas.immovable)
			{
				if (tas.nazCount > 0)
				{
					--tas.nazCount;
				}
				else
				{
					tas.nazCount = 3;
					tas.makeImmovable();
				}
			}
		}

		public void death()
		{
			resetState();
		}

		public void nazTekmeOverlap(Naz asd, SoyutTekme tekme)
		{
			if (talhaVar)
			{
				if (tekme.byNaz && asd == talha)
				{
					if (talha.x < naz.x && naz.facing == FlxObject.LEFT)
					{
						talha.tekmelen("left");
					}
					else if (talha.x > naz.x && naz.facing == FlxObject.RIGHT)
					{
						talha.tekmelen("right");
					}
				}
				else if (!tekme.byNaz && asd == naz)
				{
					if (naz.x < talha.x && talha.facing == FlxObject.LEFT)
					{
						naz.tekmelen("up");
					}
					else if (naz.x > talha.x && talha.facing == FlxObject.RIGHT)
					{
						naz.tekmelen("up");
					}
				}
			}
		}

		public void nazOkOverlap(Naz asd, Ok ok)
		{
			if (ok.dir == "up" && asd.velocity.y >= 0 || ok.dir == "down" && asd.velocity.y <= 0 || ok.dir == "right" && asd.velocity.x <= 0 || ok.dir == "left" && asd.velocity.x >= 0)
			{
				asd.x = ok.x - 8;
				asd.y = ok.y - 14;
			}

			asd.tekmelen(ok.dir);
		}

		public void tasOkOverlap(Tas tas, Ok ok)
		{
			if (ok.dir == "up" && tas.velocity.y != -Tas.SPEED || ok.dir == "down" && tas.velocity.y != Tas.SPEED || ok.dir == "right" && tas.velocity.x != Tas.SPEED || ok.dir == "left" && tas.velocity.x != -Tas.SPEED)
			{
				tas.x = ok.x - 12;
				tas.y = ok.y - 12;
			}

			tas.tekmelen(ok.dir);
		}

		public void bulut(int yogunluk)
		{
			for (int i = 0; i < yogunluk; i++)
			{
				bulutlar.add(new Bulut("0", FlxG.random() * 500 - 40, 30 + FlxG.random() * 50));
				bulutlar.add(new Bulut("1", FlxG.random() * 500 - 40, 30 + FlxG.random() * 50));
				bulutlar.add(new Bulut("2", FlxG.random() * 500 - 40, 30 + FlxG.random() * 50));
				bulutlar.add(new Bulut("3", FlxG.random() * 500 - 40, 30 + FlxG.random() * 50));
			}
		}

		public void sarmasik(int yogunluk, double genislik, double _X, double _Y)
		{
			double random = double.NaN;
			string dir = null;
			for (int i = 0; i < yogunluk * 4; i++)
			{
				random = 4 * FlxG.random();
				if (random > 3)
				{
					dir = "3";
				}
				else if (random > 2)
				{
					dir = "2";
				}
				else if (random > 1)
				{
					dir = "1";
				}
				else
				{
					dir = "0";
				}

				sarmasiklar.add(new Sarmasik(dir, _X + i * 3 * genislik + 10 * FlxG.random(), _Y + 30 * FlxG.random()));
			}
		}

		public void agac(string no, double _X, double _Y)
		{
			agaclar.add(new Agac(no, _X, _Y));
		}

		public void kapiOverlap(Naz asd, Kapi kapii)
		{
			if (talhaVar)
			{
				asd.kapida = true;
			}
			else if (kapiCount == -1)
			{
				kapiCount = 50;
				naz.makeImmovable(50);
				naz.play("idle");
				naz.kapidaTek = true;
			}
		}

		public virtual void nextLevel()
		{
			save.data.dialog = true;
		}

		public void mainMenu()
		{
			save.data.dialog = true;
			FlxG.switchState(new Menu());
		}

		public void kapi(double _X, double _Y)
		{
			kapilar.add(kapi1 = new Kapi(_X + 16, _Y + 16));
			enOndekiler.add(new FlxSprite(_X - 18, _Y + 64, KapiEsigi));
		}

		public void kapi2(double _X, double _Y)
		{
			kapi2var = true;
			kapilar.add(kapi2_ = new Kapi(_X + 16, _Y + 16));
			enOndekiler.add(new FlxSprite(_X - 18, _Y + 64, KapiEsigi));
		}

		public void ok(string direction, double _X, double _Y)
		{
			oklar.add(new Ok(direction, _X + 16, _Y + 16));
		}

		public void tas(double _X, double _Y)
		{
			taslar.add(new Tas(_X + 4, _Y + 4));
		}

		public void opus()
		{
			if (is1player || !kapi2var)
			{
				if (kapiCount == -1)
				{
					talha.makeImmovable(150);
					naz.makeImmovable(150);
					kapiCount = 150;
					if (naz.x > talha.x)
					{
						naz.facing = FlxObject.LEFT;
						talha.facing = FlxObject.RIGHT;
						naz.x = kapi1.x + 16;
						talha.x = kapi1.x;
						naz.y = kapi1.y + 18;
						talha.y = kapi1.y + 18;
					}
					else
					{
						naz.facing = FlxObject.RIGHT;
						talha.facing = FlxObject.LEFT;
						talha.x = kapi1.x + 16;
						naz.x = kapi1.x;
						naz.y = kapi1.y + 18;
						talha.y = kapi1.y + 18;
					}

					naz.op();
					talha.op();
				}
			}
			else if (kapiCount == -1)
			{
				talha.makeImmovable(50);
				naz.makeImmovable(50);
				talha.velocity.x = talha.velocity.y = 0;
				naz.velocity.x = naz.velocity.y = 0;
				kapiCount = 50;
			}
		}

		public void diyalogBaslat()
		{
			diyalogKutusu1.alpha = 1;
			diyalogKutusu2.alpha = 1;
			diyalogKutusu3.alpha = 1;
			diyalogKutusu.alpha = 1;
			nazKafa.alpha = 1;
			talhaKafa.alpha = 1;
			asagi.alpha = 1;
		}

		public void diyalogKapat()
		{
			diyalogKutusu1.alpha = 0;
			diyalogKutusu2.alpha = 0;
			diyalogKutusu3.alpha = 0;
			diyalogKutusu.alpha = 0;
			nazKafa.alpha = 0;
			talhaKafa.alpha = 0;
			asagi.alpha = 0;
		}

		public void diyalogUpdate()
		{
			if (diyaloglar[di].isNaz)
			{
				diyalogText1.text = "Naz:";
				diyalogText1.alignment = "left";
				diyalogText2.alignment = "left";
				nazKafa.play("talk");
				talhaKafa.play("idle");
			}
			else
			{
				diyalogText1.text = "Talha:";
				diyalogText1.alignment = "right";
				diyalogText2.alignment = "right";
				nazKafa.play("idle");
				talhaKafa.play("talk");
			}

			diyalogText2.text = diyaloglar[di].text.Substring(0, Math.Min(dj + 1, diyaloglar[di].text.Length));
		}
	}
}

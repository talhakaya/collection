using System.Collections.Generic;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The title screen. Ported from levels/Menu.as.
	///
	/// It is a Level with no tile map and Hans parked far above the screen: that gets it the
	/// sky and the wall of big windows for free. Clouds drift past a small
	/// castle, birds cross in flocks, and starting a game whites the screen out while the
	/// castle grows towards the camera.
	///
	/// Changed for the collection: the sponsor's "More Games" button is gone and the two
	/// below it moved up into its place, and the buttons - mouse-only in the source - can
	/// also be chosen with up and down and pressed with the action or jump button. Space
	/// meant "continue" in the source; with nothing else chosen it still does, because the
	/// first button is the one that starts with the focus.
	/// </summary>
	public class Menu : Level
	{
		private static string S_ = "Menu_S_";
		private static string music = "Menu_music";

		private int count;
		private List<Cloud2> clouds;
		private const int noOfBirds = 10;
		private const double birdsY = 360;
		private TTimer timerForBirds;
		private TTimer timerForBirds2;
		private FlxSprite beyaz;
		private FlxSprite castle;
		private FlxText text1;
		private FlxText text2;
		private FlxText text3;
		private FlxText text3s;
		private FlxText text4;
		private bool pressedSpace;
		private bool pressed;
		private const double textspeed = 2;
		private const double castlespeed = 0.2;
		private List<FlxButton> buttons;

		// Not in the source: which button has the focus.
		private int selected;

		public override void create()
		{
			int i = 0;
			gameSave = new FlxSave();
			gameSave.bind("save");
			if (gameSave.data.level != null)
			{
				levelno = gameSave.data.level.Value;
			}

			if (gameSave.data.level == 0)
			{
				levelno = 46;
			}

			FlxG.playMusic(music, 1);
			levelwidth = 40;
			levelheight = 30;
			scale = new FlxPoint(1, 1);
			FlxG.bgColor = 4289374890;
			player = new Hans(0, -200, scale);
			base.create();
			add(player);
			castle = new FlxSprite(144, 60);
			castle.loadGraphic(S_, true, false, 32, 32, false);
			castle.addAnimation(" ", new[] { 0, 1 }, 1, true);
			castle.play(" ");
			bgObjects.add(castle);
			birds(true);
			birds(true);
			birds(false);
			timerForBirds = new TTimer();
			timerForBirds2 = new TTimer();
			add(timerForBirds);
			add(timerForBirds2);
			timerForBirds.start(100);
			timerForBirds2.start(400);
			clouds = new List<Cloud2>();
			for (i = 0; i < 37; i++)
			{
				clouds.Add(bgObjects.add(new Cloud2(-100 + FlxG.random() * 440, levelwidth)));
				clouds[i].x = -100 + i * 10;
				bgObjects.add(clouds[i]);
			}

			text1 = new FlxText(25, 20, 320, "Ode To Pixel Days", true);
			text1.size = 24;
			text1.color = 4289357414;
			text2 = new FlxText(27, 22, 320, "Ode To Pixel Days", true);
			text2.size = 24;
			text2.color = 4278190080;
			add(text2);
			add(text1);
			buttons = new List<FlxButton>();
			double YBUTTON = 96;
			if (gameSave.data.level == null)
			{
				buttons.Add(new FlxButton(120, YBUTTON, "New Game", newGame));
				add(buttons[buttons.Count - 1]);
			}
			else
			{
				buttons.Add(new FlxButton(120, YBUTTON, "Continue", continueGame));
				add(buttons[buttons.Count - 1]);
				buttons.Add(new FlxButton(120, YBUTTON + 22, "New Game", newGame));
				add(buttons[buttons.Count - 1]);
			}

			// Left out on purpose: the sponsor's "More Games" button at YBUTTON + 44. The two
			// below it, at + 66 and + 88 in the source, close the gap.
			buttons.Add(new FlxButton(120, YBUTTON + 44, "Soundtrack", goToMyURL2));
			add(buttons[buttons.Count - 1]);
			buttons.Add(new FlxButton(120, YBUTTON + 66, "Extras", goToMyURL3));
			add(buttons[buttons.Count - 1]);
			selected = 0;
			// In the collection: the "Press M At Any Time To Mute" line that stood here is gone,
			// with the key.
			beyaz = new FlxSprite(0, 0);
			beyaz.makeGraphic(320, 240, 4294967295);
			beyaz.alpha = 0;
			add(beyaz);
		}

		public void goToMyURL2()
		{
			UnityEngine.Application.OpenURL("http://talhakaya.bandcamp.com/album/ode-to-pixel-days-soundtrack");
		}

		public void goToMyURL3()
		{
			UnityEngine.Application.OpenURL("http://talhadevlog.blogspot.com/2013/02/ode-to-pixel-days-extras.html");
		}

		public void newGame()
		{
			if (!pressed)
			{
				pressedSpace = false;
				beyazlas();
			}
		}

		public void continueGame()
		{
			if (!pressed)
			{
				pressedSpace = true;
				beyazlas();
			}
		}

		public override void update()
		{
			int i = 0;
			if (!created || player == null)
			{
				return;
			}

			base.update();
			player.y = -200;
			if (timerForBirds.complete)
			{
				birds(false);
				birds(true);
			}

			if (timerForBirds2.complete)
			{
				birds(false);
				birds(true);
			}

			++count;
			if (count == 30)
			{
				bgObjects.add(new Cloud2(-100 + FlxG.random() * 440, levelwidth));
				count = 0;
			}

			if (!pressed)
			{
				// Not in the source: the focus follows the mouse when it is on a button, and
				// up and down otherwise.
				for (i = 0; i < buttons.Count; i++)
				{
					if (buttons[i].status != FlxButton.NORMAL)
					{
						selected = i;
					}
				}

				if (FlxG.keys.justPressed("MENU_UP") && selected > 0)
				{
					--selected;
					FlxG.mouse.visible = false;
				}

				if (FlxG.keys.justPressed("MENU_DOWN") && selected < buttons.Count - 1)
				{
					++selected;
					FlxG.mouse.visible = false;
				}

				for (i = 0; i < buttons.Count; i++)
				{
					buttons[i].focused = i == selected;
				}

				// The source: if (FlxG.keys.justPressed("SPACE")) { pressedSpace = true;
				// beyazlas(); } - continue, or start if there is nothing to continue. That is
				// what the first button does, and the first button is where the focus starts.
				if (FlxG.keys.justPressed("MENU_SELECT"))
				{
					buttons[selected].press();
				}

				if (FlxG.keys.justPressed("N") && gameSave.data.level != null)
				{
					pressedSpace = false;
					beyazlas();
				}
			}
			else
			{
				beyaz.alpha += 0.02;
				text1.y -= textspeed;
				text2.y -= textspeed;
				for (i = 0; i < buttons.Count; i++)
				{
					buttons[i].alpha -= 0.04;
				}

				castle.scale.x += castlespeed;
				castle.scale.y += castlespeed;
				castle.x = 160 - 16 * scale.x;
			}
		}

		public override void addBgDetail()
		{
			int i = 0;
			int j = 0;
			for (i = 0; i < 5; i++)
			{
				for (j = 0; j < 4; j++)
				{
					bgDetails[i][j] = "windowBig11";
				}
			}
		}

		private void birds(bool onLeft)
		{
			int i = 0;
			if (onLeft)
			{
				for (i = 0; i < noOfBirds; i++)
				{
					bgObjects.add(new Bird2(-100, player.y + birdsY));
				}
			}
			else
			{
				for (i = 0; i < noOfBirds; i++)
				{
					bgObjects.add(new Bird2(420, player.y + birdsY));
				}
			}
		}

		private void beyazlas()
		{
			if (!pressed)
			{
				new FlxTimer().start(1, 1, startPlaying);
				pressed = true;
			}
		}

		private void startPlaying(FlxTimer a)
		{
			if (pressedSpace)
			{
				startPlaying2();
			}

			if (!pressedSpace)
			{
				FlxG.switchState(new Level1());
			}
		}

		private void startPlaying2()
		{
			musicStop();
			if (gameSave.data.level == null)
			{
				FlxG.switchState(new Level1());
			}
			else if (gameSave.data.level == 1)
			{
				FlxG.switchState(new Level1());
			}
			else if (gameSave.data.level == 2)
			{
				FlxG.switchState(new Level2());
			}
			else if (gameSave.data.level == 3)
			{
				FlxG.switchState(new Level3());
			}
			else if (gameSave.data.level == 4)
			{
				FlxG.switchState(new Level3second());
			}
			else if (gameSave.data.level == 5)
			{
				FlxG.switchState(new Level4());
			}
			else if (gameSave.data.level == 6)
			{
				FlxG.switchState(new Level5());
			}
			else if (gameSave.data.level == 7)
			{
				FlxG.switchState(new Level6());
			}
			else if (gameSave.data.level == 8)
			{
				FlxG.switchState(new Level7());
			}
			else if (gameSave.data.level == 9)
			{
				FlxG.switchState(new Level8());
			}
			else if (gameSave.data.level == 10)
			{
				FlxG.switchState(new Level9());
			}
			else if (gameSave.data.level == 11)
			{
				FlxG.switchState(new Level10());
			}
			else if (gameSave.data.level == 12)
			{
				FlxG.switchState(new Level11());
			}
			else if (gameSave.data.level == 13)
			{
				FlxG.switchState(new Level12());
			}
			else if (gameSave.data.level == 14)
			{
				FlxG.switchState(new Level13());
			}
			else if (gameSave.data.level == 15)
			{
				FlxG.switchState(new Level14());
			}
			else if (gameSave.data.level == 16)
			{
				FlxG.switchState(new Level15());
			}
			else if (gameSave.data.level == 17)
			{
				FlxG.switchState(new Level16());
			}
			else if (gameSave.data.level == 18)
			{
				FlxG.switchState(new Level17());
			}
			else if (gameSave.data.level == 19)
			{
				FlxG.switchState(new Level18());
			}
			else if (gameSave.data.level == 20)
			{
				FlxG.switchState(new Level19());
			}
			else if (gameSave.data.level == 21)
			{
				FlxG.switchState(new Level20());
			}
			else if (gameSave.data.level == 22)
			{
				FlxG.switchState(new Level21());
			}
			else if (gameSave.data.level == 23)
			{
				FlxG.switchState(new Level22());
			}
			else if (gameSave.data.level == 24)
			{
				FlxG.switchState(new Level23());
			}
			else if (gameSave.data.level == 25)
			{
				FlxG.switchState(new Level24());
			}
			else if (gameSave.data.level == 26)
			{
				FlxG.switchState(new Level25());
			}
			else if (gameSave.data.level == 27)
			{
				FlxG.switchState(new Level26());
			}
			else if (gameSave.data.level == 28)
			{
				FlxG.switchState(new Level27());
			}
			else if (gameSave.data.level == 29)
			{
				FlxG.switchState(new Level27second());
			}
			else if (gameSave.data.level == 30)
			{
				FlxG.switchState(new Level28());
			}
			else if (gameSave.data.level == 31)
			{
				FlxG.switchState(new Level29());
			}
			else if (gameSave.data.level == 32)
			{
				FlxG.switchState(new Level30());
			}
			else if (gameSave.data.level == 33)
			{
				FlxG.switchState(new Level31());
			}
			else if (gameSave.data.level == 34)
			{
				FlxG.switchState(new Level32());
			}
			else if (gameSave.data.level == 35)
			{
				FlxG.switchState(new Level33());
			}
			else if (gameSave.data.level == 36)
			{
				FlxG.switchState(new Level34());
			}
			else if (gameSave.data.level == 37)
			{
				FlxG.switchState(new Level35());
			}
			else if (gameSave.data.level == 38)
			{
				FlxG.switchState(new Level35second());
			}
			else if (gameSave.data.level == 39)
			{
				FlxG.switchState(new Level36());
			}
			else if (gameSave.data.level == 40)
			{
				FlxG.switchState(new Level37());
			}
			else if (gameSave.data.level == 41)
			{
				FlxG.switchState(new Level38());
			}
			else if (gameSave.data.level == 42)
			{
				FlxG.switchState(new Level39());
			}
			else if (gameSave.data.level == 43)
			{
				FlxG.switchState(new Level40());
			}
			else if (gameSave.data.level == 44)
			{
				FlxG.switchState(new Level41());
			}
			else if (gameSave.data.level == 45)
			{
				FlxG.switchState(new Level42());
			}
			else if (gameSave.data.level == 46)
			{
				FlxG.switchState(new Level43());
			}
		}
	}
}

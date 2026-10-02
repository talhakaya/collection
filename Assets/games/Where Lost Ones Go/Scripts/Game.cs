using System.Collections.Generic;

namespace Games.WhereLostOnesGo
{
	/// <summary>
	/// The piece. Ported from Game.as.
	///
	/// A photo of the Bosphorus Bridge, with traffic, birds and glints on the water drawn
	/// in single pixels, and the player - one pixel on the bridge. The first click zooms
	/// twenty times into that pixel; the second makes it fall. The fall takes 117 seconds,
	/// and everything else happens during it: a chain of timers shows a line of text, opens
	/// a scene (the screen goes black around the player), closes it, shows more text, and
	/// so on through five scenes. When the fall ends the view zooms back out to the photo.
	///
	/// STATE is which scene is open, or 0 between scenes; things a scene spawns remove
	/// themselves when it goes back to 0.
	/// </summary>
	public class Game : Sprite
	{
		public static Game instance;
		public const double debugTimerConst = 1;
		public const double CarY = 194;
		public const double CarXMin = 276;
		public const double CarXMax = 552;
		public const double BirdX = 20;
		public const double BirdY = 20;
		public static int STATE = 0;

		public background BackgroundImage;
		public double zoomLocationX = -386;
		public double zoomLocationY = -188;
		public double zoomDuration = 10;
		public double zoomScale = 20;
		public bool zoomBool;
		public Player player;
		public FlashTimer timer;
		public bool playerFalling;
		public int count;
		public List<DisplayObject> collisionCheckList;

		public Game()
		{
			instance = this;
			BackgroundImage = new background();
			addChild(BackgroundImage);
			createSeaEffects(40);
			createCars(40);
			createBirds(40);
			addEventListener(Event.ENTER_FRAME, enterFrameHandler);
			Main.Foreground.addEventListener(MouseEvent.CLICK, mouseClickHandler);
			player = new Player();
			addChild(player);
			STATE = 1;
			count = 0;
			collisionCheckList = new List<DisplayObject>();
			SoundManager.playBeach();
			ScreenTextHandler.instance.makeTitleTextFieldVisible("\"Where Lost Ones Go\"\n\n                       by Talha Kaya\n\n\n                                                                                   Play with mouse");
		}

		public void enterFrameHandler()
		{
			if (STATE == 3)
			{
				++count;
				if (count >= 10)
				{
					count = 0;
					Main.instance.addChild(new DropState2());
				}

				collisionCheck();
			}
			else if (STATE == 7)
			{
				++count;
				if (count >= 30)
				{
					count = 0;
					Main.instance.addChild(new Scorpion(stage.mouseX, stage.mouseY, 0.25));
				}

				collisionCheck();
			}
		}

		public void createSeaEffects(int howMany)
		{
			for (int i = 0; i < howMany; i++)
			{
				addChild(new SeaEffect());
			}
		}

		public void createCars(int howMany)
		{
			for (int i = 0; i < howMany; i++)
			{
				addChild(new Car());
			}
		}

		public void createBirds(int howMany)
		{
			Bird.birdX = 140;
			Bird.birdY = 125;
			Bird.destinationX = 680;
			Bird.destinationY = 50 + 100 * Flash.random();
			for (int i = 0; i < howMany; i++)
			{
				addChild(new Bird(Bird.birdX, Bird.birdY, Bird.destinationX, Bird.destinationY));
			}

			Bird.birdX = -40;
			Bird.birdY = 50 + 100 * Flash.random();
		}

		public void zoomIn()
		{
			zoomBool = !zoomBool;
			TweenLite.to(this, zoomDuration * debugTimerConst, new TweenVars
			{
				x = zoomLocationX * zoomScale,
				y = zoomLocationY * zoomScale,
				scaleX = zoomScale,
				scaleY = zoomScale,
				ease = Cubic.easeIn,
			});
		}

		public void zoomOut()
		{
			zoomBool = !zoomBool;
			TweenLite.to(this, zoomDuration * debugTimerConst, new TweenVars
			{
				x = 0,
				y = 0,
				scaleX = 1,
				scaleY = 1,
				ease = Cubic.easeOut,
				onComplete = zoomOutCompleteHandler,
			});
		}

		public void zoomOutCompleteHandler()
		{
			ScreenTextHandler.instance.makeTitleTextFieldVisible("\n                       THE END\n\n \"Where Lost Ones Go\"\n\n                       by Talha Kaya");
		}

		public void mouseClickHandler()
		{
			if (STATE == 1)
			{
				TweenLite.killTweensOf(this);
				zoomIn();
				TweenLite.to(player, 18 * debugTimerConst, new TweenVars { x = player.x + 21 });
				STATE = 0;
				timer = new FlashTimer(11000 * debugTimerConst, 1);
				timer.addEventListener(TimerEvent.TIMER, timerHandlerState1);
				timer.start();
				SoundManager.stopBeach();
				ScreenTextHandler.instance.makeTitleTextFieldInvisible();
			}
			else if (STATE == 2)
			{
				TweenLite.to(player, 117 * debugTimerConst, new TweenVars
				{
					y = player.y + 32,
					onComplete = playerFalled,
				});
				TweenLite.to(this, 117 * debugTimerConst, new TweenVars { y = y - 24 * zoomScale });
				STATE = 0;
				playerFalling = true;
				ScreenTextHandler.instance.makeCustomTextFieldInvisible();
				timer = new FlashTimer(7000 * debugTimerConst, 1);
				timer.addEventListener(TimerEvent.TIMER, timerHandlerState2);
				timer.start();
				SoundManager.playMusic();
			}
			else if (STATE == 8)
			{
			}
		}

		public void playerFalled()
		{
			player.die();
			zoomOut();
			SoundManager.continueBeach();
		}

		/// Starts the next link of the chain: after this many milliseconds, then.
		private void after(double milliseconds, System.Action then)
		{
			timer = new FlashTimer(milliseconds * debugTimerConst, 1);
			timer.addEventListener(TimerEvent.TIMER, then);
			timer.start();
		}

		// The chain. Each handler in the source removes itself from the timer that fired it
		// and makes a new timer for the next; after() does the same.

		public void timerHandlerState1()
		{
			MessageBox.instance.makeVisible(1);
			after(7000, timerHandlerState1_1);
		}

		public void timerHandlerState1_1()
		{
			MessageBox.instance.makeVisible(2);
			after(3000, timerHandlerState1_2);
		}

		public void timerHandlerState1_2()
		{
			STATE = 2;
			ScreenTextHandler.instance.makeCustomTextFieldVisible("JUMP");
		}

		public void timerHandlerState2()
		{
			MessageBox.instance.makeVisible(3);
			after(4000, timerHandlerState2_1);
		}

		public void timerHandlerState2_1()
		{
			MessageBox.instance.makeVisible(4);
			after(3000, timerHandlerState2_2);
		}

		public void timerHandlerState2_2()
		{
			STATE = 3;
			player.openScene();
			after(13000, timerHandlerState3);
			Main.instance.addChild(new Scorpion(100, 100, 1.5));
			Main.instance.addChild(new Scorpion(600, 200, 1.5));
			Main.instance.addChild(new Scorpion(400, 300, 1.5));
		}

		public void timerHandlerState3()
		{
			STATE = 0;
			player.closeScene();
			after(4000, timerHandlerState3_1);
		}

		public void timerHandlerState3_1()
		{
			MessageBox.instance.makeVisible(5);
			after(3000, timerHandlerState3_2);
		}

		public void timerHandlerState3_2()
		{
			player.openScene();
			STATE = 4;
			after(10000, timerHandlerState4);
			SeaState4.instance.isActive = true;
			SeaState4.instance.isBlood = true;
			Main.instance.addChild(new Human(30, 320, true));
			Main.instance.addChild(new Human(70, 310, true));
			Main.instance.addChild(new Human(45, 330, true));
			Main.instance.addChild(new Human(120, 340, true));
			Main.instance.addChild(new Human(140, 320, true));
			Main.instance.addChild(new Human(180, 330, true));
			Main.instance.addChild(new Human(230, 340, true));
			Main.instance.addChild(new Human(250 + 30, 320, true));
			Main.instance.addChild(new Human(250 + 70, 310, true));
			Main.instance.addChild(new Human(250 + 45, 330, true));
			Main.instance.addChild(new Human(250 + 120, 340, true));
			Main.instance.addChild(new Human(250 + 140, 320, true));
			Main.instance.addChild(new Human(250 + 180, 330, true));
			Main.instance.addChild(new Human(250 + 230, 340, true));
			Main.instance.addChild(new Human(500 + 30, 320, true));
			Main.instance.addChild(new Human(500 + 70, 310, true));
			Main.instance.addChild(new Human(500 + 45, 330, true));
			Main.instance.addChild(new Human(500 + 120, 340, true));
			Main.instance.addChild(new Human(500 + 140, 320, true));
			Main.instance.addChild(new Human(500 + 180, 330, true));
			Main.instance.addChild(new Human(500 + 230, 340, true));
		}

		public void timerHandlerState4()
		{
			player.closeScene();
			STATE = 0;
			after(3000, timerHandlerState4_1);
		}

		public void timerHandlerState4_1()
		{
			MessageBox.instance.makeVisible(6);
			after(4000, timerHandlerState4_2);
		}

		public void timerHandlerState4_2()
		{
			MessageBox.instance.makeVisible(7);
			after(3000, timerHandlerState4_3);
		}

		public void timerHandlerState4_3()
		{
			player.openScene();
			STATE = 5;
			after(15000, timerHandlerState5);
			SeaState4.instance.isActive = true;
		}

		public void timerHandlerState5()
		{
			player.closeScene();
			STATE = 0;
			after(2000, timerHandlerState5_1);
		}

		public void timerHandlerState5_1()
		{
			MessageBox.instance.makeVisible(8);
			after(3000, timerHandlerState5_2);
		}

		public void timerHandlerState5_2()
		{
			player.openScene();
			STATE = 6;
			Main.instance.addChild(new Human(100, 100, false));
			Main.instance.addChild(new Human(200, 80, false));
			Main.instance.addChild(new Human(40, 320, false));
			Main.instance.addChild(new Human(580, 60, false));
			Main.instance.addChild(new Human(600, 200, false));
			Main.instance.addChild(new Human(400, 300, false));
			Main.instance.addChild(new Human(200, 270, false));
			Main.instance.addChild(new Human(250, 200, false));
			Main.instance.addChild(new Human(550, 260, false));
			Main.instance.addChild(new Human(360, 120, false));
			after(10000, timerHandlerState6);
		}

		public void timerHandlerState6()
		{
			player.closeScene();
			STATE = 0;
			after(2000, timerHandlerState6_1);
		}

		public void timerHandlerState6_1()
		{
			MessageBox.instance.makeVisible(9);
			after(4000, timerHandlerState6_2);
		}

		public void timerHandlerState6_2()
		{
			MessageBox.instance.makeVisible(10);
			after(4000, timerHandlerState6_3);
		}

		public void timerHandlerState6_3()
		{
			MessageBox.instance.makeVisible(11);
			after(4000, timerHandlerState6_4);
		}

		public void timerHandlerState6_4()
		{
			MessageBox.instance.makeVisible(12);
			after(3000, timerHandlerState6_5);
		}

		public void timerHandlerState6_5()
		{
			player.openScene();
			STATE = 7;
			after(16000, timerHandlerState7);
		}

		public void timerHandlerState7()
		{
			player.closeScene();
			STATE = 0;
		}

		/// Scorpions touching a drop are pushed back; scorpions touching the player draw
		/// blood and stop.
		public void collisionCheck()
		{
			for (int i = 0; i < collisionCheckList.Count; i++)
			{
				var scorpion = collisionCheckList[i] as Scorpion;
				if (scorpion == null)
				{
					continue;
				}

				for (int j = 0; j < collisionCheckList.Count; j++)
				{
					var drop = collisionCheckList[j] as DropState2;
					if (drop != null && Main.distanceBetweenSpritesCloserThanRadius(scorpion.radius + drop.radius, scorpion, drop))
					{
						scorpion.dieAlready = true;
					}
				}

				if (player != null && Main.distanceBetweenPlayerCloserThanRadius(player.radius + scorpion.radius, scorpion))
				{
					player.isThereBlood = true;
					scorpion.touchingPlayer = true;
				}
			}
		}
	}
}

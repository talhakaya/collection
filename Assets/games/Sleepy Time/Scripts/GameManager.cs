using Collection.Controls;
using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// The root of the game, and the clock everything else reads. Ported from GameManager.as.
	///
	/// Time is integer milliseconds, everywhere, exactly as in the original: the song clock
	/// starts at TimeBeginningTheSong (-2000) for the two-second lead-in, rhythm is
	/// milliseconds per beat and changes per song, and Beat.timingMax is a 200 ms window.
	/// Nothing here is ever converted to seconds.
	///
	/// It also owns the screens. They overlap: newScreen slides the incoming one in over a
	/// second while the outgoing one leaves over two, and garbageCollector only destroys a
	/// screen once it has actually reached its off-stage coordinate. GameManager outlives
	/// all of them, which is why the Pause and Menu buttons it creates persist everywhere.
	/// </summary>
	public class GameManager : FlashObject
	{
		public static int time;
		public static int dt;
		public static bool keyDown = false;
		public static bool keyboardDown = false;
		public static bool mouseDown = false;
		public static bool mouseDownOld = false;
		public static int rhythm = 500;
		public static bool paused = false;
		public static int id = 0;

		// enterFrameHandler computed these from the live stage size and then immediately set
		// both back to 1, so they were only ever 1. Button.isColliding multiplies its hit box
		// by them, so they're kept rather than folded away.
		public static float ScaleX = 1f;
		public static float ScaleY = 1f;

		public static int TimeBeginningTheSong = -2000;

		/// Longest single step the lead-in accumulator will take, in ms - about six frames.
		private const int MaxAccumulatedDt = 100;

		public ScoreTable scoreTable;
		public SceneManager sceneManager;
		public Menu menu;
		public DialogueScreen dialogueScreen;
		public Button pauseButton;
		public Button menuButton;

		private int lastTime;
		private int currentTime;

		/// <summary>
		/// Statics survive a play-mode restart when domain reload is off, which would carry a
		/// finished song's clock into the next run. Cheaper to reset them than to rely on the
		/// project setting.
		/// </summary>
		public static void ResetStatics()
		{
			time = 0;
			dt = 0;
			keyDown = false;
			keyboardDown = false;
			mouseDown = false;
			mouseDownOld = false;
			rhythm = 500;
			paused = false;
			id = 0;
			ScaleX = 1f;
			ScaleY = 1f;
		}

		/// <summary>
		/// Transcribed exactly, because the shape of it decides the whole input plan: the
		/// hover check guards the *mouse* branch only. The mouse either clicks a button or
		/// plays the game, never both, while the keyboard always plays regardless of where
		/// the pointer is.
		///
		/// That is why gamepad South is bound into keyboardDown rather than emulating a
		/// cursor - with no emulated pointer to park over a button, this function needs no
		/// changes at all.
		/// </summary>
		public static bool getKeyDown()
		{
			keyDown = !paused && (!Button.isCollidingWithAny() && mouseDown || keyboardDown);
			return keyDown;
		}

		/// <summary>
		/// The per-frame alpha flicker that gives everything in this game its nervous look.
		///
		/// Deviation from the original, and the only reason this is a function at all:
		/// it stops while paused. enterFrameHandler had no paused check anywhere, so pausing
		/// froze dt but left the whole update chain running - and blink() re-rolls from
		/// Math.random() every frame regardless of dt, so a paused screen kept flickering.
		/// Holding the current value freezes the picture along with the clock.
		/// </summary>
		public static float blink(float current, float min, float range)
		{
			return paused ? current : min + Random.value * range;
		}

		public static int getTimer()
		{
			return (int)(Time.unscaledTime * 1000f);
		}

		/// <summary>
		/// GameManager.as's constructor. The stage listener registrations become polling in
		/// pollInput, and checkOtherKeys is gone with them: its Esc branch was empty and its
		/// F branch toggled fullscreen, which the Fullscreen button still does.
		/// </summary>
		public static GameManager New()
		{
			// Cleared here rather than from Awake. A node starts inactive - nothing draws
			// until addChild puts it on the display list - so Awake does not run until this
			// GameManager is added to the stage, which is after everything below has already
			// registered buttons and the screen's slide-in tween. Resetting from Awake wiped
			// both: the game stayed parked 800 px to the right and no button responded.
			ResetStatics();
			SceneManager.ResetStatics();
			Button.reset();
			Actuate.reset();
			SleepyGamepad.reset();

			GameManager gameManager = NewNode("GameManager").AddComponent<GameManager>();

			// SleepyTime.swf's header declares 800x450 at 60 fps, and several effects are
			// written per-frame rather than per-millisecond: Citmap.blink randomises alpha
			// every frame and updateGraphicsTimeline swaps line1/line2 every frame. Their
			// rate *is* the frame rate, so an uncapped editor strobes them several times too
			// fast. Golfinity pins 60 the same way.
			Application.targetFrameRate = 60;

			FlashBitmap bg = FlashBitmap.New(SleepyAssets.GetSprite("img/bg.jpg"));
			bg.scaleX = Main.stageWidth / 600f;
			bg.scaleY = Main.stageHeight / 600f;
			gameManager.addChild(bg);

			new SaveManager();
			new SoundManager();
			SoundManager.changeMusic("menu1");
			SoundManager.playMusic();

			// A first-time player is dropped straight into the opening dialogue; everyone else
			// gets the song select.
			if (SaveManager.playedBefore)
			{
				gameManager.menu = Menu.New();
				gameManager.newScreen(gameManager.menu);
			}
			else
			{
				gameManager.dialogueScreen = DialogueScreen.New();
				gameManager.newScreen(gameManager.dialogueScreen);
			}

			time = -2000;
			gameManager.lastTime = getTimer();

			// Created once here and kept by every screen for the rest of the session.
			gameManager.pauseButton = Button.New("Pause");
			gameManager.pauseButton.x = 60f;
			gameManager.pauseButton.y = 20f;
			gameManager.addChild(gameManager.pauseButton);

			gameManager.menuButton = Button.New("Menu");
			gameManager.menuButton.x = Main.stageWidth - 60f;
			gameManager.menuButton.y = 20f;
			gameManager.addChild(gameManager.menuButton);

			return gameManager;
		}

		/// <summary>
		/// Slides a screen in from the right while whatever is already showing slides out -
		/// one second in, two seconds out, so the two overlap. garbageCollector only destroys
		/// the outgoing screen once it has actually reached its off-stage coordinate.
		///
		/// Screens leave in different directions: the menu and score table slide left, while
		/// gameplay flies up and the dialogue drops down.
		/// </summary>
		public void newScreen(FlashObject screen)
		{
			screen.x = Main.stageWidth;
			addChild(screen);
			Actuate.tween(screen, 1f, x: 0f);

			// No "is this the screen we just added" guard, and none is needed: the incoming
			// screen's x was set to stageWidth two lines up and the tween has not run yet, so
			// a screen never slides itself out. That is how the original does it.
			if (scoreTable != null && scoreTable.x != Main.stageWidth)
			{
				Actuate.stop(scoreTable);
				Actuate.tween(scoreTable, 2f, x: -Main.stageWidth);
			}

			if (menu != null && menu.x != Main.stageWidth)
			{
				Actuate.stop(menu);
				Actuate.tween(menu, 2f, x: -Main.stageWidth);
			}

			if (sceneManager != null && sceneManager.x != Main.stageWidth)
			{
				Actuate.stop(sceneManager);
				Actuate.tween(sceneManager, 2f, y: -Main.stageHeight * 2f);
			}

			if (dialogueScreen != null && dialogueScreen.x != Main.stageWidth)
			{
				Actuate.stop(dialogueScreen);
				Actuate.tween(dialogueScreen, 2f, y: Main.stageHeight * 2f);
			}
		}

		/// <summary>
		/// Destroys a screen only once its outgoing tween has actually put it off stage,
		/// which is what lets transitions overlap - the incoming screen is already running
		/// while the old one is still sliding.
		///
		/// Each screen also has to take its buttons out of the static list with it. Unity
		/// needs one thing Flash did not: the GameObject destroyed, since removeChild only
		/// takes it off the display list.
		/// </summary>
		public void garbageCollector()
		{
			if (scoreTable != null && scoreTable.x == -Main.stageWidth)
			{
				Retire(scoreTable);
				scoreTable = null;
			}

			if (sceneManager != null && sceneManager.y == -Main.stageHeight * 2f)
			{
				Retire(sceneManager);
				sceneManager = null;
			}

			if (dialogueScreen != null && dialogueScreen.y == Main.stageHeight * 2f)
			{
				Retire(dialogueScreen);
				dialogueScreen = null;
			}

			if (menu != null && menu.x == -Main.stageWidth)
			{
				Retire(menu);
				menu = null;
			}
		}

		private void Retire(FlashObject screen)
		{
			removeChild(screen);

			for (int i = 0; i < Button.buttons.Count; i++)
			{
				if (Button.buttons[i] != null && Button.buttons[i].Parent == screen)
				{
					Button.buttons[i] = null;
				}
			}

			Button.cleanButtonsArray();
			Destroy(screen.gameObject);
		}

		/// The Fullscreen button, and what the original's F key did.
		public void switchFullScreen()
		{
			Screen.fullScreen = !Screen.fullScreen;
		}

		/// <summary>
		/// enterFrameHandler. The first four lines of the original computed ScaleX/ScaleY from
		/// the live stage size and then set both back to 1 on the next two, so only the clock,
		/// the buttons, the screens and the sound queue are real work.
		/// </summary>
		private void Update()
		{
			pollInput();
			calculateTime();
			buttonHandler();

			// Pause was mouse-only in Flash - the original bound no key to it at all - which
			// leaves a gamepad with no way to reach it, since the Pause/Menu buttons are
			// deliberately not gamepad-selectable. Start routes through the same
			// buttonPressHandler the mouse path calls, so the ordering quirk that makes
			// unpausing work applies to both.
			//
			// Deliberately not on the keyboard: Press is bound to anyKey, so any pause key
			// would also register as a note hit.
			if (pauseButton != null && TaloketoInputManager.GetButtonDown("Pause"))
			{
				buttonPressHandler(pauseButton);
			}

			// The Menu button is never gamepad-selectable - putting it in reach of the stick
			// would give gameplay screens a selectable button again, and with it the South
			// ambiguity the whole selection rule exists to avoid. East reaches it directly
			// instead, so a pad can get back to the song select without a mouse.
			if (menuButton != null && TaloketoInputManager.GetButtonDown("Menu"))
			{
				buttonPressHandler(menuButton);
			}

			// After buttonHandler, so the mouse keeps first claim on a button they both point
			// at, and before the screens, so a confirm this frame is acted on immediately.
			SleepyGamepad.update(this);

			if (sceneManager != null)
			{
				sceneManager.Tick();
				if (sceneManager.activeScene.destroyMePlease && !sceneManager.activeScene.destroyMePleaseMessageTaken)
				{
					// Each song hands over to a specific menu track. Song 6 (id 5) is the
					// exception and deliberately keeps playing into the score screen.
					if (id == 0 || id == 3)
					{
						SoundManager.changeMusic("menu2");
					}
					else if (id == 1 || id == 4)
					{
						SoundManager.changeMusic("menu3");
					}
					else if (id == 2)
					{
						SoundManager.changeMusic("menu1");
					}

					if (id != 5)
					{
						SoundManager.pauseMusic();
						SoundManager.playMusic();
					}

					sceneManager.activeScene.destroyMePleaseMessageTaken = true;
					scoreTable = ScoreTable.New(sceneManager.activeScene.score);
					newScreen(scoreTable);
				}
			}

			if (scoreTable != null)
			{
				scoreTable.Tick();
			}

			if (dialogueScreen != null)
			{
				dialogueScreen.Tick();
				if (dialogueScreen.destroyMePlease && !dialogueScreen.destroyMePleaseMessageTaken)
				{
					dialogueScreen.destroyMePleaseMessageTaken = true;
					time = -2000;
					// id 6 is the ending and anything negative is the tutorial or the
					// fullscreen note - none of them start a song, so they go back to the menu.
					if (id == 6 || id < 0)
					{
						menu = Menu.New();
						newScreen(menu);
					}
					else
					{
						sceneManager = SceneManager.New();
						newScreen(sceneManager);
					}
				}
			}

			if (menu != null)
			{
				menu.Tick();
			}

			SoundManager.update();

			// Actuate ran on its own ENTER_FRAME in Flash, so it is driven here rather than
			// from a screen - and it keeps running while paused, as it did there.
			Actuate.update();

			garbageCollector();
		}

		/// <summary>
		/// Walks every registered button once per frame. A button under the pointer either
		/// fires - but only on the frame the mouse goes down, which is what mouseDownOld
		/// tracks - or just gets its hover feedback.
		/// </summary>
		public void buttonHandler()
		{
			for (int i = 0; i < Button.buttons.Count; i++)
			{
				Button.buttons[i].Tick();
				if (Button.buttons[i].isColliding())
				{
					if (mouseDown && !mouseDownOld)
					{
						buttonPressHandler(Button.buttons[i]);
					}
					else
					{
						Button.buttons[i].mouseOverHandler();
					}
				}
			}

			mouseDownOld = mouseDown;
		}

		/// <summary>
		/// Every button in the game arrives here, identified by its label.
		///
		/// Note the ordering: "Pause" is handled *before* the !paused check, which is the only
		/// reason you can unpause - every other button is dead while the game is paused.
		/// Preserve that.
		///
		/// The destroyMePlease guards are what stop a double click queueing two screens.
		/// </summary>
		public void buttonPressHandler(Button button)
		{
			if (button.text == "Pause")
			{
				if (paused)
				{
					paused = false;
					SoundManager.playMusic();
				}
				else
				{
					paused = true;
					SoundManager.pauseMusic();
				}
			}
			else if (button.text == "Menu")
			{
				// Deliberately outside the !paused gate, unlike the original. Pause and Menu are
				// the two buttons present on every screen, and leaving Menu inside meant pausing
				// a song trapped you there - the only way out was to unpause first.
				if (menu == null)
				{
					// Dropped on the way out, or the song select would arrive still paused and
					// every button on it would be inert for exactly the same reason.
					bool wasPaused = paused;
					paused = false;

					// Leaving a song mid-play swaps back to menu music; leaving any other
					// screen keeps whatever is already playing - but if the pause had stopped
					// the channel, it still has to be started again, or the music would stay
					// silent for the rest of the session.
					if (sceneManager != null)
					{
						SoundManager.pauseMusic();
						SoundManager.changeMusic("menu1");
						SoundManager.playMusic();
					}
					else if (wasPaused)
					{
						SoundManager.playMusic();
					}

					menu = Menu.New();
					newScreen(menu);
				}
			}
			else if (!paused)
			{
				if (button.text == "Retry")
				{
					if (!scoreTable.destroyMePlease)
					{
						scoreTable.destroyMePlease = true;
						scoreTable.destroyMePleaseMessageTaken = true;
						sceneManager = SceneManager.New();
						newScreen(sceneManager);
					}
				}
				else if (button.text == "Next Level" || button.text == "Finish")
				{
					if (!scoreTable.destroyMePlease)
					{
						scoreTable.destroyMePlease = true;
						scoreTable.destroyMePleaseMessageTaken = true;
						++id;
						dialogueScreen = DialogueScreen.New();
						newScreen(dialogueScreen);
					}
				}
				else if (button.text == "Twitter")
				{
					if (Screen.fullScreen)
					{
						switchFullScreen();
					}

					Application.OpenURL("https://twitter.com/kayabros");
				}
				else if (button.text == "Soundtrack")
				{
					if (Screen.fullScreen)
					{
						switchFullScreen();
					}

					Application.OpenURL("http://talhakaya.bandcamp.com/album/sleepy-time-soundtrack");
				}
				else if (button.text == "Song 1")
				{
					if (!menu.destroyMePlease)
					{
						menu.destroyMePlease = true;
						id = 0;
						dialogueScreen = DialogueScreen.New();
						newScreen(dialogueScreen);
					}
				}
				else if (button.text == "Song 2")
				{
					if (!menu.destroyMePlease)
					{
						menu.destroyMePlease = true;
						id = 1;
						dialogueScreen = DialogueScreen.New();
						newScreen(dialogueScreen);
					}
				}
				else if (button.text == "Song 3")
				{
					if (!menu.destroyMePlease)
					{
						menu.destroyMePlease = true;
						id = 2;
						dialogueScreen = DialogueScreen.New();
						newScreen(dialogueScreen);
					}
				}
				else if (button.text == "Song 4")
				{
					if (!menu.destroyMePlease)
					{
						menu.destroyMePlease = true;
						id = 3;
						dialogueScreen = DialogueScreen.New();
						newScreen(dialogueScreen);
					}
				}
				else if (button.text == "Song 5")
				{
					if (!menu.destroyMePlease)
					{
						menu.destroyMePlease = true;
						id = 4;
						dialogueScreen = DialogueScreen.New();
						newScreen(dialogueScreen);
					}
				}
				else if (button.text == "Song 6")
				{
					if (!menu.destroyMePlease)
					{
						menu.destroyMePlease = true;
						id = 5;
						dialogueScreen = DialogueScreen.New();
						newScreen(dialogueScreen);
					}
				}
				else if (button.text == "Tutorial")
				{
					if (!menu.destroyMePlease)
					{
						menu.destroyMePlease = true;
						id = -1;
						dialogueScreen = DialogueScreen.New();
						newScreen(dialogueScreen);
					}
				}
				else if (button.text == "Fullscreen")
				{
					switchFullScreen();
					if (!menu.destroyMePlease)
					{
						menu.destroyMePlease = true;
						id = -2;
						dialogueScreen = DialogueScreen.New();
						newScreen(dialogueScreen);
					}
				}
			}
		}

		/// <summary>
		/// The one deliberate deviation from the original.
		///
		/// GameManager.as accumulated getTimer() deltas into time and synced the music to it
		/// exactly once, when time crossed 0 (SceneManager.update). Over a three-minute song
		/// that drifts against the audio clock, and every hit window is only 200 ms wide, so
		/// the inversion is worth it: while a song is playing, time is read from the audio
		/// clock and the accumulator only covers the -2000 to 0 lead-in.
		///
		/// dt stays the difference between successive values of time, so everything that
		/// integrates dt (Citmap.rotate, Spike, the particles) keeps agreeing with it.
		///
		/// _temp_1.time in the decompiled calculateTime is GameManager.time - the decompiler
		/// lost the target of the compound assignment.
		/// </summary>
		public void calculateTime()
		{
			currentTime = getTimer();
			if (!paused)
			{
				int musicTime;
				if (SoundManager.TryGetSyncedTime(out musicTime))
				{
					dt = musicTime - time;
					time = musicTime;
				}
				else
				{
					// Clamped, because this path only covers the -2000 lead-in and a single
					// editor hitch there is measured in seconds: it would swallow the whole
					// lead-in, and fling every oscillator that integrates dt far outside its
					// range - the sway is written to reverse when it passes rhythm / 2, not
					// to clamp, so one huge step leaves the entire screen visibly tilted for
					// seconds afterwards. Flash never saw a stall this size.
					dt = Mathf.Min(currentTime - lastTime, MaxAccumulatedDt);
					time += dt;
				}
			}
			else
			{
				dt = 0;
			}

			lastTime = currentTime;
		}

		/// <summary>
		/// Replaces the stage's KEY_DOWN/KEY_UP and MOUSE_DOWN/MOUSE_UP listeners. The
		/// original set keyboardDown for every key except Esc (27) and F (70, its fullscreen
		/// toggle) - the Press action is bound to anyKey for that reason, and fullscreen is
		/// the collection's job now, so neither special case survives.
		/// </summary>
		public void pollInput()
		{
			keyboardDown = TaloketoInputManager.GetButton("Press");
			mouseDown = TaloketoInputManager.GetMouseButton(0);
		}

		protected override void Awake()
		{
			base.Awake();
			lastTime = getTimer();
		}
	}
}

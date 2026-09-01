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
	/// The screen/button half of GameManager.as lands with Button and the screen classes;
	/// this is the clock and the raw input state it needs.
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

		public SceneManager sceneManager;
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

		public static int getTimer()
		{
			return (int)(Time.unscaledTime * 1000f);
		}

		/// <summary>
		/// GameManager.as's constructor, as far as it goes without the screens and buttons.
		/// The stage listener registrations become polling in pollInput; fullscreen is the
		/// collection's job now, so the Esc/F handling is gone.
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

			// The Menu / DialogueScreen chosen from SaveManager.playedBefore lands with the
			// screen classes. Until then there is no way into a song, so one starts directly.
			gameManager.sceneManager = SceneManager.New();
			gameManager.newScreen(gameManager.sceneManager);

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
		/// The branches for the other screens land with them.
		/// </summary>
		public void newScreen(FlashObject screen)
		{
			screen.x = Main.stageWidth;
			addChild(screen);
			Actuate.tween(screen, 1f, x: 0f);

			// No "is this the screen we just added" guard, and none is needed: the incoming
			// screen's x was set to stageWidth two lines up and the tween has not run yet, so
			// a screen never slides itself out. That is how the original does it.
			if (sceneManager != null && sceneManager.x != Main.stageWidth)
			{
				Actuate.stop(sceneManager);
				Actuate.tween(sceneManager, 2f, y: -Main.stageHeight * 2f);
			}
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

			if (sceneManager != null)
			{
				sceneManager.Tick();
				// The hand-off to ScoreTable when activeScene.destroyMePlease goes up lands
				// with the screens.
			}

			SoundManager.update();

			// Actuate ran on its own ENTER_FRAME in Flash, so it is driven here rather than
			// from a screen - and it keeps running while paused, as it did there.
			Actuate.update();
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
		/// Note the ordering: "Pause" is handled *before* the !paused check, which is the
		/// only reason you can unpause. Preserve that.
		///
		/// The remaining labels - Retry, Next Level, Menu, the song buttons - land with the
		/// screens they belong to.
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
			else if (!paused)
			{
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

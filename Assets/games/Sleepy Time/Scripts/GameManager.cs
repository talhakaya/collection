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

			new SoundManager();
			SoundManager.changeMusic("menu1");
			SoundManager.playMusic();

			// SaveManager, and the Menu / DialogueScreen chosen from playedBefore, land with
			// the screen classes. So do the Pause and Menu buttons, which the constructor
			// creates once and every screen then keeps.

			time = -2000;
			gameManager.lastTime = getTimer();
			return gameManager;
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

			// buttonHandler() and the screen updates land with Button and the screens.

			SoundManager.update();
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
					dt = currentTime - lastTime;
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
			ResetStatics();
			lastTime = getTimer();
		}
	}
}

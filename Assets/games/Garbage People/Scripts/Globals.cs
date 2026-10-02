using System.Collections.Generic;

namespace Games.GarbagePeople
{
	/// <summary>
	/// The game's global variables and shared functions. Ported from consts.js, game.js and
	/// the globals at the end of bathroom1.js and dialogue.js.
	///
	/// They are statics, as globals were in the page; GarbagePeople resets them when the
	/// game starts, since statics outlive a scene in Unity where globals did not outlive a
	/// page load.
	/// </summary>
	public static class Globals
	{
		public const int WIDTH = PhaserGame.WIDTH;
		public const int HEIGHT = PhaserGame.HEIGHT;

		public static CursorKeys cursors;
		public static Key spacebar;
		public static string[] dialogueLines;
		public static string dialogueNextScene;
		public static Sound music;
		public static bool victory;
		public static int moneyAmount;
		public static List<Sprite> particles;
		public static int particleIndex;
		public static int attackTimeBase;
		public static int attackTimeLevel;
		public static int attackTimeLevelInc;
		public static int attackTimePrice;
		public static int movementSpeedBase;
		public static int movementSpeedLevel;
		public static int movementSpeedLevelInc;
		public static int movementSpeedPrice;
		public static bool isGameFinished;

		// bathroom1.js
		public static bool isMusicPlaying;
		public static bool isMovementLocked;
		public static bool isHitMade;
		public static bool canAnimatePlayer;
		public const double bathroom1XStart = 240;
		public const double bathroom1XEnd = 740;
		public const double bathroom1XOutside = 830;
		public const double bathroom1XRoadStart = 960;
		public const double bathroom1XRoadHit = 3600;
		public const double bathroom1XJumpStart = 3900;
		public const double bathroom1XJumpEnd = 4000;
		public const double bathroom1XLevelEnd = 4030;

		// dialogue.js
		public static int dialogueIndex;

		/// What consts.js and bathroom1.js set when the page loaded.
		public static void Reset()
		{
			cursors = null;
			spacebar = null;
			dialogueLines = new[]
			{
				"yo",
				"hey",
				"all good?",
				"yea",
			};
			dialogueNextScene = "bathroom0";
			music = null;
			victory = false;
			moneyAmount = 0;
			particles = null;
			particleIndex = 0;
			attackTimeBase = 60;
			attackTimeLevel = 0;
			attackTimeLevelInc = -2;
			attackTimePrice = 250;
			movementSpeedBase = 200;
			movementSpeedLevel = 0;
			movementSpeedLevelInc = 25;
			movementSpeedPrice = 250;
			isGameFinished = false;
			isMusicPlaying = false;
			isMovementLocked = false;
			isHitMade = false;
			canAnimatePlayer = true;
			dialogueIndex = 0;
		}

		public static bool isClose(GameObject obj0, GameObject obj1, double dist)
		{
			return PhaserMath.DistanceBetween(obj0.x, obj0.y, obj1.x, obj1.y) < dist;
		}

		/// Money, coin and banknote particles, a shake, and a sound, for a hit on a thing;
		/// more of everything when the hit finishes it off.
		public static void HandleHitEffects(Scene scene, Sprite thing)
		{
			if (thing.hp > 0)
			{
				moneyAmount += PhaserMath.Between(3, 7);
				for (int i = 0; i < 5; i++)
				{
					Particle(scene, thing.x, thing.y);
				}

				scene.tweens.add(new TweenConfig
				{
					targets = thing,
					rotation = PhaserMath.FloatBetween(-0.02, 0.02) * (9 - thing.hp),
					scaleX = 1 + PhaserMath.FloatBetween(-0.02, 0.02) * (9 - thing.hp),
					scaleY = 1 + PhaserMath.FloatBetween(-0.02, 0.02) * (9 - thing.hp),
					ease = "Sine.easeInOut",
					duration = 150,
					yoyo = true,
				});
				scene.sound.add("HitWeak" + PhaserMath.Between(0, 2)).play();
			}
			else
			{
				moneyAmount += PhaserMath.Between(15, 65);
				for (int i = 0; i < 20; i++)
				{
					Particle(scene, thing.x, thing.y);
				}

				scene.sound.add("HitStrong" + PhaserMath.Between(0, 1)).play();
			}
		}

		/// A pool of 100 hidden banknotes and coins, reused in turn by Particle.
		public static void InitParticles(Scene scene)
		{
			particles = new List<Sprite>();
			for (int i = 0; i < 100; i++)
			{
				if (PhaserMath.FloatBetween(0, 1) > 0.5)
				{
					Sprite p = scene.add.sprite(0, 0, "money");
					p.setAlpha(0);
					particles.Add(p);
				}
				else
				{
					Sprite p = scene.add.sprite(0, 0, "coin");
					p.setAlpha(0);
					particles.Add(p);
				}
			}
		}

		/// One particle: up and back down, drifting sideways, spinning and fading.
		public static void Particle(Scene scene, double x, double y)
		{
			Sprite p = particles[particleIndex];
			p.setAlpha(1);
			p.x = x + PhaserMath.FloatBetween(-50, 50);
			p.y = y + PhaserMath.FloatBetween(-50, 50);

			double dur = PhaserMath.FloatBetween(250, 500);
			scene.tweens.add(new TweenConfig
			{
				targets = p,
				y = "-= " + Num(PhaserMath.FloatBetween(0, 300)),
				ease = "Sine.easeInOut",
				duration = dur * 0.5,
				yoyo = true,
			});
			if (PhaserMath.FloatBetween(0, 1) > 0.5)
			{
				scene.tweens.add(new TweenConfig
				{
					targets = p,
					x = "+= " + Num(PhaserMath.FloatBetween(0, 100)),
					rotation = PhaserMath.FloatBetween(-3, 3),
					alpha = 0,
					ease = "Linear",
					duration = dur,
				});
			}
			else
			{
				scene.tweens.add(new TweenConfig
				{
					targets = p,
					x = "-= " + Num(PhaserMath.FloatBetween(0, 100)),
					rotation = PhaserMath.FloatBetween(-3, 3),
					alpha = 0,
					ease = "Linear",
					duration = dur,
				});
			}

			particleIndex++;
			if (particleIndex >= particles.Count) particleIndex = 0;
		}

		/// A number as JavaScript writes it into a string, whatever the machine's language.
		public static string Num(double value)
		{
			return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}

		/// Frames between hits.
		public static int getAttackTime()
		{
			return System.Math.Max(10, attackTimeBase + attackTimeLevel * attackTimeLevelInc);
		}

		/// Pixels a second.
		public static int getMovementSpeed()
		{
			return System.Math.Min(600, movementSpeedBase + movementSpeedLevel * movementSpeedLevelInc);
		}

		/// <summary>
		/// Not in the game: the name of a key in an on-screen prompt, or of the gamepad
		/// button that stands in for it while a pad is being used.
		/// </summary>
		public static string Prompt(string keyboard, string gamepad)
		{
			return PhaserInput.usingGamepad ? gamepad : keyboard;
		}
	}
}

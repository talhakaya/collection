using static Games.GarbagePeople.Globals;

namespace Games.GarbagePeople
{
	/// <summary>
	/// The opening: a boy brushing his teeth in a narrow bathroom, the light bulb swinging,
	/// eyes appearing in the dark. Ten seconds, no control. Ported from bathroom0.js.
	///
	/// Everything is cued off the progress of one ten-second timer, each cue firing on the
	/// frame the progress passes its mark.
	/// </summary>
	public class Bathroom0 : Scene
	{
		private Sprite bg;
		private Sprite lightbulb;
		private Sprite light;
		private Sprite eyes;
		private Sprite kid;
		private Sprite arm;
		private Sprite black0;
		private Sprite black1;
		private TimerEvent timer;
		private double progress;
		private double progressOld;

		public Bathroom0() : base("bathroom0")
		{
		}

		public override void create()
		{
			physics.world.setBoundsCollision(true, true, true, true);
			sound.add("Bathroom0").play();
			double bgWidth = HEIGHT * 392.0 / 720;
			double bgBlackWidth = (WIDTH - bgWidth) / 2;
			bg = add.image(WIDTH * 0.5, HEIGHT * 0.5, "bathroom0bg");
			bg.setDisplaySize(bgWidth, HEIGHT);
			lightbulb = add.sprite(WIDTH * 0.5, 0, "lightbulb0");
			lightbulb.setOrigin(0.5, 0);
			light = add.sprite(WIDTH * 0.5, HEIGHT * 0.5, "light");
			light.setAlpha(0.4);
			light.blendMode = BlendModes.ADD;
			light.setScale(1.3);
			eyes = add.image(WIDTH * 0.5 - 12, HEIGHT - 439, "eyes");
			kid = add.image(WIDTH * 0.5, HEIGHT, "kid");
			kid.setOrigin(0.5, 1);
			arm = add.image(WIDTH * 0.5 + 124, HEIGHT - 520 + 254, "arm0");
			arm.setOrigin(149.0 / 165, 151.0 / 169);
			arm.rotation = 0.01;
			black0 = add.image(bgBlackWidth * 0.5, HEIGHT * 0.5, "wallSide");
			black0.setDisplaySize(bgBlackWidth, HEIGHT);
			black1 = add.image(WIDTH - bgBlackWidth * 0.5, HEIGHT * 0.5, "wallSide");
			black1.setDisplaySize(-bgBlackWidth, HEIGHT);
			progress = 0.0;
			progressOld = 0.0;
			timer = time.addEvent(new TimerConfig { delay = 10000, callback = bathroom0End });
		}

		public override void update()
		{
			progress = timer.getProgress();
			if (progress > 0.0 && progressOld <= 0.0)
			{
				tweens.add(new TweenConfig { targets = arm, rotation = -0.04, ease = "Sine.easeInOut", duration = 50, delay = 0, repeat = 12, yoyo = true, repeatDelay = 50 });
			}
			else if (progress > 0.2 && progressOld <= 0.2)
			{
				swing(1);
			}
			else if (progress > 0.22 && progressOld <= 0.22)
			{
				tweens.add(new TweenConfig { targets = eyes, x = "-=5", ease = "Sine.easeInOut", duration = 50 });
			}
			else if (progress > 0.4 && progressOld <= 0.4)
			{
				tweens.add(new TweenConfig { targets = eyes, x = "+=5", ease = "Sine.easeInOut", duration = 50 });
			}
			else if (progress > 0.45 && progressOld <= 0.45)
			{
				tweens.add(new TweenConfig { targets = arm, rotation = -0.04, ease = "Sine.easeInOut", duration = 50, delay = 0, repeat = 25, yoyo = true, repeatDelay = 50 });
			}
			else if (progress > 0.7 && progressOld <= 0.7)
			{
				swing(-1);
			}
			else if (progress > 0.72 && progressOld <= 0.72)
			{
				tweens.add(new TweenConfig { targets = eyes, x = "+=5", ease = "Sine.easeInOut", duration = 50 });
			}
			else if (progress > 0.9 && progressOld <= 0.9)
			{
				tweens.add(new TweenConfig { targets = eyes, x = "-=5", ease = "Sine.easeInOut", duration = 50 });
			}
			else if (progress > 0.95 && progressOld <= 0.95)
			{
				tweens.add(new TweenConfig { targets = arm, rotation = -0.04, ease = "Sine.easeInOut", duration = 50, delay = 0, repeat = 5, yoyo = true, repeatDelay = 50 });
			}

			progressOld = progress;
		}

		/// <summary>
		/// The bulb swinging four times, smaller each time, and its light with it, then
		/// settling. The source writes this out twice, for progress 0.2 (starting one way)
		/// and 0.7 (starting the other); direction is the only difference.
		/// </summary>
		private void swing(int direction)
		{
			string first = direction > 0 ? "+=0." : "-=0.";
			string second = direction > 0 ? "-=0." : "+=0.";
			for (int i = 1; i <= 4; i++)
			{
				tweens.add(new TweenConfig { targets = lightbulb, rotation = first + (5 - i), ease = "Sine.easeInOut", duration = 250, delay = 0 + 500 * (i - 1), yoyo = true });
				tweens.add(new TweenConfig { targets = lightbulb, rotation = second + (5 - i), ease = "Sine.easeInOut", duration = 250, delay = 250 + 500 * (i - 1), yoyo = true });
				tweens.add(new TweenConfig { targets = light, x = WIDTH / 2 - direction * 50 * (5 - i), ease = "Sine.easeInOut", duration = 250, delay = 0 + 500 * (i - 1), yoyo = true });
				tweens.add(new TweenConfig { targets = light, x = WIDTH / 2 + direction * 50 * (5 - i), ease = "Sine.easeInOut", duration = 250, delay = 250 + 500 * (i - 1), yoyo = true });
			}

			tweens.add(new TweenConfig { targets = lightbulb, rotation = 0, ease = "Sine.easeInOut", duration = 250, delay = 500 * 5 });
			tweens.add(new TweenConfig { targets = light, x = WIDTH / 2, ease = "Sine.easeInOut", duration = 250, delay = 500 * 5 });
		}

		private void bathroom0End()
		{
			scene.start("bathroom1");
		}
	}
}

using static Games.GarbagePeople.Globals;

namespace Games.GarbagePeople
{
	/// <summary>
	/// The second bathroom, and the walk out. Ported from bathroom1.js.
	///
	/// Three seconds of brushing, then the wall is torn away by a giant hand (the creep),
	/// the toilet flies, the curtain blows. The boy walks right - out through the hole,
	/// down onto the road and along it, the camera pulling out - until rocks block the
	/// way and the game teaches the hit. Past the rocks he jumps off the end of the road,
	/// and the creep's dialogue starts.
	///
	/// The walk is a path: bathroom1SetPlayerY puts him at the height of the ground for
	/// his x, and locks movement where the script needs him to stop.
	/// </summary>
	public class Bathroom1 : Scene
	{
		public double width;
		public double height;
		private TimerEvent timer;
		private double progress;
		private double progressOld;
		private Sprite bgBlack;
		private Sprite bg;
		private Sprite curtain;
		private Sprite lightbulb;
		private Sprite toilet;
		private Sprite light;
		private Sprite player;
		private Sprite arm;
		private Sprite rock0;
		private Sprite rock1;
		private Sprite rock2;
		public Text text0;
		public Text text1;

		public Bathroom1() : base("bathroom1")
		{
		}

		public override void create()
		{
			isMusicPlaying = false;
			isMovementLocked = false;
			isHitMade = false;
			canAnimatePlayer = true;
			physics.world.setBoundsCollision(true, true, true, true);
			sound.add("Bathroom1").play();
			width = WIDTH * 6;
			height = HEIGHT * 1.5;
			cameras.main.setBounds(0, 0, width, height);
			Sprite sky = add.image(width * 0.5 + 480 + 265 + 320, height * 0.5, "sky");
			sky.setScale(width / 2 + 320, height / 256);
			add.image(1500, height - 200, "building0").setOrigin(0.5, 1);
			add.image(2200, height - 220, "building1").setOrigin(0.5, 1);
			add.image(2800, height - 220, "building2").setOrigin(0.5, 1);
			add.image(3300, height - 200, "building3").setOrigin(0.5, 1);
			for (int i = 1; i <= 5; i++)
			{
				add.image(i * 1015, height - 124, "road");
			}

			bgBlack = add.sprite(sky.x - 320, sky.y, "black");
			bgBlack.setScale(width / 16, height / 16);
			rock0 = add.image(bathroom1XRoadHit + 90, height - 100, "rock0");
			rock1 = add.image(bathroom1XRoadHit + 90, height - 150, "rock1");
			rock2 = add.image(bathroom1XRoadHit + 90, height - 200, "rock2");
			add.image(bathroom1XJumpStart + 390, height, "creep").setOrigin(0.5, 1).setScale(1.2);
			bg = add.sprite(960 * 0.5, height - 270, "bg");
			cameras.main.startFollow(bg);
			cameras.main.setZoom(1.33);
			curtain = add.sprite(536, height - 540 + 264, "curtain0");
			lightbulb = add.sprite(400, height - 540 + 44, "lightbulb1");
			lightbulb.setOrigin(0.5, 0);
			toilet = add.sprite(480 + 255, height - 540 + 422, "toilet");
			light = add.sprite(400, height - 270, "light");
			light.setAlpha(0.4);
			light.blendMode = BlendModes.ADD;
			light.setScale(1.3);
			player = physics.add.sprite(340, bg.y, "kid_brush");
			arm = add.image(240 + 114, height - 520 + 272, "arm1");
			arm.setOrigin(49.0 / 64, 34.0 / 70);
			arm.rotation = 0.01;
			tweens.add(new TweenConfig { targets = arm, rotation = -0.04, ease = "Sine.easeInOut", duration = 50, delay = 0, repeat = 24, yoyo = true, repeatDelay = 50 });
			progress = 0.0;
			progressOld = 0.0;
			timer = time.addEvent(new TimerConfig { delay = 3000, callback = bathroom1Event0 });
			cursors = input.keyboard.createCursorKeys();
			spacebar = input.keyboard.addKey(KeyCodes.SPACE);
			anims.create(new AnimationConfig { key = "idle", frames = { "kid_idle0" }, frameRate = 12, repeat = -1 });
			anims.create(new AnimationConfig { key = "walk", frames = { "kid_walk0", "kid_walk1", "kid_walk2", "kid_walk3", "kid_walk4", "kid_walk5" }, frameRate = 8, repeat = -1 });
			anims.create(new AnimationConfig { key = "attack3", frames = { "kid_attack3" }, frameRate = 12, repeat = -1 });
			anims.create(new AnimationConfig
			{
				key = "curtainBlow",
				frames = { "curtain1", "curtain2", "curtain3", "curtain2", "curtain3", "curtain2", "curtain3", "curtain2", "curtain3", "curtain2", "curtain1", "curtain0" },
				frameRate = 8,
			});
			anims.create(new AnimationConfig { key = "curtainBlowSoft", frames = { "curtain0" }, frameRate = 4, repeat = -1 });
			bathroom1SetPlayerY(this, player);
		}

		public override void update()
		{
			progress = timer.getProgress();
			if (progress > 0.8 && progressOld <= 0.8)
			{
				bg.setTexture("bg_broken");
				arm.destroy();
				bgBlack.destroy();
				player.setTexture("kid_scared");
				curtain.play("curtainBlow");
				curtain.anims.chain("curtainBlowSoft");
				for (int i = 1; i <= 4; i++)
				{
					tweens.add(new TweenConfig { targets = lightbulb, rotation = "+=0." + (5 - i), ease = "Sine.easeInOut", duration = 250, delay = 0 + 500 * (i - 1), yoyo = true });
					tweens.add(new TweenConfig { targets = lightbulb, rotation = "-=0." + (5 - i), ease = "Sine.easeInOut", duration = 250, delay = 250 + 500 * (i - 1), yoyo = true });
					tweens.add(new TweenConfig { targets = light, x = 400 - 50 * (5 - i), ease = "Sine.easeInOut", duration = 250, delay = 0 + 500 * (i - 1), yoyo = true });
					tweens.add(new TweenConfig { targets = light, x = 400 + 50 * (5 - i), ease = "Sine.easeInOut", duration = 250, delay = 250 + 500 * (i - 1), yoyo = true });
				}

				tweens.add(new TweenConfig { targets = lightbulb, rotation = 0, ease = "Sine.easeInOut", duration = 250, delay = 500 * 5 });
				tweens.add(new TweenConfig { targets = light, x = 400, ease = "Sine.easeInOut", duration = 250, delay = 500 * 5 });
				tweens.add(new TweenConfig { targets = toilet, x = 224, y = height - 540 + 94, rotation = -4.5, ease = "Sine.easeInOut", duration = 150 });
				cameras.main.startFollow(player, false, 0.4, 0);
			}

			if (progress >= 1)
			{
				if (canAnimatePlayer && !isMovementLocked)
				{
					if (PhaserInput.JustDown(cursors.left) || PhaserInput.JustDown(cursors.right))
					{
						if (text0 != null) text0.destroy();
						player.play("walk");
					}
					else if (!cursors.right.isDown && !cursors.left.isDown)
					{
						player.play("idle");
					}

					if (cursors.left.isDown && !cursors.right.isDown)
					{
						player.setVelocityX(-200);
						player.setScale(-1, 1);
					}
					else if (cursors.right.isDown && !cursors.left.isDown)
					{
						player.setVelocityX(200);
						player.setScale(1, 1);
					}
					else
					{
						player.setVelocity(0);
					}
				}
				else if (!isHitMade && PhaserInput.JustDown(spacebar))
				{
					if (text1 != null) text1.destroy();
					sound.add("HitStrong0").play();
					player.play("attack3");
					time.addEvent(new TimerConfig { delay = 500, callback = bathroom1Event1 });
					isHitMade = true;
					canAnimatePlayer = false;
					isMovementLocked = false;
					tweens.add(new TweenConfig { targets = rock0, rotation = "+=3", x = "+=10", y = "+=10", ease = "Sine.easeInOut", duration = 100 });
					tweens.add(new TweenConfig { targets = rock1, rotation = "+=4", x = "+=50", y = "+=70", ease = "Sine.easeInOut", duration = 130 });
					tweens.add(new TweenConfig { targets = rock2, rotation = "-=6", x = "-=70", y = "+=140", ease = "Sine.easeInOut", duration = 150 });
				}

				bathroom1SetPlayerY(this, player);
			}

			progressOld = progress;
		}

		/// Puts the boy at the ground's height for where he is, and does what each stretch
		/// of the walk calls for.
		private static void bathroom1SetPlayerY(Bathroom1 scene, Sprite player)
		{
			isMovementLocked = false;
			if (player.x < bathroom1XStart)
			{
				player.x = bathroom1XStart;
			}
			else if (player.x > bathroom1XLevelEnd)
			{
				bathroom1End(scene);
				return;
			}

			if (player.x < bathroom1XEnd)
			{
				player.y = scene.height - 540 + 400;
			}
			else if (player.x < bathroom1XOutside)
			{
				double animRatio = (player.x - bathroom1XEnd) / (bathroom1XOutside - bathroom1XEnd);
				player.y = scene.height - 540 + 400 + animRatio * 50;
			}
			else if (player.x < bathroom1XRoadStart)
			{
				if (!isMusicPlaying)
				{
					isMusicPlaying = true;
					music = scene.sound.add("Outside");
					music.setLoop(true);
					music.play();
				}

				double animRatio = (player.x - bathroom1XOutside) / (bathroom1XRoadStart - bathroom1XOutside);
				player.y = scene.height - 540 + 450 - animRatio * 100;
			}
			else if (player.x < bathroom1XRoadHit)
			{
				player.y = scene.height - 540 + 350;
			}
			else if (player.x < bathroom1XJumpStart)
			{
				player.y = scene.height - 540 + 350;
				if (!isHitMade)
				{
					// The source makes this text anew every frame until the hit, each one
					// over the last, and only destroys the last. Identical text in the same
					// place looks the same as one; here it is one, made once and kept up to
					// date, rather than sixty more objects for every second spent waiting.
					string prompt = "Press " + Prompt("SPACE", "A") + " to hit";
					if (scene.text1 == null || scene.text1.destroyed)
					{
						scene.text1 = scene.add.text(player.x, player.y - 250, prompt, new TextStyle { align = "center" }).setFont("32px Arial Black").setFill("#ffffff").setShadow(2, 2, "#888888", 2);
					}
					else
					{
						scene.text1.setText(prompt);
						scene.text1.x = player.x;
						scene.text1.y = player.y - 250;
					}
					player.play("idle");
					isMovementLocked = true;
					player.setVelocityX(0);
				}
			}
			else if (player.x < bathroom1XJumpEnd)
			{
				double animRatio = (player.x - bathroom1XJumpStart) / (bathroom1XJumpEnd - bathroom1XJumpStart);
				player.y = scene.height - 540 + 350 - 200 * (animRatio - animRatio * animRatio) - 40 * animRatio;
				isMovementLocked = true;
			}
			else
			{
				player.y = scene.height - 540 + 310;
			}

			if (player.x < bathroom1XOutside)
			{
				scene.cameras.main.setZoom(1.33);
			}
			else if (player.x < bathroom1XOutside + 500)
			{
				double animRatio = (player.x - bathroom1XOutside) / 500;
				scene.cameras.main.setZoom(1.33 - animRatio * 0.43);
			}
			else
			{
				scene.cameras.main.setZoom(0.9);
			}
		}

		private void bathroom1Event0()
		{
			canAnimatePlayer = true;
			text0 = add.text(player.x - 250, player.y - 250, "Press DIRECTION BUTTONS to move", new TextStyle { align = "center" }).setFont("32px Arial Black").setFill("#ffffff").setShadow(2, 2, "#888888", 2);
		}

		private void bathroom1Event1()
		{
			canAnimatePlayer = true;
		}

		private static void bathroom1End(Bathroom1 scene)
		{
			dialogueLines = new[]
			{
				"You are the chosen one.",
				"Did you just break my bathroom wall?",
				"Yes, that happened.",
				"You are going to pay for that.",
				"We have bigger issues at hand.",
				"Like what?",
				"How do you feel these days?",
				"I feel OK?",
				"No, think about it. How do you feel?",
				"I dunno. Good?",
				"Don't you feel like garbage?",
				"Garbage?",
				"Yes. Don't you? Garbage?",
				"What do you mean by feeling like garbage?",
				"Feeling bad, aimless, stupid. Cmon, catch up with me!",
				"Oh, yeah, I guess I do kind of feel like that.",
				"That is why you have been chosen.",
				"To do what exactly?",
				"Destroy some garbage.",
				"Why would I do that?",
				"$$$$$ I will pay you handsomely. $$$$$",
				"Ohhh okay! But can I get dressed before we do that?",
				"No.",
				"...",
				"Let's go.",
			};
			dialogueNextScene = "forest";
			scene.scene.start("dialogue");
		}
	}
}

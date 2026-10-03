using static Games.GarbagePeople.Globals;

namespace Games.GarbagePeople
{
	/// <summary>
	/// What the three levels share: forest.js, city.js and road.js are one file copied three
	/// times, differing only in music, ground colour, the things to smash and the missions.
	/// The shared lines are here, in the order the files run them; each level fills in the
	/// rest.
	///
	/// The boy walks a field three screens wide and three screens tall, the camera following
	/// at 0.8 zoom, and smashes things near him with Space for cash.
	/// </summary>
	public abstract class Level : Scene
	{
		protected double width;
		protected double height;
		protected double playerScale;
		protected int attackTime;

		protected Text text;
		protected Sprite player;

		protected Level(string key) : base(key)
		{
		}

		protected abstract string musicKey { get; }

		/// The counts and goals, the ground and the things to smash.
		protected abstract void createWorld();

		/// One swing: every thing in reach is hit.
		protected abstract void hitThings();

		/// The mission text, and the victory when the goals are met.
		protected abstract void updateMissions();

		public override void create()
		{
			victory = false;
			if (music != null) music.stop();
			music = sound.add(musicKey);
			music.setLoop(true);
			music.play();

			anims.create(new AnimationConfig { key = "idle", frames = { "kid_idle0" }, frameRate = 12, repeat = -1 });
			anims.create(new AnimationConfig { key = "walk", frames = { "kid_walk0", "kid_walk1", "kid_walk2", "kid_walk3", "kid_walk4", "kid_walk5" }, frameRate = 8, repeat = -1 });
			anims.create(new AnimationConfig { key = "attack0", frames = { "kid_attack0" }, frameRate = 12, repeat = -1 });
			anims.create(new AnimationConfig { key = "attack1", frames = { "kid_attack1" }, frameRate = 12, repeat = -1 });
			anims.create(new AnimationConfig { key = "attack2", frames = { "kid_attack2" }, frameRate = 12, repeat = -1 });
			anims.create(new AnimationConfig { key = "attack3", frames = { "kid_attack3" }, frameRate = 12, repeat = -1 });

			width = WIDTH * 3;
			height = WIDTH * 3;
			createWorld();

			InitParticles(this);

			playerScale = 0.8;
			attackTime = 0;
			player = physics.add.sprite(width / 2, height / 2, "kid_idle0").setOrigin(0.5, 0.5).setScale(playerScale);
			cameras.main.startFollow(player);
			physics.world.setBoundsCollision(true, true, true, true);
			cameras.main.setBounds(0, 0, width, height);
			cameras.main.setZoom(0.8);

			text = add.text(-100, -50, "", new TextStyle { align = "left" }).setFont("32px Arial Black").setFill("#ffffff").setShadow(2, 2, "#444444", 2).setScrollFactor(0);

			cursors = input.keyboard.createCursorKeys();
			spacebar = input.keyboard.addKey(KeyCodes.SPACE);
		}

		/// The 160 scattered bits of ground detail every level has.
		protected void addDetails()
		{
			for (int i = 0; i < 160; i++)
			{
				add.image(PhaserMath.FloatBetween(0, width), PhaserMath.FloatBetween(0, height), "detail" + PhaserMath.Between(0, 6));
			}
		}

		public override void update()
		{
			if (victory) return;
			double speed = getMovementSpeed();
			if ((cursors.left.isDown || cursors.right.isDown) == (cursors.up.isDown || cursors.down.isDown))
			{
				speed *= 144.0 / 200;
			}

			if (attackTime <= 0)
			{
				if (PhaserInput.JustDown(cursors.left) || PhaserInput.JustDown(cursors.right) || PhaserInput.JustDown(cursors.up) || PhaserInput.JustDown(cursors.down))
				{
					player.play("walk");
				}
				else if (!cursors.right.isDown && !cursors.left.isDown && !cursors.up.isDown && !cursors.down.isDown)
				{
					player.play("idle");
				}

				if (cursors.left.isDown)
				{
					player.setVelocityX(-speed);
					player.setScale(-playerScale, playerScale);
				}
				else if (cursors.right.isDown)
				{
					player.setVelocityX(speed);
					player.setScale(playerScale, playerScale);
				}
				else
				{
					player.setVelocityX(0);
				}

				if (cursors.up.isDown)
				{
					player.setVelocityY(-speed);
				}
				else if (cursors.down.isDown)
				{
					player.setVelocityY(speed);
				}
				else
				{
					player.setVelocityY(0);
				}
			}

			if (attackTime <= 0)
			{
				if (spacebar.isDown)
				{
					player.setVelocityX(0);
					player.setVelocityY(0);
					attackTime = getAttackTime();
					player.play("attack" + PhaserMath.Between(0, 3));
					hitThings();
				}
			}
			else
			{
				attackTime -= 1;
			}

			updateMissions();
		}

		/// <summary>
		/// Hits a thing within reach: one hp off, the cash and effects, and when it is done
		/// for, debris in its place. Returns whether it was destroyed (and removed from
		/// things), so the level can count it, or else tint it by the hp left.
		///
		/// The source removes it from the list in the middle of looping over it without
		/// stepping back, so the thing after a destroyed one is skipped for that swing. The
		/// callers loop the same way, and keep that.
		/// </summary>
		protected bool hit(System.Collections.Generic.List<Sprite> things, int i, string debris)
		{
			Sprite thing = things[i];
			thing.hp--;
			HandleHitEffects(this, thing);
			if (thing.hp <= 0)
			{
				add.sprite(thing.x, thing.y, debris);
				thing.destroy();
				things.RemoveAt(i);
				return true;
			}

			return false;
		}

		/// The jubilant spin and the fanfare when the missions are done, then end after two
		/// seconds.
		protected void win(System.Action end)
		{
			victory = true;
			music.stop();
			sound.add("Victory").play();

			tweens.add(new TweenConfig
			{
				targets = player,
				rotation = PhaserMath.FloatBetween(-25, 25),
				scaleX = PhaserMath.FloatBetween(-25, 25),
				scaleY = PhaserMath.FloatBetween(-25, 25),
				ease = "Sine.easeInOut",
				duration = 2000,
			});

			time.addEvent(new TimerConfig { delay = 2000, callback = end });
		}
	}
}

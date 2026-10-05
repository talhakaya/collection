using System;
using System.Collections.Generic;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// What every level has in common. Ported from levels/Level.as.
	///
	/// A level is a Flixel state: its subclass fills in the tile map and places the
	/// objects in create(), and this class runs the rules - collisions, what happens when
	/// Hans touches each kind of thing, getting hurt, dying, leaving through the door.
	///
	/// Sequencing is done with nine frame-counting timers polled each step: start one, and
	/// some steps later its "complete" flag is up for exactly one step and the next thing
	/// happens. timer8 is "the door has been used"; sixty steps later, nextLevel().
	///
	/// Drawing order is the order things were added: timers, the background wall, then
	/// interacts, blocks, background objects, narrators and monsters, and after those
	/// whatever the subclass adds - its tile map, Hans, the cheerleader.
	/// </summary>
	public class Level : FlxState
	{
		private static string Sfxdie1 = "Level_Sfxdie1";
		private static string Sfxdie2 = "Level_Sfxdie2";
		private static string Sfxdie3 = "Level_Sfxdie3";
		private static string Sfxdie4 = "Level_Sfxdie4";
		private static string Sfxdie5 = "Level_Sfxdie5";
		private static string Sfxkuslar = "Level_Sfxkuslar";
		private static string Sfxhansongoomba = "Level_Sfxhansongoomba";
		private static string Sfxwind = "Level_Sfxwind";
		private static string Sfxblockfalling = "Level_Sfxblockfalling";
		private static string music = "Level_music";

		public FlxTilemap level;
		public FlxPoint scale;
		public Hans player;
		public Cheerleader cheerleader;
		public FlxGroup monsters;
		public FlxGroup interacts;
		public FlxGroup blocks;
		public FlxGroup narrators;
		public TTimer timer1;
		public TTimer timer2;
		public TTimer timer3;
		public TTimer timer4;
		public TTimer timer5;
		public TTimer timer6;
		public TTimer timer7;
		public TTimer timer8;
		public TTimer timer9;
		public FlxTimer timerForMusicFadeOut;
		public Narrator narrator;
		public Narrator narratorFollow;
		public Narrator narrator2;
		public bool editorMode;
		public int levelwidth;
		public int levelheight;
		public bool leversUsable;
		public double xprev;
		public double yprev;
		public bool isThereCheerlover;
		public Cheerlover cheerlover;
		public bool didCheerloverYell;
		public Narrator cheerloverYellText;
		public Machine machineForCheerleader;
		public FlxGroup bg;
		public FlxGroup bgObjects;
		private FlxText xAndY;
		public List<List<string>> bgDetails;
		public List<Bg> bgBirds;
		public List<Bg> bgPalms;
		public bool playedSfxdie;
		public bool cheerleaderTouchedDoor;
		public int levelno;
		public FlxSave gameSave;
		public bool noMusic;
		public bool created;

		public override void create()
		{
			base.create();
			if (levelno == 0)
			{
				levelno = gameSave.data.level ?? 0;
			}

			if (!noMusic && FlxG.music == null)
			{
				FlxG.playMusic(music);
			}

			timer1 = new TTimer();
			timer2 = new TTimer();
			timer3 = new TTimer();
			timer4 = new TTimer();
			timer5 = new TTimer();
			timer6 = new TTimer();
			timer7 = new TTimer();
			timer8 = new TTimer();
			timer9 = new TTimer();
			timerForMusicFadeOut = new FlxTimer();
			add(timer1);
			add(timer2);
			add(timer3);
			add(timer4);
			add(timer5);
			add(timer6);
			add(timer7);
			add(timer8);
			add(timer9);
			timer9.start(1);
			bgDetails = new List<List<string>>();
			bgBirds = new List<Bg>();
			bgPalms = new List<Bg>();
			bg = new FlxGroup();
			add(bg);
			addBg();
			interacts = new FlxGroup();
			add(interacts);
			blocks = new FlxGroup();
			add(blocks);
			bgObjects = new FlxGroup();
			add(bgObjects);
			narrators = new FlxGroup();
			add(narrators);
			monsters = new FlxGroup();
			add(monsters);
			editorMode = false;
			if (editorMode)
			{
				FlxG.mouse.hide();
			}
			else
			{
				FlxG.mouse.show();
			}

			leversUsable = true;
			isThereCheerlover = false;
			didCheerloverYell = false;
			cheerleaderTouchedDoor = false;
			if (editorMode)
			{
				xAndY = new FlxText(0, 0, 400, "", true);
				xAndY.color = 4294967295;
				add(xAndY);
			}

			playedSfxdie = false;
			created = true;
		}

		public override void update()
		{
			if (!created || player == null)
			{
				return;
			}

			base.update();
			FlxG.collide(level, player);
			FlxG.collide(level, monsters);
			FlxG.overlap<FlxSprite, Monster>(player, monsters, overlappedMonsters);
			FlxG.overlap<FlxSprite, FlxSprite>(player, interacts, overlapped);
			FlxG.overlap<FlxSprite, FlxSprite>(player, narrators, overlapped);
			if (player.y > 720 && levelheight < 120)
			{
				instantDeath();
			}

			if (editorMode)
			{
				// The source's built-in level editor: click to toggle tiles, E to print the
				// grid. editorMode is hard-coded false in create(), so it never runs; it is
				// kept so the levels can still be edited the way they were made.
				if (FlxG.keys.ENTER)
				{
					FlxG.resetState();
				}

				double xx = Math.Floor(FlxG.mouse.x / 8);
				double yy = Math.Floor(FlxG.mouse.y / 8);
				bool mouseMoved = false;
				if (xprev != xx || yprev != yy)
				{
					mouseMoved = true;
				}

				xprev = xx;
				yprev = yy;
				if (FlxG.mouse.justPressed())
				{
					uint tile = level.getTile((int)xx, (int)yy);
					level.setTile((int)xx, (int)yy, tile > 0 ? 0u : 1u);
				}

				if (FlxG.mouse.pressed())
				{
					if (mouseMoved)
					{
						uint tile = level.getTile((int)xx, (int)yy);
						level.setTile((int)xx, (int)yy, tile > 0 ? 0u : 1u);
					}
				}

				if (FlxG.keys.justPressed("E"))
				{
					string s = "";
					for (int i = 0; i < levelheight; i++)
					{
						for (int j = 0; j < levelwidth; j++)
						{
							s += level.getTile(j, i) + ", ";
						}

						s += "\n";
					}

					UnityEngine.Debug.Log(s);
				}

				xAndY.x = player.x - 12;
				xAndY.y = player.y - 16;
				xAndY.text = Math.Floor(player.x) + ", " + Math.Floor(player.y);
			}

			if (FlxG.keys.justPressed("ESCAPE"))
			{
				FlxG.switchState(new Menu());
			}

			if (timer1.complete)
			{
				fadeOut();
			}

			if (timer2.complete)
			{
				nextLevel2();
			}

			if (timer3.complete)
			{
				nextLevel3();
			}

			if (timer4.complete)
			{
				nextLevel();
			}

			if (timer5.complete)
			{
				die();
			}

			if (timer6.complete)
			{
				getWell();
			}

			if (timer7.complete)
			{
				die();
			}

			if (timer8.complete)
			{
				nextLevel();
			}

			if (timer9.complete)
			{
				add(player.timer);
				add(player.timer2);
				add(player.timer3);
			}

			for (int k = 0; k < bgBirds.Count; k++)
			{
				if (!bgBirds[k].birdFlied && player.x - bgBirds[k].x < 64 && bgBirds[k].x - player.x < 32 && player.y - bgBirds[k].y < 64 && bgBirds[k].y - player.y < 32)
				{
					bgBirds[k].birdFlied = true;
					bgBirds[k].play("fly");
					FlxG.play(Sfxkuslar);
				}
			}

			for (int k = 0; k < bgPalms.Count; k++)
			{
				if (!bgPalms[k].windEsti && player.x - bgPalms[k].x < 64 && bgPalms[k].x - player.x < 32)
				{
					bgPalms[k].wind();
					FlxG.play(Sfxwind);
				}
			}

			// In the collection: the M key that muted the game is gone.

			// Left out on purpose: the source adds a logoButton here, the sponsor's logo in
			// the corner of every level, linking to their site.
		}

		protected virtual void overlapped(FlxSprite Sprite1, FlxSprite Sprite2)
		{
			if (Sprite1 is Hans && Sprite2 is Door && player.interact)
			{
				interactWithDoor(Sprite2);
			}
			else if (Sprite1 is Hans && Sprite2 is Door2 && player.interact)
			{
				if (!isThereCheerlover)
				{
					interactWithDoor(Sprite2);
				}
				else if (player.x - cheerlover.x < 100)
				{
					interactWithDoor(Sprite2);
				}
				else if (!didCheerloverYell)
				{
					didCheerloverYell = true;
					cheerloverYellText = new Narrator(levelwidth * 8 - 200, 126, 180, 9999, false);
					narrators.add(cheerloverYellText);
					timer1.start(120);
				}
			}
			else if (Sprite1 is Hans && (Sprite2 is Lever || Sprite2 is Lever2) && player.interact && leversUsable)
			{
				Sprite2.kill();
			}
			else if (Sprite1 is Hans && Sprite2 is NarratorTouch)
			{
				Sprite2.kill();
			}
			else if (Sprite1 is Cheerleader && (Sprite2 is Door || Sprite2 is Door2) && !cheerleaderTouchedDoor)
			{
				Sprite2.kill();
				cheerleaderTouchedDoor = true;
			}
			else if (Sprite1 is Cheerlover && (Sprite2 is Door || Sprite2 is Door2) && !cheerleaderTouchedDoor)
			{
				Sprite2.kill();
				cheerleaderTouchedDoor = true;
			}
			else if (Sprite1 is Hans && Sprite2 is Machine && player.interact)
			{
				Sprite2.kill();
				timer2.start(180);
				add(new Narrator(16, 370, 150, 11, false));
				musicFadeOut();
			}
		}

		public void musicFadeOut()
		{
			timerForMusicFadeOut.start(0.2, 10, musicFadeOut2);
		}

		public void musicFadeOut2(FlxTimer a)
		{
			if (FlxG.music != null)
			{
				FlxG.music.volume -= 0.1;
			}
		}

		public void musicStop()
		{
			if (FlxG.music != null)
			{
				FlxG.music.stop();
				FlxG.music = null;
			}

			noMusic = true;
		}

		protected virtual void cheerleaderOverlappedWithMachine(Cheerleader Sprite1, Machine Sprite2)
		{
			Sprite2.kill();
			timer3.start(60);
			musicFadeOut();
		}

		protected void overlappedMonsters(FlxSprite Sprite1, Monster Sprite2)
		{
			if (Sprite1 is Hans && Sprite2 is MonsterGoomba && !Sprite2.isDead)
			{
				if (player.velocity.y > 0)
				{
					if (scale.x < 4)
					{
						Sprite2.isDead = true;
					}
					else
					{
						FlxG.play(Sfxhansongoomba);
					}

					player.velocity.y = -player.maxVelocity.y * 0.4;
					player.jumpThrottle = 0;
				}
				else if (Sprite1.y > Sprite2.y - 25)
				{
					getHurt();
				}
			}
			else if (Sprite1 is Hans && Sprite2 is MonsterGel && !Sprite2.isDead)
			{
				if (scale.x > 1)
				{
					if (player.velocity.y > 0)
					{
						Sprite2.isDead = true;
						player.velocity.y = -player.maxVelocity.y * 0.4;
						player.jumpThrottle = 0;
					}
					else if (Sprite1.y > Sprite2.y - 25)
					{
						getHurt();
					}
				}
				else
				{
					Sprite2.isDead = true;
				}
			}
			else if (Sprite1 is Hans && Sprite2 is MonsterGelBlue && !Sprite2.isDead)
			{
				instantDeath();
			}
			else if (Sprite1 is Hans && Sprite2 is MonsterHans && !Sprite2.isDead)
			{
				getHurt();
			}
		}

		public virtual void getWell()
		{
			FlxG.bgColor = 4289357414;
		}

		public virtual void die()
		{
			FlxG.resetState();
		}

		public virtual void nextLevel2()
		{
			timer4.start(60);
			player.fading = true;
			player.moveEnable = false;
		}

		public virtual void nextLevel3()
		{
			nextLevel2();
			cheerleader.gone = true;
		}

		public virtual void nextLevel()
		{
		}

		public virtual void getHurt()
		{
			player.getHurt();
			FlxG.bgColor = 2009866786;
			if (player.hp == 0)
			{
				instantDeath();
			}
			else
			{
				timer6.start(240);
			}
		}

		public virtual void instantDeath()
		{
			FlxG.bgColor = 2009866786;
			timer7.start(30);
			player.fading = true;
			player.moveEnable = false;
			if (!playedSfxdie)
			{
				playedSfxdie = true;
				if (player.scale.x == 1)
				{
					FlxG.play(Sfxdie1);
				}
				else if (player.scale.x == 2)
				{
					FlxG.play(Sfxdie2);
				}
				else if (player.scale.x == 4)
				{
					FlxG.play(Sfxdie3);
				}
				else if (player.scale.x == 8)
				{
					FlxG.play(Sfxdie4);
				}
				else if (player.scale.x == 16)
				{
					FlxG.play(Sfxdie5);
				}
			}
		}

		public virtual void interactWithDoor(FlxSprite Sprite2)
		{
			Sprite2.kill();
			timer8.start(60);
			player.fading = true;
			player.moveEnable = false;
		}

		private void fadeOut()
		{
			cheerloverYellText.erase = true;
			didCheerloverYell = false;
		}

		public void createParticles(double _X, double _Y)
		{
			for (uint i = 0; i < 5; i++)
			{
				add(new ParticleBlack(_X, _Y));
			}
		}

		public void blockParticle(FlxTilemap _level, BlockFalling _block)
		{
			if (!_block.touchedFloor && _block.isTouching(FlxObject.FLOOR))
			{
				_block.touchedFloor = true;
				createParticles(_block.x + _block.width * 0.5, _block.y + _block.height);
				FlxG.play(Sfxblockfalling);
			}
		}

		public void blockCollision(Hans _hans, FlxSprite _block)
		{
			if (_block is BlockFalling && _block.velocity.y > 0 && _block.y + 90 < player.y)
			{
				instantDeath();
			}
			else if (_block is BlockDead && player.y + player.height <= _block.y)
			{
				_block.kill();
			}
		}

		/// Covers the level in 64-pixel wall tiles (scaled with everything else), then
		/// turns the ones addBgDetail named into their details.
		public void addBg()
		{
			int a = (int)Math.Ceiling(levelwidth / 8.0 / scale.x) + 1;
			int b = (int)Math.Ceiling(levelheight / 8.0 / scale.x) + 1;
			for (int i = 0; i < a; i++)
			{
				bgDetails.Add(new List<string>());
				for (int j = 0; j < b; j++)
				{
					bgDetails[i].Add("0");
				}
			}

			addBgDetail();
			for (int i = 0; i < a; i++)
			{
				for (int j = 0; j < b; j++)
				{
					Bg tempBg = new Bg(64 * scale.x * i, 64 * scale.x * j, scale);
					switch (bgDetails[i][j])
					{
						case "torch": tempBg.torch(); break;
						case "windowPalm": tempBg.windowPalm(); bgPalms.Add(tempBg); break;
						case "damaged": tempBg.damaged(); break;
						case "sun00": tempBg.sun00(); break;
						case "sun01": tempBg.sun01(); break;
						case "sun10": tempBg.sun10(); break;
						case "sun11": tempBg.sun11(); break;
						case "bird": tempBg.bird(); break;
						case "windowSmall": tempBg.windowSmall(); break;
						case "bird2": tempBg.bird2(); bgBirds.Add(tempBg); break;
						case "windowBig00": tempBg.windowBig00(); break;
						case "windowBig01": tempBg.windowBig01(); break;
						case "windowBig02": tempBg.windowBig02(); break;
						case "windowBig10": tempBg.windowBig10(); break;
						case "windowBig11": tempBg.windowBig11(); break;
						case "windowBig12": tempBg.windowBig12(); break;
						case "windowBig20": tempBg.windowBig20(); break;
						case "windowBig21": tempBg.windowBig21(); break;
						case "windowBig22": tempBg.windowBig22(); break;
						case "corner": tempBg.corner(); break;
						case "greyCloud": tempBg.greyCloud(); break;
						case "hans": tempBg.hans(); break;
					}

					add(tempBg);
				}
			}
		}

		public virtual void addBgDetail()
		{
		}
	}
}

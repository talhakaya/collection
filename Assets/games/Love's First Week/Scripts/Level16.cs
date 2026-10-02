using System;

namespace Games.LovesFirstWeek
{
	/// <summary>
	/// The ending: the two of them sitting, a last conversation, and THE END. Ported from
	/// Level16.as.
	///
	/// Its update does not call State1's - nor, as the source has it, the one that updates
	/// what is on screen - so nothing in the scene moves but the dialogue.
	/// </summary>
	public class Level16 : State1
	{
		private static string Asagi = "Level16_Asagi";

		public FlxText son;
		public FlxSprite asagi1;
		public bool asagiBas;

		public Level16(bool _is1player) : base(_is1player)
		{
		}

		public override void create()
		{
			int[] data;
			isMenuButton = false;
			bgYogunluk = 16;
			talhaVar = true;
			diyalogVar = true;
			levelwidth = 16;
			levelheight = 8;
			base.create();
			save.data.level = 16;
			data = new int[]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 7, 13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				15, 15, 15, 15, 16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				16, 16, 16, 16, 16, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			};
			level = new FlxTilemap();
			level.loadMap(FlxTilemap.arrayToCSV(data, levelwidth), Tile, 32, 32, FlxTilemap.AUTO);
			ondekiler.add(level);
			ondekiler.add(naz = new Naz(true, 166, 130));
			ondekiler.add(talha = new Naz(false, 150, 130));
			naz.sonBolum = true;
			talha.sonBolum = true;
			naz.play("otur1");
			talha.play("otur2");
			sarmasik(4, 3, -10, 100);
			agac("2", 50, 122);
			agac("2", 0, 122);
			agac("0", 20, 122);
			agac("0", -20, 122);
			agac("1", -10, 122);
			agac("1", 40, 122);
			agac("3", 60, 122);
			agac("3", 80, 122);
			agac("2", 175, 187);
			agac("1", 195, 187);
			agac("3", 220, 190);
			agac("0", 250, 230);
			kafaoku.x = -8;
			kafaoku.y = -8;
			if (save.data.lang == "tur")
			{
				diyaloglar.Add(new Diyalog(false, "Bugün Cuma."));
				diyaloglar.Add(new Diyalog(true, "Evet."));
				diyaloglar.Add(new Diyalog(false, "Beni terk edicek misin?"));
				diyaloglar.Add(new Diyalog(true, "Hayır."));
				diyaloglar.Add(new Diyalog(false, "..."));
				diyaloglar.Add(new Diyalog(true, "..."));
				diyaloglar.Add(new Diyalog(false, "Seni çok seviyorum."));
				diyaloglar.Add(new Diyalog(true, "Ben de seni çok seviyorum."));
				diyaloglar.Add(new Diyalog(false, "..."));
				diyaloglar.Add(new Diyalog(true, "Seni Pazar günü terk edicem."));
				diyaloglar.Add(new Diyalog(false, "..."));
				son = new FlxText(228 - 34, 113, 64, "SON");
			}
			else
			{
				diyaloglar.Add(new Diyalog(false, "Today's Friday."));
				diyaloglar.Add(new Diyalog(true, "Yes."));
				diyaloglar.Add(new Diyalog(false, "Will you really dump me?"));
				diyaloglar.Add(new Diyalog(true, "No."));
				diyaloglar.Add(new Diyalog(false, "..."));
				diyaloglar.Add(new Diyalog(true, "..."));
				diyaloglar.Add(new Diyalog(false, "I love you.."));
				diyaloglar.Add(new Diyalog(true, "I love you too."));
				diyaloglar.Add(new Diyalog(false, "..."));
				diyaloglar.Add(new Diyalog(true, "I'm gonna dump you on Sunday."));
				diyaloglar.Add(new Diyalog(false, "..."));
				son = new FlxText(228 - 33, 102, 64, "THE END");
			}

			son.setFormat("NES", 20, 16777215, "center", 2);
			add(son);
			son.alpha = 0;
			enOndekiler.add(asagi1 = new FlxSprite(320, 200, Asagi));
			asagi1.alpha = 0;
		}

		public override void update()
		{
			double xx = double.NaN;
			double yy = double.NaN;
			bool mouseMoved = false;
			int tile = 0;
			string s = null;
			int i = 0;
			int j = 0;
			if (FlxG.keys.justPressed("R"))
			{
				FlxG.resetState();
			}

			if (editorMode)
			{
				Mouse.show();
				xx = Math.Floor(FlxG.mouse.x / 32);
				yy = Math.Floor(FlxG.mouse.y / 32);
				mouseMoved = false;
				if (xprev != xx || yprev != yy)
				{
					mouseMoved = true;
				}

				xprev = xx;
				yprev = yy;
				if (FlxG.mouse.justPressed())
				{
					tile = (int)level.getTile((int)xx, (int)yy);
					level.setTile((int)xx, (int)yy, tile > 0 ? 0u : 1u);
				}

				if (FlxG.mouse.pressed())
				{
					if (mouseMoved)
					{
						tile = (int)level.getTile((int)xx, (int)yy);
						level.setTile((int)xx, (int)yy, tile > 0 ? 0u : 1u);
					}
				}

				if (FlxG.keys.justPressed("E"))
				{
					s = "";
					for (i = 0; i < levelheight; i++)
					{
						for (j = 0; j < levelwidth; j++)
						{
							s += level.getTile(j, i) + ", ";
						}

						s += "\n";
					}

				}

				xAndY.x = camera.x - 12;
				xAndY.y = camera.y - 16;
			}

			if (isFullscreenAvailable)
			{
				if (FlxG.keys.justPressed("F"))
				{
					if (FlxG.stage.displayState == StageDisplayState.NORMAL)
					{
						FlxG.stage.displayState = StageDisplayState.FULL_SCREEN;
						FlxCamera.defaultZoom = 3;
						FlxG.resetCameras(new FlxCamera(0, 0, FlxG.width, FlxG.height));
						FlxG.camera.setBounds(0, 0, levelwidth * 32, levelheight * 32, true);
						FlxG.camera.follow(camera, FlxCamera.STYLE_LOCKON);
					}
					else
					{
						FlxG.stage.displayState = StageDisplayState.NORMAL;
						FlxCamera.defaultZoom = 2;
						FlxG.resetCameras(new FlxCamera(0, 0, FlxG.width, FlxG.height));
						FlxG.camera.setBounds(0, 0, levelwidth * 32, levelheight * 32, true);
						FlxG.camera.follow(camera, FlxCamera.STYLE_LOCKON);
					}
				}

				if (FlxG.stage.displayState == StageDisplayState.NORMAL && FlxCamera.defaultZoom == 3)
				{
					FlxCamera.defaultZoom = 2;
					FlxG.resetCameras(new FlxCamera(0, 0, FlxG.width, FlxG.height));
					FlxG.camera.setBounds(0, 0, levelwidth * 32, levelheight * 32, true);
					FlxG.camera.follow(camera, FlxCamera.STYLE_LOCKON);
				}
			}

			if (controlNaz)
			{
				if (naz.x > camera.x + CAMERASPEED / 10)
				{
					camera.velocity.x = CAMERASPEED;
				}
				else if (naz.x < camera.x - CAMERASPEED / 10)
				{
					camera.velocity.x = -CAMERASPEED;
				}
				else
				{
					camera.velocity.x = 0;
				}

				if (naz.y > camera.y + CAMERASPEED / 10)
				{
					camera.velocity.y = CAMERASPEED;
				}
				else if (naz.y < camera.y - CAMERASPEED / 10)
				{
					camera.velocity.y = -CAMERASPEED;
				}
				else
				{
					camera.velocity.y = 0;
				}
			}
			else
			{
				if (talha.x > camera.x + CAMERASPEED / 10)
				{
					camera.velocity.x = CAMERASPEED;
				}
				else if (talha.x < camera.x - CAMERASPEED / 10)
				{
					camera.velocity.x = -CAMERASPEED;
				}
				else
				{
					camera.velocity.x = 0;
				}

				if (talha.y > camera.y + CAMERASPEED / 10)
				{
					camera.velocity.y = CAMERASPEED;
				}
				else if (talha.y < camera.y - CAMERASPEED / 10)
				{
					camera.velocity.y = -CAMERASPEED;
				}
				else
				{
					camera.velocity.y = 0;
				}
			}

			cameraX = camera.x;
			cameraY = camera.y;
			if (kapiCount == 0)
			{
				nextLevel();
			}
			else if (kapiCount > 0)
			{
				--kapiCount;
			}

			if (diyalogVar)
			{
				if (di == -1 && dj == -1)
				{
					diyalogBaslat();
					++di;
					diyalogCount = 6;
				}
				else if (di == diyaloglar.Count)
				{
					diyalogKapat();
					diyalogVar = false;
				}
				else if (diyaloglar[di].text.Length != dj)
				{
					if (FlxG.keys.S || FlxG.keys.DOWN || diyalogCount == 0)
					{
						++dj;
						diyalogUpdate();
						diyalogCount = 6;
					}
					else if (diyalogCount > 0)
					{
						--diyalogCount;
					}
				}
				else if (FlxG.keys.justPressed("S") || FlxG.keys.justPressed("DOWN"))
				{
					++di;
					diyalogCount = 6;
					dj = 0;
					diyalogText1.text = "";
					diyalogText2.text = "";
				}
			}
			else if (son.alpha < 1)
			{
				son.alpha += 0.005;
			}
			else
			{
				if (asagi1.alpha < 1)
				{
					asagi1.alpha += 0.01;
				}

				if (FlxG.keys.justPressed("DOWN") || FlxG.keys.justPressed("S"))
				{
					nextLevel();
				}
			}
		}

		public override void nextLevel()
		{
			base.nextLevel();
			FlxG.switchState(new Menu());
		}
	}
}

using System;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// Hans's shrinking machine, at the end of each world. Ported from Machine.as.
	///
	/// "kill" means "use". Starting it takes a second and a half, and while it runs it hums -
	/// quieter the further away Hans is, in the levels where that is switched on.
	/// </summary>
	public class Machine : FlxSprite
	{
		private static string S_machine = "Machine_S_machine";
		private static string SfxOpen = "Machine_SfxOpen";
		private static string SfxClose = "Machine_SfxClose";
		private static string Sfx = "Machine_Sfx";

		private bool used;
		public bool isOpen;
		public bool wait;
		public bool cheerleader;
		public FlxTimer timer;
		public FlxTimer timer2use;
		public bool leverUsed;
		private int count;
		private bool working;
		public Hans player;
		public bool playerSensitive;
		private double volume;

		public Machine(double _X, double _Y, bool _used, FlxPoint _scale)
		{
			leverUsed = false;
			scale = _scale;
			x = _X;
			y = _Y;
			isOpen = false;
			loadGraphic(S_machine, true, false, 32, 16, false);
			addAnimation("closed", new[] { 0 }, 1, false);
			addAnimation("start", new[] { 1, 2, 3 }, 2, false);
			addAnimation("open", new[] { 4, 8, 5, 9, 6, 10, 7, 11 }, 12, true);
			addAnimation("stop", new[] { 3, 2, 1, 0 }, 2, false);
			used = _used;
			if (!used)
			{
				play("closed");
				working = false;
			}
			else
			{
				play("open");
				working = true;
			}

			if (scale.x == 2)
			{
				width = 64;
				height = 32;
				centerOffsets();
			}
			else if (scale.x == 4)
			{
				width = 128;
				height = 64;
				centerOffsets();
			}
			else if (scale.x == 8)
			{
				width = 256;
				height = 128;
				centerOffsets();
			}
			else if (scale.x == 16)
			{
				width = 512;
				height = 256;
				centerOffsets();
			}

			cheerleader = false;
			wait = false;
			timer2use = new FlxTimer();
			timer = new FlxTimer();
			count = 0;
			playerSensitive = false;
		}

		public override void update()
		{
			base.update();
			if (used && working)
			{
				if (count == 10)
				{
					if (playerSensitive)
					{
						if (Math.Abs(player.x - x - scale.x * 10) > 160)
						{
							if (Math.Abs(player.x - x - scale.x * 10) > 320)
							{
								volume = 0;
							}
							else
							{
								volume = 25600 / (Math.Abs(player.x - x - scale.x * 10) * Math.Abs(player.x - x - scale.x * 10));
							}
						}
						else
						{
							volume = 1;
						}
					}
					else
					{
						volume = 1;
					}

					FlxG.play(Sfx, volume);
					count = 0;
				}

				++count;
			}
		}

		public override void kill()
		{
			if (!wait && !leverUsed)
			{
				timer2use.start(1, 1, usable);
				leverUsed = true;
				wait = true;
				if (!used)
				{
					play("start");
					timer.start(1.5, 1, openMode);
					used = true;
					FlxG.play(SfxOpen);
				}
				else
				{
					play("stop");
					timer.start(1.5, 1, closedMode);
					used = false;
					FlxG.play(SfxClose);
				}
			}
		}

		private void openMode(FlxTimer e)
		{
			play("open");
			working = true;
		}

		private void closedMode(FlxTimer e)
		{
			play("closed");
		}

		private void usable(FlxTimer e)
		{
		}
	}
}

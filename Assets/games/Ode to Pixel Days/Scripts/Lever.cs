namespace Games.OdeToPixelDays
{
	/// <summary>
	/// A lever Hans can throw. Ported from Lever.as.
	///
	/// "kill" means "use", as for doors: it toggles isOpen, which is what blocks watch, and
	/// then ignores further pulls for a second.
	/// </summary>
	public class Lever : FlxSprite
	{
		private static string S_lever = "Lever_S_lever";
		private static string S_leverkucuk = "Lever_S_leverkucuk";
		private static string Sfx = "Lever_Sfx";

		public bool used;
		public bool isOpen;
		private FlxTimer timer;
		private bool usable;

		public Lever(double _X, double _Y, bool _used, FlxPoint _scale, bool _small)
		{
			scale = _scale;
			x = _X;
			y = _Y;
			isOpen = false;
			if (_small)
			{
				loadGraphic(S_leverkucuk, true, false, 5, 5, false);
				width = 3 * scale.x;
				height = 5 * scale.x;
			}
			else
			{
				loadGraphic(S_lever, true, false, 9, 16, false);
				width = 9 * scale.x;
				height = 16 * scale.x;
			}

			addAnimation("closed", new[] { 0 }, 1, false);
			addAnimation("opened", new[] { 5 }, 1, false);
			addAnimation("open", new[] { 1, 2, 3, 4 }, 6, false);
			addAnimation("close", new[] { 3, 2, 1, 0 }, 6, false);
			used = _used;
			timer = new FlxTimer();
			usable = true;
			if (scale.x != 1)
			{
				centerOffsets();
			}
		}

		public override void kill()
		{
			if (usable && !used)
			{
				if (!isOpen)
				{
					play("open");
					isOpen = true;
				}
				else
				{
					play("close");
					isOpen = false;
				}

				usable = false;
				timer.start(1, 1, makeUsable);
				FlxG.play(Sfx);
			}
		}

		private void makeUsable(FlxTimer e)
		{
			usable = true;
		}
	}
}

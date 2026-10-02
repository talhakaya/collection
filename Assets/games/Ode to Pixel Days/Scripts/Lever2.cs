namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The boss room's levers, one under each light. Ported from Lever2.as.
	///
	/// Like Lever, but it springs straight back, and the level can switch all of them on and
	/// off - they only work while the pattern is to be answered.
	/// </summary>
	public class Lever2 : FlxSprite
	{
		private static string S_lever = "Lever2_S_lever";
		private static string S_leverkucuk = "Lever2_S_leverkucuk";
		private static string Sfx = "Lever2_Sfx";

		public bool used;
		public bool isOpen;
		private FlxTimer timer;
		public bool usable;
		public bool bossFight;

		public Lever2(double _X, double _Y, bool _used, FlxPoint _scale, bool _small)
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
			addAnimation("opened", new[] { 0 }, 1, false);
			addAnimation("usable", new[] { 5, 6, 7, 8, 9, 0 }, 12, false);
			addAnimation("notusable", new[] { 0, 9, 8, 7, 6, 5 }, 12, false);
			addAnimation("open", new[] { 1, 2, 3, 4, 3, 2, 1, 0 }, 12, false);
			addAnimation("close", new[] { 1, 2, 3, 4, 3, 2, 1, 0 }, 12, false);
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

		public void letBeUsable()
		{
			play("usable");
			usable = true;
		}

		public void notUsable()
		{
			play("notusable");
			usable = false;
		}
	}
}

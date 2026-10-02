namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The door of the coarser levels, drawn from a 5-pixel sheet. Ported from Door2.as.
	///
	/// As with Door, "kill" means "use": it plays the open-and-close animation.
	/// </summary>
	public class Door2 : FlxSprite
	{
		private static string S_kapi = "Door2_S_kapi";
		private static string Sfx = "Door2_Sfx";
		private static string Sfx2 = "Door2_Sfx2";

		private bool used;
		public bool isOpen;
		public bool cheerleader;

		public Door2(double _X, double _Y, bool _used, FlxPoint _scale)
		{
			scale = _scale;
			x = _X;
			y = _Y;
			isOpen = false;
			loadGraphic(S_kapi, true, false, 5, 5, false);
			addAnimation("closed", new[] { 0 }, 1, false);
			addAnimation("closedForever", new[] { 5 }, 1, false);
			addAnimation("open", new[] { 1, 2, 3, 4 }, 6, false);
			addAnimation("openAndClose", new[] { 1, 2, 3, 4, 3, 2, 1, 0 }, 6, false);
			used = _used;
			if (!used)
			{
				play("closed");
			}
			else
			{
				play("closedForever");
				FlxG.play(Sfx2);
			}

			cheerleader = false;
			width = 5 * scale.x;
			height = 5 * scale.x;
			centerOffsets();
		}

		public override void kill()
		{
			if (!used)
			{
				play("openAndClose");
				FlxG.play(Sfx);
			}
		}
	}
}

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The way out of a level. Ported from Door.as.
	///
	/// "Killing" a door opens it: Level's overlap callback kills whatever Hans interacts
	/// with, and Door overrides kill to play its animation instead of disappearing. A door
	/// created as used is the one Hans just came through, shut for good.
	/// </summary>
	public class Door : FlxSprite
	{
		private static string S_kapi = "Door_S_kapi";
		private static string Sfx = "Door_Sfx";
		private static string Sfx2 = "Door_Sfx2";

		private bool used;
		public bool isOpen;
		public bool cheerleader;

		public Door(double _X, double _Y, bool _used, FlxPoint _scale)
		{
			scale = _scale;
			x = _X;
			y = _Y;
			isOpen = false;
			loadGraphic(S_kapi, true, false, 20, 40, false);
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
			if (scale.x == 2)
			{
				width = 40;
				height = 80;
				centerOffsets();
			}
			else if (scale.x == 4)
			{
				width = 80;
				height = 160;
				centerOffsets();
			}
			else if (scale.x == 8)
			{
				width = 160;
				height = 320;
				centerOffsets();
			}
			else if (scale.x == 16)
			{
				width = 320;
				height = 640;
				centerOffsets();
			}
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

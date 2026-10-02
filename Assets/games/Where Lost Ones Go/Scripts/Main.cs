using System;

namespace Games.WhereLostOnesGo
{
	/// <summary>
	/// The document class. Ported from Main.as.
	///
	/// Builds the stage, bottom to top: the sound, the game (the photo, the bridge, the
	/// player), the rising sea, the on-screen text and particles, the message box, and the
	/// vignette over everything - which is also what is clicked. Scorpions, figures and
	/// drops are added later still, on top of the vignette.
	/// </summary>
	public class Main : Sprite
	{
		public static foreground Foreground;
		public static Main instance;

		public static double distanceBetweenSprites(DisplayObject s1, DisplayObject s2)
		{
			return Math.Sqrt(Math.Pow(s1.x - s2.x, 2) + Math.Pow(s1.y - s2.y, 2));
		}

		public static bool distanceBetweenSpritesCloserThanRadius(double radius, DisplayObject s1, DisplayObject s2)
		{
			return radius > distanceBetweenSprites(s1, s2);
		}

		public static bool distanceBetweenPlayerCloserThanRadius(double radius, DisplayObject s1)
		{
			bool result = false;
			if (Game.instance != null && Game.instance.player != null)
			{
				Point playerPos = Game.instance.player.globalPosition();
				result = radius > Math.Sqrt(Math.Pow(s1.x - playerPos.x, 2) + Math.Pow(s1.y - playerPos.y, 2));
			}

			return result;
		}

		public static bool distanceBetweenMouseCloserThanRadius(double radius, DisplayObject s1)
		{
			bool result = false;
			if (Game.instance != null && Game.instance.player != null)
			{
				result = radius > Math.Sqrt(Math.Pow(s1.x - Game.instance.stage.mouseX, 2) + Math.Pow(s1.y - Game.instance.stage.mouseY, 2));
			}

			return result;
		}

		/// Called once the document is on the stage.
		public void init()
		{
			instance = this;

			// Statics outlive a scene in Unity, where they did not outlive the page in Flash.
			Game.STATE = 0;

			Foreground = new foreground();
			addChild(new SoundManager());
			var screenTextHandler = new ScreenTextHandler();
			addChild(new Game());
			addChild(new SeaState4());
			addChild(screenTextHandler);
			addChild(new MessageBox());
			addChild(Foreground);
		}
	}
}

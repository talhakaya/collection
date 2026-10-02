namespace Games.WhereLostOnesGo
{
	// The library symbols from the game's assets.swf: one bitmap each, placed relative to
	// the symbol's registration point as the SWF has it.

	/// The photo of the Bosphorus Bridge (symbol17).
	public class background : MovieClip
	{
		public background()
		{
			bitmapSymbol("background", 0, 0);
		}
	}

	/// The vignette over the whole stage (symbol3).
	public class foreground : MovieClip
	{
		public foreground()
		{
			bitmapSymbol("foreground", 0, 0);
		}
	}

	/// A figure, 9 by 24, held near its middle (symbol11).
	public class human : MovieClip
	{
		public human()
		{
			bitmapSymbol("human", -4.5, -11.55);
		}
	}

	/// A scorpion, 22 by 16, held near its middle (symbol14).
	public class scorpion : MovieClip
	{
		public scorpion()
		{
			bitmapSymbol("scorpion", -12, -8.5);
		}
	}
}

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// Spikes. Ported from BlockDiken.as.
	/// </summary>
	public class BlockDiken : FlxSprite
	{
		private static string S_ = "BlockDiken_S_";

		public BlockDiken(double _x, double _y, FlxPoint _scale)
		{
			x = _x;
			y = _y;
			loadGraphic(S_, false, false, 48, 4, false);
			scale = _scale;
			width = 48 * scale.x;
			height = 4 * scale.x;
			if (scale.x == 3)
			{
				offset.y = -4;
				offset.x = -48;
			}
			else
			{
				centerOffsets();
			}

			immovable = true;
		}
	}
}

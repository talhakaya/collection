namespace Games.LovesFirstWeek
{
	/// <summary>
	/// A kick, as an invisible box in front of the kicker that lasts a quarter of a second;
	/// what it overlaps gets kicked. Ported from SoyutTekme.as.
	/// </summary>
	public class SoyutTekme : FlxSprite
	{
		public bool byNaz;
		private int count = 15;

		public SoyutTekme(double _X, double _Y, bool naz)
		{
			x = _X;
			y = _Y;
			byNaz = naz;
			makeGraphic(40, 8, 4294901760);
			alpha = 0;
		}

		public override void update()
		{
			base.update();
			--count;
			if (count < 0)
			{
				kill();
			}
		}
	}
}

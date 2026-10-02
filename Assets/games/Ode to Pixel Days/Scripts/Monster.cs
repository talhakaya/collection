namespace Games.OdeToPixelDays
{
	/// What the monsters share. Ported from Monster.as.
	public class Monster : FlxSprite
	{
		public bool isDead;
		public bool isDeadOnce;
		public bool isWalkin;
		public bool isLeft;
		public bool isJumpin;
		public double randomNumber;

		public Monster()
		{
			isDead = false;
		}
	}
}

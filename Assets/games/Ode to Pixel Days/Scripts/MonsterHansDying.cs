namespace Games.OdeToPixelDays
{
	/// <summary>
	/// What is left where the boss falls: a one-off dying animation. Ported from
	/// MonsterHansDying.as.
	/// </summary>
	public class MonsterHansDying : FlxSprite
	{
		private static string S_ = "MonsterHansDying_S_";
		private static string Sfx = "MonsterHansDying_Sfx";

		public MonsterHansDying(double _X, double _Y, FlxPoint _scale)
		{
			x = _X;
			y = _Y;
			scale = _scale;
			loadGraphic(S_, true, false, 32, 32, false);
			addAnimation("ol", new[] { 0, 1, 2, 3 }, 1.5, false);
			play("ol");
			maxVelocity.x = 100;
			maxVelocity.y = 400;
			acceleration.y = 400;
			drag.x = maxVelocity.x * 4;
			width = 64;
			height = 14;
			centerOffsets();
			offset.y = 34;
			FlxG.play(Sfx);
		}
	}
}

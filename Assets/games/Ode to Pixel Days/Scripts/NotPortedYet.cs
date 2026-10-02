namespace Games.OdeToPixelDays
{
	// Placeholders for classes the first milestone does not reach.
	//
	// Level.as tests what Hans has touched by type (is Door2, is Lever, is MonsterGoomba),
	// so those types have to exist for Level to be a complete transcription - but Level 1
	// contains none of them. Each of these is an empty stand-in carrying only the members
	// Level itself reads. They are replaced by real ports, one file per class like the
	// rest, as the levels that use them are brought over; this file should end up deleted.

	public class MonsterGoomba : Monster { }

	public class MonsterGel : Monster { }

	public class MonsterGelBlue : Monster { }

	public class MonsterHans : Monster { }

	public class Door2 : FlxSprite { }

	public class Lever : FlxSprite { }

	public class Lever2 : FlxSprite { }

	public class Machine : FlxSprite { }

	public class Cheerlover : FlxSprite { }

	public class BlockFalling : FlxSprite
	{
		public bool touchedFloor;
	}

	public class BlockDead : FlxSprite { }
}

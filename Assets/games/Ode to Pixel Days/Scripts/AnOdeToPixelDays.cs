namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The game. Ported from AnOdeToPixelDays.as, which is one line:
	/// super(320, 240, Menu, 2) - a 320x240 game, starting at the menu, drawn at twice
	/// its size.
	///
	/// This sits on a GameObject in the game's scene. The sponsor's logo animation that
	/// Main.as played before constructing the game is not part of the port.
	/// </summary>
	public class AnOdeToPixelDays : FlxGame
	{
		private void Awake()
		{
			// Not ported yet: the source starts at Menu. Until the menu exists the game
			// starts straight in the first level.
			Init(320, 240, () => new Level1(), 2);
		}
	}
}

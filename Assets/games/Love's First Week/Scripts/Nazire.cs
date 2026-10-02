namespace Games.LovesFirstWeek
{
	/// <summary>
	/// The game. Ported from Nazire.as, which is one line:
	/// super(455, 256, Menu, 1.5) - a 455x256 game, starting at the menu, drawn at one and
	/// a half times its size.
	///
	/// This sits on a GameObject in the game's scene. What Main.as showed before
	/// constructing the game - the sponsor's "play more games" screen and logo animation -
	/// is not part of the port.
	/// </summary>
	public class Nazire : FlxGame
	{
		private void Awake()
		{
			Init(455, 256, () => new Menu(), 1.5);
		}
	}
}

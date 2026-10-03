namespace Games.GarbagePeople
{
	/// <summary>
	/// Garbage People, the Phaser 3 game, in the collection. Its scenes in the order the
	/// page's game config lists them; the first one starts.
	/// </summary>
	public class GarbagePeople : PhaserGame
	{
		private void Awake()
		{
			Globals.Reset();
			// Not in the game: the title's prompt names the pad's button if a pad is
			// plugged in, before anything has been pressed.
			PhaserInput.usingGamepad = UnityEngine.InputSystem.Gamepad.current != null;
			Run(new Scene[]
			{
				new Loading(),
				new Bathroom0(),
				new Bathroom1(),
				new Dialogue(),
				new Forest(),
				new Store(),
				new City(),
				new Road(),
			});
		}
	}
}

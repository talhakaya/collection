namespace Games.WhereLostOnesGo
{
	/// <summary>
	/// The scene's entry point. The SWF's preloader drew a progress bar and then made a
	/// Main; there is nothing to load here.
	/// </summary>
	public class WhereLostOnesGo : FlashPlayer
	{
		private void Awake()
		{
			Run(() => new Main());
			((Main)root).init();
		}
	}
}

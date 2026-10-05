using static Games.GarbagePeople.Globals;

namespace Games.GarbagePeople
{
	/// <summary>
	/// The title screen, and the end screen when the game has been played through: the
	/// logo over three slowly turning, tinted shines. Ported from loading.js.
	///
	/// Space (or A on a pad) starts the game from the first bathroom. Money and upgrades
	/// are kept on a second play, as in the original.
	/// </summary>
	public class Loading : Scene
	{
		private Sprite shine0;
		private Sprite shine1;
		private Sprite shine2;
		private Sprite logo;
		private Text text;

		public Loading() : base("loading")
		{
		}

		public override void create()
		{
			shine2 = add.image(WIDTH / 2, HEIGHT / 2, "shine").setScale(5).setAlpha(0.5);
			shine2.setTint(944149);
			shine2.blendMode = BlendModes.ADD;

			shine1 = add.image(WIDTH / 2, HEIGHT / 2, "shine").setScale(3);
			shine1.setTint(944149).setAlpha(0.75);
			shine1.blendMode = BlendModes.ADD;

			shine0 = add.image(WIDTH / 2, HEIGHT / 2, "shine").setScale(2.5);
			shine0.setTint(944149);

			logo = add.image(WIDTH / 2, HEIGHT * 2 / 5, "logo");
			logo.blendMode = BlendModes.ADD;

			if (isGameFinished)
			{
				text = add.text(440, 500, "THANKS FOR PLAYING!!\n\nGame by Talha Kaya\n@taloketo\n\n{SPACE} to restart", new TextStyle { align = "center" }).setFont("32px Arial Black").setFill("#ffffff").setShadow(2, 2, "#944149", 2);
			}
			else
			{
				text = add.text(450, 530, "Game by Talha Kaya\n@taloketo\n\n{SPACE} to begin", new TextStyle { align = "center" }).setFont("32px Arial Black").setFill("#ffffff").setShadow(2, 2, "#944149", 2);
			}

			spacebar = input.keyboard.addKey(KeyCodes.SPACE);
		}

		public override void update()
		{
			shine0.rotation -= 0.004;
			shine1.rotation += 0.002;
			shine2.rotation -= 0.001;

			if (PhaserInput.JustDown(spacebar))
			{
				scene.start("bathroom0");
				if (music != null) music.stop();
			}
		}
	}
}

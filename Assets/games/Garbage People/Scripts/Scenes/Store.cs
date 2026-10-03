using static Games.GarbagePeople.Globals;

namespace Games.GarbagePeople
{
	/// <summary>
	/// The shop between levels: spend the money on movement speed (Z) or hit rate (X),
	/// 250 each, until it runs out. Ported from store.js.
	///
	/// On a pad the two are X and Y; the prompts say so while a pad is in use.
	/// </summary>
	public class Store : Scene
	{
		private Sprite shine0;
		private Sprite shine1;
		private Sprite shine2;
		private Key zButton;
		private Key xButton;
		private Text text;
		private bool isDone;

		public Store() : base("store")
		{
		}

		public override void create()
		{
			isDone = false;
			if (music != null) music.stop();
			music = sound.add("Outside");
			music.setLoop(true);
			music.play();
			shine2 = add.image(WIDTH / 2, HEIGHT / 2, "shine").setScale(5).setAlpha(0.5);
			shine2.setTint(944149);
			shine2.blendMode = BlendModes.ADD;

			shine1 = add.image(WIDTH / 2, HEIGHT / 2, "shine").setScale(3);
			shine1.setTint(944149).setAlpha(0.75);
			shine1.blendMode = BlendModes.ADD;

			shine0 = add.image(WIDTH / 2, HEIGHT / 2, "shine").setScale(2.5);
			shine0.setTint(944149);

			text = add.text(370, 200, "", new TextStyle { align = "left" }).setFont("32px Arial Black").setFill("#ffffff").setShadow(2, 2, "#944149", 2);

			zButton = input.keyboard.addKey(KeyCodes.Z);
			xButton = input.keyboard.addKey(KeyCodes.X);
		}

		public override void update()
		{
			shine0.rotation -= 0.004;
			shine1.rotation += 0.002;
			shine2.rotation -= 0.001;

			if (moneyAmount >= attackTimePrice || moneyAmount >= movementSpeedPrice)
			{
				text.setText("Cash to spend: " + moneyAmount + "$\n\nMovement speed: Level " + movementSpeedLevel + "\nPress " + Prompt("Z", "X") + " to upgrade for " + movementSpeedPrice + "$\n\nHit rate: Level " + attackTimeLevel + "\nPress " + Prompt("X", "Y") + " to upgrade for " + attackTimePrice + "$\n\n");
				if (PhaserInput.JustDown(zButton) && moneyAmount >= movementSpeedPrice)
				{
					moneyAmount -= movementSpeedPrice;
					movementSpeedLevel++;
					sound.add("HitStrong1").play();
				}

				if (PhaserInput.JustDown(xButton) && moneyAmount >= attackTimePrice)
				{
					moneyAmount -= attackTimePrice;
					attackTimeLevel++;
					sound.add("HitStrong1").play();
				}
			}
			else if (!isDone)
			{
				text.setText("All cash spent!\nGood job!\nWho needs groceries anyway!");
				isDone = true;
				time.addEvent(new TimerConfig { delay = 2000, callback = storeEnd });
			}
		}

		private void storeEnd()
		{
			scene.start("dialogue");
		}
	}
}

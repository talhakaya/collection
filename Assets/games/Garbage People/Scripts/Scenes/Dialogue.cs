using static Games.GarbagePeople.Globals;

namespace Games.GarbagePeople
{
	/// <summary>
	/// A conversation with the creep, between levels. Ported from dialogue.js.
	///
	/// The lines come from whichever scene led here (dialogueLines), the creep and the boy
	/// taking turns, Space for the next. After the last line the boy is flung up and away
	/// and the next scene (dialogueNextScene) starts.
	/// </summary>
	public class Dialogue : Scene
	{
		public Text textPlayer;
		public Text textCreep;
		public Sprite shine0;
		public Sprite shine1;
		public Sprite shine2;
		public Sprite player;
		public Sprite creep;

		public Dialogue() : base("dialogue")
		{
		}

		public override void create()
		{
			dialogueIndex = 0;
			if (music != null) music.stop();
			music = sound.add("Dialogue");
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

			creep = add.image(640, HEIGHT, "creep").setOrigin(0.5, 1);
			player = add.image(400, HEIGHT - 100, "kid_idle0").setOrigin(0.5, 0.8).setScale(0.6);

			textPlayer = add.text(60, 330, "", new TextStyle { align = "left" }).setFont("32px Arial Black").setFill("#eaffa2").setShadow(2, 2, "#944149", 2);

			textCreep = add.text(300, 230, "", new TextStyle { align = "right" }).setFont("32px Arial Black").setFill("#a993d3").setShadow(2, 2, "#944149", 2);

			spacebar = input.keyboard.addKey(KeyCodes.SPACE);
			dialogueSet(this);
		}

		public override void update()
		{
			shine0.rotation -= 0.004;
			shine1.rotation += 0.002;
			shine2.rotation -= 0.001;

			if (PhaserInput.JustDown(spacebar))
			{
				dialogueSet(this);
			}
		}

		private static void dialogueSet(Dialogue scene)
		{
			if (dialogueIndex >= dialogueLines.Length)
			{
				scene.textPlayer.setText("");
				scene.textCreep.setText("");
				dialogueEndAnim(scene);
			}
			else
			{
				if (dialogueIndex % 2 == 0)
				{
					scene.textPlayer.setText("");
					scene.textCreep.setText(dialogueLines[dialogueIndex]);
				}
				else
				{
					scene.textPlayer.setText(dialogueLines[dialogueIndex]);
					scene.textCreep.setText("");
				}
			}

			dialogueIndex++;
		}

		private static void dialogueEndAnim(Dialogue scene)
		{
			scene.tweens.add(new TweenConfig { targets = scene.player, y = "-=400", rotation = -4.5, ease = "Sine.easeInOut", duration = 1500 });
			scene.tweens.add(new TweenConfig { targets = scene.shine0, y = "-=100", ease = "Sine.easeInOut", duration = 1500 });
			scene.tweens.add(new TweenConfig { targets = scene.shine1, y = "-=100", ease = "Sine.easeInOut", duration = 1500 });
			scene.tweens.add(new TweenConfig { targets = scene.shine2, y = "-=100", ease = "Sine.easeInOut", duration = 1500 });
			scene.tweens.add(new TweenConfig { targets = scene.creep, y = "+=200", rotation = 0.1, ease = "Sine.easeInOut", duration = 1500 });

			scene.time.addEvent(new TimerConfig { delay = 1500, callback = scene.dialogueEnd });
		}

		private void dialogueEnd()
		{
			scene.start(dialogueNextScene);
		}
	}
}

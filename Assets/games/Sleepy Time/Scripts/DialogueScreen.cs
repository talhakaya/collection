using System.Collections.Generic;

namespace Games.SleepyTime
{
	/// <summary>
	/// The between-songs conversation. Ported from DialogueScreen.as.
	///
	/// Speakers alternate sides. There are two permanent containers, left and right, each
	/// holding a character and three lines of text; advancing swaps which one is on stage by
	/// tweening the incoming one to centre over a second and the outgoing one off over two,
	/// so they cross. The character inside a container is swapped out for the next speaker's
	/// artwork while it is off screen.
	///
	/// The 1000 ms debounce in inputHandler is what stops one press skipping the whole
	/// conversation, and it doubles as the gamepad's repeat guard.
	/// </summary>
	public class DialogueScreen : FlashObject
	{
		public static int PressConst = 1000;

		public TextTalha textRight3;
		public TextTalha textRight2;
		public TextTalha textRight1;
		public TextTalha textLeft3;
		public TextTalha textLeft2;
		public TextTalha textLeft1;
		public int pressCounter = 0;
		public List<string> linesThird = new List<string>();
		public List<string> linesSecond = new List<string>();
		public List<string> linesFirst = new List<string>();
		public bool isCharacterRight = true;
		public int id;
		public int dialoguePointer = 0;
		public bool destroyMePleaseMessageTaken = false;
		public bool destroyMePlease = false;
		public List<string> characters = new List<string>();
		public Citmap characterRight;
		public Citmap characterLeft;
		public FlashObject characterContainerRight;
		public FlashObject characterContainerLeft;
		public GriddyBackground backgroundGriddyBackground;
		public FlashObject background;

		public static DialogueScreen New()
		{
			DialogueScreen screen = NewNode("DialogueScreen").AddComponent<DialogueScreen>();

			screen.background = NewNode("background").AddComponent<FlashObject>();
			screen.background.x = Main.stageWidth / 2f;
			screen.background.y = Main.stageHeight / 2f;
			screen.addChild(screen.background);

			screen.backgroundGriddyBackground = GriddyBackground.New();
			screen.backgroundGriddyBackground.x = -Main.stageWidth / 2f;
			screen.backgroundGriddyBackground.y = -Main.stageHeight / 2f;
			screen.background.addChild(screen.backgroundGriddyBackground);

			screen.characterContainerRight = NewNode("characterContainerRight").AddComponent<FlashObject>();
			screen.characterContainerLeft = NewNode("characterContainerLeft").AddComponent<FlashObject>();
			screen.characterContainerRight.x = Main.stageWidth * 4f / 2f;
			screen.characterContainerLeft.x = -Main.stageWidth * 4f / 2f;
			screen.addChild(screen.characterContainerRight);
			screen.addChild(screen.characterContainerLeft);

			screen.characterRight = Citmap.New(SleepyAssets.GetSprite("img/bereket.png"), 200, 250);
			screen.characterLeft = Citmap.New(SleepyAssets.GetSprite("img/bereket.png"), 200, 250);
			screen.characterRight.x = Main.stageWidth / 2f;
			screen.characterRight.y = Main.stageHeight * 2.5f / 8f;
			screen.characterLeft.x = Main.stageWidth / 2f;
			screen.characterLeft.y = Main.stageHeight * 2.5f / 8f;
			screen.characterRight.scaleX = screen.characterRight.scaleY = 0.5f;
			screen.characterLeft.scaleX = screen.characterLeft.scaleY = 0.5f;
			screen.characterContainerRight.addChild(screen.characterRight);
			screen.characterContainerLeft.addChild(screen.characterLeft);

			screen.textLeft1 = screen.AddLine(screen.characterContainerLeft, "textLeft1", 5.5f);
			screen.textLeft2 = screen.AddLine(screen.characterContainerLeft, "textLeft2", 6.5f);
			screen.textLeft3 = screen.AddLine(screen.characterContainerLeft, "textLeft3", 7.5f);
			screen.textRight1 = screen.AddLine(screen.characterContainerRight, "textRight1", 5.5f);
			screen.textRight2 = screen.AddLine(screen.characterContainerRight, "textRight2", 6.5f);
			screen.textRight3 = screen.AddLine(screen.characterContainerRight, "textRight3", 7.5f);

			screen.id = GameManager.id;
			foreach (SleepyDialogue.Entry entry in SleepyDialogue.forId(screen.id))
			{
				screen.characters.Add(entry.character);
				screen.linesFirst.Add(entry.first);
				screen.linesSecond.Add(entry.second);
				screen.linesThird.Add(entry.third);
			}

			screen.updateCharacter();
			return screen;
		}

		private TextTalha AddLine(FlashObject container, string placeholder, float eighths)
		{
			TextTalha line = TextTalha.New(placeholder);
			line.x = Main.stageWidth / 2f;
			line.y = Main.stageHeight * eighths / 8f;
			container.addChild(line);
			return line;
		}

		/// <summary>
		/// The artwork for one speaker: image, registration point, and the scale that makes
		/// them all read at roughly the same size on screen.
		/// </summary>
		private static Citmap NewCharacter(string name)
		{
			Citmap character;
			switch (name)
			{
				case "bereket": character = Citmap.New(SleepyAssets.GetSprite("img/bereket.png"), 200, 250); character.scaleX = character.scaleY = 0.5f; break;
				case "kayabros": character = Citmap.New(SleepyAssets.GetSprite("img/kayabros.png"), 160, 173); character.scaleX = character.scaleY = 0.8f; break;
				case "headphones": character = Citmap.New(SleepyAssets.GetSprite("img/headphones.png"), 99, 90); character.scaleX = character.scaleY = 1f; break;
				case "tache": character = Citmap.New(SleepyAssets.GetSprite("img/tache.png"), 128, 221); character.scaleX = character.scaleY = 0.55f; break;
				case "politician": character = Citmap.New(SleepyAssets.GetSprite("img/politician.png"), 155, 250); character.scaleX = character.scaleY = 0.5f; break;
				case "mother": character = Citmap.New(SleepyAssets.GetSprite("img/mother.png"), 173, 250); character.scaleX = character.scaleY = 0.5f; break;
				case "lover": character = Citmap.New(SleepyAssets.GetSprite("img/lover.png"), 194, 250); character.scaleX = character.scaleY = 0.5f; break;
				case "freaky": character = Citmap.New(SleepyAssets.GetSprite("img/freaky.png"), 140, 172); character.scaleX = character.scaleY = 0.7f; break;
				case "ceasar": character = Citmap.New(SleepyAssets.GetSprite("img/ceasar.png"), 139, 176); character.scaleX = character.scaleY = 0.7f; break;
				case "buda": character = Citmap.New(SleepyAssets.GetSprite("img/buda.png"), 221, 181); character.scaleX = character.scaleY = 0.65f; break;
				case "blind": character = Citmap.New(SleepyAssets.GetSprite("img/blind.png"), 196, 250); character.scaleX = character.scaleY = 0.5f; break;
				case "bald": character = Citmap.New(SleepyAssets.GetSprite("img/bald.png"), 177, 250); character.scaleX = character.scaleY = 0.5f; break;
				case "sad": character = Citmap.New(SleepyAssets.GetSprite("img/sad.png"), 132, 213); character.scaleX = character.scaleY = 0.6f; break;
				case "sleepy time": character = Citmap.New(SleepyAssets.GetSprite("img/sleepy time.png"), 50, 38); character.scaleX = character.scaleY = 3f; break;
				case "tutorial": character = Tutorial.New(); character.scaleX = character.scaleY = 1f; break;
				default: character = Citmap.New(SleepyAssets.GetSprite("img/bereket.png"), 200, 250); character.scaleX = character.scaleY = 0.5f; break;
			}

			return character;
		}

		public void updateCharacter()
		{
			if (dialoguePointer >= characters.Count)
			{
				destroyMePlease = true;
				return;
			}

			// The new character inherits the outgoing one's position, so swapping artwork
			// mid-conversation doesn't move the speaker.
			float x;
			float y;
			if (isCharacterRight)
			{
				x = characterRight.x;
				y = characterRight.y;
				characterContainerRight.removeChild(characterRight);
			}
			else
			{
				x = characterLeft.x;
				y = characterLeft.y;
				characterContainerLeft.removeChild(characterLeft);
			}

			Citmap replacement = NewCharacter(characters[dialoguePointer]);

			if (isCharacterRight)
			{
				characterRight = replacement;
				characterRight.x = x;
				characterRight.y = y;
				characterContainerRight.addChild(characterRight);
				textRight1.text = linesFirst[dialoguePointer];
				textRight2.text = linesSecond[dialoguePointer];
				textRight3.text = linesThird[dialoguePointer];
				Actuate.tween(characterContainerRight, 1f, x: 0f);
				Actuate.tween(characterContainerLeft, 2f, x: -Main.stageWidth);
			}
			else
			{
				characterLeft = replacement;
				characterLeft.x = x;
				characterLeft.y = y;
				characterContainerLeft.addChild(characterLeft);
				textLeft1.text = linesFirst[dialoguePointer];
				textLeft2.text = linesSecond[dialoguePointer];
				textLeft3.text = linesThird[dialoguePointer];
				Actuate.tween(characterContainerLeft, 1f, x: 0f);
				Actuate.tween(characterContainerRight, 2f, x: Main.stageWidth);
			}

			isCharacterRight = !isCharacterRight;
			++dialoguePointer;
		}

		public void Tick()
		{
			inputHandler();
			backgroundGriddyBackground.Tick();
			textLeft1.Tick();
			textLeft2.Tick();
			textLeft3.Tick();
			textRight1.Tick();
			textRight2.Tick();
			textRight3.Tick();
			characterLeft.Tick();
			characterRight.Tick();
		}

		public void inputHandler()
		{
			if (pressCounter < 1000)
			{
				pressCounter += GameManager.dt;
			}
			else if (GameManager.getKeyDown())
			{
				pressCounter = 0;
				updateCharacter();
			}
		}
	}
}

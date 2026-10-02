namespace Games.WhereLostOnesGo
{
	/// <summary>
	/// The title and end text, the "JUMP" prompt, every particle, and two effects drawn
	/// straight onto the screen. Ported from ScreenTextHandler.as.
	///
	/// Blood: while anything is hurting the player, red strokes cross their pixel and red
	/// particles fly off it. Rain: while the water is rising, white streaks fall at the
	/// mouse, longer the faster it moves down.
	/// </summary>
	public class ScreenTextHandler : Sprite
	{
		public static ScreenTextHandler instance;
		public static TextFormat textFormat;
		public int counter;
		public TextField customTextField;
		public TextField titleTextField;
		public TextField titleTextFieldShadow;
		public double customTextFieldRadius = 2;
		public double titleTextFieldRadius = 4;
		public double mouseY0 = 0;
		public double mouseY1 = 0;
		public double mouseY2 = 0;

		public ScreenTextHandler()
		{
			instance = this;
			addEventListener(Event.ENTER_FRAME, enterFrameHandler);
			counter = 0;
			textFormat = new TextFormat();
			textFormat.align = TextFormatAlign.CENTER;
			textFormat.color = 4278190080;
			textFormat.font = "Verdana";
			textFormat.size = 12;
			var textFormat2 = new TextFormat();
			textFormat2.color = 4278190080;
			textFormat2.font = "Verdana";
			textFormat2.size = 10;
			customTextField = new TextField();
			customTextField.mouseEnabled = false;
			customTextField.selectable = false;
			customTextField.x = 0;
			customTextField.width = 640;
			customTextField.y = 180;
			customTextField.defaultTextFormat = textFormat;
			addChild(customTextField);
			titleTextField = new TextField();
			titleTextField.mouseEnabled = false;
			titleTextField.selectable = false;
			titleTextField.x = 40;
			titleTextField.width = 600;
			titleTextField.height = 240;
			titleTextField.y = 80;
			titleTextField.defaultTextFormat = textFormat2;
			addChild(titleTextField);
			titleTextFieldShadow = new TextField();
			titleTextFieldShadow.mouseEnabled = false;
			titleTextFieldShadow.selectable = false;
			titleTextFieldShadow.x = 40;
			titleTextFieldShadow.width = 600;
			titleTextFieldShadow.height = 240;
			titleTextFieldShadow.y = 80;
			titleTextFieldShadow.defaultTextFormat = textFormat2;
			addChild(titleTextFieldShadow);
		}

		public static void createParticle(double _x, double _y, double _force, uint _color, Point _direction, bool _isThereGravity)
		{
			instance.addChild(new ParticleEffect(_x, _y, _force, _color, _direction, _isThereGravity));
		}

		public static void createRandomDirectionParticle(int howMany, double _x, double _y, double _force, uint _color, Point _direction, bool _isThereGravity)
		{
			for (int i = 0; i < howMany; i++)
			{
				instance.addChild(new ParticleEffect(_x, _y, _force * Flash.random(), _color, new Point(_direction.x - 1 + 2 * Flash.random(), _direction.y - 1 + 2 * Flash.random()), _isThereGravity));
			}
		}

		public void enterFrameHandler()
		{
			++counter;
			if (counter >= 3)
			{
				counter = 0;
				customTextField.alpha = 0.2 + Flash.random() * 0.4;
				customTextField.x = -customTextFieldRadius + 2 * customTextFieldRadius * Flash.random();
				customTextField.y = 180 - customTextFieldRadius + 2 * customTextFieldRadius * Flash.random();
				titleTextFieldShadow.alpha = 0.1 + Flash.random() * 0.2;
				titleTextFieldShadow.x = titleTextField.x - titleTextFieldRadius + 2 * titleTextFieldRadius * Flash.random();
				titleTextFieldShadow.y = titleTextField.y - titleTextFieldRadius + 2 * titleTextFieldRadius * Flash.random();
			}

			graphics.clear();
			if (Game.instance != null && Game.instance.player != null && Game.instance.player.isThereBlood)
			{
				Point playerPos = Game.instance.player.globalPosition();
				double radius = 5 + Flash.random() * 35;
				for (int i = 0; i < 5; i++)
				{
					var randomPoint = new Point(playerPos.x - radius + 2 * radius * Flash.random(), playerPos.y - radius + 2 * radius * Flash.random());
					graphics.lineStyle(0.5 + Flash.random() * 4.5, 4289335569, 0.5 + Flash.random() * 0.5, true);
					graphics.moveTo(randomPoint.x, randomPoint.y);
					graphics.lineTo(2 * playerPos.x - randomPoint.x + 10 * Flash.random(), 2 * playerPos.y - randomPoint.y + 10 * Flash.random());
				}

				createRandomDirectionParticle(5, playerPos.x, playerPos.y, 20, 4289339938, new Point(-1 + 2 * Flash.random(), -1 + 2 * Flash.random()), true);
				Game.instance.player.isThereBlood = false;
			}

			if (stage != null && SeaState4.instance.isActive && !SeaState4.instance.isBlood)
			{
				mouseY2 = mouseY1;
				mouseY1 = mouseY0;
				if (SeaState4.instance.mouseYDifference > 0)
				{
					mouseY0 = SeaState4.instance.mouseYDifference;
				}
				else
				{
					mouseY0 = 0;
				}

				for (int i = 0; i < 5; i++)
				{
					double randomX = stage.mouseX - 10 + 20 * Flash.random();
					graphics.lineStyle(0.5 + Flash.random() * 2.5, 2868903935, 0.2 + Flash.random() * 0.7, true);
					graphics.moveTo(randomX, stage.mouseY - (mouseY0 + mouseY1 + mouseY2));
					graphics.lineTo(randomX - 1 + 2 * Flash.random(), stage.mouseY);
				}
			}
		}

		public void makeTitleTextFieldVisible(string text)
		{
			titleTextField.text = text;
			titleTextFieldShadow.text = text;
		}

		public void makeTitleTextFieldInvisible()
		{
			titleTextField.text = "";
			titleTextFieldShadow.text = "";
		}

		public void makeCustomTextFieldVisible(string text)
		{
			customTextField.text = text;
		}

		public void makeCustomTextFieldInvisible()
		{
			customTextField.text = "";
		}
	}
}

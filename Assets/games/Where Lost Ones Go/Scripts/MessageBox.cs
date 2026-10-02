namespace Games.WhereLostOnesGo
{
	/// <summary>
	/// The lines of text between scenes: white, centred on a dark screen, with a copy
	/// jittering behind it, for three seconds each. Ported from MessageBox.as.
	/// </summary>
	public class MessageBox : Sprite
	{
		public static MessageBox instance;
		public static TextFormat textFormat;
		public string message;
		public TextField textField;
		public TextField textFieldShadow;
		public double textFieldShadowRadius = 5;
		public int counter;
		public FlashTimer timer;

		public MessageBox()
		{
			instance = this;
			addEventListener(Event.ENTER_FRAME, enterFrameHandler);
			alpha = 0;
			counter = 0;
			graphics.beginFill(4278190080, 0.8);
			graphics.drawRect(0, 0, 640, 360);
			graphics.endFill();
			textFormat = new TextFormat();
			textFormat.align = TextFormatAlign.CENTER;
			textFormat.color = 4294967295;
			textFormat.font = "Verdana";
			textFormat.size = 12;
			textField = new TextField();
			textField.mouseEnabled = false;
			textField.selectable = false;
			textField.x = 0;
			textField.width = 640;
			textField.y = 170;
			textField.defaultTextFormat = textFormat;
			addChild(textField);
			textFieldShadow = new TextField();
			textFieldShadow.mouseEnabled = false;
			textFieldShadow.selectable = false;
			textFieldShadow.x = 0;
			textFieldShadow.width = 640;
			textFieldShadow.y = 170;
			textFieldShadow.defaultTextFormat = textFormat;
			textFieldShadow.alpha = 0.3;
			addChild(textFieldShadow);
			timer = new FlashTimer(3000 * Game.debugTimerConst, 1);
			timer.stop();
			timer.addEventListener(TimerEvent.TIMER, timerHandler);
		}

		public void enterFrameHandler()
		{
			++counter;
			if (counter >= 5)
			{
				counter = 0;
				textField.alpha = 0.6 + Flash.random() * 0.4;
				textFieldShadow.x = textField.x - textFieldShadowRadius + 2 * textFieldShadowRadius * Flash.random();
				textFieldShadow.y = textField.y - textFieldShadowRadius + 2 * textFieldShadowRadius * Flash.random();
			}
		}

		public void timerHandler()
		{
			makeInvisible();
		}

		public void makeVisible(int id)
		{
			selectMessage(id);
			textField.text = message;
			textFieldShadow.text = message;
			alpha = 1;
			timer.reset();
			timer.start();
		}

		public void makeInvisible()
		{
			textField.text = "";
			textFieldShadow.text = "";
			alpha = 0;
		}

		public void selectMessage(int id)
		{
			switch (id)
			{
				case 1: message = "Noone knows why I'm doing this"; break;
				case 2: message = "Even I don't know why I'm this way"; break;
				case 3: message = "I'm doing this only because..."; break;
				case 4: message = "...I've lost my innerpeace completely"; break;
				case 5: message = "Every day is another torment"; break;
				case 6: message = "There's no point when you know"; break;
				case 7: message = "When you know it's not going to get better"; break;
				case 8: message = "And no one will understand"; break;
				case 9: message = "And no one will know why"; break;
				case 10: message = "Except the ones"; break;
				case 11: message = "The ones that die every day"; break;
				case 12: message = "Every day die a little inside"; break;
			}
		}
	}
}

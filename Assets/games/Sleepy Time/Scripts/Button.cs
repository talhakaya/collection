using System.Collections.Generic;
using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// Ported from Button.as: a line of text with a rectangular hit box, driven by the mouse
	/// only. Every button registers itself in a static list that GameManager walks each
	/// frame, and screens clear their own entries out of it when they are destroyed - which
	/// is why the list tolerates nulls until cleanButtonsArray runs.
	///
	/// The hit box is a fixed 120x40 around the button's origin regardless of how long the
	/// label is, and it is what getKeyDown() consults to decide whether a click was aimed at
	/// a button or at the game.
	/// </summary>
	public class Button : FlashObject
	{
		public static List<Button> buttons = new List<Button>();

		public TextTalha textField;
		public string text;
		public bool mouseOn = false;

		public static Button New(string text = null)
		{
			Button button = NewNode("Button").AddComponent<Button>();
			button.text = text;
			button.textField = TextTalha.New(button.text);
			button.textField.blinking = true;
			button.addChild(button.textField);

			cleanButtonsArray();
			buttons.Add(button);
			return button;
		}

		public static void cleanButtonsArray()
		{
			for (int i = buttons.Count - 1; i >= 0; i--)
			{
				if (buttons[i] == null)
				{
					buttons.RemoveAt(i);
				}
			}
		}

		public static bool isCollidingWithAny()
		{
			for (int i = 0; i < buttons.Count; i++)
			{
				if (buttons[i] != null && buttons[i].isColliding())
				{
					return true;
				}
			}

			return false;
		}

		/// Dropped when a game is left, so buttons from a previous run can't linger.
		public static void reset()
		{
			buttons.Clear();
		}

		public void Tick()
		{
			textField.Tick();
		}

		public void mouseOverHandler()
		{
			textField.scaleUpCounter = FlashMath.round(Mathf.Max(textField.scaleUpCounter, 250));
		}

		public void mouseClickHandler()
		{
			textField.scaleUpCounter = 1000;
		}

		public bool isColliding()
		{
			return Main.STAGE.mouseX > (x - 60) * GameManager.ScaleX
				&& Main.STAGE.mouseX < (x + 60) * GameManager.ScaleX
				&& Main.STAGE.mouseY > (y - 20) * GameManager.ScaleY
				&& Main.STAGE.mouseY < (y + 20) * GameManager.ScaleY;
		}

		public void destroy()
		{
			for (int i = 0; i < buttons.Count; i++)
			{
				if (buttons[i] == this)
				{
					buttons[i] = null;
					cleanButtonsArray();
					break;
				}
			}
		}
	}
}

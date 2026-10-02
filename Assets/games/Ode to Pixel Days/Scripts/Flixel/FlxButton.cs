using System;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// org.flixel.FlxButton: a three-frame sprite (normal, highlighted, pressed) with a
	/// centred label, that calls onUp when the mouse is released on it.
	///
	/// Flixel heard the release as a Flash mouse event; here it is read from FlxG.mouse in
	/// update, which comes to the same thing.
	///
	/// One addition for the collection: a button can be given the focus and pressed without
	/// a mouse, so the menu can be worked from a gamepad or the keyboard. Flixel's buttons
	/// were mouse-only.
	/// </summary>
	public class FlxButton : FlxSprite
	{
		public const uint NORMAL = 0;
		public const uint HIGHLIGHT = 1;
		public const uint PRESSED = 2;

		private const string ImgDefaultButton = "FlxButton_ImgDefaultButton";

		public FlxText label;
		public FlxPoint labelOffset;
		public Action onUp;
		public Action onDown;
		public Action onOver;
		public Action onOut;
		public uint status;

		/// Not in Flixel: drawn highlighted although the mouse is elsewhere.
		public bool focused;

		protected bool _onToggle;

		public FlxButton(double X = 0, double Y = 0, string Label = null, Action OnClick = null) : base(X, Y)
		{
			if (Label != null)
			{
				label = new FlxText(0, 0, 80, Label);
				label.setFormat(null, 8, 0x333333, "center");
				labelOffset = new FlxPoint(-1, 3);
			}

			loadGraphic(ImgDefaultButton, true, false, 80, 20);

			onUp = OnClick;
			onDown = null;
			onOut = null;
			onOver = null;

			status = NORMAL;
			_onToggle = false;
			focused = false;
		}

		public override void destroy()
		{
			if (label != null)
			{
				label.destroy();
				label = null;
			}

			onUp = null;
			onDown = null;
			onOut = null;
			onOver = null;
			base.destroy();
		}

		public override void update()
		{
			updateButton();

			// Flixel's onMouseUp.
			if (FlxG.mouse.justReleased() && exists && visible && active && status == PRESSED)
			{
				press();
			}

			if (label == null)
			{
				return;
			}

			switch ((uint)frame)
			{
				case HIGHLIGHT:
					label.alpha = 1.0;
					break;
				case PRESSED:
					label.alpha = 0.5;
					label.y++;
					break;
				default:
					label.alpha = 0.8;
					break;
			}
		}

		/// What a click does. Public so that a focused button can be pressed from a key.
		public void press()
		{
			if (onUp != null)
			{
				onUp();
			}
		}

		protected void updateButton()
		{
			if (FlxG.mouse.visible)
			{
				_point.x = FlxG.mouse.x;
				_point.y = FlxG.mouse.y;
				if (overlapsPoint(_point))
				{
					if (FlxG.mouse.justPressed())
					{
						status = PRESSED;
						if (onDown != null)
						{
							onDown();
						}
					}

					if (status == NORMAL)
					{
						status = HIGHLIGHT;
						if (onOver != null)
						{
							onOver();
						}
					}
				}
				else
				{
					if (status != NORMAL)
					{
						if (onOut != null)
						{
							onOut();
						}
					}

					status = NORMAL;
				}
			}
			else
			{
				status = NORMAL;
			}

			// Then if the label and/or the label offset exist, position them to match the button.
			if (label != null)
			{
				label.x = x;
				label.y = y;
			}

			if (labelOffset != null)
			{
				label.x += labelOffset.x;
				label.y += labelOffset.y;
			}

			// Then pick the appropriate frame of animation
			if (status == HIGHLIGHT && _onToggle)
			{
				frame = (int)NORMAL;
			}
			else if (status == NORMAL && focused)
			{
				frame = (int)HIGHLIGHT;
			}
			else
			{
				frame = (int)status;
			}
		}

		public override void draw()
		{
			base.draw();
			if (label != null)
			{
				label.scrollFactor = scrollFactor;
				label.draw();
			}
		}

		public bool on
		{
			get { return _onToggle; }
			set { _onToggle = value; }
		}
	}
}

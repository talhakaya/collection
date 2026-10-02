namespace Games.OdeToPixelDays
{
	/// <summary>
	/// A coloured gate that a lever raises. Ported from Block.as.
	///
	/// It shoots up 96 pixels when its lever opens. The slow kind then sinks back by itself
	/// and closes the lever again when it lands; the other kind stays up until the lever is
	/// closed.
	/// </summary>
	public class Block : FlxSprite
	{
		private static string S_block = "Block_S_block";
		private static string Sfx = "Block_Sfx";
		private static string S_blockkucuk = "Block_S_blockkucuk";

		protected bool isOpen;
		public double initialX;
		private double initialY;
		private bool slowlyLower;
		private bool slowlyLowerAtTheMoment;
		private Lever lever;
		private bool onUse;
		private bool leverIsOpen;
		private bool sfxPlayedGoingDown;
		private bool sfxPlayedGoingUp;

		public Block(double _X, double _Y, FlxPoint _scale, int _color, bool _slow, Lever _lever, bool _small)
		{
			x = _X;
			y = _Y;
			initialX = _X;
			initialY = _Y;
			lever = _lever;
			scale = _scale;
			if (_small)
			{
				loadGraphic(S_blockkucuk, true, false, 4, 64, false);
				width = 4 * scale.x;
				height = 64 * scale.x;
			}
			else
			{
				loadGraphic(S_block, true, false, 16, 128, false);
				width = 16 * scale.x;
				height = 128 * scale.x;
			}

			centerOffsets();
			addAnimation("red", new[] { 0 }, 1, true);
			addAnimation("green", new[] { 1 }, 1, true);
			addAnimation("blue", new[] { 2 }, 1, true);
			if (_color == 1)
			{
				play("red");
			}
			else if (_color == 2)
			{
				play("green");
			}
			else
			{
				play("blue");
			}

			isOpen = false;
			onUse = false;
			leverIsOpen = false;
			slowlyLowerAtTheMoment = false;
			slowlyLower = _slow;
			immovable = true;
			sfxPlayedGoingDown = false;
			sfxPlayedGoingUp = false;
		}

		public override void update()
		{
			base.update();
			if (!sfxPlayedGoingUp && !isOpen && lever.isOpen)
			{
				FlxG.play(Sfx);
				sfxPlayedGoingDown = false;
				sfxPlayedGoingUp = true;
			}
			else if (!sfxPlayedGoingDown && velocity.y == 160)
			{
				FlxG.play(Sfx);
				sfxPlayedGoingDown = true;
				sfxPlayedGoingUp = false;
			}
			else if (!lever.isOpen)
			{
				sfxPlayedGoingUp = false;
			}

			isOpen = lever.isOpen;
			if (slowlyLower && isOpen && !onUse)
			{
				if (y < initialY)
				{
					velocity.y = 8;
					slowlyLowerAtTheMoment = true;
				}
				else if (velocity.y == 8)
				{
					slowlyLowerAtTheMoment = false;
					velocity.y = 0;
					y = initialY;
					isOpen = false;
					lever.kill();
				}
			}

			if (!onUse)
			{
				if (isOpen && y >= initialY)
				{
					velocity.y = -160;
					onUse = true;
				}
				else if (!isOpen && (y < initialY || slowlyLowerAtTheMoment))
				{
					slowlyLowerAtTheMoment = false;
					velocity.y = 160;
					onUse = true;
				}
				else if (!slowlyLower)
				{
					velocity.y = 0;
				}
			}
			else if (y >= initialY && !isOpen)
			{
				velocity.y = 0;
				y = initialY;
				onUse = false;
				isOpen = true;
			}
			else if (y <= initialY - 96 && isOpen)
			{
				velocity.y = 0;
				y = initialY - 96;
				onUse = false;
				isOpen = false;
			}

			if (onUse && velocity.y == 0)
			{
				onUse = false;
			}
		}

		private void restartPosition()
		{
			velocity.y = 0;
			acceleration.y = 0;
			y = initialY;
			onUse = false;
			isOpen = false;
			leverIsOpen = false;
			slowlyLowerAtTheMoment = false;
		}
	}
}

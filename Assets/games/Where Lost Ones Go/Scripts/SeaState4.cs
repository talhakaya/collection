namespace Games.WhereLostOnesGo
{
	/// <summary>
	/// The sea that fills the bottom of the screen in two of the scenes. Ported from
	/// SeaState4.as.
	///
	/// As blood it lies still and low. As water it rises towards the player, drawing blood
	/// when it reaches them, and every downward movement of the mouse pushes it back down
	/// by a sixth of the distance moved.
	/// </summary>
	public class SeaState4 : Sprite
	{
		public static SeaState4 instance;
		public bool isActive;
		public bool isBlood;
		public const double maxRotation = 2;
		public double speed = 1.5;
		public double mouseYOld = 360;
		public double difficulty = 6;
		public Point particleDirection = new Point(0, -2);
		public double mouseYDifference = 0;

		public SeaState4()
		{
			addEventListener(Event.ENTER_FRAME, enterFrameHandler);
			instance = this;
			x = 320;
			y = 360;
		}

		public void enterFrameHandler()
		{
			graphics.clear();
			if (isActive)
			{
				if (Game.STATE == 0)
				{
					isActive = false;
					isBlood = false;
					y = 360;
				}

				if (!isBlood)
				{
					if (y + 32 > Game.instance.player.globalPosition().y)
					{
						y -= speed;
					}

					x = 320 - 250 + 500 * Flash.random();
					graphics.beginFill(4280427178, 0.5 + Flash.random() * 0.5);
					graphics.drawRect(-640, 0, 1280, 720 - y);
					graphics.endFill();
					rotation = -maxRotation + 2 * maxRotation * Flash.random();
					if (y <= Game.instance.player.globalPosition().y)
					{
						Game.instance.player.isThereBlood = true;
					}

					if (stage != null && stage.mouseY > mouseYOld)
					{
						mouseYDifference = stage.mouseY - mouseYOld;
						y += mouseYDifference / difficulty;
					}
					else
					{
						mouseYDifference = 0;
					}

					ScreenTextHandler.createRandomDirectionParticle(1, 20 + 600 * Flash.random(), y - 10, 10, 4280427178, particleDirection, true);
					mouseYOld = stage.mouseY;
				}
				else
				{
					y = 280;
					x = 320 - 250 + 500 * Flash.random();
					graphics.beginFill(4289335569, 0.5 + Flash.random() * 0.5);
					graphics.drawRect(-640, 0, 1280, 720 - y);
					graphics.endFill();
					rotation = -maxRotation + 2 * maxRotation * Flash.random();
					ScreenTextHandler.createRandomDirectionParticle(1, 20 + 600 * Flash.random(), y - 10, 10, 4289335569, particleDirection, true);
				}
			}
		}
	}
}

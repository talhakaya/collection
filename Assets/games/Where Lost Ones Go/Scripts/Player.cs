namespace Games.WhereLostOnesGo
{
	/// <summary>
	/// The player: one white pixel on the bridge, breathing in and out, with a black box
	/// around it that is shown while a scene is open - at twenty times zoom it fills the
	/// screen. Ported from Player.as.
	///
	/// When the fall ends it turns into a black pixel that fades out, drifting.
	/// </summary>
	public class Player : Sprite
	{
		public bool alphaGoingDown;
		public Sprite backgroundBlack;
		public Sprite player;
		public double radius = 10;
		public bool isThereBlood;
		public bool died;

		public Player()
		{
			addEventListener(Event.ENTER_FRAME, enterFrameHandler);
			x = (Game.CarXMin + Game.CarXMax) / 2 - 33;
			y = Game.CarY - 1;
			alphaGoingDown = true;
			backgroundBlack = new Sprite();
			backgroundBlack.graphics.beginFill(4278190080, 1);
			backgroundBlack.graphics.drawRect(-32, -18, 64, 36);
			backgroundBlack.graphics.endFill();
			backgroundBlack.alpha = 0;
			player = new Sprite();
			player.graphics.beginFill(2868903935, 1);
			player.graphics.drawRect(0, 0, 1, 1);
			player.graphics.endFill();
			addChild(backgroundBlack);
			addChild(player);
		}

		public void enterFrameHandler()
		{
			if (!died)
			{
				if (alphaGoingDown)
				{
					if (player.alpha <= 0.6)
					{
						alphaGoingDown = false;
					}
					else
					{
						player.alpha -= 0.05;
					}
				}
				else if (player.alpha >= 1)
				{
					alphaGoingDown = true;
				}
				else
				{
					player.alpha += 0.05;
				}
			}
			else if (player.alpha > 0)
			{
				player.alpha -= 0.001;
				player.x += 0.1;
			}
			else
			{
				removeEventListener(Event.ENTER_FRAME, enterFrameHandler);
			}
		}

		public void openScene()
		{
			backgroundBlack.alpha = 1;
		}

		public void closeScene()
		{
			backgroundBlack.alpha = 0;
		}

		/// Where the player's pixel is on the stage - the middle of it, at full zoom.
		public Point globalPosition()
		{
			var playerPos = new Point(player.x, player.y);
			return new Point(localToGlobal(playerPos).x + Game.instance.zoomScale / 2, localToGlobal(playerPos).y + Game.instance.zoomScale / 2);
		}

		public void die()
		{
			died = true;
			player.alpha = 1;
			player.graphics.clear();
			player.graphics.beginFill(2852126720, 1);
			player.graphics.drawRect(0, 0, 1, 1);
			player.graphics.endFill();
		}
	}
}

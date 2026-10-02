namespace Games.WhereLostOnesGo
{
	/// A red drop left where the mouse is, every third of a second in the scorpion scene,
	/// shrinking away. Scorpions that touch one are driven back. Ported from DropState2.as.
	public class DropState2 : Sprite
	{
		public double radius = 10;

		public DropState2()
		{
			Game.instance.collisionCheckList.Add(this);
			addEventListener(Event.ENTER_FRAME, enterFrameHandler);
			x = Game.instance.stage.mouseX;
			y = Game.instance.stage.mouseY;
		}

		public void enterFrameHandler()
		{
			graphics.clear();
			graphics.beginFill(2863272209, 0.5 + 0.5 * Flash.random());
			graphics.drawCircle(0, 0, radius);
			graphics.endFill();
			radius -= 0.2;
			if ((radius <= 0 || Game.STATE == 0) && parent != null)
			{
				removeEventListener(Event.ENTER_FRAME, enterFrameHandler);
				Game.instance.collisionCheckList.Remove(this);
				parent.removeChild(this);
			}
		}
	}
}

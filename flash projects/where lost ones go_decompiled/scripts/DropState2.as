package
{
   import flash.display.Sprite;
   import flash.events.Event;
   
   public class DropState2 extends Sprite
   {
      
      public var radius:Number = 10;
      
      public function DropState2()
      {
         super();
         Game.instance.collisionCheckList.addItem(this);
         addEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
         x = Game.instance.stage.mouseX;
         y = Game.instance.stage.mouseY;
      }
      
      public function enterFrameHandler(e:Event) : void
      {
         graphics.clear();
         graphics.beginFill(2863272209,0.5 + 0.5 * Math.random());
         graphics.drawCircle(0,0,this.radius);
         graphics.endFill();
         this.radius -= 0.2;
         if((this.radius <= 0 || Game.STATE == 0) && parent != null)
         {
            removeEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
            Game.instance.collisionCheckList.removeItem(this);
            parent.removeChild(this);
         }
      }
   }
}


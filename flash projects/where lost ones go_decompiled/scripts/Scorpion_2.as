package
{
   import flash.events.Event;
   
   public class Scorpion extends scorpion
   {
      
      public var radius:Number = 20;
      
      public var dieAlready:Boolean;
      
      public var touchingPlayer:Boolean;
      
      public var speed:Number = 1.5;
      
      public function Scorpion(_x:Number, _y:Number, _speed:Number)
      {
         super();
         x = _x;
         y = _y;
         this.speed = _speed;
         Game.instance.collisionCheckList.addItem(this);
         addEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
         scaleX = 2;
         scaleY = 2;
      }
      
      public function enterFrameHandler(e:Event) : void
      {
         x += -1 + 2 * Math.random();
         y += -1 + 2 * Math.random();
         alpha = 0.5 + 0.5 * Math.random();
         if(Game.instance.player != null)
         {
            rotation = Math.atan2(Game.instance.player.globalPosition().y - y,Game.instance.player.globalPosition().x - x) * 180 / Math.PI;
            if(!this.dieAlready)
            {
               if(!this.touchingPlayer)
               {
                  x += this.speed * Math.cos(rotation * Math.PI / 180);
                  y += this.speed * Math.sin(rotation * Math.PI / 180);
               }
            }
            else
            {
               x -= 2 * this.speed * Math.cos(rotation * Math.PI / 180);
               y -= 2 * this.speed * Math.sin(rotation * Math.PI / 180);
            }
         }
         if(Game.STATE == 0 && parent != null)
         {
            removeEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
            Game.instance.collisionCheckList.removeItem(this);
            parent.removeChild(this);
         }
         this.dieAlready = false;
         this.touchingPlayer = false;
      }
   }
}


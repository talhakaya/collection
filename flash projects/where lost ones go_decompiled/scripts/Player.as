package
{
   import flash.display.Sprite;
   import flash.events.Event;
   import flash.geom.Point;
   
   public class Player extends Sprite
   {
      
      public var alphaGoingDown:Boolean;
      
      public var backgroundBlack:Sprite;
      
      public var player:Sprite;
      
      public var radius:Number = 10;
      
      public var isThereBlood:Boolean;
      
      public var died:Boolean;
      
      public function Player()
      {
         super();
         addEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
         x = (Game.CarXMin + Game.CarXMax) / 2 - 33;
         y = Game.CarY - 1;
         this.alphaGoingDown = true;
         this.backgroundBlack = new Sprite();
         this.backgroundBlack.graphics.beginFill(4278190080,1);
         this.backgroundBlack.graphics.drawRect(-32,-18,64,36);
         this.backgroundBlack.graphics.endFill();
         this.backgroundBlack.alpha = 0;
         this.player = new Sprite();
         this.player.graphics.beginFill(2868903935,1);
         this.player.graphics.drawRect(0,0,1,1);
         this.player.graphics.endFill();
         addChild(this.backgroundBlack);
         addChild(this.player);
      }
      
      public function enterFrameHandler(e:Event) : void
      {
         if(!this.died)
         {
            if(this.alphaGoingDown)
            {
               if(this.player.alpha <= 0.6)
               {
                  this.alphaGoingDown = false;
               }
               else
               {
                  this.player.alpha -= 0.05;
               }
            }
            else if(this.player.alpha >= 1)
            {
               this.alphaGoingDown = true;
            }
            else
            {
               this.player.alpha += 0.05;
            }
         }
         else if(this.player.alpha > 0)
         {
            this.player.alpha -= 0.001;
            this.player.x += 0.1;
         }
         else
         {
            removeEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
         }
      }
      
      public function openScene() : void
      {
         this.backgroundBlack.alpha = 1;
      }
      
      public function closeScene() : void
      {
         this.backgroundBlack.alpha = 0;
      }
      
      public function globalPosition() : Point
      {
         var playerPos:Point = new Point(this.player.x,this.player.y);
         return new Point(localToGlobal(playerPos).x + Game.instance.zoomScale / 2,localToGlobal(playerPos).y + Game.instance.zoomScale / 2);
      }
      
      public function die() : void
      {
         this.died = true;
         this.player.alpha = 1;
         this.player.graphics.clear();
         this.player.graphics.beginFill(2852126720,1);
         this.player.graphics.drawRect(0,0,1,1);
         this.player.graphics.endFill();
      }
   }
}


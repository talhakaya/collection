package
{
   import flash.display.Sprite;
   import flash.events.Event;
   import flash.geom.Point;
   
   public class SeaState4 extends Sprite
   {
      
      public static var instance:SeaState4;
      
      public var isActive:Boolean;
      
      public var isBlood:Boolean;
      
      public const maxRotation:Number = 2;
      
      public var speed:Number = 1.5;
      
      public var mouseYOld:Number = 360;
      
      public var difficulty:Number = 6;
      
      public var particleDirection:Point = new Point(0,-2);
      
      public var mouseYDifference:Number = 0;
      
      public function SeaState4()
      {
         super();
         addEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
         instance = this;
         x = 320;
         y = 360;
      }
      
      public function enterFrameHandler(e:Event) : void
      {
         graphics.clear();
         if(this.isActive)
         {
            if(Game.STATE == 0)
            {
               this.isActive = false;
               this.isBlood = false;
               y = 360;
            }
            if(!this.isBlood)
            {
               if(y + 32 > Game.instance.player.globalPosition().y)
               {
                  y -= this.speed;
               }
               x = 320 - 250 + 500 * Math.random();
               graphics.beginFill(4280427178,0.5 + Math.random() * 0.5);
               graphics.drawRect(-640,0,1280,720 - y);
               graphics.endFill();
               rotation = -this.maxRotation + 2 * this.maxRotation * Math.random();
               if(y <= Game.instance.player.globalPosition().y)
               {
                  Game.instance.player.isThereBlood = true;
               }
               if(stage != null && stage.mouseY > this.mouseYOld)
               {
                  this.mouseYDifference = stage.mouseY - this.mouseYOld;
                  y += this.mouseYDifference / this.difficulty;
               }
               else
               {
                  this.mouseYDifference = 0;
               }
               ScreenTextHandler.createRandomDirectionParticle(1,20 + 600 * Math.random(),y - 10,10,4280427178,this.particleDirection,true);
               this.mouseYOld = stage.mouseY;
            }
            else
            {
               y = 280;
               x = 320 - 250 + 500 * Math.random();
               graphics.beginFill(4289335569,0.5 + Math.random() * 0.5);
               graphics.drawRect(-640,0,1280,720 - y);
               graphics.endFill();
               rotation = -this.maxRotation + 2 * this.maxRotation * Math.random();
               ScreenTextHandler.createRandomDirectionParticle(1,20 + 600 * Math.random(),y - 10,10,4289335569,this.particleDirection,true);
            }
         }
      }
   }
}


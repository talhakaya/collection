package
{
   import flash.events.Event;
   import flash.geom.Point;
   
   public class Human extends human
   {
      
      public var radius:Number = 20;
      
      public var dieAlready:Boolean;
      
      public var speed:Number = 1.5;
      
      public var speedInBlood:Number = 1.5;
      
      public var inBlood:Boolean;
      
      public var shakingAngle:Number = 5;
      
      private var lyingToRight:Boolean;
      
      public function Human(_x:Number, _y:Number, _inBlood:Boolean)
      {
         super();
         x = _x;
         y = _y;
         addEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
         this.inBlood = _inBlood;
         if(this.inBlood)
         {
            scaleX = 2;
            scaleY = 2;
         }
         else
         {
            scaleX = 3;
            scaleY = 3;
         }
         if(Math.random() > 0.5)
         {
            this.lyingToRight = true;
         }
      }
      
      public function enterFrameHandler(e:Event) : void
      {
         var angleToPlayer:Number = NaN;
         var globalPos:Point = null;
         if(Game.STATE == 0 || x < -2 * this.radius || x > 640 + 2 * this.radius || y < -2 * this.radius || y > 360 + 2 * this.radius)
         {
            if(parent != null)
            {
               removeEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
               parent.removeChild(this);
            }
         }
         if(Game.instance.player != null)
         {
            if(!this.inBlood)
            {
               angleToPlayer = Math.atan2(Game.instance.player.globalPosition().y - y,Game.instance.player.globalPosition().x - x);
               if(!this.dieAlready)
               {
                  if(Main.distanceBetweenMouseCloserThanRadius(this.radius,this))
                  {
                     this.dieAlready = true;
                  }
               }
               else
               {
                  x -= this.speed * Math.cos(angleToPlayer);
                  y -= this.speed * Math.sin(angleToPlayer);
                  x += -1 + 2 * Math.random();
                  y += -1 + 2 * Math.random();
                  alpha = 0.5 + 0.5 * Math.random();
                  globalPos = localToGlobal(new Point(0,-8));
                  ScreenTextHandler.createRandomDirectionParticle(1,globalPos.x,globalPos.y,10,4286611584,new Point(10 * Math.cos(angleToPlayer),10 * Math.sin(angleToPlayer)),true);
               }
            }
            else if(!this.dieAlready)
            {
               x += -2 + 4 * Math.random();
               y += -2 + 4 * Math.random();
               alpha = 0.5 + 0.5 * Math.random();
               if(Main.distanceBetweenMouseCloserThanRadius(this.radius,this))
               {
                  this.dieAlready = true;
               }
               if(this.lyingToRight)
               {
                  rotation = 90 - this.shakingAngle + 2 * this.shakingAngle * Math.random();
               }
               else
               {
                  rotation = 270 - this.shakingAngle + 2 * this.shakingAngle * Math.random();
               }
            }
            else
            {
               y += 1;
               alpha -= 0.01;
            }
         }
      }
   }
}


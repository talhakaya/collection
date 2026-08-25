package
{
   import com.greensock.TweenLite;
   import flash.display.Sprite;
   import flash.events.Event;
   
   public class Car extends Sprite
   {
      
      public var whichCarX:int;
      
      public var whichCarY:int;
      
      public function Car()
      {
         super();
         addEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
         x = Game.CarXMin + Math.random() * (Game.CarXMax - Game.CarXMin);
         y = Game.CarY;
         var rand:Number = Math.random();
         if(rand < 0.25)
         {
            this.whichCarX = 1;
            this.whichCarY = 1;
         }
         else if(rand < 0.5)
         {
            this.whichCarX = 2;
            this.whichCarY = 1;
         }
         else if(rand < 0.75)
         {
            this.whichCarX = 4;
            this.whichCarY = 2;
         }
         else
         {
            this.whichCarX = 5;
            this.whichCarY = 2;
         }
         y -= this.whichCarY;
         rand = Math.random();
         var destinationX:Number = Game.CarXMin;
         if(rand < 0.5)
         {
            destinationX = Game.CarXMax;
         }
         TweenLite.to(this,(25 + 25 * Math.random()) * Game.debugTimerConst,{
            "x":destinationX,
            "y":Game.CarY - this.whichCarY,
            "onComplete":this.tweenDoneHandler
         });
      }
      
      public function enterFrameHandler(e:Event) : void
      {
         graphics.clear();
         graphics.beginFill(4278190080,0.25);
         graphics.drawRect(0,0,this.whichCarX,this.whichCarY);
         graphics.endFill();
      }
      
      public function tweenDoneHandler() : void
      {
         removeEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
         parent.addChild(new Car());
         parent.removeChild(this);
      }
   }
}


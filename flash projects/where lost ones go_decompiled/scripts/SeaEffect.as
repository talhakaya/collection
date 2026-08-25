package
{
   import flash.display.Sprite;
   import flash.events.Event;
   
   public class SeaEffect extends Sprite
   {
      
      public var _alpha:Number = 0;
      
      public var whichX:int;
      
      public var alphaSpeed:Number = 0.01 + 0.01 * Math.random();
      
      public var rightSpot:Boolean;
      
      public function SeaEffect()
      {
         super();
         addEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
         this.doneHandler();
      }
      
      public function enterFrameHandler(e:Event) : void
      {
         this._alpha += this.alphaSpeed;
         var graphicsAlpha:Number = 0;
         if(this._alpha < 0.5)
         {
            graphicsAlpha = this._alpha;
         }
         else
         {
            graphicsAlpha = 1 - this._alpha;
         }
         graphics.clear();
         graphics.beginFill(2868903935,graphicsAlpha);
         graphics.drawRect(0,0,this.whichX,1);
         graphics.endFill();
         if(this._alpha >= 1)
         {
            this._alpha = 0;
            this.doneHandler();
         }
      }
      
      public function doneHandler() : void
      {
         var rand:Number = NaN;
         do
         {
            x = 640 * Math.random();
            y = 230 + Math.random() * 90;
            rand = Math.random();
            if(rand < 0.25)
            {
               this.whichX = 1;
            }
            else if(rand < 0.5)
            {
               this.whichX = 2;
            }
            else if(rand < 0.75)
            {
               this.whichX = 3;
            }
            else
            {
               this.whichX = 4;
            }
            this.calculateRightSpot();
         }
         while(!this.rightSpot);
      }
      
      public function calculateRightSpot() : void
      {
         this.rightSpot = !(x > 330 && y > 270 && (x - 330) / 310 + (y - 270) / 90 > 1);
      }
   }
}


package
{
   import flash.Boot;
   import flash.display.Bitmap;
   import flash.display.BitmapData;
   import flash.display.Sprite;
   
   public class Citmap extends Sprite
   {
      
      public var rotationTimeCurrent:int;
      
      public var rotationMax:Number;
      
      public var rotatingToMax:Boolean;
      
      public var rotating:Boolean;
      
      public var centerY:Number;
      
      public var centerX:Number;
      
      public var blinking:Boolean;
      
      public var bitmap:Bitmap;
      
      public function Citmap(param1:BitmapData = undefined, param2:Number = 0, param3:Number = 0)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         rotatingToMax = false;
         rotationTimeCurrent = 0;
         rotationMax = 15;
         blinking = false;
         rotating = true;
         super();
         bitmap = new Bitmap(param1);
         addChild(bitmap);
         centerX = -param2;
         centerY = -param3;
         bitmap.x = centerX;
         bitmap.y = centerY;
      }
      
      public function update() : void
      {
         if(blinking)
         {
            blink();
         }
         if(rotating)
         {
            rotate();
         }
      }
      
      public function rotate() : void
      {
         if(rotatingToMax)
         {
            rotationTimeCurrent += GameManager.dt;
            if(rotationTimeCurrent >= GameManager.rhythm)
            {
               rotatingToMax = false;
            }
         }
         else
         {
            rotationTimeCurrent -= GameManager.dt;
            if(rotationTimeCurrent <= -GameManager.rhythm)
            {
               rotatingToMax = true;
            }
         }
         rotation = rotationMax * (rotationTimeCurrent / GameManager.rhythm);
      }
      
      public function blink() : void
      {
         bitmap.alpha = 0.4 + Math.random() * 0.6;
      }
   }
}


package
{
   import flash.Boot;
   import flash.display.BitmapData;
   import openfl.Assets;
   
   public class ScoreStar extends Citmap
   {
      
      public var timeCounter:int;
      
      public var maxScale:Number;
      
      public var delay:int;
      
      public function ScoreStar(param1:Boolean = false)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         timeCounter = 0;
         delay = 0;
         maxScale = 0.9;
         if(param1)
         {
            super(Assets.getBitmapData("img/star_full.png"),62,57);
            delay = int(Math.round(Math.random() * 500));
         }
         else
         {
            super(Assets.getBitmapData("img/star_empty.png"),62,57);
            delay = 500 + int(Math.round(Math.random() * 500));
            blinking = true;
         }
         scaleX = scaleY = 0;
      }
      
      override public function update() : void
      {
         var _loc1_:Number = NaN;
         super.update();
         timeCounter += GameManager.dt;
         if(timeCounter >= delay)
         {
            if(timeCounter < delay + 1000)
            {
               scaleX = scaleY = Math.pow((timeCounter - delay) / 1000,1.5) * maxScale;
            }
            else
            {
               scaleX = scaleY = maxScale;
            }
         }
      }
   }
}


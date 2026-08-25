package
{
   import flash.Boot;
   import flash.display.BitmapData;
   import openfl.Assets;
   
   public class Particle extends Citmap
   {
      
      public var timeCounter:int;
      
      public var rotatePerDt:Number;
      
      public var needsToBeKilled:Boolean;
      
      public var lifeTime:int;
      
      public var alive:Boolean;
      
      public var accelerationY:Number;
      
      public var accelerationX:Number;
      
      public function Particle()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         alive = false;
         super(Assets.getBitmapData("img/particle.png"),0.5,0.5);
         scaleX = scaleY = 8;
      }
      
      override public function update() : void
      {
         if(alive)
         {
            timeCounter += GameManager.dt;
            if(timeCounter >= lifeTime)
            {
               needsToBeKilled = true;
            }
            rotation += rotatePerDt * GameManager.dt;
            x += accelerationX * GameManager.dt / 50;
            y += accelerationY * GameManager.dt / 50;
            accelerationY += GameManager.dt / 15;
            alpha = 0.5 + 0.5 * Math.random();
         }
      }
      
      public function reset(param1:Number, param2:Number) : void
      {
         x = param1;
         y = param2;
         lifeTime = 200 + int(Math.round(300 * Math.random()));
         rotatePerDt = -1 + 2 * Math.random();
         accelerationX = -5 + 10 * Math.random();
         accelerationY = -5 - 10 * Math.random();
         timeCounter = 0;
         alive = true;
         needsToBeKilled = false;
      }
      
      public function kill() : void
      {
         alive = false;
         needsToBeKilled = false;
      }
   }
}


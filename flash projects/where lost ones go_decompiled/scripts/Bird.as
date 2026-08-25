package
{
   import com.greensock.TweenLite;
   import flash.display.Sprite;
   
   public class Bird extends Sprite
   {
      
      public static var birdX:Number;
      
      public static var birdY:Number;
      
      public static var destinationX:Number;
      
      public static var destinationY:Number;
      
      public function Bird(_x:Number, _y:Number, _destinationX:Number, _destinationY:Number)
      {
         super();
         graphics.beginFill(4278190080,0.5);
         graphics.drawRect(0,0,1,1);
         graphics.endFill();
         x = _x - Game.BirdX + Game.BirdX * 2 * Math.random();
         y = _y - Game.BirdY + Game.BirdY * 2 * Math.random();
         var destinationX:Number = _destinationX - Game.BirdX + Game.BirdX * 2 * Math.random();
         var destinationY:Number = _destinationY - Game.BirdY + Game.BirdY * 2 * Math.random();
         TweenLite.to(this,(45 + 5 * Math.random()) * Game.debugTimerConst,{
            "x":destinationX,
            "y":destinationY,
            "onComplete":this.tweenDoneHandler
         });
      }
      
      public function tweenDoneHandler() : void
      {
         parent.addChild(new Bird(birdX,birdY,destinationX,destinationY));
         parent.removeChild(this);
      }
   }
}


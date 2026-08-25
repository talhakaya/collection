package
{
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   
   public class Cloud2 extends FlxSprite
   {
      
      private static var S_:Class = Cloud2_S_;
      
      private var maxX:Number;
      
      public function Cloud2(_y:Number, _levelwidth:int)
      {
         var random:Number = NaN;
         super();
         loadGraphic(S_,false,true,64,32,false);
         if(Math.random() > 0.5)
         {
            facing = LEFT;
         }
         else
         {
            facing = RIGHT;
         }
         velocity.x = Math.random() * 20;
         x = -Math.random() * 100 - 100;
         y = _y;
         random = Math.random();
         scale = new FlxPoint(1 + random,1 + random);
         alpha = 0.7;
         this.maxX = _levelwidth * 8 + 180;
      }
      
      override public function update() : void
      {
         super.update();
         if(x > this.maxX || x < -180)
         {
            kill();
         }
      }
   }
}


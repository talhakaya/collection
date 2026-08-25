package
{
   import org.flixel.FlxSprite;
   
   public class ParticleBlack extends FlxSprite
   {
      
      private var count:int;
      
      public function ParticleBlack(_X:Number, _Y:Number)
      {
         super();
         x = _X + 16 * (Math.random() - 0.5);
         y = _Y;
         acceleration.y = 100;
         makeGraphic(4,4,4278190080);
         velocity.x = 50 * (Math.random() - 0.5);
         velocity.y = -50;
         this.count = 0;
      }
      
      override public function update() : void
      {
         super.update();
         ++this.count;
         if(this.count >= 60)
         {
            kill();
         }
      }
   }
}


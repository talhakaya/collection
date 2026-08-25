package
{
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   
   public class Cloud extends FlxSprite
   {
      
      private static var S_:Class = Cloud_S_;
      
      private var player:Hans;
      
      private var grey:Boolean;
      
      public function Cloud(_player:Hans, _grey:Boolean)
      {
         var random:Number = NaN;
         super();
         loadGraphic(S_,false,true,64,32,false);
         this.player = _player;
         if(Math.random() > 0.5)
         {
            facing = LEFT;
         }
         else
         {
            facing = RIGHT;
         }
         this.grey = _grey;
         if(_grey)
         {
            velocity.x = Math.random() * 20;
            y = 30 + (Math.random() - 0.5) * 60;
            x = -Math.random() * 100 - 100;
            random = Math.random();
            scale = new FlxPoint(1 + random,1 + random);
         }
         else
         {
            y = this.player.y + 320;
            x = Math.random() * 300;
            random = Math.random() * 3;
            scale = new FlxPoint(1 + random,1 + random);
         }
         alpha = 0.5;
      }
      
      override public function update() : void
      {
         super.update();
         if(!this.grey && this.player.y > y + 320 && y < 9000)
         {
            kill();
         }
         if(this.grey && x > 3500 || x < -180)
         {
            kill();
         }
      }
   }
}


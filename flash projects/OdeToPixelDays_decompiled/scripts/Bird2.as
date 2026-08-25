package
{
   import org.flixel.FlxSprite;
   
   public class Bird2 extends FlxSprite
   {
      
      private static var S_:Class = Bird2_S_;
      
      public function Bird2(_x:Number, _y:Number)
      {
         super();
         x = _x + (Math.random() - 0.5) * 100;
         y = _y + (Math.random() - 0.5) * 100;
         loadGraphic(S_,true,true,6,6,false);
         addAnimation("fly",[0,1,2],6,true);
         addAnimation("fly2",[2,0,1],7,true);
         addAnimation("fly3",[1,2,0],5,true);
         var random:Number = Math.random();
         if(random < 0.34)
         {
            play("fly");
         }
         else if(random < 0.67)
         {
            play("fly2");
         }
         else
         {
            play("fly3");
         }
         var velocityX:Number = 50 + Math.random() * 60;
         var velocityY:Number = (Math.random() - 0.5) * 60;
         if(x < 160)
         {
            velocity.x = velocityX;
            facing = LEFT;
         }
         else
         {
            velocity.x = -velocityX;
            facing = RIGHT;
         }
         velocity.y = velocityY;
      }
      
      override public function update() : void
      {
         if(x < -20 && facing == RIGHT)
         {
            kill();
         }
         else if(x > 340 && facing == LEFT)
         {
            kill();
         }
      }
   }
}


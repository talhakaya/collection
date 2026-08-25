package
{
   import org.flixel.FlxSprite;
   
   public class Bulut extends FlxSprite
   {
      
      private static var img:Class = Bulut_img;
      
      public function Bulut(no:String, _X:Number, _Y:Number)
      {
         super();
         x = _X;
         y = _Y;
         loadGraphic(img,false,false,32,32,false);
         addAnimation("0",[0],0,false);
         addAnimation("1",[1],0,false);
         addAnimation("2",[2],0,false);
         addAnimation("3",[3],0,false);
         play(no);
         scrollFactor.x = scrollFactor.y = 0;
         velocity.x = 10 * Math.random();
         scale.x = 4;
         scale.y = 2;
      }
      
      override public function update() : void
      {
         super.update();
         if(x > 17 * 32)
         {
            x = -100;
         }
      }
   }
}


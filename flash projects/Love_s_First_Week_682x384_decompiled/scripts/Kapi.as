package
{
   import org.flixel.FlxSprite;
   
   public class Kapi extends FlxSprite
   {
      
      private static var img:Class = Kapi_img;
      
      public function Kapi(_X:Number, _Y:Number)
      {
         super();
         x = _X;
         y = _Y;
         loadGraphic(img,true,false,64,64,false);
         width = 32;
         height = 48;
         offset.x = 16;
         offset.y = 16;
         addAnimation("0",[0],0,false);
         addAnimation("open",[1,2,3,4],8,false);
         play("0");
      }
      
      public function open() : void
      {
         play("open");
      }
   }
}


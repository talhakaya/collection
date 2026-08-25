package
{
   import org.flixel.FlxSprite;
   
   public class Agac extends FlxSprite
   {
      
      private static var img:Class = Agac_img;
      
      public function Agac(no:String, _X:Number, _Y:Number)
      {
         super();
         x = _X;
         y = _Y;
         loadGraphic(img,false,false,50,125,false);
         addAnimation("0",[0],0,false);
         addAnimation("1",[1],0,false);
         addAnimation("2",[2],0,false);
         addAnimation("3",[3],0,false);
         addAnimation("4",[4],0,false);
         play(no);
         scrollFactor.x = 0.2 + 0.2 * Math.random();
         scrollFactor.y = 1;
      }
   }
}


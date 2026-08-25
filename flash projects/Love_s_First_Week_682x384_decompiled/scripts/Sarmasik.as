package
{
   import org.flixel.FlxSprite;
   
   public class Sarmasik extends FlxSprite
   {
      
      private static var img:Class = Sarmasik_img;
      
      public function Sarmasik(no:String, _X:Number, _Y:Number)
      {
         super();
         x = _X;
         y = _Y;
         loadGraphic(img,false,false,20,100,false);
         addAnimation("0",[0],0,false);
         addAnimation("1",[1],0,false);
         addAnimation("2",[2],0,false);
         addAnimation("3",[3],0,false);
         addAnimation("4",[4],0,false);
         play(no);
         scrollFactor.x = 0.4 + 0.2 * Math.random();
         scrollFactor.y = 1;
      }
   }
}


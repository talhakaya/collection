package
{
   import org.flixel.FlxSprite;
   
   public class TutoSpace extends FlxSprite
   {
      
      private static var S_tutoSpace:Class = TutoSpace_S_tutoSpace;
      
      public function TutoSpace(_x:Number, _y:Number)
      {
         super();
         x = _x;
         y = _y;
         loadGraphic(S_tutoSpace,true,false,48,24,false);
         addAnimation("0",[0,1,2],1,true);
         play("0");
      }
   }
}


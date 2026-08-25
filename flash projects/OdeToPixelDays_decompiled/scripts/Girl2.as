package
{
   import org.flixel.FlxSprite;
   
   public class Girl2 extends FlxSprite
   {
      
      private static var S_:Class = Girl2_S_;
      
      public function Girl2(_x:Number, _y:Number)
      {
         super();
         x = _x;
         y = _y;
         loadGraphic(S_,true,false,32,32,false);
         addAnimation("talk",[2,2,0,1,0,1,0,1,0,1,0,0,1,1,2,2,2,2,2,2,2,2,2,2,2],3,true);
         play("talk");
      }
   }
}


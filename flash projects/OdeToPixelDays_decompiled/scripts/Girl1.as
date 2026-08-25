package
{
   import org.flixel.FlxSprite;
   
   public class Girl1 extends FlxSprite
   {
      
      private static var S_:Class = Girl1_S_;
      
      public function Girl1(_x:Number, _y:Number)
      {
         super();
         x = _x;
         y = _y;
         loadGraphic(S_,true,false,32,32,false);
         addAnimation("talk",[1,1,1,1,0,2,0,2,0,0,2,0,2,2,0,1,1,1,1],3,true);
         play("talk");
      }
   }
}


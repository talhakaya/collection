package
{
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   
   public class Tel extends FlxSprite
   {
      
      private static var S_:Class = Tel_S_;
      
      public function Tel(_x:Number, _y:Number, _scale:FlxPoint)
      {
         super();
         loadGraphic(S_,false,false,64,64,false);
         x = _x;
         y = _y;
         scale = _scale;
      }
   }
}


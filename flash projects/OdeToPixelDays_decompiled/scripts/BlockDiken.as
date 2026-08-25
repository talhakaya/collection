package
{
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   
   public class BlockDiken extends FlxSprite
   {
      
      private static var S_:Class = BlockDiken_S_;
      
      public function BlockDiken(_x:Number, _y:Number, _scale:FlxPoint)
      {
         super();
         x = _x;
         y = _y;
         loadGraphic(S_,false,false,48,4,false);
         scale = _scale;
         width = 48 * scale.x;
         height = 4 * scale.x;
         if(scale.x == 3)
         {
            offset.y = -4;
            offset.x = -48;
         }
         else
         {
            centerOffsets();
         }
         immovable = true;
      }
   }
}


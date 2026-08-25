package
{
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   
   public class Door2UpsideDown extends FlxSprite
   {
      
      private static var S_kapi:Class = Door2UpsideDown_S_kapi;
      
      public function Door2UpsideDown(_X:Number, _Y:Number, _scale:FlxPoint)
      {
         super();
         scale = _scale;
         x = _X;
         y = _Y;
         loadGraphic(S_kapi,true,false,5,5,false);
         addAnimation("closed",[6],1,false);
         play("closed");
         width = 5 * scale.x;
         height = 5 * scale.x;
         centerOffsets();
      }
   }
}


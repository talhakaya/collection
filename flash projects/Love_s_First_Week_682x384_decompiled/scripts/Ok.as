package
{
   import org.flixel.FlxSprite;
   
   public class Ok extends FlxSprite
   {
      
      private static var img:Class = Ok_img;
      
      public var dir:String;
      
      public function Ok(direction:String, _X:Number, _Y:Number)
      {
         super();
         x = _X;
         y = _Y;
         loadGraphic(img,false,false,16,16,false);
         addAnimation("up",[0,1,2,3,2,1],12,true);
         addAnimation("right",[4,5,6,7,6,5],12,true);
         addAnimation("down",[8,9,10,11,10,9],12,true);
         addAnimation("left",[12,13,14,15,14,13],12,true);
         play(direction);
         this.dir = direction;
         width = 1;
         height = 1;
         offset.x = offset.y = 8;
      }
   }
}


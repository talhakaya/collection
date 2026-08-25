package
{
   import org.flixel.FlxSprite;
   
   public class Kafa extends FlxSprite
   {
      
      private static var img1:Class = Kafa_img1;
      
      private static var img2:Class = Kafa_img2;
      
      public function Kafa(isNaz:Boolean)
      {
         super();
         if(isNaz)
         {
            loadGraphic(img1,true,false,20,17);
            x = 3 + 18;
            y = 198 + 20;
         }
         else
         {
            loadGraphic(img2,true,false,20,17);
            x = 393 + 18;
            y = 198 + 20;
         }
         addAnimation("idle",[0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,3,3,3,3,3,3,3,3,2,4,4,4,4,4,4,4,2,0,0,0,0,0,0,2],6,true);
         addAnimation("talk",[1,1,0,0,1,1,2,3,3,1,1,4,4,2],6,true);
         alpha = 0;
         scale.x = scale.y = 3;
         scrollFactor.x = scrollFactor.y = 0;
         play("idle");
      }
   }
}


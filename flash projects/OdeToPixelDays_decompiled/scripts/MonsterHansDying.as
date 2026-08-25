package
{
   import org.flixel.FlxG;
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   
   public class MonsterHansDying extends FlxSprite
   {
      
      private static var S_:Class = MonsterHansDying_S_;
      
      private static var Sfx:Class = MonsterHansDying_Sfx;
      
      public function MonsterHansDying(_X:Number, _Y:Number, _scale:FlxPoint)
      {
         super();
         x = _X;
         y = _Y;
         scale = _scale;
         loadGraphic(S_,true,false,32,32,false);
         addAnimation("ol",[0,1,2,3],1.5,false);
         play("ol");
         maxVelocity.x = 100;
         maxVelocity.y = 400;
         acceleration.y = 400;
         drag.x = maxVelocity.x * 4;
         width = 64;
         height = 14;
         centerOffsets();
         offset.y = 34;
         FlxG.play(Sfx);
      }
   }
}


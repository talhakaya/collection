package
{
   import org.flixel.FlxG;
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   
   public class Door2 extends FlxSprite
   {
      
      private static var S_kapi:Class = Door2_S_kapi;
      
      private static var Sfx:Class = Door2_Sfx;
      
      private static var Sfx2:Class = Door2_Sfx2;
      
      private var used:Boolean;
      
      public var isOpen:Boolean;
      
      public var cheerleader:Boolean;
      
      public function Door2(_X:Number, _Y:Number, _used:Boolean, _scale:FlxPoint)
      {
         super();
         scale = _scale;
         x = _X;
         y = _Y;
         this.isOpen = false;
         loadGraphic(S_kapi,true,false,5,5,false);
         addAnimation("closed",[0],1,false);
         addAnimation("closedForever",[5],1,false);
         addAnimation("open",[1,2,3,4],6,false);
         addAnimation("openAndClose",[1,2,3,4,3,2,1,0],6,false);
         this.used = _used;
         if(!this.used)
         {
            play("closed");
         }
         else
         {
            play("closedForever");
            FlxG.play(Sfx2);
         }
         this.cheerleader = false;
         width = 5 * scale.x;
         height = 5 * scale.x;
         centerOffsets();
      }
      
      override public function kill() : void
      {
         if(!this.used)
         {
            play("openAndClose");
            FlxG.play(Sfx);
         }
      }
   }
}


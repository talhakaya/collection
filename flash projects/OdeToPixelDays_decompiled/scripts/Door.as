package
{
   import org.flixel.FlxG;
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   
   public class Door extends FlxSprite
   {
      
      private static var S_kapi:Class = Door_S_kapi;
      
      private static var Sfx:Class = Door_Sfx;
      
      private static var Sfx2:Class = Door_Sfx2;
      
      private var used:Boolean;
      
      public var isOpen:Boolean;
      
      public var cheerleader:Boolean;
      
      public function Door(_X:Number, _Y:Number, _used:Boolean, _scale:FlxPoint)
      {
         super();
         scale = _scale;
         x = _X;
         y = _Y;
         this.isOpen = false;
         loadGraphic(S_kapi,true,false,20,40,false);
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
         if(scale.x == 2)
         {
            width = 40;
            height = 80;
            centerOffsets();
         }
         else if(scale.x == 4)
         {
            width = 80;
            height = 160;
            centerOffsets();
         }
         else if(scale.x == 8)
         {
            width = 160;
            height = 320;
            centerOffsets();
         }
         else if(scale.x == 16)
         {
            width = 320;
            height = 640;
            centerOffsets();
         }
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


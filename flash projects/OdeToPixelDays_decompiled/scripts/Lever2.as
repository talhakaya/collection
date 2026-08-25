package
{
   import org.flixel.FlxG;
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   import org.flixel.FlxTimer;
   
   public class Lever2 extends FlxSprite
   {
      
      private static var S_lever:Class = Lever2_S_lever;
      
      private static var S_leverkucuk:Class = Lever2_S_leverkucuk;
      
      private static var Sfx:Class = Lever2_Sfx;
      
      public var used:Boolean;
      
      public var isOpen:Boolean;
      
      private var timer:FlxTimer;
      
      public var usable:Boolean;
      
      public var bossFight:Boolean;
      
      public function Lever2(_X:Number, _Y:Number, _used:Boolean, _scale:FlxPoint, _small:Boolean)
      {
         super();
         scale = _scale;
         x = _X;
         y = _Y;
         this.isOpen = false;
         if(_small)
         {
            loadGraphic(S_leverkucuk,true,false,5,5,false);
            width = 3 * scale.x;
            height = 5 * scale.x;
         }
         else
         {
            loadGraphic(S_lever,true,false,9,16,false);
            width = 9 * scale.x;
            height = 16 * scale.x;
         }
         addAnimation("closed",[0],1,false);
         addAnimation("opened",[0],1,false);
         addAnimation("usable",[5,6,7,8,9,0],12,false);
         addAnimation("notusable",[0,9,8,7,6,5],12,false);
         addAnimation("open",[1,2,3,4,3,2,1,0],12,false);
         addAnimation("close",[1,2,3,4,3,2,1,0],12,false);
         this.used = _used;
         this.timer = new FlxTimer();
         this.usable = true;
         if(scale.x != 1)
         {
            centerOffsets();
         }
      }
      
      override public function kill() : void
      {
         if(this.usable && !this.used)
         {
            if(!this.isOpen)
            {
               play("open");
               this.isOpen = true;
            }
            else
            {
               play("close");
               this.isOpen = false;
            }
            this.usable = false;
            this.timer.start(1,1,this.makeUsable);
            FlxG.play(Sfx);
         }
      }
      
      private function makeUsable(e:FlxTimer) : void
      {
         this.usable = true;
      }
      
      public function letBeUsable() : void
      {
         play("usable");
         this.usable = true;
      }
      
      public function notUsable() : void
      {
         play("notusable");
         this.usable = false;
      }
   }
}


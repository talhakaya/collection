package
{
   import org.flixel.FlxG;
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   import org.flixel.FlxTimer;
   
   public class Machine extends FlxSprite
   {
      
      private static var S_machine:Class = Machine_S_machine;
      
      private static var SfxOpen:Class = Machine_SfxOpen;
      
      private static var SfxClose:Class = Machine_SfxClose;
      
      private static var Sfx:Class = Machine_Sfx;
      
      private var used:Boolean;
      
      public var isOpen:Boolean;
      
      public var wait:Boolean;
      
      public var cheerleader:Boolean;
      
      public var timer:FlxTimer;
      
      public var timer2use:FlxTimer;
      
      public var leverUsed:Boolean;
      
      private var count:int;
      
      private var working:Boolean;
      
      public var player:Hans;
      
      public var playerSensitive:Boolean;
      
      private var volume:Number;
      
      public function Machine(_X:Number, _Y:Number, _used:Boolean, _scale:FlxPoint)
      {
         super();
         this.leverUsed = false;
         scale = _scale;
         x = _X;
         y = _Y;
         this.isOpen = false;
         loadGraphic(S_machine,true,false,32,16,false);
         addAnimation("closed",[0],1,false);
         addAnimation("start",[1,2,3],2,false);
         addAnimation("open",[4,8,5,9,6,10,7,11],12,true);
         addAnimation("stop",[3,2,1,0],2,false);
         this.used = _used;
         if(!this.used)
         {
            play("closed");
            this.working = false;
         }
         else
         {
            play("open");
            this.working = true;
         }
         if(scale.x == 2)
         {
            width = 64;
            height = 32;
            centerOffsets();
         }
         else if(scale.x == 4)
         {
            width = 128;
            height = 64;
            centerOffsets();
         }
         else if(scale.x == 8)
         {
            width = 256;
            height = 128;
            centerOffsets();
         }
         else if(scale.x == 16)
         {
            width = 512;
            height = 256;
            centerOffsets();
         }
         this.cheerleader = false;
         this.wait = false;
         this.timer2use = new FlxTimer();
         this.timer = new FlxTimer();
         this.count = 0;
         this.playerSensitive = false;
      }
      
      override public function update() : void
      {
         super.update();
         if(this.used && this.working)
         {
            if(this.count == 10)
            {
               if(this.playerSensitive)
               {
                  if(Math.abs(this.player.x - x - scale.x * 10) > 160)
                  {
                     if(Math.abs(this.player.x - x - scale.x * 10) > 320)
                     {
                        this.volume = 0;
                     }
                     else
                     {
                        this.volume = 25600 / (Math.abs(this.player.x - x - scale.x * 10) * Math.abs(this.player.x - x - scale.x * 10));
                     }
                  }
                  else
                  {
                     this.volume = 1;
                  }
               }
               else
               {
                  this.volume = 1;
               }
               FlxG.play(Sfx,this.volume);
               this.count = 0;
            }
            ++this.count;
         }
      }
      
      override public function kill() : void
      {
         if(!this.wait && !this.leverUsed)
         {
            this.timer2use.start(1,1,this.usable);
            this.leverUsed = true;
            this.wait = true;
            if(!this.used)
            {
               play("start");
               this.timer.start(1.5,1,this.openMode);
               this.used = true;
               FlxG.play(SfxOpen);
            }
            else
            {
               play("stop");
               this.timer.start(1.5,1,this.closedMode);
               this.used = false;
               FlxG.play(SfxClose);
            }
         }
      }
      
      private function openMode(e:FlxTimer) : void
      {
         play("open");
         this.working = true;
      }
      
      private function closedMode(e:FlxTimer) : void
      {
         play("closed");
      }
      
      private function usable(e:FlxTimer) : void
      {
      }
   }
}


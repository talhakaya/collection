package
{
   import org.flixel.FlxG;
   import org.flixel.FlxObject;
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   import org.flixel.FlxTimer;
   
   public class Cheerleader extends FlxSprite
   {
      
      private static var S_cheerleader:Class = Cheerleader_S_cheerleader;
      
      private static var S_cheerleader16:Class = Cheerleader_S_cheerleader16;
      
      private static var S_cheerleader8:Class = Cheerleader_S_cheerleader8;
      
      private static var S_cheerleader4:Class = Cheerleader_S_cheerleader4;
      
      private static var S_cheerleader2:Class = Cheerleader_S_cheerleader2;
      
      private static var Sfxwalk1:Class = Cheerleader_Sfxwalk1;
      
      private static var Sfxwalk2:Class = Cheerleader_Sfxwalk2;
      
      private static var Sfxwalk3:Class = Cheerleader_Sfxwalk3;
      
      private static var Sfxwalk4:Class = Cheerleader_Sfxwalk4;
      
      private static var Sfxwalk5:Class = Cheerleader_Sfxwalk5;
      
      public var isWalkin:Boolean;
      
      public var isLeft:Boolean;
      
      public var randomNumber:Number;
      
      public var whichLevel:uint;
      
      private var player:Hans;
      
      public var gone:Boolean;
      
      public var count:int;
      
      public var countForSfxwalk:int;
      
      private var sonBolumBakti:Boolean;
      
      public function Cheerleader(_X:uint, _Y:uint, _scale:FlxPoint, _level:uint, _player:Hans)
      {
         super();
         this.x = _X;
         this.y = _Y;
         this.scale = _scale;
         this.whichLevel = _level;
         this.player = _player;
         acceleration.y = 200;
         if(scale.x == 1)
         {
            loadGraphic(S_cheerleader,true,true,20,32,false);
            width = 8;
            height = 30;
            offset.x = 6;
            offset.y = 2;
            addAnimation("idle",[0],12,true);
            addAnimation("walk",[8,7,6,5,4,3,2,1],8,true);
            addAnimation("talk",[10,11,10,11,11,10,0,0,0,0,0,0,0],4,true);
            addAnimation("stare",[9,9,9,9,9,9,10,11,10,10,11,11,10,11,10,11,11,10,0,0,0,0,0,0,0],4,false);
         }
         else if(scale.x == 2)
         {
            loadGraphic(S_cheerleader16,true,true,10,16,false);
            width = 8;
            height = 30;
            offset.x = 6;
            offset.y = -6;
            addAnimation("idle",[0],12,true);
            addAnimation("walk",[1,2,3,4,5,6],6,true);
         }
         else if(scale.x == 4)
         {
            loadGraphic(S_cheerleader8,true,true,6,8,false);
            width = 8;
            height = 32;
            addAnimation("idle",[0],12,true);
            addAnimation("walk",[1,3,2,3],4,true);
         }
         else if(scale.x == 8)
         {
            loadGraphic(S_cheerleader4,true,true,4,4,false);
            width = 16;
            height = 32;
            addAnimation("idle",[0],12,true);
            addAnimation("walk",[1,2],2,true);
         }
         else if(scale.x == 16)
         {
            loadGraphic(S_cheerleader2,true,true,1,2,false);
            width = 16;
            height = 32;
            addAnimation("idle",[0,1,2,3,2,1,0],12,true);
            addAnimation("walk",[0,1,2,3,2,1,0],12,true);
         }
         if(scale.x > 3)
         {
            centerOffsets();
         }
         if(this.whichLevel == 1)
         {
            maxVelocity.x = 48;
         }
         else if(this.whichLevel == 7)
         {
            maxVelocity.x = 60;
         }
         else if(this.whichLevel == 11)
         {
            maxVelocity.x = 100;
         }
         else if(this.whichLevel == 15 || this.whichLevel == 24 || this.whichLevel == 34)
         {
            maxVelocity.x = 130;
         }
         else if(this.whichLevel == 23)
         {
            maxVelocity.x = 80;
         }
         else if(this.whichLevel == 30 || this.whichLevel == 31)
         {
            maxVelocity.x = 120;
         }
         drag.x = maxVelocity.x * 4;
         this.isWalkin = false;
         this.isLeft = false;
         if(this.whichLevel == 43)
         {
            play("talk");
         }
         else
         {
            play("idle");
         }
         this.gone = false;
         facing = LEFT;
         this.count = 0;
         this.countForSfxwalk = 0;
      }
      
      override public function update() : void
      {
         if((this.whichLevel == 1 || this.whichLevel == 7) && !this.gone)
         {
            if(x > 284)
            {
               this.isWalkin = false;
               this.gone = true;
            }
            else if(this.player.x > x - 140)
            {
               this.isWalkin = true;
               this.isLeft = false;
            }
         }
         else if(this.whichLevel == 11 && !this.gone)
         {
            if(x > 424)
            {
               this.isWalkin = false;
               this.gone = true;
            }
            else if(this.player.x > x - 100)
            {
               this.isWalkin = true;
               this.isLeft = false;
            }
         }
         else if(this.whichLevel == 15 && !this.gone)
         {
            if(x > 560)
            {
               this.isWalkin = false;
               this.gone = true;
            }
            else if(this.player.x > x - 64)
            {
               this.isWalkin = true;
               this.isLeft = false;
            }
         }
         else if(this.whichLevel == 23 && !this.gone)
         {
            if(x < 220)
            {
               this.isWalkin = false;
            }
            else if(this.player.y > 475)
            {
               this.isWalkin = true;
               this.isLeft = true;
            }
         }
         else if(this.whichLevel == 24 && !this.gone)
         {
            if(x > 560)
            {
               this.isWalkin = false;
               this.gone = true;
            }
            else if(this.player.x > x - 120)
            {
               this.isWalkin = true;
               this.isLeft = false;
            }
         }
         else if(this.whichLevel == 30 && !this.gone)
         {
            if(x > 200)
            {
               this.isLeft = true;
               this.isWalkin = false;
               ++this.count;
            }
            else if(this.player.x > 63)
            {
               this.isWalkin = true;
               this.isLeft = false;
            }
         }
         else if(this.whichLevel == 31 && !this.gone)
         {
            if(x > 576)
            {
               this.isWalkin = false;
               this.gone = true;
            }
            else if(this.player.x > x - 64)
            {
               this.isWalkin = true;
               this.isLeft = false;
            }
         }
         else if(this.whichLevel == 34 && !this.gone)
         {
            if(x > 480)
            {
               this.isWalkin = false;
            }
            else if(this.player.x > x - 130)
            {
               this.isWalkin = true;
               this.isLeft = false;
            }
         }
         else if(this.whichLevel == 43)
         {
            this.isWalkin = false;
            this.isLeft = false;
            facing = RIGHT;
            if(!this.sonBolumBakti && this.player.x > x - 28)
            {
               play("stare");
               this.sonBolumBakti = true;
               new FlxTimer().start(2,1,this.talkAgain);
            }
            else if(!this.sonBolumBakti)
            {
               play("talk");
            }
         }
         if(this.gone && alpha > 0)
         {
            alpha -= 0.025;
         }
         else if(alpha == 0)
         {
            kill();
         }
         if(this.whichLevel != 43)
         {
            if(this.isWalkin)
            {
               play("walk");
               if(this.isLeft)
               {
                  acceleration.x = -maxVelocity.x * 4;
                  facing = LEFT;
               }
               else
               {
                  acceleration.x = maxVelocity.x * 4;
                  facing = RIGHT;
               }
            }
            else
            {
               acceleration.x = 0;
               play("idle");
            }
         }
         if(isTouching(FlxObject.FLOOR) && this.isWalkin)
         {
            if(this.countForSfxwalk == 0)
            {
               if(scale.x == 1)
               {
                  FlxG.play(Sfxwalk1);
               }
               else if(scale.x == 2)
               {
                  FlxG.play(Sfxwalk2);
               }
               else if(scale.x == 4)
               {
                  FlxG.play(Sfxwalk3);
               }
               else if(scale.x == 8)
               {
                  FlxG.play(Sfxwalk4);
               }
               else if(scale.x == 16)
               {
                  FlxG.play(Sfxwalk5);
               }
               this.countForSfxwalk = 0;
            }
            ++this.countForSfxwalk;
            if(this.countForSfxwalk == 30)
            {
               this.countForSfxwalk = 0;
            }
         }
         else
         {
            this.countForSfxwalk = 0;
         }
         super.update();
      }
      
      private function talkAgain(a:FlxTimer) : void
      {
         play("talk");
      }
   }
}


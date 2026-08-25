package
{
   import org.flixel.FlxG;
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   
   public class Cheerlover extends FlxSprite
   {
      
      private static var S_cheerleader2:Class = Cheerlover_S_cheerleader2;
      
      private static var Sfxjump:Class = Cheerlover_Sfxjump;
      
      public var isWalkin:Boolean;
      
      public var isJumpin:Boolean;
      
      public var isLeft:Boolean;
      
      public var fading:Boolean;
      
      private var randomNumber:Number;
      
      private var count:Number;
      
      private var count2:Number;
      
      public var countForSfxwalk:int;
      
      public var whichLevel:uint;
      
      private var player:Hans;
      
      public var firstStateChange:Boolean;
      
      public var isAttachedToHans:Boolean;
      
      public var isGoneCrazy:Boolean;
      
      public var isOnLeftOfHans:Boolean;
      
      public var isPlayerWalkinRight:Boolean;
      
      public function Cheerlover(_X:uint, _Y:uint, _scale:FlxPoint, _level:uint, _player:Hans, _attached:Boolean, _crazy:Boolean)
      {
         super();
         this.x = _X;
         this.y = _Y;
         this.scale = _scale;
         this.whichLevel = _level;
         this.player = _player;
         this.isAttachedToHans = _attached;
         this.isGoneCrazy = _crazy;
         this.fading = false;
         loadGraphic(S_cheerleader2,true,true,1,2,false);
         width = 16;
         height = 32;
         addAnimation("idle",[0,1,2,3,2,1,0],6,true);
         play("idle");
         centerOffsets();
         maxVelocity.x = 170;
         maxVelocity.y = 200;
         drag.x = maxVelocity.x * 4;
         acceleration.y = 400;
         this.isWalkin = false;
         this.isJumpin = false;
         this.isLeft = false;
         play("idle");
         this.count = 0;
         this.count2 = 0;
         this.isOnLeftOfHans = false;
         if(this.isAttachedToHans && this.isGoneCrazy)
         {
            maxVelocity.x = 100;
         }
      }
      
      override public function update() : void
      {
         this.randomNumber = Math.random();
         if(this.count < 21)
         {
            ++this.count;
         }
         else
         {
            this.count = 0;
         }
         if(this.count2 < 41)
         {
            ++this.count2;
         }
         else
         {
            this.count2 = 0;
         }
         if(!this.isAttachedToHans && !this.isGoneCrazy && this.firstStateChange)
         {
            this.firstStateChange = false;
            this.isAttachedToHans = true;
         }
         if(!this.isAttachedToHans && !this.isGoneCrazy)
         {
            this.isWalkin = false;
         }
         else if(this.isAttachedToHans && !this.isGoneCrazy)
         {
            this.isWalkin = true;
            if(!this.isOnLeftOfHans)
            {
               if(this.count == 0)
               {
                  this.isLeft = true;
               }
            }
            else if(this.count == 0)
            {
               this.isLeft = false;
            }
            if(this.count2 == 0)
            {
               this.isJumpin = true;
            }
         }
         else if(this.isAttachedToHans && this.isGoneCrazy)
         {
            if(!this.isOnLeftOfHans)
            {
               if(this.count == 0)
               {
                  this.isWalkin = false;
               }
            }
            else if(this.count == 0)
            {
               this.isWalkin = true;
               this.isLeft = false;
            }
         }
         else if(!this.isAttachedToHans && this.isGoneCrazy)
         {
            maxVelocity.x = 170;
            this.isWalkin = true;
            this.isLeft = false;
            if(x > 2496)
            {
               this.isWalkin = false;
               this.fading = true;
               x = 2496;
            }
         }
         if(x > 2200)
         {
            this.isAttachedToHans = false;
         }
         if(this.isWalkin)
         {
            if(this.isLeft)
            {
               if(this.isPlayerWalkinRight)
               {
                  acceleration.x = 0;
               }
               else
               {
                  acceleration.x = -maxVelocity.x * 4;
               }
            }
            else
            {
               acceleration.x = maxVelocity.x * 4;
            }
         }
         else
         {
            acceleration.x = 0;
         }
         if(this.isJumpin && isTouching(FLOOR))
         {
            this.isJumpin = false;
            velocity.y = -maxVelocity.y * 0.7;
            FlxG.play(Sfxjump);
         }
         if(this.fading && alpha > 0)
         {
            alpha -= 0.025;
         }
         else if(alpha <= 0)
         {
            kill();
         }
         super.update();
      }
   }
}


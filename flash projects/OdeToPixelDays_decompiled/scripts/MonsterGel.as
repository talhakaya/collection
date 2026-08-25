package
{
   import org.flixel.FlxG;
   import org.flixel.FlxObject;
   import org.flixel.FlxPoint;
   
   public class MonsterGel extends Monster
   {
      
      private static var S_monstergel:Class = MonsterGel_S_monstergel;
      
      private static var Sfxdie:Class = MonsterGel_Sfxdie;
      
      private static var Sfxjump:Class = MonsterGel_Sfxjump;
      
      public function MonsterGel(_X:Number, _Y:Number, _scale:FlxPoint)
      {
         super();
         this.x = _X;
         this.y = _Y;
         this.scale = _scale;
         loadGraphic(S_monstergel,true,true,8,8,false);
         maxVelocity.x = 30;
         maxVelocity.y = 200;
         acceleration.y = 200;
         drag.x = maxVelocity.x * 4;
         if(scale.x == 1)
         {
            width = 6;
            height = 5;
            offset.x = 1;
            offset.y = 3;
         }
         else if(scale.x == 2)
         {
            width = 14;
            height = 12;
            offset.x = -3;
            offset.y = 0;
         }
         else if(scale.x == 4)
         {
            width = 28;
            height = 24;
            offset.x = -10;
            offset.y = -4;
         }
         else if(scale.x == 8)
         {
            width = 64;
            height = 48;
            offset.x = -28;
            offset.y = -12;
         }
         addAnimation("idle",[0],2,true);
         addAnimation("walk",[0,1],3,true);
         addAnimation("die",[2,3,4,5],6,false);
         isDead = false;
         isDeadOnce = false;
         isWalkin = false;
         if(Math.random() > 0.5)
         {
            isLeft = true;
         }
         else
         {
            isLeft = false;
         }
      }
      
      override public function update() : void
      {
         if(!isDead)
         {
            randomNumber = Math.random();
            if(!isWalkin)
            {
               if(randomNumber < 0.01)
               {
                  isWalkin = true;
               }
            }
            else if(randomNumber < 0.006)
            {
               isWalkin = false;
            }
            else if(randomNumber < 0.013 || velocity.x == 0)
            {
               isLeft = !isLeft;
            }
            randomNumber = Math.random();
            if(randomNumber < 0.006)
            {
               isJumpin = true;
            }
         }
         if(!isDead)
         {
            if(isWalkin)
            {
               play("walk");
               if(isLeft)
               {
                  acceleration.x = -maxVelocity.x * 4;
               }
               else
               {
                  acceleration.x = maxVelocity.x * 4;
               }
            }
            else
            {
               acceleration.x = 0;
               play("idle");
            }
            if(isJumpin && isTouching(FlxObject.FLOOR))
            {
               isJumpin = false;
               velocity.y = -maxVelocity.y / 2;
               FlxG.play(Sfxjump);
            }
         }
         else if(!isDeadOnce)
         {
            play("die");
            acceleration.x = 0;
            isDeadOnce = true;
            FlxG.play(Sfxdie);
         }
         super.update();
      }
   }
}


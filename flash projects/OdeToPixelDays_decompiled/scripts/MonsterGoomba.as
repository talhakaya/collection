package
{
   import org.flixel.FlxG;
   import org.flixel.FlxPoint;
   
   public class MonsterGoomba extends Monster
   {
      
      private static var S_monstergoomba:Class = MonsterGoomba_S_monstergoomba;
      
      private static var S_monstergoomba1:Class = MonsterGoomba_S_monstergoomba1;
      
      private static var Sfxdie:Class = MonsterGoomba_Sfxdie;
      
      private static var Sfx:Class = MonsterGoomba_Sfx;
      
      public var simpleWalk:Boolean;
      
      public function MonsterGoomba(_X:Number, _Y:Number, _scale:FlxPoint)
      {
         super();
         this.x = _X;
         this.y = _Y;
         this.scale = _scale;
         if(scale.x == 1)
         {
            loadGraphic(S_monstergoomba1,true,true,16,16,false);
         }
         else
         {
            loadGraphic(S_monstergoomba,true,true,16,16,false);
         }
         maxVelocity.x = 20;
         maxVelocity.y = 155;
         acceleration.y = 200;
         drag.x = maxVelocity.x * 4;
         if(scale.x == 1)
         {
            width = 10;
            height = 12;
            offset.x = 3;
            offset.y = 4;
         }
         else if(scale.x == 2)
         {
            width = 20;
            height = 24;
            offset.x = -2;
            offset.y = 0;
         }
         else if(scale.x == 4)
         {
            width = 40;
            height = 48;
            offset.x = -8;
            offset.y = -8;
         }
         else if(scale.x == 8)
         {
            width = 128;
            height = 128;
            offset.x = 0;
            offset.y = 0;
         }
         else if(scale.x == 16)
         {
            width = 256;
            height = 256;
            offset.x = 0;
            offset.y = 0;
         }
         else if(scale.x == 32)
         {
            width = 512;
            height = 512;
            offset.x = 0;
            offset.y = 0;
         }
         addAnimation("idle",[0,1,0,0],4,true);
         addAnimation("walk",[2,3,10,4,5,11],4,true);
         addAnimation("die",[6,7,8,9],6,false);
         addAnimation("getHit",[6],6,false);
         play("idle");
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
         this.simpleWalk = false;
      }
      
      override public function update() : void
      {
         if(!isDead)
         {
            if(!this.simpleWalk)
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
            }
            else
            {
               isWalkin = true;
               if(velocity.x == 0)
               {
                  isLeft = !isLeft;
               }
            }
            if(Math.random() < 0.002)
            {
               FlxG.play(Sfx);
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


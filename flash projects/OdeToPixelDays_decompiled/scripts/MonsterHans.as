package
{
   import org.flixel.FlxG;
   import org.flixel.FlxObject;
   import org.flixel.FlxPoint;
   
   public class MonsterHans extends Monster
   {
      
      private static var S_:Class = MonsterHans_S_;
      
      private static var Sfxfloor:Class = MonsterHans_Sfxfloor;
      
      private static var Sfxjump:Class = MonsterHans_Sfxjump;
      
      private static var Sfxwalk:Class = MonsterHans_Sfxwalk;
      
      private static var Sfxhurt:Class = MonsterHans_Sfxhurt;
      
      private var whichlevel:int;
      
      private var hans:Hans;
      
      public var fading:Boolean;
      
      public var hp:int;
      
      private var alpha2fade:Number;
      
      private var fadingOut:Boolean;
      
      private var istouchingfloor:Boolean;
      
      private var count:int;
      
      public function MonsterHans(_X:Number, _Y:Number, _scale:FlxPoint, _level:int, _player:Hans)
      {
         super();
         this.x = _X;
         this.y = _Y;
         this.scale = _scale;
         this.whichlevel = _level;
         this.hans = _player;
         loadGraphic(S_,true,true,16,32,false);
         addAnimation("stand",[0],12,true);
         addAnimation("run",[1,2,3,4,5,6,7,8],12,true);
         addAnimation("jump",[9],12,true);
         addAnimation("fall",[10,11],6,true);
         if(this.whichlevel == 36)
         {
            maxVelocity.x = 110;
         }
         else if(this.whichlevel == 37)
         {
            maxVelocity.x = 140;
         }
         else if(this.whichlevel == 38)
         {
            maxVelocity.x = 70;
         }
         else if(this.whichlevel == 39)
         {
            maxVelocity.x = 100;
         }
         maxVelocity.y = 400;
         acceleration.y = 400;
         drag.x = maxVelocity.x * 4;
         width = 24;
         height = 64;
         centerOffsets();
         isDead = false;
         isDeadOnce = false;
         this.istouchingfloor = false;
         isWalkin = false;
         if(Math.random() > 0.5)
         {
            isLeft = true;
         }
         else
         {
            isLeft = false;
         }
         if(this.whichlevel == 39)
         {
            this.hp = 6;
            this.fadingOut = true;
         }
         this.count = 0;
      }
      
      override public function update() : void
      {
         if(!isDead)
         {
            if(this.whichlevel == 36)
            {
               if(this.hans.x > 450)
               {
                  if(this.hans.x > x + 32)
                  {
                     isWalkin = true;
                     isLeft = false;
                  }
                  else if(x > this.hans.x + 32)
                  {
                     isWalkin = true;
                     isLeft = true;
                  }
                  else
                  {
                     isWalkin = false;
                     if(this.hans.x > x)
                     {
                        isLeft = false;
                     }
                     else
                     {
                        isLeft = true;
                     }
                  }
                  if(x > 400 && x < 410 || x > 560 && x < 570)
                  {
                     isJumpin = true;
                  }
               }
               else if(x < 440)
               {
                  isWalkin = false;
               }
            }
            else if(this.whichlevel == 37)
            {
               if(this.hans.x > x + 12)
               {
                  isWalkin = true;
                  isLeft = false;
               }
               else if(x > this.hans.x + 12)
               {
                  isWalkin = true;
                  isLeft = true;
               }
               else
               {
                  isWalkin = false;
                  if(this.hans.x > x)
                  {
                     isLeft = false;
                  }
                  else
                  {
                     isLeft = true;
                  }
               }
               if((x > 200 && x < 210 || x > 304 && x < 312 || x > 392 && x < 416 || x > 488 && x < 502 || x > 600 && x < 624 || x > 720 && x < 744) && (this.hans.x < x + 64 && x < this.hans.x + 64))
               {
                  isJumpin = true;
               }
            }
            else if(this.whichlevel == 38)
            {
               isWalkin = true;
               isLeft = false;
            }
            else if(this.whichlevel == 39)
            {
               if(this.hans.x > 776)
               {
                  if(isWalkin && (velocity.x == 0 || this.hans.y < y - 32))
                  {
                     if(this.hp > 2)
                     {
                        isJumpin = true;
                     }
                  }
                  if(y >= this.hans.y - 64)
                  {
                     if(x > this.hans.x + 4)
                     {
                        isWalkin = true;
                        isLeft = true;
                     }
                     else if(x < this.hans.x - 4)
                     {
                        isWalkin = true;
                        isLeft = false;
                     }
                     else
                     {
                        isWalkin = false;
                     }
                  }
                  else
                  {
                     isWalkin = true;
                     if(velocity.x == 0)
                     {
                        if(x > this.hans.x)
                        {
                           isLeft = true;
                        }
                        else
                        {
                           isLeft = false;
                        }
                     }
                  }
               }
               if(this.hp == 6)
               {
                  this.alpha2fade = 1;
               }
               else if(this.hp == 5)
               {
                  this.alpha2fade = 0.8;
               }
               else if(this.hp == 4)
               {
                  this.alpha2fade = 0.6;
               }
               else if(this.hp == 3)
               {
                  this.alpha2fade = 0.4;
               }
               else if(this.hp == 2)
               {
                  this.alpha2fade = 0.2;
               }
               else if(this.hp == 1)
               {
                  this.alpha2fade = 0;
               }
               if(this.fadingOut)
               {
                  if(alpha <= this.alpha2fade)
                  {
                     this.fadingOut = false;
                     alpha = this.alpha2fade;
                  }
                  else
                  {
                     alpha -= (1 - this.alpha2fade) / 20;
                  }
               }
               else if(alpha >= 1)
               {
                  this.fadingOut = true;
                  alpha = 1;
               }
               else
               {
                  alpha += (1 - this.alpha2fade) / 20;
               }
            }
            if(isTouching(FlxObject.FLOOR) && isWalkin && velocity.x != 0)
            {
               if(this.count == 10)
               {
                  FlxG.play(Sfxwalk);
               }
               ++this.count;
               if(this.count == 20)
               {
                  this.count = 0;
               }
            }
            else
            {
               this.count = 0;
            }
         }
         if(this.fading && alpha > 0)
         {
            alpha -= 0.02;
         }
         if(!isDead)
         {
            if(isWalkin)
            {
               play("run");
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
               play("stand");
            }
            if(isLeft)
            {
               facing = FlxObject.LEFT;
            }
            else
            {
               facing = FlxObject.RIGHT;
            }
            if(isJumpin && isTouching(FlxObject.FLOOR))
            {
               isJumpin = false;
               velocity.y = -maxVelocity.y / 2;
               FlxG.play(Sfxjump);
            }
            if(velocity.y > 0)
            {
               play("fall");
            }
            else if(velocity.y < 0)
            {
               play("jump");
            }
         }
         else if(!isDeadOnce)
         {
            acceleration.x = 0;
            isDeadOnce = true;
         }
         if(!this.istouchingfloor && isTouching(FlxObject.FLOOR))
         {
            FlxG.play(Sfxfloor);
         }
         this.istouchingfloor = isTouching(FlxObject.FLOOR);
         super.update();
      }
   }
}


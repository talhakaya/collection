package
{
   import org.flixel.FlxG;
   import org.flixel.FlxObject;
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   
   public class Hans extends FlxSprite
   {
      
      private static var S_hans:Class = Hans_S_hans;
      
      private static var S_hans16:Class = Hans_S_hans16;
      
      private static var S_hans8:Class = Hans_S_hans8;
      
      private static var S_hans4:Class = Hans_S_hans4;
      
      private static var S_hans2:Class = Hans_S_hans2;
      
      private static var S_hans1:Class = Hans_S_hans1;
      
      private static var S_dusme:Class = Hans_S_dusme;
      
      private static var Sfxfloor1:Class = Hans_Sfxfloor1;
      
      private static var Sfxfloor2:Class = Hans_Sfxfloor2;
      
      private static var Sfxfloor3:Class = Hans_Sfxfloor3;
      
      private static var Sfxfloor4:Class = Hans_Sfxfloor4;
      
      private static var Sfxfloor5:Class = Hans_Sfxfloor5;
      
      private static var Sfxhurt1:Class = Hans_Sfxhurt1;
      
      private static var Sfxhurt2:Class = Hans_Sfxhurt2;
      
      private static var Sfxhurt3:Class = Hans_Sfxhurt3;
      
      private static var Sfxhurt4:Class = Hans_Sfxhurt4;
      
      private static var Sfxhurt5:Class = Hans_Sfxhurt5;
      
      private static var Sfxjump1:Class = Hans_Sfxjump1;
      
      private static var Sfxjump2:Class = Hans_Sfxjump2;
      
      private static var Sfxjump3:Class = Hans_Sfxjump3;
      
      private static var Sfxjump4:Class = Hans_Sfxjump4;
      
      private static var Sfxjump5:Class = Hans_Sfxjump5;
      
      private static var Sfxwalk1:Class = Hans_Sfxwalk1;
      
      private static var Sfxwalk2:Class = Hans_Sfxwalk2;
      
      private static var Sfxwalk3:Class = Hans_Sfxwalk3;
      
      private static var Sfxwalk4:Class = Hans_Sfxwalk4;
      
      private static var Sfxwalk5:Class = Hans_Sfxwalk5;
      
      public var interact:Boolean;
      
      private var count:uint;
      
      private var countFlash:int;
      
      public var fading:Boolean;
      
      public var fadeIn:Boolean;
      
      public var hp:Number;
      
      private var hpDecreased:Boolean;
      
      public var timer:TTimer;
      
      public var timer2:TTimer;
      
      public var timer3:TTimer;
      
      public var jumpThrottle:int;
      
      private var jumpThrottleMax:int;
      
      public var moveEnable:Boolean;
      
      public var jumpEnable:Boolean;
      
      public var lastLevel:Boolean;
      
      private var initialScale:FlxPoint;
      
      private var born:Boolean;
      
      public var walkingSound:Boolean;
      
      public var smaller:Boolean;
      
      public var dontAnimate:Boolean;
      
      private var floortouch:Boolean;
      
      private var playfloor:Boolean;
      
      private var playjump:Boolean;
      
      private var countForJump:int;
      
      public function Hans(_X:Number, _Y:Number, _scale:FlxPoint)
      {
         super();
         this.dontAnimate = false;
         this.playfloor = false;
         this.playjump = false;
         this.walkingSound = true;
         this.born = true;
         this.x = _X;
         this.y = _Y;
         this.scale = _scale;
         this.initialScale = _scale;
         if(scale.x == 1)
         {
            loadGraphic(S_hans,true,true,16,32,false);
            addAnimation("stand",[0],12,true);
            addAnimation("run",[1,2,3,4,5,6,7,8],12,true);
            addAnimation("jump",[9],12,true);
            addAnimation("fall",[10,11],6,true);
            maxVelocity.x = 100;
         }
         else if(scale.x == 2)
         {
            loadGraphic(S_hans16,true,true,10,16,false);
            addAnimation("stand",[0],12,true);
            addAnimation("run",[1,2,3,4],6,true);
            addAnimation("jump",[6],12,true);
            addAnimation("fall",[5],6,true);
            maxVelocity.x = 110;
         }
         else if(scale.x == 4)
         {
            loadGraphic(S_hans8,true,true,5,8,false);
            addAnimation("stand",[0],12,true);
            addAnimation("run",[1,2,3,2],6,true);
            addAnimation("jump",[4],12,true);
            addAnimation("fall",[5],6,true);
            maxVelocity.x = 120;
         }
         else if(scale.x == 8)
         {
            loadGraphic(S_hans4,true,true,4,4,false);
            addAnimation("stand",[0],12,true);
            addAnimation("run",[0],4,true);
            addAnimation("jump",[1],12,true);
            addAnimation("fall",[0],6,true);
            maxVelocity.x = 130;
         }
         else if(scale.x == 16)
         {
            loadGraphic(S_hans2,true,true,1,2,false);
            addAnimation("stand",[0,1,2,3,2,1,0],6,true);
            addAnimation("run",[0,1,2,3,2,1,0],6,true);
            addAnimation("jump",[0,1,2,3,2,1,0],6,true);
            addAnimation("fall",[0,1,2,3,2,1,0],6,true);
            maxVelocity.x = 140;
         }
         else if(scale.x == 32)
         {
            loadGraphic(S_hans1,true,true,1,1,false);
            addAnimation("stand",[0,1,2,3,2,1,0],6,true);
            addAnimation("run",[0,1,2,3,2,1,0],6,true);
            addAnimation("jump",[0,1,2,3,2,1,0],6,true);
            addAnimation("fall",[0,1,2,3,2,1,0],6,true);
            maxVelocity.x = 150;
         }
         maxVelocity.y = 305;
         acceleration.y = 600;
         this.jumpThrottleMax = 12;
         this.jumpThrottle = 0;
         drag.x = maxVelocity.x * 4;
         if(scale.x == 1)
         {
            width = 8;
            height = 28;
            offset.x = 4;
            offset.y = 4;
         }
         else if(scale.x == 2)
         {
            width = 12;
            height = 32;
            offset.x = 0;
            offset.y = -8;
         }
         else if(scale.x == 4)
         {
            width = 12;
            height = 32;
            offset.x = -4;
            offset.y = -12;
         }
         else if(scale.x == 8)
         {
            width = 16;
            height = 32;
            offset.x = -6;
            offset.y = -14;
         }
         else if(scale.x == 16)
         {
            width = 16;
            height = 32;
            centerOffsets();
         }
         else if(scale.x == 32)
         {
            width = 32;
            height = 32;
            offset.x = -17;
            offset.y = -15;
         }
         this.interact = false;
         this.fading = false;
         this.fadeIn = false;
         this.hp = 2;
         this.hpDecreased = false;
         this.moveEnable = true;
         this.jumpEnable = true;
         this.lastLevel = false;
         this.timer = new TTimer();
         this.timer2 = new TTimer();
         this.timer3 = new TTimer();
         this.floortouch = true;
         this.countForJump = 10;
      }
      
      override public function update() : void
      {
         super.update();
         if(this.playfloor && this.walkingSound && this.floortouch && isTouching(FlxObject.FLOOR))
         {
            this.playfloor = false;
            if(scale.x == 1)
            {
               FlxG.play(Sfxfloor1);
            }
            else if(scale.x == 2)
            {
               FlxG.play(Sfxfloor2);
            }
            else if(scale.x == 4)
            {
               FlxG.play(Sfxfloor3);
            }
            else if(scale.x == 8)
            {
               FlxG.play(Sfxfloor4);
            }
            else if(scale.x == 16)
            {
               FlxG.play(Sfxfloor5);
            }
         }
         if(this.countForJump < 10)
         {
            ++this.countForJump;
         }
         if(this.walkingSound && this.playjump && velocity.y < 0 && this.countForJump == 10)
         {
            this.playjump = false;
            this.countForJump = 0;
            if(scale.x == 1)
            {
               FlxG.play(Sfxjump1);
            }
            else if(scale.x == 2)
            {
               FlxG.play(Sfxjump2);
            }
            else if(scale.x == 4)
            {
               FlxG.play(Sfxjump3);
            }
            else if(scale.x == 8)
            {
               FlxG.play(Sfxjump4);
            }
            else if(scale.x == 16)
            {
               FlxG.play(Sfxjump5);
            }
         }
         if(this.walkingSound && !this.floortouch && isTouching(FlxObject.FLOOR))
         {
            this.playfloor = true;
         }
         this.floortouch = isTouching(FlxObject.FLOOR);
         if(this.walkingSound && !this.playfloor && !this.playjump)
         {
            if(isTouching(FlxObject.FLOOR) && (FlxG.keys.LEFT || FlxG.keys.A || FlxG.keys.RIGHT || FlxG.keys.D) && velocity.x != 0)
            {
               if(this.count == 10)
               {
                  if(scale.x == 1)
                  {
                     FlxG.play(Sfxwalk1);
                  }
               }
               if(this.count == 0)
               {
                  if(scale.x == 2)
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
         if(this.lastLevel)
         {
            if(this.born && scale.x > 1)
            {
               maxVelocity.x = 100;
            }
            if(scale.x > 1)
            {
               scale.x -= (this.initialScale.x - 1) * 0.05;
               scale.y = scale.x;
            }
            else
            {
               scale = new FlxPoint(1,1);
               this.initialScale = scale;
            }
         }
         acceleration.x = 0;
         if((FlxG.keys.LEFT || FlxG.keys.A) && this.moveEnable)
         {
            facing = FlxObject.LEFT;
            acceleration.x = -maxVelocity.x * 4;
         }
         if((FlxG.keys.RIGHT || FlxG.keys.D) && this.moveEnable)
         {
            facing = FlxObject.RIGHT;
            acceleration.x = maxVelocity.x * 4;
         }
         if(isTouching(FlxObject.FLOOR))
         {
            this.jumpThrottle = 0;
            if((FlxG.keys.UP || FlxG.keys.W) && this.moveEnable && this.jumpEnable)
            {
               velocity.y = -maxVelocity.y * 0.4;
               if(this.walkingSound)
               {
                  this.playjump = true;
               }
            }
         }
         if((FlxG.keys.UP || FlxG.keys.W) && this.jumpThrottle < this.jumpThrottleMax && this.moveEnable && this.jumpEnable && velocity.y < 0)
         {
            ++this.jumpThrottle;
            velocity.y -= maxVelocity.y * 0.043;
         }
         if(FlxG.keys.justPressed("SPACE") && this.moveEnable)
         {
            this.interact = true;
         }
         else
         {
            this.interact = false;
         }
         if(!this.dontAnimate)
         {
            if(velocity.y < 0)
            {
               play("jump");
            }
            else if(velocity.y > 0)
            {
               play("fall");
            }
            else if(velocity.x == 0)
            {
               play("stand");
            }
            else
            {
               play("run");
            }
         }
         if(this.fading && alpha > 0)
         {
            alpha -= 0.02;
         }
         if(this.born)
         {
            if(alpha < 1)
            {
               alpha += 0.02;
            }
            else
            {
               this.born = false;
            }
         }
         if(this.fadeIn)
         {
            if(alpha == 1)
            {
               alpha = 0;
            }
            else if(alpha < 0.98)
            {
               alpha += 0.02;
            }
            else
            {
               alpha = 1;
               this.fadeIn = false;
            }
         }
         if(this.timer.complete)
         {
            this.getVulnerable();
         }
         if(this.timer2.complete)
         {
            this.invulnerable();
         }
         if(this.timer3.complete)
         {
            this.getWell();
         }
      }
      
      public function getHurt() : void
      {
         if(!this.hpDecreased)
         {
            if(this.hp == 2)
            {
               if(scale.x == 1)
               {
                  FlxG.play(Sfxhurt1);
               }
               else if(scale.x == 2)
               {
                  FlxG.play(Sfxhurt2);
               }
               else if(scale.x == 4)
               {
                  FlxG.play(Sfxhurt3);
               }
               else if(scale.x == 8)
               {
                  FlxG.play(Sfxhurt4);
               }
               else if(scale.x == 16)
               {
                  FlxG.play(Sfxhurt5);
               }
            }
            if(this.hp > 0)
            {
               --this.hp;
            }
            this.hpDecreased = true;
            this.timer.start(120);
            this.timer2.start(6);
            this.countFlash = 20;
         }
         if(this.hp == 0)
         {
            this.fading = true;
            this.moveEnable = false;
         }
      }
      
      private function getVulnerable() : void
      {
         this.hpDecreased = false;
         this.timer3.start(120);
      }
      
      private function getWell() : void
      {
         this.hp = 2;
      }
      
      private function invulnerable() : void
      {
         if(this.countFlash > 0)
         {
            if(!this.fading)
            {
               if(alpha == 0)
               {
                  alpha = 1;
               }
               else
               {
                  alpha = 0;
               }
            }
            --this.countFlash;
            this.timer2.start(6);
         }
      }
      
      public function gokyuzundenDus() : void
      {
         loadGraphic(S_dusme,true,true,32,32,false);
         width = 8;
         height = 28;
         offset.x = 12;
         offset.y = 4;
         addAnimation("fall0",[0,1],6,true);
         addAnimation("fall1",[2,3],6,true);
         addAnimation("fall2",[4,5],6,true);
         addAnimation("fall3",[6,7],6,true);
         this.dontAnimate = true;
      }
      
      public function fall0() : void
      {
         play("fall0");
      }
      
      public function fall1() : void
      {
         play("fall1");
      }
      
      public function fall2() : void
      {
         play("fall2");
      }
      
      public function fall3() : void
      {
         play("fall3");
      }
   }
}


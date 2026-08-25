package
{
   import org.flixel.FlxG;
   import org.flixel.FlxSprite;
   
   public class Naz extends FlxSprite
   {
      
      private static var naz:Class = Naz_naz;
      
      private static var talha:Class = Naz_talha;
      
      private static var talhatekme:Class = Naz_talhatekme;
      
      private static var naztekme:Class = Naz_naztekme;
      
      private static var naztekmelen:Class = Naz_naztekmelen;
      
      private static var talhatekmelen:Class = Naz_talhatekmelen;
      
      public const SPEED:Number = 120;
      
      public var movable:Boolean = true;
      
      public var jumpThrottle:int = 0;
      
      public var jumpThrottleMax:int = 14;
      
      public var moveCount:int = 0;
      
      public var tekmeCount:int = -1;
      
      public var tekmeAtiyor:Boolean;
      
      public var tekmeAtti:Boolean;
      
      public var kapida:Boolean;
      
      public var kapidaTek:Boolean;
      
      public var sonBolum:Boolean;
      
      public var isNaz:Boolean;
      
      public var sesCaldi:Boolean;
      
      public var sesCount:int = -1;
      
      public function Naz(byNaz:Boolean, _X:Number, _Y:Number)
      {
         super();
         x = _X;
         y = _Y;
         maxVelocity.y = 490;
         maxVelocity.x = this.SPEED;
         this.isNaz = byNaz;
         if(this.isNaz)
         {
            loadGraphic(naz,true,true,32,32,true);
         }
         else
         {
            loadGraphic(talha,true,true,32,32,true);
         }
         width = 16;
         height = 30;
         offset.y = 2;
         offset.x = 8;
         addAnimation("idle",[0,0,0,0,0,0,0,0,1,1,1,1,2,1,1,1,2,3,3,3,3],6,true);
         addAnimation("walk",[4,5,6,7],6,true);
         addAnimation("up",[10,11],3,true);
         addAnimation("down",[12,13],3,true);
         addAnimation("tekme1",[8],2,true);
         addAnimation("tekme2",[9],2,true);
         addAnimation("op",[14,15],1,false);
         addAnimation("havada",[16,17,18,19],6,true);
         addAnimation("otur1",[20,20,20,20,20,20,20,20,21,21,21,21,22,21,21,21,22,23,23,23,23],6,true);
         addAnimation("otur2",[20,20,21,21,21,21,22,21,21,21,22,23,23,23,23,20,20,20,20,20,20],6,true);
         play("idle");
      }
      
      override public function update() : void
      {
         if(!this.sonBolum)
         {
            super.update();
            if(this.movable)
            {
               acceleration.y = 700;
               drag.x = maxVelocity.x * 4;
            }
            else if(!this.kapidaTek)
            {
               acceleration.y = 0;
               drag.x = 0;
               if(this.moveCount > 0)
               {
                  --this.moveCount;
               }
               else if(this.moveCount == 0)
               {
                  this.movable = true;
                  this.tekmeAtiyor = false;
               }
               if(this.moveCount == -1 && velocity.x * velocity.x + velocity.y * velocity.y < this.SPEED)
               {
                  this.movable = true;
                  this.moveCount = 0;
               }
            }
            else
            {
               acceleration.y = 700;
               drag.x = maxVelocity.x * 4;
               velocity.x = 0;
            }
            if(this.tekmeCount > 0)
            {
               --this.tekmeCount;
            }
            else if(this.tekmeCount == 0)
            {
               this.tekmeAtiyor = true;
               this.tekmeAtti = true;
               play("tekme2");
               this.tekmeCount = -1;
               if(this.isNaz)
               {
                  FlxG.play(naztekme,1,false,true);
               }
               else
               {
                  FlxG.play(talhatekme,1,false,true);
               }
            }
            if(this.sesCaldi)
            {
               if(this.sesCount == 0)
               {
                  this.sesCaldi = false;
               }
               --this.sesCount;
            }
         }
      }
      
      public function makeImmovable(count:int) : void
      {
         this.moveCount = count;
         this.movable = false;
      }
      
      public function tekme() : void
      {
         this.tekmeCount = 15;
         this.makeImmovable(30);
         velocity.x = 0;
         velocity.y = 0;
         acceleration.x = 0;
         play("tekme1");
      }
      
      public function tekmelen(direction:String) : void
      {
         velocity.x = 0;
         velocity.y = 0;
         acceleration.x = 0;
         if(!this.sesCaldi && !(direction == "left" && velocity.x == -this.SPEED || direction == "right" && velocity.x == this.SPEED || direction == "up" && velocity.y == -this.SPEED || direction == "down" && velocity.y == this.SPEED))
         {
            if(this.isNaz)
            {
               FlxG.play(naztekmelen,1,false);
            }
            else
            {
               FlxG.play(talhatekmelen,1,false);
            }
            this.sesCaldi = true;
            this.sesCount = 20;
         }
         if(direction == "left")
         {
            velocity.x = -this.SPEED;
         }
         else if(direction == "right")
         {
            velocity.x = this.SPEED;
         }
         else if(direction == "up")
         {
            velocity.y = -this.SPEED;
         }
         else if(direction == "down")
         {
            velocity.y = this.SPEED;
         }
         play("havada");
         this.makeImmovable(-1);
      }
      
      public function op() : void
      {
         play("op");
         velocity.x = 0;
         acceleration.x = 0;
         velocity.y = 0;
      }
   }
}


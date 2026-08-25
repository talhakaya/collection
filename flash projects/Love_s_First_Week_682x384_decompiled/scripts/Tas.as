package
{
   import org.flixel.FlxG;
   import org.flixel.FlxSprite;
   
   public class Tas extends FlxSprite
   {
      
      private static var tas:Class = Tas_tas;
      
      private static var ses:Class = Tas_ses;
      
      public const SPEED:Number = 300;
      
      public var nazCount:int = 1;
      
      public var move:Boolean;
      
      public var sesCaldi:Boolean;
      
      public var sesCount:int = -1;
      
      public function Tas(_X:Number, _Y:Number)
      {
         super();
         x = _X;
         y = _Y;
         immovable = true;
         this.move = false;
         drag.x = 0;
         maxVelocity.y = 500;
         maxVelocity.x = 500;
         loadGraphic(tas,false,false,32,32,false);
         width = 24;
         height = 28;
         offset.x = 4;
         offset.y = 4;
      }
      
      override public function update() : void
      {
         super.update();
         if(!immovable && velocity.x * velocity.x + velocity.y * velocity.y < this.SPEED * this.SPEED / 4 && !this.move)
         {
            velocity.x = 0;
            acceleration.y = 600;
            velocity.y = 20;
            this.move = true;
         }
         else if(!immovable && velocity.x * velocity.x + velocity.y * velocity.y < this.SPEED && isTouching(FLOOR))
         {
            this.makeImmovable();
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
      
      public function tekmelen(direction:String) : void
      {
         if(!this.sesCaldi && !(direction == "left" && velocity.x == -this.SPEED || direction == "right" && velocity.x == this.SPEED || direction == "up" && velocity.y == -this.SPEED || direction == "down" && velocity.y == this.SPEED))
         {
            FlxG.play(ses,1,false);
            this.sesCaldi = true;
            this.sesCount = 20;
         }
         velocity.x = 0;
         velocity.y = 0;
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
         immovable = false;
      }
      
      public function makeImmovable() : void
      {
         immovable = true;
         this.move = false;
         velocity.x = 0;
         velocity.y = 0;
         acceleration.y = 0;
      }
   }
}


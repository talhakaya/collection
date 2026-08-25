package
{
   import org.flixel.FlxG;
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   
   public class Block extends FlxSprite
   {
      
      private static var S_block:Class = Block_S_block;
      
      private static var Sfx:Class = Block_Sfx;
      
      private static var S_blockkucuk:Class = Block_S_blockkucuk;
      
      protected var isOpen:Boolean;
      
      public var initialX:Number;
      
      private var initialY:Number;
      
      private var slowlyLower:Boolean;
      
      private var slowlyLowerAtTheMoment:Boolean;
      
      private var lever:Lever;
      
      private var onUse:Boolean;
      
      private var leverIsOpen:Boolean;
      
      private var sfxPlayedGoingDown:Boolean;
      
      private var sfxPlayedGoingUp:Boolean;
      
      public function Block(_X:Number, _Y:Number, _scale:FlxPoint, _color:uint, _slow:Boolean, _lever:Lever, _small:Boolean)
      {
         super();
         x = _X;
         y = _Y;
         this.initialX = _X;
         this.initialY = _Y;
         this.lever = _lever;
         scale = _scale;
         if(_small)
         {
            loadGraphic(S_blockkucuk,true,false,4,64,false);
            width = 4 * scale.x;
            height = 64 * scale.x;
         }
         else
         {
            loadGraphic(S_block,true,false,16,128,false);
            width = 16 * scale.x;
            height = 128 * scale.x;
         }
         centerOffsets();
         addAnimation("red",[0],1,true);
         addAnimation("green",[1],1,true);
         addAnimation("blue",[2],1,true);
         if(_color == 1)
         {
            play("red");
         }
         else if(_color == 2)
         {
            play("green");
         }
         else
         {
            play("blue");
         }
         this.isOpen = false;
         this.onUse = false;
         this.leverIsOpen = false;
         this.slowlyLowerAtTheMoment = false;
         this.slowlyLower = _slow;
         immovable = true;
         this.sfxPlayedGoingDown = false;
         this.sfxPlayedGoingUp = false;
      }
      
      override public function update() : void
      {
         super.update();
         if(!this.sfxPlayedGoingUp && !this.isOpen && this.lever.isOpen)
         {
            FlxG.play(Sfx);
            this.sfxPlayedGoingDown = false;
            this.sfxPlayedGoingUp = true;
         }
         else if(!this.sfxPlayedGoingDown && velocity.y == 160)
         {
            FlxG.play(Sfx);
            this.sfxPlayedGoingDown = true;
            this.sfxPlayedGoingUp = false;
         }
         else if(!this.lever.isOpen)
         {
            this.sfxPlayedGoingUp = false;
         }
         this.isOpen = this.lever.isOpen;
         if(this.slowlyLower && this.isOpen && !this.onUse)
         {
            if(y < this.initialY)
            {
               velocity.y = 8;
               this.slowlyLowerAtTheMoment = true;
            }
            else if(velocity.y == 8)
            {
               this.slowlyLowerAtTheMoment = false;
               velocity.y = 0;
               y = this.initialY;
               this.isOpen = false;
               this.lever.kill();
            }
         }
         if(!this.onUse)
         {
            if(this.isOpen && y >= this.initialY)
            {
               velocity.y = -160;
               this.onUse = true;
            }
            else if(!this.isOpen && (y < this.initialY || this.slowlyLowerAtTheMoment))
            {
               this.slowlyLowerAtTheMoment = false;
               velocity.y = 160;
               this.onUse = true;
            }
            else if(!this.slowlyLower)
            {
               velocity.y = 0;
            }
         }
         else if(y >= this.initialY && !this.isOpen)
         {
            velocity.y = 0;
            y = this.initialY;
            this.onUse = false;
            this.isOpen = true;
         }
         else if(y <= this.initialY - 96 && this.isOpen)
         {
            velocity.y = 0;
            y = this.initialY - 96;
            this.onUse = false;
            this.isOpen = false;
         }
         if(this.onUse && velocity.y == 0)
         {
            this.onUse = false;
         }
      }
      
      private function restartPosition() : void
      {
         velocity.y = 0;
         acceleration.y = 0;
         y = this.initialY;
         this.onUse = false;
         this.isOpen = false;
         this.leverIsOpen = false;
         this.slowlyLowerAtTheMoment = false;
      }
   }
}


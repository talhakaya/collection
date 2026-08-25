package
{
   import org.flixel.FlxG;
   import org.flixel.FlxObject;
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   
   public class BlockDead extends FlxSprite
   {
      
      private static var S_duz:Class = BlockDead_S_duz;
      
      private static var S_yan:Class = BlockDead_S_yan;
      
      private static var Sfx:Class = BlockDead_Sfx;
      
      public var touched:Boolean;
      
      public var count:int;
      
      public var usesLever:Boolean;
      
      public var lever:Lever2;
      
      private var leverIsOpen:Boolean;
      
      private var initialY:Number;
      
      public var rising:Boolean;
      
      public function BlockDead(_X:Number, _Y:Number, _scale:FlxPoint, _yan:Boolean, _small:Boolean, _usesLever:Boolean)
      {
         super();
         this.count = 0;
         this.touched = false;
         x = _X;
         y = _Y;
         this.initialY = _Y;
         scale = _scale;
         this.usesLever = _usesLever;
         this.rising = false;
         var _color:uint = Math.floor(1 + 3 * Math.random());
         if(_small)
         {
            if(_yan)
            {
               loadGraphic(S_yan,true,false,12,4,false);
               width = 12 * scale.x;
               height = 4 * scale.x;
            }
            else
            {
               loadGraphic(S_duz,true,false,4,12,false);
               width = 4 * scale.x;
               height = 12 * scale.x;
            }
            centerOffsets();
         }
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
         immovable = true;
      }
      
      override public function update() : void
      {
         super.update();
         if(this.count < 30 && this.touched)
         {
            ++this.count;
         }
         if(this.count == 30)
         {
            acceleration.y = 200;
            ++this.count;
         }
         if(this.leverIsOpen != this.lever.isOpen)
         {
            this.rising = true;
         }
         this.leverIsOpen = this.lever.isOpen;
         if(this.rising)
         {
            if(acceleration.y > 0)
            {
               acceleration.y = 0;
            }
            else if(acceleration.y == 0)
            {
               if(y > this.initialY)
               {
                  velocity.y = -500;
               }
               else
               {
                  this.count = 0;
                  velocity.y = 0;
                  y = this.initialY;
                  this.rising = false;
                  this.touched = false;
                  FlxG.play(Sfx);
               }
            }
         }
         if(y > this.initialY + 720)
         {
            y = this.initialY + 720;
         }
         if(isTouching(FlxObject.FLOOR))
         {
            velocity.y = 0;
         }
      }
      
      override public function kill() : void
      {
         this.touched = true;
      }
   }
}


package
{
   import flash.display.Sprite;
   import flash.events.Event;
   import flash.geom.Point;
   
   public class ParticleEffect extends Sprite
   {
      
      public var force:Number;
      
      public var color:uint;
      
      public var direction:Point;
      
      public var isThereGravity:Boolean;
      
      public const constToDeath:int = 10;
      
      public var countDownToDeath:int = 10;
      
      public function ParticleEffect(_x:Number, _y:Number, _force:Number, _color:uint, _direction:Point, _isThereGravity:Boolean)
      {
         super();
         x = _x;
         y = _y;
         this.force = _force;
         this.color = _color;
         this.direction = _direction;
         this.isThereGravity = _isThereGravity;
         addEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
         this.directionNormaling();
      }
      
      public function enterFrameHandler(e:Event) : void
      {
         graphics.clear();
         --this.countDownToDeath;
         if((this.countDownToDeath <= 0 || Game.STATE == 0) && parent != null)
         {
            removeEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
            parent.removeChild(this);
         }
         else
         {
            graphics.beginFill(this.color,0.5 + Math.random() * 0.5);
            graphics.drawRect(0,0,4,4);
            graphics.endFill();
            x += this.direction.x * this.force * this.countDownToDeath / this.constToDeath;
            y += this.direction.y * this.force * this.countDownToDeath / this.constToDeath;
            if(this.isThereGravity)
            {
               ++y;
            }
         }
      }
      
      public function directionNormaling() : void
      {
         var radiusOfDirection:Number = NaN;
         if(this.direction.x == 0 && this.direction.y != 0)
         {
            if(this.direction.y > 0)
            {
               this.direction.y = 1;
            }
            else
            {
               this.direction.y = -1;
            }
         }
         else if(this.direction.y == 0 && this.direction.x != 0)
         {
            if(this.direction.x > 0)
            {
               this.direction.x = 1;
            }
            else
            {
               this.direction.x = -1;
            }
         }
         else if(this.direction.y != 0 && this.direction.x != 0)
         {
            radiusOfDirection = Math.sqrt(Math.pow(this.direction.x,2) + Math.pow(this.direction.y,2));
            this.direction.x /= radiusOfDirection;
            this.direction.y /= radiusOfDirection;
         }
      }
   }
}


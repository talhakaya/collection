package
{
   import org.flixel.FlxG;
   import org.flixel.FlxObject;
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   
   public class BlockFalling extends FlxSprite
   {
      
      private static var S_duz:Class = BlockFalling_S_duz;
      
      private static var S_yan:Class = BlockFalling_S_yan;
      
      private static var Sfx:Class = BlockFalling_Sfx;
      
      public var touchedFloor:Boolean;
      
      private var surukleniyor:Boolean;
      
      private var count:int;
      
      public function BlockFalling(_X:Number, _Y:Number, _scale:FlxPoint, _yan:Boolean, _small:Boolean)
      {
         super();
         x = _X;
         y = _Y;
         scale = _scale;
         this.count = 0;
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
         acceleration.y = 600;
         drag.x = 400;
         this.touchedFloor = false;
         this.surukleniyor = false;
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
      }
      
      override public function update() : void
      {
         super.update();
         if(velocity.y > 100)
         {
            this.touchedFloor = false;
         }
         else if(velocity.y == 0)
         {
            this.touchedFloor = true;
         }
         if(isTouching(FlxObject.FLOOR) && (velocity.x > 50 || velocity.x < -50))
         {
            if(this.count == 0)
            {
               FlxG.play(Sfx);
            }
            ++this.count;
            if(this.count == 6)
            {
               this.count = 0;
            }
         }
      }
   }
}


package
{
   import org.flixel.FlxG;
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   import org.flixel.FlxTimer;
   
   public class Lightbulb extends FlxSprite
   {
      
      private static var S_:Class = Lightbulb_S_;
      
      private static var Sfx1:Class = Lightbulb_Sfx1;
      
      private static var Sfx2:Class = Lightbulb_Sfx2;
      
      private static var Sfx3:Class = Lightbulb_Sfx3;
      
      private static var Sfx4:Class = Lightbulb_Sfx4;
      
      public var lever:Lever2;
      
      public var leverIsOpen:Boolean;
      
      public var isOpen:Boolean;
      
      public var justOpened:Boolean;
      
      private var _timer:FlxTimer;
      
      private var clr:int;
      
      public function Lightbulb(_X:Number, _Y:Number, _scale:FlxPoint, _lever:Lever2, _color:int)
      {
         super();
         x = _X;
         y = _Y;
         scale = _scale;
         this.lever = _lever;
         loadGraphic(S_,true,false,48,48,false);
         this.clr = _color;
         if(_color == 1)
         {
            addAnimation("0",[0],1,false);
            addAnimation("1",[1],1,false);
         }
         else if(_color == 2)
         {
            addAnimation("0",[2],1,false);
            addAnimation("1",[3],1,false);
         }
         else if(_color == 3)
         {
            addAnimation("0",[4],1,false);
            addAnimation("1",[5],1,false);
         }
         else
         {
            addAnimation("0",[6],1,false);
            addAnimation("1",[7],1,false);
         }
         play("0");
         this.leverIsOpen = this.lever.isOpen;
         this.isOpen = false;
         this.justOpened = false;
         this._timer = new FlxTimer();
      }
      
      override public function update() : void
      {
         super.update();
         if(this.justOpened)
         {
            this.justOpened = false;
         }
         if(this.leverIsOpen != this.lever.isOpen && !this.isOpen)
         {
            this.light();
            this.justOpened = true;
         }
         this.leverIsOpen = this.lever.isOpen;
      }
      
      public function light() : void
      {
         this.isOpen = true;
         play("1");
         this._timer.start(1,1,this.close);
         if(this.clr == 1)
         {
            FlxG.play(Sfx1);
         }
         else if(this.clr == 2)
         {
            FlxG.play(Sfx2);
         }
         else if(this.clr == 3)
         {
            FlxG.play(Sfx3);
         }
         else if(this.clr == 4)
         {
            FlxG.play(Sfx4);
         }
      }
      
      private function close(_t:FlxTimer) : void
      {
         this.isOpen = false;
         play("0");
      }
   }
}


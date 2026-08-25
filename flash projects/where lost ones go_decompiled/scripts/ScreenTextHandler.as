package
{
   import flash.display.Sprite;
   import flash.events.Event;
   import flash.geom.Point;
   import flash.text.TextField;
   import flash.text.TextFormat;
   import flash.text.TextFormatAlign;
   
   public class ScreenTextHandler extends Sprite
   {
      
      public static var instance:ScreenTextHandler;
      
      public static var textFormat:TextFormat;
      
      public var Verdana:Class;
      
      public var counter:int;
      
      public var customTextField:TextField;
      
      public var titleTextField:TextField;
      
      public var titleTextFieldShadow:TextField;
      
      public var customTextFieldRadius:Number = 2;
      
      public var titleTextFieldRadius:Number = 4;
      
      public var mouseY0:Number = 0;
      
      public var mouseY1:Number = 0;
      
      public var mouseY2:Number = 0;
      
      public function ScreenTextHandler()
      {
         var textFormat2:TextFormat = null;
         this.Verdana = ScreenTextHandler_Verdana;
         super();
         instance = this;
         addEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
         this.counter = 0;
         textFormat = new TextFormat();
         textFormat.align = TextFormatAlign.CENTER;
         textFormat.color = 4278190080;
         textFormat.font = "Verdana";
         textFormat.size = 12;
         textFormat2 = new TextFormat();
         textFormat2.color = 4278190080;
         textFormat2.font = "Verdana";
         textFormat2.size = 10;
         this.customTextField = new TextField();
         this.customTextField.mouseEnabled = false;
         this.customTextField.selectable = false;
         this.customTextField.x = 0;
         this.customTextField.width = 640;
         this.customTextField.y = 180;
         this.customTextField.defaultTextFormat = textFormat;
         addChild(this.customTextField);
         this.titleTextField = new TextField();
         this.titleTextField.mouseEnabled = false;
         this.titleTextField.selectable = false;
         this.titleTextField.x = 40;
         this.titleTextField.width = 600;
         this.titleTextField.height = 240;
         this.titleTextField.y = 80;
         this.titleTextField.defaultTextFormat = textFormat2;
         addChild(this.titleTextField);
         this.titleTextFieldShadow = new TextField();
         this.titleTextFieldShadow.mouseEnabled = false;
         this.titleTextFieldShadow.selectable = false;
         this.titleTextFieldShadow.x = 40;
         this.titleTextFieldShadow.width = 600;
         this.titleTextFieldShadow.height = 240;
         this.titleTextFieldShadow.y = 80;
         this.titleTextFieldShadow.defaultTextFormat = textFormat2;
         addChild(this.titleTextFieldShadow);
      }
      
      public static function createParticle(_x:Number, _y:Number, _force:Number, _color:uint, _direction:Point, _isThereGravity:Boolean) : void
      {
         instance.addChild(new ParticleEffect(_x,_y,_force,_color,_direction,_isThereGravity));
      }
      
      public static function createRandomDirectionParticle(howMany:int, _x:Number, _y:Number, _force:Number, _color:uint, _direction:Point, _isThereGravity:Boolean) : void
      {
         for(var i:int = 0; i < howMany; i++)
         {
            instance.addChild(new ParticleEffect(_x,_y,_force * Math.random(),_color,new Point(_direction.x - 1 + 2 * Math.random(),_direction.y - 1 + 2 * Math.random()),_isThereGravity));
         }
      }
      
      public function enterFrameHandler(e:Event) : void
      {
         var playerPos:Point = null;
         var radius:Number = NaN;
         var i:int = 0;
         var randomPoint:Point = null;
         var randomX:Number = NaN;
         ++this.counter;
         if(this.counter >= 3)
         {
            this.counter = 0;
            this.customTextField.alpha = 0.2 + Math.random() * 0.4;
            this.customTextField.x = -this.customTextFieldRadius + 2 * this.customTextFieldRadius * Math.random();
            this.customTextField.y = 180 - this.customTextFieldRadius + 2 * this.customTextFieldRadius * Math.random();
            this.titleTextFieldShadow.alpha = 0.1 + Math.random() * 0.2;
            this.titleTextFieldShadow.x = this.titleTextField.x - this.titleTextFieldRadius + 2 * this.titleTextFieldRadius * Math.random();
            this.titleTextFieldShadow.y = this.titleTextField.y - this.titleTextFieldRadius + 2 * this.titleTextFieldRadius * Math.random();
         }
         graphics.clear();
         if(Game.instance != null && Game.instance.player != null && Game.instance.player.isThereBlood)
         {
            playerPos = Game.instance.player.globalPosition();
            radius = 5 + Math.random() * 35;
            for(i = 0; i < 5; i++)
            {
               randomPoint = new Point(playerPos.x - radius + 2 * radius * Math.random(),playerPos.y - radius + 2 * radius * Math.random());
               graphics.lineStyle(0.5 + Math.random() * 4.5,4289335569,0.5 + Math.random() * 0.5,true);
               graphics.moveTo(randomPoint.x,randomPoint.y);
               graphics.lineTo(2 * playerPos.x - randomPoint.x + 10 * Math.random(),2 * playerPos.y - randomPoint.y + 10 * Math.random());
            }
            ScreenTextHandler.createRandomDirectionParticle(5,playerPos.x,playerPos.y,20,4289339938,new Point(-1 + 2 * Math.random(),-1 + 2 * Math.random()),true);
            Game.instance.player.isThereBlood = false;
         }
         if(stage != null && SeaState4.instance.isActive && !SeaState4.instance.isBlood)
         {
            this.mouseY2 = this.mouseY1;
            this.mouseY1 = this.mouseY0;
            if(SeaState4.instance.mouseYDifference > 0)
            {
               this.mouseY0 = SeaState4.instance.mouseYDifference;
            }
            else
            {
               this.mouseY0 = 0;
            }
            for(i = 0; i < 5; i++)
            {
               randomX = stage.mouseX - 10 + 20 * Math.random();
               graphics.lineStyle(0.5 + Math.random() * 2.5,2868903935,0.2 + Math.random() * 0.7,true);
               graphics.moveTo(randomX,stage.mouseY - (this.mouseY0 + this.mouseY1 + this.mouseY2));
               graphics.lineTo(randomX - 1 + 2 * Math.random(),stage.mouseY);
            }
         }
      }
      
      public function makeTitleTextFieldVisible(text:String) : void
      {
         this.titleTextField.text = text;
         this.titleTextFieldShadow.text = text;
      }
      
      public function makeTitleTextFieldInvisible() : void
      {
         this.titleTextField.text = "";
         this.titleTextFieldShadow.text = "";
      }
      
      public function makeCustomTextFieldVisible(text:String) : void
      {
         this.customTextField.text = text;
      }
      
      public function makeCustomTextFieldInvisible() : void
      {
         this.customTextField.text = "";
      }
   }
}


package
{
   import flash.display.Sprite;
   import flash.events.Event;
   import flash.events.TimerEvent;
   import flash.text.TextField;
   import flash.text.TextFormat;
   import flash.text.TextFormatAlign;
   import flash.utils.Timer;
   
   public class MessageBox extends Sprite
   {
      
      public static var instance:MessageBox;
      
      public static var textFormat:TextFormat;
      
      public var Verdana:Class = MessageBox_Verdana;
      
      public var message:String;
      
      public var textField:TextField;
      
      public var textFieldShadow:TextField;
      
      public var textFieldShadowRadius:Number = 5;
      
      public var counter:int;
      
      public var timer:Timer;
      
      public function MessageBox()
      {
         super();
         instance = this;
         addEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
         alpha = 0;
         this.counter = 0;
         graphics.beginFill(4278190080,0.8);
         graphics.drawRect(0,0,640,360);
         graphics.endFill();
         textFormat = new TextFormat();
         textFormat.align = TextFormatAlign.CENTER;
         textFormat.color = 4294967295;
         textFormat.font = "Verdana";
         textFormat.size = 12;
         this.textField = new TextField();
         this.textField.mouseEnabled = false;
         this.textField.selectable = false;
         this.textField.x = 0;
         this.textField.width = 640;
         this.textField.y = 170;
         this.textField.defaultTextFormat = textFormat;
         addChild(this.textField);
         this.textFieldShadow = new TextField();
         this.textFieldShadow.mouseEnabled = false;
         this.textFieldShadow.selectable = false;
         this.textFieldShadow.x = 0;
         this.textFieldShadow.width = 640;
         this.textFieldShadow.y = 170;
         this.textFieldShadow.defaultTextFormat = textFormat;
         this.textFieldShadow.alpha = 0.3;
         addChild(this.textFieldShadow);
         this.timer = new Timer(3000 * Game.debugTimerConst,1);
         this.timer.stop();
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandler);
      }
      
      public function enterFrameHandler(e:Event) : void
      {
         ++this.counter;
         if(this.counter >= 5)
         {
            this.counter = 0;
            this.textField.alpha = 0.6 + Math.random() * 0.4;
            this.textFieldShadow.x = this.textField.x - this.textFieldShadowRadius + 2 * this.textFieldShadowRadius * Math.random();
            this.textFieldShadow.y = this.textField.y - this.textFieldShadowRadius + 2 * this.textFieldShadowRadius * Math.random();
         }
      }
      
      public function timerHandler(e:TimerEvent) : void
      {
         this.makeInvisible();
      }
      
      public function makeVisible(id:int) : void
      {
         this.selectMessage(id);
         this.textField.text = this.message;
         this.textFieldShadow.text = this.message;
         alpha = 1;
         this.timer.reset();
         this.timer.start();
      }
      
      public function makeInvisible() : void
      {
         this.textField.text = "";
         this.textFieldShadow.text = "";
         alpha = 0;
      }
      
      public function selectMessage(id:int) : void
      {
         switch(id)
         {
            case 1:
               this.message = "Noone knows why I\'m doing this";
               break;
            case 2:
               this.message = "Even I don\'t know why I\'m this way";
               break;
            case 3:
               this.message = "I\'m doing this only because...";
               break;
            case 4:
               this.message = "...I\'ve lost my innerpeace completely";
               break;
            case 5:
               this.message = "Every day is another torment";
               break;
            case 6:
               this.message = "There\'s no point when you know";
               break;
            case 7:
               this.message = "When you know it\'s not going to get better";
               break;
            case 8:
               this.message = "And no one will understand";
               break;
            case 9:
               this.message = "And no one will know why";
               break;
            case 10:
               this.message = "Except the ones";
               break;
            case 11:
               this.message = "The ones that die every day";
               break;
            case 12:
               this.message = "Every day die a little inside";
         }
      }
   }
}


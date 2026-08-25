package
{
   import flash.Boot;
   import flash.display.Sprite;
   import flash.text.Font;
   import flash.text.TextField;
   import flash.text.TextFormat;
   import flash.text.TextFormatAlign;
   
   public class TextTalha extends Sprite
   {
      
      public static var scaleUpConstant:int = 1000;
      
      public var text:String;
      
      public var scaleUpCounter:int;
      
      public var rotationTimeCurrent:int;
      
      public var rotationMax:Number;
      
      public var rotatingToMax:Boolean;
      
      public var rotating:Boolean;
      
      public var format:TextFormat;
      
      public var field2:TextField;
      
      public var field1:TextField;
      
      public var color1:uint;
      
      public var blinking:Boolean;
      
      public function TextTalha(param1:String = undefined, param2:uint = 16777215)
      {
         if(param1 == null)
         {
            param1 = "";
         }
         if(Boot.skip_constructor)
         {
            return;
         }
         rotatingToMax = false;
         rotationTimeCurrent = 0;
         rotationMax = 15;
         rotating = true;
         blinking = false;
         text = "";
         super();
         text = param1;
         color1 = param2;
         field1 = new TextField();
         field1.textColor = color1;
         field1.selectable = false;
         field1.embedFonts = true;
         field1.width = Main.stageWidth;
         field1.x = -Main.stageWidth / 2;
         field1.y = -15;
         field2 = new TextField();
         field2.textColor = color1;
         field2.alpha = 0.5;
         field2.selectable = false;
         field2.embedFonts = true;
         field2.width = Main.stageWidth;
         field2.x = field1.x + 2;
         field2.y = field1.y + 2;
         Font.registerFont(DefaultFont);
         format = new TextFormat("Victor\'s Pixel Font",30);
         format.color = color1;
         format.align = TextFormatAlign.CENTER;
         field1.defaultTextFormat = format;
         field2.defaultTextFormat = format;
         addChild(field1);
         addChild(field2);
         update();
      }
      
      public function update() : void
      {
         var _loc1_:Number = NaN;
         field1.text = text;
         field2.text = text;
         scaleUpCounter -= GameManager.dt;
         if(scaleUpCounter <= 0)
         {
            scaleUpCounter = 0;
            scaleX = scaleY = 1;
         }
         else
         {
            scaleX = scaleY = 1 + scaleUpCounter / 1000;
         }
         if(rotating)
         {
            if(rotatingToMax)
            {
               rotationTimeCurrent += GameManager.dt;
               if(rotationTimeCurrent >= GameManager.rhythm / 2)
               {
                  rotatingToMax = false;
               }
            }
            else
            {
               rotationTimeCurrent -= GameManager.dt;
               if(rotationTimeCurrent <= -GameManager.rhythm / 2)
               {
                  rotatingToMax = true;
               }
            }
            rotation = rotationMax * (rotationTimeCurrent / GameManager.rhythm / 2);
         }
         if(blinking)
         {
            field1.alpha = 0.4 + 0.6 * Math.random();
            field2.alpha = field1.alpha / 2;
         }
      }
      
      public function changeShadow() : void
      {
         if(Math.random() < 0.25)
         {
            field2.x = field1.x + 2;
            field2.y = field1.y + 2;
         }
         else
         {
            field2.x = field1.x - 2 + 4 * Math.random();
            field2.y = field1.y - 2 + 4 * Math.random();
         }
      }
   }
}


package
{
   import flash.Boot;
   import flash.display.Sprite;
   
   public class Button extends Sprite
   {
      
      public static var init__:Boolean;
      
      public static var buttons:Array;
      
      public static var collisionWidth:Number;
      
      public static var collisionHeight:Number;
      
      public var textField:TextTalha;
      
      public var text:String;
      
      public var mouseOn:Boolean;
      
      public function Button(param1:String = undefined)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         mouseOn = false;
         super();
         text = param1;
         textField = new TextTalha(text);
         textField.blinking = true;
         addChild(textField);
         Button.cleanButtonsArray();
         Button.buttons.push(this);
      }
      
      public static function cleanButtonsArray() : void
      {
         var _loc1_:int = 0;
         while(_loc1_ < int(Button.buttons.length))
         {
            if(Button.buttons[_loc1_] == null)
            {
               Button.buttons.splice(_loc1_,1);
               _loc1_--;
            }
            _loc1_++;
         }
      }
      
      public static function isCollidingWithAny() : Boolean
      {
         var _loc4_:int = 0;
         var _loc1_:Boolean = false;
         var _loc2_:int = 0;
         var _loc3_:int = int(Button.buttons.length);
         while(_loc2_ < _loc3_)
         {
            _loc4_ = _loc2_++;
            if(Button.buttons[_loc4_].isColliding())
            {
               _loc1_ = true;
               break;
            }
         }
         return _loc1_;
      }
      
      public function update() : void
      {
         textField.update();
      }
      
      public function mouseOverHandler() : void
      {
         textField.scaleUpCounter = int(Math.round(Math.max(textField.scaleUpCounter,250)));
      }
      
      public function mouseClickHandler() : void
      {
         textField.scaleUpCounter = 1000;
      }
      
      public function isColliding() : Boolean
      {
         return Main.STAGE.mouseX > (x - 60) * GameManager.ScaleX && Main.STAGE.mouseX < (x + 60) * GameManager.ScaleX && Main.STAGE.mouseY > (y - 20) * GameManager.ScaleY && Main.STAGE.mouseY < (y + 20) * GameManager.ScaleY;
      }
      
      public function destroy() : void
      {
         var _loc3_:int = 0;
         var _loc1_:int = 0;
         var _loc2_:int = int(Button.buttons.length);
         while(_loc1_ < _loc2_)
         {
            _loc3_ = _loc1_++;
            if(Button.buttons[_loc3_] == this)
            {
               Button.buttons[_loc3_] = null;
               Button.cleanButtonsArray();
               break;
            }
         }
      }
   }
}


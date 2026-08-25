package
{
   import flash.Boot;
   import flash.display.BitmapData;
   import flash.display.Sprite;
   import motion.Actuate;
   import openfl.Assets;
   
   public class Body extends Sprite
   {
      
      public static var init__:Boolean;
      
      public static var PenisHeightMax:Number;
      
      public var shakingPenisCounter:int;
      
      public var shakingHeadCounter:int;
      
      public var shakingHandCounter:int;
      
      public var shakeFactor:Number;
      
      public var penisHeight:Number;
      
      public var penis:Citmap;
      
      public var oldKeyDown:Boolean;
      
      public var headRotationTimeCurrent:int;
      
      public var headRotationMax:Number;
      
      public var headRotatingToMax:Boolean;
      
      public var headRotating:Boolean;
      
      public var head:Citmap;
      
      public var hand:Citmap;
      
      public var endOfSong:Number;
      
      public var YOffset:Number;
      
      public var XOffset:Number;
      
      public function Body(param1:int = 0, param2:Number = 0)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         oldKeyDown = false;
         shakeFactor = 2;
         shakingHeadCounter = 0;
         shakingHandCounter = 0;
         shakingPenisCounter = 0;
         headRotatingToMax = false;
         headRotationTimeCurrent = 0;
         headRotationMax = 15;
         headRotating = true;
         penisHeight = 36;
         YOffset = Main.stageHeight;
         XOffset = Main.stageWidth / 2;
         super();
         endOfSong = param2;
         penis = new Citmap(Assets.getBitmapData("img/scene" + param1 + "/penis.png"),8,72);
         hand = new Citmap(Assets.getBitmapData("img/scene" + param1 + "/hand.png"),64,90);
         head = new Citmap(Assets.getBitmapData("img/scene" + param1 + "/head.png"),45,90);
         penis.scaleX = penis.scaleY = Number(hand.scaleX = Number(hand.scaleY = Number(head.scaleX = Number(head.scaleY = 4))));
         head.x = 45 * head.scaleX - XOffset;
         hand.x = Main.stageWidth - XOffset;
         penis.x = Main.stageWidth - 32 * hand.scaleX - XOffset;
         hand.y = Main.stageHeight + 72 * hand.scaleY + shakeFactor - YOffset;
         addChild(penis);
         addChild(head);
         addChild(hand);
      }
      
      public function update() : void
      {
         if(!oldKeyDown && GameManager.getKeyDown())
         {
            shakingPenisCounter = 500;
            shakingHeadCounter = 500;
            Actuate.stop(hand);
            Actuate.tween(hand,1,{
               "y":Main.stageHeight + penisHeight * hand.scaleY - YOffset,
               "x":Main.stageWidth + 3 * hand.scaleX * (-1 + 2 * Math.random()) - XOffset
            });
         }
         else if(oldKeyDown && !GameManager.getKeyDown())
         {
            Actuate.stop(hand);
            Actuate.tween(hand,1,{
               "y":Main.stageHeight + 72 * hand.scaleY - YOffset,
               "x":Main.stageWidth + 3 * hand.scaleX * (-1 + 2 * Math.random()) - XOffset
            });
         }
         oldKeyDown = GameManager.getKeyDown();
         penisHeight = Math.max(0,36 * (endOfSong - GameManager.time) / endOfSong);
         penis.y = Main.stageHeight + penisHeight * penis.scaleY + shakeFactor - YOffset;
         head.y = Main.stageHeight + (36 - penisHeight) * penis.scaleY / 4 + shakeFactor - YOffset;
         head.x = 45 * head.scaleX - XOffset;
         penis.x = Main.stageWidth - 32 * penis.scaleX - XOffset;
         penis.bitmap.alpha = 0.4 + 0.6 * Math.random();
         head.bitmap.alpha = 0.4 + 0.6 * Math.random();
         if(headRotating)
         {
            if(headRotatingToMax)
            {
               headRotationTimeCurrent += GameManager.dt;
               if(headRotationTimeCurrent >= GameManager.rhythm / 2)
               {
                  headRotatingToMax = false;
               }
            }
            else
            {
               headRotationTimeCurrent -= GameManager.dt;
               if(headRotationTimeCurrent <= -GameManager.rhythm / 2)
               {
                  headRotatingToMax = true;
               }
            }
            head.rotation = headRotationMax * (headRotationTimeCurrent / GameManager.rhythm / 2);
         }
         if(shakingHandCounter > 0)
         {
            shakingHandCounter -= GameManager.dt;
            _temp_1.x += -shakeFactor + 2 * shakeFactor * Math.random();
            _temp_2.y += -shakeFactor + 2 * shakeFactor * Math.random();
         }
         if(shakingHeadCounter > 0)
         {
            shakingHeadCounter -= GameManager.dt;
            _temp_3.x += -shakeFactor + 2 * shakeFactor * Math.random();
            _temp_4.y += -shakeFactor + 2 * shakeFactor * Math.random();
         }
         if(shakingPenisCounter > 0)
         {
            shakingPenisCounter -= GameManager.dt;
            _temp_5.x += -shakeFactor + 2 * shakeFactor * Math.random();
            _temp_6.y += -shakeFactor + 2 * shakeFactor * Math.random();
         }
      }
   }
}


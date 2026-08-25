package
{
   import flash.Boot;
   import flash.Lib;
   import flash.display.Sprite;
   import flash.events.Event;
   
   public class NMEPreloader extends Sprite
   {
      
      public var progress:Sprite;
      
      public var outline:Sprite;
      
      public function NMEPreloader()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         super();
         var _loc1_:int = getBackgroundColor();
         var _loc2_:int = _loc1_ >> 16 & 0xFF;
         var _loc3_:int = _loc1_ >> 8 & 0xFF;
         var _loc4_:int = _loc1_ & 0xFF;
         var _loc5_:Number = 0.299 * _loc2_ + 0.587 * _loc3_ + 0.114 * _loc4_;
         var _loc6_:int = 0;
         if(_loc5_ < 70)
         {
            _loc6_ = 16777215;
         }
         var _loc7_:int = 30;
         var _loc8_:int = 9;
         var _loc9_:Number = getHeight() / 2 - _loc8_ / 2;
         var _loc10_:Number = getWidth() - _loc7_ * 2;
         var _loc11_:int = 3;
         outline = new Sprite();
         outline.graphics.lineStyle(1,_loc6_,0.15,true);
         outline.graphics.drawRoundRect(0,0,_loc10_,_loc8_,_loc11_ * 2,_loc11_ * 2);
         outline.x = _loc7_;
         outline.y = _loc9_;
         addChild(outline);
         progress = new Sprite();
         progress.graphics.beginFill(_loc6_,0.35);
         progress.graphics.drawRect(0,0,_loc10_ - _loc11_ * 2,_loc8_ - _loc11_ * 2);
         progress.x = _loc7_ + _loc11_;
         progress.y = _loc9_ + _loc11_;
         progress.scaleX = 0;
         addChild(progress);
      }
      
      public function onUpdate(param1:int, param2:int) : void
      {
         var _loc3_:Number = param1 / param2;
         if(_loc3_ > 1)
         {
            _loc3_ == 1;
         }
         progress.scaleX = _loc3_;
      }
      
      public function onLoaded() : void
      {
         dispatchEvent(new Event(Event.COMPLETE));
      }
      
      public function onInit() : void
      {
      }
      
      public function getWidth() : Number
      {
         var _loc1_:int = 800;
         if(_loc1_ > 0)
         {
            return _loc1_;
         }
         return Lib.current.stage.stageWidth;
      }
      
      public function getHeight() : Number
      {
         var _loc1_:int = 450;
         if(_loc1_ > 0)
         {
            return _loc1_;
         }
         return Lib.current.stage.stageHeight;
      }
      
      public function getBackgroundColor() : int
      {
         return 0;
      }
   }
}


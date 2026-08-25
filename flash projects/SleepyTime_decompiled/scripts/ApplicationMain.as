package
{
   import flash.Lib;
   import flash.display.DisplayObject;
   import flash.display.LoaderInfo;
   import flash.display.StageAlign;
   import flash.display.StageScaleMode;
   import flash.events.Event;
   import flash.events.ProgressEvent;
   
   public class ApplicationMain
   {
      
      public static var complete:Boolean;
      
      public static var loaderInfo:LoaderInfo;
      
      public static var preloader:NMEPreloader;
      
      public function ApplicationMain()
      {
      }
      
      public static function main() : void
      {
         ApplicationMain.loaderInfo = Lib.current.loaderInfo;
         ApplicationMain.loaderInfo.addEventListener(Event.COMPLETE,ApplicationMain.loaderInfo_onComplete);
         ApplicationMain.loaderInfo.addEventListener(Event.INIT,ApplicationMain.loaderInfo_onInit);
         ApplicationMain.loaderInfo.addEventListener(ProgressEvent.PROGRESS,ApplicationMain.loaderInfo_onProgress);
         Lib.current.stage.align = StageAlign.TOP_LEFT;
         Lib.current.stage.scaleMode = StageScaleMode.NO_SCALE;
         ApplicationMain.preloader = new NMEPreloader();
         Lib.current.addChild(ApplicationMain.preloader);
         ApplicationMain.preloader.onInit();
         ApplicationMain.preloader.onUpdate(ApplicationMain.loaderInfo.bytesLoaded,ApplicationMain.loaderInfo.bytesTotal);
         Lib.current.addEventListener(Event.ENTER_FRAME,ApplicationMain.current_onEnter);
      }
      
      public static function start() : void
      {
         var _loc5_:* = null as String;
         var _loc6_:* = null;
         var _loc7_:* = null as DocumentClass;
         var _loc1_:Boolean = false;
         var _loc2_:Class = Type.resolveClass("Main");
         var _loc3_:int = 0;
         var _loc4_:Array = Type.getClassFields(_loc2_);
         while(_loc3_ < int(_loc4_.length))
         {
            _loc5_ = _loc4_[_loc3_];
            _loc3_++;
            if(_loc5_ == "main")
            {
               _loc1_ = true;
               break;
            }
         }
         if(_loc1_)
         {
            _loc6_ = Reflect.field(_loc2_,"main");
            _loc6_.apply(_loc2_,[]);
         }
         else
         {
            _loc7_ = Type.createInstance(DocumentClass,[]);
            if(_loc7_ is DisplayObject)
            {
               Lib.current.addChild(_loc7_);
            }
         }
      }
      
      public static function update() : void
      {
         if(ApplicationMain.preloader != null)
         {
            ApplicationMain.preloader.onUpdate(ApplicationMain.loaderInfo.bytesLoaded,ApplicationMain.loaderInfo.bytesTotal);
         }
      }
      
      public static function current_onEnter(param1:Event) : void
      {
         if(ApplicationMain.complete)
         {
            Lib.current.removeEventListener(Event.ENTER_FRAME,ApplicationMain.current_onEnter);
            ApplicationMain.loaderInfo.removeEventListener(Event.COMPLETE,ApplicationMain.loaderInfo_onComplete);
            ApplicationMain.loaderInfo.removeEventListener(Event.INIT,ApplicationMain.loaderInfo_onInit);
            ApplicationMain.loaderInfo.removeEventListener(ProgressEvent.PROGRESS,ApplicationMain.loaderInfo_onProgress);
            if(ApplicationMain.preloader != null)
            {
               ApplicationMain.preloader.addEventListener(Event.COMPLETE,ApplicationMain.preloader_onComplete);
               ApplicationMain.preloader.onLoaded();
            }
            else
            {
               ApplicationMain.start();
            }
         }
      }
      
      public static function loaderInfo_onComplete(param1:Event) : void
      {
         ApplicationMain.complete = true;
         ApplicationMain.update();
      }
      
      public static function loaderInfo_onInit(param1:Event) : void
      {
         ApplicationMain.update();
      }
      
      public static function loaderInfo_onProgress(param1:ProgressEvent) : void
      {
         ApplicationMain.update();
      }
      
      public static function preloader_onComplete(param1:Event) : void
      {
         ApplicationMain.preloader.removeEventListener(Event.COMPLETE,ApplicationMain.preloader_onComplete);
         Lib.current.removeChild(ApplicationMain.preloader);
         ApplicationMain.preloader = null;
         ApplicationMain.start();
      }
   }
}


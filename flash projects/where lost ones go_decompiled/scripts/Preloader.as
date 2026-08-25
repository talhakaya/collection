package
{
   import flash.display.DisplayObject;
   import flash.display.MovieClip;
   import flash.display.StageAlign;
   import flash.display.StageScaleMode;
   import flash.events.Event;
   import flash.events.IOErrorEvent;
   import flash.events.ProgressEvent;
   import flash.utils.getDefinitionByName;
   
   public class Preloader extends MovieClip
   {
      
      public function Preloader()
      {
         super();
         if(Boolean(stage))
         {
            stage.scaleMode = StageScaleMode.NO_SCALE;
            stage.align = StageAlign.TOP_LEFT;
         }
         addEventListener(Event.ENTER_FRAME,this.checkFrame);
         loaderInfo.addEventListener(ProgressEvent.PROGRESS,this.progress);
         loaderInfo.addEventListener(IOErrorEvent.IO_ERROR,this.ioError);
      }
      
      private function ioError(e:IOErrorEvent) : void
      {
         trace(e.text);
      }
      
      private function progress(e:ProgressEvent) : void
      {
         if(stage != null)
         {
            graphics.clear();
            graphics.beginFill(2868903935,0.8);
            graphics.drawRect(0,0,640 * stage.loaderInfo.bytesLoaded / stage.loaderInfo.bytesTotal,360);
            graphics.endFill();
         }
      }
      
      private function checkFrame(e:Event) : void
      {
         if(currentFrame == totalFrames)
         {
            stop();
            this.loadingFinished();
         }
      }
      
      private function loadingFinished() : void
      {
         removeEventListener(Event.ENTER_FRAME,this.checkFrame);
         loaderInfo.removeEventListener(ProgressEvent.PROGRESS,this.progress);
         loaderInfo.removeEventListener(IOErrorEvent.IO_ERROR,this.ioError);
         this.startup();
      }
      
      private function startup() : void
      {
         var mainClass:Class = getDefinitionByName("Main") as Class;
         addChild(new mainClass() as DisplayObject);
      }
   }
}


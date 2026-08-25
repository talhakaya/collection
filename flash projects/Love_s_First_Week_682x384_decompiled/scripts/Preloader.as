package
{
   import flash.display.DisplayObject;
   import flash.display.MovieClip;
   import flash.events.Event;
   import flash.events.MouseEvent;
   import flash.net.URLRequest;
   import flash.net.navigateToURL;
   import flash.utils.getDefinitionByName;
   
   public class Preloader extends MovieClip
   {
      
      public var loader:preloader;
      
      public var blank:blankbutton;
      
      public var mainClassName:String = "Main";
      
      private var _firstEnterFrame:Boolean;
      
      public function Preloader()
      {
         super();
         addEventListener(Event.ADDED_TO_STAGE,this.onAddedToStage);
         stop();
      }
      
      public function start() : void
      {
         this._firstEnterFrame = true;
         addEventListener(Event.ENTER_FRAME,this.onEnterFrame);
      }
      
      private function onAddedToStage(event:Event) : void
      {
         removeEventListener(Event.ADDED_TO_STAGE,this.onAddedToStage);
         this.start();
      }
      
      private function onEnterFrame(event:Event) : void
      {
         var percent:Number = NaN;
         if(this._firstEnterFrame)
         {
            this._firstEnterFrame = false;
            this.beginLoading();
            return;
         }
         if(root.loaderInfo.bytesLoaded >= root.loaderInfo.bytesTotal)
         {
            this.dispose();
            this.run();
         }
         else
         {
            percent = root.loaderInfo.bytesLoaded / root.loaderInfo.bytesTotal;
            this.updateLoading(percent);
         }
      }
      
      private function updateLoading(a_percent:Number) : void
      {
         this.loader.loadingbar.gotoAndStop(Math.floor(a_percent * 300));
      }
      
      private function beginLoading() : void
      {
         this.loader = new preloader();
         this.loader.x = 455 / 4 * 3;
         this.loader.y = 256 / 4 * 3;
         this.loader.scaleX = 91 / 80 / 4 * 3;
         this.loader.scaleY = 91 / 80 / 4 * 3;
         stage.addChild(this.loader);
         this.blank = new blankbutton();
         this.blank.buttonMode = true;
         this.blank.x = 455 / 4 * 3;
         this.blank.y = 256 / 4 * 3;
         this.blank.alpha = 0;
         this.blank.addEventListener(MouseEvent.CLICK,this.gotoSite2);
         stage.addChild(this.blank);
      }
      
      private function dispose() : void
      {
         removeEventListener(Event.ENTER_FRAME,this.onEnterFrame);
         if(Boolean(this.loader))
         {
            stage.removeChild(this.loader);
         }
         if(Boolean(this.blank))
         {
            stage.removeChild(this.blank);
         }
         this.loader = null;
         this.blank = null;
      }
      
      private function run() : void
      {
         nextFrame();
         var MainClass:Class = getDefinitionByName(this.mainClassName) as Class;
         if(MainClass == null)
         {
            throw new Error("AbstractPreloader:initialize. There was no class matching that name. Did you remember to override mainClassName?");
         }
         var main:DisplayObject = new MainClass() as DisplayObject;
         if(main == null)
         {
            throw new Error("AbstractPreloader:initialize. Main class needs to inherit from Sprite or MovieClip.");
         }
         addChildAt(main,0);
      }
      
      private function gotoSite2(e:MouseEvent) : void
      {
         navigateToURL(new URLRequest("http://www.2pg.com"),"_blank");
      }
   }
}


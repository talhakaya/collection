package preloader_fla
{
   import flash.display.MovieClip;
   import flash.events.Event;
   import flash.events.MouseEvent;
   import flash.net.URLRequest;
   import flash.net.navigateToURL;
   
   [Embed(source="/_assets/assets.swf", symbol="symbol54")]
   public dynamic class Button_logo_5 extends MovieClip
   {
      
      public var animation:MovieClip;
      
      public function Button_logo_5()
      {
         super();
         addFrameScript(1,this.frame2);
      }
      
      public function onMouseOver(param1:MouseEvent) : *
      {
         this.animation.gotoAndPlay(1);
      }
      
      public function onMouseOut(param1:MouseEvent) : *
      {
         this.animation.stop();
      }
      
      public function onClick(param1:MouseEvent) : *
      {
         navigateToURL(new URLRequest("http://www.2pg.com"),"_blank");
      }
      
      public function removeIt(param1:Event) : *
      {
         removeEventListener(MouseEvent.CLICK,this.onClick);
         removeEventListener(MouseEvent.ROLL_OVER,this.onMouseOver);
         removeEventListener(MouseEvent.ROLL_OUT,this.onMouseOut);
         removeEventListener(Event.REMOVED_FROM_STAGE,this.removeIt);
      }
      
      internal function frame2() : *
      {
         stop();
         gotoAndStop(2);
         this.animation.stop();
         buttonMode = true;
         addEventListener(MouseEvent.ROLL_OVER,this.onMouseOver);
         addEventListener(MouseEvent.ROLL_OUT,this.onMouseOut);
         addEventListener(MouseEvent.CLICK,this.onClick);
         addEventListener(Event.REMOVED_FROM_STAGE,this.removeIt);
         addEventListener(MouseEvent.CLICK,this.onClick);
      }
   }
}


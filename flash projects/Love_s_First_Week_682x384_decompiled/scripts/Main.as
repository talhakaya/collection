package
{
   import flash.display.Sprite;
   import flash.events.Event;
   import flash.events.MouseEvent;
   import flash.net.URLRequest;
   import flash.net.navigateToURL;
   
   [Frame(factoryClass="Preloader")]
   public class Main extends Sprite
   {
      
      public var intro:introanimation;
      
      public var txt:yazi;
      
      public var blank:blankbutton;
      
      public var playmoregames:playmore;
      
      public var FontNES:String = "Main_FontNES";
      
      public function Main()
      {
         super();
         if(Boolean(stage))
         {
            this.init();
         }
         else
         {
            addEventListener(Event.ADDED_TO_STAGE,this.init);
         }
      }
      
      private function init(e:Event = null) : void
      {
         removeEventListener(Event.ADDED_TO_STAGE,this.init);
         MochiBot.track(this,"550281c4");
         this.putPlayMore();
         stage.addEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
      }
      
      private function putPlayMore() : void
      {
         this.playmoregames = new playmore();
         this.playmoregames.x = 417 / 4 * 3;
         this.playmoregames.y = (256 - 80) / 4 * 3;
         this.playmoregames.scaleX = 92 / 80 / 4 * 3;
         this.playmoregames.scaleY = 92 / 80 / 4 * 3;
         this.playmoregames.moreGamesButton.addEventListener(MouseEvent.CLICK,this.gotoSite2);
         this.playmoregames.textButton.addEventListener(MouseEvent.CLICK,this.gotoSite2);
         this.playmoregames.playButton.addEventListener(MouseEvent.CLICK,this.deleteMoreGames);
         stage.addChild(this.playmoregames);
      }
      
      private function putIntro() : void
      {
         this.intro = new introanimation();
         this.intro.x = 455 / 4 * 3;
         this.intro.y = (256 - 60) / 4 * 3;
         this.intro.buttonMode = true;
         this.intro.addEventListener(MouseEvent.CLICK,this.gotoSite2);
         this.intro.scaleX = 92 / 80 / 4 * 3;
         this.intro.scaleY = 92 / 80 / 4 * 3;
         stage.addChild(this.intro);
         this.txt = new yazi();
         this.txt.buttonMode = true;
         this.txt.x = 455 / 4 * 3;
         this.txt.y = (256 + 210) / 4 * 3;
         stage.addChild(this.txt);
         this.blank = new blankbutton();
         this.blank.buttonMode = true;
         this.blank.x = 455 / 4 * 3;
         this.blank.y = 256 / 4 * 3;
         this.blank.alpha = 0;
         this.blank.addEventListener(MouseEvent.CLICK,this.gotoSite2);
         stage.addChild(this.blank);
      }
      
      private function deleteMoreGames(e:MouseEvent) : void
      {
         this.playmoregames.moreGamesButton.removeEventListener(MouseEvent.CLICK,this.gotoSite2);
         this.playmoregames.textButton.removeEventListener(MouseEvent.CLICK,this.gotoSite2);
         this.playmoregames.playButton.removeEventListener(MouseEvent.CLICK,this.deleteMoreGames);
         stage.removeChild(this.playmoregames);
         this.playmoregames = null;
         this.putIntro();
      }
      
      private function enterFrameHandler(a:Event) : void
      {
         var game:Nazire = null;
         if(this.intro != null && this.intro.addGame)
         {
            stage.removeChild(this.intro);
            stage.removeChild(this.txt);
            stage.removeChild(this.blank);
            this.intro = null;
            this.txt = null;
            this.blank = null;
            stage.removeEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
            game = new Nazire();
            stage.addChild(game);
         }
      }
      
      private function gotoSite2(e:MouseEvent) : void
      {
         navigateToURL(new URLRequest("http://www.2pg.com"),"_blank");
      }
   }
}


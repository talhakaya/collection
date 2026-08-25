package
{
   import MaxGames_fla.MaxGames_MAXLOGOINTRO1_1;
   import flash.display.Bitmap;
   import flash.display.BitmapData;
   import flash.display.MovieClip;
   import flash.display.Sprite;
   import flash.events.Event;
   import flash.events.MouseEvent;
   import flash.net.URLRequest;
   import flash.net.navigateToURL;
   
   [Frame(factoryClass="Preloader")]
   [SWF(width="640",height="480",frameRate="30",backgroundColor="#3DDAE5")]
   public class Main extends Sprite
   {
      
      protected var ImgLogoCorners:Class = Main_ImgLogoCorners;
      
      protected var ImgLogoLight:Class = Main_ImgLogoLight;
      
      private var logo:MovieClip;
      
      private var bitmap:Bitmap;
      
      private var bitmap1:Bitmap;
      
      private var _bmpBar:Bitmap;
      
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
         this.logoAdd();
      }
      
      private function logoAdd() : void
      {
         var h:Number = 480;
         var w:Number = 640;
         this.bitmap1 = new this.ImgLogoCorners();
         this.bitmap1.smoothing = true;
         this.bitmap1.width = w;
         this.bitmap1.height = h;
         addChild(this.bitmap1);
         this.bitmap = new Bitmap(new BitmapData(w,h,false,16777215));
         var i:uint = 0;
         var j:uint = 0;
         while(i < h)
         {
            j = 0;
            while(j < w)
            {
               this.bitmap.bitmapData.setPixel(j++,i + 1,0);
            }
            this.bitmap.bitmapData.setPixel(j++,i,0);
            i += 4;
         }
         this.bitmap.blendMode = "overlay";
         this.bitmap.alpha = 0.25;
         addChild(this.bitmap);
         this.logo = new MaxGames_MAXLOGOINTRO1_1();
         this.logo.addEventListener("LOGO_FINISHED",this.logoStop2);
         this.logo.addEventListener(MouseEvent.CLICK,this.goToMyURL);
         addChild(this.logo);
         this.logo.x = 63;
         this.logo.y = 180;
      }
      
      private function logoStop2(e:Event) : void
      {
         this.logo.removeEventListener("LOGO_FINISHED",this.logoStop2);
         this.logo.removeEventListener(MouseEvent.CLICK,this.goToMyURL);
         this.logo.stop();
         removeChild(this.logo);
         removeChild(this.bitmap);
         removeChild(this.bitmap1);
         this.logo = null;
         this.bitmap = null;
         this.bitmap1 = null;
         var game:AnOdeToPixelDays = new AnOdeToPixelDays();
         addChild(game);
      }
      
      private function goToMyURL(event:MouseEvent = null) : void
      {
         navigateToURL(new URLRequest("http://www.maxgames.com"));
      }
   }
}


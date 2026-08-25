package
{
   import flash.display.Bitmap;
   import flash.display.BitmapData;
   import flash.display.MovieClip;
   import flash.display.Sprite;
   import flash.events.Event;
   import flash.events.MouseEvent;
   import flash.net.URLRequest;
   import flash.net.navigateToURL;
   import flash.text.TextField;
   import flash.text.TextFormat;
   import org.flixel.system.FlxPreloader;
   
   public class Preloader extends FlxPreloader
   {
      
      private var bar:Sprite;
      
      private var h:Number = 480;
      
      private var w:Number = 640;
      
      public function Preloader()
      {
         className = "Main";
         super();
      }
      
      override protected function create() : void
      {
         _buffer = new Sprite();
         addChild(_buffer);
         var bitmap:Bitmap = new ImgLogoLight();
         bitmap.smoothing = true;
         bitmap.width = bitmap.height = this.h;
         bitmap.x = (this.w - bitmap.width) / 2;
         _buffer.addChild(bitmap);
         bitmap = new ImgLogoCorners();
         bitmap.smoothing = true;
         bitmap.width = this.w;
         bitmap.height = this.h;
         _buffer.addChild(bitmap);
         var tmp:Bitmap = new Bitmap(new BitmapData(640,480,true,16777215));
         _buffer.addChild(tmp);
         var format:TextFormat = new TextFormat();
         format.color = 0;
         format.size = 16;
         format.align = "center";
         format.bold = true;
         format.font = "system";
         var textField:TextField = new TextField();
         textField.selectable = false;
         textField.width = tmp.width - 16;
         textField.height = tmp.height - 16;
         textField.y = 168;
         textField.multiline = true;
         textField.wordWrap = true;
         textField.embedFonts = true;
         textField.defaultTextFormat = format;
         textField.text = "Presents";
         _buffer.addChild(textField);
         var format1:TextFormat = new TextFormat();
         format1.color = 11167334;
         format1.size = 40;
         format1.align = "center";
         format1.bold = true;
         format1.font = "system";
         var format2:TextFormat = new TextFormat();
         format2.color = 0;
         format2.size = 40;
         format2.align = "center";
         format2.bold = true;
         format2.font = "system";
         bitmap = new Bitmap(new BitmapData(this.w,this.h,false,16777215));
         var i:uint = 0;
         var j:uint = 0;
         while(i < this.h)
         {
            j = 0;
            while(j < this.w)
            {
               bitmap.bitmapData.setPixel(j++,i + 1,0);
            }
            bitmap.bitmapData.setPixel(j++,i,0);
            i += 4;
         }
         bitmap.blendMode = "overlay";
         bitmap.alpha = 0.25;
         _buffer.addChild(bitmap);
         var logo:MovieClip = new Logoo();
         logo.addEventListener(MouseEvent.CLICK,this.goToMyURL);
         logo.buttonMode = true;
         _buffer.addChild(logo);
         logo.x = 320;
         logo.y = 112;
         textField = new TextField();
         textField.selectable = false;
         textField.width = tmp.width - 16;
         textField.height = tmp.height - 16;
         textField.x = 4;
         textField.y = 204;
         textField.multiline = true;
         textField.wordWrap = true;
         textField.embedFonts = true;
         textField.defaultTextFormat = format2;
         textField.text = "Ode To Pixel Days";
         _buffer.addChild(textField);
         textField = new TextField();
         textField.selectable = false;
         textField.width = tmp.width - 16;
         textField.height = tmp.height - 16;
         textField.y = 200;
         textField.multiline = true;
         textField.wordWrap = true;
         textField.embedFonts = true;
         textField.defaultTextFormat = format1;
         textField.text = "Ode To Pixel Days";
         _buffer.addChild(textField);
         _bmpBar = new Bitmap(new BitmapData(1,32,false,0));
         _bmpBar.x = 2;
         _bmpBar.y = 444;
         _buffer.addChild(_bmpBar);
         var _text:TextField = new TextField();
         _text.defaultTextFormat = new TextFormat("system",16,0);
         _text.embedFonts = true;
         _text.selectable = false;
         _text.multiline = false;
         _text.x = 2;
         _text.y = _bmpBar.y - 19;
         _text.width = 800;
         _text.text = "Loading, Please Wait...";
         _buffer.addChild(_text);
         _text = new TextField();
         _text.defaultTextFormat = new TextFormat("system",16,0);
         _text.embedFonts = true;
         _text.selectable = false;
         _text.multiline = false;
         _text.x = 4;
         _text.y = _bmpBar.y + 6;
         _text.width = 612;
         _text.defaultTextFormat.align = "right";
         _text.alpha = 0.5;
         _text.text = "LoadingLoadingLoadingLoadingLoadingLoadingLoadingLoadingLoadingLoading";
         _buffer.addChild(_text);
         stage.addEventListener(Event.ENTER_FRAME,this.enterFrame);
      }
      
      override protected function update(Percent:Number) : void
      {
         _bmpBar.scaleX = root.loaderInfo.bytesLoaded / root.loaderInfo.bytesTotal * 636;
         _text.text = "" + root.loaderInfo.bytesLoaded + " / " + root.loaderInfo.bytesTotal + " Downloaded";
      }
      
      private function goToMyURL(event:MouseEvent = null) : void
      {
         navigateToURL(new URLRequest("http://www.maxgames.com"));
      }
      
      private function enterFrame(event:Event) : void
      {
         if(_bmpBar == null)
         {
            removeEventListener(Event.ENTER_FRAME,this.enterFrame);
            return;
         }
         _bmpBar.scaleX = root.loaderInfo.bytesLoaded / root.loaderInfo.bytesTotal * 636;
         _text.text = "" + root.loaderInfo.bytesLoaded + " / " + root.loaderInfo.bytesTotal + " Downloaded";
      }
   }
}


package
{
   import flash.net.URLRequest;
   import flash.net.navigateToURL;
   import org.flixel.FlxButton;
   import org.flixel.FlxG;
   
   public class logoButton extends FlxButton
   {
      
      private static var handCursor:Class = logoButton_handCursor;
      
      private static var logoInGame:Class = logoButton_logoInGame;
      
      private static var text:Class = logoButton_text;
      
      public const Y:Number = 206;
      
      public function logoButton(diyalogVar:Boolean, isLogo:Boolean)
      {
         super(0,0,"",this.goToMyURL);
         if(diyalogVar)
         {
            y = 0;
         }
         if(isLogo)
         {
            loadGraphic(logoInGame,false,false,96,50);
            width = 96;
            height = 50;
         }
         else
         {
            loadGraphic(text,false,false,74,12);
            width = 74;
            height = 12;
            x = (455 - 74) / 2;
         }
         scrollFactor.x = scrollFactor.y = 0;
         onOver = function():void
         {
            FlxG.mouse.load(handCursor,1,-15,-10);
         };
         onOut = function():void
         {
            FlxG.mouse.unload();
         };
      }
      
      public function goToMyURL() : void
      {
         navigateToURL(new URLRequest("http://www.2pg.com"));
      }
   }
}


package
{
   import flash.net.URLRequest;
   import flash.net.navigateToURL;
   import org.flixel.FlxButton;
   import org.flixel.FlxG;
   import org.flixel.FlxPoint;
   
   public class logoButton extends FlxButton
   {
      
      private static var handCursor:Class = logoButton_handCursor;
      
      private static var logoInGame:Class = logoButton_logoInGame;
      
      private static var logoInGame2:Class = logoButton_logoInGame2;
      
      private static var logoInGame4:Class = logoButton_logoInGame4;
      
      private static var logoInGame8:Class = logoButton_logoInGame8;
      
      private static var logoInGame16:Class = logoButton_logoInGame16;
      
      private var scale1:FlxPoint;
      
      public function logoButton(_scale:FlxPoint)
      {
         super(83,220,"",this.goToMyURL);
         this.scale1 = _scale;
         if(this.scale1.x == 1)
         {
            loadGraphic(logoInGame,false,false,154,20);
         }
         else if(this.scale1.x == 2)
         {
            loadGraphic(logoInGame2,false,false,154,20);
         }
         else if(this.scale1.x == 4)
         {
            loadGraphic(logoInGame4,false,false,156,20);
         }
         else if(this.scale1.x == 8)
         {
            loadGraphic(logoInGame8,false,false,133,20);
         }
         else
         {
            loadGraphic(logoInGame16,false,false,140,20);
         }
         width = 154;
         height = 20;
         alpha = 0.5;
         scrollFactor.x = scrollFactor.y = 0;
         onOver = function():void
         {
            FlxG.mouse.load(handCursor,1,-15,-10);
            alpha = 1;
         };
         onOut = function():void
         {
            FlxG.mouse.unload();
            alpha = 0.5;
         };
      }
      
      public function goToMyURL() : void
      {
         navigateToURL(new URLRequest("http://www.maxgames.com"));
      }
   }
}


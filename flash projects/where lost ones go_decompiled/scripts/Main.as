package
{
   import flash.display.Sprite;
   import flash.events.Event;
   import flash.geom.Point;
   
   [Frame(factoryClass="Preloader")]
   public class Main extends Sprite
   {
      
      public static var Foreground:foreground;
      
      public static var instance:Main;
      
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
      
      public static function distanceBetweenSprites(s1:Sprite, s2:Sprite) : Number
      {
         return Math.sqrt(Math.pow(s1.x - s2.x,2) + Math.pow(s1.y - s2.y,2));
      }
      
      public static function distanceBetweenSpritesCloserThanRadius(radius:Number, s1:Sprite, s2:Sprite) : Boolean
      {
         return radius > distanceBetweenSprites(s1,s2);
      }
      
      public static function distanceBetweenPlayerCloserThanRadius(radius:Number, s1:Sprite) : Boolean
      {
         var playerPos:Point = null;
         var bool:Boolean = false;
         if(Game.instance != null && Game.instance.player != null)
         {
            playerPos = Game.instance.player.globalPosition();
            bool = radius > Math.sqrt(Math.pow(s1.x - playerPos.x,2) + Math.pow(s1.y - playerPos.y,2));
         }
         return bool;
      }
      
      public static function distanceBetweenMouseCloserThanRadius(radius:Number, s1:Sprite) : Boolean
      {
         var bool:Boolean = false;
         if(Game.instance != null && Game.instance.player != null)
         {
            bool = radius > Math.sqrt(Math.pow(s1.x - Game.instance.stage.mouseX,2) + Math.pow(s1.y - Game.instance.stage.mouseY,2));
         }
         return bool;
      }
      
      private function init(e:Event = null) : void
      {
         removeEventListener(Event.ADDED_TO_STAGE,this.init);
         instance = this;
         Foreground = new foreground();
         addChild(new SoundManager());
         var screenTextHandler:ScreenTextHandler = new ScreenTextHandler();
         addChild(new Game());
         addChild(new SeaState4());
         addChild(screenTextHandler);
         addChild(new MessageBox());
         addChild(Foreground);
      }
   }
}


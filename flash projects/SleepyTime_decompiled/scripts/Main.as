package
{
   import flash.Boot;
   import flash.Lib;
   import flash.display.Sprite;
   import flash.display.Stage;
   import flash.display.StageAlign;
   import flash.display.StageScaleMode;
   import flash.events.Event;
   
   public class Main extends Sprite
   {
      
      public static var stageWidth:int;
      
      public static var stageHeight:int;
      
      public static var STAGE:Stage;
      
      public var inited:Boolean;
      
      public var gameManager:GameManager;
      
      public function Main()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         super();
         addEventListener(Event.ADDED_TO_STAGE,added);
      }
      
      public static function main() : void
      {
         Lib.current.stage.align = StageAlign.TOP_LEFT;
         Lib.current.stage.scaleMode = StageScaleMode.NO_SCALE;
         Lib.current.addChild(new Main());
      }
      
      public function resize(param1:*) : void
      {
         if(!inited)
         {
            init();
         }
      }
      
      public function init() : void
      {
         if(inited)
         {
            return;
         }
         inited = true;
         Main.stageWidth = stage.stageWidth;
         Main.stageHeight = stage.stageHeight;
         Main.STAGE = stage;
         gameManager = new GameManager();
         addChild(gameManager);
      }
      
      public function added(param1:*) : void
      {
         removeEventListener(Event.ADDED_TO_STAGE,added);
         stage.addEventListener(Event.RESIZE,resize);
         init();
      }
   }
}


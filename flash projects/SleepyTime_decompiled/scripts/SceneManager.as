package
{
   import flash.Boot;
   import flash.display.Sprite;
   
   public class SceneManager extends Sprite
   {
      
      public static var replayInputs:Array = [[],[],[],[],[],[]];
      
      public static var scores:Array = [0,0,0,0,0,0];
      
      public static var WHICHSONG:int = 1;
      
      public var replayInputPointers:Array;
      
      public var passiveScenes:Array;
      
      public var passiveSceneInputs:Array;
      
      public var musicStarted:Boolean;
      
      public var activeScene:Scene;
      
      public function SceneManager()
      {
         var _loc3_:int = 0;
         var _loc4_:Number = NaN;
         if(Boot.skip_constructor)
         {
            return;
         }
         musicStarted = false;
         super();
         GameManager.time = -2000;
         SoundManager.changeMusic("" + (GameManager.id + 1));
         activeScene = new Scene(GameManager.id + 1,false);
         addChild(activeScene);
         passiveScenes = [];
         passiveSceneInputs = [];
         replayInputPointers = [];
         var _loc1_:int = 0;
         var _loc2_:int = GameManager.id;
         while(_loc1_ < _loc2_)
         {
            _loc3_ = _loc1_++;
            passiveScenes.push(new Scene(_loc3_ + 1,true));
            passiveScenes[_loc3_].scaleX = passiveScenes[_loc3_].scaleY = 0.2;
            passiveScenes[_loc3_].x = Main.stageWidth * _loc3_ / GameManager.id;
            passiveScenes[_loc3_].y = Main.stageHeight * (1 - passiveScenes[_loc3_].scaleY);
            addChild(passiveScenes[_loc3_]);
            passiveSceneInputs.push(false);
            replayInputPointers.push(0);
         }
      }
      
      public function update() : void
      {
         var _loc3_:int = 0;
         var _loc4_:* = null as Array;
         activeScene.inputHandler(GameManager.getKeyDown());
         if(!musicStarted && GameManager.time >= 0)
         {
            musicStarted = true;
            SoundManager.skipToMusic(GameManager.time);
            SoundManager.playMusic();
         }
         var _loc1_:int = 0;
         var _loc2_:int = int(passiveScenes.length);
         while(_loc1_ < _loc2_)
         {
            _loc3_ = _loc1_++;
            if(int(SceneManager.replayInputs[_loc3_].length) > int(replayInputPointers[_loc3_]) && GameManager.time > Number(SceneManager.replayInputs[_loc3_][int(replayInputPointers[_loc3_])]) * GameManager.rhythm)
            {
               passiveSceneInputs[_loc3_] = !Boolean(passiveSceneInputs[_loc3_]);
               _loc4_ = replayInputPointers;
               ++_loc4_[_loc3_];
            }
            if(int(SceneManager.replayInputs[_loc3_].length) > int(replayInputPointers[_loc3_]))
            {
               passiveScenes[_loc3_].inputHandler(Boolean(passiveSceneInputs[_loc3_]));
            }
            else
            {
               passiveScenes[_loc3_].inputHandler(GameManager.getKeyDown());
            }
         }
         sceneGraphicHandler();
      }
      
      public function sceneGraphicHandler() : void
      {
         var _loc3_:int = 0;
         activeScene.updateGraphics();
         var _loc1_:int = 0;
         var _loc2_:int = int(passiveScenes.length);
         while(_loc1_ < _loc2_)
         {
            _loc3_ = _loc1_++;
            passiveScenes[_loc3_].updateGraphics();
         }
      }
   }
}


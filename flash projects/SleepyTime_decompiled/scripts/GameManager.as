package
{
   import flash.Boot;
   import flash.Lib;
   import flash.display.Bitmap;
   import flash.display.BitmapData;
   import flash.display.Sprite;
   import flash.display.StageDisplayState;
   import flash.events.Event;
   import flash.events.KeyboardEvent;
   import flash.events.MouseEvent;
   import flash.net.URLRequest;
   import flash.utils.getTimer;
   import haxe.Log;
   import motion.Actuate;
   import openfl.Assets;
   
   public class GameManager extends Sprite
   {
      
      public static var time:int;
      
      public static var dt:int;
      
      public static var keyDown:Boolean = false;
      
      public static var keyboardDown:Boolean = false;
      
      public static var mouseDown:Boolean = false;
      
      public static var mouseDownOld:Boolean = false;
      
      public static var rhythm:int = 500;
      
      public static var paused:Boolean = false;
      
      public static var id:int = 0;
      
      public static var ScaleX:Number = 1;
      
      public static var ScaleY:Number = 1;
      
      public static var TimeBeginningTheSong:int = -2000;
      
      public var scoreTable:ScoreTable;
      
      public var sceneManager:SceneManager;
      
      public var pauseButton:Button;
      
      public var menuButton:Button;
      
      public var menu:Menu;
      
      public var lastTime:int;
      
      public var f4KeyDownOld:Boolean;
      
      public var f4KeyDown:Boolean;
      
      public var escKeyDownOld:Boolean;
      
      public var escKeyDown:Boolean;
      
      public var dialogueScreen:DialogueScreen;
      
      public var currentTime:int;
      
      public function GameManager()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         f4KeyDownOld = false;
         f4KeyDown = false;
         escKeyDownOld = false;
         escKeyDown = false;
         super();
         var _loc1_:Bitmap = new Bitmap(Assets.getBitmapData("img/bg.jpg"));
         _loc1_.scaleX = Main.stageWidth / 600;
         _loc1_.scaleY = Main.stageHeight / 600;
         addChild(_loc1_);
         Main.STAGE.addEventListener(KeyboardEvent.KEY_DOWN,keyDownHandler);
         Main.STAGE.addEventListener(KeyboardEvent.KEY_UP,keyUpHandler);
         Main.STAGE.addEventListener(MouseEvent.MOUSE_DOWN,mouseDownHandler);
         Main.STAGE.addEventListener(MouseEvent.MOUSE_UP,mouseUpHandler);
         new SaveManager();
         new SoundManager();
         SoundManager.changeMusic("menu1");
         SoundManager.playMusic();
         if(SaveManager.playedBefore)
         {
            menu = new Menu();
            newScreen(menu);
         }
         else
         {
            dialogueScreen = new DialogueScreen();
            newScreen(dialogueScreen);
         }
         GameManager.time = -2000;
         lastTime = getTimer();
         addEventListener(Event.ENTER_FRAME,enterFrameHandler);
         pauseButton = new Button("Pause");
         pauseButton.x = 60;
         pauseButton.y = 20;
         addChild(pauseButton);
         menuButton = new Button("Menu");
         menuButton.x = Main.stageWidth - 60;
         menuButton.y = 20;
         addChild(menuButton);
      }
      
      public static function getKeyDown() : Boolean
      {
         GameManager.keyDown = !GameManager.paused && (!Button.isCollidingWithAny() && GameManager.mouseDown || GameManager.keyboardDown);
         return GameManager.keyDown;
      }
      
      public function switchFullScreen() : void
      {
         if(Main.STAGE.displayState == StageDisplayState.NORMAL)
         {
            Main.STAGE.displayState = StageDisplayState.FULL_SCREEN;
         }
         else
         {
            Main.STAGE.displayState = StageDisplayState.NORMAL;
         }
      }
      
      public function newScreen(param1:Sprite) : void
      {
         param1.x = Main.stageWidth;
         addChild(param1);
         Actuate.tween(param1,1,{"x":0});
         if(scoreTable != null && scoreTable.x != Main.stageWidth)
         {
            Actuate.stop(scoreTable);
            Actuate.tween(scoreTable,2,{"x":-Main.stageWidth});
         }
         if(menu != null && menu.x != Main.stageWidth)
         {
            Actuate.stop(menu);
            Actuate.tween(menu,2,{"x":-Main.stageWidth});
         }
         if(sceneManager != null && sceneManager.x != Main.stageWidth)
         {
            Actuate.stop(sceneManager);
            Actuate.tween(sceneManager,2,{"y":-Main.stageHeight * 2});
         }
         if(dialogueScreen != null && dialogueScreen.x != Main.stageWidth)
         {
            Actuate.stop(dialogueScreen);
            Actuate.tween(dialogueScreen,2,{"y":Main.stageHeight * 2});
         }
      }
      
      public function mouseUpHandler(param1:MouseEvent) : void
      {
         GameManager.mouseDown = false;
      }
      
      public function mouseDownHandler(param1:MouseEvent) : void
      {
         GameManager.mouseDown = true;
      }
      
      public function killDetails() : void
      {
         removeEventListener(KeyboardEvent.KEY_DOWN,keyDownHandler);
         removeEventListener(MouseEvent.MOUSE_DOWN,mouseDownHandler);
         removeEventListener(KeyboardEvent.KEY_UP,keyUpHandler);
         removeEventListener(MouseEvent.MOUSE_UP,mouseUpHandler);
      }
      
      public function keyUpHandler(param1:KeyboardEvent) : void
      {
         if(param1.keyCode == 27)
         {
            escKeyDown = false;
         }
         else if(param1.keyCode == 70)
         {
            f4KeyDown = false;
         }
         else
         {
            GameManager.keyboardDown = false;
         }
      }
      
      public function keyDownHandler(param1:KeyboardEvent) : void
      {
         if(param1.keyCode == 27)
         {
            escKeyDown = true;
         }
         else if(param1.keyCode == 70)
         {
            f4KeyDown = true;
         }
         else
         {
            GameManager.keyboardDown = true;
         }
      }
      
      public function garbageCollector() : void
      {
         var _loc1_:int = 0;
         var _loc2_:int = 0;
         var _loc3_:int = 0;
         if(scoreTable != null && scoreTable.x == -Main.stageWidth)
         {
            removeChild(scoreTable);
            scoreTable = null;
            _loc1_ = 0;
            _loc2_ = int(Button.buttons.length);
            while(_loc1_ < _loc2_)
            {
               _loc3_ = _loc1_++;
               if(Button.buttons[_loc3_].parent is ScoreTable)
               {
                  Button.buttons[_loc3_] = null;
               }
            }
            Button.cleanButtonsArray();
         }
         if(sceneManager != null && sceneManager.y == -Main.stageHeight * 2)
         {
            removeChild(sceneManager);
            sceneManager = null;
            _loc1_ = 0;
            _loc2_ = int(Button.buttons.length);
            while(_loc1_ < _loc2_)
            {
               _loc3_ = _loc1_++;
               if(Button.buttons[_loc3_].parent is SceneManager)
               {
                  Button.buttons[_loc3_] = null;
               }
            }
            Button.cleanButtonsArray();
         }
         if(dialogueScreen != null && dialogueScreen.y == Main.stageHeight * 2)
         {
            removeChild(dialogueScreen);
            dialogueScreen = null;
            _loc1_ = 0;
            _loc2_ = int(Button.buttons.length);
            while(_loc1_ < _loc2_)
            {
               _loc3_ = _loc1_++;
               if(Button.buttons[_loc3_].parent is DialogueScreen)
               {
                  Button.buttons[_loc3_] = null;
               }
            }
            Button.cleanButtonsArray();
         }
         if(menu != null && menu.x == -Main.stageWidth)
         {
            removeChild(menu);
            menu = null;
            _loc1_ = 0;
            _loc2_ = int(Button.buttons.length);
            while(_loc1_ < _loc2_)
            {
               _loc3_ = _loc1_++;
               if(Button.buttons[_loc3_].parent is Menu)
               {
                  Button.buttons[_loc3_] = null;
               }
            }
            Button.cleanButtonsArray();
         }
      }
      
      public function enterFrameHandler(param1:Event) : void
      {
         GameManager.ScaleX = scaleX = stage.stageWidth / 800;
         GameManager.ScaleY = scaleY = stage.stageHeight / 450;
         GameManager.ScaleX = scaleX = 1;
         GameManager.ScaleY = scaleY = 1;
         stage.focus = stage;
         calculateTime();
         buttonHandler();
         if(sceneManager != null)
         {
            sceneManager.update();
            if(sceneManager.activeScene.destroyMePlease && !sceneManager.activeScene.destroyMePleaseMessageTaken)
            {
               if(GameManager.id == 0 || GameManager.id == 3)
               {
                  SoundManager.changeMusic("menu2");
               }
               else if(GameManager.id == 1 || GameManager.id == 4)
               {
                  SoundManager.changeMusic("menu3");
               }
               else if(GameManager.id == 2)
               {
                  SoundManager.changeMusic("menu1");
               }
               if(GameManager.id != 5)
               {
                  SoundManager.pauseMusic();
                  SoundManager.playMusic();
               }
               sceneManager.activeScene.destroyMePleaseMessageTaken = true;
               scoreTable = new ScoreTable(sceneManager.activeScene.score);
               newScreen(scoreTable);
            }
         }
         if(scoreTable != null)
         {
            scoreTable.update();
         }
         if(dialogueScreen != null)
         {
            dialogueScreen.update();
            if(dialogueScreen.destroyMePlease && !dialogueScreen.destroyMePleaseMessageTaken)
            {
               dialogueScreen.destroyMePleaseMessageTaken = true;
               GameManager.time = -2000;
               if(GameManager.id == 6 || GameManager.id < 0)
               {
                  menu = new Menu();
                  newScreen(menu);
               }
               else
               {
                  sceneManager = new SceneManager();
                  newScreen(sceneManager);
               }
            }
         }
         if(menu != null)
         {
            menu.update();
         }
         SoundManager.update();
         checkOtherKeys();
         garbageCollector();
      }
      
      public function checkOtherKeys() : void
      {
         if(escKeyDown && !escKeyDownOld)
         {
         }
         if(f4KeyDown && !f4KeyDownOld)
         {
            switchFullScreen();
         }
         escKeyDownOld = escKeyDown;
         f4KeyDownOld = f4KeyDown;
      }
      
      public function calculateTime() : void
      {
         currentTime = getTimer();
         if(!GameManager.paused)
         {
            GameManager.dt = currentTime - lastTime;
            _temp_1.time += GameManager.dt;
         }
         else
         {
            GameManager.dt = 0;
         }
         lastTime = currentTime;
      }
      
      public function buttonPressHandler(param1:Button) : void
      {
         if(param1.text == "Pause")
         {
            if(GameManager.paused)
            {
               GameManager.paused = false;
               SoundManager.playMusic();
            }
            else
            {
               GameManager.paused = true;
               SoundManager.pauseMusic();
            }
         }
         else if(!GameManager.paused)
         {
            if(param1.text == "testButton")
            {
               Log.trace("DONT TOUCH ME YOU PRICK",{
                  "fileName":"GameManager.hx",
                  "lineNumber":386,
                  "className":"GameManager",
                  "methodName":"buttonPressHandler"
               });
            }
            else if(param1.text == "Retry")
            {
               if(!scoreTable.destroyMePlease)
               {
                  scoreTable.destroyMePlease = true;
                  scoreTable.destroyMePleaseMessageTaken = true;
                  sceneManager = new SceneManager();
                  newScreen(sceneManager);
               }
            }
            else if(param1.text == "Next Level" || param1.text == "Finish")
            {
               if(!scoreTable.destroyMePlease)
               {
                  scoreTable.destroyMePlease = true;
                  scoreTable.destroyMePleaseMessageTaken = true;
                  ++GameManager.id;
                  dialogueScreen = new DialogueScreen();
                  newScreen(dialogueScreen);
               }
            }
            else if(param1.text == "Menu")
            {
               if(menu == null)
               {
                  if(sceneManager != null)
                  {
                     SoundManager.pauseMusic();
                     SoundManager.changeMusic("menu1");
                     SoundManager.playMusic();
                  }
                  menu = new Menu();
                  newScreen(menu);
               }
            }
            else if(param1.text == "Twitter")
            {
               if(stage.displayState == StageDisplayState.FULL_SCREEN)
               {
                  switchFullScreen();
               }
               Lib.getURL(new URLRequest("https://twitter.com/kayabros"));
            }
            else if(param1.text == "Soundtrack")
            {
               if(stage.displayState == StageDisplayState.FULL_SCREEN)
               {
                  switchFullScreen();
               }
               Lib.getURL(new URLRequest("http://talhakaya.bandcamp.com/album/sleepy-time-soundtrack"));
            }
            else if(param1.text == "Song 1")
            {
               if(!menu.destroyMePlease)
               {
                  menu.destroyMePlease = true;
                  GameManager.id = 0;
                  dialogueScreen = new DialogueScreen();
                  newScreen(dialogueScreen);
               }
            }
            else if(param1.text == "Song 2")
            {
               if(!menu.destroyMePlease)
               {
                  menu.destroyMePlease = true;
                  GameManager.id = 1;
                  dialogueScreen = new DialogueScreen();
                  newScreen(dialogueScreen);
               }
            }
            else if(param1.text == "Song 3")
            {
               if(!menu.destroyMePlease)
               {
                  menu.destroyMePlease = true;
                  GameManager.id = 2;
                  dialogueScreen = new DialogueScreen();
                  newScreen(dialogueScreen);
               }
            }
            else if(param1.text == "Song 4")
            {
               if(!menu.destroyMePlease)
               {
                  menu.destroyMePlease = true;
                  GameManager.id = 3;
                  dialogueScreen = new DialogueScreen();
                  newScreen(dialogueScreen);
               }
            }
            else if(param1.text == "Song 5")
            {
               if(!menu.destroyMePlease)
               {
                  menu.destroyMePlease = true;
                  GameManager.id = 4;
                  dialogueScreen = new DialogueScreen();
                  newScreen(dialogueScreen);
               }
            }
            else if(param1.text == "Song 6")
            {
               if(!menu.destroyMePlease)
               {
                  menu.destroyMePlease = true;
                  GameManager.id = 5;
                  dialogueScreen = new DialogueScreen();
                  newScreen(dialogueScreen);
               }
            }
            else if(param1.text == "Tutorial")
            {
               if(!menu.destroyMePlease)
               {
                  menu.destroyMePlease = true;
                  GameManager.id = -1;
                  dialogueScreen = new DialogueScreen();
                  newScreen(dialogueScreen);
               }
            }
            else if(param1.text == "Fullscreen")
            {
               switchFullScreen();
               if(!menu.destroyMePlease)
               {
                  menu.destroyMePlease = true;
                  GameManager.id = -2;
                  dialogueScreen = new DialogueScreen();
                  newScreen(dialogueScreen);
               }
            }
         }
      }
      
      public function buttonHandler() : void
      {
         var _loc3_:int = 0;
         var _loc1_:int = 0;
         var _loc2_:int = int(Button.buttons.length);
         while(_loc1_ < _loc2_)
         {
            _loc3_ = _loc1_++;
            Button.buttons[_loc3_].update();
            if(Button.buttons[_loc3_].isColliding())
            {
               if(GameManager.mouseDown && !GameManager.mouseDownOld)
               {
                  buttonPressHandler(Button.buttons[_loc3_]);
               }
               else
               {
                  Button.buttons[_loc3_].mouseOverHandler();
               }
            }
         }
         GameManager.mouseDownOld = GameManager.mouseDown;
      }
   }
}


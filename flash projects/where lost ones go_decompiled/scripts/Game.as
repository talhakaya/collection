package
{
   import com.greensock.*;
   import com.greensock.easing.*;
   import flash.display.MovieClip;
   import flash.display.Sprite;
   import flash.events.Event;
   import flash.events.MouseEvent;
   import flash.events.TimerEvent;
   import flash.text.TextField;
   import flash.utils.Timer;
   import mx.collections.ArrayList;
   
   public class Game extends Sprite
   {
      
      public static var instance:Game;
      
      public static const debugTimerConst:Number = 1;
      
      public static const CarY:Number = 194;
      
      public static const CarXMin:Number = 276;
      
      public static const CarXMax:Number = 552;
      
      public static const BirdX:Number = 20;
      
      public static const BirdY:Number = 20;
      
      public static var STATE:int = 0;
      
      public var Verdana:Class = Game_Verdana;
      
      public var BackgroundImage:MovieClip;
      
      public var zoomLocationX:Number = -386;
      
      public var zoomLocationY:Number = -188;
      
      public var zoomDuration:Number = 10;
      
      public var zoomScale:Number = 20;
      
      public var zoomBool:Boolean;
      
      public var player:Player;
      
      public var timer:Timer;
      
      public var playerFalling:Boolean;
      
      public var textFieldClickIfYouDare:TextField;
      
      public var count:int;
      
      public var collisionCheckList:ArrayList;
      
      public function Game()
      {
         super();
         instance = this;
         this.BackgroundImage = new background();
         addChild(this.BackgroundImage);
         this.createSeaEffects(40);
         this.createCars(40);
         this.createBirds(40);
         addEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
         Main.Foreground.addEventListener(MouseEvent.CLICK,this.mouseClickHandler);
         this.player = new Player();
         addChild(this.player);
         STATE = 1;
         this.count = 0;
         this.collisionCheckList = new ArrayList();
         SoundManager.playBeach();
         ScreenTextHandler.instance.makeTitleTextFieldVisible("\"Where Lost Ones Go\"\n\n                       by Talha Kaya\n\n\n                                                                                   Play with mouse");
      }
      
      public function enterFrameHandler(e:Event) : void
      {
         if(STATE == 3)
         {
            ++this.count;
            if(this.count >= 10)
            {
               this.count = 0;
               Main.instance.addChild(new DropState2());
            }
            this.collisionCheck();
         }
         else if(STATE == 7)
         {
            ++this.count;
            if(this.count >= 30)
            {
               this.count = 0;
               Main.instance.addChild(new Scorpion(stage.mouseX,stage.mouseY,0.25));
            }
            this.collisionCheck();
         }
      }
      
      public function createSeaEffects(howMany:int) : void
      {
         var particle:SeaEffect = null;
         for(var i:int = 0; i < howMany; i++)
         {
            particle = new SeaEffect();
            addChild(particle);
         }
      }
      
      public function createCars(howMany:int) : void
      {
         var car:Car = null;
         for(var i:int = 0; i < howMany; i++)
         {
            car = new Car();
            addChild(car);
         }
      }
      
      public function createBirds(howMany:int) : void
      {
         var bird:Bird = null;
         Bird.birdX = 140;
         Bird.birdY = 125;
         Bird.destinationX = 680;
         Bird.destinationY = 50 + 100 * Math.random();
         for(var i:int = 0; i < howMany; i++)
         {
            bird = new Bird(Bird.birdX,Bird.birdY,Bird.destinationX,Bird.destinationY);
            addChild(bird);
         }
         Bird.birdX = -40;
         Bird.birdY = 50 + 100 * Math.random();
      }
      
      public function zoomIn() : void
      {
         this.zoomBool = !this.zoomBool;
         TweenLite.to(this,this.zoomDuration * debugTimerConst,{
            "x":this.zoomLocationX * this.zoomScale,
            "y":this.zoomLocationY * this.zoomScale,
            "scaleX":this.zoomScale,
            "scaleY":this.zoomScale,
            "ease":Cubic.easeIn
         });
      }
      
      public function zoomOut() : void
      {
         this.zoomBool = !this.zoomBool;
         TweenLite.to(this,this.zoomDuration * debugTimerConst,{
            "x":0,
            "y":0,
            "scaleX":1,
            "scaleY":1,
            "ease":Cubic.easeOut,
            "onComplete":this.zoomOutCompleteHandler
         });
      }
      
      public function zoomOutCompleteHandler() : void
      {
         ScreenTextHandler.instance.makeTitleTextFieldVisible("\n                       THE END\n\n \"Where Lost Ones Go\"\n\n                       by Talha Kaya");
      }
      
      public function mouseClickHandler(e:MouseEvent) : void
      {
         if(STATE == 1)
         {
            TweenLite.killTweensOf(this);
            this.zoomIn();
            TweenLite.to(this.player,18 * debugTimerConst,{"x":this.player.x + 21});
            STATE = 0;
            this.timer = new Timer(11000 * debugTimerConst,1);
            this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState1);
            this.timer.start();
            SoundManager.stopBeach();
            ScreenTextHandler.instance.makeTitleTextFieldInvisible();
         }
         else if(STATE == 2)
         {
            TweenLite.to(this.player,117 * debugTimerConst,{
               "y":this.player.y + 32,
               "onComplete":this.playerFalled
            });
            TweenLite.to(this,117 * debugTimerConst,{"y":y - 24 * this.zoomScale});
            STATE = 0;
            this.playerFalling = true;
            ScreenTextHandler.instance.makeCustomTextFieldInvisible();
            this.timer = new Timer(7000 * debugTimerConst,1);
            this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState2);
            this.timer.start();
            SoundManager.playMusic();
         }
         else if(STATE == 8)
         {
         }
      }
      
      public function playerFalled() : void
      {
         this.player.die();
         this.zoomOut();
         SoundManager.continueBeach();
      }
      
      public function timerHandlerState1(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState1);
         MessageBox.instance.makeVisible(1);
         this.timer = new Timer(7000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState1_1);
         this.timer.start();
      }
      
      public function timerHandlerState1_1(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState1_1);
         MessageBox.instance.makeVisible(2);
         this.timer = new Timer(3000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState1_2);
         this.timer.start();
      }
      
      public function timerHandlerState1_2(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState1_2);
         STATE = 2;
         ScreenTextHandler.instance.makeCustomTextFieldVisible("JUMP");
      }
      
      public function timerHandlerState2(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState2);
         MessageBox.instance.makeVisible(3);
         this.timer = new Timer(4000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState2_1);
         this.timer.start();
      }
      
      public function timerHandlerState2_1(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState2_1);
         MessageBox.instance.makeVisible(4);
         this.timer = new Timer(3000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState2_2);
         this.timer.start();
      }
      
      public function timerHandlerState2_2(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState2_2);
         STATE = 3;
         this.player.openScene();
         this.timer = new Timer(13000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState3);
         this.timer.start();
         Main.instance.addChild(new Scorpion(100,100,1.5));
         Main.instance.addChild(new Scorpion(600,200,1.5));
         Main.instance.addChild(new Scorpion(400,300,1.5));
      }
      
      public function timerHandlerState3(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState3);
         STATE = 0;
         this.player.closeScene();
         this.timer = new Timer(4000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState3_1);
         this.timer.start();
      }
      
      public function timerHandlerState3_1(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState3_1);
         MessageBox.instance.makeVisible(5);
         this.timer = new Timer(3000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState3_2);
         this.timer.start();
      }
      
      public function timerHandlerState3_2(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState3_2);
         this.player.openScene();
         STATE = 4;
         this.timer = new Timer(10000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState4);
         this.timer.start();
         SeaState4.instance.isActive = true;
         SeaState4.instance.isBlood = true;
         Main.instance.addChild(new Human(30,320,true));
         Main.instance.addChild(new Human(70,310,true));
         Main.instance.addChild(new Human(45,330,true));
         Main.instance.addChild(new Human(120,340,true));
         Main.instance.addChild(new Human(140,320,true));
         Main.instance.addChild(new Human(180,330,true));
         Main.instance.addChild(new Human(230,340,true));
         Main.instance.addChild(new Human(250 + 30,320,true));
         Main.instance.addChild(new Human(250 + 70,310,true));
         Main.instance.addChild(new Human(250 + 45,330,true));
         Main.instance.addChild(new Human(250 + 120,340,true));
         Main.instance.addChild(new Human(250 + 140,320,true));
         Main.instance.addChild(new Human(250 + 180,330,true));
         Main.instance.addChild(new Human(250 + 230,340,true));
         Main.instance.addChild(new Human(500 + 30,320,true));
         Main.instance.addChild(new Human(500 + 70,310,true));
         Main.instance.addChild(new Human(500 + 45,330,true));
         Main.instance.addChild(new Human(500 + 120,340,true));
         Main.instance.addChild(new Human(500 + 140,320,true));
         Main.instance.addChild(new Human(500 + 180,330,true));
         Main.instance.addChild(new Human(500 + 230,340,true));
      }
      
      public function timerHandlerState4(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState4);
         this.player.closeScene();
         STATE = 0;
         this.timer = new Timer(3000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState4_1);
         this.timer.start();
      }
      
      public function timerHandlerState4_1(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState4_1);
         MessageBox.instance.makeVisible(6);
         this.timer = new Timer(4000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState4_2);
         this.timer.start();
      }
      
      public function timerHandlerState4_2(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState4_2);
         MessageBox.instance.makeVisible(7);
         this.timer = new Timer(3000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState4_3);
         this.timer.start();
      }
      
      public function timerHandlerState4_3(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState4_3);
         this.player.openScene();
         STATE = 5;
         this.timer = new Timer(15000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState5);
         this.timer.start();
         SeaState4.instance.isActive = true;
      }
      
      public function timerHandlerState5(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState5);
         this.player.closeScene();
         STATE = 0;
         this.timer = new Timer(2000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState5_1);
         this.timer.start();
      }
      
      public function timerHandlerState5_1(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState5_1);
         MessageBox.instance.makeVisible(8);
         this.timer = new Timer(3000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState5_2);
         this.timer.start();
      }
      
      public function timerHandlerState5_2(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState5_2);
         this.player.openScene();
         STATE = 6;
         Main.instance.addChild(new Human(100,100,false));
         Main.instance.addChild(new Human(200,80,false));
         Main.instance.addChild(new Human(40,320,false));
         Main.instance.addChild(new Human(580,60,false));
         Main.instance.addChild(new Human(600,200,false));
         Main.instance.addChild(new Human(400,300,false));
         Main.instance.addChild(new Human(200,270,false));
         Main.instance.addChild(new Human(250,200,false));
         Main.instance.addChild(new Human(550,260,false));
         Main.instance.addChild(new Human(360,120,false));
         this.timer = new Timer(10000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState6);
         this.timer.start();
      }
      
      public function timerHandlerState6(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState6);
         this.player.closeScene();
         STATE = 0;
         this.timer = new Timer(2000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState6_1);
         this.timer.start();
      }
      
      public function timerHandlerState6_1(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState6_1);
         MessageBox.instance.makeVisible(9);
         this.timer = new Timer(4000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState6_2);
         this.timer.start();
      }
      
      public function timerHandlerState6_2(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState6_2);
         MessageBox.instance.makeVisible(10);
         this.timer = new Timer(4000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState6_3);
         this.timer.start();
      }
      
      public function timerHandlerState6_3(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState6_3);
         MessageBox.instance.makeVisible(11);
         this.timer = new Timer(4000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState6_4);
         this.timer.start();
      }
      
      public function timerHandlerState6_4(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState6_4);
         MessageBox.instance.makeVisible(12);
         this.timer = new Timer(3000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState6_5);
         this.timer.start();
      }
      
      public function timerHandlerState6_5(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState6_5);
         this.player.openScene();
         STATE = 7;
         this.timer = new Timer(16000 * debugTimerConst,1);
         this.timer.addEventListener(TimerEvent.TIMER,this.timerHandlerState7);
         this.timer.start();
      }
      
      public function timerHandlerState7(e:TimerEvent) : void
      {
         this.timer.removeEventListener(TimerEvent.TIMER,this.timerHandlerState7);
         this.player.closeScene();
         STATE = 0;
      }
      
      public function collisionCheck() : void
      {
         var i:int = 0;
         var j:int = 0;
         for(i = 0; i < this.collisionCheckList.length; i++)
         {
            if(this.collisionCheckList.getItemAt(i) != null)
            {
               if(this.collisionCheckList.getItemAt(i) is Scorpion)
               {
                  for(j = 0; j < this.collisionCheckList.length; j++)
                  {
                     if(this.collisionCheckList.getItemAt(j) != null)
                     {
                        if(this.collisionCheckList.getItemAt(j) is DropState2)
                        {
                           if(Main.distanceBetweenSpritesCloserThanRadius(this.collisionCheckList.getItemAt(i).radius + this.collisionCheckList.getItemAt(j).radius,this.collisionCheckList.getItemAt(i) as Sprite,this.collisionCheckList.getItemAt(j) as Sprite))
                           {
                              this.collisionCheckList.getItemAt(i).dieAlready = true;
                           }
                        }
                     }
                  }
                  if(this.player != null && Main.distanceBetweenPlayerCloserThanRadius(this.player.radius + this.collisionCheckList.getItemAt(i).radius,this.collisionCheckList.getItemAt(i) as Sprite))
                  {
                     this.player.isThereBlood = true;
                     this.collisionCheckList.getItemAt(i).touchingPlayer = true;
                  }
               }
            }
         }
      }
   }
}


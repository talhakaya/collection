package levels
{
   import flash.net.URLRequest;
   import flash.net.navigateToURL;
   import org.flixel.*;
   
   public class Menu extends Level
   {
      
      private static var S_:Class = Menu_S_;
      
      private static var music:Class = Menu_music;
      
      private var count:int;
      
      private var clouds:Array;
      
      private const noOfBirds:int = 10;
      
      private const birdsY:Number = 360;
      
      private var timerForBirds:TTimer;
      
      private var timerForBirds2:TTimer;
      
      private var beyaz:FlxSprite;
      
      private var castle:FlxSprite;
      
      private var text1:FlxText;
      
      private var text2:FlxText;
      
      private var text3:FlxText;
      
      private var text3s:FlxText;
      
      private var text3a:FlxText;
      
      private var text4:FlxText;
      
      private var pressedSpace:Boolean;
      
      private var pressed:Boolean;
      
      private const textspeed:Number = 2;
      
      private const castlespeed:Number = 0.2;
      
      private var buttons:Array;
      
      public function Menu()
      {
         super();
      }
      
      override public function create() : void
      {
         var i:int = 0;
         gameSave = new FlxSave();
         gameSave.bind("save");
         if(gameSave.data.level != null)
         {
            levelno = gameSave.data.level;
         }
         if(gameSave.data.level == 0)
         {
            levelno = 46;
         }
         FlxG.playMusic(music,1);
         levelwidth = 40;
         levelheight = 30;
         scale = new FlxPoint(1,1);
         FlxG.bgColor = 4289374890;
         player = new Hans(0,-200,scale);
         super.create();
         add(player);
         this.castle = new FlxSprite(144,60);
         this.castle.loadGraphic(S_,true,false,32,32,false);
         this.castle.addAnimation(" ",[0,1],1,true);
         this.castle.play(" ");
         bgObjects.add(this.castle);
         this.birds(true);
         this.birds(true);
         this.birds(false);
         this.timerForBirds = new TTimer();
         this.timerForBirds2 = new TTimer();
         add(this.timerForBirds);
         add(this.timerForBirds2);
         this.timerForBirds.start(100);
         this.timerForBirds2.start(400);
         this.clouds = new Array();
         for(i = 0; i < 37; i++)
         {
            this.clouds.push(bgObjects.add(new Cloud2(-100 + Math.random() * 440,levelwidth)));
            this.clouds[i].x = -100 + i * 10;
            bgObjects.add(this.clouds[i]);
         }
         this.text1 = new FlxText(25,20,320,"Ode To Pixel Days",true);
         this.text1.size = 24;
         this.text1.color = 4289357414;
         this.text2 = new FlxText(27,22,320,"Ode To Pixel Days",true);
         this.text2.size = 24;
         this.text2.color = 4278190080;
         add(this.text2);
         add(this.text1);
         this.buttons = new Array();
         var YBUTTON:Number = 96;
         if(gameSave.data.level == null)
         {
            this.buttons.push(new FlxButton(120,YBUTTON,"New Game",this.newGame));
            add(this.buttons[this.buttons.length - 1]);
         }
         else
         {
            this.buttons.push(new FlxButton(120,YBUTTON,"Continue",this.continueGame));
            add(this.buttons[this.buttons.length - 1]);
            this.buttons.push(new FlxButton(120,YBUTTON + 22,"New Game",this.newGame));
            add(this.buttons[this.buttons.length - 1]);
         }
         this.buttons.push(new FlxButton(120,YBUTTON + 44,"More Games",this.goToMyURL));
         add(this.buttons[this.buttons.length - 1]);
         this.buttons.push(new FlxButton(120,YBUTTON + 66,"Soundtrack",this.goToMyURL2));
         add(this.buttons[this.buttons.length - 1]);
         this.buttons.push(new FlxButton(120,YBUTTON + 88,"Extras",this.goToMyURL3));
         add(this.buttons[this.buttons.length - 1]);
         this.text3a = new FlxText(30,206,320,"Press M At Any Time To Mute On/Off Sounds and Music",true);
         this.text3a.color = 4289357414;
         add(this.text3a);
         this.beyaz = new FlxSprite(0,0);
         this.beyaz.makeGraphic(320,240,4294967295);
         this.beyaz.alpha = 0;
         add(this.beyaz);
      }
      
      public function goToMyURL() : void
      {
         navigateToURL(new URLRequest("http://www.maxgames.com"));
      }
      
      public function goToMyURL2() : void
      {
         navigateToURL(new URLRequest("http://talhakaya.bandcamp.com/album/ode-to-pixel-days-soundtrack"));
      }
      
      public function goToMyURL3() : void
      {
         navigateToURL(new URLRequest("http://talhadevlog.blogspot.com/2013/02/ode-to-pixel-days-extras.html"));
      }
      
      public function newGame() : void
      {
         if(!this.pressed)
         {
            this.pressedSpace = false;
            this.beyazlas();
         }
      }
      
      public function continueGame() : void
      {
         if(!this.pressed)
         {
            this.pressedSpace = true;
            this.beyazlas();
         }
      }
      
      override public function update() : void
      {
         var i:int = 0;
         if(!created || player == null)
         {
            return;
         }
         super.update();
         player.y = -200;
         if(this.timerForBirds.complete)
         {
            this.birds(false);
            this.birds(true);
         }
         if(this.timerForBirds2.complete)
         {
            this.birds(false);
            this.birds(true);
         }
         ++this.count;
         if(this.count == 30)
         {
            bgObjects.add(new Cloud2(-100 + Math.random() * 440,levelwidth));
            this.count = 0;
         }
         if(!this.pressed)
         {
            if(FlxG.keys.justPressed("SPACE"))
            {
               this.pressedSpace = true;
               this.beyazlas();
            }
            if(FlxG.keys.justPressed("N") && gameSave.data.level != null)
            {
               this.pressedSpace = false;
               this.beyazlas();
            }
         }
         else
         {
            this.beyaz.alpha += 0.02;
            this.text1.y -= this.textspeed;
            this.text2.y -= this.textspeed;
            for(i = 0; i < this.buttons.length; i++)
            {
               this.buttons[i].alpha -= 0.04;
            }
            this.castle.scale.x += this.castlespeed;
            this.castle.scale.y += this.castlespeed;
            this.castle.x = 160 - 16 * scale.x;
         }
      }
      
      override public function addBgDetail() : void
      {
         var i:int = 0;
         var j:int = 0;
         for(i = 0; i < 5; i++)
         {
            for(j = 0; j < 4; j++)
            {
               bgDetails[i][j] = "windowBig11";
            }
         }
      }
      
      private function birds(onLeft:Boolean) : void
      {
         var i:int = 0;
         if(onLeft)
         {
            for(i = 0; i < this.noOfBirds; i++)
            {
               bgObjects.add(new Bird2(-100,player.y + this.birdsY));
            }
         }
         else
         {
            for(i = 0; i < this.noOfBirds; i++)
            {
               bgObjects.add(new Bird2(420,player.y + this.birdsY));
            }
         }
      }
      
      private function beyazlas() : void
      {
         if(!this.pressed)
         {
            new FlxTimer().start(1,1,this.startPlaying);
            this.pressed = true;
         }
      }
      
      private function startPlaying(a:FlxTimer) : void
      {
         if(this.pressedSpace)
         {
            this.startPlaying2();
         }
         if(!this.pressedSpace)
         {
            FlxG.switchState(new Level1());
         }
      }
      
      private function startPlaying2() : void
      {
         musicStop();
         trace(gameSave.data.level);
         if(gameSave.data.level == null)
         {
            FlxG.switchState(new Level1());
         }
         else if(gameSave.data.level == 1)
         {
            FlxG.switchState(new Level1());
         }
         else if(gameSave.data.level == 2)
         {
            FlxG.switchState(new Level2());
         }
         else if(gameSave.data.level == 3)
         {
            FlxG.switchState(new Level3());
         }
         else if(gameSave.data.level == 4)
         {
            FlxG.switchState(new Level3second());
         }
         else if(gameSave.data.level == 5)
         {
            FlxG.switchState(new Level4());
         }
         else if(gameSave.data.level == 6)
         {
            FlxG.switchState(new Level5());
         }
         else if(gameSave.data.level == 7)
         {
            FlxG.switchState(new Level6());
         }
         else if(gameSave.data.level == 8)
         {
            FlxG.switchState(new Level7());
         }
         else if(gameSave.data.level == 9)
         {
            FlxG.switchState(new Level8());
         }
         else if(gameSave.data.level == 10)
         {
            FlxG.switchState(new Level9());
         }
         else if(gameSave.data.level == 11)
         {
            FlxG.switchState(new Level10());
         }
         else if(gameSave.data.level == 12)
         {
            FlxG.switchState(new Level11());
         }
         else if(gameSave.data.level == 13)
         {
            FlxG.switchState(new Level12());
         }
         else if(gameSave.data.level == 14)
         {
            FlxG.switchState(new Level13());
         }
         else if(gameSave.data.level == 15)
         {
            FlxG.switchState(new Level14());
         }
         else if(gameSave.data.level == 16)
         {
            FlxG.switchState(new Level15());
         }
         else if(gameSave.data.level == 17)
         {
            FlxG.switchState(new Level16());
         }
         else if(gameSave.data.level == 18)
         {
            FlxG.switchState(new Level17());
         }
         else if(gameSave.data.level == 19)
         {
            FlxG.switchState(new Level18());
         }
         else if(gameSave.data.level == 20)
         {
            FlxG.switchState(new Level19());
         }
         else if(gameSave.data.level == 21)
         {
            FlxG.switchState(new Level20());
         }
         else if(gameSave.data.level == 22)
         {
            FlxG.switchState(new Level21());
         }
         else if(gameSave.data.level == 23)
         {
            FlxG.switchState(new Level22());
         }
         else if(gameSave.data.level == 24)
         {
            FlxG.switchState(new Level23());
         }
         else if(gameSave.data.level == 25)
         {
            FlxG.switchState(new Level24());
         }
         else if(gameSave.data.level == 26)
         {
            FlxG.switchState(new Level25());
         }
         else if(gameSave.data.level == 27)
         {
            FlxG.switchState(new Level26());
         }
         else if(gameSave.data.level == 28)
         {
            FlxG.switchState(new Level27());
         }
         else if(gameSave.data.level == 29)
         {
            FlxG.switchState(new Level27second());
         }
         else if(gameSave.data.level == 30)
         {
            FlxG.switchState(new Level28());
         }
         else if(gameSave.data.level == 31)
         {
            FlxG.switchState(new Level29());
         }
         else if(gameSave.data.level == 32)
         {
            FlxG.switchState(new Level30());
         }
         else if(gameSave.data.level == 33)
         {
            FlxG.switchState(new Level31());
         }
         else if(gameSave.data.level == 34)
         {
            FlxG.switchState(new Level32());
         }
         else if(gameSave.data.level == 35)
         {
            FlxG.switchState(new Level33());
         }
         else if(gameSave.data.level == 36)
         {
            FlxG.switchState(new Level34());
         }
         else if(gameSave.data.level == 37)
         {
            FlxG.switchState(new Level35());
         }
         else if(gameSave.data.level == 38)
         {
            FlxG.switchState(new Level35second());
         }
         else if(gameSave.data.level == 39)
         {
            FlxG.switchState(new Level36());
         }
         else if(gameSave.data.level == 40)
         {
            FlxG.switchState(new Level37());
         }
         else if(gameSave.data.level == 41)
         {
            FlxG.switchState(new Level38());
         }
         else if(gameSave.data.level == 42)
         {
            FlxG.switchState(new Level39());
         }
         else if(gameSave.data.level == 43)
         {
            FlxG.switchState(new Level40());
         }
         else if(gameSave.data.level == 44)
         {
            FlxG.switchState(new Level41());
         }
         else if(gameSave.data.level == 45)
         {
            FlxG.switchState(new Level42());
         }
         else if(gameSave.data.level == 46)
         {
            FlxG.switchState(new Level43());
         }
      }
   }
}


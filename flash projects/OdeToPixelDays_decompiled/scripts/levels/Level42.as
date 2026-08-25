package levels
{
   import org.flixel.*;
   
   public class Level42 extends Level
   {
      
      private static var S_tiles:Class = Level42_S_tiles;
      
      private static var music:Class = Level42_music;
      
      private var makeClouds:Boolean;
      
      private var count:int;
      
      private var count2:int;
      
      private var period:int;
      
      private const noOfBirds:int = 50;
      
      private var birdsSent:int;
      
      private var hansSent:int;
      
      private const birdsY:Number = 360;
      
      private var hans2:Hans;
      
      private var hans4:Hans;
      
      private var hans8:Hans;
      
      private var hans16:Hans;
      
      private const hansDistance:Number = 48;
      
      private var fadeToBlack:FlxSprite;
      
      private var hansLoaded:Boolean;
      
      private var hansAnimated:Boolean;
      
      private var gecis:Boolean;
      
      public function Level42()
      {
         super();
      }
      
      override public function create() : void
      {
         noMusic = true;
         gameSave = new FlxSave();
         gameSave.bind("save");
         gameSave.data.level = 45;
         this.gecis = false;
         levelwidth = 40;
         levelheight = 1200;
         scale = new FlxPoint(1,1);
         super.create();
         FlxG.bgColor = 4289357414;
         var data:Array = new Array(16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,16,16,15,15,15,15,15,15,15,15,13,0,7,13,0,5,0,7,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,16,16,16,16,16,16,16,16,12,16,16,15,16,16,11,12,11,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,12,12,12,12,16,12,16,14,0,8,16,16,12,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,0,8,16,15,16,16,10,0,0,0,0
         ,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,15,16,16,12,12,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,16,12,10,0,0,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,0,0,7,11,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,15,15,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,40),S_tiles,0,0,FlxTilemap.AUTO);
         add(level);
         bgObjects.add(new Door(26,24,true,scale));
         player = new Hans(32,36,scale);
         add(player);
         narrators.add(new Narrator(160,40,200,74,false));
         this.makeClouds = false;
         this.hansLoaded = false;
         this.hansAnimated = false;
         this.birdsSent = 0;
         this.hansSent = 0;
         this.count2 = 0;
         FlxG.camera.setBounds(0,0,320,9600,true);
         FlxG.camera.follow(player,FlxCamera.STYLE_LOCKON);
         this.fadeToBlack = new FlxSprite(0,9360);
         this.fadeToBlack.makeGraphic(320,240,4294967295,false);
         this.fadeToBlack.alpha = 0;
         add(this.fadeToBlack);
      }
      
      override public function update() : void
      {
         super.update();
         if(!this.gecis && player.y > 320)
         {
            FlxG.playMusic(music,1);
            this.gecis = true;
         }
         if(!this.makeClouds)
         {
            if(player.y > 320 && player.y < 9600)
            {
               this.makeClouds = true;
               this.period = 10;
            }
         }
         else if(this.count == this.period)
         {
            if(Math.random() > 0.3)
            {
               bgObjects.add(new Cloud(player,false));
            }
            else
            {
               add(new Cloud(player,false));
            }
            this.count = 0;
            if(player.y < 2000)
            {
               this.period = 4;
               if(this.birdsSent == 0)
               {
                  ++this.birdsSent;
                  this.birds(true);
               }
            }
            else if(player.y < 4000)
            {
               this.period = 1;
               if(this.birdsSent == 1)
               {
                  ++this.birdsSent;
                  this.birds(false);
               }
            }
            else if(player.y < 6000)
            {
               this.period = 3;
               if(this.birdsSent == 2)
               {
                  ++this.birdsSent;
                  this.birds(false);
               }
            }
            else if(player.y < 8000)
            {
               this.period = 2;
               if(this.birdsSent == 3)
               {
                  ++this.birdsSent;
                  this.birds(true);
                  this.birds(true);
               }
            }
            else if(player.y < 9600)
            {
               this.period = 4;
            }
            else
            {
               this.makeClouds = false;
            }
            if(player.y > 5000)
            {
               if(this.hansSent == 0)
               {
                  ++this.hansSent;
                  this.hans2 = new Hans(player.x - this.hansDistance,player.y + 36,new FlxPoint(2,2));
                  this.hans2.lastLevel = true;
                  this.hans2.jumpEnable = false;
                  add(this.hans2);
               }
               this.hans2.x = player.x - this.hansDistance;
            }
            if(player.y > 5500)
            {
               if(this.hansSent == 1)
               {
                  ++this.hansSent;
                  this.hans4 = new Hans(player.x + this.hansDistance - 8,player.y + 44,new FlxPoint(4,4));
                  this.hans4.lastLevel = true;
                  this.hans4.jumpEnable = false;
                  add(this.hans4);
               }
               this.hans4.x = player.x + this.hansDistance - 8;
            }
            if(player.y > 6500)
            {
               if(this.hansSent == 2)
               {
                  ++this.hansSent;
                  this.hans8 = new Hans(player.x + this.hansDistance - 16,player.y + 92,new FlxPoint(8,8));
                  this.hans8.lastLevel = true;
                  this.hans8.jumpEnable = false;
                  add(this.hans8);
               }
               this.hans8.x = player.x + this.hansDistance - 16;
            }
            if(player.y > 7000)
            {
               if(this.hansSent == 3)
               {
                  ++this.hansSent;
                  this.hans16 = new Hans(player.x - this.hansDistance + 20,player.y + 88,new FlxPoint(16,16));
                  this.hans16.lastLevel = true;
                  this.hans16.jumpEnable = false;
                  add(this.hans16);
               }
               this.hans16.x = player.x - this.hansDistance + 20;
            }
         }
         else
         {
            ++this.count;
         }
         if(player.x < 0)
         {
            player.x = 0;
         }
         else if(player.x > 312)
         {
            player.x = 312;
         }
         ++this.count2;
         if(this.count2 == 3)
         {
            this.count2 = 0;
         }
         if(!this.hansAnimated && player.y > 240)
         {
            if(!this.hansLoaded)
            {
               this.hansLoaded = true;
               player.gokyuzundenDus();
            }
            else if(player.y < 400)
            {
               player.fall0();
            }
            else if(player.y < 560)
            {
               player.fall1();
            }
            else if(player.y < 720)
            {
               player.fall2();
            }
            else
            {
               player.fall3();
            }
         }
         if(player.y > 9600 && this.fadeToBlack.alpha < 1)
         {
            this.fadeToBlack.alpha += 0.005;
            if(FlxG.music != null)
            {
               FlxG.music.volume -= 0.005;
            }
         }
         if(player.y > 11000)
         {
            this.nextLevel();
         }
      }
      
      override public function addBgDetail() : void
      {
         var i:int = 0;
         var j:int = 0;
         for(i = 0; i < 5; i++)
         {
            bgDetails[i][0] = "damaged";
            bgDetails[i][1] = "damaged";
            bgDetails[i][2] = "windowBig10";
            bgDetails[i][3] = "windowBig11";
         }
         for(j = 4; j < 151; j++)
         {
            for(i = 0; i < 5; i++)
            {
               bgDetails[i][j] = "windowBig11";
            }
         }
         bgDetails[4][0] = "windowBig01";
         bgDetails[4][1] = "windowBig01";
         bgDetails[4][2] = "corner";
      }
      
      override public function nextLevel() : void
      {
         FlxG.switchState(new Level43());
      }
      
      private function birds(onLeft:Boolean) : void
      {
         var i:int = 0;
         if(onLeft)
         {
            for(i = 0; i < this.noOfBirds; i++)
            {
               if(Math.random() > 0.5)
               {
                  add(new Bird(-100,player.y + this.birdsY));
               }
               else
               {
                  bgObjects.add(new Bird(-100,player.y + this.birdsY));
               }
            }
         }
         else
         {
            for(i = 0; i < this.noOfBirds; i++)
            {
               if(Math.random() > 0.5)
               {
                  add(new Bird(420,player.y + this.birdsY));
               }
               else
               {
                  bgObjects.add(new Bird(420,player.y + this.birdsY));
               }
            }
         }
      }
   }
}


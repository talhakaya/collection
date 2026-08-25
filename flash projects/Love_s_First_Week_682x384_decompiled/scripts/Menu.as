package
{
   import org.flixel.*;
   
   public class Menu extends State1
   {
      
      private static var Asagi:Class = Menu_Asagi;
      
      private static var img1:Class = Menu_img1;
      
      private static var img2:Class = Menu_img2;
      
      private static var img3:Class = Menu_img3;
      
      private static var img4:Class = Menu_img4;
      
      private static var music:Class = Menu_music;
      
      public var son:FlxText;
      
      public var asagi1:FlxSprite;
      
      public var asagiBas:Boolean;
      
      public var oyun:FlxText;
      
      public var oyun2:FlxText;
      
      public var credits:FlxText;
      
      public var playButton:FlxButton;
      
      public function Menu()
      {
         super(true);
      }
      
      override public function create() : void
      {
         isMenuButton = false;
         FlxG.playMusic(music,0.4);
         talhaVar = true;
         diyalogVar = true;
         levelwidth = 16;
         levelheight = 8;
         bgYogunluk = 0;
         super.create();
         var data:Array = new Array(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,13,0,0,0,0,0,0,0,0,0,0,15,15,15,15,16,14,0,0,0,0,0,0,0,0,0,0,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(naz = new Naz(true,166,130));
         ondekiler.add(talha = new Naz(false,150,130));
         naz.sonBolum = true;
         talha.sonBolum = true;
         naz.play("otur1");
         talha.play("otur2");
         sarmasik(4,3,-10,100);
         agac("2",50,122);
         agac("2",0,122);
         agac("0",20,122);
         agac("0",-20,122);
         agac("1",-10,122);
         agac("1",40,122);
         agac("3",60,122);
         agac("3",80,122);
         agac("2",175,187);
         agac("1",195,187);
         agac("3",220,190);
         agac("0",250,230);
         bulut(20);
         kafaoku.x = -8;
         kafaoku.y = -8;
         if(isFullscreenAvailable)
         {
            enOndekiler.add(new FlxSprite((455 - 120) / 2,184,img3));
         }
         enOndekiler.add(new FlxSprite((455 - 120) / 2,204,img4));
         this.oyun = new FlxText(0,0,455,"");
         this.oyun.setFormat("NES",60,4281017343,"center",2);
         add(this.oyun);
         this.oyun2 = new FlxText(0,50,455,"");
         this.oyun2.setFormat("NES",60,4281017343,"center",2);
         add(this.oyun2);
         enOndekiler.add(new FlxText(193,212,100,"Language / Dil"));
         enOndekiler.add(new FlxButton(150,224,"Turkce",this.turkce));
         enOndekiler.add(new FlxButton(232,224,"English",this.english));
         enOndekiler.add(new FlxButton((455 - 80) / 2,183,"More Games",goToMyURL));
         if(save.data.level == null)
         {
            enOndekiler.add(this.playButton = new FlxButton((455 - 80) / 2,120,"1 Player Game",this.onePlayerNewGame));
            enOndekiler.add(this.playButton = new FlxButton((455 - 80) / 2,141,"2 Player Game",this.twoPlayerNewGame));
         }
         else
         {
            enOndekiler.add(this.playButton = new FlxButton((455 - 80) / 2,120,"1 Player Game",this.onePlayerNewGame));
            enOndekiler.add(this.playButton = new FlxButton((455 - 80) / 2,141,"2 Player Game",this.twoPlayerNewGame));
            enOndekiler.add(new FlxButton((455 - 80) / 2,162,"Continue Game",this.nextLevel2));
         }
         this.credits = new FlxText(180,244,455,"Made by Talha Kaya");
         add(this.credits);
      }
      
      override public function update() : void
      {
         super.update();
         talha.x = -100;
         talha.y = -100;
         naz.x = -100;
         naz.y = -100;
         if(save.data.lang == "tur")
         {
            this.oyun.text = "Askın";
            this.oyun2.text = "Ilk Haftası";
            this.credits.text = "Yapan: Talha Kaya";
         }
         else
         {
            this.oyun.text = "Love\'s";
            this.oyun2.text = "First Week";
            this.credits.text = "Made by Talha Kaya";
         }
      }
      
      private function turkce() : void
      {
         save.data.lang = "tur";
      }
      
      private function english() : void
      {
         save.data.lang = "eng";
      }
      
      public function onePlayerNewGame() : void
      {
         is1player = true;
         this.nextLevel();
      }
      
      public function twoPlayerNewGame() : void
      {
         is1player = false;
         this.nextLevel();
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         save.data.is1p = is1player;
         if(is1player)
         {
            FlxG.switchState(new Level1(is1player));
         }
         else
         {
            FlxG.switchState(new Level3two(is1player));
         }
      }
      
      public function nextLevel2() : void
      {
         super.nextLevel();
         if(save.data.is1p != null)
         {
            is1player = save.data.is1p;
         }
         if(save.data.level == 1)
         {
            if(is1player)
            {
               FlxG.switchState(new Level1(is1player));
            }
            else
            {
               FlxG.switchState(new Level3two(is1player));
            }
         }
         else if(save.data.level == 2)
         {
            if(is1player)
            {
               FlxG.switchState(new Level2(is1player));
            }
            else
            {
               FlxG.switchState(new Level3two(is1player));
            }
         }
         else if(save.data.level == 3)
         {
            if(is1player)
            {
               FlxG.switchState(new Level3(is1player));
            }
            else
            {
               FlxG.switchState(new Level3two(is1player));
            }
         }
         else if(save.data.level == 4)
         {
            if(is1player)
            {
               FlxG.switchState(new Level4(is1player));
            }
            else
            {
               FlxG.switchState(new Level4two(is1player));
            }
         }
         else if(save.data.level == 5)
         {
            if(is1player)
            {
               FlxG.switchState(new Level5(is1player));
            }
            else
            {
               FlxG.switchState(new Level5two(is1player));
            }
         }
         else if(save.data.level == 6)
         {
            FlxG.switchState(new Level6(is1player));
         }
         else if(save.data.level == 7)
         {
            FlxG.switchState(new Level7(is1player));
         }
         else if(save.data.level == 8)
         {
            FlxG.switchState(new Level8(is1player));
         }
         else if(save.data.level == 9)
         {
            FlxG.switchState(new Level9(is1player));
         }
         else if(save.data.level == 10)
         {
            FlxG.switchState(new Level10(is1player));
         }
         else if(save.data.level == 11)
         {
            FlxG.switchState(new Level11(is1player));
         }
         else if(save.data.level == 12)
         {
            FlxG.switchState(new Level12(is1player));
         }
         else if(save.data.level == 13)
         {
            FlxG.switchState(new Level13(is1player));
         }
         else if(save.data.level == 14)
         {
            FlxG.switchState(new Level14(is1player));
         }
         else if(save.data.level == 15)
         {
            FlxG.switchState(new Level15(is1player));
         }
         else if(save.data.level == 16)
         {
            FlxG.switchState(new Level16(is1player));
         }
      }
   }
}


package levels
{
   import org.flixel.*;
   
   public class Level44 extends Level
   {
      
      public var text:FlxText;
      
      public var poem:FlxText;
      
      public var fadeOut:Boolean;
      
      public function Level44()
      {
         super();
      }
      
      override public function create() : void
      {
         gameSave = new FlxSave();
         gameSave.bind("save");
         gameSave.data.level = 46;
         levelwidth = 40;
         levelheight = 30;
         scale = new FlxPoint(1,1);
         super.create();
         FlxG.bgColor = 4278190080;
         this.text = new FlxText(120,220,400,"Playtesters:\n\nTarik Kaya\nNazire Aslan\nCem Evin\nOrcun Nisli\nBurak Tezateser\nAli Bati\nUmut Dervis\nEmrah Ozer\nOzan Komurcu\nRefik Toksoy\nGokhan Yildiz\nCaglar Sahin\nMurat Kalkavan\nHande Basar\nOguzhan Tocan\nMelis Colak\nBurak Kadron\nEfe Alacamli\nOzan Yildiz\nDurmus Ali Collu\nAhmet Erdem\nYunus Emre Tekin\nCagatay Yildiz\nEmre Erdogan\nMelih Saglam\nOguz Demir\nMuhammed Cicek\nKutay Ata Sen\nBugrahan Memis\nHakan Yilmaz");
         this.text.color = 4289374890;
         this.text.alpha = 0;
         add(this.text);
         this.poem = new FlxText(25,70,400,"Hans will grow up\nBe a random guy on a street\nBut the pain in his throat\nIt\'s going to stay for a while.");
         this.poem.color = 4294967295;
         this.poem.size = 16;
         this.poem.alpha = 0;
         add(this.poem);
         player = new Hans(0,-200,scale);
         add(player);
      }
      
      override public function update() : void
      {
         super.update();
         player.y = -200;
         if(this.text.alpha < 1)
         {
            this.text.alpha += 0.01;
         }
         else if(this.text.y > -270)
         {
            this.text.y -= 0.15;
         }
         else if(this.text.y > -480)
         {
            this.text.y -= 0.15;
            if(this.poem.alpha < 1)
            {
               this.poem.alpha += 0.01;
            }
         }
         else if(this.text.y > -510)
         {
            this.text.y -= 0.2;
            if(this.poem.alpha > 0)
            {
               this.poem.alpha -= 0.01;
            }
            if(!this.fadeOut)
            {
               musicFadeOut();
               this.fadeOut = true;
            }
         }
         else
         {
            musicStop();
            this.nextLevel();
         }
         if(FlxG.keys.justPressed("ENTER"))
         {
            this.nextLevel();
         }
      }
      
      override public function nextLevel() : void
      {
         FlxG.switchState(new Menu());
      }
   }
}


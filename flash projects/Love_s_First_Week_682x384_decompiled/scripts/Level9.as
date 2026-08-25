package
{
   import org.flixel.*;
   
   public class Level9 extends State1
   {
      
      public function Level9(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         var data:Array = null;
         bgYogunluk = 3;
         talhaVar = true;
         diyalogVar = true;
         levelwidth = 15;
         levelheight = 16;
         super.create();
         save.data.level = 9;
         data = new Array(16,16,12,12,12,12,12,12,12,12,12,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,8,16,16,16,16,14,0,0,0,0,0,0,0,0,0,8,16,16,16,16,14,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,11,11,11,9,0,0,0,0,0,8,16,16,16,16,14,0,0,0,0,0,0,0,0,0,8,16,16,16,16,14,0,0,0,0,0,0,3,11,11,16,16,16,16,16,14,0,0,0,5,0,0,0,0,0,0,0,16,16,16,14,0,0,0,6,0,0,0,0,0,0,0,8,16,16,14,0,0,0,4,11,11,11,11,9,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,15,13,0,0,0,0,0,0,0,0,7,16,16,16,16,16,16,15,15,15,15,15,15,15,15,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(naz = new Naz(true,144,130 + 256));
         ondekiler.add(talha = new Naz(false,204,130 + 256));
         tas(224,384);
         talha.facing = FlxObject.LEFT;
         bulut(7);
         sarmasik(3,12,0,68 + 256);
         kapi(96,64);
         ok("left",128,288);
         ok("down",96,288);
         if(save.data.lang == "tur")
         {
            diyaloglar.push(new Diyalog(true,"Daha sevgili olalı 5 dakika olmadı, sen beni tekmeliyorsun!"));
            diyaloglar.push(new Diyalog(false,"Sen de beni tekmele, ödeselim!"));
            diyaloglar.push(new Diyalog(true,"Essek!"));
         }
         else
         {
            diyaloglar.push(new Diyalog(true,"We became lovers only less than five minutes ago, and you\'re kicking me!"));
            diyaloglar.push(new Diyalog(false,"Then kick me and we\'re even."));
            diyaloglar.push(new Diyalog(true,"Idiot!"));
         }
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level10(is1player));
      }
   }
}


package
{
   import org.flixel.*;
   
   public class Level11 extends State1
   {
      
      public function Level11(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         var data:Array = null;
         bgYogunluk = 13;
         talhaVar = true;
         diyalogVar = true;
         levelwidth = 15;
         levelheight = 16;
         super.create();
         save.data.level = 11;
         data = new Array(16,16,12,12,12,12,12,12,12,12,12,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,8,16,16,16,16,14,0,0,0,0,0,0,0,0,0,8,16,16,16,16,14,0,0,0,0,0,0,0,0,0,8,16,16,16,16,14,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,11,11,11,9,0,0,0,0,0,8,16,16,16,16,14,0,0,0,0,0,0,0,0,0,8,16,16,16,16,14,0,0,0,0,0,0,3,11,11,12,12,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,14,0,0,3,11,9,0,0,0,0,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,15,15,15,15,15,15,15,15,15,15,15,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(naz = new Naz(true,144,130 + 256));
         ondekiler.add(talha = new Naz(false,204,130 + 256));
         tas(224,384);
         tas(288,192);
         talha.facing = FlxObject.LEFT;
         bulut(5);
         sarmasik(3,12,0,68 + 256);
         kapi(96,96);
         if(save.data.lang == "tur")
         {
            diyaloglar.push(new Diyalog(true,"Canım sıkıldı."));
            diyaloglar.push(new Diyalog(false,"?"));
            diyaloglar.push(new Diyalog(true,"Cuma günü seni terkedicem."));
            diyaloglar.push(new Diyalog(false,"???????"));
         }
         else
         {
            diyaloglar.push(new Diyalog(true,"I\'m bored"));
            diyaloglar.push(new Diyalog(false,"?"));
            diyaloglar.push(new Diyalog(true,"I\'m gonna break up with you on Friday."));
            diyaloglar.push(new Diyalog(false,"???????"));
         }
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level12(is1player));
      }
   }
}


package
{
   import org.flixel.*;
   
   public class Level12 extends State1
   {
      
      public function Level12(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         var data:Array = null;
         bgYogunluk = 13;
         talhaVar = true;
         diyalogVar = true;
         levelwidth = 30;
         levelheight = 16;
         super.create();
         save.data.level = 12;
         data = new Array(16,16,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,7,15,15,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,1,0,0,0,7,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,15,11,9,0,0,0,0,0,0,0,0,7,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,8,16,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,16,16,10,0,0,0,0,0,0,7,15,15,15,16,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,4,12,12,12,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,3,9,0,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,8,13,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,15,15,15,15,15,15,13,0,0,8,16,13,0,0,0,0,0
         ,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,15,15,16,16,16,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(naz = new Naz(true,50,386));
         ondekiler.add(talha = new Naz(false,100,386));
         talha.facing = FlxObject.LEFT;
         tas(464,96);
         bulut(4);
         kapi(844,384);
         agac("2",0,75 + 256);
         sarmasik(20,1,450,356);
         if(save.data.lang == "tur")
         {
            diyaloglar.push(new Diyalog(false,"Beni terk etmemen için ne yapabilirim?"));
            diyaloglar.push(new Diyalog(true,"Tadelle ve Pınar süt alabilirsin."));
            diyaloglar.push(new Diyalog(false,"Bu beni terk etmemeni saglar mı?"));
            diyaloglar.push(new Diyalog(true,"Hayır."));
         }
         else
         {
            diyaloglar.push(new Diyalog(false,"What can I do to make you not break up with me?"));
            diyaloglar.push(new Diyalog(true,"Some chocolate and milk would be nice."));
            diyaloglar.push(new Diyalog(false,"Is that enough not to break up?"));
            diyaloglar.push(new Diyalog(true,"No."));
         }
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level13(is1player));
      }
   }
}


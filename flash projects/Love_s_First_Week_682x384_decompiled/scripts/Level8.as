package
{
   import org.flixel.*;
   
   public class Level8 extends State1
   {
      
      public function Level8(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         var data:Array = null;
         bgYogunluk = 3;
         talhaVar = true;
         diyalogVar = true;
         levelwidth = 16;
         levelheight = 16;
         super.create();
         save.data.level = 8;
         data = new Array(16,12,12,12,12,12,12,12,12,12,12,12,12,12,12,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,3,11,11,9,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,11,11,11,11,11,11,11,11,11,11,11,9,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,7,15,16,14,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,7,16,16,16,16,15,15,15,15,15,15,15,15,15,15,15,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(naz = new Naz(true,42,130 + 256));
         ondekiler.add(talha = new Naz(false,102,130 + 256));
         talha.facing = FlxObject.LEFT;
         bulut(8);
         sarmasik(5,8,0,68 + 256);
         kapi(48,128);
         tas(246,96);
         tas(352,160);
         for(var u:int = 0; u < 9; u++)
         {
            ok("right",32 + 16 * u,0);
         }
         ok("down",32 + 32 * 9,0);
         if(save.data.lang == "tur")
         {
            diyaloglar.push(new Diyalog(true,"Ikimizin degisik tekmeleriyle biz yenilmez bir takımız."));
            diyaloglar.push(new Diyalog(false,"Superiz biz!"));
         }
         else
         {
            diyaloglar.push(new Diyalog(true,"We\'re invincible with our super cool different kicks!"));
            diyaloglar.push(new Diyalog(false,"We\'re awesome!"));
         }
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level9(is1player));
      }
   }
}


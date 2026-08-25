package
{
   import org.flixel.*;
   
   public class Level14 extends State1
   {
      
      public function Level14(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         var data:Array = null;
         bgYogunluk = 14;
         talhaVar = true;
         diyalogVar = true;
         levelwidth = 15;
         levelheight = 16;
         super.create();
         save.data.level = 14;
         data = new Array(16,16,12,12,12,12,12,12,12,12,12,12,12,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,14,0,0,0,3,11,11,11,9,0,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,15,15,15,15,15,15,15,15,15,15,15,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(naz = new Naz(true,144,130 + 256));
         ondekiler.add(talha = new Naz(false,204,130 + 256));
         talha.facing = FlxObject.LEFT;
         bulut(2);
         sarmasik(6,6,0,58 + 256);
         for(var u:int = 0; u < 6; u++)
         {
            agac("0",32 + 64 * u,350);
            agac("1",32 + 64 * u + 16,350);
            agac("2",32 + 64 * u + 32,350);
            agac("3",32 + 64 * u + 48,350);
         }
         kapi(208,96);
         ok("up",64,384);
         ok("up",384,384);
         ok("left",384,32);
         ok("right",64,32);
         if(save.data.lang == "tur")
         {
            diyaloglar.push(new Diyalog(false,"Terk etme olayı saka, degil mi?"));
            diyaloglar.push(new Diyalog(true,"Neden?"));
            diyaloglar.push(new Diyalog(false,"Cunku tam belli olmuyor saka olup olmadıgı."));
            diyaloglar.push(new Diyalog(true,"Sence?"));
            diyaloglar.push(new Diyalog(false,"Saka."));
            diyaloglar.push(new Diyalog(true,"..."));
            diyaloglar.push(new Diyalog(false,"Degil."));
            diyaloglar.push(new Diyalog(true,"..."));
            diyaloglar.push(new Diyalog(false,"..."));
         }
         else
         {
            diyaloglar.push(new Diyalog(false,"The breaking up thing.. It\'s a joke, right?"));
            diyaloglar.push(new Diyalog(true,"Why?"));
            diyaloglar.push(new Diyalog(false,"Because I can\'t tell if it\'s a joke or not!"));
            diyaloglar.push(new Diyalog(true,"What do you think?"));
            diyaloglar.push(new Diyalog(false,"Yes?"));
            diyaloglar.push(new Diyalog(true,"..."));
            diyaloglar.push(new Diyalog(false,"No?"));
            diyaloglar.push(new Diyalog(true,"..."));
            diyaloglar.push(new Diyalog(false,"..."));
         }
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level15(is1player));
      }
   }
}


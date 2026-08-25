package
{
   import org.flixel.*;
   
   public class Level7 extends State1
   {
      
      public function Level7(_is1player:Boolean)
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
         levelheight = 8;
         super.create();
         save.data.level = 7;
         data = new Array(14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,5,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,6,0,0,0,8,14,0,0,0,0,0,1,0,0,0,0,6,0,0,0,8,16,15,15,15,15,15,15,15,15,15,15,16,15,15,15,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(naz = new Naz(true,42,130));
         ondekiler.add(talha = new Naz(false,102,130));
         talha.facing = FlxObject.LEFT;
         bulut(9);
         sarmasik(5,8,0,68);
         kapi(400,96);
         tas(130,128);
         for(var u:int = 0; u < 9; u++)
         {
            ok("right",32 + 16 * u,0);
         }
         ok("down",32 + 32 * 9,0);
         if(save.data.lang == "tur")
         {
            diyaloglar.push(new Diyalog(true,"Simdi sevgili olduk ama burayı nasıl gecicez?"));
            diyaloglar.push(new Diyalog(false,"Benim tekmem çok degisik bak izle simdi."));
         }
         else
         {
            diyaloglar.push(new Diyalog(true,"Now we\'re lovers! But how will we climb up there?"));
            diyaloglar.push(new Diyalog(false,"My kick is super cool, just watch!"));
         }
      }
      
      override public function update() : void
      {
         super.update();
         if(naz.x > levelwidth * 32 - 17)
         {
            super.nextLevel();
            FlxG.switchState(new LevelSecret1(is1player));
         }
         else if(talha.x > levelwidth * 32 - 17)
         {
            super.nextLevel();
            FlxG.switchState(new LevelSecret1(is1player));
         }
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level8(is1player));
      }
   }
}


package
{
   import org.flixel.*;
   
   public class Level4 extends State1
   {
      
      public function Level4(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         bgYogunluk = 1;
         levelwidth = 18;
         levelheight = 8;
         super.create();
         save.data.level = 4;
         var data:Array = new Array(14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,7,15,15,8,8,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,8,8,16,14,0,0,0,0,0,0,0,0,0,0,0,8,16,16,8,8,16,16,15,15,15,15,15,15,15,15,15,15,15,16,16,16,8,8,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(naz = new Naz(true,64,194));
         bulut(12);
         kapi(464,64);
         sarmasik(3,8,32,132);
         ok("up",11 * 32,128);
         ok("left",11 * 32,0);
         ok("down",32,0);
         ok("right",32,96);
         ok("up",10 * 32,96);
         ok("left",10 * 32,32);
         ok("down",64,32);
         ok("right",64,64);
      }
      
      override public function update() : void
      {
         super.update();
         if(naz.x > levelwidth * 32 - 32)
         {
            naz.x = levelwidth * 32 - 32;
         }
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level5(is1player));
      }
   }
}


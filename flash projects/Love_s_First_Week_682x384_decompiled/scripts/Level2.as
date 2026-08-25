package
{
   import org.flixel.*;
   
   public class Level2 extends State1
   {
      
      private static var img:Class = Level2_img;
      
      public function Level2(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         var data:Array = null;
         bgYogunluk = 0;
         levelwidth = 16;
         levelheight = 8;
         super.create();
         save.data.level = 2;
         data = new Array(16,12,12,12,12,16,16,12,12,12,12,12,12,12,12,16,14,0,0,0,0,8,14,0,0,0,0,0,0,0,0,8,14,0,0,0,0,8,14,0,0,0,0,0,0,0,0,8,14,0,0,0,0,8,14,0,0,0,0,0,0,0,0,8,14,0,0,0,0,4,10,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,15,15,15,15,15,15,15,13,0,0,7,15,15,15,16,16,16,16,16,16,16,16,16,14,0,0,8,16,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(naz = new Naz(true,42,162));
         taslar.add(new Tas(168,164));
         bulut(14);
         sarmasik(11,1,100,100);
         agac("2",50,75);
         kapi(376,128);
         enOndekiler.add(new FlxSprite(128,200,img));
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level3(is1player));
      }
   }
}


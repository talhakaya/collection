package
{
   import org.flixel.*;
   
   public class Level3 extends State1
   {
      
      private static var img:Class = Level3_img;
      
      public function Level3(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         var data:Array = null;
         bgYogunluk = 1;
         levelwidth = 16;
         levelheight = 8;
         super.create();
         save.data.level = 3;
         data = new Array(14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,5,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,6,0,0,0,0,8,14,0,0,3,11,9,0,0,0,0,6,0,0,0,0,8,14,0,0,0,0,0,0,0,0,7,14,0,0,0,0,8,14,0,0,0,0,0,0,0,0,8,14,0,0,0,0,8,14,0,0,0,0,0,0,0,0,8,16,15,15,15,15,16,16,15,15,15,15,15,15,15,15,16,16,16,16,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(naz = new Naz(true,42,194));
         taslar.add(new Tas(140,196));
         taslar.add(new Tas(136,68));
         bulut(13);
         sarmasik(11,3,100,132);
         agac("1",50,107);
         kapi(388,128);
         enOndekiler.add(new FlxSprite(42,232,img));
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level4(is1player));
      }
   }
}


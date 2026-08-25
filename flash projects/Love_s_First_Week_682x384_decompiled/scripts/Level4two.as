package
{
   import org.flixel.*;
   
   public class Level4two extends State1
   {
      
      private static var img1:Class = Level4two_img1;
      
      private static var img2:Class = Level4two_img2;
      
      public function Level4two(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         var data:Array = null;
         controlNaz = false;
         bgYogunluk = 3;
         talhaVar = true;
         levelwidth = 15;
         levelheight = 8;
         super.create();
         save.data.level = 4;
         data = new Array(14,0,0,0,0,0,0,6,0,0,0,0,0,8,16,14,0,0,0,0,0,0,6,0,0,0,0,0,8,16,14,0,0,0,0,0,0,6,0,0,0,0,0,8,16,16,11,11,11,13,0,0,8,11,15,9,0,0,8,16,14,0,0,0,6,0,0,6,0,6,0,0,7,16,16,14,0,0,0,2,0,7,14,0,2,0,0,8,16,16,14,0,0,0,0,0,8,14,0,0,0,0,8,16,16,16,15,15,15,15,15,16,16,15,15,15,15,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(talha = new Naz(false,64,130 + 64));
         ondekiler.add(naz = new Naz(true,256,130 + 64));
         ondekiler.add(new FlxSprite(158,228,img1));
         ondekiler.add(new FlxSprite(296,228,img2));
         tas(160,192);
         tas(288,192);
         bulut(14);
         sarmasik(3,12,0,68 + 64);
         kapi(40,32);
         kapi2(264,32);
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level5two(is1player));
      }
   }
}


package
{
   import org.flixel.*;
   
   public class Level3two extends State1
   {
      
      private static var img1:Class = Level3two_img1;
      
      private static var img2:Class = Level3two_img2;
      
      public function Level3two(_is1player:Boolean)
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
         save.data.level = 3;
         data = new Array(14,0,0,0,0,0,0,6,0,0,0,0,0,8,16,14,0,0,0,0,0,0,6,0,0,0,0,0,8,16,14,0,0,0,0,0,0,6,0,0,0,0,0,8,16,16,11,11,9,0,0,0,8,11,11,9,0,0,8,16,14,0,0,0,0,0,0,6,0,0,0,0,0,8,16,14,0,0,0,0,0,7,14,0,0,0,0,7,16,16,14,0,0,0,0,0,8,14,0,0,0,0,8,16,16,16,15,15,15,15,15,16,16,15,15,15,15,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(talha = new Naz(false,64,130 + 64));
         ondekiler.add(naz = new Naz(true,256,130 + 64));
         ondekiler.add(new FlxSprite(48,228,img2));
         ondekiler.add(new FlxSprite(240,228,img1));
         bulut(15);
         sarmasik(12,3,0,68 + 64);
         agac("0",100,100);
         kapi(40,32);
         kapi2(264,32);
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level4two(is1player));
      }
   }
}


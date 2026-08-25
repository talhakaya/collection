package
{
   import org.flixel.*;
   
   public class Level5two extends State1
   {
      
      private static var img:Class = Level5two_img;
      
      public function Level5two(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         var data:Array = null;
         bgYogunluk = 3;
         talhaVar = true;
         levelwidth = 15;
         levelheight = 12;
         super.create();
         save.data.level = 5;
         data = new Array(14,0,0,0,0,0,0,6,0,0,0,0,0,8,16,14,0,0,0,0,0,0,6,0,0,0,0,0,8,16,14,0,0,0,0,0,0,6,0,0,0,0,0,8,16,16,11,11,11,11,9,0,8,11,11,9,0,0,8,16,14,0,0,0,0,0,0,6,0,0,0,0,0,8,16,14,0,0,0,0,0,0,6,0,0,3,11,11,16,16,14,0,0,0,0,0,3,14,0,0,0,0,0,8,16,16,11,11,9,0,0,0,8,11,9,0,0,0,8,16,14,0,0,0,0,0,0,6,0,0,0,0,0,8,16,14,0,0,0,0,0,0,6,0,0,0,0,0,8,16,14,0,0,0,0,0,0,6,0,0,0,0,0,8,16,16,15,15,15,15,15,15,16,15,15,15,15,15,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(talha = new Naz(false,64,66));
         ondekiler.add(new FlxSprite(58,98,img));
         ondekiler.add(naz = new Naz(true,256,130 + 192));
         ondekiler.add(new FlxSprite(250,354,img));
         tas(224,384);
         bulut(13);
         agac("3",150,228);
         agac("2",300,228);
         kapi(48,288);
         kapi2(264,32);
         ok("left",384,192);
         ok("up",384,256);
         ok("down",192,64);
         ok("left",192,128);
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level6(is1player));
      }
   }
}


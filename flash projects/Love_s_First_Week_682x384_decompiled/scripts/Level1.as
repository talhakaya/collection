package
{
   import org.flixel.*;
   
   public class Level1 extends State1
   {
      
      private static var img1:Class = Level1_img1;
      
      private static var img2:Class = Level1_img2;
      
      private static var img3:Class = Level1_img3;
      
      private static var img4:Class = Level1_img4;
      
      public function Level1(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         var data:Array = null;
         bgYogunluk = 0;
         levelwidth = 30;
         levelheight = 8;
         super.create();
         save.data.level = 1;
         data = new Array(14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,13,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,13,0,0,0,7,16,16,13,0,0,0,0,0,8,16,15,15,15,15,15,15,15,15,15,15,15,15,15,15,16,16,15,15,15,16,16,16,16,15,15,15,15,15,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(naz = new Naz(true,50,162));
         enOndekiler.add(new FlxSprite(16,200,img1));
         enOndekiler.add(new FlxSprite(64,200,img2));
         taslar.add(new Tas(208,164));
         bulut(15);
         sarmasik(30,2,16,100);
         agac("0",120,75);
         agac("1",260,75);
         agac("2",300,75);
         agac("3",420,75);
         kapi(844,128);
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level2(is1player));
      }
   }
}


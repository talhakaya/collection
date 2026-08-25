package
{
   import org.flixel.*;
   
   public class Level15 extends State1
   {
      
      public function Level15(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         var data:Array = null;
         bgYogunluk = 15;
         talhaVar = true;
         diyalogVar = true;
         levelwidth = 30;
         levelheight = 16;
         super.create();
         save.data.level = 15;
         data = new Array(16,12,12,12,12,12,12,12,12,12,12,12,12,12,12,16,12,12,16,12,12,12,12,12,12,12,12,12,12,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,6,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,3,13,0,0,0,0,0,0,0,4,13,0,0,3,11,13,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,8,11,11,11,11,9,0,0,0,6,0,0,0,0,6,0,0,0,0,8,14,0,0,0,0,0,3,9,0,0,6,0,0,0,0,0,0,0,0,8,13,0,0,0,4,11,11,11,11,16,14,0,0,0,0,0,0,0,0,0,6,0,0,0,0,0,0,3,11,16,14,0,0,0,0,0,0,0,0,8,14,0,0,7,11,13,0,0,3,15,14,0,0,0,0,0,0,0,0,4,12,13,0,0,0,0,0,0,0,8,16,11,11,10,0,4,9,0,0,4,14,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,8,11,11,11,11,11,11,11,13,0,0,0,0,0,7,15,15,15,15,16,14,0,0,0,0,3,11,9,0,0,6,0,0,0,0,0,0,0,4,11,13,0,0,0,8,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,4,9,0,0,8,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,16,16,16,16,16,16,16,16,16,16
         ,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(naz = new Naz(true,50,386));
         ondekiler.add(talha = new Naz(false,100,386));
         talha.facing = FlxObject.LEFT;
         tas(304,32);
         ok("right",352,224);
         bulut(1);
         kapi(830,64);
         sarmasik(40,1,150,356);
         if(save.data.lang == "tur")
         {
            diyaloglar.push(new Diyalog(true,"Sona yaklasıyoruz artık."));
            diyaloglar.push(new Diyalog(false,"Bana bunu neden yapıyorsun?"));
            diyaloglar.push(new Diyalog(true,"Çok eglenceli çünkü!"));
            diyaloglar.push(new Diyalog(false,"..."));
            diyaloglar.push(new Diyalog(true,"Noldu? Üzülüyor musun? Inanıyor musun gerçekten söylediklerime?"));
            diyaloglar.push(new Diyalog(false,"..."));
         }
         else
         {
            diyaloglar.push(new Diyalog(true,"We\'re close to the end now..."));
            diyaloglar.push(new Diyalog(false,"Why are you doing this to me?"));
            diyaloglar.push(new Diyalog(true,"Because it\'s so much fun!"));
            diyaloglar.push(new Diyalog(false,"..."));
            diyaloglar.push(new Diyalog(true,"Hey, are you sad? Do you really believe what I\'m saying?"));
            diyaloglar.push(new Diyalog(false,"..."));
         }
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level16(is1player));
      }
   }
}


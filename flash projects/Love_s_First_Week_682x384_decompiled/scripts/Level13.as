package
{
   import org.flixel.*;
   
   public class Level13 extends State1
   {
      
      public function Level13(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         var data:Array = null;
         var u:int = 0;
         bgYogunluk = 14;
         talhaVar = true;
         diyalogVar = true;
         levelwidth = 30;
         levelheight = 8;
         super.create();
         save.data.level = 13;
         data = new Array(16,12,12,12,16,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,16,14,0,0,0,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,3,11,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,5,0,0,0,0,0,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,2,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,15,15,15,15,15,15,15,15,15,15,15,15,15,16,16,15,15,15,15,13,0,0,0,0,0,0,0,7,15,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(naz = new Naz(true,72,66));
         ondekiler.add(talha = new Naz(false,104,66));
         talha.facing = FlxObject.LEFT;
         for(tas(574,160); u < 10; )
         {
            ok("up",216 + u * 16,128);
            u++;
         }
         bulut(13);
         kapi(844,128);
         agac("2",0,100);
         sarmasik(20,1,410,132);
         if(save.data.lang == "tur")
         {
            diyaloglar.push(new Diyalog(false,"Biliyor musun, ben normalde gözlük takıyorum."));
            diyaloglar.push(new Diyalog(true,"Aa, peki simdi niye takmıyorsun?"));
            diyaloglar.push(new Diyalog(false,"Bilmem."));
            diyaloglar.push(new Diyalog(true,"..."));
         }
         else
         {
            diyaloglar.push(new Diyalog(false,"Did you know that I wear glasses, in real life?"));
            diyaloglar.push(new Diyalog(true,"Oh, why don\'t you wear them in the game?"));
            diyaloglar.push(new Diyalog(false,"Dunno."));
            diyaloglar.push(new Diyalog(true,"..."));
         }
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level14(is1player));
      }
   }
}


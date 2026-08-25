package
{
   import org.flixel.*;
   
   public class Level6 extends State1
   {
      
      private static var img:Class = Level6_img;
      
      public function Level6(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         var data:Array = null;
         bgYogunluk = 2;
         talhaVar = true;
         diyalogVar = true;
         levelwidth = 16;
         levelheight = 8;
         super.create();
         save.data.level = 6;
         data = new Array(16,12,12,12,12,16,16,12,12,12,12,12,12,12,12,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,15,15,15,15,15,15,15,13,1,1,7,15,15,15,16,16,15,15,15,15,15,15,15,13,1,1,7,15,15,15,16,16,16,16,16,16,16,16,16,14,1,1,8,16,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(naz = new Naz(true,42,130));
         ondekiler.add(talha = new Naz(false,102,130));
         talha.facing = FlxObject.LEFT;
         bulut(10);
         sarmasik(41,1,0,100);
         agac("2",50,43);
         agac("1",150,43);
         agac("3",190,43);
         agac("4",250,43);
         agac("1",350,43);
         kapi(400,96);
         if(is1player)
         {
            enOndekiler.add(new FlxSprite(92,192,img));
         }
         if(save.data.lang == "tur")
         {
            diyaloglar.push(new Diyalog(false,"Merabaa"));
            diyaloglar.push(new Diyalog(true,"Selam, tanısalım mı?"));
            diyaloglar.push(new Diyalog(false,"Olur, ben Talha."));
            diyaloglar.push(new Diyalog(true,"Ben de " + NAZIRE + "."));
            diyaloglar.push(new Diyalog(false,"Hadi su deposuna gidelim."));
            diyaloglar.push(new Diyalog(true,"Yanlıslıkla sevgili falan olmayalım sonra?"));
            diyaloglar.push(new Diyalog(false,"Kısmet."));
         }
         else
         {
            diyaloglar.push(new Diyalog(false,"Hellooo"));
            diyaloglar.push(new Diyalog(true,"Hey, let\'s introduce ourselves!"));
            diyaloglar.push(new Diyalog(false,"Okay, I\'m Talha."));
            diyaloglar.push(new Diyalog(true,"And I\'m " + NAZIRE + "."));
            diyaloglar.push(new Diyalog(false,"Let\'s go to that place with the nice view!"));
            diyaloglar.push(new Diyalog(true,"But what if we accidently become lovers?"));
            diyaloglar.push(new Diyalog(false,"We\'ll see."));
         }
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level7(is1player));
      }
   }
}


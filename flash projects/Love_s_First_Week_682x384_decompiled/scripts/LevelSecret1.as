package
{
   import org.flixel.*;
   
   public class LevelSecret1 extends State1
   {
      
      private static var img:Class = LevelSecret1_img;
      
      public function LevelSecret1(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         var data:Array = null;
         controlNaz = false;
         diyalogVar = true;
         bgYogunluk = 3;
         talhaVar = true;
         levelwidth = 15;
         levelheight = 8;
         super.create();
         data = new Array(14,0,0,0,0,0,0,0,0,0,0,0,0,8,16,14,0,0,0,0,0,0,0,0,0,0,0,0,8,16,14,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,15,15,15,15,15,15,15,15,15,15,15,15,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(talha = new Naz(false,64,66));
         ondekiler.add(naz = new Naz(true,40,66));
         kapi(320,32);
         ondekiler.add(new FlxSprite(148,102,img));
         if(save.data.lang == "tur")
         {
            diyaloglar.push(new Diyalog(false,"Oha burası neresi?"));
            diyaloglar.push(new Diyalog(true,"Uu sanırım gizli bir oda bulduk."));
            diyaloglar.push(new Diyalog(false,"Yoksa bunlar da bizim gercek halimiz mi?"));
            diyaloglar.push(new Diyalog(true,"Yok artık!"));
            diyaloglar.push(new Diyalog(false,"Vay be!"));
         }
         else
         {
            diyaloglar.push(new Diyalog(false,"What? Where are we?"));
            diyaloglar.push(new Diyalog(true,"Oh, I think we found a secret room!"));
            diyaloglar.push(new Diyalog(false,"Are these us in real life?"));
            diyaloglar.push(new Diyalog(true,"Whoa!"));
            diyaloglar.push(new Diyalog(false,"Wow!"));
         }
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Level8(is1player));
      }
   }
}


package levels
{
   import org.flixel.*;
   
   public class Level26 extends Level
   {
      
      private static var S_tiles:Class = Level26_S_tiles;
      
      private var greyText:FlxText;
      
      private var greyText2:FlxText;
      
      private var greyText6:FlxText;
      
      private var greyText3:FlxText;
      
      private var greyText4:FlxText;
      
      private var greyText5:FlxText;
      
      private var textPut:Boolean;
      
      private var textPut2:Boolean;
      
      private var countTime:int;
      
      private var count:int;
      
      public function Level26()
      {
         super();
      }
      
      override public function create() : void
      {
         gameSave = new FlxSave();
         gameSave.bind("save");
         gameSave.data.level = 27;
         musicStop();
         levelwidth = 40;
         levelheight = 30;
         scale = new FlxPoint(8,8);
         super.create();
         FlxG.bgColor = 4290199552;
         var data:Array = new Array(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,12,12,16,12,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,0,0,6,0,11,15,11,11,15,11,11,15,11,11,15,11,11,15,11,11,15,11,11,15,11,11,15,11,11,15,11,13,0,0,0,0,0,0,0,8,11,15,12,11,0,6,0,0,6,0,0,6,0,0,6,0,0,6,0,0,6,0,0,6,0,0,6,0,0,6,0,6,0,0,0,0,0,0,0,6,0,6,0,0,15,12,11,15,12,11,15,12,11,15,12,11,15,12,11,15,12,11,15,12,11,15,12,11,15,12,11,14,0,0,0,0,0,0,0,8,11,12,15,11,14,0,0,6,0,0,6,0,0,6,0,0,6,0,0,6,0,0,6,0,0,6,0,0,6,0,0,6,0,0,0,0,0,0,0,6,0,0,6,0,16,11,11,12,11,11,12,11,11,12,11,11,12,11,11,12,11,11,12,11,11,12,11,11,12,11,11,10,0,0,0,0,0,0,0,8,11,15,12,11,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,0,6,0,0,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,11,12,15,11,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,0,0,6,0,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
         ,0,0,8,11,15,12,11,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,0,6,0,0,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,11,12,15,11,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,0,0,6,0,14,0,0,0,0,0,0,0,7,11,11,11,11,11,11,13,0,7,11,11,11,11,11,11,13,0,7,11,11,11,11,11,11,13,0,8,11,15,12,11,14,0,0,0,0,0,0,0,6,0,0,0,0,0,0,6,0,6,0,0,0,0,0,0,6,0,6,0,0,0,0,0,0,6,0,6,0,6,0,0,14,0,0,0,0,0,0,0,6,0,0,0,0,0,0,6,0,6,0,0,0,0,0,0,6,0,6,0,0,0,0,0,0,6,0,8,11,12,15,11,14,0,0,0,0,0,0,0,4,15,11,11,11,11,15,10,0,4,15,11,11,11,11,15,10,0,4,15,11,11,11,11,15,10,0,6,0,0,6,0,14,0,0,0,0,0,0,0,0,6,0,0,0,0,6,0,0,0,6,0,0,0,0,6,0,0,0,6,0,0,0,0,6,0,0,8,11,15,12,11,14,0,0,0,0,0,0,0,0,6,0,0,0,0,6,0,0,0,6,0,0,0,0,6,0,0,0,6,0,0,0,0,6,0,0,6,0,6,0,0,14,0,0,0,0,0,0,0,0,6,0,0,0,0,6,0,0,0,6,0,0,0,0,6,0,0,0,6,0,0,0,0,6,0,0,8,11,12,15,11,14,0,0,0,0,0,0,0,0,6,0,0,0,0,6,0,0,0,6,0,0,0,0,6,0,0,0,6,0,0,0,0,6,0,0,6,0,0,6,0,14,0,0,0,0,0,0,0,0,6,0,0,0,0,6
         ,0,0,0,6,0,0,0,0,6,0,0,0,6,0,0,0,0,6,0,0,8,11,15,12,11,14,0,0,0,0,0,0,0,0,6,0,0,0,0,6,0,0,0,6,0,0,0,0,6,0,0,0,6,0,0,0,0,6,0,0,6,0,6,0,0,14,0,0,0,0,0,0,0,0,6,0,0,0,0,6,0,0,0,6,0,0,0,0,6,0,0,0,6,0,0,0,0,6,0,0,8,15,16,15,15,16,15,11,15,15,11,15,15,11,16,15,11,15,15,12,15,15,11,16,15,11,15,15,12,15,15,11,16,15,11,15,15,12,15,15,12,16,16,12,16,16,10,0,4,10,0,4,10,0,4,10,0,4,10,0,4,10,0,4,10,0,4,10,0,4,10,0,4,10,0,4,10,0,4,10,0,4,10,0,8,14,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,0,7,16,14,0,4,9,0,4,9,0,4,9,0,4,9,0,4,9,0,4,9,0,4,9,0,4,9,0,4,9,0,4,9,0,4,9,0,4,9,0,4,16,16,13,0,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,0,5,0,8);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,40),S_tiles,0,0,FlxTilemap.AUTO);
         add(level);
         interacts.add(new Door2(16,160,false,scale));
         this.countTime = 150;
         this.count = 0;
         this.greyText = new FlxText(8,-2,300,"  HANS                                               WORLD             TIME",true);
         this.greyText.color = 4286019447;
         add(this.greyText);
         this.greyText2 = new FlxText(8,6,300,"999999                   0 x 99                 9 - 2                " + this.countTime,true);
         this.greyText2.color = 4286019447;
         add(this.greyText2);
         this.greyText3 = new FlxText(92,56,300,"WELCOME TO WARP ZONE!",true);
         this.greyText3.color = 4286019447;
         add(this.greyText3);
         this.greyText6 = new FlxText(8,96,300,"Your way            4                      3                      2",true);
         this.greyText6.color = 4286019447;
         add(this.greyText6);
         this.greyText4 = new FlxText(64,70,300,"YOU CAN\'T PASS YOUR PROBLEMS BY, HANS!",true);
         this.greyText4.color = 4294910498;
         this.greyText4.alpha = 0;
         add(this.greyText4);
         this.greyText5 = new FlxText(11,82,300,"You thought something cool would happen, didn\'t you?",true);
         this.greyText5.color = 4281575987;
         this.greyText5.alpha = 0;
         add(this.greyText5);
         this.textPut = false;
         this.textPut2 = false;
         player = new Hans(0,-16,scale);
         add(player);
      }
      
      override public function update() : void
      {
         super.update();
         ++this.count;
         if(this.count >= 60)
         {
            this.count = 0;
            --this.countTime;
            if(this.countTime == 0)
            {
               this.textPut2 = true;
            }
         }
         if((FlxG.keys.DOWN || FlxG.keys.S) && !this.textPut)
         {
            this.textPut = true;
         }
         if(this.greyText4.alpha < 1 && this.textPut)
         {
            this.greyText4.alpha += 0.02;
         }
         if(this.greyText5.alpha < 1 && this.textPut2)
         {
            this.greyText5.alpha += 0.02;
         }
         this.greyText2.text = "999999                   0 x 99                 9 - 2                " + this.countTime;
         if(player.x < 0)
         {
            player.x = 0;
         }
         else if(player.x > 256)
         {
            player.x = 256;
         }
      }
      
      override public function getWell() : void
      {
         FlxG.bgColor = 4290199552;
      }
      
      override public function nextLevel() : void
      {
         FlxG.switchState(new Level27());
      }
   }
}


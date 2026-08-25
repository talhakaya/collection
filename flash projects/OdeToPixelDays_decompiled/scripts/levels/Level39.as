package levels
{
   import org.flixel.*;
   
   public class Level39 extends Level
   {
      
      private static var S_tiles:Class = Level39_S_tiles;
      
      private static var S_kutufirlatan:Class = Level39_S_kutufirlatan;
      
      private static var Sfxhurt:Class = Level39_Sfxhurt;
      
      private static var Sfxkutuol:Class = Level39_Sfxkutuol;
      
      private var giderayak:MonsterHans;
      
      private var camera:FlxSprite;
      
      private var abstractLever:Lever;
      
      private var abstractLever2:Lever;
      
      private var leverHapis:Lever;
      
      private var leverHapisIsOpen:Boolean;
      
      private var block1:Block;
      
      private var block2:Block;
      
      private var blockHapis:Block;
      
      private var boss:MonsterHans;
      
      private var lever2light1:Lever2;
      
      private var lever2light2:Lever2;
      
      private var lever2light3:Lever2;
      
      private var lever2light4:Lever2;
      
      private var bossIsTrapped:Boolean;
      
      private var light1:Lightbulb;
      
      private var light2:Lightbulb;
      
      private var light3:Lightbulb;
      
      private var light4:Lightbulb;
      
      private var pattern1Complete:Boolean;
      
      private var pattern2Complete:Boolean;
      
      private var pattern3Complete:Boolean;
      
      private var pattern4Complete:Boolean;
      
      private var pattern5Complete:Boolean;
      
      private var pattern6Complete:Boolean;
      
      private var patternMatcher:PatternMatcher;
      
      public var i:int;
      
      private var denemeMonster:Monster;
      
      private var bossIsDead:Boolean;
      
      private var poet:Array;
      
      public function Level39()
      {
         super();
      }
      
      override public function create() : void
      {
         var data:Array = null;
         gameSave = new FlxSave();
         gameSave.bind("save");
         gameSave.data.level = 42;
         if(FlxG.music != null)
         {
            FlxG.music.volume = 0.2;
         }
         levelwidth = 160;
         levelheight = 30;
         scale = new FlxPoint(2,2);
         super.create();
         bgObjects.add(new Door(48,24,true,scale));
         FlxG.bgColor = 4288900388;
         data = new Array(16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
         ,0,0,4,12,12,12,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,12,12,12,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,12,12,12,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16
         ,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,12,12,12,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,12,12,12,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,12,12,12,16,16,16,16,16,16,16,16
         ,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,15,15,15,15,15,15,15,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,12,12,12,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,12,12,12,12,12,12,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,12,12,12,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
         ,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,12,12,12,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,12,12,12,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,12,12,12,12,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,14,0,0,0,0,0,0,4,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12
         ,12,12,12,16,16,16,16,15,15,15,15,15,15,15,15,15,15,15,15,13,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,15,15,15,15,15,13,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,15,15,15,15,15,15,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,12,12,12,12,12,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,12,12,12,12,12,12,12,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
         ,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,15,15,13,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
         ,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,15,15,13,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,15,15,13,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,12,12,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,14,0,0
         ,8,13,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,15,15,13,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,10,0,0,4,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
         ,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,13,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16
         ,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,13,0,0,0,0,7,15,15,15,15,15,15,15,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,14,0,0,0,0,8,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16
         ,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,15,15,15,15,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,160),S_tiles,0,0,FlxTilemap.AUTO);
         add(level);
         interacts.add(new Door(1208,120,false,scale));
         player = new Hans(64,72,scale);
         add(player);
         this.abstractLever = new Lever(0,0,false,scale,false);
         this.abstractLever.alpha = 0;
         add(this.abstractLever);
         this.abstractLever2 = new Lever(0,0,false,scale,false);
         this.abstractLever2.alpha = 0;
         add(this.abstractLever2);
         this.leverHapis = new Lever(1032,24,false,scale,false);
         interacts.add(this.leverHapis);
         this.leverHapisIsOpen = false;
         this.block1 = new Block(744,200,scale,1,false,this.abstractLever,false);
         blocks.add(this.block1);
         this.block2 = new Block(1072,200,scale,2,false,this.abstractLever2,false);
         blocks.add(this.block2);
         this.blockHapis = new Block(976,216,scale,3,false,this.leverHapis,false);
         blocks.add(this.blockHapis);
         this.lever2light1 = new Lever2(816,72,false,scale,false);
         this.lever2light2 = new Lever2(942,72,false,scale,false);
         this.lever2light3 = new Lever2(840,184,false,scale,false);
         this.lever2light4 = new Lever2(918,184,false,scale,false);
         interacts.add(this.lever2light1);
         interacts.add(this.lever2light2);
         interacts.add(this.lever2light3);
         interacts.add(this.lever2light4);
         this.light1 = new Lightbulb(778,32,scale,this.lever2light1,1);
         this.light2 = new Lightbulb(950,32,scale,this.lever2light2,2);
         this.light3 = new Lightbulb(802,144,scale,this.lever2light3,3);
         this.light4 = new Lightbulb(926,144,scale,this.lever2light4,4);
         bgObjects.add(this.light1);
         bgObjects.add(this.light2);
         bgObjects.add(this.light3);
         bgObjects.add(this.light4);
         this.boss = new MonsterHans(960,100,scale,39,player);
         monsters.add(this.boss);
         narrators.add(new Narrator(96,40,190,62,false));
         var narrator2:Narrator = new Narrator(1088,128,90,70,true);
         narrators.add(narrator2);
         narrator = new Narrator(832,72,200,63,true);
         narrators.add(narrator);
         narrators.add(new NarratorTouch(864,0,narrator));
         narrators.add(new NarratorTouch(1080,0,narrator2));
         var kutufirlatan:FlxSprite = new FlxSprite(1028,80,S_kutufirlatan);
         kutufirlatan.scale = scale;
         bgObjects.add(kutufirlatan);
         this.pattern1Complete = false;
         this.pattern2Complete = false;
         this.pattern3Complete = false;
         this.pattern4Complete = false;
         this.pattern5Complete = false;
         this.pattern6Complete = false;
         this.patternMatcher = new PatternMatcher(this.pattern1Complete,this.pattern2Complete,this.pattern3Complete,this.pattern4Complete,this.pattern5Complete,this.pattern6Complete,this.light1,this.light2,this.light3,this.light4,this.boss);
         add(this.patternMatcher);
         this.camera = new FlxSprite(0,0);
         this.camera.alpha = 0;
         add(this.camera);
         this.lever2sNotUsable();
         FlxG.camera.setBounds(0,0,1280,240,true);
         FlxG.camera.follow(this.camera,FlxCamera.STYLE_LOCKON);
         this.poet = new Array();
         for(this.i = 0; this.i < 7; ++this.i)
         {
            this.poet.push(new FlxText(1120,6 + this.i * 12,200,"",true));
            this.poet[this.i].color = 1715749956;
            add(this.poet[this.i]);
         }
         this.poet[0].text = "Know your instincts";
         this.poet[1].text = "Take control";
         this.poet[2].text = "You\'re what you are";
         this.poet[3].text = "Find your way";
         this.poet[4].text = "Don\'t expect from others";
         this.poet[5].text = "You\'re one and only";
         this.poet[6].text = "Explore yourself";
      }
      
      override public function update() : void
      {
         var _m:Monster = null;
         super.update();
         FlxG.collide(blocks,level,this.kutuOl);
         FlxG.collide(player,blocks);
         FlxG.collide(blocks,monsters,this.kutuOl2);
         if(this.patternMatcher.kutuFirlat)
         {
            this.patternMatcher.kutuFirlat = false;
            blocks.add(new BlockFalling(1020,96,scale,false,true));
            blocks.add(new BlockFalling(1036,96,scale,false,true));
            blocks.add(new BlockFalling(1052,96,scale,false,true));
            --this.boss.hp;
            ++narrator.line;
            if(this.boss.hp == 0)
            {
               new FlxTimer().start(1,1,this.bossDeath);
            }
         }
         if(this.patternMatcher.yanlisBildin)
         {
            this.patternMatcher.yanlisBildin = false;
            this.leverHapis.isOpen = false;
            this.lever2sNotUsable();
         }
         if(this.patternMatcher.dogruBildin)
         {
            this.patternMatcher.dogruBildin = false;
            this.lever2sNotUsable();
         }
         if(this.patternMatcher.cevapver)
         {
            this.patternMatcher.cevapver = false;
            this.lever2sUsable();
         }
         if((!this.leverHapis.isOpen || this.boss.x < 1000) && this.lever2light1.usable && !this.bossIsDead)
         {
            this.lever2sNotUsable();
         }
         if(this.patternMatcher.canavarGonder)
         {
            this.patternMatcher.canavarGonder = false;
            for(this.i = 1; this.i < 3; ++this.i)
            {
               if(Math.random() < 0.2)
               {
                  monsters.add(new MonsterGoomba(800 + this.i * 60,-64,scale));
               }
               else
               {
                  monsters.add(new MonsterGel(800 + this.i * 60,-64,scale));
               }
            }
         }
         if(!this.abstractLever.isOpen && player.x >= 776)
         {
            this.abstractLever.isOpen = true;
            this.abstractLever2.isOpen = true;
            if(FlxG.music != null)
            {
               FlxG.music.volume = 0.5;
            }
         }
         if(this.boss.x > 944 && this.boss.x < 1008 && this.boss.y > 100 && this.leverHapis.isOpen && this.blockHapis.y > 120)
         {
            if(this.boss.velocity.x < 0)
            {
               this.boss.velocity.x = -800;
            }
            else
            {
               this.boss.velocity.x = 800;
            }
            if(this.boss.x >= 944 && this.boss.x <= 1008)
            {
               this.boss.x = 1024;
            }
         }
         if(!this.bossIsTrapped && (this.boss.x >= 1008 && this.boss.y > 24 && this.leverHapis.isOpen))
         {
            this.patternMatcher.start();
         }
         this.bossIsTrapped = this.boss.x >= 1008 && this.boss.y > 24 && this.leverHapis.isOpen;
         if(!this.leverHapisIsOpen && this.leverHapis.isOpen)
         {
            for each(_m in monsters.members)
            {
               if(!_m.isDead && _m.y > 88 && _m.x >= 960 && _m.x <= 1008)
               {
                  if(!(_m is MonsterHans))
                  {
                     _m.x = 950;
                  }
               }
            }
         }
         this.leverHapisIsOpen = this.leverHapis.isOpen;
         if(player.x < 776)
         {
            this.camera.x = player.x;
         }
         else if(player.x <= 1068)
         {
            if(this.camera.x < 907)
            {
               this.camera.velocity.x = 160;
            }
            else if(this.camera.x > 917)
            {
               this.camera.velocity.x = -160;
            }
            else
            {
               this.camera.velocity.x = 0;
               this.camera.x = 912;
            }
         }
         else if(this.camera.x < player.x)
         {
            this.camera.velocity.x = 300;
         }
         else
         {
            this.camera.velocity.x = 0;
            this.camera.x = player.x;
         }
      }
      
      private function bossDeath(_t:FlxTimer) : void
      {
         blocks.add(new MonsterHansDying(this.boss.x,this.boss.y + 50,scale));
         this.abstractLever2.isOpen = false;
         this.boss.kill();
         musicStop();
         this.bossIsDead = true;
         this.lever2sNotUsable();
      }
      
      private function kutuOl(Sprite1:FlxSprite, Sprite2:FlxTilemap) : void
      {
         if(Sprite1 is BlockFalling)
         {
            createParticles(Sprite1.x + Sprite1.width * 0.5,Sprite1.y + Sprite1.height);
            Sprite1.kill();
            FlxG.play(Sfxkutuol);
         }
      }
      
      private function kutuOl2(Sprite1:FlxSprite, Sprite2:FlxSprite) : void
      {
         if(Sprite1 is BlockFalling)
         {
            createParticles(Sprite1.x + Sprite1.width * 0.5,Sprite1.y + Sprite1.height);
            Sprite1.kill();
            FlxG.play(Sfxhurt);
            FlxG.play(Sfxkutuol);
         }
      }
      
      override protected function overlapped(Sprite1:FlxSprite, Sprite2:FlxSprite) : void
      {
         if(Sprite1 is Hans && Sprite2 is Door && player.interact)
         {
            interactWithDoor(Sprite2);
         }
         else if(Sprite1 is Hans && Sprite2 is Door2 && player.interact)
         {
            if(!isThereCheerlover)
            {
               interactWithDoor(Sprite2);
            }
            else if(player.x - cheerlover.x < 100)
            {
               interactWithDoor(Sprite2);
            }
            else if(!didCheerloverYell)
            {
               didCheerloverYell = true;
               cheerloverYellText = new Narrator(levelwidth * 8 - 200,126,180,9999,false);
               narrators.add(cheerloverYellText);
               timer1.start(120);
            }
         }
         else if(Sprite1 is Hans && Sprite2 is Lever && player.interact && leversUsable)
         {
            Sprite2.kill();
         }
         else if(Sprite1 is Hans && Sprite2 is Lever2 && player.interact && leversUsable)
         {
            if(this.bossIsTrapped && !this.patternMatcher.asking && this.patternMatcher.answering)
            {
               Sprite2.kill();
            }
         }
         else if(Sprite1 is Hans && Sprite2 is NarratorTouch)
         {
            Sprite2.kill();
         }
         else if(Sprite1 is Cheerleader && (Sprite2 is Door || Sprite2 is Door2) && !cheerleader.gone)
         {
            Sprite2.kill();
         }
         else if(Sprite1 is Cheerlover && (Sprite2 is Door || Sprite2 is Door2) && !cheerlover.fading)
         {
            Sprite2.kill();
         }
         else if(Sprite1 is Hans && Sprite2 is Machine && player.interact)
         {
            Sprite2.kill();
            timer2.start(180);
            add(new Narrator(16,370,150,11,false));
         }
      }
      
      override public function getWell() : void
      {
         FlxG.bgColor = 4288900388;
      }
      
      override public function nextLevel() : void
      {
         FlxG.switchState(new Level40());
      }
      
      override public function addBgDetail() : void
      {
         bgDetails[2][0] = "windowBig00";
         bgDetails[2][1] = "windowBig02";
         bgDetails[3][0] = "windowBig20";
         bgDetails[3][1] = "windowBig22";
         bgDetails[4][1] = "windowSmall";
         bgDetails[5][1] = "windowSmall";
      }
      
      private function lever2sUsable() : void
      {
         this.lever2light1.letBeUsable();
         this.lever2light2.letBeUsable();
         this.lever2light3.letBeUsable();
         this.lever2light4.letBeUsable();
      }
      
      private function lever2sNotUsable() : void
      {
         this.lever2light1.notUsable();
         this.lever2light2.notUsable();
         this.lever2light3.notUsable();
         this.lever2light4.notUsable();
      }
   }
}


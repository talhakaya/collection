package levels
{
   import flash.ui.Mouse;
   import flash.utils.getTimer;
   import org.flixel.*;
   
   public class Level extends FlxState
   {
      
      private static var Sfxdie1:Class = Level_Sfxdie1;
      
      private static var Sfxdie2:Class = Level_Sfxdie2;
      
      private static var Sfxdie3:Class = Level_Sfxdie3;
      
      private static var Sfxdie4:Class = Level_Sfxdie4;
      
      private static var Sfxdie5:Class = Level_Sfxdie5;
      
      private static var Sfxkuslar:Class = Level_Sfxkuslar;
      
      private static var Sfxhansongoomba:Class = Level_Sfxhansongoomba;
      
      private static var Sfxwind:Class = Level_Sfxwind;
      
      private static var Sfxblockfalling:Class = Level_Sfxblockfalling;
      
      private static var music:Class = Level_music;
      
      public var level:FlxTilemap;
      
      public var scale:FlxPoint;
      
      public var player:Hans;
      
      public var cheerleader:Cheerleader;
      
      public var monsters:FlxGroup;
      
      public var interacts:FlxGroup;
      
      public var blocks:FlxGroup;
      
      public var narrators:FlxGroup;
      
      public var timer1:TTimer;
      
      public var timer2:TTimer;
      
      public var timer3:TTimer;
      
      public var timer4:TTimer;
      
      public var timer5:TTimer;
      
      public var timer6:TTimer;
      
      public var timer7:TTimer;
      
      public var timer8:TTimer;
      
      public var timer9:TTimer;
      
      public var timerForMusicFadeOut:FlxTimer;
      
      public var narrator:Narrator;
      
      public var narratorFollow:Narrator;
      
      public var narrator2:Narrator;
      
      public var editorMode:Boolean;
      
      public var levelwidth:uint;
      
      public var levelheight:uint;
      
      public var leversUsable:Boolean;
      
      public var xprev:Number;
      
      public var yprev:Number;
      
      public var isThereCheerlover:Boolean;
      
      public var cheerlover:Cheerlover;
      
      public var didCheerloverYell:Boolean;
      
      public var cheerloverYellText:Narrator;
      
      public var machineForCheerleader:Machine;
      
      public var bg:FlxGroup;
      
      public var bgObjects:FlxGroup;
      
      private var xAndY:FlxText;
      
      public var bgDetails:Array;
      
      public var bgBirds:Array;
      
      public var bgPalms:Array;
      
      public var playedSfxdie:Boolean;
      
      public var cheerleaderTouchedDoor:Boolean;
      
      public var palmwind:Array;
      
      public var levelno:int;
      
      public var gameSave:FlxSave;
      
      public var noMusic:Boolean;
      
      public var logo:FlxButton;
      
      private var startTime:int;
      
      public var created:Boolean;
      
      public function Level()
      {
         super();
      }
      
      override public function create() : void
      {
         this.startTime = getTimer();
         super.create();
         if(this.levelno == 0)
         {
            this.levelno = this.gameSave.data.level;
         }
         if(!this.noMusic && FlxG.music == null)
         {
            FlxG.playMusic(music);
         }
         this.timer1 = new TTimer();
         this.timer2 = new TTimer();
         this.timer3 = new TTimer();
         this.timer4 = new TTimer();
         this.timer5 = new TTimer();
         this.timer6 = new TTimer();
         this.timer7 = new TTimer();
         this.timer8 = new TTimer();
         this.timer9 = new TTimer();
         this.timerForMusicFadeOut = new FlxTimer();
         add(this.timer1);
         add(this.timer2);
         add(this.timer3);
         add(this.timer4);
         add(this.timer5);
         add(this.timer6);
         add(this.timer7);
         add(this.timer8);
         add(this.timer9);
         this.timer9.start(1);
         this.bgDetails = new Array();
         this.bgBirds = new Array();
         this.bgPalms = new Array();
         this.bg = new FlxGroup();
         add(this.bg);
         this.addBg();
         this.interacts = new FlxGroup();
         add(this.interacts);
         this.blocks = new FlxGroup();
         add(this.blocks);
         this.bgObjects = new FlxGroup();
         add(this.bgObjects);
         this.narrators = new FlxGroup();
         add(this.narrators);
         this.monsters = new FlxGroup();
         add(this.monsters);
         this.editorMode = false;
         if(this.editorMode)
         {
            FlxG.mouse.hide();
            Mouse.show();
         }
         else
         {
            FlxG.mouse.show();
         }
         this.leversUsable = true;
         this.isThereCheerlover = false;
         this.didCheerloverYell = false;
         this.cheerleaderTouchedDoor = false;
         if(this.editorMode)
         {
            this.xAndY = new FlxText(0,0,400,"",true);
            this.xAndY.color = 4294967295;
            add(this.xAndY);
         }
         this.playedSfxdie = false;
         this.created = true;
      }
      
      override public function update() : void
      {
         var k:int = 0;
         var xx:Number = NaN;
         var yy:Number = NaN;
         var mouseMoved:Boolean = false;
         var tile:uint = 0;
         var s:String = null;
         var i:int = 0;
         var j:int = 0;
         if(!this.created || this.player == null)
         {
            return;
         }
         super.update();
         FlxG.collide(this.level,this.player);
         FlxG.collide(this.level,this.monsters);
         FlxG.overlap(this.player,this.monsters,this.overlappedMonsters);
         FlxG.overlap(this.player,this.interacts,this.overlapped);
         FlxG.overlap(this.player,this.narrators,this.overlapped);
         if(this.player.y > 720 && this.levelheight < 120)
         {
            this.instantDeath();
         }
         if(this.editorMode)
         {
            if(FlxG.keys.ENTER)
            {
               FlxG.resetState();
            }
            xx = Math.floor(FlxG.mouse.x / 8);
            yy = Math.floor(FlxG.mouse.y / 8);
            mouseMoved = false;
            if(this.xprev != xx || this.yprev != yy)
            {
               mouseMoved = true;
            }
            this.xprev = xx;
            this.yprev = yy;
            if(FlxG.mouse.justPressed())
            {
               tile = this.level.getTile(xx,yy);
               this.level.setTile(xx,yy,tile > 0 ? 0 : 1);
            }
            if(FlxG.mouse.pressed())
            {
               if(mouseMoved)
               {
                  tile = this.level.getTile(xx,yy);
                  this.level.setTile(xx,yy,tile > 0 ? 0 : 1);
               }
            }
            if(FlxG.keys.justPressed("E"))
            {
               s = "";
               for(i = 0; i < this.levelheight; i++)
               {
                  for(j = 0; j < this.levelwidth; j++)
                  {
                     s += this.level.getTile(j,i) + ", ";
                  }
                  s += "\n";
               }
               trace(s);
            }
            this.xAndY.x = this.player.x - 12;
            this.xAndY.y = this.player.y - 16;
            this.xAndY.text = Math.floor(this.player.x) + ", " + Math.floor(this.player.y);
         }
         if(FlxG.keys.justPressed("ESCAPE"))
         {
            FlxG.switchState(new Menu());
         }
         if(this.timer1.complete)
         {
            this.fadeOut();
         }
         if(this.timer2.complete)
         {
            this.nextLevel2();
         }
         if(this.timer3.complete)
         {
            this.nextLevel3();
         }
         if(this.timer4.complete)
         {
            this.nextLevel();
         }
         if(this.timer5.complete)
         {
            this.die();
         }
         if(this.timer6.complete)
         {
            this.getWell();
         }
         if(this.timer7.complete)
         {
            this.die();
         }
         if(this.timer8.complete)
         {
            this.nextLevel();
         }
         if(this.timer9.complete)
         {
            add(this.player.timer);
            add(this.player.timer2);
            add(this.player.timer3);
         }
         for(k = 0; k < this.bgBirds.length; k++)
         {
            if(!this.bgBirds[k].birdFlied && this.player.x - this.bgBirds[k].x < 64 && this.bgBirds[k].x - this.player.x < 32 && this.player.y - this.bgBirds[k].y < 64 && this.bgBirds[k].y - this.player.y < 32)
            {
               this.bgBirds[k].birdFlied = true;
               this.bgBirds[k].play("fly");
               FlxG.play(Sfxkuslar);
            }
         }
         for(k = 0; k < this.bgPalms.length; k++)
         {
            if(!this.bgPalms[k].windEsti && this.player.x - this.bgPalms[k].x < 64 && this.bgPalms[k].x - this.player.x < 32)
            {
               this.bgPalms[k].wind();
               FlxG.play(Sfxwind);
            }
         }
         if(FlxG.keys.justPressed("M"))
         {
            if(FlxG.volume > 0)
            {
               FlxG.volume = 0;
            }
            else
            {
               FlxG.volume = 1;
            }
         }
         if(this.logo == null)
         {
            this.logo = new logoButton(this.scale);
            add(this.logo);
         }
      }
      
      protected function overlapped(Sprite1:FlxSprite, Sprite2:FlxSprite) : void
      {
         if(Sprite1 is Hans && Sprite2 is Door && this.player.interact)
         {
            this.interactWithDoor(Sprite2);
         }
         else if(Sprite1 is Hans && Sprite2 is Door2 && this.player.interact)
         {
            if(!this.isThereCheerlover)
            {
               this.interactWithDoor(Sprite2);
            }
            else if(this.player.x - this.cheerlover.x < 100)
            {
               this.interactWithDoor(Sprite2);
            }
            else if(!this.didCheerloverYell)
            {
               this.didCheerloverYell = true;
               this.cheerloverYellText = new Narrator(this.levelwidth * 8 - 200,126,180,9999,false);
               this.narrators.add(this.cheerloverYellText);
               this.timer1.start(120);
            }
         }
         else if(Sprite1 is Hans && (Sprite2 is Lever || Sprite2 is Lever2) && this.player.interact && this.leversUsable)
         {
            Sprite2.kill();
         }
         else if(Sprite1 is Hans && Sprite2 is NarratorTouch)
         {
            Sprite2.kill();
         }
         else if(Sprite1 is Cheerleader && (Sprite2 is Door || Sprite2 is Door2) && !this.cheerleaderTouchedDoor)
         {
            Sprite2.kill();
            this.cheerleaderTouchedDoor = true;
         }
         else if(Sprite1 is Cheerlover && (Sprite2 is Door || Sprite2 is Door2) && !this.cheerleaderTouchedDoor)
         {
            Sprite2.kill();
            this.cheerleaderTouchedDoor = true;
         }
         else if(Sprite1 is Hans && Sprite2 is Machine && this.player.interact)
         {
            Sprite2.kill();
            this.timer2.start(180);
            add(new Narrator(16,370,150,11,false));
            this.musicFadeOut();
         }
      }
      
      public function musicFadeOut() : void
      {
         this.timerForMusicFadeOut.start(0.2,10,this.musicFadeOut2);
      }
      
      public function musicFadeOut2(a:FlxTimer) : void
      {
         if(FlxG.music != null)
         {
            FlxG.music.volume -= 0.1;
         }
      }
      
      public function musicStop() : void
      {
         if(FlxG.music != null)
         {
            FlxG.music.stop();
            FlxG.music = null;
         }
         this.noMusic = true;
      }
      
      protected function cheerleaderOverlappedWithMachine(Sprite1:Cheerleader, Sprite2:Machine) : void
      {
         Sprite2.kill();
         this.timer3.start(60);
         this.musicFadeOut();
      }
      
      protected function overlappedMonsters(Sprite1:FlxSprite, Sprite2:Monster) : void
      {
         if(Sprite1 is Hans && Sprite2 is MonsterGoomba && !Sprite2.isDead)
         {
            if(this.player.velocity.y > 0)
            {
               if(this.scale.x < 4)
               {
                  Sprite2.isDead = true;
               }
               else
               {
                  FlxG.play(Sfxhansongoomba);
               }
               this.player.velocity.y = -this.player.maxVelocity.y * 0.4;
               this.player.jumpThrottle = 0;
            }
            else if(Sprite1.y > Sprite2.y - 25)
            {
               this.getHurt();
            }
         }
         else if(Sprite1 is Hans && Sprite2 is MonsterGel && !Sprite2.isDead)
         {
            if(this.scale.x > 1)
            {
               if(this.player.velocity.y > 0)
               {
                  Sprite2.isDead = true;
                  this.player.velocity.y = -this.player.maxVelocity.y * 0.4;
                  this.player.jumpThrottle = 0;
               }
               else if(Sprite1.y > Sprite2.y - 25)
               {
                  this.getHurt();
               }
            }
            else
            {
               Sprite2.isDead = true;
            }
         }
         else if(Sprite1 is Hans && Sprite2 is MonsterGelBlue && !Sprite2.isDead)
         {
            this.instantDeath();
         }
         else if(Sprite1 is Hans && Sprite2 is MonsterHans && !Sprite2.isDead)
         {
            this.getHurt();
         }
      }
      
      public function getWell() : void
      {
         FlxG.bgColor = 4289357414;
      }
      
      public function die() : void
      {
         FlxG.resetState();
      }
      
      public function nextLevel2() : void
      {
         this.timer4.start(60);
         this.player.fading = true;
         this.player.moveEnable = false;
      }
      
      public function nextLevel3() : void
      {
         this.nextLevel2();
         this.cheerleader.gone = true;
      }
      
      public function nextLevel() : void
      {
      }
      
      public function getHurt() : void
      {
         this.player.getHurt();
         FlxG.bgColor = 2009866786;
         if(this.player.hp == 0)
         {
            this.instantDeath();
         }
         else
         {
            this.timer6.start(240);
         }
      }
      
      public function instantDeath() : void
      {
         FlxG.bgColor = 2009866786;
         this.timer7.start(30);
         this.player.fading = true;
         this.player.moveEnable = false;
         if(!this.playedSfxdie)
         {
            this.playedSfxdie = true;
            if(this.player.scale.x == 1)
            {
               FlxG.play(Sfxdie1);
            }
            else if(this.player.scale.x == 2)
            {
               FlxG.play(Sfxdie2);
            }
            else if(this.player.scale.x == 4)
            {
               FlxG.play(Sfxdie3);
            }
            else if(this.player.scale.x == 8)
            {
               FlxG.play(Sfxdie4);
            }
            else if(this.player.scale.x == 16)
            {
               FlxG.play(Sfxdie5);
            }
         }
      }
      
      public function interactWithDoor(Sprite2:FlxSprite) : void
      {
         Sprite2.kill();
         this.timer8.start(60);
         this.player.fading = true;
         this.player.moveEnable = false;
      }
      
      private function fadeOut() : void
      {
         this.cheerloverYellText.erase = true;
         this.didCheerloverYell = false;
      }
      
      public function createParticles(_X:Number, _Y:Number) : void
      {
         var i:uint = 0;
         for(i = 0; i < 5; i++)
         {
            add(new ParticleBlack(_X,_Y));
         }
      }
      
      public function blockParticle(_level:FlxTilemap, _block:BlockFalling) : void
      {
         if(!_block.touchedFloor && _block.isTouching(FlxObject.FLOOR))
         {
            _block.touchedFloor = true;
            this.createParticles(_block.x + _block.width * 0.5,_block.y + _block.height);
            FlxG.play(Sfxblockfalling);
         }
      }
      
      public function blockCollision(_hans:Hans, _block:FlxSprite) : void
      {
         if(_block is BlockFalling && _block.velocity.y > 0 && _block.y + 90 < this.player.y)
         {
            this.instantDeath();
         }
         else if(_block is BlockDead && this.player.y + this.player.height <= _block.y)
         {
            _block.kill();
         }
      }
      
      public function addBg() : void
      {
         var i:int = 0;
         var j:int = 0;
         var tempBg:Bg = null;
         var a:int = Math.ceil(this.levelwidth / 8 / this.scale.x) + 1;
         var b:int = Math.ceil(this.levelheight / 8 / this.scale.x) + 1;
         for(i = 0; i < a; i++)
         {
            this.bgDetails.push(new Array());
            for(j = 0; j < b; j++)
            {
               this.bgDetails[i].push("0");
            }
         }
         this.addBgDetail();
         for(i = 0; i < a; i++)
         {
            for(j = 0; j < b; j++)
            {
               tempBg = new Bg(64 * this.scale.x * i,64 * this.scale.x * j,this.scale);
               if(this.bgDetails[i][j] != "0")
               {
                  if(this.bgDetails[i][j] == "torch")
                  {
                     tempBg.torch();
                  }
                  else if(this.bgDetails[i][j] == "windowPalm")
                  {
                     tempBg.windowPalm();
                     this.bgPalms.push(tempBg);
                  }
                  else if(this.bgDetails[i][j] == "damaged")
                  {
                     tempBg.damaged();
                  }
                  else if(this.bgDetails[i][j] == "sun00")
                  {
                     tempBg.sun00();
                  }
                  else if(this.bgDetails[i][j] == "sun01")
                  {
                     tempBg.sun01();
                  }
                  else if(this.bgDetails[i][j] == "sun10")
                  {
                     tempBg.sun10();
                  }
                  else if(this.bgDetails[i][j] == "sun11")
                  {
                     tempBg.sun11();
                  }
                  else if(this.bgDetails[i][j] == "bird")
                  {
                     tempBg.bird();
                  }
                  else if(this.bgDetails[i][j] == "windowSmall")
                  {
                     tempBg.windowSmall();
                  }
                  else if(this.bgDetails[i][j] == "bird2")
                  {
                     tempBg.bird2();
                     this.bgBirds.push(tempBg);
                  }
                  else if(this.bgDetails[i][j] == "windowBig00")
                  {
                     tempBg.windowBig00();
                  }
                  else if(this.bgDetails[i][j] == "windowBig01")
                  {
                     tempBg.windowBig01();
                  }
                  else if(this.bgDetails[i][j] == "windowBig02")
                  {
                     tempBg.windowBig02();
                  }
                  else if(this.bgDetails[i][j] == "windowBig10")
                  {
                     tempBg.windowBig10();
                  }
                  else if(this.bgDetails[i][j] == "windowBig11")
                  {
                     tempBg.windowBig11();
                  }
                  else if(this.bgDetails[i][j] == "windowBig12")
                  {
                     tempBg.windowBig12();
                  }
                  else if(this.bgDetails[i][j] == "windowBig20")
                  {
                     tempBg.windowBig20();
                  }
                  else if(this.bgDetails[i][j] == "windowBig21")
                  {
                     tempBg.windowBig21();
                  }
                  else if(this.bgDetails[i][j] == "windowBig22")
                  {
                     tempBg.windowBig22();
                  }
                  else if(this.bgDetails[i][j] == "corner")
                  {
                     tempBg.corner();
                  }
                  else if(this.bgDetails[i][j] == "greyCloud")
                  {
                     tempBg.greyCloud();
                  }
                  else if(this.bgDetails[i][j] == "hans")
                  {
                     tempBg.hans();
                  }
               }
               add(tempBg);
            }
         }
      }
      
      public function addBgDetail() : void
      {
      }
   }
}


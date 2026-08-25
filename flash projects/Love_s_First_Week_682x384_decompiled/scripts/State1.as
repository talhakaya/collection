package
{
   import flash.display.StageDisplayState;
   import flash.net.URLRequest;
   import flash.net.navigateToURL;
   import flash.ui.Mouse;
   import org.flixel.*;
   
   public class State1 extends FlxState
   {
      
      public static var Tile:Class = State1_Tile;
      
      private static var kafadakiok:Class = State1_kafadakiok;
      
      private static var Gunes:Class = State1_Gunes;
      
      private static var KapiEsigi:Class = State1_KapiEsigi;
      
      private static var DiyalogKutusu:Class = State1_DiyalogKutusu;
      
      private static var Asagi:Class = State1_Asagi;
      
      private static var talhaJump:Class = State1_talhaJump;
      
      private static var talhaFloor:Class = State1_talhaFloor;
      
      private static var nazFloor:Class = State1_nazFloor;
      
      private static var nazJump:Class = State1_nazJump;
      
      private static var nazWalk:Class = State1_nazWalk;
      
      private static var talhaWalk:Class = State1_talhaWalk;
      
      private static var Dogru:Class = State1_Dogru;
      
      private static var menubg:Class = State1_menubg;
      
      private static var img1:Class = State1_img1;
      
      private static var img2:Class = State1_img2;
      
      private static var img3:Class = State1_img3;
      
      private static var imgR:Class = State1_imgR;
      
      private static var imgSpace:Class = State1_imgSpace;
      
      private static var img4:Class = State1_img4;
      
      public var FontNES:String = "State1_FontNES";
      
      public var xAndY:FlxText;
      
      public var camera:FlxSprite;
      
      public var cameraX:Number = 100;
      
      public var cameraY:Number = 100;
      
      public var editorMode:Boolean;
      
      public const CAMERASPEED:Number = 150;
      
      public var level:FlxTilemap;
      
      public var levelwidth:uint = 15;
      
      public var levelheight:uint = 8;
      
      public var levelno:int;
      
      public var xprev:Number;
      
      public var yprev:Number;
      
      public var controlNaz:Boolean = true;
      
      public var naz:Naz;
      
      public var talha:Naz;
      
      public var talhaVar:Boolean = false;
      
      public var tasaTekmeler:FlxGroup;
      
      public var enArkalar:FlxGroup;
      
      public var bulutlar:FlxGroup;
      
      public var sarmasiklar:FlxGroup;
      
      public var agaclar:FlxGroup;
      
      public var enOndekiler:FlxGroup;
      
      public var diyalogKutusu1:FlxSprite;
      
      public var diyalogKutusu2:FlxSprite;
      
      public var diyalogKutusu3:FlxSprite;
      
      public var diyalogKutusu:FlxSprite;
      
      public var diyaloglar:Array;
      
      public var diyalogText1:FlxText;
      
      public var diyalogText2:FlxText;
      
      public var asagi:FlxSprite;
      
      public var nazKafa:Kafa;
      
      public var talhaKafa:Kafa;
      
      public var ondekiler:FlxGroup;
      
      public var taslar:FlxGroup;
      
      public var kapilar:FlxGroup;
      
      public var oklar:FlxGroup;
      
      public var kafaoku:FlxSprite;
      
      public var kapi1:Kapi;
      
      public var kapi2_:Kapi;
      
      public var kapiCount:int = -1;
      
      public var diyalogCount:int = -1;
      
      public var diyalogVar:Boolean;
      
      public var di:int = -1;
      
      public var dj:int = -1;
      
      public var nazIsTouching:Boolean;
      
      public var talhaIsTouching:Boolean;
      
      public var nazWalkCount:int = -1;
      
      public var talhaWalkCount:int = -1;
      
      public const WALKSOUND:int = 20;
      
      public var save:FlxSave;
      
      public const NAZIRE:String = "Naz";
      
      public var isMenuOn:Boolean;
      
      public var isFullscreenAvailable:Boolean = false;
      
      public var isMenuButton:Boolean = true;
      
      public var bgYogunluk:Number = 16;
      
      private var menuBack:FlxSprite;
      
      private var menuContinue:FlxButton;
      
      private var menuRestart:FlxButton;
      
      private var menuMute:FlxButton;
      
      private var menuMore:FlxButton;
      
      private var menuMain:FlxButton;
      
      private var menuFullscreen:FlxSprite;
      
      private var menuSound:FlxSprite;
      
      private var menuLanguage:FlxText;
      
      private var menuTurkce:FlxButton;
      
      private var menuEnglish:FlxButton;
      
      private var menuWASD:FlxSprite;
      
      private var menuDirs:FlxSprite;
      
      private var menuR:FlxSprite;
      
      private var menuSpace:FlxSprite;
      
      private var menuRText:FlxText;
      
      private var menuSpaceText:FlxText;
      
      public var is1player:Boolean;
      
      public var kapi2var:Boolean;
      
      public var logo:logoButton;
      
      public function State1(_is1player:Boolean)
      {
         super();
         this.is1player = _is1player;
      }
      
      override public function create() : void
      {
         var stripe:FlxSprite = null;
         var menuButton:FlxButton = null;
         this.save = new FlxSave();
         this.save.bind("0");
         if(this.save.data.lang == null)
         {
            this.save.data.lang = "eng";
         }
         this.editorMode = false;
         FlxG.bgColor = 4294967295;
         this.enArkalar = new FlxGroup();
         add(this.enArkalar);
         this.bulutlar = new FlxGroup();
         add(this.bulutlar);
         this.agaclar = new FlxGroup();
         add(this.agaclar);
         this.sarmasiklar = new FlxGroup();
         add(this.sarmasiklar);
         this.kapilar = new FlxGroup();
         add(this.kapilar);
         this.oklar = new FlxGroup();
         add(this.oklar);
         this.ondekiler = new FlxGroup();
         add(this.ondekiler);
         this.taslar = new FlxGroup();
         this.ondekiler.add(this.taslar);
         this.tasaTekmeler = new FlxGroup();
         add(this.tasaTekmeler);
         this.enOndekiler = new FlxGroup();
         add(this.enOndekiler);
         this.diyalogKutusu1 = new FlxSprite(0,192);
         this.diyalogKutusu1.makeGraphic(64,64,4294934399);
         this.diyalogKutusu1.scrollFactor.x = 0;
         this.diyalogKutusu1.scrollFactor.y = 0;
         this.diyalogKutusu1.alpha = 0;
         add(this.diyalogKutusu1);
         this.diyalogKutusu2 = new FlxSprite(64,192);
         this.diyalogKutusu2.makeGraphic(327,64,4290822336);
         this.diyalogKutusu2.scrollFactor.x = 0;
         this.diyalogKutusu2.scrollFactor.y = 0;
         this.diyalogKutusu2.alpha = 0;
         add(this.diyalogKutusu2);
         this.diyalogKutusu3 = new FlxSprite(391,192);
         this.diyalogKutusu3.makeGraphic(64,64,4281017343);
         this.diyalogKutusu3.scrollFactor.x = 0;
         this.diyalogKutusu3.scrollFactor.y = 0;
         this.diyalogKutusu3.alpha = 0;
         add(this.diyalogKutusu3);
         add(this.nazKafa = new Kafa(true));
         add(this.talhaKafa = new Kafa(false));
         this.diyaloglar = new Array();
         this.diyalogKutusu = new FlxSprite(0,192,DiyalogKutusu);
         this.diyalogKutusu.scrollFactor.x = 0;
         this.diyalogKutusu.scrollFactor.y = 0;
         this.diyalogKutusu.alpha = 0;
         add(this.diyalogKutusu);
         this.diyalogText1 = new FlxText(90,195,300,"");
         this.diyalogText1.setFormat("NES",12,4278190080,"left",1);
         this.diyalogText1.scrollFactor.x = 0;
         this.diyalogText1.scrollFactor.y = 0;
         add(this.diyalogText1);
         this.diyalogText2 = new FlxText(90,208,300,"");
         this.diyalogText2.setFormat("NES",12,4278190080,"left",1);
         this.diyalogText2.scrollFactor.x = 0;
         this.diyalogText2.scrollFactor.y = 0;
         add(this.diyalogText2);
         this.asagi = new FlxSprite(300,240,Asagi);
         this.asagi.scrollFactor.x = this.asagi.scrollFactor.y = 0;
         this.asagi.alpha = 0;
         add(this.asagi);
         stripe = new FlxSprite(0,0);
         stripe.makeGraphic(455,256,4286377455);
         stripe.scrollFactor.x = stripe.scrollFactor.y = 0;
         stripe.alpha = (16 - this.bgYogunluk) / 16;
         this.enArkalar.add(stripe);
         stripe = new FlxSprite(0,0);
         stripe.makeGraphic(455,256,4294934128);
         stripe.scrollFactor.x = stripe.scrollFactor.y = 0;
         stripe.alpha = this.bgYogunluk / 16;
         this.enArkalar.add(stripe);
         for(var i:int = 0; i < 16; i++)
         {
            stripe = new FlxSprite(0,i * 16);
            stripe.makeGraphic(FlxG.stage.width,16,4278190080);
            stripe.alpha = i / 32 - 0.1;
            stripe.scrollFactor.x = stripe.scrollFactor.y = 0;
            this.enArkalar.add(stripe);
         }
         this.kafaoku = new FlxSprite(6,6);
         this.kafaoku.loadGraphic(kafadakiok,true,false,6,6,false);
         this.kafaoku.addAnimation("0",[0,1,2,1],6,true);
         this.kafaoku.play("0");
         this.enOndekiler.add(this.kafaoku);
         this.kafaoku.x = -20;
         var gunes:FlxSprite = new FlxSprite(228 - 32,128 - 32,Gunes);
         gunes.scrollFactor.x = gunes.scrollFactor.y = 0;
         this.enArkalar.add(gunes);
         super.create();
         if(this.editorMode)
         {
            FlxG.mouse.show();
            this.xAndY = new FlxText(0,0,400,"",true);
            this.xAndY.color = 4294967295;
            add(this.xAndY);
         }
         else
         {
            FlxG.mouse.show();
         }
         this.camera = new FlxSprite(this.cameraX,this.cameraY);
         this.camera.alpha = 0;
         add(this.camera);
         if(this.isFullscreenAvailable)
         {
            FlxG.stage.displayState = StageDisplayState.FULL_SCREEN;
            FlxCamera.defaultZoom = 3;
         }
         else
         {
            FlxG.stage.displayState = StageDisplayState.NORMAL;
            FlxCamera.defaultZoom = 1.5;
         }
         FlxG.resetCameras(new FlxCamera(0,0,FlxG.width,FlxG.height));
         FlxG.camera.setBounds(0,0,this.levelwidth * 32,this.levelheight * 32,true);
         FlxG.camera.follow(this.camera,FlxCamera.STYLE_LOCKON);
         if(this.isMenuButton)
         {
            this.enOndekiler.add(menuButton = new FlxButton(455 - 80,0,"Menu",this.menuButtonClick));
            menuButton.scrollFactor.x = menuButton.scrollFactor.y = 0;
         }
         this.enOndekiler.add(this.logo = new logoButton(this.diyalogVar,true));
         this.enOndekiler.add(this.logo = new logoButton(this.diyalogVar,false));
      }
      
      public function menuButtonClick() : void
      {
         var sub:Number = NaN;
         if(!this.isMenuOn)
         {
            sub = 16;
            this.isMenuOn = true;
            this.enOndekiler.add(this.menuBack = new FlxSprite((455 - 200) / 2,8,menubg));
            this.menuBack.alpha = 0.4;
            this.enOndekiler.add(this.menuContinue = new FlxButton((455 - 80) / 2,40 - sub,"Continue",this.menuButtonClick));
            this.enOndekiler.add(this.menuRestart = new FlxButton((455 - 80) / 2,64 - sub,"Restart",this.resetState));
            this.enOndekiler.add(this.menuMute = new FlxButton((455 - 80) / 2,88 - sub,"Mute On/Off",this.muteOnOff));
            this.enOndekiler.add(this.menuMain = new FlxButton((455 - 80) / 2,112 - sub,"Main Menu",this.mainMenu));
            this.enOndekiler.add(this.menuMore = new FlxButton((455 - 80) / 2,136 - sub,"More Games",this.goToMyURL));
            if(this.isFullscreenAvailable)
            {
               this.enOndekiler.add(this.menuFullscreen = new FlxSprite((455 - 120) / 2,130,img3));
               this.menuFullscreen.scrollFactor.x = this.menuFullscreen.scrollFactor.y = 0;
            }
            this.enOndekiler.add(this.menuSound = new FlxSprite((455 - 120) / 2,142,img4));
            this.enOndekiler.add(this.menuLanguage = new FlxText((455 - 120) / 2,160,130,"Language / Dil:"));
            if(this.save.data.lang == "tur")
            {
               this.menuLanguage.text = "Language / Dil: Turkce";
            }
            else
            {
               this.menuLanguage.text = "Language / Dil: English";
            }
            this.enOndekiler.add(this.menuTurkce = new FlxButton(150,172,"Turkce",this.turkce));
            this.enOndekiler.add(this.menuEnglish = new FlxButton(232,172,"English",this.english));
            this.menuBack.scrollFactor.x = this.menuBack.scrollFactor.y = 0;
            this.menuContinue.scrollFactor.x = this.menuContinue.scrollFactor.y = 0;
            this.menuEnglish.scrollFactor.x = this.menuEnglish.scrollFactor.y = 0;
            this.menuLanguage.scrollFactor.x = this.menuLanguage.scrollFactor.y = 0;
            this.menuMute.scrollFactor.x = this.menuMute.scrollFactor.y = 0;
            this.menuMain.scrollFactor.x = this.menuMain.scrollFactor.y = 0;
            this.menuRestart.scrollFactor.x = this.menuRestart.scrollFactor.y = 0;
            this.menuSound.scrollFactor.x = this.menuSound.scrollFactor.y = 0;
            this.menuTurkce.scrollFactor.x = this.menuTurkce.scrollFactor.y = 0;
            this.menuMore.scrollFactor.x = this.menuMore.scrollFactor.y = 0;
            this.enOndekiler.add(this.menuWASD = new FlxSprite(140,193,img2));
            this.enOndekiler.add(this.menuDirs = new FlxSprite(188,193,img1));
            this.enOndekiler.add(this.menuR = new FlxSprite(286,210,imgR));
            this.enOndekiler.add(this.menuSpace = new FlxSprite(270,192,imgSpace));
            this.enOndekiler.add(this.menuRText = new FlxText(240,210,2000,"Restart:"));
            this.enOndekiler.add(this.menuSpaceText = new FlxText(240,192,2000,"Kick:"));
            this.menuWASD.scrollFactor.x = this.menuWASD.scrollFactor.y = 0;
            this.menuDirs.scrollFactor.x = this.menuDirs.scrollFactor.y = 0;
            this.menuR.scrollFactor.x = this.menuR.scrollFactor.y = 0;
            this.menuSpace.scrollFactor.x = this.menuSpace.scrollFactor.y = 0;
            this.menuRText.scrollFactor.x = this.menuRText.scrollFactor.y = 0;
            this.menuSpaceText.scrollFactor.x = this.menuSpaceText.scrollFactor.y = 0;
         }
         else
         {
            this.isMenuOn = false;
            this.menuBack.kill();
            this.menuContinue.kill();
            this.menuRestart.kill();
            this.menuMute.kill();
            this.menuMain.kill();
            this.menuMore.kill();
            if(this.isFullscreenAvailable)
            {
               this.menuFullscreen.kill();
            }
            this.menuSound.kill();
            this.menuLanguage.kill();
            this.menuTurkce.kill();
            this.menuEnglish.kill();
            this.menuWASD.kill();
            this.menuDirs.kill();
            this.menuR.kill();
            this.menuSpace.kill();
            this.menuRText.kill();
            this.menuSpaceText.kill();
         }
      }
      
      public function goToMyURL() : void
      {
         navigateToURL(new URLRequest("http://www.2pg.com"));
      }
      
      private function turkce() : void
      {
         if(this.isMenuOn)
         {
            this.menuLanguage.text = "Language / Dil: Turkce";
         }
         this.save.data.lang = "tur";
      }
      
      private function english() : void
      {
         if(this.isMenuOn)
         {
            this.menuLanguage.text = "Language / Dil: English";
         }
         this.save.data.lang = "eng";
      }
      
      public function muteOnOff() : void
      {
         if(FlxG.volume > 0)
         {
            FlxG.volume = 0;
         }
         else
         {
            FlxG.volume = 0.5;
         }
      }
      
      override public function update() : void
      {
         var xx:Number = NaN;
         var yy:Number = NaN;
         var mouseMoved:Boolean = false;
         var tile:uint = 0;
         var s:String = null;
         var i:int = 0;
         var j:int = 0;
         super.update();
         if(this.isMenuOn)
         {
            if(this.save.data.lang == "tur")
            {
               this.menuLanguage.text = "Language / Dil: Turkce";
            }
            else
            {
               this.menuLanguage.text = "Language / Dil: English";
            }
         }
         this.naz.kapida = false;
         FlxG.overlap(this.tasaTekmeler,this.taslar,this.overlapTekme);
         FlxG.collide(this.ondekiler);
         FlxG.collide(this.naz,this.taslar,this.nazTasCollide);
         FlxG.overlap(this.naz,this.tasaTekmeler,this.nazTekmeOverlap);
         FlxG.overlap(this.naz,this.kapilar,this.kapiOverlap);
         if(this.talhaVar)
         {
            this.talha.kapida = false;
            FlxG.overlap(this.talha,this.tasaTekmeler,this.nazTekmeOverlap);
            FlxG.overlap(this.talha,this.oklar,this.nazOkOverlap);
            FlxG.collide(this.talha,this.taslar,this.nazTasCollide);
            FlxG.overlap(this.talha,this.kapilar,this.kapiOverlap);
         }
         FlxG.overlap(this.naz,this.oklar,this.nazOkOverlap);
         if(this.naz.kapida && this.talhaVar && this.talha.kapida)
         {
            this.opus();
         }
         FlxG.overlap(this.taslar,this.oklar,this.tasOkOverlap);
         if(!this.controlNaz && this.naz.movable)
         {
            if(this.naz.velocity.y > 0)
            {
               this.naz.play("down");
            }
            else if(this.naz.velocity.y < 0)
            {
               this.naz.play("up");
            }
            else
            {
               this.naz.play("idle");
            }
         }
         if(this.controlNaz && this.talhaVar && this.talha.movable)
         {
            if(this.talha.velocity.y > 0)
            {
               this.talha.play("down");
            }
            else if(this.talha.velocity.y < 0)
            {
               this.talha.play("up");
            }
            else
            {
               this.talha.play("idle");
            }
         }
         if(this.talhaVar)
         {
            if(!this.talhaIsTouching && this.talha.isTouching(FlxObject.FLOOR))
            {
               FlxG.play(talhaFloor);
            }
            else if(this.talhaIsTouching && !this.talha.isTouching(FlxObject.FLOOR))
            {
               FlxG.play(talhaJump);
            }
            this.talhaIsTouching = this.talha.isTouching(FlxObject.FLOOR);
            if(this.talha.x > this.levelwidth * 32 - 16)
            {
               this.talha.x = this.levelwidth * 32 - 16;
            }
            else if(this.talha.x < 16)
            {
               this.talha.x = 16;
            }
         }
         if(!this.nazIsTouching && this.naz.isTouching(FlxObject.FLOOR))
         {
            FlxG.play(nazFloor);
         }
         else if(this.nazIsTouching && !this.naz.isTouching(FlxObject.FLOOR))
         {
            FlxG.play(nazJump);
         }
         this.nazIsTouching = this.naz.isTouching(FlxObject.FLOOR);
         if(this.naz.x > this.levelwidth * 32 - 16)
         {
            this.naz.x = this.levelwidth * 32 - 16;
         }
         else if(this.naz.x < 16)
         {
            this.naz.x = 16;
         }
         if(this.is1player)
         {
            if(this.controlNaz && this.naz.movable)
            {
               if(FlxG.keys.LEFT || FlxG.keys.A)
               {
                  this.naz.facing = FlxObject.LEFT;
                  this.naz.acceleration.x = -this.naz.maxVelocity.x * 4;
                  this.naz.play("walk");
                  if(this.nazIsTouching)
                  {
                     if(this.nazWalkCount == 0)
                     {
                        this.nazWalkCount = this.WALKSOUND;
                        FlxG.play(nazWalk);
                     }
                     --this.nazWalkCount;
                  }
               }
               else if(FlxG.keys.RIGHT || FlxG.keys.D)
               {
                  this.naz.facing = FlxObject.RIGHT;
                  this.naz.acceleration.x = this.naz.maxVelocity.x * 4;
                  this.naz.play("walk");
                  if(this.nazIsTouching)
                  {
                     if(this.nazWalkCount == 0)
                     {
                        this.nazWalkCount = this.WALKSOUND;
                        FlxG.play(nazWalk);
                     }
                     --this.nazWalkCount;
                  }
               }
               else
               {
                  this.naz.acceleration.x = 0;
                  this.naz.play("idle");
                  this.nazWalkCount = this.WALKSOUND;
               }
               if((FlxG.keys.UP || FlxG.keys.W) && this.naz.isTouching(FlxObject.FLOOR))
               {
                  this.naz.jumpThrottle = 0;
                  this.naz.velocity.y = -this.naz.maxVelocity.y * 0.2;
               }
               if((FlxG.keys.UP || FlxG.keys.W) && this.naz.jumpThrottle < this.naz.jumpThrottleMax && this.naz.velocity.y < 0)
               {
                  ++this.naz.jumpThrottle;
                  this.naz.velocity.y -= this.naz.maxVelocity.y * 0.043;
               }
               if(this.naz.velocity.y > 0)
               {
                  this.naz.play("down");
               }
               else if(this.naz.velocity.y < 0)
               {
                  this.naz.play("up");
               }
               if(FlxG.keys.SPACE && this.naz.isTouching(FlxObject.FLOOR))
               {
                  this.naz.tekme();
               }
            }
            else if(!this.controlNaz && this.talha.movable)
            {
               if(FlxG.keys.LEFT || FlxG.keys.A)
               {
                  this.talha.facing = FlxObject.LEFT;
                  this.talha.acceleration.x = -this.talha.maxVelocity.x * 4;
                  this.talha.play("walk");
                  if(this.talhaIsTouching)
                  {
                     if(this.talhaWalkCount == 0)
                     {
                        this.talhaWalkCount = this.WALKSOUND;
                        FlxG.play(talhaWalk);
                     }
                     --this.talhaWalkCount;
                  }
               }
               else if(FlxG.keys.RIGHT || FlxG.keys.D)
               {
                  this.talha.facing = FlxObject.RIGHT;
                  this.talha.acceleration.x = this.talha.maxVelocity.x * 4;
                  this.talha.play("walk");
                  if(this.talhaIsTouching)
                  {
                     if(this.talhaWalkCount == 0)
                     {
                        this.talhaWalkCount = this.WALKSOUND;
                        FlxG.play(talhaWalk);
                     }
                     --this.talhaWalkCount;
                  }
               }
               else
               {
                  this.talha.acceleration.x = 0;
                  this.talha.play("idle");
                  this.talhaWalkCount = this.WALKSOUND;
               }
               if((FlxG.keys.UP || FlxG.keys.W) && this.talha.isTouching(FlxObject.FLOOR))
               {
                  this.talha.jumpThrottle = 0;
                  this.talha.velocity.y = -this.talha.maxVelocity.y * 0.2;
               }
               if((FlxG.keys.UP || FlxG.keys.W) && this.talha.jumpThrottle < this.talha.jumpThrottleMax && this.talha.velocity.y < 0)
               {
                  ++this.talha.jumpThrottle;
                  this.talha.velocity.y -= this.talha.maxVelocity.y * 0.043;
               }
               if(this.talha.velocity.y > 0)
               {
                  this.talha.play("down");
               }
               else if(this.talha.velocity.y < 0)
               {
                  this.talha.play("up");
               }
               if(FlxG.keys.SPACE && this.talha.isTouching(FlxObject.FLOOR))
               {
                  this.talha.tekme();
               }
            }
         }
         else
         {
            if(this.naz.movable)
            {
               if(FlxG.keys.LEFT)
               {
                  this.naz.facing = FlxObject.LEFT;
                  this.naz.acceleration.x = -this.naz.maxVelocity.x * 4;
                  this.naz.play("walk");
                  if(this.nazIsTouching)
                  {
                     if(this.nazWalkCount == 0)
                     {
                        this.nazWalkCount = this.WALKSOUND;
                        FlxG.play(nazWalk);
                     }
                     --this.nazWalkCount;
                  }
               }
               else if(FlxG.keys.RIGHT)
               {
                  this.naz.facing = FlxObject.RIGHT;
                  this.naz.acceleration.x = this.naz.maxVelocity.x * 4;
                  this.naz.play("walk");
                  if(this.nazIsTouching)
                  {
                     if(this.nazWalkCount == 0)
                     {
                        this.nazWalkCount = this.WALKSOUND;
                        FlxG.play(nazWalk);
                     }
                     --this.nazWalkCount;
                  }
               }
               else
               {
                  this.naz.acceleration.x = 0;
                  this.naz.play("idle");
                  this.nazWalkCount = this.WALKSOUND;
               }
               if(FlxG.keys.UP && this.naz.isTouching(FlxObject.FLOOR))
               {
                  this.naz.jumpThrottle = 0;
                  this.naz.velocity.y = -this.naz.maxVelocity.y * 0.2;
               }
               if(FlxG.keys.UP && this.naz.jumpThrottle < this.naz.jumpThrottleMax && this.naz.velocity.y < 0)
               {
                  ++this.naz.jumpThrottle;
                  this.naz.velocity.y -= this.naz.maxVelocity.y * 0.043;
               }
               if(this.naz.velocity.y > 0)
               {
                  this.naz.play("down");
               }
               else if(this.naz.velocity.y < 0)
               {
                  this.naz.play("up");
               }
               if(FlxG.keys.K && this.naz.isTouching(FlxObject.FLOOR))
               {
                  this.naz.tekme();
               }
            }
            if(this.talha.movable)
            {
               if(FlxG.keys.A)
               {
                  this.talha.facing = FlxObject.LEFT;
                  this.talha.acceleration.x = -this.talha.maxVelocity.x * 4;
                  this.talha.play("walk");
                  if(this.talhaIsTouching)
                  {
                     if(this.talhaWalkCount == 0)
                     {
                        this.talhaWalkCount = this.WALKSOUND;
                        FlxG.play(talhaWalk);
                     }
                     --this.talhaWalkCount;
                  }
               }
               else if(FlxG.keys.D)
               {
                  this.talha.facing = FlxObject.RIGHT;
                  this.talha.acceleration.x = this.talha.maxVelocity.x * 4;
                  this.talha.play("walk");
                  if(this.talhaIsTouching)
                  {
                     if(this.talhaWalkCount == 0)
                     {
                        this.talhaWalkCount = this.WALKSOUND;
                        FlxG.play(talhaWalk);
                     }
                     --this.talhaWalkCount;
                  }
               }
               else
               {
                  this.talha.acceleration.x = 0;
                  this.talha.play("idle");
                  this.talhaWalkCount = this.WALKSOUND;
               }
               if(FlxG.keys.W && this.talha.isTouching(FlxObject.FLOOR))
               {
                  this.talha.jumpThrottle = 0;
                  this.talha.velocity.y = -this.talha.maxVelocity.y * 0.2;
               }
               if(FlxG.keys.W && this.talha.jumpThrottle < this.talha.jumpThrottleMax && this.talha.velocity.y < 0)
               {
                  ++this.talha.jumpThrottle;
                  this.talha.velocity.y -= this.talha.maxVelocity.y * 0.043;
               }
               if(this.talha.velocity.y > 0)
               {
                  this.talha.play("down");
               }
               else if(this.talha.velocity.y < 0)
               {
                  this.talha.play("up");
               }
               if(FlxG.keys.SPACE && this.talha.isTouching(FlxObject.FLOOR))
               {
                  this.talha.tekme();
               }
            }
         }
         if(FlxG.keys.justPressed("ENTER") && this.talhaVar)
         {
            this.controlNaz = !this.controlNaz;
         }
         if(this.naz.tekmeAtti)
         {
            this.naz.tekmeAtti = false;
            this.tasaTekmeler.add(new SoyutTekme(this.naz.x - 12,this.naz.y + 16,true));
         }
         else if(this.talhaVar && this.talha.tekmeAtti)
         {
            this.talha.tekmeAtti = false;
            this.tasaTekmeler.add(new SoyutTekme(this.talha.x - 12,this.talha.y + 16,false));
         }
         if(FlxG.keys.justPressed("R"))
         {
            this.resetState();
         }
         if(this.editorMode)
         {
            Mouse.show();
            xx = Math.floor(FlxG.mouse.x / 32);
            yy = Math.floor(FlxG.mouse.y / 32);
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
            this.xAndY.x = this.camera.x - 12;
            this.xAndY.y = this.camera.y - 16;
         }
         if(this.isFullscreenAvailable)
         {
            if(FlxG.keys.justPressed("F"))
            {
               if(FlxG.stage.displayState == StageDisplayState.NORMAL)
               {
                  FlxG.stage.displayState = StageDisplayState.FULL_SCREEN;
                  FlxCamera.defaultZoom = 3;
                  FlxG.resetCameras(new FlxCamera(0,0,FlxG.width,FlxG.height));
                  FlxG.camera.setBounds(0,0,this.levelwidth * 32,this.levelheight * 32,true);
                  FlxG.camera.follow(this.camera,FlxCamera.STYLE_LOCKON);
               }
               else
               {
                  FlxG.stage.displayState = StageDisplayState.NORMAL;
                  FlxCamera.defaultZoom = 1.5;
                  FlxG.resetCameras(new FlxCamera(0,0,FlxG.width,FlxG.height));
                  FlxG.camera.setBounds(0,0,this.levelwidth * 32,this.levelheight * 32,true);
                  FlxG.camera.follow(this.camera,FlxCamera.STYLE_LOCKON);
               }
            }
            if(FlxG.stage.displayState == StageDisplayState.NORMAL && FlxCamera.defaultZoom == 3)
            {
               FlxCamera.defaultZoom = 1.5;
               FlxG.resetCameras(new FlxCamera(0,0,FlxG.width,FlxG.height));
               FlxG.camera.setBounds(0,0,this.levelwidth * 32,this.levelheight * 32,true);
               FlxG.camera.follow(this.camera,FlxCamera.STYLE_LOCKON);
            }
         }
         if(this.is1player)
         {
            if(this.controlNaz)
            {
               if(this.naz.x > this.camera.x + 10)
               {
                  this.camera.velocity.x = this.CAMERASPEED;
               }
               else if(this.naz.x < this.camera.x - 10)
               {
                  this.camera.velocity.x = -this.CAMERASPEED;
               }
               else
               {
                  this.camera.velocity.x = 0;
               }
               if(this.naz.y > this.camera.y + 10)
               {
                  this.camera.velocity.y = this.CAMERASPEED;
               }
               else if(this.naz.y < this.camera.y - 10)
               {
                  this.camera.velocity.y = -this.CAMERASPEED;
               }
               else
               {
                  this.camera.velocity.y = 0;
               }
            }
            else
            {
               if(this.talha.x > this.camera.x + 10)
               {
                  this.camera.velocity.x = this.CAMERASPEED;
               }
               else if(this.talha.x < this.camera.x - 10)
               {
                  this.camera.velocity.x = -this.CAMERASPEED;
               }
               else
               {
                  this.camera.velocity.x = 0;
               }
               if(this.talha.y > this.camera.y + 10)
               {
                  this.camera.velocity.y = this.CAMERASPEED;
               }
               else if(this.talha.y < this.camera.y - 10)
               {
                  this.camera.velocity.y = -this.CAMERASPEED;
               }
               else
               {
                  this.camera.velocity.y = 0;
               }
            }
         }
         else if(this.controlNaz)
         {
            this.camera.x = this.naz.x;
            this.camera.y = this.naz.y;
         }
         else
         {
            this.camera.x = this.talha.x;
            this.camera.y = this.talha.y;
         }
         this.cameraX = this.camera.x;
         this.cameraY = this.camera.y;
         if(this.is1player)
         {
            if(this.controlNaz)
            {
               this.kafaoku.x = this.naz.x + 6;
               this.kafaoku.y = this.naz.y - 10;
            }
            else
            {
               this.kafaoku.x = this.talha.x + 6;
               this.kafaoku.y = this.talha.y - 10;
            }
         }
         if(this.kapiCount == 50)
         {
            this.kapi1.open();
            this.kapi2open();
            FlxG.play(Dogru);
            --this.kapiCount;
         }
         else if(this.kapiCount == 0)
         {
            this.nextLevel();
         }
         else if(this.kapiCount > 0)
         {
            --this.kapiCount;
         }
         if(this.naz.y > this.levelheight * 32 || this.talhaVar && this.talha.y > this.levelheight * 32)
         {
            this.death();
         }
         if(this.diyalogVar && Boolean(this.save.data.dialog))
         {
            if(this.di == -1 && this.dj == -1)
            {
               this.diyalogBaslat();
               ++this.di;
               this.diyalogCount = 6;
            }
            else if(this.di == this.diyaloglar.length)
            {
               this.diyalogKapat();
               this.diyalogVar = false;
            }
            else if(this.diyaloglar[this.di].text.length != this.dj)
            {
               if(FlxG.keys.S || FlxG.keys.DOWN || this.diyalogCount == 0)
               {
                  ++this.dj;
                  this.diyalogUpdate();
                  this.diyalogCount = 2;
               }
               else if(this.diyalogCount > 0)
               {
                  --this.diyalogCount;
               }
            }
            else if(FlxG.keys.justPressed("S") || FlxG.keys.justPressed("DOWN"))
            {
               ++this.di;
               this.diyalogCount = 6;
               this.dj = 0;
               this.diyalogText1.text = "";
               this.diyalogText2.text = "";
            }
         }
      }
      
      public function kapi2open() : void
      {
         if(this.kapi2var)
         {
            this.kapi2_.open();
         }
      }
      
      public function resetState() : void
      {
         this.save.data.dialog = false;
         if(this.save.data.level == 1)
         {
            if(this.is1player)
            {
               FlxG.switchState(new Level1(this.is1player));
            }
            else
            {
               FlxG.switchState(new Level3two(this.is1player));
            }
         }
         else if(this.save.data.level == 2)
         {
            if(this.is1player)
            {
               FlxG.switchState(new Level2(this.is1player));
            }
            else
            {
               FlxG.switchState(new Level3two(this.is1player));
            }
         }
         else if(this.save.data.level == 3)
         {
            if(this.is1player)
            {
               FlxG.switchState(new Level3(this.is1player));
            }
            else
            {
               FlxG.switchState(new Level3two(this.is1player));
            }
         }
         else if(this.save.data.level == 4)
         {
            if(this.is1player)
            {
               FlxG.switchState(new Level4(this.is1player));
            }
            else
            {
               FlxG.switchState(new Level4two(this.is1player));
            }
         }
         else if(this.save.data.level == 5)
         {
            if(this.is1player)
            {
               FlxG.switchState(new Level5(this.is1player));
            }
            else
            {
               FlxG.switchState(new Level5two(this.is1player));
            }
         }
         else if(this.save.data.level == 6)
         {
            FlxG.switchState(new Level6(this.is1player));
         }
         else if(this.save.data.level == 7)
         {
            FlxG.switchState(new Level7(this.is1player));
         }
         else if(this.save.data.level == 8)
         {
            FlxG.switchState(new Level8(this.is1player));
         }
         else if(this.save.data.level == 9)
         {
            FlxG.switchState(new Level9(this.is1player));
         }
         else if(this.save.data.level == 10)
         {
            FlxG.switchState(new Level10(this.is1player));
         }
         else if(this.save.data.level == 11)
         {
            FlxG.switchState(new Level11(this.is1player));
         }
         else if(this.save.data.level == 12)
         {
            FlxG.switchState(new Level12(this.is1player));
         }
         else if(this.save.data.level == 13)
         {
            FlxG.switchState(new Level13(this.is1player));
         }
         else if(this.save.data.level == 14)
         {
            FlxG.switchState(new Level14(this.is1player));
         }
         else if(this.save.data.level == 15)
         {
            FlxG.switchState(new Level15(this.is1player));
         }
         else if(this.save.data.level == 16)
         {
            FlxG.switchState(new Level16(this.is1player));
         }
      }
      
      public function overlapTekme(tekme:SoyutTekme, tas:Tas) : void
      {
         if(tekme.byNaz)
         {
            if(tas.x < this.naz.x && this.naz.facing == FlxObject.LEFT)
            {
               tas.tekmelen("left");
            }
            else if(tas.x > this.naz.x && this.naz.facing == FlxObject.RIGHT)
            {
               tas.tekmelen("right");
            }
         }
         else if(tas.x < this.talha.x && this.talha.facing == FlxObject.LEFT)
         {
            tas.tekmelen("up");
         }
         else if(tas.x > this.talha.x && this.talha.facing == FlxObject.RIGHT)
         {
            tas.tekmelen("up");
         }
      }
      
      public function nazTasCollide(asd:Naz, tas:Tas) : void
      {
         if(tas.y < asd.y + 25 && tas.velocity.y > 0)
         {
            this.death();
         }
         if(!tas.immovable)
         {
            if(tas.nazCount > 0)
            {
               --tas.nazCount;
            }
            else
            {
               tas.nazCount = 3;
               tas.makeImmovable();
            }
         }
      }
      
      public function death() : void
      {
         this.resetState();
      }
      
      public function nazTekmeOverlap(asd:Naz, tekme:SoyutTekme) : void
      {
         if(this.talhaVar)
         {
            if(tekme.byNaz && asd == this.talha)
            {
               if(this.talha.x < this.naz.x && this.naz.facing == FlxObject.LEFT)
               {
                  this.talha.tekmelen("left");
               }
               else if(this.talha.x > this.naz.x && this.naz.facing == FlxObject.RIGHT)
               {
                  this.talha.tekmelen("right");
               }
            }
            else if(!tekme.byNaz && asd == this.naz)
            {
               if(this.naz.x < this.talha.x && this.talha.facing == FlxObject.LEFT)
               {
                  this.naz.tekmelen("up");
               }
               else if(this.naz.x > this.talha.x && this.talha.facing == FlxObject.RIGHT)
               {
                  this.naz.tekmelen("up");
               }
            }
         }
      }
      
      public function nazOkOverlap(asd:Naz, ok:Ok) : void
      {
         if(ok.dir == "up" && asd.velocity.y >= 0 || ok.dir == "down" && asd.velocity.y <= 0 || ok.dir == "right" && asd.velocity.x <= 0 || ok.dir == "left" && asd.velocity.x >= 0)
         {
            asd.x = ok.x - 8;
            asd.y = ok.y - 14;
         }
         asd.tekmelen(ok.dir);
      }
      
      public function tasOkOverlap(tas:Tas, ok:Ok) : void
      {
         if(ok.dir == "up" && tas.velocity.y != -tas.SPEED || ok.dir == "down" && tas.velocity.y != tas.SPEED || ok.dir == "right" && tas.velocity.x != tas.SPEED || ok.dir == "left" && tas.velocity.x != -tas.SPEED)
         {
            tas.x = ok.x - 12;
            tas.y = ok.y - 12;
         }
         tas.tekmelen(ok.dir);
      }
      
      public function bulut(yogunluk:uint) : void
      {
         for(var i:int = 0; i < yogunluk; i++)
         {
            this.bulutlar.add(new Bulut("0",Math.random() * 500 - 40,30 + Math.random() * 50));
            this.bulutlar.add(new Bulut("1",Math.random() * 500 - 40,30 + Math.random() * 50));
            this.bulutlar.add(new Bulut("2",Math.random() * 500 - 40,30 + Math.random() * 50));
            this.bulutlar.add(new Bulut("3",Math.random() * 500 - 40,30 + Math.random() * 50));
         }
      }
      
      public function sarmasik(yogunluk:uint, genislik:Number, _X:Number, _Y:Number) : void
      {
         var random:Number = NaN;
         var dir:String = null;
         for(var i:int = 0; i < yogunluk * 4; i++)
         {
            random = 4 * Math.random();
            if(random > 3)
            {
               dir = "3";
            }
            else if(random > 2)
            {
               dir = "2";
            }
            else if(random > 1)
            {
               dir = "1";
            }
            else
            {
               dir = "0";
            }
            this.sarmasiklar.add(new Sarmasik(dir,_X + i * 3 * genislik + 10 * Math.random(),_Y + 30 * Math.random()));
         }
      }
      
      public function agac(no:String, _X:Number, _Y:Number) : void
      {
         this.agaclar.add(new Agac(no,_X,_Y));
      }
      
      public function kapiOverlap(asd:Naz, kapii:Kapi) : void
      {
         if(this.talhaVar)
         {
            asd.kapida = true;
         }
         else if(this.kapiCount == -1)
         {
            this.kapiCount = 50;
            this.naz.makeImmovable(50);
            this.naz.play("idle");
            this.naz.kapidaTek = true;
         }
      }
      
      public function nextLevel() : void
      {
         this.save.data.dialog = true;
      }
      
      public function mainMenu() : void
      {
         this.save.data.dialog = true;
         FlxG.switchState(new Menu());
      }
      
      public function kapi(_X:Number, _Y:Number) : void
      {
         this.kapilar.add(this.kapi1 = new Kapi(_X + 16,_Y + 16));
         this.enOndekiler.add(new FlxSprite(_X - 18,_Y + 64,KapiEsigi));
      }
      
      public function kapi2(_X:Number, _Y:Number) : void
      {
         this.kapi2var = true;
         this.kapilar.add(this.kapi2_ = new Kapi(_X + 16,_Y + 16));
         this.enOndekiler.add(new FlxSprite(_X - 18,_Y + 64,KapiEsigi));
      }
      
      public function ok(direction:String, _X:Number, _Y:Number) : void
      {
         this.oklar.add(new Ok(direction,_X + 16,_Y + 16));
      }
      
      public function tas(_X:Number, _Y:Number) : void
      {
         this.taslar.add(new Tas(_X + 4,_Y + 4));
      }
      
      public function opus() : void
      {
         if(this.is1player || !this.kapi2var)
         {
            if(this.kapiCount == -1)
            {
               this.talha.makeImmovable(150);
               this.naz.makeImmovable(150);
               this.kapiCount = 150;
               if(this.naz.x > this.talha.x)
               {
                  this.naz.facing = FlxObject.LEFT;
                  this.talha.facing = FlxObject.RIGHT;
                  this.naz.x = this.kapi1.x + 16;
                  this.talha.x = this.kapi1.x;
                  this.naz.y = this.kapi1.y + 18;
                  this.talha.y = this.kapi1.y + 18;
               }
               else
               {
                  this.naz.facing = FlxObject.RIGHT;
                  this.talha.facing = FlxObject.LEFT;
                  this.talha.x = this.kapi1.x + 16;
                  this.naz.x = this.kapi1.x;
                  this.naz.y = this.kapi1.y + 18;
                  this.talha.y = this.kapi1.y + 18;
               }
               this.naz.op();
               this.talha.op();
            }
         }
         else if(this.kapiCount == -1)
         {
            this.talha.makeImmovable(50);
            this.naz.makeImmovable(50);
            this.talha.velocity.x = this.talha.velocity.y = 0;
            this.naz.velocity.x = this.naz.velocity.y = 0;
            this.kapiCount = 50;
         }
      }
      
      public function diyalogBaslat() : void
      {
         this.diyalogKutusu1.alpha = 1;
         this.diyalogKutusu2.alpha = 1;
         this.diyalogKutusu3.alpha = 1;
         this.diyalogKutusu.alpha = 1;
         this.nazKafa.alpha = 1;
         this.talhaKafa.alpha = 1;
         this.asagi.alpha = 1;
      }
      
      public function diyalogKapat() : void
      {
         this.diyalogKutusu1.alpha = 0;
         this.diyalogKutusu2.alpha = 0;
         this.diyalogKutusu3.alpha = 0;
         this.diyalogKutusu.alpha = 0;
         this.nazKafa.alpha = 0;
         this.talhaKafa.alpha = 0;
         this.asagi.alpha = 0;
      }
      
      public function diyalogUpdate() : void
      {
         if(Boolean(this.diyaloglar[this.di].isNaz))
         {
            this.diyalogText1.text = "Naz:";
            this.diyalogText1.alignment = "left";
            this.diyalogText2.alignment = "left";
            this.nazKafa.play("talk");
            this.talhaKafa.play("idle");
         }
         else
         {
            this.diyalogText1.text = "Talha:";
            this.diyalogText1.alignment = "right";
            this.diyalogText2.alignment = "right";
            this.nazKafa.play("idle");
            this.talhaKafa.play("talk");
         }
         this.diyalogText2.text = this.diyaloglar[this.di].text.substring(0,this.dj + 1);
      }
   }
}


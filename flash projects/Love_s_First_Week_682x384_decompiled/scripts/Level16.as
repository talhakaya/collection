package
{
   import flash.display.StageDisplayState;
   import flash.ui.Mouse;
   import org.flixel.*;
   
   public class Level16 extends State1
   {
      
      private static var Asagi:Class = Level16_Asagi;
      
      public var son:FlxText;
      
      public var asagi1:FlxSprite;
      
      public var asagiBas:Boolean;
      
      public function Level16(_is1player:Boolean)
      {
         super(_is1player);
      }
      
      override public function create() : void
      {
         var data:Array = null;
         isMenuButton = false;
         bgYogunluk = 16;
         talhaVar = true;
         diyalogVar = true;
         levelwidth = 16;
         levelheight = 8;
         super.create();
         save.data.level = 16;
         data = new Array(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,13,0,0,0,0,0,0,0,0,0,0,15,15,15,15,16,14,0,0,0,0,0,0,0,0,0,0,16,16,16,16,16,14,0,0,0,0,0,0,0,0,0,0);
         level = new FlxTilemap();
         level.loadMap(FlxTilemap.arrayToCSV(data,levelwidth),Tile,32,32,FlxTilemap.AUTO);
         ondekiler.add(level);
         ondekiler.add(naz = new Naz(true,166,130));
         ondekiler.add(talha = new Naz(false,150,130));
         naz.sonBolum = true;
         talha.sonBolum = true;
         naz.play("otur1");
         talha.play("otur2");
         sarmasik(4,3,-10,100);
         agac("2",50,122);
         agac("2",0,122);
         agac("0",20,122);
         agac("0",-20,122);
         agac("1",-10,122);
         agac("1",40,122);
         agac("3",60,122);
         agac("3",80,122);
         agac("2",175,187);
         agac("1",195,187);
         agac("3",220,190);
         agac("0",250,230);
         kafaoku.x = -8;
         kafaoku.y = -8;
         if(save.data.lang == "tur")
         {
            diyaloglar.push(new Diyalog(false,"Bugün Cuma."));
            diyaloglar.push(new Diyalog(true,"Evet."));
            diyaloglar.push(new Diyalog(false,"Beni terk edicek misin?"));
            diyaloglar.push(new Diyalog(true,"Hayır."));
            diyaloglar.push(new Diyalog(false,"..."));
            diyaloglar.push(new Diyalog(true,"..."));
            diyaloglar.push(new Diyalog(false,"Seni çok seviyorum."));
            diyaloglar.push(new Diyalog(true,"Ben de seni çok seviyorum."));
            diyaloglar.push(new Diyalog(false,"..."));
            diyaloglar.push(new Diyalog(true,"Seni Pazar günü terk edicem."));
            diyaloglar.push(new Diyalog(false,"..."));
            this.son = new FlxText(228 - 34,113,64,"SON");
         }
         else
         {
            diyaloglar.push(new Diyalog(false,"Today\'s Friday."));
            diyaloglar.push(new Diyalog(true,"Yes."));
            diyaloglar.push(new Diyalog(false,"Will you really dump me?"));
            diyaloglar.push(new Diyalog(true,"No."));
            diyaloglar.push(new Diyalog(false,"..."));
            diyaloglar.push(new Diyalog(true,"..."));
            diyaloglar.push(new Diyalog(false,"I love you.."));
            diyaloglar.push(new Diyalog(true,"I love you too."));
            diyaloglar.push(new Diyalog(false,"..."));
            diyaloglar.push(new Diyalog(true,"I\'m gonna dump you on Sunday."));
            diyaloglar.push(new Diyalog(false,"..."));
            this.son = new FlxText(228 - 33,102,64,"THE END");
         }
         this.son.setFormat("NES",20,16777215,"center",2);
         add(this.son);
         this.son.alpha = 0;
         enOndekiler.add(this.asagi1 = new FlxSprite(320,200,Asagi));
         this.asagi1.alpha = 0;
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
         if(FlxG.keys.justPressed("R"))
         {
            FlxG.resetState();
         }
         if(editorMode)
         {
            Mouse.show();
            xx = Math.floor(FlxG.mouse.x / 32);
            yy = Math.floor(FlxG.mouse.y / 32);
            mouseMoved = false;
            if(xprev != xx || yprev != yy)
            {
               mouseMoved = true;
            }
            xprev = xx;
            yprev = yy;
            if(FlxG.mouse.justPressed())
            {
               tile = level.getTile(xx,yy);
               level.setTile(xx,yy,tile > 0 ? 0 : 1);
            }
            if(FlxG.mouse.pressed())
            {
               if(mouseMoved)
               {
                  tile = level.getTile(xx,yy);
                  level.setTile(xx,yy,tile > 0 ? 0 : 1);
               }
            }
            if(FlxG.keys.justPressed("E"))
            {
               s = "";
               for(i = 0; i < levelheight; i++)
               {
                  for(j = 0; j < levelwidth; j++)
                  {
                     s += level.getTile(j,i) + ", ";
                  }
                  s += "\n";
               }
               trace(s);
            }
            xAndY.x = camera.x - 12;
            xAndY.y = camera.y - 16;
         }
         if(isFullscreenAvailable)
         {
            if(FlxG.keys.justPressed("F"))
            {
               if(FlxG.stage.displayState == StageDisplayState.NORMAL)
               {
                  FlxG.stage.displayState = StageDisplayState.FULL_SCREEN;
                  FlxCamera.defaultZoom = 3;
                  FlxG.resetCameras(new FlxCamera(0,0,FlxG.width,FlxG.height));
                  FlxG.camera.setBounds(0,0,levelwidth * 32,levelheight * 32,true);
                  FlxG.camera.follow(camera,FlxCamera.STYLE_LOCKON);
               }
               else
               {
                  FlxG.stage.displayState = StageDisplayState.NORMAL;
                  FlxCamera.defaultZoom = 2;
                  FlxG.resetCameras(new FlxCamera(0,0,FlxG.width,FlxG.height));
                  FlxG.camera.setBounds(0,0,levelwidth * 32,levelheight * 32,true);
                  FlxG.camera.follow(camera,FlxCamera.STYLE_LOCKON);
               }
            }
            if(FlxG.stage.displayState == StageDisplayState.NORMAL && FlxCamera.defaultZoom == 3)
            {
               FlxCamera.defaultZoom = 2;
               FlxG.resetCameras(new FlxCamera(0,0,FlxG.width,FlxG.height));
               FlxG.camera.setBounds(0,0,levelwidth * 32,levelheight * 32,true);
               FlxG.camera.follow(camera,FlxCamera.STYLE_LOCKON);
            }
         }
         if(controlNaz)
         {
            if(naz.x > camera.x + CAMERASPEED / 10)
            {
               camera.velocity.x = CAMERASPEED;
            }
            else if(naz.x < camera.x - CAMERASPEED / 10)
            {
               camera.velocity.x = -CAMERASPEED;
            }
            else
            {
               camera.velocity.x = 0;
            }
            if(naz.y > camera.y + CAMERASPEED / 10)
            {
               camera.velocity.y = CAMERASPEED;
            }
            else if(naz.y < camera.y - CAMERASPEED / 10)
            {
               camera.velocity.y = -CAMERASPEED;
            }
            else
            {
               camera.velocity.y = 0;
            }
         }
         else
         {
            if(talha.x > camera.x + CAMERASPEED / 10)
            {
               camera.velocity.x = CAMERASPEED;
            }
            else if(talha.x < camera.x - CAMERASPEED / 10)
            {
               camera.velocity.x = -CAMERASPEED;
            }
            else
            {
               camera.velocity.x = 0;
            }
            if(talha.y > camera.y + CAMERASPEED / 10)
            {
               camera.velocity.y = CAMERASPEED;
            }
            else if(talha.y < camera.y - CAMERASPEED / 10)
            {
               camera.velocity.y = -CAMERASPEED;
            }
            else
            {
               camera.velocity.y = 0;
            }
         }
         cameraX = camera.x;
         cameraY = camera.y;
         if(kapiCount == 0)
         {
            this.nextLevel();
         }
         else if(kapiCount > 0)
         {
            --kapiCount;
         }
         if(diyalogVar)
         {
            if(di == -1 && dj == -1)
            {
               diyalogBaslat();
               ++di;
               diyalogCount = 6;
            }
            else if(di == diyaloglar.length)
            {
               diyalogKapat();
               diyalogVar = false;
            }
            else if(diyaloglar[di].text.length != dj)
            {
               if(FlxG.keys.S || FlxG.keys.DOWN || diyalogCount == 0)
               {
                  ++dj;
                  diyalogUpdate();
                  diyalogCount = 6;
               }
               else if(diyalogCount > 0)
               {
                  --diyalogCount;
               }
            }
            else if(FlxG.keys.justPressed("S") || FlxG.keys.justPressed("DOWN"))
            {
               ++di;
               diyalogCount = 6;
               dj = 0;
               diyalogText1.text = "";
               diyalogText2.text = "";
            }
         }
         else if(this.son.alpha < 1)
         {
            this.son.alpha += 0.005;
         }
         else
         {
            if(this.asagi1.alpha < 1)
            {
               this.asagi1.alpha += 0.01;
            }
            if(FlxG.keys.justPressed("DOWN") || FlxG.keys.justPressed("S"))
            {
               this.nextLevel();
            }
         }
      }
      
      override public function nextLevel() : void
      {
         super.nextLevel();
         FlxG.switchState(new Menu());
      }
   }
}


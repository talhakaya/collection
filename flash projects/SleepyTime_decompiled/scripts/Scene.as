package
{
   import flash.Boot;
   import flash.display.Bitmap;
   import flash.display.BitmapData;
   import flash.display.Sprite;
   import motion.Actuate;
   import openfl.Assets;
   
   public class Scene extends Sprite
   {
      
      public static var init__:Boolean;
      
      public static var lengthOfSection:Number;
      
      public static var PenisHeightMax:Number;
      
      public static var LineYRatioToScene:Number = 0.3;
      
      public static var numberOfSections:int = 5;
      
      public static var finishingScoreTableConst:int = 3000;
      
      public var trackArray:Array;
      
      public var timeAfterTrackArrayFinishes:int;
      
      public var textScore:TextTalha;
      
      public var textRating:TextTalha;
      
      public var textLyric:TextTalha;
      
      public var textCombo:TextTalha;
      
      public var spikes:Array;
      
      public var shakingPenisCounter:int;
      
      public var shakingHeadCounter:int;
      
      public var shakingHandCounter:int;
      
      public var shakeFactor:Number;
      
      public var score:int;
      
      public var pressingSpikeScoreCounter:int;
      
      public var pressingSpikeScoreConstant:int;
      
      public var pressingSpike:Spike;
      
      public var pointerForSpikeCreation:int;
      
      public var penisHeight:Number;
      
      public var penis:Citmap;
      
      public var particlePoolPointer:int;
      
      public var particlePoolMaxLength:int;
      
      public var particlePool:Array;
      
      public var panning:Number;
      
      public var oldKeyDown:Boolean;
      
      public var lyricsPointer:int;
      
      public var lyrics:Array;
      
      public var lines:Array;
      
      public var lineY:Number;
      
      public var isReplay:Boolean;
      
      public var inputSaver:Array;
      
      public var id:int;
      
      public var headRotationTimeCurrent:int;
      
      public var headRotationMax:Number;
      
      public var headRotatingToMax:Boolean;
      
      public var headRotating:Boolean;
      
      public var head:Citmap;
      
      public var hand:Citmap;
      
      public var finishingScoreTableCounter:int;
      
      public var finishingParticleCounter:int;
      
      public var finished:Boolean;
      
      public var endOfSong:Number;
      
      public var destroyMePleaseMessageTaken:Boolean;
      
      public var destroyMePlease:Boolean;
      
      public var crosshair:Citmap;
      
      public var comboCounter:int;
      
      public var color2:uint;
      
      public var color1:uint;
      
      public var bodiesLevel6:Array;
      
      public function Scene(param1:int = 0, param2:Boolean = false)
      {
         var _loc3_:int = 0;
         var _loc4_:int = 0;
         var _loc5_:int = 0;
         var _loc6_:Number = NaN;
         if(Boot.skip_constructor)
         {
            return;
         }
         headRotatingToMax = false;
         headRotationTimeCurrent = 0;
         headRotationMax = 15;
         headRotating = true;
         finishingScoreTableCounter = 0;
         finishingParticleCounter = 0;
         finished = false;
         shakeFactor = 2;
         shakingHeadCounter = 0;
         shakingHandCounter = 0;
         shakingPenisCounter = 0;
         penisHeight = 36;
         comboCounter = 0;
         pressingSpikeScoreConstant = 100;
         pressingSpikeScoreCounter = 0;
         particlePoolMaxLength = 100;
         particlePoolPointer = 0;
         oldKeyDown = false;
         lyricsPointer = 0;
         lyrics = [];
         destroyMePleaseMessageTaken = false;
         destroyMePlease = false;
         score = 0;
         pointerForSpikeCreation = 0;
         super();
         id = param1;
         isReplay = param2;
         if(!isReplay)
         {
            panning = 0;
         }
         else
         {
            panning = -1 + 2 * (id + 1) / (GameManager.id + 1);
         }
         trackArray = Scene.getTrackArray(id);
         _loc3_ = id;
         switch(_loc3_)
         {
            case 1:
               color1 = 11158596;
               color2 = 5579298;
               timeAfterTrackArrayFinishes = 1000;
               break;
            default:
               color1 = 11158596;
               color2 = 5579298;
         }
         spikes = [];
         lines = [];
         Scene.lengthOfSection = Main.stageWidth / Scene.numberOfSections;
         lineY = Main.stageHeight * 0.3;
         _loc3_ = 0;
         _loc4_ = Scene.numberOfSections + 1;
         while(_loc3_ < _loc4_)
         {
            _loc5_ = _loc3_++;
            lines.push(new Bitmap(Assets.getBitmapData("img/scene" + id + "/line1.png")));
            lines[_loc5_].scaleX = GameManager.ScaleX * 8 / Scene.numberOfSections;
            lines[_loc5_].x = _loc5_ * Scene.lengthOfSection;
            lines[_loc5_].y = lineY;
            addChild(lines[_loc5_]);
         }
         endOfSong = trackArray[int(trackArray.length) - 1].timeInSong + trackArray[int(trackArray.length) - 1].lengthOfPress;
         if(GameManager.id != 5)
         {
            penis = new Citmap(Assets.getBitmapData("img/scene" + id + "/penis.png"),8,72);
            hand = new Citmap(Assets.getBitmapData("img/scene" + id + "/hand.png"),64,90);
            head = new Citmap(Assets.getBitmapData("img/scene" + id + "/head.png"),45,90);
            penis.scaleX = penis.scaleY = Number(hand.scaleX = Number(hand.scaleY = Number(head.scaleX = Number(head.scaleY = 4))));
            head.x = 45 * head.scaleX;
            hand.x = Main.stageWidth;
            penis.x = Main.stageWidth - 32 * hand.scaleX;
            hand.y = Main.stageHeight + 72 * hand.scaleY + shakeFactor;
            addChild(penis);
            addChild(head);
            addChild(hand);
         }
         else
         {
            bodiesLevel6 = [];
            _loc3_ = 1;
            while(_loc3_ < 6)
            {
               _loc4_ = _loc3_++;
               bodiesLevel6.push(new Body(_loc4_,endOfSong));
               addChild(bodiesLevel6[_loc4_ - 1]);
            }
         }
         crosshair = new Citmap(Assets.getBitmapData("img/crosshair1.png"),25,25);
         crosshair.y = lineY + 3;
         crosshair.x = Scene.lengthOfSection;
         addChild(crosshair);
         textScore = new TextTalha("");
         textScore.x = Main.stageWidth / 2;
         textScore.y = Main.stageHeight / 7;
         textScore.blinking = true;
         addChild(textScore);
         textRating = new TextTalha("");
         textRating.alpha = 0;
         textRating.x = Main.stageWidth / 4;
         textRating.y = Main.stageHeight * 3 / 7;
         textRating.blinking = true;
         addChild(textRating);
         particlePool = [];
         textCombo = new TextTalha("");
         textCombo.x = Main.stageWidth * 3 / 4;
         textCombo.y = Main.stageHeight * 3 / 7;
         textCombo.blinking = true;
         addChild(textCombo);
         textLyric = new TextTalha("");
         textLyric.x = Main.stageWidth / 2;
         textLyric.y = Main.stageHeight * 5 / 7;
         addChild(textLyric);
         particlePool = [];
         _loc3_ = 0;
         _loc4_ = particlePoolMaxLength;
         while(_loc3_ < _loc4_)
         {
            _loc5_ = _loc3_++;
            particlePool.push(new Particle());
         }
         inputSaver = [];
         putInLyrics();
      }
      
      public static function getTrackArray(param1:int) : Array
      {
         var _loc3_:int = 0;
         var _loc4_:int = 0;
         var _loc5_:int = 0;
         var _loc6_:int = 0;
         var _loc2_:Array = [];
         switch(param1)
         {
            case 1:
               _loc2_.push(4);
               _loc3_ = 0;
               while(_loc3_ < 8)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(2);
                  _loc2_.push(2);
               }
               _loc3_ = 0;
               while(_loc3_ < 6)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(2);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(2);
                  _loc2_.push(2);
               }
               _loc3_ = 0;
               while(_loc3_ < 4)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(2);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(2);
                  _loc2_.push(2);
               }
               _loc3_ = 0;
               while(_loc3_ < 8)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc2_.push(7);
               _loc2_.push(1);
               _loc3_ = 0;
               while(_loc3_ < 4)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(2);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(2);
                  _loc2_.push(2);
               }
               _loc3_ = 0;
               while(_loc3_ < 6)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(2);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(2);
                  _loc2_.push(2);
               }
               break;
            case 2:
               _loc2_.push(4);
               _loc3_ = 0;
               while(_loc3_ < 8)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 4)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(2);
                  _loc2_.push(3);
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(1);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(1);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc2_.push(15);
               _loc2_.push(1);
               _loc3_ = 0;
               while(_loc3_ < 8)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 4)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(2);
                  _loc2_.push(3);
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(1);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(1);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc2_.push(15);
               _loc2_.push(1);
               break;
            case 3:
               _loc2_.push(4);
               _loc3_ = 0;
               while(_loc3_ < 8)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(1.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 4)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 4)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(1);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.5);
                  _loc2_.push(1);
                  _loc2_.push(0.5);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 8)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(1.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 4)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 4)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(1);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 8)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(1);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.5);
                  _loc2_.push(1);
                  _loc2_.push(0.5);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
               }
               _loc2_.push(0);
               break;
            case 4:
               _loc2_.push(4);
               _loc3_ = 0;
               while(_loc3_ < 12)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(1);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 4)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(1.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(1);
                  _loc2_.push(2);
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(6.5);
                  _loc2_.push(0.5);
               }
               _loc3_ = 0;
               while(_loc3_ < 12)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(1);
                  _loc2_.push(1);
               }
               _loc3_ = 0;
               while(_loc3_ < 3)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(1.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(1);
                  _loc2_.push(2);
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(6.5);
                  _loc2_.push(0.5);
               }
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(1.5);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(2);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(14.5);
               _loc2_.push(0.5);
               _loc3_ = 0;
               while(_loc3_ < 5)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(1);
                  _loc2_.push(1);
               }
               _loc2_.push(4);
               break;
            case 5:
               _loc2_.push(4);
               _loc3_ = 0;
               while(_loc3_ < 4)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(0.5);
                  _loc2_.push(0);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
               }
               _loc3_ = 0;
               while(_loc3_ < 2)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(0.5);
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(1);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
               }
               _loc2_.push(0.25);
               _loc2_.push(0.75);
               _loc2_.push(0.25);
               _loc2_.push(0.75);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.75);
               _loc2_.push(0.25);
               _loc2_.push(0.75);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc3_ = 0;
               while(_loc3_ < 3)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.25);
                  _loc2_.push(0.75);
                  _loc2_.push(0.25);
                  _loc2_.push(0.75);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.75);
                  _loc2_.push(0.25);
                  _loc2_.push(0.75);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(1);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
               }
               _loc3_ = 0;
               while(_loc3_ < 4)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(0.5);
                  _loc2_.push(0);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
               }
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(1.5);
               _loc2_.push(0.5);
               _loc2_.push(2);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(1.5);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(1.5);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc3_ = 0;
               while(_loc3_ < 9)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.25);
                  _loc2_.push(0.75);
                  _loc2_.push(0.25);
                  _loc2_.push(0.75);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.75);
                  _loc2_.push(0.25);
                  _loc2_.push(0.75);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(1);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
               }
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(2);
               _loc2_.push(0);
               _loc2_.push(3);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(4.5);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(2);
               _loc2_.push(0);
               _loc2_.push(3);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(8.5);
               _loc3_ = 0;
               while(_loc3_ < 3)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0.25);
                  _loc2_.push(0.75);
                  _loc2_.push(0.25);
                  _loc2_.push(0.75);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.75);
                  _loc2_.push(0.25);
                  _loc2_.push(0.75);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(1);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
                  _loc2_.push(0.5);
               }
               _loc2_.push(0.25);
               _loc2_.push(0.75);
               _loc2_.push(0.25);
               _loc2_.push(0.75);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.75);
               _loc2_.push(0);
               break;
            case 6:
               _loc2_.push(3);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.75);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(1.5);
               _loc2_.push(1.5);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(1.25);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(0.75);
               _loc2_.push(0);
               _loc2_.push(0.75);
               _loc2_.push(0);
               _loc2_.push(0.75);
               _loc2_.push(0);
               _loc2_.push(0.75);
               _loc2_.push(0);
               _loc2_.push(0.75);
               _loc2_.push(0);
               _loc2_.push(1.5);
               _loc2_.push(0);
               _loc2_.push(0.75);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(1.25);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(0.75);
               _loc2_.push(0);
               _loc2_.push(0.75);
               _loc2_.push(0);
               _loc2_.push(0.75);
               _loc2_.push(0);
               _loc2_.push(0.75);
               _loc2_.push(0);
               _loc2_.push(0.75);
               _loc2_.push(0);
               _loc2_.push(1.5);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc3_ = 0;
               while(_loc3_ < 3)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0);
                  _loc2_.push(0.5);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
               }
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc3_ = 0;
               while(_loc3_ < 3)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0);
                  _loc2_.push(0.5);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
               }
               _loc2_.push(0);
               _loc2_.push(1.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.75);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(1);
               _loc2_.push(0.5);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.5);
               _loc2_.push(0.5);
               _loc2_.push(1.5);
               _loc2_.push(1.5);
               _loc2_.push(0);
               _loc2_.push(1.5);
               _loc2_.push(0);
               _loc2_.push(1.5);
               _loc3_ = 0;
               while(_loc3_ < 4)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0);
                  _loc2_.push(0.5);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(1);
                  _loc2_.push(0.5);
                  _loc2_.push(0);
                  _loc2_.push(0.5);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(0.25);
                  _loc2_.push(1.25);
                  _loc2_.push(0);
                  _loc2_.push(0.5);
               }
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0.25);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc2_.push(0);
               _loc2_.push(1);
               _loc2_.push(0);
               _loc2_.push(0.5);
               _loc3_ = 0;
               while(_loc3_ < 8)
               {
                  _loc4_ = _loc3_++;
                  _loc5_ = 0;
                  while(_loc5_ < 3)
                  {
                     _loc6_ = _loc5_++;
                     _loc2_.push(0);
                     _loc2_.push(0.5);
                     _loc2_.push(0.25);
                     _loc2_.push(0.25);
                     _loc2_.push(0.25);
                     _loc2_.push(0.25);
                  }
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(0.5);
               }
               _loc3_ = 0;
               while(_loc3_ < 13)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(0);
                  _loc2_.push(1.5);
                  _loc2_.push(0);
                  _loc2_.push(1.5);
                  _loc2_.push(0);
                  _loc2_.push(1.5);
                  _loc2_.push(0);
                  _loc2_.push(1.5);
               }
               _loc3_ = 0;
               while(_loc3_ < 4)
               {
                  _loc4_ = _loc3_++;
                  _loc5_ = 0;
                  while(_loc5_ < 3)
                  {
                     _loc6_ = _loc5_++;
                     _loc2_.push(0.25);
                     _loc2_.push(0.25);
                     _loc2_.push(0.25);
                     _loc2_.push(0.25);
                     _loc2_.push(0.25);
                     _loc2_.push(0.25);
                  }
                  _loc2_.push(1);
                  _loc2_.push(0.5);
               }
               _loc3_ = 0;
               while(_loc3_ < 4)
               {
                  _loc4_ = _loc3_++;
                  _loc5_ = 0;
                  while(_loc5_ < 3)
                  {
                     _loc6_ = _loc5_++;
                     _loc2_.push(0);
                     _loc2_.push(0.5);
                     _loc2_.push(0.25);
                     _loc2_.push(0.25);
                     _loc2_.push(0.25);
                     _loc2_.push(0.25);
                  }
                  _loc2_.push(0);
                  _loc2_.push(1);
                  _loc2_.push(0);
                  _loc2_.push(0.5);
               }
               _loc2_.push(0);
               break;
            default:
               _loc3_ = 0;
               while(_loc3_ < 6)
               {
                  _loc4_ = _loc3_++;
                  _loc2_.push(1);
               }
         }
         return Scene.putInBeats(_loc2_);
      }
      
      public static function putInBeats(param1:Array) : Array
      {
         var _loc7_:int = 0;
         var _loc2_:Array = [];
         var _loc3_:int = 0;
         var _loc4_:int = -2;
         var _loc5_:int = 0;
         var _loc6_:int = int(param1.length);
         while(_loc5_ < _loc6_)
         {
            _loc7_ = _loc5_++;
            if(_loc4_ == -2)
            {
               _loc4_ = -1;
            }
            else if(_loc4_ == -1)
            {
               _loc4_ = int(Math.round(Number(param1[_loc7_]) * GameManager.rhythm));
               _loc2_.push(new Beat(_loc3_,_loc4_));
               _loc4_ = -2;
            }
            _loc3_ += int(Math.round(Number(param1[_loc7_]) * GameManager.rhythm));
         }
         return _loc2_;
      }
      
      public function updateTexts() : void
      {
         textScore.text = "Score: " + score;
         textScore.update();
         _temp_1.alpha -= GameManager.dt / GameManager.rhythm / 2;
         textRating.update();
         if(comboCounter > 1)
         {
            textCombo.text = "Combo x" + comboCounter;
         }
         else
         {
            textCombo.text = "No Combo";
         }
         textCombo.update();
         textLyric.update();
      }
      
      public function updateParticles() : void
      {
         var _loc3_:int = 0;
         var _loc1_:int = 0;
         var _loc2_:int = particlePoolMaxLength;
         while(_loc1_ < _loc2_)
         {
            _loc3_ = _loc1_++;
            if(particlePool[_loc3_].needsToBeKilled)
            {
               particlePool[_loc3_].kill();
               removeChild(particlePool[_loc3_]);
            }
            particlePool[_loc3_].update();
         }
      }
      
      public function updateLyrics() : void
      {
         if(int(lyrics.length) > lyricsPointer && GameManager.time / GameManager.rhythm >= lyrics[lyricsPointer].time)
         {
            textLyric.text = lyrics[lyricsPointer].text;
            textLyric.scaleUpCounter = 125;
            ++lyricsPointer;
         }
      }
      
      public function updateGraphicsTimeline() : void
      {
         var _loc6_:int = 0;
         var _loc1_:int = int(GameManager.time % GameManager.rhythm);
         var _loc2_:Number = _loc1_ / GameManager.rhythm;
         var _loc3_:Boolean = _loc2_ > 0.5;
         var _loc4_:int = 0;
         var _loc5_:int = Scene.numberOfSections + 1;
         while(_loc4_ < _loc5_)
         {
            _loc6_ = _loc4_++;
            _loc3_ = !_loc3_;
            if(_loc3_)
            {
               lines[_loc6_].bitmapData = Assets.getBitmapData("img/scene" + id + "/line1.png");
            }
            else
            {
               lines[_loc6_].bitmapData = Assets.getBitmapData("img/scene" + id + "/line2.png");
            }
            lines[_loc6_].x = _loc6_ * Scene.lengthOfSection - _loc2_ * Scene.lengthOfSection;
            lines[_loc6_].y = lineY;
         }
      }
      
      public function updateGraphicsSpikes() : void
      {
         var _loc2_:* = null as Spike;
         var _loc4_:* = null as Beat;
         var _loc5_:Number = NaN;
         var _loc1_:Boolean = true;
         while(_loc1_ && pointerForSpikeCreation < int(trackArray.length))
         {
            if(trackArray[pointerForSpikeCreation].getRelativePosition() < GameManager.rhythm * Scene.numberOfSections)
            {
               _loc2_ = new Spike(pointerForSpikeCreation,trackArray[pointerForSpikeCreation].lengthOfPress,id);
               spikes.push(_loc2_);
               addChild(_loc2_);
               ++pointerForSpikeCreation;
            }
            else
            {
               _loc1_ = false;
            }
         }
         var _loc3_:int = 0;
         while(_loc3_ < int(spikes.length))
         {
            _loc4_ = trackArray[spikes[_loc3_].idInTrack];
            if(!_loc4_.isHit && _loc4_.getRelativePosition() < -Beat.timingMax || _loc4_.lengthOfPress != 0 && !_loc4_.isHitEnd && _loc4_.getRelativePositionToEnd() < -Beat.timingMax)
            {
               comboCounter = 0;
            }
            if(_loc4_.isHit && _loc4_.getRelativePosition() < Beat.timingMax)
            {
               spikes[_loc3_].firstSpike.scaleX = spikes[_loc3_].firstSpike.scaleY = 300 * (1 - Math.abs(_loc4_.getRelativePosition()) / Beat.timingMax) / 100;
               if(spikes[_loc3_].firstSpike.scaleX < 0)
               {
                  spikes[_loc3_].firstSpike.scaleX = spikes[_loc3_].firstSpike.scaleY = 0;
               }
            }
            if(_loc4_.lengthOfPress != 0 && _loc4_.isHitEnd && _loc4_.getRelativePositionToEnd() < Beat.timingMax)
            {
               spikes[_loc3_].secondSpike.scaleX = spikes[_loc3_].secondSpike.scaleY = 300 * (1 - Math.abs(_loc4_.getRelativePositionToEnd()) / Beat.timingMax) / 100;
               if(spikes[_loc3_].secondSpike.scaleX < 0)
               {
                  spikes[_loc3_].secondSpike.scaleX = spikes[_loc3_].secondSpike.scaleY = 0;
               }
            }
            if(spikes[_loc3_] == pressingSpike)
            {
               spikes[_loc3_].line.scaleY = 2 * (_loc4_.getRelativePositionToEnd() / _loc4_.lengthOfPress);
               pressingSpikeScoreCounter += GameManager.dt;
               if(pressingSpikeScoreCounter >= pressingSpikeScoreConstant)
               {
                  pressingSpikeScoreCounter = 0;
                  addScore(1);
               }
            }
            else if(_loc4_.isHit && _loc4_.isHitEnd)
            {
               if(_loc4_.lengthOfPress != 0)
               {
                  spikes[_loc3_].line.scaleY = 0;
               }
            }
            else if(_loc4_.lengthOfPress != 0)
            {
               spikes[_loc3_].line.scaleY = 0.5;
            }
            spikes[_loc3_].x = Scene.lengthOfSection + Scene.lengthOfSection * (_loc4_.getRelativePosition() / GameManager.rhythm);
            spikes[_loc3_].y = lineY;
            if(spikes[_loc3_].scaleX <= 0 || _loc4_.getRelativePosition() < -2 * GameManager.rhythm && _loc4_.getRelativePositionToEnd() < -2 * GameManager.rhythm)
            {
               removeChild(spikes[_loc3_]);
               spikes.splice(_loc3_,1);
               _loc3_--;
            }
            _loc3_++;
         }
      }
      
      public function updateGraphicsBody() : void
      {
         penisHeight = Math.max(0,36 * (endOfSong - GameManager.time) / endOfSong);
         penis.y = Main.stageHeight + penisHeight * penis.scaleY + shakeFactor;
         head.y = Main.stageHeight + (36 - penisHeight) * penis.scaleY / 4 + shakeFactor;
         head.x = 45 * head.scaleX;
         penis.x = Main.stageWidth - 32 * penis.scaleX;
         penis.bitmap.alpha = 0.4 + 0.6 * Math.random();
         head.bitmap.alpha = 0.4 + 0.6 * Math.random();
         if(headRotating)
         {
            if(headRotatingToMax)
            {
               headRotationTimeCurrent += GameManager.dt;
               if(headRotationTimeCurrent >= GameManager.rhythm / 2)
               {
                  headRotatingToMax = false;
               }
            }
            else
            {
               headRotationTimeCurrent -= GameManager.dt;
               if(headRotationTimeCurrent <= -GameManager.rhythm / 2)
               {
                  headRotatingToMax = true;
               }
            }
            head.rotation = headRotationMax * (headRotationTimeCurrent / GameManager.rhythm / 2);
         }
         if(shakingHandCounter > 0)
         {
            shakingHandCounter -= GameManager.dt;
            _temp_1.x += -shakeFactor + 2 * shakeFactor * Math.random();
            _temp_2.y += -shakeFactor + 2 * shakeFactor * Math.random();
         }
         if(shakingHeadCounter > 0)
         {
            shakingHeadCounter -= GameManager.dt;
            _temp_3.x += -shakeFactor + 2 * shakeFactor * Math.random();
            _temp_4.y += -shakeFactor + 2 * shakeFactor * Math.random();
         }
         if(shakingPenisCounter > 0)
         {
            shakingPenisCounter -= GameManager.dt;
            _temp_5.x += -shakeFactor + 2 * shakeFactor * Math.random();
            _temp_6.y += -shakeFactor + 2 * shakeFactor * Math.random();
         }
      }
      
      public function updateGraphicsBodiesLevel6() : void
      {
         var _loc4_:int = 0;
         var _loc5_:Number = NaN;
         var _loc1_:Number = 2 * (int(GameManager.time % (GameManager.rhythm * 6))) / (GameManager.rhythm * 6) - 1;
         var _loc2_:int = 0;
         var _loc3_:int = int(bodiesLevel6.length);
         while(_loc2_ < _loc3_)
         {
            _loc4_ = _loc2_++;
            bodiesLevel6[_loc4_].x = Main.stageWidth / 2 + _loc4_ * _loc1_ * Main.stageWidth / 16 + (_loc1_ + 2) * _loc4_ * 30 - 60;
            bodiesLevel6[_loc4_].y = Main.stageHeight;
            bodiesLevel6[_loc4_].scaleX = bodiesLevel6[_loc4_].scaleY = Math.abs(1 - Math.abs(_loc1_));
            bodiesLevel6[_loc4_].update();
         }
      }
      
      public function updateGraphics() : void
      {
         updateGraphicsTimeline();
         updateGraphicsSpikes();
         updateLyrics();
         updateTexts();
         updateParticles();
         if(GameManager.id != 5)
         {
            updateGraphicsBody();
         }
         else
         {
            updateGraphicsBodiesLevel6();
         }
         checkIfFinished();
      }
      
      public function putInLyrics() : void
      {
         var _loc4_:int = 0;
         var _loc1_:int = id;
         switch(_loc1_)
         {
            case 1:
               lyrics.push(new Lyric(5,"Everyday you wake up at seven"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(2,"Your shift starts at eight"));
               lyrics.push(new Lyric(6,""));
               lyrics.push(new Lyric(2,"You work thirteen hours a day"));
               lyrics.push(new Lyric(6,""));
               lyrics.push(new Lyric(2,"Don\'t you deserve some rest?"));
               lyrics.push(new Lyric(8,""));
               lyrics.push(new Lyric(7,"Oh how you\'d like a job that is"));
               lyrics.push(new Lyric(7,"Nine to five"));
               lyrics.push(new Lyric(8,""));
               lyrics.push(new Lyric(7,"You\'d have all the time in the world"));
               lyrics.push(new Lyric(9,"Just work nine to five"));
               lyrics.push(new Lyric(8,""));
               lyrics.push(new Lyric(7,"Every night you get home at eleven"));
               lyrics.push(new Lyric(7,"You\'re so tired you can\'t watch a movie"));
               lyrics.push(new Lyric(8,"Without your head falling"));
               lyrics.push(new Lyric(6,""));
               lyrics.push(new Lyric(4,"How you\'d watch everything you find"));
               lyrics.push(new Lyric(4,"When you lived with your mom"));
               lyrics.push(new Lyric(8,""));
               lyrics.push(new Lyric(5,"But now how you\'d like a job that is"));
               lyrics.push(new Lyric(7,"Nine to five"));
               lyrics.push(new Lyric(8,""));
               lyrics.push(new Lyric(7,"You\'d have all the time in the world"));
               lyrics.push(new Lyric(7,"Just work nine to five"));
               lyrics.push(new Lyric(10,""));
               break;
            case 2:
               lyrics.push(new Lyric(5,"Everyday, you wake up at eleven"));
               lyrics.push(new Lyric(13,""));
               lyrics.push(new Lyric(2,"With booze near you, some cigarette fallen"));
               lyrics.push(new Lyric(14,""));
               lyrics.push(new Lyric(2,"To the floor where there is nothing"));
               lyrics.push(new Lyric(14,""));
               lyrics.push(new Lyric(2,"But your whole messy life in a nutshell"));
               lyrics.push(new Lyric(10,""));
               lyrics.push(new Lyric(8,"Some say you are a loser"));
               lyrics.push(new Lyric(12,""));
               lyrics.push(new Lyric(4,"Your friends look at you with pity"));
               lyrics.push(new Lyric(8,""));
               lyrics.push(new Lyric(4,"And you,"));
               lyrics.push(new Lyric(8,"You know"));
               lyrics.push(new Lyric(8,"There is"));
               lyrics.push(new Lyric(8,"No hope"));
               lyrics.push(new Lyric(6,"For you"));
               lyrics.push(new Lyric(12,""));
               lyrics.push(new Lyric(8,"Since your mother had cancer"));
               lyrics.push(new Lyric(13,""));
               lyrics.push(new Lyric(2,"You\'ve been living off of your father"));
               lyrics.push(new Lyric(14,""));
               lyrics.push(new Lyric(2,"Even if he didn\'t hate you"));
               lyrics.push(new Lyric(14,""));
               lyrics.push(new Lyric(2,"You would feel bad about the money"));
               lyrics.push(new Lyric(6,"He\'s giving you"));
               lyrics.push(new Lyric(4,""));
               lyrics.push(new Lyric(8,"Some say you are a loser"));
               lyrics.push(new Lyric(12,""));
               lyrics.push(new Lyric(4,"Your friends look at you with pity"));
               lyrics.push(new Lyric(8,""));
               lyrics.push(new Lyric(4,"And you,"));
               lyrics.push(new Lyric(8,"You know"));
               lyrics.push(new Lyric(8,"There is"));
               lyrics.push(new Lyric(8,"No hope"));
               lyrics.push(new Lyric(6,"For you"));
               lyrics.push(new Lyric(6,""));
               break;
            case 3:
               lyrics.push(new Lyric(4,"Everyday you wake up at eight"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"With power and struggle you hate"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"Nausea keeps coming back for more"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"Oh you\'d rather just seeing your boys"));
               lyrics.push(new Lyric(7,"Your boys"));
               lyrics.push(new Lyric(8,"One is lost and one is high"));
               lyrics.push(new Lyric(8,"Your boys"));
               lyrics.push(new Lyric(8,"One is dying and one has tried"));
               lyrics.push(new Lyric(8,"Your boys"));
               lyrics.push(new Lyric(4,""));
               lyrics.push(new Lyric(4,"Since you lost your wife"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"You can\'t sleep before five AM"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"You\'d die for one more moment"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"With your love"));
               lyrics.push(new Lyric(7,"Your love"));
               lyrics.push(new Lyric(8,"Cancer took her she is gone"));
               lyrics.push(new Lyric(8,"Your love"));
               lyrics.push(new Lyric(8,"She is missed and she is loved"));
               lyrics.push(new Lyric(8,"Your love"));
               lyrics.push(new Lyric(8,""));
               break;
            case 4:
               lyrics.push(new Lyric(5,"Everyday you wake up at whatever"));
               lyrics.push(new Lyric(7.5,""));
               lyrics.push(new Lyric(0.5,"It\'s hard, from your bed, to see the clock"));
               lyrics.push(new Lyric(15,""));
               lyrics.push(new Lyric(1,"Not that you\'d miss out on a lot"));
               lyrics.push(new Lyric(7.5,""));
               lyrics.push(new Lyric(0.5,"Without love, love, love"));
               lyrics.push(new Lyric(15,""));
               lyrics.push(new Lyric(14,"Like the wind once here it\'s gone"));
               lyrics.push(new Lyric(12,""));
               lyrics.push(new Lyric(4,"Their eyes looking from far"));
               lyrics.push(new Lyric(12,""));
               lyrics.push(new Lyric(4,"So far that you can\'t see"));
               lyrics.push(new Lyric(12,""));
               lyrics.push(new Lyric(7,"Two women that you cared about"));
               lyrics.push(new Lyric(7.5,""));
               lyrics.push(new Lyric(0.5,"One was your mother one was your lover"));
               lyrics.push(new Lyric(13,""));
               lyrics.push(new Lyric(0.5,"All the times you felt like one another"));
               lyrics.push(new Lyric(9.5,""));
               lyrics.push(new Lyric(0.5,"But both have gone, gone away"));
               lyrics.push(new Lyric(15.5,""));
               lyrics.push(new Lyric(14,"Like the wind once here it\'s gone"));
               lyrics.push(new Lyric(12,""));
               lyrics.push(new Lyric(4,"Now the doctor\'s saying"));
               lyrics.push(new Lyric(6,"You don\'t have much time"));
               lyrics.push(new Lyric(6,""));
               lyrics.push(new Lyric(4,"As if you had much to live for"));
               lyrics.push(new Lyric(12,""));
               lyrics.push(new Lyric(15,"All the times you felt like you\'ve had it"));
               lyrics.push(new Lyric(7.5,""));
               lyrics.push(new Lyric(0.5,"You should be gone, gone away"));
               lyrics.push(new Lyric(15,""));
               break;
            case 5:
               lyrics.push(new Lyric(5,"Everyday you wake up again"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"Wish you didn\'t have to begin"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"Every new day is a new black page"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"Shouldn\'t have happened, happened anyway"));
               lyrics.push(new Lyric(8,""));
               lyrics.push(new Lyric(4,"Pathetic"));
               lyrics.push(new Lyric(15,""));
               lyrics.push(new Lyric(1,"What is this sound in your head"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"Hitting walls and finding its way"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(2,"Pathetic"));
               lyrics.push(new Lyric(15,""));
               lyrics.push(new Lyric(3,"The time you tried to end it all"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"Accepting that everything was your fault"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"Tried to be at peace, and let it go"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"The only way you knew how to"));
               lyrics.push(new Lyric(8,""));
               lyrics.push(new Lyric(14,"Pathetic"));
               lyrics.push(new Lyric(15,""));
               lyrics.push(new Lyric(1,"What is this sound in your head"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"Hitting walls and finding its way"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(2,"Pathetic"));
               lyrics.push(new Lyric(53,""));
               lyrics.push(new Lyric(9,"What is this sound in your head"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"Hitting walls and finding its way"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(6,"Pathetic"));
               lyrics.push(new Lyric(30,""));
               break;
            case 6:
               lyrics.push(new Lyric(4,"Everyday you wake up together"));
               lyrics.push(new Lyric(6,"You don\'t know, you\'re not getting better"));
               lyrics.push(new Lyric(6,"At doing things, connecting with others"));
               lyrics.push(new Lyric(6,"All sad, since you lost your mother"));
               lyrics.push(new Lyric(7,""));
               lyrics.push(new Lyric(1,"Now and here"));
               lyrics.push(new Lyric(6,"To share what you feel"));
               lyrics.push(new Lyric(6,"No matter how bad"));
               lyrics.push(new Lyric(6,"We are all that there is"));
               lyrics.push(new Lyric(6,""));
               lyrics.push(new Lyric(2,"From the sun, from the wind"));
               lyrics.push(new Lyric(11,""));
               lyrics.push(new Lyric(2,"Although it\'s hard"));
               lyrics.push(new Lyric(6,"It\'s the only thing we can try"));
               lyrics.push(new Lyric(6,"It\'s just us, nothing to hide"));
               lyrics.push(new Lyric(6,"We\'re fucked up, we should be proud"));
               lyrics.push(new Lyric(11,""));
               lyrics.push(new Lyric(1,"Now and here"));
               lyrics.push(new Lyric(6,"To share what you feel"));
               lyrics.push(new Lyric(6,"No matter how bad"));
               lyrics.push(new Lyric(6,"We are all that there is"));
               lyrics.push(new Lyric(6,""));
               lyrics.push(new Lyric(5,"From the sun, from the wind"));
               lyrics.push(new Lyric(48,""));
         }
         var _loc2_:Number = 0;
         _loc1_ = 0;
         var _loc3_:int = int(lyrics.length);
         while(_loc1_ < _loc3_)
         {
            _loc4_ = _loc1_++;
            _loc2_ += lyrics[_loc4_].time;
            lyrics[_loc4_].time = _loc2_;
         }
      }
      
      public function particle(param1:Number, param2:Number) : Particle
      {
         if(particlePoolPointer >= particlePoolMaxLength)
         {
            particlePoolPointer = 0;
         }
         particlePool[particlePoolPointer].reset(param1,param2);
         addChild(particlePool[particlePoolPointer]);
         ++particlePoolPointer;
         return particlePool[particlePoolPointer - 1];
      }
      
      public function inputHandler(param1:Boolean) : void
      {
         var _loc2_:Number = NaN;
         var _loc3_:int = 0;
         var _loc4_:int = 0;
         var _loc5_:int = 0;
         var _loc6_:* = null as Beat;
         if(!oldKeyDown && param1)
         {
            SoundManager.playSound("explosion",!isReplay,panning);
            if(!isReplay)
            {
               inputSaver.push(GameManager.time / GameManager.rhythm);
            }
            if(GameManager.id != 5)
            {
               shakingPenisCounter = 500;
               shakingHeadCounter = 500;
               Actuate.stop(hand);
               Actuate.tween(hand,1,{
                  "y":Main.stageHeight + penisHeight * hand.scaleY,
                  "x":Main.stageWidth + 3 * hand.scaleX * (-1 + 2 * Math.random())
               });
            }
            crosshair.scaleX = crosshair.scaleY = 2;
            if(pressingSpike == null)
            {
               _loc3_ = 0;
               _loc4_ = int(spikes.length);
               while(_loc3_ < _loc4_)
               {
                  _loc5_ = _loc3_++;
                  _loc6_ = trackArray[spikes[_loc5_].idInTrack];
                  if(_loc6_.getRelativePosition() > Beat.timingMax)
                  {
                     break;
                  }
                  if(_loc6_.hit())
                  {
                     addToCombo();
                     changeRating(int(Math.round(Math.abs(_loc6_.getRelativePosition()))));
                     if(_loc6_.lengthOfPress != 0)
                     {
                        pressingSpike = spikes[_loc5_];
                     }
                  }
                  else if(_loc6_.getRelativePosition() < 0 && _loc6_.getRelativePositionToEnd() > 0)
                  {
                     pressingSpike = spikes[_loc5_];
                  }
               }
            }
         }
         else if(oldKeyDown && !param1)
         {
            SoundManager.playSound("explosion",!isReplay,panning);
            if(!isReplay)
            {
               inputSaver.push(GameManager.time / GameManager.rhythm);
            }
            if(GameManager.id != 5)
            {
               Actuate.stop(hand);
               Actuate.tween(hand,1,{
                  "y":Main.stageHeight + 72 * hand.scaleY,
                  "x":Main.stageWidth + 3 * hand.scaleX * (-1 + 2 * Math.random())
               });
            }
            crosshair.scaleX = crosshair.scaleY = 1;
            if(pressingSpike != null)
            {
               _loc6_ = trackArray[pressingSpike.idInTrack];
               if(_loc6_.getRelativePositionToEnd() <= Beat.timingMax)
               {
                  if(_loc6_.hitEnd())
                  {
                     addToCombo();
                     changeRating(int(Math.round(Math.abs(_loc6_.getRelativePositionToEnd()))));
                  }
               }
            }
            pressingSpike = null;
         }
         else if(oldKeyDown && param1)
         {
            if(pressingSpike != null)
            {
               if(trackArray[pressingSpike.idInTrack].getRelativePositionToEnd() < -Beat.timingMax)
               {
                  pressingSpike = null;
               }
            }
         }
         oldKeyDown = param1;
      }
      
      public function checkIfFinished() : void
      {
         var _loc1_:int = 0;
         var _loc2_:int = 0;
         var _loc3_:int = 0;
         var _loc4_:int = 0;
         var _loc5_:int = 0;
         if(!finished)
         {
            if(GameManager.time > endOfSong + timeAfterTrackArrayFinishes)
            {
               finished = true;
               if(GameManager.id != 5)
               {
                  shakingHandCounter = 5000;
                  shakingHeadCounter = 5000;
                  shakingPenisCounter = 5000;
                  _loc1_ = 0;
                  while(_loc1_ < 40)
                  {
                     _loc2_ = _loc1_++;
                     particle(penis.x,penis.y - 56 * penis.scaleY);
                  }
               }
               else
               {
                  _loc1_ = 0;
                  _loc2_ = int(bodiesLevel6.length);
                  while(_loc1_ < _loc2_)
                  {
                     _loc3_ = _loc1_++;
                     bodiesLevel6[_loc3_].shakingHandCounter = 5000;
                     bodiesLevel6[_loc3_].shakingHeadCounter = 5000;
                     bodiesLevel6[_loc3_].shakingPenisCounter = 5000;
                     _loc4_ = 0;
                     while(_loc4_ < 20)
                     {
                        _loc5_ = _loc4_++;
                        particle(bodiesLevel6[_loc3_].x + bodiesLevel6[_loc3_].penis.x * bodiesLevel6[_loc3_].scaleX,bodiesLevel6[_loc3_].y + (bodiesLevel6[_loc3_].penis.y - 56 * bodiesLevel6[_loc3_].penis.scaleY) * bodiesLevel6[_loc3_].scaleY);
                     }
                  }
               }
            }
         }
         else
         {
            finishingParticleCounter += GameManager.dt;
            if(finishingParticleCounter > 25)
            {
               if(GameManager.id != 5)
               {
                  particle(penis.x,penis.y - 56 * penis.scaleY);
               }
               else
               {
                  _loc1_ = 0;
                  _loc2_ = int(bodiesLevel6.length);
                  while(_loc1_ < _loc2_)
                  {
                     _loc3_ = _loc1_++;
                     particle(bodiesLevel6[_loc3_].x + bodiesLevel6[_loc3_].penis.x * bodiesLevel6[_loc3_].scaleX,bodiesLevel6[_loc3_].y + (bodiesLevel6[_loc3_].penis.y - 56 * bodiesLevel6[_loc3_].penis.scaleY) * bodiesLevel6[_loc3_].scaleY);
                  }
               }
               finishingParticleCounter = 0;
            }
            finishingScoreTableCounter += GameManager.dt;
            if(finishingScoreTableCounter > 3000)
            {
               destroyMePlease = true;
               if(score > int(SceneManager.scores[id - 1]))
               {
                  SceneManager.scores[id - 1] = score;
                  SceneManager.replayInputs[id - 1] = inputSaver;
                  SaveManager.save();
               }
            }
         }
      }
      
      public function changeRating(param1:int) : void
      {
         if(param1 <= Beat.timingMax / 10)
         {
            textRating.text = "Perfect!";
            addScore(10);
            SoundManager.playSound("perfect",!isReplay,panning);
         }
         else if(param1 <= Beat.timingMax * 3 / 10)
         {
            textRating.text = "Great!";
            addScore(5);
            SoundManager.playSound("great",!isReplay,panning);
         }
         else if(param1 <= Beat.timingMax * 5 / 10)
         {
            textRating.text = "Good";
            addScore(2);
            SoundManager.playSound("good",!isReplay,panning);
         }
         else if(param1 <= Beat.timingMax * 7 / 10)
         {
            textRating.text = "OK";
            addScore(1);
            SoundManager.playSound("ok",!isReplay,panning);
         }
         else
         {
            textRating.text = "Sad :(";
            SoundManager.playSound("sad",!isReplay,panning);
         }
         addScore(10);
      }
      
      public function addToCombo() : void
      {
         ++comboCounter;
         if(comboCounter > 1)
         {
            textCombo.scaleUpCounter = int(Math.round(Math.min(1250,750 + (comboCounter - 2) * 1000 / 20)));
         }
      }
      
      public function addScore(param1:int) : void
      {
         var _loc3_:int = 0;
         var _loc4_:int = 0;
         score += int(Math.round(Math.max(param1,param1 * comboCounter / 10)));
         textScore.scaleUpCounter = 1000;
         textRating.alpha = 1;
         textRating.scaleUpCounter = int(Math.round(750));
         var _loc2_:int = -param1;
         _loc3_ = param1 + 1;
         while(_loc2_ < _loc3_)
         {
            _loc4_ = _loc2_++;
            particle(textScore.x + _loc4_ * 100 / param1 - 4 + 8 * Math.random(),textScore.y - 4 + 8 * Math.random());
         }
         _loc2_ = 0;
         while(_loc2_ < param1)
         {
            _loc3_ = _loc2_++;
            particle(crosshair.x - 4 + 8 * Math.random(),crosshair.y - 4 + 8 * Math.random());
         }
      }
   }
}


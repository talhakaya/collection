package
{
   import flash.Boot;
   import flash.display.Sprite;
   
   public class ScoreTable extends Sprite
   {
      
      public static var init__:Boolean;
      
      public static var ButtonsXBetween:Number;
      
      public static var StarsXBetween:Number;
      
      public static var TextScoreStartConst:int = 1000;
      
      public static var TextScoreEndConst:int = 3000;
      
      public static var TextScoreCountingConst:int = 5000;
      
      public static var TextScoreSize:int = 1500;
      
      public static var TextScoreY:Number = 0.3;
      
      public static var StarsY:Number = 0.55;
      
      public static var ButtonsY:Number = 0.75;
      
      public static var myScores:Array = [15738,60864,45189,80776,150000,150000];
      
      public static var ratioOfStars:Array = [0.09,0.16,0.25,0.36,0.49];
      
      public var textScoreStartCounter:int;
      
      public var textScoreScore:int;
      
      public var textScoreEndCounter:int;
      
      public var textScoreCountingCounter:int;
      
      public var textScore:TextTalha;
      
      public var starsGained:int;
      
      public var starsCreated:Boolean;
      
      public var stars:Array;
      
      public var speedUpCounting:int;
      
      public var score:int;
      
      public var destroyMePleaseMessageTaken:Boolean;
      
      public var destroyMePlease:Boolean;
      
      public var background:GriddyBackground;
      
      public function ScoreTable(param1:int = 0)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         destroyMePleaseMessageTaken = false;
         destroyMePlease = false;
         stars = [];
         starsCreated = false;
         speedUpCounting = 1;
         textScoreScore = -5000;
         textScoreCountingCounter = 0;
         textScoreEndCounter = 0;
         textScoreStartCounter = 0;
         super();
         score = param1;
         background = new GriddyBackground();
         addChild(background);
         textScore = new TextTalha("Score: ");
         textScore.scaleUpCounter = 1500;
         textScore.x = Main.stageWidth / 2;
         textScore.y = Main.stageHeight * 0.3;
         addChild(textScore);
      }
      
      public static function howManyStars(param1:int) : int
      {
         var _loc5_:int = 0;
         var _loc2_:int = 0;
         var _loc3_:int = 0;
         var _loc4_:int = int(ScoreTable.ratioOfStars.length);
         while(_loc3_ < _loc4_)
         {
            _loc5_ = _loc3_++;
            if(int(SceneManager.scores[param1]) <= int(ScoreTable.myScores[param1]) * Number(ScoreTable.ratioOfStars[_loc5_]))
            {
               break;
            }
            _loc2_++;
         }
         return _loc2_;
      }
      
      public function updateTextScore() : void
      {
         var _loc3_:int = 0;
         if(GameManager.getKeyDown())
         {
            speedUpCounting = 300;
         }
         else
         {
            speedUpCounting = 30;
         }
         if(textScoreStartCounter < 1000)
         {
            textScoreStartCounter += speedUpCounting * GameManager.dt;
            textScore.scaleUpCounter = 1500;
         }
         else if(textScoreCountingCounter < 5000)
         {
            if(textScoreScore == score)
            {
               if(!starsCreated)
               {
                  starsCreated = true;
                  createStars();
                  createButtons();
               }
               textScore.scaleUpCounter = 1500;
               if(textScoreEndCounter < 3000)
               {
                  textScoreEndCounter += GameManager.dt;
               }
            }
            else
            {
               textScoreCountingCounter += speedUpCounting * GameManager.dt;
            }
         }
         else
         {
            textScoreCountingCounter = 0;
            if(textScoreScore < score)
            {
               textScoreScore += 5000;
               textScore.scaleUpCounter = 1500;
               if(textScoreScore >= score)
               {
                  textScoreScore = score;
               }
            }
            textScore.text = "Score: " + textScoreScore;
         }
         textScore.update();
         var _loc1_:int = 0;
         var _loc2_:int = int(stars.length);
         while(_loc1_ < _loc2_)
         {
            _loc3_ = _loc1_++;
            stars[_loc3_].update();
         }
      }
      
      public function update() : void
      {
         background.update();
         updateTextScore();
      }
      
      public function createStars() : void
      {
         var _loc3_:int = 0;
         calculateStarsGained();
         var _loc1_:int = starsGained;
         var _loc2_:int = 0;
         while(_loc2_ < 5)
         {
            _loc3_ = _loc2_++;
            stars.push(new ScoreStar(_loc1_ > 0));
            _loc1_--;
            stars[_loc3_].x = Main.stageWidth / 2 + (_loc3_ - 2) * 110;
            stars[_loc3_].y = Main.stageHeight * 0.55;
            addChild(stars[_loc3_]);
         }
      }
      
      public function createButtons() : void
      {
         var _loc2_:* = null as Button;
         var _loc1_:Button = new Button("Retry");
         _loc1_.y = Main.stageHeight * 0.75;
         _loc1_.x = Main.stageWidth / 2;
         addChild(_loc1_);
         if(starsGained > 0)
         {
            _loc1_.x -= 300 / 2;
            if(GameManager.id == 5)
            {
               _loc2_ = new Button("Finish");
               _loc2_.y = Main.stageHeight * 0.75;
               _loc2_.x = Main.stageWidth / 2 + 300 / 2;
               addChild(_loc2_);
            }
            else
            {
               _loc2_ = new Button("Next Level");
               _loc2_.y = Main.stageHeight * 0.75;
               _loc2_.x = Main.stageWidth / 2 + 300 / 2;
               addChild(_loc2_);
            }
         }
      }
      
      public function calculateStarsGained() : void
      {
         var _loc4_:int = 0;
         starsGained = 0;
         var _loc1_:Number = Math.max(score,int(SceneManager.scores[GameManager.id]));
         var _loc2_:int = 0;
         var _loc3_:int = int(ScoreTable.ratioOfStars.length);
         while(_loc2_ < _loc3_)
         {
            _loc4_ = _loc2_++;
            if(_loc1_ <= int(ScoreTable.myScores[GameManager.id]) * Number(ScoreTable.ratioOfStars[_loc4_]))
            {
               break;
            }
            ++starsGained;
         }
      }
   }
}


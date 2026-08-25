package
{
   import flash.Boot;
   import flash.display.BitmapData;
   import flash.display.Sprite;
   import openfl.Assets;
   
   public class Menu extends Sprite
   {
      
      public var textTitle:TextTalha;
      
      public var textDevelopedBy:TextTalha;
      
      public var scoreStars:Array;
      
      public var mainIcon:Citmap;
      
      public var kayabrosLogo:Citmap;
      
      public var destroyMePlease:Boolean;
      
      public function Menu()
      {
         var _loc9_:int = 0;
         var _loc10_:int = 0;
         var _loc11_:* = null as Button;
         var _loc12_:int = 0;
         var _loc13_:int = 0;
         var _loc14_:int = 0;
         var _loc15_:* = null as ScoreStar;
         if(Boot.skip_constructor)
         {
            return;
         }
         scoreStars = [];
         destroyMePlease = false;
         super();
         mainIcon = new Citmap(Assets.getBitmapData("img/sleepy time.png"),50,38);
         mainIcon.scaleX = mainIcon.scaleY = 2;
         mainIcon.x = Main.stageWidth / 2;
         mainIcon.y = Main.stageHeight / 4;
         addChild(mainIcon);
         kayabrosLogo = new Citmap(Assets.getBitmapData("img/kayabros.png"),160,173);
         kayabrosLogo.scaleX = kayabrosLogo.scaleY = 0.25;
         kayabrosLogo.x = Main.stageWidth * 9 / 32;
         kayabrosLogo.y = Main.stageHeight * 7 / 8;
         addChild(kayabrosLogo);
         textDevelopedBy = new TextTalha("Made By");
         textDevelopedBy.x = Main.stageWidth / 7;
         textDevelopedBy.y = Main.stageHeight * 7 / 8;
         addChild(textDevelopedBy);
         textTitle = new TextTalha("Sleepy Time");
         textTitle.x = Main.stageWidth / 2;
         textTitle.y = Main.stageHeight / 8;
         addChild(textTitle);
         var _loc2_:Button = new Button("Twitter");
         _loc2_.x = Main.stageWidth / 2;
         _loc2_.y = Main.stageHeight * 7 / 8;
         addChild(_loc2_);
         var _loc3_:Button = new Button("Soundtrack");
         _loc3_.x = Main.stageWidth * 3 / 4;
         _loc3_.y = Main.stageHeight * 7 / 8;
         addChild(_loc3_);
         var _loc4_:Button = new Button("Tutorial");
         _loc4_.x = Main.stageWidth * 13 / 16;
         _loc4_.y = Main.stageHeight * 4 / 8;
         addChild(_loc4_);
         var _loc5_:Button = new Button("Fullscreen");
         _loc5_.x = Main.stageWidth * 3 / 16;
         _loc5_.y = Main.stageHeight * 4 / 8;
         addChild(_loc5_);
         var _loc6_:int = 0;
         var _loc7_:int = 0;
         var _loc8_:int = int(SceneManager.scores.length);
         while(_loc7_ < _loc8_)
         {
            _loc9_ = _loc7_++;
            _loc6_++;
            _loc10_ = ScoreTable.howManyStars(_loc9_);
            if(_loc10_ == 0)
            {
               break;
            }
         }
         _loc7_ = 0;
         _loc8_ = int(SceneManager.scores.length);
         while(_loc7_ < _loc8_)
         {
            _loc9_ = _loc7_++;
            _loc10_ = ScoreTable.howManyStars(_loc9_);
            _loc11_ = new Button("Song " + (_loc9_ + 1));
            _loc11_.x = (_loc9_ + 0.5) * Main.stageWidth / _loc6_;
            _loc11_.y = Main.stageHeight * 5 / 8;
            addChild(_loc11_);
            if(int(SceneManager.scores[_loc9_]) != 0)
            {
               _loc12_ = _loc10_;
               _loc13_ = 0;
               while(_loc13_ < 5)
               {
                  _loc14_ = _loc13_++;
                  _loc15_ = new ScoreStar(_loc12_ > 0);
                  _loc15_.maxScale = 0.2;
                  _loc15_.x = _loc11_.x + 20 * (_loc14_ - 2);
                  _loc15_.y = _loc11_.y + 30;
                  addChild(_loc15_);
                  scoreStars.push(_loc15_);
                  _loc12_--;
               }
            }
            if(_loc10_ == 0)
            {
               break;
            }
         }
      }
      
      public function update() : void
      {
         var _loc3_:int = 0;
         mainIcon.update();
         kayabrosLogo.update();
         textTitle.scaleUpCounter = 2000;
         textTitle.update();
         textDevelopedBy.update();
         var _loc1_:int = 0;
         var _loc2_:int = int(scoreStars.length);
         while(_loc1_ < _loc2_)
         {
            _loc3_ = _loc1_++;
            scoreStars[_loc3_].update();
         }
      }
   }
}


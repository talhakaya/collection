package
{
   import flash.Boot;
   import flash.display.Bitmap;
   import flash.display.BitmapData;
   import flash.display.Sprite;
   import openfl.Assets;
   
   public class GriddyBackground extends Sprite
   {
      
      public static var init__:Boolean;
      
      public static var SquareEdgeFactor:Number;
      
      public static var SquareXFactor:Number;
      
      public static var SquareYFactor:Number;
      
      public static var AlphaPeriod:int = 2000;
      
      public static var NumberOfSquaresPerEdge:int = 9;
      
      public var squaresPeriods:Array;
      
      public var squaresAlphaGoingUps:Array;
      
      public var squares:Array;
      
      public function GriddyBackground()
      {
         var _loc2_:int = 0;
         var _loc3_:int = 0;
         var _loc4_:int = 0;
         var _loc5_:int = 0;
         if(Boot.skip_constructor)
         {
            return;
         }
         super();
         squares = [];
         squaresPeriods = [];
         squaresAlphaGoingUps = [];
         var _loc1_:int = 0;
         while(_loc1_ < 9)
         {
            _loc2_ = _loc1_++;
            squares.push([]);
            squaresPeriods.push([]);
            squaresAlphaGoingUps.push([]);
            _loc3_ = 0;
            while(_loc3_ < 9)
            {
               _loc4_ = _loc3_++;
               _loc5_ = int(Math.round(Math.random()));
               createSquare(_loc2_,_loc4_,2 - _loc5_);
               squares[_loc2_].push(createSquare(_loc2_,_loc4_,1 + _loc5_));
               squaresPeriods[_loc2_].push(int(Math.round(2000 * (0.4 + 0.6 * Math.random()))));
               squaresAlphaGoingUps[_loc2_].push(false);
            }
         }
      }
      
      public function update() : void
      {
         var _loc2_:int = 0;
         var _loc3_:int = 0;
         var _loc4_:int = 0;
         var _loc1_:int = 0;
         while(_loc1_ < 9)
         {
            _loc2_ = _loc1_++;
            _loc3_ = 0;
            while(_loc3_ < 9)
            {
               _loc4_ = _loc3_++;
               if(Boolean(squaresAlphaGoingUps[_loc2_][_loc4_]))
               {
                  _temp_3.alpha += GameManager.dt / int(squaresPeriods[_loc2_][_loc4_]);
                  if(squares[_loc2_][_loc4_].alpha >= 1)
                  {
                     squaresAlphaGoingUps[_loc2_][_loc4_] = false;
                  }
               }
               else
               {
                  _temp_4.alpha -= GameManager.dt / int(squaresPeriods[_loc2_][_loc4_]);
                  if(squares[_loc2_][_loc4_].alpha <= 0)
                  {
                     squaresAlphaGoingUps[_loc2_][_loc4_] = true;
                  }
               }
            }
         }
      }
      
      public function createSquare(param1:int, param2:int, param3:int) : Bitmap
      {
         var _loc4_:Bitmap = new Bitmap(Assets.getBitmapData("img/scene" + (1 + int(Math.round(4 * Math.random()))) + "/color" + param3 + ".png"));
         addChild(_loc4_);
         _loc4_.scaleX = _loc4_.scaleY = 45;
         _loc4_.x = (800 - 45 * 9) / 2 + param1 * 45;
         _loc4_.y = (450 - 45 * 9) / 2 + param2 * 45;
         return _loc4_;
      }
   }
}


package
{
   import flash.Boot;
   import flash.Lib;
   import flash.display.MovieClip;
   import haxe.Unserializer;
   import haxe.ds.ObjectMap;
   import haxe.ds.StringMap;
   import motion.Actuate;
   import motion.actuators.SimpleActuator;
   import motion.easing.Expo;
   import motion.easing.IEasing;
   import openfl.AssetCache;
   import openfl.Assets;
   
   public dynamic class boot_bd69 extends Boot
   {
      
      public function boot_bd69()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         super();
         if(Lib.current == null)
         {
            Lib.current = this;
         }
         start();
      }
      
      override public function init() : void
      {
         var _loc1_:* = Date;
         _loc1_.now = function():*
         {
            return new Date();
         };
         _loc1_.fromTime = function(param1:*):Date
         {
            var _loc2_:Date = new Date();
            _loc2_.setTime(param1);
            return _loc2_;
         };
         _loc1_.fromString = function(param1:String):Date
         {
            var _loc3_:* = null as Array;
            var _loc4_:* = null as Date;
            var _loc5_:* = null as Array;
            var _loc6_:* = null as Array;
            var _loc2_:int = param1.length;
            switch(_loc2_)
            {
               case 8:
                  _loc3_ = param1.split(":");
                  _loc4_ = new Date();
                  _loc4_.setTime(0);
                  _loc4_.setUTCHours(_loc3_[0]);
                  _loc4_.setUTCMinutes(_loc3_[1]);
                  _loc4_.setUTCSeconds(_loc3_[2]);
                  return _loc4_;
               case 10:
                  _loc3_ = param1.split("-");
                  return new Date(int(_loc3_[0]),_loc3_[1] - 1,int(_loc3_[2]),0,0,0);
               case 19:
                  _loc3_ = param1.split(" ");
                  _loc5_ = _loc3_[0].split("-");
                  _loc6_ = _loc3_[1].split(":");
                  return new Date(int(_loc5_[0]),_loc5_[1] - 1,int(_loc5_[2]),int(_loc6_[0]),int(_loc6_[1]),int(_loc6_[2]));
               default:
                  throw "Invalid date format : " + param1;
            }
         };
         _loc1_.prototype["toString"] = function():String
         {
            var _loc1_:Date = this;
            var _loc2_:int = int(_loc1_.getMonth()) + 1;
            var _loc3_:int = int(_loc1_.getDate());
            var _loc4_:int = int(_loc1_.getHours());
            var _loc5_:int = int(_loc1_.getMinutes());
            var _loc6_:int = int(_loc1_.getSeconds());
            return int(_loc1_.getFullYear()) + "-" + (_loc2_ < 10 ? "0" + _loc2_ : "" + _loc2_) + "-" + (_loc3_ < 10 ? "0" + _loc3_ : "" + _loc3_) + " " + (_loc4_ < 10 ? "0" + _loc4_ : "" + _loc4_) + ":" + (_loc5_ < 10 ? "0" + _loc5_ : "" + _loc5_) + ":" + (_loc6_ < 10 ? "0" + _loc6_ : "" + _loc6_);
         };
         Math.NaN = Number(Number.NaN);
         Math.NEGATIVE_INFINITY = Number(Number.NEGATIVE_INFINITY);
         Math.POSITIVE_INFINITY = Number(Number.POSITIVE_INFINITY);
         Math.isFinite = function(param1:Number):Boolean
         {
            return isFinite(param1);
         };
         Math.isNaN = function(param1:Number):Boolean
         {
            return isNaN(param1);
         };
         if(!Button.init__)
         {
            Button.init__ = true;
            Button.buttons = [];
            Button.collisionWidth = 60;
            Button.collisionHeight = 20;
         }
         if(!DefaultAssetLibrary.init__)
         {
            DefaultAssetLibrary.init__ = true;
            DefaultAssetLibrary.className = new StringMap();
            DefaultAssetLibrary.path = new StringMap();
            DefaultAssetLibrary.type = new StringMap();
         }
         if(!GriddyBackground.init__)
         {
            GriddyBackground.init__ = true;
            GriddyBackground.SquareEdgeFactor = 45;
            GriddyBackground.SquareXFactor = (800 - 45 * 9) / 2;
            GriddyBackground.SquareYFactor = (450 - 45 * 9) / 2;
         }
         if(!Scene.init__)
         {
            Scene.init__ = true;
            Scene.PenisHeightMax = 36;
         }
         if(!Body.init__)
         {
            Body.init__ = true;
            Body.PenisHeightMax = 36;
         }
         if(!ScoreTable.init__)
         {
            ScoreTable.init__ = true;
            ScoreTable.ButtonsXBetween = 300;
            ScoreTable.StarsXBetween = 110;
         }
         if(!Unserializer.init__)
         {
            Unserializer.init__ = true;
            Unserializer.DEFAULT_RESOLVER = Type;
         }
         if(!SimpleActuator.init__)
         {
            SimpleActuator.init__ = true;
            SimpleActuator.actuators = [];
         }
         if(!Actuate.init__)
         {
            Actuate.init__ = true;
            Actuate.defaultActuator = SimpleActuator;
            Actuate.defaultEase = Expo.get_easeOut();
            Actuate.targetLibraries = new ObjectMap();
         }
         if(!Assets.init__)
         {
            Assets.init__ = true;
            Assets.cache = new AssetCache();
            Assets.libraries = new StringMap();
         }
         ApplicationMain.main();
      }
   }
}


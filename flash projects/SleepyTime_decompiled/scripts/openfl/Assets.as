package openfl
{
   import flash.display.BitmapData;
   import flash.display.MovieClip;
   import flash.media.Sound;
   import flash.text.Font;
   import flash.utils.ByteArray;
   import haxe.Log;
   import haxe.Unserializer;
   
   public class Assets
   {
      
      public static var init__:Boolean;
      
      public static var cache:AssetCache;
      
      public static var libraries:IMap;
      
      public static var initialized:Boolean = false;
      
      public function Assets()
      {
      }
      
      public static function exists(param1:String, param2:AssetType = undefined) : Boolean
      {
         Assets.initialize();
         if(param2 == null)
         {
            param2 = AssetType.BINARY;
         }
         var _loc3_:String = param1.substring(0,param1.indexOf(":"));
         var _loc4_:String = param1.substr(param1.indexOf(":") + 1);
         var _loc5_:AssetLibrary = Assets.getLibrary(_loc3_);
         if(_loc5_ != null)
         {
            return _loc5_.exists(_loc4_,param2);
         }
         return false;
      }
      
      public static function getBitmapData(param1:String, param2:Boolean = true) : BitmapData
      {
         var _loc3_:* = null as BitmapData;
         Assets.initialize();
         if(param2 && Assets.cache.enabled && "$" + param1 in Assets.cache.bitmapData.h)
         {
            _loc3_ = Assets.cache.bitmapData.get(param1);
            if(Assets.isValidBitmapData(_loc3_))
            {
               return _loc3_;
            }
         }
         var _loc4_:String = param1.substring(0,param1.indexOf(":"));
         var _loc5_:String = param1.substr(param1.indexOf(":") + 1);
         var _loc6_:AssetLibrary = Assets.getLibrary(_loc4_);
         if(_loc6_ != null)
         {
            if(_loc6_.exists(_loc5_,AssetType.IMAGE))
            {
               if(_loc6_.isLocal(_loc5_,AssetType.IMAGE))
               {
                  _loc3_ = _loc6_.getBitmapData(_loc5_);
                  if(param2 && Assets.cache.enabled)
                  {
                     Assets.cache.bitmapData.set(param1,_loc3_);
                  }
                  return _loc3_;
               }
               Log.trace("[openfl.Assets] BitmapData asset \"" + param1 + "\" exists, but only asynchronously",{
                  "fileName":"Assets.hx",
                  "lineNumber":118,
                  "className":"openfl.Assets",
                  "methodName":"getBitmapData"
               });
            }
            else
            {
               Log.trace("[openfl.Assets] There is no BitmapData asset with an ID of \"" + param1 + "\"",{
                  "fileName":"Assets.hx",
                  "lineNumber":124,
                  "className":"openfl.Assets",
                  "methodName":"getBitmapData"
               });
            }
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + _loc4_ + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":130,
               "className":"openfl.Assets",
               "methodName":"getBitmapData"
            });
         }
         return null;
      }
      
      public static function getBytes(param1:String) : ByteArray
      {
         Assets.initialize();
         var _loc2_:String = param1.substring(0,param1.indexOf(":"));
         var _loc3_:String = param1.substr(param1.indexOf(":") + 1);
         var _loc4_:AssetLibrary = Assets.getLibrary(_loc2_);
         if(_loc4_ != null)
         {
            if(_loc4_.exists(_loc3_,AssetType.BINARY))
            {
               if(_loc4_.isLocal(_loc3_,AssetType.BINARY))
               {
                  return _loc4_.getBytes(_loc3_);
               }
               Log.trace("[openfl.Assets] String or ByteArray asset \"" + param1 + "\" exists, but only asynchronously",{
                  "fileName":"Assets.hx",
                  "lineNumber":167,
                  "className":"openfl.Assets",
                  "methodName":"getBytes"
               });
            }
            else
            {
               Log.trace("[openfl.Assets] There is no String or ByteArray asset with an ID of \"" + param1 + "\"",{
                  "fileName":"Assets.hx",
                  "lineNumber":173,
                  "className":"openfl.Assets",
                  "methodName":"getBytes"
               });
            }
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + _loc2_ + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":179,
               "className":"openfl.Assets",
               "methodName":"getBytes"
            });
         }
         return null;
      }
      
      public static function getFont(param1:String, param2:Boolean = true) : Font
      {
         var _loc6_:* = null as Font;
         Assets.initialize();
         if(param2 && Assets.cache.enabled && "$" + param1 in Assets.cache.font.h)
         {
            return Assets.cache.font.get(param1);
         }
         var _loc3_:String = param1.substring(0,param1.indexOf(":"));
         var _loc4_:String = param1.substr(param1.indexOf(":") + 1);
         var _loc5_:AssetLibrary = Assets.getLibrary(_loc3_);
         if(_loc5_ != null)
         {
            if(_loc5_.exists(_loc4_,AssetType.FONT))
            {
               if(_loc5_.isLocal(_loc4_,AssetType.FONT))
               {
                  _loc6_ = _loc5_.getFont(_loc4_);
                  if(param2 && Assets.cache.enabled)
                  {
                     Assets.cache.font.set(param1,_loc6_);
                  }
                  return _loc6_;
               }
               Log.trace("[openfl.Assets] Font asset \"" + param1 + "\" exists, but only asynchronously",{
                  "fileName":"Assets.hx",
                  "lineNumber":230,
                  "className":"openfl.Assets",
                  "methodName":"getFont"
               });
            }
            else
            {
               Log.trace("[openfl.Assets] There is no Font asset with an ID of \"" + param1 + "\"",{
                  "fileName":"Assets.hx",
                  "lineNumber":236,
                  "className":"openfl.Assets",
                  "methodName":"getFont"
               });
            }
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + _loc3_ + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":242,
               "className":"openfl.Assets",
               "methodName":"getFont"
            });
         }
         return null;
      }
      
      public static function getLibrary(param1:String) : AssetLibrary
      {
         if(param1 == null || param1 == "")
         {
            param1 = "default";
         }
         return Assets.libraries.get(param1);
      }
      
      public static function getMovieClip(param1:String) : MovieClip
      {
         Assets.initialize();
         var _loc2_:String = param1.substring(0,param1.indexOf(":"));
         var _loc3_:String = param1.substr(param1.indexOf(":") + 1);
         var _loc4_:AssetLibrary = Assets.getLibrary(_loc2_);
         if(_loc4_ != null)
         {
            if(_loc4_.exists(_loc3_,AssetType.MOVIE_CLIP))
            {
               if(_loc4_.isLocal(_loc3_,AssetType.MOVIE_CLIP))
               {
                  return _loc4_.getMovieClip(_loc3_);
               }
               Log.trace("[openfl.Assets] MovieClip asset \"" + param1 + "\" exists, but only asynchronously",{
                  "fileName":"Assets.hx",
                  "lineNumber":292,
                  "className":"openfl.Assets",
                  "methodName":"getMovieClip"
               });
            }
            else
            {
               Log.trace("[openfl.Assets] There is no MovieClip asset with an ID of \"" + param1 + "\"",{
                  "fileName":"Assets.hx",
                  "lineNumber":298,
                  "className":"openfl.Assets",
                  "methodName":"getMovieClip"
               });
            }
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + _loc2_ + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":304,
               "className":"openfl.Assets",
               "methodName":"getMovieClip"
            });
         }
         return null;
      }
      
      public static function getMusic(param1:String, param2:Boolean = true) : Sound
      {
         var _loc3_:* = null as Sound;
         Assets.initialize();
         if(param2 && Assets.cache.enabled && "$" + param1 in Assets.cache.sound.h)
         {
            _loc3_ = Assets.cache.sound.get(param1);
            if(Assets.isValidSound(_loc3_))
            {
               return _loc3_;
            }
         }
         var _loc4_:String = param1.substring(0,param1.indexOf(":"));
         var _loc5_:String = param1.substr(param1.indexOf(":") + 1);
         var _loc6_:AssetLibrary = Assets.getLibrary(_loc4_);
         if(_loc6_ != null)
         {
            if(_loc6_.exists(_loc5_,AssetType.MUSIC))
            {
               if(_loc6_.isLocal(_loc5_,AssetType.MUSIC))
               {
                  _loc3_ = _loc6_.getMusic(_loc5_);
                  if(param2 && Assets.cache.enabled)
                  {
                     Assets.cache.sound.set(param1,_loc3_);
                  }
                  return _loc3_;
               }
               Log.trace("[openfl.Assets] Sound asset \"" + param1 + "\" exists, but only asynchronously",{
                  "fileName":"Assets.hx",
                  "lineNumber":361,
                  "className":"openfl.Assets",
                  "methodName":"getMusic"
               });
            }
            else
            {
               Log.trace("[openfl.Assets] There is no Sound asset with an ID of \"" + param1 + "\"",{
                  "fileName":"Assets.hx",
                  "lineNumber":367,
                  "className":"openfl.Assets",
                  "methodName":"getMusic"
               });
            }
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + _loc4_ + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":373,
               "className":"openfl.Assets",
               "methodName":"getMusic"
            });
         }
         return null;
      }
      
      public static function getPath(param1:String) : String
      {
         Assets.initialize();
         var _loc2_:String = param1.substring(0,param1.indexOf(":"));
         var _loc3_:String = param1.substr(param1.indexOf(":") + 1);
         var _loc4_:AssetLibrary = Assets.getLibrary(_loc2_);
         if(_loc4_ != null)
         {
            if(_loc4_.exists(_loc3_,null))
            {
               return _loc4_.getPath(_loc3_);
            }
            Log.trace("[openfl.Assets] There is no asset with an ID of \"" + param1 + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":408,
               "className":"openfl.Assets",
               "methodName":"getPath"
            });
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + _loc2_ + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":414,
               "className":"openfl.Assets",
               "methodName":"getPath"
            });
         }
         return null;
      }
      
      public static function getSound(param1:String, param2:Boolean = true) : Sound
      {
         var _loc3_:* = null as Sound;
         Assets.initialize();
         if(param2 && Assets.cache.enabled && "$" + param1 in Assets.cache.sound.h)
         {
            _loc3_ = Assets.cache.sound.get(param1);
            if(Assets.isValidSound(_loc3_))
            {
               return _loc3_;
            }
         }
         var _loc4_:String = param1.substring(0,param1.indexOf(":"));
         var _loc5_:String = param1.substr(param1.indexOf(":") + 1);
         var _loc6_:AssetLibrary = Assets.getLibrary(_loc4_);
         if(_loc6_ != null)
         {
            if(_loc6_.exists(_loc5_,AssetType.SOUND))
            {
               if(_loc6_.isLocal(_loc5_,AssetType.SOUND))
               {
                  _loc3_ = _loc6_.getSound(_loc5_);
                  if(param2 && Assets.cache.enabled)
                  {
                     Assets.cache.sound.set(param1,_loc3_);
                  }
                  return _loc3_;
               }
               Log.trace("[openfl.Assets] Sound asset \"" + param1 + "\" exists, but only asynchronously",{
                  "fileName":"Assets.hx",
                  "lineNumber":471,
                  "className":"openfl.Assets",
                  "methodName":"getSound"
               });
            }
            else
            {
               Log.trace("[openfl.Assets] There is no Sound asset with an ID of \"" + param1 + "\"",{
                  "fileName":"Assets.hx",
                  "lineNumber":477,
                  "className":"openfl.Assets",
                  "methodName":"getSound"
               });
            }
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + _loc4_ + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":483,
               "className":"openfl.Assets",
               "methodName":"getSound"
            });
         }
         return null;
      }
      
      public static function getText(param1:String) : String
      {
         Assets.initialize();
         var _loc2_:String = param1.substring(0,param1.indexOf(":"));
         var _loc3_:String = param1.substr(param1.indexOf(":") + 1);
         var _loc4_:AssetLibrary = Assets.getLibrary(_loc2_);
         if(_loc4_ != null)
         {
            if(_loc4_.exists(_loc3_,AssetType.TEXT))
            {
               if(_loc4_.isLocal(_loc3_,AssetType.TEXT))
               {
                  return _loc4_.getText(_loc3_);
               }
               Log.trace("[openfl.Assets] String asset \"" + param1 + "\" exists, but only asynchronously",{
                  "fileName":"Assets.hx",
                  "lineNumber":520,
                  "className":"openfl.Assets",
                  "methodName":"getText"
               });
            }
            else
            {
               Log.trace("[openfl.Assets] There is no String asset with an ID of \"" + param1 + "\"",{
                  "fileName":"Assets.hx",
                  "lineNumber":526,
                  "className":"openfl.Assets",
                  "methodName":"getText"
               });
            }
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + _loc2_ + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":532,
               "className":"openfl.Assets",
               "methodName":"getText"
            });
         }
         return null;
      }
      
      public static function initialize() : void
      {
         if(!Assets.initialized)
         {
            Assets.registerLibrary("default",new DefaultAssetLibrary());
            Assets.initialized = true;
         }
      }
      
      public static function isLocal(param1:String, param2:AssetType = undefined, param3:Boolean = true) : Boolean
      {
         Assets.initialize();
         if(param3 && Assets.cache.enabled)
         {
            if(param2 == AssetType.IMAGE || param2 == null)
            {
               if("$" + param1 in Assets.cache.bitmapData.h)
               {
                  return true;
               }
            }
            if(param2 == AssetType.FONT || param2 == null)
            {
               if("$" + param1 in Assets.cache.font.h)
               {
                  return true;
               }
            }
            if(param2 == AssetType.SOUND || param2 == AssetType.MUSIC || param2 == null)
            {
               if("$" + param1 in Assets.cache.sound.h)
               {
                  return true;
               }
            }
         }
         var _loc4_:String = param1.substring(0,param1.indexOf(":"));
         var _loc5_:String = param1.substr(param1.indexOf(":") + 1);
         var _loc6_:AssetLibrary = Assets.getLibrary(_loc4_);
         if(_loc6_ != null)
         {
            return _loc6_.isLocal(_loc5_,param2);
         }
         return false;
      }
      
      public static function isValidBitmapData(param1:BitmapData) : Boolean
      {
         var _loc3_:* = null;
         try
         {
            param1.width;
         }
         catch(_loc_e_:*)
         {
         }
      }
      
      public static function isValidSound(param1:Sound) : Boolean
      {
         return true;
      }
      
      public static function loadBitmapData(param1:String, param2:Function, param3:Boolean = true) : void
      {
         var handler:Function;
         var id:String;
         var _loc4_:* = null as BitmapData;
         id = param1;
         handler = param2;
         Assets.initialize();
         if(param3 && Assets.cache.enabled && "$" + id in Assets.cache.bitmapData.h)
         {
            _loc4_ = Assets.cache.bitmapData.get(id);
            if(Assets.isValidBitmapData(_loc4_))
            {
               handler(_loc4_);
               return;
            }
         }
         var _loc5_:String = id.substring(0,id.indexOf(":"));
         var _loc6_:String = id.substr(id.indexOf(":") + 1);
         var _loc7_:AssetLibrary = Assets.getLibrary(_loc5_);
         if(_loc7_ != null)
         {
            if(_loc7_.exists(_loc6_,AssetType.IMAGE))
            {
               if(param3 && Assets.cache.enabled)
               {
                  _loc7_.loadBitmapData(_loc6_,function(param1:BitmapData):void
                  {
                     Assets.cache.bitmapData.set(id,param1);
                     handler(param1);
                  });
               }
               else
               {
                  _loc7_.loadBitmapData(_loc6_,handler);
               }
               return;
            }
            Log.trace("[openfl.Assets] There is no BitmapData asset with an ID of \"" + id + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":695,
               "className":"openfl.Assets",
               "methodName":"loadBitmapData"
            });
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + _loc5_ + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":701,
               "className":"openfl.Assets",
               "methodName":"loadBitmapData"
            });
         }
         handler(null);
      }
      
      public static function loadBytes(param1:String, param2:Function) : void
      {
         Assets.initialize();
         var _loc3_:String = param1.substring(0,param1.indexOf(":"));
         var _loc4_:String = param1.substr(param1.indexOf(":") + 1);
         var _loc5_:AssetLibrary = Assets.getLibrary(_loc3_);
         if(_loc5_ != null)
         {
            if(_loc5_.exists(_loc4_,AssetType.BINARY))
            {
               _loc5_.loadBytes(_loc4_,param2);
               return;
            }
            Log.trace("[openfl.Assets] There is no String or ByteArray asset with an ID of \"" + param1 + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":731,
               "className":"openfl.Assets",
               "methodName":"loadBytes"
            });
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + _loc3_ + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":737,
               "className":"openfl.Assets",
               "methodName":"loadBytes"
            });
         }
         param2(null);
      }
      
      public static function loadFont(param1:String, param2:Function, param3:Boolean = true) : void
      {
         var id:String = param1;
         var handler:Function = param2;
         Assets.initialize();
         if(param3 && Assets.cache.enabled && "$" + id in Assets.cache.font.h)
         {
            handler(Assets.cache.font.get(id));
            return;
         }
         var _loc4_:String = id.substring(0,id.indexOf(":"));
         var _loc5_:String = id.substr(id.indexOf(":") + 1);
         var _loc6_:AssetLibrary = Assets.getLibrary(_loc4_);
         if(_loc6_ != null)
         {
            if(_loc6_.exists(_loc5_,AssetType.FONT))
            {
               if(param3 && Assets.cache.enabled)
               {
                  _loc6_.loadFont(_loc5_,function(param1:Font):void
                  {
                     Assets.cache.font.set(id,param1);
                     handler(param1);
                  });
               }
               else
               {
                  _loc6_.loadFont(_loc5_,handler);
               }
               return;
            }
            Log.trace("[openfl.Assets] There is no Font asset with an ID of \"" + id + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":788,
               "className":"openfl.Assets",
               "methodName":"loadFont"
            });
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + _loc4_ + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":794,
               "className":"openfl.Assets",
               "methodName":"loadFont"
            });
         }
         handler(null);
      }
      
      public static function loadLibrary(param1:String, param2:Function) : void
      {
         var _loc4_:* = null as Unserializer;
         var _loc5_:* = null as AssetLibrary;
         Assets.initialize();
         var _loc3_:String = Assets.getText("libraries/" + param1 + ".dat");
         if(_loc3_ != null && _loc3_ != "")
         {
            _loc4_ = new Unserializer(_loc3_);
            _loc4_.setResolver({
               "resolveEnum":Assets.resolveEnum,
               "resolveClass":Assets.resolveClass
            });
            _loc5_ = _loc4_.unserialize();
            Assets.libraries.set(param1,_loc5_);
            _loc5_.load(param2);
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + param1 + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":824,
               "className":"openfl.Assets",
               "methodName":"loadLibrary"
            });
         }
      }
      
      public static function loadMusic(param1:String, param2:Function, param3:Boolean = true) : void
      {
         var handler:Function;
         var id:String;
         var _loc4_:* = null as Sound;
         id = param1;
         handler = param2;
         Assets.initialize();
         if(param3 && Assets.cache.enabled && "$" + id in Assets.cache.sound.h)
         {
            _loc4_ = Assets.cache.sound.get(id);
            if(Assets.isValidSound(_loc4_))
            {
               handler(_loc4_);
               return;
            }
         }
         var _loc5_:String = id.substring(0,id.indexOf(":"));
         var _loc6_:String = id.substr(id.indexOf(":") + 1);
         var _loc7_:AssetLibrary = Assets.getLibrary(_loc5_);
         if(_loc7_ != null)
         {
            if(_loc7_.exists(_loc6_,AssetType.MUSIC))
            {
               if(param3 && Assets.cache.enabled)
               {
                  _loc7_.loadMusic(_loc6_,function(param1:Sound):void
                  {
                     Assets.cache.sound.set(id,param1);
                     handler(param1);
                  });
               }
               else
               {
                  _loc7_.loadMusic(_loc6_,handler);
               }
               return;
            }
            Log.trace("[openfl.Assets] There is no Sound asset with an ID of \"" + id + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":879,
               "className":"openfl.Assets",
               "methodName":"loadMusic"
            });
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + _loc5_ + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":885,
               "className":"openfl.Assets",
               "methodName":"loadMusic"
            });
         }
         handler(null);
      }
      
      public static function loadMovieClip(param1:String, param2:Function) : void
      {
         Assets.initialize();
         var _loc3_:String = param1.substring(0,param1.indexOf(":"));
         var _loc4_:String = param1.substr(param1.indexOf(":") + 1);
         var _loc5_:AssetLibrary = Assets.getLibrary(_loc3_);
         if(_loc5_ != null)
         {
            if(_loc5_.exists(_loc4_,AssetType.MOVIE_CLIP))
            {
               _loc5_.loadMovieClip(_loc4_,param2);
               return;
            }
            Log.trace("[openfl.Assets] There is no MovieClip asset with an ID of \"" + param1 + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":915,
               "className":"openfl.Assets",
               "methodName":"loadMovieClip"
            });
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + _loc3_ + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":921,
               "className":"openfl.Assets",
               "methodName":"loadMovieClip"
            });
         }
         param2(null);
      }
      
      public static function loadSound(param1:String, param2:Function, param3:Boolean = true) : void
      {
         var handler:Function;
         var id:String;
         var _loc4_:* = null as Sound;
         id = param1;
         handler = param2;
         Assets.initialize();
         if(param3 && Assets.cache.enabled && "$" + id in Assets.cache.sound.h)
         {
            _loc4_ = Assets.cache.sound.get(id);
            if(Assets.isValidSound(_loc4_))
            {
               handler(_loc4_);
               return;
            }
         }
         var _loc5_:String = id.substring(0,id.indexOf(":"));
         var _loc6_:String = id.substr(id.indexOf(":") + 1);
         var _loc7_:AssetLibrary = Assets.getLibrary(_loc5_);
         if(_loc7_ != null)
         {
            if(_loc7_.exists(_loc6_,AssetType.SOUND))
            {
               if(param3 && Assets.cache.enabled)
               {
                  _loc7_.loadSound(_loc6_,function(param1:Sound):void
                  {
                     Assets.cache.sound.set(id,param1);
                     handler(param1);
                  });
               }
               else
               {
                  _loc7_.loadSound(_loc6_,handler);
               }
               return;
            }
            Log.trace("[openfl.Assets] There is no Sound asset with an ID of \"" + id + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":978,
               "className":"openfl.Assets",
               "methodName":"loadSound"
            });
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + _loc5_ + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":984,
               "className":"openfl.Assets",
               "methodName":"loadSound"
            });
         }
         handler(null);
      }
      
      public static function loadText(param1:String, param2:Function) : void
      {
         Assets.initialize();
         var _loc3_:String = param1.substring(0,param1.indexOf(":"));
         var _loc4_:String = param1.substr(param1.indexOf(":") + 1);
         var _loc5_:AssetLibrary = Assets.getLibrary(_loc3_);
         if(_loc5_ != null)
         {
            if(_loc5_.exists(_loc4_,AssetType.TEXT))
            {
               _loc5_.loadText(_loc4_,param2);
               return;
            }
            Log.trace("[openfl.Assets] There is no String asset with an ID of \"" + param1 + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":1014,
               "className":"openfl.Assets",
               "methodName":"loadText"
            });
         }
         else
         {
            Log.trace("[openfl.Assets] There is no asset library named \"" + _loc3_ + "\"",{
               "fileName":"Assets.hx",
               "lineNumber":1020,
               "className":"openfl.Assets",
               "methodName":"loadText"
            });
         }
         param2(null);
      }
      
      public static function registerLibrary(param1:String, param2:AssetLibrary) : void
      {
         if("$" + param1 in Assets.libraries.h)
         {
            Assets.unloadLibrary(param1);
         }
         Assets.libraries.set(param1,param2);
      }
      
      public static function resolveClass(param1:String) : Class
      {
         return Type.resolveClass(param1);
      }
      
      public static function resolveEnum(param1:String) : Class
      {
         var _loc2_:Class = Type.resolveEnum(param1);
         if(_loc2_ == null)
         {
            return Type.resolveClass(param1);
         }
         return _loc2_;
      }
      
      public static function unloadLibrary(param1:String) : void
      {
         var _loc4_:* = null as String;
         var _loc5_:* = null as String;
         var _loc6_:* = null as String;
         Assets.initialize();
         var _loc2_:* = Assets.cache.bitmapData.keys();
         var _loc3_:* = _loc2_;
         while(_loc3_.hasNext())
         {
            _loc4_ = _loc3_.next();
            _loc5_ = _loc4_.substring(0,_loc4_.indexOf(":"));
            _loc6_ = _loc4_.substr(_loc4_.indexOf(":") + 1);
            if(_loc5_ == param1)
            {
               Assets.cache.bitmapData.remove(_loc4_);
            }
         }
         Assets.libraries.remove(param1);
      }
   }
}


package haxe
{
   import flash.Boot;
   import flash.utils.ByteArray;
   import haxe.ds.IntMap;
   import haxe.ds.ObjectMap;
   import haxe.ds.StringMap;
   import haxe.io.Bytes;
   
   public class Unserializer
   {
      
      public static var init__:Boolean;
      
      public static var DEFAULT_RESOLVER:Object;
      
      public static var BASE64:String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789%:";
      
      public static var CODES:ByteArray = null;
      
      public var scache:Array;
      
      public var resolver:Object;
      
      public var pos:int;
      
      public var length:int;
      
      public var cache:Array;
      
      public var buf:String;
      
      public function Unserializer(param1:String = undefined)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         buf = param1;
         length = param1.length;
         pos = 0;
         scache = [];
         cache = [];
         var _loc2_:* = Unserializer.DEFAULT_RESOLVER;
         if(_loc2_ == null)
         {
            _loc2_ = Type;
            Unserializer.DEFAULT_RESOLVER = _loc2_;
         }
         setResolver(_loc2_);
      }
      
      public static function initCodes() : ByteArray
      {
         var _loc4_:int = 0;
         var _loc1_:ByteArray = new ByteArray();
         var _loc2_:int = 0;
         var _loc3_:int = Unserializer.BASE64.length;
         while(_loc2_ < _loc3_)
         {
            _loc4_ = _loc2_++;
            _loc1_[int(Unserializer.BASE64.charCodeAt(_loc4_))] = _loc4_;
         }
         return _loc1_;
      }
      
      public function unserializeObject(param1:Object) : void
      {
         var _loc2_:* = null as String;
         var _loc3_:* = null;
         while(true)
         {
            if(pos >= length)
            {
               throw "Invalid object";
            }
            if(int(buf.charCodeAt(pos)) == 103)
            {
               break;
            }
            _loc2_ = unserialize();
            if(!(_loc2_ is String))
            {
               throw "Invalid object key";
            }
            _loc3_ = unserialize();
            param1[_loc2_] = _loc3_;
         }
         ++pos;
      }
      
      public function unserializeEnum(param1:Class, param2:String) : *
      {
         var _loc4_:int;
         pos = (_loc4_ = pos) + 1;
         var _loc3_:int = _loc4_;
         if(int(buf.charCodeAt(_loc3_)) != 58)
         {
            throw "Invalid enum format";
         }
         _loc3_ = readDigits();
         if(_loc3_ == 0)
         {
            return Type.createEnum(param1,param2);
         }
         var _loc5_:Array = [];
         while(_loc3_-- > 0)
         {
            _loc5_.push(unserialize());
         }
         return Type.createEnum(param1,param2,_loc5_);
      }
      
      public function unserialize() : *
      {
         var _loc2_:int = 0;
         var _loc3_:int = 0;
         var _loc4_:int = 0;
         var _loc5_:* = null as String;
         var _loc6_:* = null as Array;
         var _loc7_:* = null;
         var _loc8_:* = null as Class;
         var _loc9_:* = null as String;
         var _loc10_:* = null as List;
         var _loc11_:* = null as StringMap;
         var _loc12_:* = null as IntMap;
         var _loc13_:int = 0;
         var _loc14_:* = null as ObjectMap;
         var _loc15_:* = null;
         var _loc16_:* = null as Date;
         var _loc17_:* = null as ByteArray;
         var _loc18_:int = 0;
         var _loc19_:* = null as Bytes;
         var _loc20_:int = 0;
         var _loc21_:int = 0;
         var _loc22_:int = 0;
         var _loc23_:int = 0;
         var _loc24_:int = 0;
         var _loc25_:int = 0;
         pos = (_loc3_ = pos) + 1;
         _loc2_ = _loc3_;
         var _loc1_:int = int(buf.charCodeAt(_loc2_));
         switch(_loc1_)
         {
            case 67:
               _loc5_ = unserialize();
               _loc8_ = resolver.resolveClass(_loc5_);
               if(_loc8_ == null)
               {
                  throw "Class not found " + _loc5_;
               }
               _loc7_ = Type.createEmptyInstance(_loc8_);
               cache.push(_loc7_);
               _loc7_.hxUnserialize(this);
               pos = (_loc3_ = pos) + 1;
               _loc2_ = _loc3_;
               if(int(buf.charCodeAt(_loc2_)) != 103)
               {
                  throw "Invalid custom data";
               }
               return _loc7_;
               break;
            case 77:
               _loc14_ = new ObjectMap();
               cache.push(_loc14_);
               _loc5_ = buf;
               while(int(buf.charCodeAt(pos)) != 104)
               {
                  _loc7_ = unserialize();
                  _loc15_ = unserialize();
                  _loc14_[_loc7_] = _loc15_;
               }
               ++pos;
               return _loc14_;
            case 82:
               _loc2_ = readDigits();
               if(_loc2_ < 0 || _loc2_ >= int(scache.length))
               {
                  throw "Invalid string reference";
               }
               return scache[_loc2_];
               break;
            case 97:
               _loc5_ = buf;
               _loc6_ = [];
               cache.push(_loc6_);
               while(true)
               {
                  _loc2_ = int(buf.charCodeAt(pos));
                  if(_loc2_ == 104)
                  {
                     ++pos;
                     break;
                  }
                  if(_loc2_ == 117)
                  {
                     ++pos;
                     _loc3_ = readDigits();
                     _loc6_[int(_loc6_.length) + _loc3_ - 1] = null;
                  }
                  else
                  {
                     _loc6_.push(unserialize());
                  }
               }
               return _loc6_;
            case 98:
               _loc11_ = new StringMap();
               cache.push(_loc11_);
               _loc5_ = buf;
               while(int(buf.charCodeAt(pos)) != 104)
               {
                  _loc9_ = unserialize();
                  _loc11_.set(_loc9_,unserialize());
               }
               ++pos;
               return _loc11_;
            case 99:
               _loc5_ = unserialize();
               _loc8_ = resolver.resolveClass(_loc5_);
               if(_loc8_ == null)
               {
                  throw "Class not found " + _loc5_;
               }
               _loc7_ = Type.createEmptyInstance(_loc8_);
               cache.push(_loc7_);
               unserializeObject(_loc7_);
               return _loc7_;
               break;
            case 100:
               _loc2_ = pos;
               while(true)
               {
                  _loc3_ = int(buf.charCodeAt(pos));
                  if(!(_loc3_ >= 43 && _loc3_ < 58 || _loc3_ == 101 || _loc3_ == 69))
                  {
                     break;
                  }
                  ++pos;
               }
               return Std.parseFloat(buf.substr(_loc2_,pos - _loc2_));
            case 102:
               return false;
            case 105:
               return readDigits();
            case 106:
               _loc5_ = unserialize();
               _loc8_ = resolver.resolveEnum(_loc5_);
               if(_loc8_ == null)
               {
                  throw "Enum not found " + _loc5_;
               }
               ++pos;
               _loc2_ = readDigits();
               _loc9_ = Type.getEnumConstructs(_loc8_)[_loc2_];
               if(_loc9_ == null)
               {
                  throw "Unknown enum index " + _loc5_ + "@" + _loc2_;
               }
               _loc7_ = unserializeEnum(_loc8_,_loc9_);
               cache.push(_loc7_);
               return _loc7_;
               break;
            case 107:
               return Number(Math.NaN);
            case 108:
               _loc10_ = new List();
               cache.push(_loc10_);
               _loc5_ = buf;
               while(int(buf.charCodeAt(pos)) != 104)
               {
                  _loc10_.add(unserialize());
               }
               ++pos;
               return _loc10_;
            case 109:
               return Number(Math.NEGATIVE_INFINITY);
            case 110:
               return null;
            case 111:
               _loc7_ = {};
               cache.push(_loc7_);
               unserializeObject(_loc7_);
               return _loc7_;
            case 112:
               return Number(Math.POSITIVE_INFINITY);
            case 113:
               _loc12_ = new IntMap();
               cache.push(_loc12_);
               _loc5_ = buf;
               pos = (_loc4_ = pos) + 1;
               _loc3_ = _loc4_;
               _loc2_ = int(buf.charCodeAt(_loc3_));
               while(_loc2_ == 58)
               {
                  _loc3_ = readDigits();
                  _loc12_.set(_loc3_,unserialize());
                  pos = (_loc13_ = pos) + 1;
                  _loc4_ = _loc13_;
                  _loc2_ = int(buf.charCodeAt(_loc4_));
               }
               if(_loc2_ != 104)
               {
                  throw "Invalid IntMap format";
               }
               return _loc12_;
               break;
            case 114:
               _loc2_ = readDigits();
               if(_loc2_ < 0 || _loc2_ >= int(cache.length))
               {
                  throw "Invalid reference";
               }
               return cache[_loc2_];
               break;
            case 115:
               _loc2_ = readDigits();
               _loc5_ = buf;
               §§push(true);
               pos = (_loc4_ = pos) + 1;
               _loc3_ = _loc4_;
               if(int(buf.charCodeAt(_loc3_)) == 58)
               {
                  §§pop();
                  §§push(length - pos < _loc2_);
               }
               if(§§pop())
               {
                  throw "Invalid bytes length";
               }
               _loc17_ = Unserializer.CODES;
               if(_loc17_ == null)
               {
                  _loc17_ = Unserializer.initCodes();
                  Unserializer.CODES = _loc17_;
               }
               _loc3_ = pos;
               _loc4_ = _loc2_ & 3;
               _loc13_ = (_loc2_ >> 2) * 3 + (_loc4_ >= 2 ? _loc4_ - 1 : 0);
               _loc18_ = _loc3_ + (_loc2_ - _loc4_);
               _loc19_ = Bytes.alloc(_loc13_);
               _loc20_ = 0;
               while(_loc3_ < _loc18_)
               {
                  var _temp_6:* = _loc17_;
                  _loc22_ = _loc3_++;
                  _loc21_ = int(_temp_6[int(_loc5_.charCodeAt(_loc22_))]);
                  var _temp_7:* = _loc17_;
                  _loc23_ = _loc3_++;
                  _loc22_ = int(_temp_7[int(_loc5_.charCodeAt(_loc23_))]);
                  _loc23_ = _loc20_++;
                  _loc19_.b[_loc23_] = _loc21_ << 2 | _loc22_ >> 4;
                  var _temp_8:* = _loc17_;
                  _loc24_ = _loc3_++;
                  _loc23_ = int(_temp_8[int(_loc5_.charCodeAt(_loc24_))]);
                  _loc24_ = _loc20_++;
                  _loc19_.b[_loc24_] = _loc22_ << 4 | _loc23_ >> 2;
                  var _temp_9:* = _loc17_;
                  _loc25_ = _loc3_++;
                  _loc24_ = int(_temp_9[int(_loc5_.charCodeAt(_loc25_))]);
                  _loc25_ = _loc20_++;
                  _loc19_.b[_loc25_] = _loc23_ << 6 | _loc24_;
               }
               if(_loc4_ >= 2)
               {
                  var _temp_10:* = _loc17_;
                  _loc22_ = _loc3_++;
                  _loc21_ = int(_temp_10[int(_loc5_.charCodeAt(_loc22_))]);
                  var _temp_11:* = _loc17_;
                  _loc23_ = _loc3_++;
                  _loc22_ = int(_temp_11[int(_loc5_.charCodeAt(_loc23_))]);
                  _loc23_ = _loc20_++;
                  _loc19_.b[_loc23_] = _loc21_ << 2 | _loc22_ >> 4;
                  if(_loc4_ == 3)
                  {
                     var _temp_12:* = _loc17_;
                     _loc24_ = _loc3_++;
                     _loc23_ = int(_temp_12[int(_loc5_.charCodeAt(_loc24_))]);
                     _loc24_ = _loc20_++;
                     _loc19_.b[_loc24_] = _loc22_ << 4 | _loc23_ >> 2;
                  }
               }
               pos += _loc2_;
               cache.push(_loc19_);
               return _loc19_;
               break;
            case 116:
               return true;
            case 118:
               _loc16_ = Date.fromString(buf.substr(pos,19));
               cache.push(_loc16_);
               pos += 19;
               return _loc16_;
            case 119:
               _loc5_ = unserialize();
               _loc8_ = resolver.resolveEnum(_loc5_);
               if(_loc8_ == null)
               {
                  throw "Enum not found " + _loc5_;
               }
               _loc7_ = unserializeEnum(_loc8_,unserialize());
               cache.push(_loc7_);
               return _loc7_;
               break;
            case 120:
               throw unserialize();
            case 121:
               _loc2_ = readDigits();
               §§push(true);
               pos = (_loc4_ = pos) + 1;
               _loc3_ = _loc4_;
               if(int(buf.charCodeAt(_loc3_)) == 58)
               {
                  §§pop();
                  §§push(length - pos < _loc2_);
               }
               if(§§pop())
               {
                  throw "Invalid string length";
               }
               _loc5_ = buf.substr(pos,_loc2_);
               pos += _loc2_;
               _loc5_ = decodeURIComponent(_loc5_.split("+").join(" "));
               scache.push(_loc5_);
               return _loc5_;
               break;
            case 122:
               return 0;
            default:
               --pos;
               throw "Invalid char " + buf.charAt(pos) + " at position " + pos;
         }
      }
      
      public function setResolver(param1:Object) : void
      {
         if(param1 == null)
         {
            resolver = {
               "resolveClass":function(param1:String):Class
               {
                  return null;
               },
               "resolveEnum":function(param1:String):Class
               {
                  return null;
               }
            };
         }
         else
         {
            resolver = param1;
         }
      }
      
      public function readDigits() : int
      {
         var _loc4_:int = 0;
         var _loc1_:int = 0;
         var _loc2_:Boolean = false;
         var _loc3_:int = pos;
         while(true)
         {
            _loc4_ = int(buf.charCodeAt(pos));
            if(_loc4_ == 0)
            {
               break;
            }
            if(_loc4_ == 45)
            {
               if(pos != _loc3_)
               {
                  break;
               }
               _loc2_ = true;
               ++pos;
            }
            else
            {
               if(_loc4_ < 48 || _loc4_ > 57)
               {
                  break;
               }
               _loc1_ = _loc1_ * 10 + (_loc4_ - 48);
               ++pos;
            }
         }
         if(_loc2_)
         {
            _loc1_ *= -1;
         }
         return _loc1_;
      }
   }
}

import flash.utils.ByteArray;


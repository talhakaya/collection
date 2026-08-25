package haxe
{
   public class Log
   {
      
      public static function trace(param1:*, param2:Object = undefined):void
      {
         Boot.__trace(param1,param2);
      }
      public function Log()
      {
      }
   }
}

import flash.Boot;

var _temp_2:* = global;
var _temp_1:* = §§newclass(Log,Object);


package haxe.io
{
   import flash.Boot;
   import flash.utils.ByteArray;
   
   public class Bytes
   {
      
      public var length:int;
      
      public var b:ByteArray;
      
      public function Bytes(param1:int = 0, param2:ByteArray = undefined)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         length = param1;
         b = param2;
      }
      
      public static function alloc(param1:int) : Bytes
      {
         var _loc2_:ByteArray = new ByteArray();
         _loc2_.length = param1;
         return new Bytes(param1,_loc2_);
      }
   }
}


package haxe.ds
{
   import flash.Boot;
   import flash.utils.Dictionary;
   
   public class StringMap implements IMap
   {
      
      public var h:Dictionary;
      
      public function StringMap()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         h = new Dictionary();
      }
      
      public function set(param1:String, param2:Object) : void
      {
         h["$" + param1] = param2;
      }
      
      public function remove(param1:String) : Boolean
      {
         param1 = "$" + param1;
         if(!h.hasOwnProperty(param1))
         {
            return false;
         }
         delete h[param1];
         return true;
      }
      
      public function keys() : Object
      {
         var _loc2_:* = 0;
         var _loc1_:Array = [];
         var _loc3_:* = h;
         for(_loc2_ in _loc3_)
         {
            _loc1_.push((_loc2_).substr(1));
         }
         return _loc1_.iterator();
      }
      
      public function get(param1:String) : Object
      {
         return h["$" + param1];
      }
   }
}


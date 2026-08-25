package
{
   public class Reflect
   {
      
      public function Reflect()
      {
      }
      
      public static function hasField(param1:*, param2:String) : Boolean
      {
         return param1.hasOwnProperty(param2);
      }
      
      public static function field(param1:*, param2:String) : *
      {
         var _loc4_:* = null;
         try
         {
            return param1[param2];
         }
         catch(_loc_e_:*)
         {
         }
      }
      
      public static function getProperty(param1:*, param2:String) : *
      {
         var _loc4_:* = null;
         var _loc5_:* = null;
         try
         {
            return param1["get_" + param2]();
         }
         catch(_loc_e_:*)
         {
            try
            {
               return param1[param2];
            }
            catch(_loc_e_:*)
            {
            }
         }
      }
      
      public static function setProperty(param1:*, param2:String, param3:*) : void
      {
         var _loc5_:* = null;
         try
         {
            param1["set_" + param2](param3);
         }
         catch(_loc_e_:*)
         {
            return;
         }
      }
      
      public static function fields(param1:*) : Array
      {
         var _loc4_:* = null as String;
         if(param1 == null)
         {
            return [];
         }
         var _loc3_:Array = [];
         for(_loc4_ in param1)
         {
            if(param1.hasOwnProperty(_loc4_))
            {
               _loc3_.push(_loc4_);
            }
         }
         return _loc3_;
      }
      
      public static function isFunction(param1:*) : Boolean
      {
         return typeof param1 == "function";
      }
   }
}


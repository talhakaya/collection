package
{
   import flash.Boot;
   
   public class List
   {
      
      public var q:Array;
      
      public var length:int;
      
      public var h:Array;
      
      public function List()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         length = 0;
      }
      
      public function add(param1:Object) : void
      {
         var _loc2_:Array = [param1];
         if(h == null)
         {
            h = _loc2_;
         }
         else
         {
            q[1] = _loc2_;
         }
         q = _loc2_;
         ++length;
      }
   }
}


package haxe.ds
{
   import flash.Boot;
   import flash.utils.Dictionary;
   
   public class IntMap implements IMap
   {
      
      public var h:Dictionary;
      
      public function IntMap()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         h = new Dictionary();
      }
      
      public function set(param1:int, param2:Object) : void
      {
         h[param1] = param2;
      }
   }
}


package openfl
{
   import flash.Boot;
   import haxe.ds.StringMap;
   
   public class AssetCache
   {
      
      public var sound:IMap;
      
      public var font:IMap;
      
      public var enabled:Boolean;
      
      public var bitmapData:IMap;
      
      public function AssetCache()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         enabled = true;
         bitmapData = new StringMap();
         font = new StringMap();
         sound = new StringMap();
      }
      
      public function clear() : void
      {
         bitmapData = new StringMap();
         font = new StringMap();
         sound = new StringMap();
      }
   }
}


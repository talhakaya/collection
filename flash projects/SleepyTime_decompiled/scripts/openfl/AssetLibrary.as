package openfl
{
   import flash.display.BitmapData;
   import flash.display.MovieClip;
   import flash.media.Sound;
   import flash.text.Font;
   import flash.utils.ByteArray;
   
   public class AssetLibrary
   {
      
      public function AssetLibrary()
      {
      }
      
      public function loadText(param1:String, param2:Function) : void
      {
         var handler:Function = param2;
         var _loc3_:Function = function(param1:ByteArray):void
         {
            if(param1 == null)
            {
               handler(null);
            }
            else
            {
               handler(param1.readUTFBytes(param1.length));
            }
         };
         loadBytes(param1,_loc3_);
      }
      
      public function loadSound(param1:String, param2:Function) : void
      {
         param2(getSound(param1));
      }
      
      public function loadMusic(param1:String, param2:Function) : void
      {
         param2(getMusic(param1));
      }
      
      public function loadMovieClip(param1:String, param2:Function) : void
      {
         param2(getMovieClip(param1));
      }
      
      public function loadFont(param1:String, param2:Function) : void
      {
         param2(getFont(param1));
      }
      
      public function loadBytes(param1:String, param2:Function) : void
      {
         param2(getBytes(param1));
      }
      
      public function loadBitmapData(param1:String, param2:Function) : void
      {
         param2(getBitmapData(param1));
      }
      
      public function load(param1:Function) : void
      {
         param1(this);
      }
      
      public function isLocal(param1:String, param2:AssetType) : Boolean
      {
         return true;
      }
      
      public function getText(param1:String) : String
      {
         var _loc2_:ByteArray = getBytes(param1);
         if(_loc2_ == null)
         {
            return null;
         }
         return _loc2_.readUTFBytes(_loc2_.length);
      }
      
      public function getSound(param1:String) : Sound
      {
         return null;
      }
      
      public function getPath(param1:String) : String
      {
         return null;
      }
      
      public function getMusic(param1:String) : Sound
      {
         return getSound(param1);
      }
      
      public function getMovieClip(param1:String) : MovieClip
      {
         return null;
      }
      
      public function getFont(param1:String) : Font
      {
         return null;
      }
      
      public function getBytes(param1:String) : ByteArray
      {
         return null;
      }
      
      public function getBitmapData(param1:String) : BitmapData
      {
         return null;
      }
      
      public function exists(param1:String, param2:AssetType) : Boolean
      {
         return false;
      }
   }
}


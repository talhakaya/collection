package openfl
{
   import flash.Boot;
   
   public final class AssetType
   {
      
      public static const __isenum:Boolean = true;
      
      public static var __constructs__:*;
      
      public static var TEXT:AssetType;
      
      public static var TEMPLATE:AssetType;
      
      public static var SOUND:AssetType;
      
      public static var MUSIC:AssetType;
      
      public static var MOVIE_CLIP:AssetType;
      
      public static var IMAGE:AssetType;
      
      public static var FONT:AssetType;
      
      public static var BINARY:AssetType;
      
      public var tag:String;
      
      public var index:int;
      
      public var params:Array;
      
      public const __enum__:Boolean = true;
      
      public function AssetType(param1:String, param2:int, param3:*)
      {
         tag = param1;
         index = param2;
         params = param3;
      }
      
      final public function toString() : String
      {
         return Boot.enum_to_string(this);
      }
   }
}

_loc1_.§§slot[1] = new AssetType("BINARY",0,null);
_loc1_.§§slot[2] = new AssetType("FONT",1,null);
_loc1_.§§slot[3] = new AssetType("IMAGE",2,null);
_loc1_.§§slot[4] = new AssetType("MOVIE_CLIP",3,null);
_loc1_.§§slot[5] = new AssetType("MUSIC",4,null);
_loc1_.§§slot[6] = new AssetType("SOUND",5,null);
_loc1_.§§slot[7] = new AssetType("TEMPLATE",6,null);
_loc1_.§§slot[8] = new AssetType("TEXT",7,null);
_loc1_.__constructs__ = ["BINARY","FONT","IMAGE","MOVIE_CLIP","MUSIC","SOUND","TEMPLATE","TEXT"];


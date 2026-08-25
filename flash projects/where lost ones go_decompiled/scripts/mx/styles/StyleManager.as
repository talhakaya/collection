package mx.styles
{
   import flash.events.IEventDispatcher;
   import flash.system.ApplicationDomain;
   import flash.system.SecurityDomain;
   import flash.utils.Dictionary;
   import mx.core.IFlexModuleFactory;
   import mx.core.Singleton;
   import mx.core.mx_internal;
   import mx.managers.SystemManagerGlobals;
   
   use namespace mx_internal;
   
   public class StyleManager
   {
      
      private static var implClassDependency:StyleManagerImpl;
      
      private static var _impl:IStyleManager2;
      
      private static var styleManagerDictionary:Dictionary;
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      public static const NOT_A_COLOR:uint = 4294967295;
      
      public function StyleManager()
      {
         super();
      }
      
      private static function get impl() : IStyleManager2
      {
         if(!_impl)
         {
            _impl = IStyleManager2(Singleton.getInstance("mx.styles::IStyleManager2"));
         }
         return _impl;
      }
      
      public static function getStyleManager(moduleFactory:IFlexModuleFactory) : IStyleManager2
      {
         var styleManager:IStyleManager2 = null;
         var o:Object = null;
         if(!moduleFactory)
         {
            moduleFactory = SystemManagerGlobals.topLevelSystemManagers[0];
         }
         if(!styleManagerDictionary)
         {
            styleManagerDictionary = new Dictionary(true);
         }
         var dictionary:Dictionary = styleManagerDictionary[moduleFactory];
         if(dictionary == null)
         {
            styleManager = IStyleManager2(moduleFactory.getImplementation("mx.styles::IStyleManager2"));
            if(styleManager == null)
            {
               styleManager = impl;
            }
            dictionary = new Dictionary(true);
            styleManagerDictionary[moduleFactory] = dictionary;
            dictionary[styleManager] = 1;
         }
         else
         {
            var _loc5_:int = 0;
            var _loc6_:* = dictionary;
            for(o in _loc6_)
            {
               styleManager = o as IStyleManager2;
            }
         }
         return styleManager;
      }
      
      [Deprecated(replacement="IStyleManager2.stylesRoot on a style manager instance",since="4.0")]
      mx_internal static function get stylesRoot() : Object
      {
         return impl.stylesRoot;
      }
      
      mx_internal static function set stylesRoot(value:Object) : void
      {
         impl.stylesRoot = value;
      }
      
      [Deprecated(replacement="IStyleManager2.inheritingStyles on a style manager instance",since="4.0")]
      mx_internal static function get inheritingStyles() : Object
      {
         return impl.inheritingStyles;
      }
      
      mx_internal static function set inheritingStyles(value:Object) : void
      {
         impl.inheritingStyles = value;
      }
      
      [Deprecated(replacement="IStyleManager2.typeHierarchyCache on a style manager instance",since="4.0")]
      mx_internal static function get typeHierarchyCache() : Object
      {
         return impl.typeHierarchyCache;
      }
      
      mx_internal static function set typeHierarchyCache(value:Object) : void
      {
         impl.typeHierarchyCache = value;
      }
      
      [Deprecated(replacement="IStyleManager2.typeSelectorCache on a style manager instance",since="4.0")]
      mx_internal static function get typeSelectorCache() : Object
      {
         return impl.typeSelectorCache;
      }
      
      mx_internal static function set typeSelectorCache(value:Object) : void
      {
         impl.typeSelectorCache = value;
      }
      
      [Deprecated(replacement="IStyleManager2.initProtoChainRoots on a style manager instance",since="4.0")]
      mx_internal static function initProtoChainRoots() : void
      {
         impl.initProtoChainRoots();
      }
      
      [Deprecated(replacement="IStyleManager2.selectors on a style manager instance",since="4.0")]
      public static function get selectors() : Array
      {
         return impl.selectors;
      }
      
      [Deprecated(replacement="IStyleManager2.getStyleDeclaration on a style manager instance",since="4.0")]
      public static function getStyleDeclaration(selector:String) : CSSStyleDeclaration
      {
         return impl.getStyleDeclaration(selector);
      }
      
      [Deprecated(replacement="IStyleManager2.setStyleDeclaration on a style manager instance",since="4.0")]
      public static function setStyleDeclaration(selector:String, styleDeclaration:CSSStyleDeclaration, update:Boolean) : void
      {
         impl.setStyleDeclaration(selector,styleDeclaration,update);
      }
      
      [Deprecated(replacement="IStyleManager2.clearStyleDeclaration on a style manager instance",since="4.0")]
      public static function clearStyleDeclaration(selector:String, update:Boolean) : void
      {
         impl.clearStyleDeclaration(selector,update);
      }
      
      [Deprecated(replacement="IStyleManager2.styleDeclarationsChanged on a style manager instance",since="4.0")]
      mx_internal static function styleDeclarationsChanged() : void
      {
         impl.styleDeclarationsChanged();
      }
      
      [Deprecated(replacement="IStyleManager2.registerInheritingStyle on a style manager instance",since="4.0")]
      public static function registerInheritingStyle(styleName:String) : void
      {
         impl.registerInheritingStyle(styleName);
      }
      
      [Deprecated(replacement="IStyleManager2.isInheritingStyle on a style manager instance",since="4.0")]
      public static function isInheritingStyle(styleName:String) : Boolean
      {
         return impl.isInheritingStyle(styleName);
      }
      
      [Deprecated(replacement="IStyleManager2.isInheritingTextFormatStyle on a style manager instance",since="4.0")]
      public static function isInheritingTextFormatStyle(styleName:String) : Boolean
      {
         return impl.isInheritingTextFormatStyle(styleName);
      }
      
      [Deprecated(replacement="IStyleManager2.registerSizeInvalidatingStyle on a style manager instance",since="4.0")]
      public static function registerSizeInvalidatingStyle(styleName:String) : void
      {
         impl.registerSizeInvalidatingStyle(styleName);
      }
      
      [Deprecated(replacement="IStyleManager2.isSizeInvalidatingStyle on a style manager instance",since="4.0")]
      public static function isSizeInvalidatingStyle(styleName:String) : Boolean
      {
         return impl.isSizeInvalidatingStyle(styleName);
      }
      
      [Deprecated(replacement="IStyleManager2.registerParentSizeInvalidatingStyle on a style manager instance",since="4.0")]
      public static function registerParentSizeInvalidatingStyle(styleName:String) : void
      {
         impl.registerParentSizeInvalidatingStyle(styleName);
      }
      
      [Deprecated(replacement="IStyleManager2.isParentSizeInvalidatingStyle on a style manager instance",since="4.0")]
      public static function isParentSizeInvalidatingStyle(styleName:String) : Boolean
      {
         return impl.isParentSizeInvalidatingStyle(styleName);
      }
      
      [Deprecated(replacement="IStyleManager2.registerParentDisplayListInvalidatingStyle on a style manager instance",since="4.0")]
      public static function registerParentDisplayListInvalidatingStyle(styleName:String) : void
      {
         impl.registerParentDisplayListInvalidatingStyle(styleName);
      }
      
      [Deprecated(replacement="IStyleManager2.isParentDisplayListInvalidatingStyle on a style manager instance",since="4.0")]
      public static function isParentDisplayListInvalidatingStyle(styleName:String) : Boolean
      {
         return impl.isParentDisplayListInvalidatingStyle(styleName);
      }
      
      [Deprecated(replacement="IStyleManager2.registerColorName on a style manager instance",since="4.0")]
      public static function registerColorName(colorName:String, colorValue:uint) : void
      {
         impl.registerColorName(colorName,colorValue);
      }
      
      [Deprecated(replacement="IStyleManager2.isColorName on a style manager instance",since="4.0")]
      public static function isColorName(colorName:String) : Boolean
      {
         return impl.isColorName(colorName);
      }
      
      [Deprecated(replacement="IStyleManager2.getColorName on a style manager instance",since="4.0")]
      public static function getColorName(colorName:Object) : uint
      {
         return impl.getColorName(colorName);
      }
      
      [Deprecated(replacement="IStyleManager2.getColorNames on a style manager instance",since="4.0")]
      public static function getColorNames(colors:Array) : void
      {
         impl.getColorNames(colors);
      }
      
      [Deprecated(replacement="IStyleManager2.isValidStyleValue on a style manager instance",since="4.0")]
      public static function isValidStyleValue(value:*) : Boolean
      {
         return impl.isValidStyleValue(value);
      }
      
      [Deprecated(replacement="IStyleManager2.loadStyleDeclarations on a style manager instance",since="4.0")]
      public static function loadStyleDeclarations(url:String, update:Boolean = true, trustContent:Boolean = false, applicationDomain:ApplicationDomain = null, securityDomain:SecurityDomain = null) : IEventDispatcher
      {
         return impl.loadStyleDeclarations2(url,update,applicationDomain,securityDomain);
      }
      
      [Deprecated(replacement="IStyleManager2.unloadStyleDeclarations on a style manager instance",since="4.0")]
      public static function unloadStyleDeclarations(url:String, update:Boolean = true) : void
      {
         impl.unloadStyleDeclarations(url,update);
      }
   }
}


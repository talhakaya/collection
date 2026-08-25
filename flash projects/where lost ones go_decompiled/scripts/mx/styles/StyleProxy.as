package mx.styles
{
   import mx.core.mx_internal;
   
   use namespace mx_internal;
   
   public class StyleProxy implements IAdvancedStyleClient
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private var _filterMap:Object;
      
      private var _source:IStyleClient;
      
      private var _advancedSource:IAdvancedStyleClient;
      
      public function StyleProxy(source:IStyleClient, filterMap:Object)
      {
         super();
         this.filterMap = filterMap;
         this.source = source;
      }
      
      public function get filterMap() : Object
      {
         return this._filterMap;
      }
      
      public function set filterMap(value:Object) : void
      {
         this._filterMap = value;
      }
      
      public function get source() : IStyleClient
      {
         return this._source;
      }
      
      public function set source(value:IStyleClient) : void
      {
         this._source = value;
         this._advancedSource = value as IAdvancedStyleClient;
      }
      
      public function get className() : String
      {
         return this._source.className;
      }
      
      public function get inheritingStyles() : Object
      {
         return this._source.inheritingStyles;
      }
      
      public function set inheritingStyles(value:Object) : void
      {
      }
      
      public function get nonInheritingStyles() : Object
      {
         return null;
      }
      
      public function set nonInheritingStyles(value:Object) : void
      {
      }
      
      public function get styleDeclaration() : CSSStyleDeclaration
      {
         return this._source.styleDeclaration;
      }
      
      public function set styleDeclaration(value:CSSStyleDeclaration) : void
      {
         this._source.styleDeclaration = this.styleDeclaration;
      }
      
      public function get styleName() : Object
      {
         if(this._source.styleName is IStyleClient)
         {
            return new StyleProxy(IStyleClient(this._source.styleName),this.filterMap);
         }
         return this._source.styleName;
      }
      
      public function set styleName(value:Object) : void
      {
         this._source.styleName = value;
      }
      
      public function get id() : String
      {
         return Boolean(this._advancedSource) ? this._advancedSource.id : null;
      }
      
      public function get styleParent() : IAdvancedStyleClient
      {
         return Boolean(this._advancedSource) ? this._advancedSource.styleParent : null;
      }
      
      public function set styleParent(parent:IAdvancedStyleClient) : void
      {
      }
      
      public function styleChanged(styleProp:String) : void
      {
         return this._source.styleChanged(styleProp);
      }
      
      public function getStyle(styleProp:String) : *
      {
         return this._source.getStyle(styleProp);
      }
      
      public function setStyle(styleProp:String, newValue:*) : void
      {
         this._source.setStyle(styleProp,newValue);
      }
      
      public function clearStyle(styleProp:String) : void
      {
         this._source.clearStyle(styleProp);
      }
      
      public function getClassStyleDeclarations() : Array
      {
         return this._source.getClassStyleDeclarations();
      }
      
      public function notifyStyleChangeInChildren(styleProp:String, recursive:Boolean) : void
      {
         return this._source.notifyStyleChangeInChildren(styleProp,recursive);
      }
      
      public function regenerateStyleCache(recursive:Boolean) : void
      {
         this._source.regenerateStyleCache(recursive);
      }
      
      public function registerEffects(effects:Array) : void
      {
         return this._source.registerEffects(effects);
      }
      
      public function stylesInitialized() : void
      {
         if(Boolean(this._advancedSource))
         {
            this._advancedSource.stylesInitialized();
         }
      }
      
      public function matchesCSSState(cssState:String) : Boolean
      {
         return Boolean(this._advancedSource) ? this._advancedSource.matchesCSSState(cssState) : false;
      }
      
      public function matchesCSSType(cssType:String) : Boolean
      {
         return Boolean(this._advancedSource) ? this._advancedSource.matchesCSSType(cssType) : false;
      }
      
      public function hasCSSState() : Boolean
      {
         return Boolean(this._advancedSource) ? this._advancedSource.hasCSSState() : false;
      }
   }
}


package mx.core
{
   use namespace mx_internal;
   
   [ExcludeClass]
   public class EmbeddedFont
   {
      
      private static var noEmbeddedFonts:Boolean;
      
      private static var _embeddedFontRegistry:IEmbeddedFontRegistry;
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private var _bold:Boolean;
      
      private var _fontName:String;
      
      private var _fontStyle:String;
      
      private var _italic:Boolean;
      
      public function EmbeddedFont(fontName:String, bold:Boolean, italic:Boolean)
      {
         super();
         this.initialize(fontName,bold,italic);
      }
      
      private static function get embeddedFontRegistry() : IEmbeddedFontRegistry
      {
         if(!_embeddedFontRegistry && !noEmbeddedFonts)
         {
            try
            {
               _embeddedFontRegistry = IEmbeddedFontRegistry(Singleton.getInstance("mx.core::IEmbeddedFontRegistry"));
            }
            catch(e:Error)
            {
               noEmbeddedFonts = true;
            }
         }
         return _embeddedFontRegistry;
      }
      
      public function get bold() : Boolean
      {
         return this._bold;
      }
      
      public function get fontName() : String
      {
         return this._fontName;
      }
      
      public function get fontStyle() : String
      {
         return this._fontStyle;
      }
      
      public function get italic() : Boolean
      {
         return this._italic;
      }
      
      public function initialize(fontName:String, bold:Boolean, italic:Boolean) : void
      {
         this._bold = bold;
         this._italic = italic;
         this._fontName = fontName;
         this._fontStyle = embeddedFontRegistry.getFontStyle(bold,italic);
      }
   }
}


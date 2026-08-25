package mx.core
{
   import flash.text.TextFormat;
   import flash.text.TextLineMetrics;
   import mx.managers.ISystemManager;
   
   use namespace mx_internal;
   
   public class UITextFormat extends TextFormat
   {
      
      private static var noEmbeddedFonts:Boolean;
      
      private static var _embeddedFontRegistry:IEmbeddedFontRegistry;
      
      private static var _textFieldFactory:ITextFieldFactory;
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private var systemManager:ISystemManager;
      
      public var antiAliasType:String;
      
      public var direction:String;
      
      public var gridFitType:String;
      
      public var locale:String;
      
      private var _moduleFactory:IFlexModuleFactory;
      
      public var sharpness:Number;
      
      public var thickness:Number;
      
      public var useFTE:Boolean = false;
      
      public function UITextFormat(systemManager:ISystemManager, font:String = null, size:Object = null, color:Object = null, bold:Object = null, italic:Object = null, underline:Object = null, url:String = null, target:String = null, align:String = null, leftMargin:Object = null, rightMargin:Object = null, indent:Object = null, leading:Object = null)
      {
         this.systemManager = systemManager;
         super(font,size,color,bold,italic,underline,url,target,align,leftMargin,rightMargin,indent,leading);
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
      
      private static function get textFieldFactory() : ITextFieldFactory
      {
         if(!_textFieldFactory)
         {
            _textFieldFactory = ITextFieldFactory(Singleton.getInstance("mx.core::ITextFieldFactory"));
         }
         return _textFieldFactory;
      }
      
      public function get moduleFactory() : IFlexModuleFactory
      {
         return this._moduleFactory;
      }
      
      public function set moduleFactory(value:IFlexModuleFactory) : void
      {
         this._moduleFactory = value;
      }
      
      public function measureText(text:String, roundUp:Boolean = true) : TextLineMetrics
      {
         return this.measure(text,false,roundUp);
      }
      
      public function measureHTMLText(htmlText:String, roundUp:Boolean = true) : TextLineMetrics
      {
         return this.measure(htmlText,true,roundUp);
      }
      
      private function measure(s:String, html:Boolean, roundUp:Boolean) : TextLineMetrics
      {
         if(!s)
         {
            s = "";
         }
         var embeddedFont:Boolean = false;
         var fontModuleFactory:IFlexModuleFactory = noEmbeddedFonts || !embeddedFontRegistry ? null : embeddedFontRegistry.getAssociatedModuleFactory(font,bold,italic,this,this.moduleFactory,this.systemManager,this.useFTE);
         embeddedFont = fontModuleFactory != null;
         if(fontModuleFactory == null)
         {
            fontModuleFactory = this.systemManager;
         }
         var measurementTextField:Object = this.useFTE ? textFieldFactory.createFTETextField(fontModuleFactory) : textFieldFactory.createTextField(fontModuleFactory);
         if(html)
         {
            measurementTextField.htmlText = "";
         }
         else
         {
            measurementTextField.text = "";
         }
         measurementTextField.defaultTextFormat = this;
         measurementTextField.embedFonts = embeddedFont;
         if(!this.useFTE)
         {
            measurementTextField.antiAliasType = this.antiAliasType;
            measurementTextField.gridFitType = this.gridFitType;
            measurementTextField.sharpness = this.sharpness;
            measurementTextField.thickness = this.thickness;
         }
         else
         {
            measurementTextField.direction = this.direction;
            measurementTextField.locale = this.locale;
         }
         if(html)
         {
            measurementTextField.htmlText = s;
         }
         else
         {
            measurementTextField.text = s;
         }
         var lineMetrics:TextLineMetrics = measurementTextField.getLineMetrics(0);
         if(indent != null)
         {
            lineMetrics.width += indent;
         }
         if(roundUp)
         {
            lineMetrics.width = Math.ceil(lineMetrics.width);
            lineMetrics.height = Math.ceil(lineMetrics.height);
         }
         return lineMetrics;
      }
      
      mx_internal function copyFrom(source:TextFormat) : void
      {
         font = source.font;
         size = source.size;
         color = source.color;
         bold = source.bold;
         italic = source.italic;
         underline = source.underline;
         url = source.url;
         target = source.target;
         align = source.align;
         leftMargin = source.leftMargin;
         rightMargin = source.rightMargin;
         indent = source.indent;
         leading = source.leading;
         letterSpacing = source.letterSpacing;
         blockIndent = source.blockIndent;
         bullet = source.bullet;
         display = source.display;
         indent = source.indent;
         kerning = source.kerning;
         tabStops = source.tabStops;
      }
   }
}


package mx.controls
{
   import flash.display.DisplayObject;
   import flash.text.TextFieldAutoSize;
   import flash.text.TextFormat;
   import mx.core.EdgeMetrics;
   import mx.core.IFlexDisplayObject;
   import mx.core.IFlexModuleFactory;
   import mx.core.IFontContextComponent;
   import mx.core.IRectangularBorder;
   import mx.core.IToolTip;
   import mx.core.IUITextField;
   import mx.core.UIComponent;
   import mx.core.UITextField;
   import mx.core.mx_internal;
   import mx.styles.ISimpleStyleClient;
   
   use namespace mx_internal;
   
   [DiscouragedForProfile("mobileDevice")]
   [Style(name="paddingTop",type="Number",format="Length",inherit="no")]
   [Style(name="paddingBottom",type="Number",format="Length",inherit="no")]
   [Style(name="cornerRadius",type="Number",format="Length",inherit="no",theme="halo, spark, mobile")]
   [Style(name="textIndent",type="Number",format="Length",inherit="yes")]
   [Style(name="textFieldClass",type="Class",inherit="no")]
   [Style(name="textDecoration",type="String",enumeration="none,underline",inherit="yes")]
   [Style(name="textAlign",type="String",enumeration="left,center,right",inherit="yes")]
   [Style(name="locale",type="String",inherit="yes")]
   [Style(name="letterSpacing",type="Number",inherit="yes")]
   [Style(name="kerning",type="Boolean",inherit="yes")]
   [Style(name="fontWeight",type="String",enumeration="normal,bold",inherit="yes")]
   [Style(name="fontThickness",type="Number",inherit="yes")]
   [Style(name="fontStyle",type="String",enumeration="normal,italic",inherit="yes")]
   [Style(name="fontSize",type="Number",format="Length",inherit="yes")]
   [Style(name="fontSharpness",type="Number",inherit="yes")]
   [Style(name="fontGridFitType",type="String",enumeration="none,pixel,subpixel",inherit="yes")]
   [Style(name="fontFamily",type="String",inherit="yes")]
   [Style(name="fontAntiAliasType",type="String",enumeration="normal,advanced",inherit="yes")]
   [Style(name="disabledColor",type="uint",format="Color",inherit="yes")]
   [Style(name="direction",type="String",enumeration="ltr,rtl,inherit",inherit="yes")]
   [Style(name="color",type="uint",format="Color",inherit="yes")]
   [Style(name="paddingRight",type="Number",format="Length",inherit="no")]
   [Style(name="paddingLeft",type="Number",format="Length",inherit="no")]
   [Style(name="leading",type="Number",format="Length",inherit="yes")]
   [Style(name="shadowDistance",type="Number",format="Length",inherit="no",theme="halo")]
   [Style(name="shadowDirection",type="String",enumeration="left,center,right",inherit="no",theme="halo")]
   [Style(name="dropShadowColor",type="uint",format="Color",inherit="yes",theme="halo")]
   [Style(name="dropShadowVisible",type="Boolean",inherit="no",theme="spark")]
   [Style(name="dropShadowEnabled",type="Boolean",inherit="no",theme="halo")]
   [Style(name="borderVisible",type="Boolean",inherit="no",theme="spark")]
   [Style(name="borderThickness",type="Number",format="Length",inherit="no",theme="halo")]
   [Style(name="borderStyle",type="String",enumeration="inset,outset,solid,none",inherit="no")]
   [Style(name="borderSkin",type="Class",inherit="no")]
   [Style(name="borderSides",type="String",inherit="no",theme="halo")]
   [Style(name="borderColor",type="uint",format="Color",inherit="no",theme="halo, spark, mobile")]
   [Style(name="borderAlpha",type="Number",inherit="no",theme="spark")]
   [Style(name="backgroundSize",type="String",inherit="no",theme="halo")]
   [Style(name="backgroundImage",type="Object",format="File",inherit="no",theme="halo")]
   [Style(name="backgroundDisabledColor",type="uint",format="Color",inherit="yes",theme="halo")]
   [Style(name="backgroundColor",type="uint",format="Color",inherit="no",theme="halo, spark, mobile")]
   [Style(name="backgroundAlpha",type="Number",inherit="no",theme="halo, spark, mobile")]
   public class ToolTip extends UIComponent implements IToolTip, IFontContextComponent
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      [Inspectable(category="Other")]
      public static var maxWidth:Number = 300;
      
      mx_internal var border:IFlexDisplayObject;
      
      private var _text:String;
      
      private var textChanged:Boolean;
      
      protected var textField:IUITextField;
      
      public function ToolTip()
      {
         super();
         mouseEnabled = false;
      }
      
      private function get borderMetrics() : EdgeMetrics
      {
         if(this.border is IRectangularBorder)
         {
            return IRectangularBorder(this.border).borderMetrics;
         }
         return EdgeMetrics.EMPTY;
      }
      
      public function get fontContext() : IFlexModuleFactory
      {
         return moduleFactory;
      }
      
      public function set fontContext(moduleFactory:IFlexModuleFactory) : void
      {
         this.moduleFactory = moduleFactory;
      }
      
      public function get text() : String
      {
         return this._text;
      }
      
      public function set text(value:String) : void
      {
         this._text = value;
         this.textChanged = true;
         invalidateProperties();
         invalidateSize();
         invalidateDisplayList();
      }
      
      override protected function createChildren() : void
      {
         super.createChildren();
         this.createBorder();
         this.createTextField(-1);
      }
      
      override protected function commitProperties() : void
      {
         var index:int = 0;
         var textFormat:TextFormat = null;
         super.commitProperties();
         if(hasFontContextChanged() && this.textField != null)
         {
            index = getChildIndex(DisplayObject(this.textField));
            this.removeTextField();
            this.createTextField(index);
            invalidateSize();
            this.textChanged = true;
         }
         if(this.textChanged)
         {
            textFormat = this.textField.getTextFormat();
            textFormat.leftMargin = 0;
            textFormat.rightMargin = 0;
            this.textField.defaultTextFormat = textFormat;
            this.textField.text = this._text;
            this.textChanged = false;
         }
      }
      
      override protected function measure() : void
      {
         super.measure();
         var bm:EdgeMetrics = this.borderMetrics;
         var leftInset:Number = bm.left + getStyle("paddingLeft");
         var topInset:Number = bm.top + getStyle("paddingTop");
         var rightInset:Number = bm.right + getStyle("paddingRight");
         var bottomInset:Number = bm.bottom + getStyle("paddingBottom");
         var widthSlop:Number = leftInset + rightInset;
         var heightSlop:Number = topInset + bottomInset;
         this.textField.wordWrap = false;
         if(this.textField.textWidth + widthSlop > ToolTip.maxWidth)
         {
            this.textField.width = ToolTip.maxWidth - widthSlop;
            this.textField.wordWrap = true;
         }
         measuredWidth = this.textField.width + widthSlop;
         measuredHeight = this.textField.height + heightSlop;
      }
      
      override protected function updateDisplayList(unscaledWidth:Number, unscaledHeight:Number) : void
      {
         super.updateDisplayList(unscaledWidth,unscaledHeight);
         var bm:EdgeMetrics = this.borderMetrics;
         var leftInset:Number = bm.left + getStyle("paddingLeft");
         var topInset:Number = bm.top + getStyle("paddingTop");
         var rightInset:Number = bm.right + getStyle("paddingRight");
         var bottomInset:Number = bm.bottom + getStyle("paddingBottom");
         var widthSlop:Number = leftInset + rightInset;
         var heightSlop:Number = topInset + bottomInset;
         this.border.setActualSize(unscaledWidth,unscaledHeight);
         this.textField.move(leftInset,topInset);
         this.textField.setActualSize(unscaledWidth - widthSlop,unscaledHeight - heightSlop);
      }
      
      override public function styleChanged(styleProp:String) : void
      {
         super.styleChanged(styleProp);
         if(styleProp == "styleName" || styleProp == "borderSkin" || styleProp == null)
         {
            if(Boolean(this.border))
            {
               removeChild(DisplayObject(this.border));
               this.border = null;
            }
            this.createBorder();
         }
         else if(styleProp == "borderStyle")
         {
            invalidateDisplayList();
         }
      }
      
      mx_internal function createTextField(childIndex:int) : void
      {
         if(!this.textField)
         {
            this.textField = IUITextField(createInFontContext(UITextField));
            this.textField.autoSize = TextFieldAutoSize.LEFT;
            this.textField.mouseEnabled = false;
            this.textField.multiline = true;
            this.textField.selectable = false;
            this.textField.wordWrap = false;
            this.textField.styleName = this;
            if(childIndex == -1)
            {
               addChild(DisplayObject(this.textField));
            }
            else
            {
               addChildAt(DisplayObject(this.textField),childIndex);
            }
         }
      }
      
      mx_internal function removeTextField() : void
      {
         if(Boolean(this.textField))
         {
            removeChild(DisplayObject(this.textField));
            this.textField = null;
         }
      }
      
      mx_internal function getTextField() : IUITextField
      {
         return this.textField;
      }
      
      private function createBorder() : void
      {
         var borderClass:Class = null;
         if(!this.border)
         {
            borderClass = getStyle("borderSkin");
            if(borderClass != null)
            {
               this.border = new borderClass();
               if(this.border is ISimpleStyleClient)
               {
                  ISimpleStyleClient(this.border).styleName = this;
               }
               addChildAt(DisplayObject(this.border),0);
               invalidateDisplayList();
            }
         }
      }
   }
}


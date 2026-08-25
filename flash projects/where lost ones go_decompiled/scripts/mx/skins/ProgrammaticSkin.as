package mx.skins
{
   import flash.display.Graphics;
   import flash.geom.Matrix;
   import flash.utils.getDefinitionByName;
   import mx.core.FlexShape;
   import mx.core.IFlexDisplayObject;
   import mx.core.IInvalidating;
   import mx.core.IProgrammaticSkin;
   import mx.core.UIComponentGlobals;
   import mx.core.mx_internal;
   import mx.managers.ILayoutManagerClient;
   import mx.styles.ISimpleStyleClient;
   import mx.styles.IStyleClient;
   import mx.styles.IStyleManager2;
   import mx.styles.StyleManager;
   import mx.utils.GraphicsUtil;
   
   use namespace mx_internal;
   
   public class ProgrammaticSkin extends FlexShape implements IFlexDisplayObject, IInvalidating, ILayoutManagerClient, ISimpleStyleClient, IProgrammaticSkin
   {
      
      private static var uiComponentClass:Class;
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private static var tempMatrix:Matrix = new Matrix();
      
      private var invalidateDisplayListFlag:Boolean = false;
      
      private var _height:Number;
      
      private var _width:Number;
      
      private var _initialized:Boolean = false;
      
      private var _nestLevel:int = 0;
      
      private var _processedDescriptors:Boolean = false;
      
      private var _updateCompletePendingFlag:Boolean = true;
      
      private var _styleName:IStyleClient;
      
      public function ProgrammaticSkin()
      {
         super();
         this._width = this.measuredWidth;
         this._height = this.measuredHeight;
      }
      
      override public function get height() : Number
      {
         return this._height;
      }
      
      override public function set height(value:Number) : void
      {
         this._height = value;
         this.invalidateDisplayList();
      }
      
      override public function get width() : Number
      {
         return this._width;
      }
      
      override public function set width(value:Number) : void
      {
         this._width = value;
         this.invalidateDisplayList();
      }
      
      public function get measuredHeight() : Number
      {
         return 0;
      }
      
      public function get measuredWidth() : Number
      {
         return 0;
      }
      
      public function get initialized() : Boolean
      {
         return this._initialized;
      }
      
      public function set initialized(value:Boolean) : void
      {
         this._initialized = value;
      }
      
      public function get nestLevel() : int
      {
         return this._nestLevel;
      }
      
      public function set nestLevel(value:int) : void
      {
         this._nestLevel = value;
         this.invalidateDisplayList();
      }
      
      public function get processedDescriptors() : Boolean
      {
         return this._processedDescriptors;
      }
      
      public function set processedDescriptors(value:Boolean) : void
      {
         this._processedDescriptors = value;
      }
      
      public function get updateCompletePendingFlag() : Boolean
      {
         return this._updateCompletePendingFlag;
      }
      
      public function set updateCompletePendingFlag(value:Boolean) : void
      {
         this._updateCompletePendingFlag = value;
      }
      
      public function get styleName() : Object
      {
         return this._styleName;
      }
      
      public function set styleName(value:Object) : void
      {
         if(this._styleName != value)
         {
            this._styleName = value as IStyleClient;
            this.invalidateDisplayList();
         }
      }
      
      public function move(x:Number, y:Number) : void
      {
         this.x = x;
         this.y = y;
      }
      
      public function setActualSize(newWidth:Number, newHeight:Number) : void
      {
         var changed:Boolean = false;
         if(this._width != newWidth)
         {
            this._width = newWidth;
            changed = true;
         }
         if(this._height != newHeight)
         {
            this._height = newHeight;
            changed = true;
         }
         if(changed)
         {
            this.invalidateDisplayList();
         }
      }
      
      public function validateProperties() : void
      {
      }
      
      public function validateSize(recursive:Boolean = false) : void
      {
      }
      
      public function validateDisplayList() : void
      {
         this.invalidateDisplayListFlag = false;
         this.updateDisplayList(this.width,this.height);
      }
      
      public function styleChanged(styleProp:String) : void
      {
         this.invalidateDisplayList();
      }
      
      public function invalidateDisplayList() : void
      {
         if(!this.invalidateDisplayListFlag && this.nestLevel > 0)
         {
            this.invalidateDisplayListFlag = true;
            UIComponentGlobals.layoutManager.invalidateDisplayList(this);
         }
      }
      
      protected function updateDisplayList(unscaledWidth:Number, unscaledHeight:Number) : void
      {
      }
      
      public function invalidateSize() : void
      {
      }
      
      public function invalidateProperties() : void
      {
      }
      
      public function validateNow() : void
      {
         if(this.invalidateDisplayListFlag)
         {
            this.validateDisplayList();
         }
      }
      
      public function getStyle(styleProp:String) : *
      {
         return Boolean(this._styleName) ? this._styleName.getStyle(styleProp) : null;
      }
      
      protected function get styleManager() : IStyleManager2
      {
         if(uiComponentClass == null)
         {
            uiComponentClass = Class(getDefinitionByName("mx.core.UIComponent"));
         }
         if(this.styleName is uiComponentClass)
         {
            return this.styleName.styleManager;
         }
         return StyleManager.getStyleManager(null);
      }
      
      protected function horizontalGradientMatrix(x:Number, y:Number, width:Number, height:Number) : Matrix
      {
         return this.rotatedGradientMatrix(x,y,width,height,0);
      }
      
      protected function verticalGradientMatrix(x:Number, y:Number, width:Number, height:Number) : Matrix
      {
         return this.rotatedGradientMatrix(x,y,width,height,90);
      }
      
      protected function rotatedGradientMatrix(x:Number, y:Number, width:Number, height:Number, rotation:Number) : Matrix
      {
         tempMatrix.createGradientBox(width,height,rotation * Math.PI / 180,x,y);
         return tempMatrix;
      }
      
      protected function drawRoundRect(x:Number, y:Number, width:Number, height:Number, cornerRadius:Object = null, color:Object = null, alpha:Object = null, gradientMatrix:Matrix = null, gradientType:String = "linear", gradientRatios:Array = null, hole:Object = null) : void
      {
         var ellipseSize:Number = NaN;
         var alphas:Array = null;
         var holeR:Object = null;
         var g:Graphics = graphics;
         if(width == 0 || height == 0)
         {
            return;
         }
         if(color !== null)
         {
            if(color is uint)
            {
               g.beginFill(uint(color),Number(alpha));
            }
            else if(color is Array)
            {
               alphas = alpha is Array ? alpha as Array : [alpha,alpha];
               if(!gradientRatios)
               {
                  gradientRatios = [0,255];
               }
               g.beginGradientFill(gradientType,color as Array,alphas,gradientRatios,gradientMatrix);
            }
         }
         if(!cornerRadius)
         {
            g.drawRect(x,y,width,height);
         }
         else if(cornerRadius is Number)
         {
            ellipseSize = Number(cornerRadius) * 2;
            g.drawRoundRect(x,y,width,height,ellipseSize,ellipseSize);
         }
         else
         {
            GraphicsUtil.drawRoundRectComplex(g,x,y,width,height,cornerRadius.tl,cornerRadius.tr,cornerRadius.bl,cornerRadius.br);
         }
         if(Boolean(hole))
         {
            holeR = hole.r;
            if(holeR is Number)
            {
               ellipseSize = Number(holeR) * 2;
               g.drawRoundRect(hole.x,hole.y,hole.w,hole.h,ellipseSize,ellipseSize);
            }
            else
            {
               GraphicsUtil.drawRoundRectComplex(g,hole.x,hole.y,hole.w,hole.h,holeR.tl,holeR.tr,holeR.bl,holeR.br);
            }
         }
         if(color !== null)
         {
            g.endFill();
         }
      }
   }
}


package mx.core
{
   import flash.geom.Matrix;
   import flash.geom.Point;
   import mx.utils.MatrixUtil;
   
   use namespace mx_internal;
   
   [ExcludeClass]
   public class LayoutElementUIComponentUtils
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private static const DEFAULT_MAX_WIDTH:Number = 10000;
      
      private static const DEFAULT_MAX_HEIGHT:Number = 10000;
      
      public function LayoutElementUIComponentUtils()
      {
         super();
      }
      
      private static function getPreferredUBoundsWidth(obj:IUIComponent) : Number
      {
         var result:Number = obj.getExplicitOrMeasuredWidth();
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            result = obj.scaleX == 0 ? 0 : result / obj.scaleX;
         }
         return result;
      }
      
      private static function getPreferredUBoundsHeight(obj:IUIComponent) : Number
      {
         var result:Number = obj.getExplicitOrMeasuredHeight();
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            result = obj.scaleY == 0 ? 0 : result / obj.scaleY;
         }
         return result;
      }
      
      private static function getMinUBoundsWidth(obj:IUIComponent) : Number
      {
         var minWidth:Number = NaN;
         if(!isNaN(obj.explicitMinWidth))
         {
            minWidth = obj.explicitMinWidth;
         }
         else
         {
            minWidth = isNaN(obj.measuredMinWidth) ? 0 : obj.measuredMinWidth;
            if(!isNaN(obj.explicitMaxWidth))
            {
               minWidth = Math.min(minWidth,obj.explicitMaxWidth);
            }
         }
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            minWidth = obj.scaleX == 0 ? 0 : minWidth / obj.scaleX;
         }
         return minWidth;
      }
      
      private static function getMinUBoundsHeight(obj:IUIComponent) : Number
      {
         var minHeight:Number = NaN;
         if(!isNaN(obj.explicitMinHeight))
         {
            minHeight = obj.explicitMinHeight;
         }
         else
         {
            minHeight = isNaN(obj.measuredMinHeight) ? 0 : obj.measuredMinHeight;
            if(!isNaN(obj.explicitMaxHeight))
            {
               minHeight = Math.min(minHeight,obj.explicitMaxHeight);
            }
         }
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            minHeight = obj.scaleY == 0 ? 0 : minHeight / obj.scaleY;
         }
         return minHeight;
      }
      
      private static function getMaxUBoundsWidth(obj:IUIComponent) : Number
      {
         var maxWidth:Number = NaN;
         if(!isNaN(obj.explicitMaxWidth))
         {
            maxWidth = obj.explicitMaxWidth;
         }
         else
         {
            maxWidth = DEFAULT_MAX_WIDTH;
         }
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            maxWidth = obj.scaleX == 0 ? 0 : maxWidth / obj.scaleX;
         }
         return maxWidth;
      }
      
      private static function getMaxUBoundsHeight(obj:IUIComponent) : Number
      {
         var maxHeight:Number = NaN;
         if(!isNaN(obj.explicitMaxHeight))
         {
            maxHeight = obj.explicitMaxHeight;
         }
         else
         {
            maxHeight = DEFAULT_MAX_HEIGHT;
         }
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            maxHeight = obj.scaleY == 0 ? 0 : maxHeight / obj.scaleY;
         }
         return maxHeight;
      }
      
      public static function getPreferredBoundsWidth(obj:IUIComponent, transformMatrix:Matrix) : Number
      {
         var width:Number = getPreferredUBoundsWidth(obj);
         if(Boolean(transformMatrix))
         {
            width = MatrixUtil.transformSize(width,getPreferredUBoundsHeight(obj),transformMatrix).x;
         }
         return width;
      }
      
      public static function getPreferredBoundsHeight(obj:IUIComponent, transformMatrix:Matrix) : Number
      {
         var height:Number = getPreferredUBoundsHeight(obj);
         if(Boolean(transformMatrix))
         {
            height = MatrixUtil.transformSize(getPreferredUBoundsWidth(obj),height,transformMatrix).y;
         }
         return height;
      }
      
      public static function getMinBoundsWidth(obj:IUIComponent, transformMatrix:Matrix) : Number
      {
         var width:Number = getMinUBoundsWidth(obj);
         if(Boolean(transformMatrix))
         {
            width = MatrixUtil.transformSize(width,getMinUBoundsHeight(obj),transformMatrix).x;
         }
         return width;
      }
      
      public static function getMinBoundsHeight(obj:IUIComponent, transformMatrix:Matrix) : Number
      {
         var height:Number = getMinUBoundsHeight(obj);
         if(Boolean(transformMatrix))
         {
            height = MatrixUtil.transformSize(getMinUBoundsWidth(obj),height,transformMatrix).y;
         }
         return height;
      }
      
      public static function getMaxBoundsWidth(obj:IUIComponent, transformMatrix:Matrix) : Number
      {
         var width:Number = getMaxUBoundsWidth(obj);
         if(Boolean(transformMatrix))
         {
            width = MatrixUtil.transformSize(width,getMaxUBoundsHeight(obj),transformMatrix).x;
         }
         return width;
      }
      
      public static function getMaxBoundsHeight(obj:IUIComponent, transformMatrix:Matrix) : Number
      {
         var height:Number = getMaxUBoundsHeight(obj);
         if(Boolean(transformMatrix))
         {
            height = MatrixUtil.transformSize(getMaxUBoundsWidth(obj),height,transformMatrix).y;
         }
         return height;
      }
      
      public static function getBoundsXAtSize(obj:IUIComponent, width:Number, height:Number, transformMatrix:Matrix) : Number
      {
         if(!transformMatrix)
         {
            return obj.x;
         }
         var fitSize:Point = MatrixUtil.fitBounds(width,height,transformMatrix,obj.explicitWidth,obj.explicitHeight,getPreferredUBoundsWidth(obj),getPreferredUBoundsHeight(obj),getMinUBoundsWidth(obj),getMinUBoundsHeight(obj),getMaxUBoundsWidth(obj),getMaxUBoundsHeight(obj));
         if(!fitSize)
         {
            fitSize = new Point(getMinUBoundsWidth(obj),getMinUBoundsHeight(obj));
         }
         var pos:Point = new Point();
         MatrixUtil.transformBounds(fitSize.x,fitSize.y,transformMatrix,pos);
         return pos.x;
      }
      
      public static function getBoundsYAtSize(obj:IUIComponent, width:Number, height:Number, transformMatrix:Matrix) : Number
      {
         if(!transformMatrix)
         {
            return obj.y;
         }
         var fitSize:Point = MatrixUtil.fitBounds(width,height,transformMatrix,obj.explicitWidth,obj.explicitHeight,getPreferredUBoundsWidth(obj),getPreferredUBoundsHeight(obj),getMinUBoundsWidth(obj),getMinUBoundsHeight(obj),getMaxUBoundsWidth(obj),getMaxUBoundsHeight(obj));
         if(!fitSize)
         {
            fitSize = new Point(getMinUBoundsWidth(obj),getMinUBoundsHeight(obj));
         }
         var pos:Point = new Point();
         MatrixUtil.transformBounds(fitSize.x,fitSize.y,transformMatrix,pos);
         return pos.y;
      }
      
      public static function getLayoutBoundsWidth(obj:IUIComponent, transformMatrix:Matrix) : Number
      {
         var height:Number = NaN;
         var width:Number = Number(obj.width);
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            width = obj.scaleX == 0 ? 0 : width / obj.scaleX;
         }
         if(Boolean(transformMatrix))
         {
            height = Number(obj.height);
            if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
            {
               height = obj.scaleY == 0 ? 0 : height / obj.scaleY;
            }
            width = MatrixUtil.transformBounds(width,height,transformMatrix,new Point()).x;
         }
         return width;
      }
      
      public static function getLayoutBoundsHeight(obj:IUIComponent, transformMatrix:Matrix) : Number
      {
         var width:Number = NaN;
         var height:Number = Number(obj.height);
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            height = obj.scaleY == 0 ? 0 : height / obj.scaleY;
         }
         if(Boolean(transformMatrix))
         {
            width = Number(obj.width);
            if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
            {
               width = obj.scaleX == 0 ? 0 : width / obj.scaleX;
            }
            height = MatrixUtil.transformBounds(width,height,transformMatrix,new Point()).y;
         }
         return height;
      }
      
      public static function getLayoutBoundsX(obj:IUIComponent, transformMatrix:Matrix) : Number
      {
         if(transformMatrix == null)
         {
            return obj.x;
         }
         var width:Number = Number(obj.width);
         var height:Number = Number(obj.height);
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            width = obj.scaleX == 0 ? 0 : width / obj.scaleX;
            height = obj.scaleY == 0 ? 0 : height / obj.scaleY;
         }
         var pos:Point = new Point();
         MatrixUtil.transformBounds(width,height,transformMatrix,pos);
         return pos.x;
      }
      
      public static function getLayoutBoundsY(obj:IUIComponent, transformMatrix:Matrix) : Number
      {
         if(transformMatrix == null)
         {
            return obj.y;
         }
         var width:Number = Number(obj.width);
         var height:Number = Number(obj.height);
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            width = obj.scaleX == 0 ? 0 : width / obj.scaleX;
            height = obj.scaleY == 0 ? 0 : height / obj.scaleY;
         }
         var pos:Point = new Point();
         MatrixUtil.transformBounds(width,height,transformMatrix,pos);
         return pos.y;
      }
      
      public static function setLayoutBoundsPosition(obj:IUIComponent, x:Number, y:Number, transformMatrix:Matrix) : void
      {
         if(Boolean(transformMatrix))
         {
            x = x - getLayoutBoundsX(obj,transformMatrix) + obj.x;
            y = y - getLayoutBoundsY(obj,transformMatrix) + obj.y;
         }
         obj.move(x,y);
      }
      
      public static function setLayoutBoundsSize(obj:IUIComponent, width:Number, height:Number, transformMatrix:Matrix) : void
      {
         if(!transformMatrix)
         {
            if(isNaN(width))
            {
               width = getPreferredUBoundsWidth(obj);
            }
            if(isNaN(height))
            {
               height = getPreferredUBoundsHeight(obj);
            }
            if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
            {
               width *= obj.scaleX;
               height *= obj.scaleY;
            }
            obj.setActualSize(width,height);
            return;
         }
         var fitSize:Point = MatrixUtil.fitBounds(width,height,transformMatrix,obj.explicitWidth,obj.explicitHeight,getPreferredUBoundsWidth(obj),getPreferredUBoundsHeight(obj),getMinUBoundsWidth(obj),getMinUBoundsHeight(obj),getMaxUBoundsWidth(obj),getMaxUBoundsHeight(obj));
         if(!fitSize)
         {
            fitSize = new Point(getMinUBoundsWidth(obj),getMinUBoundsHeight(obj));
         }
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            obj.setActualSize(fitSize.x * obj.scaleX,fitSize.y * obj.scaleY);
         }
         else
         {
            obj.setActualSize(fitSize.x,fitSize.y);
         }
      }
   }
}


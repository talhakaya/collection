package mx.graphics
{
   import flash.display.BitmapData;
   import flash.display.Graphics;
   import flash.display.Shape;
   import flash.filters.DropShadowFilter;
   import flash.geom.Matrix;
   import flash.geom.Point;
   import flash.geom.Rectangle;
   import mx.core.FlexShape;
   import mx.core.mx_internal;
   import mx.utils.GraphicsUtil;
   
   use namespace mx_internal;
   
   public class RectangularDropShadow
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private var shadow:BitmapData;
      
      private var leftShadow:BitmapData;
      
      private var rightShadow:BitmapData;
      
      private var topShadow:BitmapData;
      
      private var bottomShadow:BitmapData;
      
      private var changed:Boolean = true;
      
      private var _alpha:Number = 0.4;
      
      private var _angle:Number = 45;
      
      private var _color:int = 0;
      
      private var _distance:Number = 4;
      
      private var _tlRadius:Number = 0;
      
      private var _trRadius:Number = 0;
      
      private var _blRadius:Number = 0;
      
      private var _brRadius:Number = 0;
      
      private var _blurX:Number = 4;
      
      private var _blurY:Number = 4;
      
      public function RectangularDropShadow()
      {
         super();
      }
      
      [Inspectable]
      public function get alpha() : Number
      {
         return this._alpha;
      }
      
      public function set alpha(value:Number) : void
      {
         if(this._alpha != value)
         {
            this._alpha = value;
            this.changed = true;
         }
      }
      
      [Inspectable]
      public function get angle() : Number
      {
         return this._angle;
      }
      
      public function set angle(value:Number) : void
      {
         if(this._angle != value)
         {
            this._angle = value;
            this.changed = true;
         }
      }
      
      [Inspectable]
      public function get color() : int
      {
         return this._color;
      }
      
      public function set color(value:int) : void
      {
         if(this._color != value)
         {
            this._color = value;
            this.changed = true;
         }
      }
      
      [Inspectable]
      public function get distance() : Number
      {
         return this._distance;
      }
      
      public function set distance(value:Number) : void
      {
         if(this._distance != value)
         {
            this._distance = value;
            this.changed = true;
         }
      }
      
      [Inspectable]
      public function get tlRadius() : Number
      {
         return this._tlRadius;
      }
      
      public function set tlRadius(value:Number) : void
      {
         if(this._tlRadius != value)
         {
            this._tlRadius = value;
            this.changed = true;
         }
      }
      
      [Inspectable]
      public function get trRadius() : Number
      {
         return this._trRadius;
      }
      
      public function set trRadius(value:Number) : void
      {
         if(this._trRadius != value)
         {
            this._trRadius = value;
            this.changed = true;
         }
      }
      
      [Inspectable]
      public function get blRadius() : Number
      {
         return this._blRadius;
      }
      
      public function set blRadius(value:Number) : void
      {
         if(this._blRadius != value)
         {
            this._blRadius = value;
            this.changed = true;
         }
      }
      
      [Inspectable]
      public function get brRadius() : Number
      {
         return this._brRadius;
      }
      
      public function set brRadius(value:Number) : void
      {
         if(this._brRadius != value)
         {
            this._brRadius = value;
            this.changed = true;
         }
      }
      
      [Inspectable]
      public function get blurX() : Number
      {
         return this._blurX;
      }
      
      public function set blurX(value:Number) : void
      {
         if(this._blurX != value)
         {
            this._blurX = value;
            this.changed = true;
         }
      }
      
      [Inspectable]
      public function get blurY() : Number
      {
         return this._blurY;
      }
      
      public function set blurY(value:Number) : void
      {
         if(this._blurY != value)
         {
            this._blurY = value;
            this.changed = true;
         }
      }
      
      public function drawShadow(g:Graphics, x:Number, y:Number, width:Number, height:Number) : void
      {
         var tlWidth:Number = NaN;
         var tlHeight:Number = NaN;
         var trWidth:Number = NaN;
         var trHeight:Number = NaN;
         var blWidth:Number = NaN;
         var blHeight:Number = NaN;
         var brWidth:Number = NaN;
         var brHeight:Number = NaN;
         if(this.changed)
         {
            this.createShadowBitmaps();
            this.changed = false;
         }
         width = Math.ceil(width);
         height = Math.ceil(height);
         var leftThickness:int = Boolean(this.leftShadow) ? this.leftShadow.width : 0;
         var rightThickness:int = Boolean(this.rightShadow) ? this.rightShadow.width : 0;
         var topThickness:int = Boolean(this.topShadow) ? this.topShadow.height : 0;
         var bottomThickness:int = Boolean(this.bottomShadow) ? this.bottomShadow.height : 0;
         var widthThickness:int = leftThickness + rightThickness;
         var heightThickness:int = topThickness + bottomThickness;
         var maxCornerHeight:Number = (height + heightThickness) / 2;
         var maxCornerWidth:Number = (width + widthThickness) / 2;
         var matrix:Matrix = new Matrix();
         if(Boolean(this.leftShadow) || Boolean(this.topShadow))
         {
            tlWidth = Math.min(this.tlRadius + widthThickness,maxCornerWidth);
            tlHeight = Math.min(this.tlRadius + heightThickness,maxCornerHeight);
            matrix.tx = x - leftThickness;
            matrix.ty = y - topThickness;
            g.beginBitmapFill(this.shadow,matrix,false);
            g.drawRect(x - leftThickness,y - topThickness,tlWidth,tlHeight);
            g.endFill();
         }
         if(Boolean(this.rightShadow) || Boolean(this.topShadow))
         {
            trWidth = Math.min(this.trRadius + widthThickness,maxCornerWidth);
            trHeight = Math.min(this.trRadius + heightThickness,maxCornerHeight);
            matrix.tx = x + width + rightThickness - this.shadow.width;
            matrix.ty = y - topThickness;
            g.beginBitmapFill(this.shadow,matrix,false);
            g.drawRect(x + width + rightThickness - trWidth,y - topThickness,trWidth,trHeight);
            g.endFill();
         }
         if(Boolean(this.leftShadow) || Boolean(this.bottomShadow))
         {
            blWidth = Math.min(this.blRadius + widthThickness,maxCornerWidth);
            blHeight = Math.min(this.blRadius + heightThickness,maxCornerHeight);
            matrix.tx = x - leftThickness;
            matrix.ty = y + height + bottomThickness - this.shadow.height;
            g.beginBitmapFill(this.shadow,matrix,false);
            g.drawRect(x - leftThickness,y + height + bottomThickness - blHeight,blWidth,blHeight);
            g.endFill();
         }
         if(Boolean(this.rightShadow) || Boolean(this.bottomShadow))
         {
            brWidth = Math.min(this.brRadius + widthThickness,maxCornerWidth);
            brHeight = Math.min(this.brRadius + heightThickness,maxCornerHeight);
            matrix.tx = x + width + rightThickness - this.shadow.width;
            matrix.ty = y + height + bottomThickness - this.shadow.height;
            g.beginBitmapFill(this.shadow,matrix,false);
            g.drawRect(x + width + rightThickness - brWidth,y + height + bottomThickness - brHeight,brWidth,brHeight);
            g.endFill();
         }
         if(Boolean(this.leftShadow))
         {
            matrix.tx = x - leftThickness;
            matrix.ty = 0;
            g.beginBitmapFill(this.leftShadow,matrix,false);
            g.drawRect(x - leftThickness,y - topThickness + tlHeight,leftThickness,height + topThickness + bottomThickness - tlHeight - blHeight);
            g.endFill();
         }
         if(Boolean(this.rightShadow))
         {
            matrix.tx = x + width;
            matrix.ty = 0;
            g.beginBitmapFill(this.rightShadow,matrix,false);
            g.drawRect(x + width,y - topThickness + trHeight,rightThickness,height + topThickness + bottomThickness - trHeight - brHeight);
            g.endFill();
         }
         if(Boolean(this.topShadow))
         {
            matrix.tx = 0;
            matrix.ty = y - topThickness;
            g.beginBitmapFill(this.topShadow,matrix,false);
            g.drawRect(x - leftThickness + tlWidth,y - topThickness,width + leftThickness + rightThickness - tlWidth - trWidth,topThickness);
            g.endFill();
         }
         if(Boolean(this.bottomShadow))
         {
            matrix.tx = 0;
            matrix.ty = y + height;
            g.beginBitmapFill(this.bottomShadow,matrix,false);
            g.drawRect(x - leftThickness + blWidth,y + height,width + leftThickness + rightThickness - blWidth - brWidth,bottomThickness);
            g.endFill();
         }
      }
      
      private function createShadowBitmaps() : void
      {
         var roundRectWidth:Number = Math.max(this.tlRadius,this.blRadius) + 3 * Math.max(Math.abs(this.distance),2) + Math.max(this.trRadius,this.brRadius);
         var roundRectHeight:Number = Math.max(this.tlRadius,this.trRadius) + 3 * Math.max(Math.abs(this.distance),2) + Math.max(this.blRadius,this.brRadius);
         if(roundRectWidth < 0 || roundRectHeight < 0)
         {
            return;
         }
         var roundRect:Shape = new FlexShape();
         var g:Graphics = roundRect.graphics;
         g.beginFill(16777215);
         GraphicsUtil.drawRoundRectComplex(g,0,0,roundRectWidth,roundRectHeight,this.tlRadius,this.trRadius,this.blRadius,this.brRadius);
         g.endFill();
         var roundRectBitmap:BitmapData = new BitmapData(roundRectWidth,roundRectHeight,true,0);
         roundRectBitmap.draw(roundRect,new Matrix());
         var filter:DropShadowFilter = new DropShadowFilter(this.distance,this.angle,this.color,this.alpha,this.blurX,this.blurY);
         filter.knockout = true;
         var inputRect:Rectangle = new Rectangle(0,0,roundRectWidth,roundRectHeight);
         var outputRect:Rectangle = roundRectBitmap.generateFilterRect(inputRect,filter);
         var leftThickness:Number = inputRect.left - outputRect.left;
         var rightThickness:Number = outputRect.right - inputRect.right;
         var topThickness:Number = inputRect.top - outputRect.top;
         var bottomThickness:Number = outputRect.bottom - inputRect.bottom;
         this.shadow = new BitmapData(outputRect.width,outputRect.height);
         this.shadow.applyFilter(roundRectBitmap,inputRect,new Point(leftThickness,topThickness),filter);
         var origin:Point = new Point(0,0);
         var rect:Rectangle = new Rectangle();
         if(leftThickness > 0)
         {
            rect.x = 0;
            rect.y = this.tlRadius + topThickness + bottomThickness;
            rect.width = leftThickness;
            rect.height = 1;
            this.leftShadow = new BitmapData(leftThickness,1);
            this.leftShadow.copyPixels(this.shadow,rect,origin);
         }
         else
         {
            this.leftShadow = null;
         }
         if(rightThickness > 0)
         {
            rect.x = this.shadow.width - rightThickness;
            rect.y = this.trRadius + topThickness + bottomThickness;
            rect.width = rightThickness;
            rect.height = 1;
            this.rightShadow = new BitmapData(rightThickness,1);
            this.rightShadow.copyPixels(this.shadow,rect,origin);
         }
         else
         {
            this.rightShadow = null;
         }
         if(topThickness > 0)
         {
            rect.x = this.tlRadius + leftThickness + rightThickness;
            rect.y = 0;
            rect.width = 1;
            rect.height = topThickness;
            this.topShadow = new BitmapData(1,topThickness);
            this.topShadow.copyPixels(this.shadow,rect,origin);
         }
         else
         {
            this.topShadow = null;
         }
         if(bottomThickness > 0)
         {
            rect.x = this.blRadius + leftThickness + rightThickness;
            rect.y = this.shadow.height - bottomThickness;
            rect.width = 1;
            rect.height = bottomThickness;
            this.bottomShadow = new BitmapData(1,bottomThickness);
            this.bottomShadow.copyPixels(this.shadow,rect,origin);
         }
         else
         {
            this.bottomShadow = null;
         }
      }
   }
}


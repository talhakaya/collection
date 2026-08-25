package mx.utils
{
   import flash.display.DisplayObject;
   import flash.geom.Matrix;
   import flash.geom.Matrix3D;
   import flash.geom.PerspectiveProjection;
   import flash.geom.Point;
   import flash.geom.Rectangle;
   import flash.geom.Utils3D;
   import flash.geom.Vector3D;
   import flash.system.ApplicationDomain;
   import mx.core.mx_internal;
   
   use namespace mx_internal;
   
   [ExcludeClass]
   public final class MatrixUtil
   {
      
      private static var fakeDollarParent:QName;
      
      private static var uiComponentClass:Class;
      
      private static var uiMovieClipClass:Class;
      
      private static var usesMarshalling:Object;
      
      private static var lastModuleFactory:Object;
      
      private static var computedMatrixProperty:QName;
      
      private static var $transformProperty:QName;
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private static const RADIANS_PER_DEGREES:Number = Math.PI / 180;
      
      mx_internal static var SOLUTION_TOLERANCE:Number = 0.1;
      
      mx_internal static var MIN_MAX_TOLERANCE:Number = 0.1;
      
      private static var staticPoint:Point = new Point();
      
      public function MatrixUtil()
      {
         super();
      }
      
      public static function clampRotation(value:Number) : Number
      {
         if(value > 180 || value < -180)
         {
            value %= 360;
            if(value > 180)
            {
               value -= 360;
            }
            else if(value < -180)
            {
               value += 360;
            }
         }
         return value;
      }
      
      public static function transformPoint(x:Number, y:Number, m:Matrix) : Point
      {
         if(!m)
         {
            staticPoint.x = x;
            staticPoint.y = y;
            return staticPoint;
         }
         staticPoint.x = m.a * x + m.c * y + m.tx;
         staticPoint.y = m.b * x + m.d * y + m.ty;
         return staticPoint;
      }
      
      public static function composeMatrix(x:Number = 0, y:Number = 0, scaleX:Number = 1, scaleY:Number = 1, rotation:Number = 0, transformX:Number = 0, transformY:Number = 0) : Matrix
      {
         var m:Matrix = new Matrix();
         m.translate(-transformX,-transformY);
         m.scale(scaleX,scaleY);
         if(rotation != 0)
         {
            m.rotate(rotation / 180 * Math.PI);
         }
         m.translate(transformX + x,transformY + y);
         return m;
      }
      
      public static function decomposeMatrix(components:Vector.<Number>, matrix:Matrix, transformX:Number = 0, transformY:Number = 0) : void
      {
         var Ux:Number = NaN;
         var Uy:Number = NaN;
         var Vx:Number = NaN;
         var Vy:Number = NaN;
         var postTransformCenter:Point = null;
         Ux = matrix.a;
         Uy = matrix.b;
         components[3] = Math.sqrt(Ux * Ux + Uy * Uy);
         Vx = matrix.c;
         Vy = matrix.d;
         components[4] = Math.sqrt(Vx * Vx + Vy * Vy);
         var determinant:Number = Ux * Vy - Uy * Vx;
         if(determinant < 0)
         {
            components[4] = -components[4];
            Vx = -Vx;
            Vy = -Vy;
         }
         components[2] = Math.atan2(Uy,Ux) / RADIANS_PER_DEGREES;
         if(transformX != 0 || transformY != 0)
         {
            postTransformCenter = matrix.transformPoint(new Point(transformX,transformY));
            components[0] = postTransformCenter.x - transformX;
            components[1] = postTransformCenter.y - transformY;
         }
         else
         {
            components[0] = matrix.tx;
            components[1] = matrix.ty;
         }
      }
      
      public static function rectUnion(left:Number, top:Number, right:Number, bottom:Number, rect:Rectangle) : Rectangle
      {
         if(!rect)
         {
            return new Rectangle(left,top,right - left,bottom - top);
         }
         var minX:Number = Math.min(rect.left,left);
         var minY:Number = Math.min(rect.top,top);
         var maxX:Number = Math.max(rect.right,right);
         var maxY:Number = Math.max(rect.bottom,bottom);
         rect.x = minX;
         rect.y = minY;
         rect.width = maxX - minX;
         rect.height = maxY - minY;
         return rect;
      }
      
      public static function getEllipseBoundingBox(cx:Number, cy:Number, rx:Number, ry:Number, matrix:Matrix, rect:Rectangle = null) : Rectangle
      {
         var t:Number = NaN;
         var t1:Number = NaN;
         var pt:Point = null;
         var a:Number = matrix.a;
         var b:Number = matrix.b;
         var c:Number = matrix.c;
         var d:Number = matrix.d;
         if(rx == 0 && ry == 0)
         {
            pt = new Point(cx,cy);
            pt = matrix.transformPoint(pt);
            return rectUnion(pt.x,pt.y,pt.x,pt.y,rect);
         }
         if(a * rx == 0)
         {
            t = Math.PI / 2;
         }
         else
         {
            t = Math.atan(c * ry / (a * rx));
         }
         if(b * rx == 0)
         {
            t1 = Math.PI / 2;
         }
         else
         {
            t1 = Math.atan(d * ry / (b * rx));
         }
         var x1:Number = a * Math.cos(t) * rx + c * Math.sin(t) * ry;
         var x2:Number = -x1;
         x1 += a * cx + c * cy + matrix.tx;
         x2 += a * cx + c * cy + matrix.tx;
         var y1:Number = b * Math.cos(t1) * rx + d * Math.sin(t1) * ry;
         var y2:Number = -y1;
         y1 += b * cx + d * cy + matrix.ty;
         y2 += b * cx + d * cy + matrix.ty;
         return rectUnion(Math.min(x1,x2),Math.min(y1,y2),Math.max(x1,x2),Math.max(y1,y2),rect);
      }
      
      public static function getQBezierSegmentBBox(x0:Number, y0:Number, x1:Number, y1:Number, x2:Number, y2:Number, sx:Number, sy:Number, matrix:Matrix, rect:Rectangle) : Rectangle
      {
         var pt:Point = null;
         var tx:Number = NaN;
         var x:Number = NaN;
         var ty:Number = NaN;
         var y:Number = NaN;
         pt = MatrixUtil.transformPoint(x0 * sx,y0 * sy,matrix);
         x0 = pt.x;
         y0 = pt.y;
         pt = MatrixUtil.transformPoint(x1 * sx,y1 * sy,matrix);
         x1 = pt.x;
         y1 = pt.y;
         pt = MatrixUtil.transformPoint(x2 * sx,y2 * sy,matrix);
         x2 = pt.x;
         y2 = pt.y;
         var minX:Number = Math.min(x0,x2);
         var maxX:Number = Math.max(x0,x2);
         var minY:Number = Math.min(y0,y2);
         var maxY:Number = Math.max(y0,y2);
         var txDiv:Number = x0 - 2 * x1 + x2;
         if(txDiv != 0)
         {
            tx = (x0 - x1) / txDiv;
            if(0 <= tx && tx <= 1)
            {
               x = (1 - tx) * (1 - tx) * x0 + 2 * tx * (1 - tx) * x1 + tx * tx * x2;
               minX = Math.min(x,minX);
               maxX = Math.max(x,maxX);
            }
         }
         var tyDiv:Number = y0 - 2 * y1 + y2;
         if(tyDiv != 0)
         {
            ty = (y0 - y1) / tyDiv;
            if(0 <= ty && ty <= 1)
            {
               y = (1 - ty) * (1 - ty) * y0 + 2 * ty * (1 - ty) * y1 + ty * ty * y2;
               minY = Math.min(y,minY);
               maxY = Math.max(y,maxY);
            }
         }
         return rectUnion(minX,minY,maxX,maxY,rect);
      }
      
      public static function transformSize(width:Number, height:Number, matrix:Matrix) : Point
      {
         var a:Number = matrix.a;
         var b:Number = matrix.b;
         var c:Number = matrix.c;
         var d:Number = matrix.d;
         var x1:Number = 0;
         var y1:Number = 0;
         var x2:Number = width * a;
         var y2:Number = width * b;
         var x3:Number = height * c;
         var y3:Number = height * d;
         var x4:Number = x2 + x3;
         var y4:Number = y2 + y3;
         var minX:Number = Math.min(Math.min(x1,x2),Math.min(x3,x4));
         var maxX:Number = Math.max(Math.max(x1,x2),Math.max(x3,x4));
         var minY:Number = Math.min(Math.min(y1,y2),Math.min(y3,y4));
         var maxY:Number = Math.max(Math.max(y1,y2),Math.max(y3,y4));
         staticPoint.x = maxX - minX;
         staticPoint.y = maxY - minY;
         return staticPoint;
      }
      
      public static function transformBounds(width:Number, height:Number, matrix:Matrix, topLeft:Point = null) : Point
      {
         var tx:Number = NaN;
         var ty:Number = NaN;
         var x:Number = NaN;
         var y:Number = NaN;
         var a:Number = matrix.a;
         var b:Number = matrix.b;
         var c:Number = matrix.c;
         var d:Number = matrix.d;
         var x1:Number = 0;
         var y1:Number = 0;
         var x2:Number = width * a;
         var y2:Number = width * b;
         var x3:Number = height * c;
         var y3:Number = height * d;
         var x4:Number = x2 + x3;
         var y4:Number = y2 + y3;
         var minX:Number = Math.min(Math.min(x1,x2),Math.min(x3,x4));
         var maxX:Number = Math.max(Math.max(x1,x2),Math.max(x3,x4));
         var minY:Number = Math.min(Math.min(y1,y2),Math.min(y3,y4));
         var maxY:Number = Math.max(Math.max(y1,y2),Math.max(y3,y4));
         staticPoint.x = maxX - minX;
         staticPoint.y = maxY - minY;
         if(Boolean(topLeft))
         {
            tx = matrix.tx;
            ty = matrix.ty;
            x = topLeft.x;
            y = topLeft.y;
            topLeft.x = minX + a * x + b * y + tx;
            topLeft.y = minY + c * x + d * y + ty;
         }
         return staticPoint;
      }
      
      public static function projectBounds(bounds:Rectangle, matrix:Matrix3D, projection:PerspectiveProjection) : Rectangle
      {
         var centerX:Number = projection.projectionCenter.x;
         var centerY:Number = projection.projectionCenter.y;
         matrix.appendTranslation(-centerX,-centerY,projection.focalLength);
         matrix.append(projection.toMatrix3D());
         var pt1:Vector3D = new Vector3D(bounds.left,bounds.top,0);
         var pt2:Vector3D = new Vector3D(bounds.right,bounds.top,0);
         var pt3:Vector3D = new Vector3D(bounds.left,bounds.bottom,0);
         var pt4:Vector3D = new Vector3D(bounds.right,bounds.bottom,0);
         pt1 = Utils3D.projectVector(matrix,pt1);
         pt2 = Utils3D.projectVector(matrix,pt2);
         pt3 = Utils3D.projectVector(matrix,pt3);
         pt4 = Utils3D.projectVector(matrix,pt4);
         var maxX:Number = Math.max(Math.max(pt1.x,pt2.x),Math.max(pt3.x,pt4.x));
         var minX:Number = Math.min(Math.min(pt1.x,pt2.x),Math.min(pt3.x,pt4.x));
         var maxY:Number = Math.max(Math.max(pt1.y,pt2.y),Math.max(pt3.y,pt4.y));
         var minY:Number = Math.min(Math.min(pt1.y,pt2.y),Math.min(pt3.y,pt4.y));
         bounds.x = minX + centerX;
         bounds.y = minY + centerY;
         bounds.width = maxX - minX;
         bounds.height = maxY - minY;
         return bounds;
      }
      
      public static function isDeltaIdentity(matrix:Matrix) : Boolean
      {
         return matrix.a == 1 && matrix.d == 1 && matrix.b == 0 && matrix.c == 0;
      }
      
      public static function fitBounds(width:Number, height:Number, matrix:Matrix, explicitWidth:Number, explicitHeight:Number, preferredWidth:Number, preferredHeight:Number, minWidth:Number, minHeight:Number, maxWidth:Number, maxHeight:Number) : Point
      {
         var actualSize:Point = null;
         var actualSize1:Point = null;
         var actualSize2:Point = null;
         var fitHeight:Number = NaN;
         var fitWidth:Number = NaN;
         if(isNaN(width) && isNaN(height))
         {
            return new Point(preferredWidth,preferredHeight);
         }
         var newMinWidth:Number = minWidth < MIN_MAX_TOLERANCE ? 0 : minWidth - MIN_MAX_TOLERANCE;
         var newMinHeight:Number = minHeight < MIN_MAX_TOLERANCE ? 0 : minHeight - MIN_MAX_TOLERANCE;
         var newMaxWidth:Number = maxWidth + MIN_MAX_TOLERANCE;
         var newMaxHeight:Number = maxHeight + MIN_MAX_TOLERANCE;
         if(!isNaN(width) && !isNaN(height))
         {
            actualSize = calcUBoundsToFitTBounds(width,height,matrix,newMinWidth,newMinHeight,newMaxWidth,newMaxHeight);
            if(!actualSize)
            {
               actualSize1 = fitTBoundsWidth(width,matrix,explicitWidth,explicitHeight,preferredWidth,preferredHeight,newMinWidth,newMinHeight,newMaxWidth,newMaxHeight);
               if(Boolean(actualSize1))
               {
                  fitHeight = transformSize(actualSize1.x,actualSize1.y,matrix).y;
                  if(fitHeight - SOLUTION_TOLERANCE > height)
                  {
                     actualSize1 = null;
                  }
               }
               actualSize2 = fitTBoundsHeight(height,matrix,explicitWidth,explicitHeight,preferredWidth,preferredHeight,newMinWidth,newMinHeight,newMaxWidth,newMaxHeight);
               if(Boolean(actualSize2))
               {
                  fitWidth = transformSize(actualSize2.x,actualSize2.y,matrix).x;
                  if(fitWidth - SOLUTION_TOLERANCE > width)
                  {
                     actualSize2 = null;
                  }
               }
               if(Boolean(actualSize1) && Boolean(actualSize2))
               {
                  actualSize = actualSize1.x * actualSize1.y > actualSize2.x * actualSize2.y ? actualSize1 : actualSize2;
               }
               else if(Boolean(actualSize1))
               {
                  actualSize = actualSize1;
               }
               else
               {
                  actualSize = actualSize2;
               }
            }
            return actualSize;
         }
         if(!isNaN(width))
         {
            return fitTBoundsWidth(width,matrix,explicitWidth,explicitHeight,preferredWidth,preferredHeight,newMinWidth,newMinHeight,newMaxWidth,newMaxHeight);
         }
         return fitTBoundsHeight(height,matrix,explicitWidth,explicitHeight,preferredWidth,preferredHeight,newMinWidth,newMinHeight,newMaxWidth,newMaxHeight);
      }
      
      private static function fitTBoundsWidth(width:Number, matrix:Matrix, explicitWidth:Number, explicitHeight:Number, preferredWidth:Number, preferredHeight:Number, minWidth:Number, minHeight:Number, maxWidth:Number, maxHeight:Number) : Point
      {
         var actualSize:Point = null;
         if(!isNaN(explicitWidth) && isNaN(explicitHeight))
         {
            actualSize = calcUBoundsToFitTBoundsWidth(width,matrix,explicitWidth,preferredHeight,explicitWidth,minHeight,explicitWidth,maxHeight);
            if(Boolean(actualSize))
            {
               return actualSize;
            }
         }
         else if(isNaN(explicitWidth) && !isNaN(explicitHeight))
         {
            actualSize = calcUBoundsToFitTBoundsWidth(width,matrix,preferredWidth,explicitHeight,minWidth,explicitHeight,maxWidth,explicitHeight);
            if(Boolean(actualSize))
            {
               return actualSize;
            }
         }
         return calcUBoundsToFitTBoundsWidth(width,matrix,preferredWidth,preferredHeight,minWidth,minHeight,maxWidth,maxHeight);
      }
      
      private static function fitTBoundsHeight(height:Number, matrix:Matrix, explicitWidth:Number, explicitHeight:Number, preferredWidth:Number, preferredHeight:Number, minWidth:Number, minHeight:Number, maxWidth:Number, maxHeight:Number) : Point
      {
         var actualSize:Point = null;
         if(!isNaN(explicitWidth) && isNaN(explicitHeight))
         {
            actualSize = calcUBoundsToFitTBoundsHeight(height,matrix,explicitWidth,preferredHeight,explicitWidth,minHeight,explicitWidth,maxHeight);
            if(Boolean(actualSize))
            {
               return actualSize;
            }
         }
         else if(isNaN(explicitWidth) && !isNaN(explicitHeight))
         {
            actualSize = calcUBoundsToFitTBoundsHeight(height,matrix,preferredWidth,explicitHeight,minWidth,explicitHeight,maxWidth,explicitHeight);
            if(Boolean(actualSize))
            {
               return actualSize;
            }
         }
         return calcUBoundsToFitTBoundsHeight(height,matrix,preferredWidth,preferredHeight,minWidth,minHeight,maxWidth,maxHeight);
      }
      
      public static function calcUBoundsToFitTBoundsHeight(h:Number, matrix:Matrix, preferredX:Number, preferredY:Number, minX:Number, minY:Number, maxX:Number, maxY:Number) : Point
      {
         var s:Point = null;
         var x:Number = NaN;
         var y:Number = NaN;
         var a:Number = NaN;
         var c:Number = NaN;
         var invD1:Number = NaN;
         var invB:Number = NaN;
         var b:Number = matrix.b;
         var d:Number = matrix.d;
         if(-1e-9 < b && b < 1e-9)
         {
            b = 0;
         }
         if(-1e-9 < d && d < 1e-9)
         {
            d = 0;
         }
         if(b == 0 && d == 0)
         {
            return null;
         }
         if(b == 0 && d == 0)
         {
            return null;
         }
         if(b == 0)
         {
            return new Point(preferredX,h / Math.abs(d));
         }
         if(d == 0)
         {
            return new Point(h / Math.abs(b),preferredY);
         }
         var d1:Number = b * d >= 0 ? d : -d;
         if(d1 != 0 && preferredX > 0)
         {
            invD1 = 1 / d1;
            preferredX = Math.max(minX,Math.min(maxX,preferredX));
            x = preferredX;
            y = (h - b * x) * invD1;
            if(minY <= y && y <= maxY && b * x + d1 * y >= 0)
            {
               s = new Point(x,y);
            }
            y = (-h - b * x) * invD1;
            if(minY <= y && y <= maxY && b * x + d1 * y < 0)
            {
               if(!s || transformSize(s.x,s.y,matrix).x > transformSize(x,y,matrix).x)
               {
                  s = new Point(x,y);
               }
            }
         }
         if(b != 0 && preferredY > 0)
         {
            invB = 1 / b;
            preferredY = Math.max(minY,Math.min(maxY,preferredY));
            y = preferredY;
            x = (h - d1 * y) * invB;
            if(minX <= x && x <= maxX && b * x + d1 * y >= 0)
            {
               if(!s || transformSize(s.x,s.y,matrix).x > transformSize(x,y,matrix).x)
               {
                  s = new Point(x,y);
               }
            }
            x = (-h - d1 * y) * invB;
            if(minX <= x && x <= maxX && b * x + d1 * y < 0)
            {
               if(!s || transformSize(s.x,s.y,matrix).x > transformSize(x,y,matrix).x)
               {
                  s = new Point(x,y);
               }
            }
         }
         if(Boolean(s))
         {
            return s;
         }
         a = matrix.a;
         c = matrix.c;
         var c1:Number = a * c >= 0 ? c : -c;
         return solveEquation(b,d1,h,minX,minY,maxX,maxY,a,c1);
      }
      
      public static function calcUBoundsToFitTBoundsWidth(w:Number, matrix:Matrix, preferredX:Number, preferredY:Number, minX:Number, minY:Number, maxX:Number, maxY:Number) : Point
      {
         var s:Point = null;
         var x:Number = NaN;
         var y:Number = NaN;
         var b:Number = NaN;
         var d:Number = NaN;
         var invC1:Number = NaN;
         var invA:Number = NaN;
         var a:Number = matrix.a;
         var c:Number = matrix.c;
         if(-1e-9 < a && a < 1e-9)
         {
            a = 0;
         }
         if(-1e-9 < c && c < 1e-9)
         {
            c = 0;
         }
         if(a == 0 && c == 0)
         {
            return null;
         }
         if(a == 0)
         {
            return new Point(preferredX,w / Math.abs(c));
         }
         if(c == 0)
         {
            return new Point(w / Math.abs(a),preferredY);
         }
         var c1:Number = a * c >= 0 ? c : -c;
         if(c1 != 0 && preferredX > 0)
         {
            invC1 = 1 / c1;
            preferredX = Math.max(minX,Math.min(maxX,preferredX));
            x = preferredX;
            y = (w - a * x) * invC1;
            if(minY <= y && y <= maxY && a * x + c1 * y >= 0)
            {
               s = new Point(x,y);
            }
            y = (-w - a * x) * invC1;
            if(minY <= y && y <= maxY && a * x + c1 * y < 0)
            {
               if(!s || transformSize(s.x,s.y,matrix).y > transformSize(x,y,matrix).y)
               {
                  s = new Point(x,y);
               }
            }
         }
         if(a != 0 && preferredY > 0)
         {
            invA = 1 / a;
            preferredY = Math.max(minY,Math.min(maxY,preferredY));
            y = preferredY;
            x = (w - c1 * y) * invA;
            if(minX <= x && x <= maxX && a * x + c1 * y >= 0)
            {
               if(!s || transformSize(s.x,s.y,matrix).y > transformSize(x,y,matrix).y)
               {
                  s = new Point(x,y);
               }
            }
            x = (-w - c1 * y) * invA;
            if(minX <= x && x <= maxX && a * x + c1 * y < 0)
            {
               if(!s || transformSize(s.x,s.y,matrix).y > transformSize(x,y,matrix).y)
               {
                  s = new Point(x,y);
               }
            }
         }
         if(Boolean(s))
         {
            return s;
         }
         b = matrix.b;
         d = matrix.d;
         var d1:Number = b * d >= 0 ? d : -d;
         return solveEquation(a,c1,w,minX,minY,maxX,maxY,b,d1);
      }
      
      private static function solveEquation(a:Number, c:Number, w:Number, minX:Number, minY:Number, maxX:Number, maxY:Number, b:Number, d:Number) : Point
      {
         var x:Number = NaN;
         var y:Number = NaN;
         var s:Point = null;
         if(a == 0 || c == 0)
         {
            return null;
         }
         var A:Number = (w - minX * a) / c;
         var B:Number = (w - maxX * a) / c;
         var rangeMinY:Number = Math.max(minY,Math.min(A,B));
         var rangeMaxY:Number = Math.min(maxY,Math.max(A,B));
         var det:Number = b * c - a * d;
         if(rangeMinY <= rangeMaxY)
         {
            if(Math.abs(det) < 1e-9)
            {
               y = w / (a + c);
            }
            else
            {
               y = b * w / det;
            }
            y = Math.max(rangeMinY,Math.min(y,rangeMaxY));
            x = (w - c * y) / a;
            return new Point(x,y);
         }
         A = -(minX * a + w) / c;
         B = -(maxX * a + w) / c;
         rangeMinY = Math.max(minY,Math.min(A,B));
         rangeMaxY = Math.min(maxY,Math.max(A,B));
         if(rangeMinY <= rangeMaxY)
         {
            if(Math.abs(det) < 1e-9)
            {
               y = -w / (a + c);
            }
            else
            {
               y = -b * w / det;
            }
            y = Math.max(rangeMinY,Math.min(y,rangeMaxY));
            x = (-w - c * y) / a;
            return new Point(x,y);
         }
         return null;
      }
      
      public static function calcUBoundsToFitTBounds(w:Number, h:Number, matrix:Matrix, minX:Number, minY:Number, maxX:Number, maxY:Number) : Point
      {
         var s:Point = null;
         var a:Number = matrix.a;
         var b:Number = matrix.b;
         var c:Number = matrix.c;
         var d:Number = matrix.d;
         if(-1e-9 < a && a < 1e-9)
         {
            a = 0;
         }
         if(-1e-9 < b && b < 1e-9)
         {
            b = 0;
         }
         if(-1e-9 < c && c < 1e-9)
         {
            c = 0;
         }
         if(-1e-9 < d && d < 1e-9)
         {
            d = 0;
         }
         if(b == 0 && c == 0)
         {
            if(a == 0 || d == 0)
            {
               return null;
            }
            return new Point(w / Math.abs(a),h / Math.abs(d));
         }
         if(a == 0 && d == 0)
         {
            if(b == 0 || c == 0)
            {
               return null;
            }
            return new Point(h / Math.abs(b),w / Math.abs(c));
         }
         var c1:Number = a * c >= 0 ? c : -c;
         var d1:Number = b * d >= 0 ? d : -d;
         var det:Number = a * d1 - b * c1;
         if(Math.abs(det) < 1e-9)
         {
            if(c1 == 0 || a == 0 || a == -c1)
            {
               return null;
            }
            if(Math.abs(a * h - b * w) > 1e-9)
            {
               return null;
            }
            return solveEquation(a,c1,w,minX,minX,maxX,maxY,b,d1);
         }
         var invDet:Number = 1 / det;
         w *= invDet;
         h *= invDet;
         s = solveSystem(a,c1,b,d1,w,h);
         if(Boolean(s && minX <= s.x && s.x <= maxX && minY <= s.y && s.y <= maxY) && Boolean(a * s.x + c1 * s.x >= 0) && b * s.x + d1 * s.y >= 0)
         {
            return s;
         }
         s = solveSystem(a,c1,b,d1,w,-h);
         if(Boolean(s && minX <= s.x && s.x <= maxX && minY <= s.y && s.y <= maxY) && Boolean(a * s.x + c1 * s.x >= 0) && b * s.x + d1 * s.y < 0)
         {
            return s;
         }
         s = solveSystem(a,c1,b,d1,-w,h);
         if(Boolean(s && minX <= s.x && s.x <= maxX && minY <= s.y && s.y <= maxY) && Boolean(a * s.x + c1 * s.x < 0) && b * s.x + d1 * s.y >= 0)
         {
            return s;
         }
         s = solveSystem(a,c1,b,d1,-w,-h);
         if(Boolean(s && minX <= s.x && s.x <= maxX && minY <= s.y && s.y <= maxY) && Boolean(a * s.x + c1 * s.x < 0) && b * s.x + d1 * s.y < 0)
         {
            return s;
         }
         return null;
      }
      
      public static function isEqual(m1:Matrix, m2:Matrix) : Boolean
      {
         return m1 && m2 && m1.a == m2.a && m1.b == m2.b && m1.c == m2.c && m1.d == m2.d && m1.tx == m2.tx && m1.ty == m2.ty || !m1 && !m2;
      }
      
      public static function isEqual3D(m1:Matrix3D, m2:Matrix3D) : Boolean
      {
         var r1:Vector.<Number> = null;
         var r2:Vector.<Number> = null;
         if(Boolean(m1) && Boolean(m2) && m1.rawData.length == m2.rawData.length)
         {
            r1 = m1.rawData;
            r2 = m2.rawData;
            return r1[0] == r2[0] && r1[1] == r2[1] && r1[2] == r2[2] && r1[3] == r2[3] && r1[4] == r2[4] && r1[5] == r2[5] && r1[6] == r2[6] && r1[7] == r2[7] && r1[8] == r2[8] && r1[9] == r2[9] && r1[10] == r2[10] && r1[11] == r2[11] && r1[12] == r2[12] && r1[13] == r2[13] && r1[14] == r2[14] && r1[15] == r2[15];
         }
         return !m1 && !m2;
      }
      
      private static function solveSystem(a:Number, c:Number, b:Number, d:Number, mOverDet:Number, nOverDet:Number) : Point
      {
         return new Point(d * mOverDet - c * nOverDet,a * nOverDet - b * mOverDet);
      }
      
      public static function getConcatenatedMatrix(displayObject:DisplayObject, topParent:DisplayObject) : Matrix
      {
         return getConcatenatedMatrixHelper(displayObject,false,topParent);
      }
      
      public static function getConcatenatedComputedMatrix(displayObject:DisplayObject, topParent:DisplayObject) : Matrix
      {
         return getConcatenatedMatrixHelper(displayObject,true,topParent);
      }
      
      private static function getConcatenatedMatrixHelper(displayObject:DisplayObject, useComputedMatrix:Boolean, topParent:DisplayObject) : Matrix
      {
         var scrollRect:Rectangle = null;
         var isUIComponent:Boolean = false;
         var isUIMovieClip:Boolean = false;
         var moduleFactory:Object = null;
         var appDomain:ApplicationDomain = null;
         var m:Matrix = new Matrix();
         if(usesMarshalling == null)
         {
            usesMarshalling = ApplicationDomain.currentDomain.hasDefinition("mx.managers.systemClasses.MarshallingSupport");
            if(!usesMarshalling && ApplicationDomain.currentDomain.hasDefinition("mx.core.UIComponent"))
            {
               uiComponentClass = Class(ApplicationDomain.currentDomain.getDefinition("mx.core.UIComponent"));
            }
            if(!usesMarshalling && ApplicationDomain.currentDomain.hasDefinition("mx.flash.UIMovieClip"))
            {
               uiMovieClipClass = Class(ApplicationDomain.currentDomain.getDefinition("mx.flash.UIMovieClip"));
            }
         }
         if(fakeDollarParent == null)
         {
            fakeDollarParent = new QName(mx_internal,"$parent");
         }
         if(useComputedMatrix && computedMatrixProperty == null)
         {
            computedMatrixProperty = new QName(mx_internal,"computedMatrix");
         }
         if($transformProperty == null)
         {
            $transformProperty = new QName(mx_internal,"$transform");
         }
         while(Boolean(displayObject) && Boolean(displayObject.transform.matrix) && displayObject != topParent)
         {
            scrollRect = displayObject.scrollRect;
            if(scrollRect != null)
            {
               m.translate(-scrollRect.x,-scrollRect.y);
            }
            if(Boolean(usesMarshalling) && "moduleFactory" in displayObject)
            {
               moduleFactory = displayObject["moduleFactory"];
               if(Boolean(moduleFactory) && Boolean(moduleFactory !== lastModuleFactory) && "info" in moduleFactory)
               {
                  appDomain = moduleFactory["info"]()["currentDomain"];
                  if(Boolean(appDomain) && appDomain.hasDefinition("mx.core.UIComponent"))
                  {
                     uiComponentClass = Class(appDomain.getDefinition("mx.core.UIComponent"));
                  }
                  if(Boolean(appDomain) && appDomain.hasDefinition("mx.flash.UIMovieClip"))
                  {
                     uiMovieClipClass = Class(appDomain.getDefinition("mx.flash.UIMovieClip"));
                  }
                  lastModuleFactory = moduleFactory;
               }
            }
            isUIComponent = Boolean(uiComponentClass) && displayObject is uiComponentClass;
            isUIMovieClip = Boolean(uiMovieClipClass) && displayObject is uiMovieClipClass;
            if(useComputedMatrix && isUIComponent)
            {
               m.concat(displayObject[computedMatrixProperty]);
            }
            else if(isUIMovieClip)
            {
               m.concat(displayObject[$transformProperty].matrix);
            }
            else
            {
               m.concat(displayObject.transform.matrix);
            }
            if(isUIComponent)
            {
               displayObject = displayObject[fakeDollarParent] as DisplayObject;
            }
            else
            {
               displayObject = displayObject.parent as DisplayObject;
            }
         }
         return m;
      }
   }
}


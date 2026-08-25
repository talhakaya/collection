package mx.core
{
   import flash.events.Event;
   import flash.geom.Matrix;
   import flash.geom.Matrix3D;
   import flash.geom.Point;
   import flash.geom.Vector3D;
   import flash.system.Capabilities;
   import mx.geom.CompoundTransform;
   import mx.geom.TransformOffsets;
   
   use namespace mx_internal;
   
   public class AdvancedLayoutFeatures implements IAssetLayoutFeatures
   {
      
      private static var tempLocalPosition:Vector3D;
      
      private static const COMPUTED_MATRIX_VALID:uint = 1;
      
      private static const COMPUTED_MATRIX3D_VALID:uint = 2;
      
      private static var reVT:Vector3D = new Vector3D(0,0,0);
      
      private static var reVR:Vector3D = new Vector3D(0,0,0);
      
      private static var reVS:Vector3D = new Vector3D(1,1,1);
      
      private static var reV:Vector.<Vector3D> = new Vector.<Vector3D>();
      
      private static const RADIANS_PER_DEGREES:Number = Math.PI / 180;
      
      private static const ZERO_REPLACEMENT_IN_3D:Number = 1e-14;
      
      private static var transformVector:Function = initTransformVectorFunction;
      
      private static var staticTranslation:Vector3D = new Vector3D();
      
      private static var staticOffsetTranslation:Vector3D = new Vector3D();
      
      reV.push(reVT);
      reV.push(reVR);
      reV.push(reVS);
      
      public var updatePending:Boolean = false;
      
      public var depth:Number = 0;
      
      protected var _computedMatrix:Matrix;
      
      protected var _computedMatrix3D:Matrix3D;
      
      protected var layout:CompoundTransform;
      
      private var _postLayoutTransformOffsets:TransformOffsets;
      
      private var _flags:uint = 0;
      
      private var _layoutWidth:Number = 0;
      
      private var _mirror:Boolean = false;
      
      private var _stretchX:Number = 1;
      
      private var _stretchY:Number = 1;
      
      public function AdvancedLayoutFeatures()
      {
         super();
         this.layout = new CompoundTransform();
      }
      
      private static function pre10_0_22_87_transformVector(m:Matrix3D, v:Vector3D) : Vector3D
      {
         var r:Vector.<Number> = m.rawData;
         return new Vector3D(r[0] * v.x + r[4] * v.y + r[8] * v.z + r[12],r[1] * v.x + r[5] * v.y + r[9] * v.z + r[13],r[2] * v.x + r[6] * v.y + r[10] * v.z + r[14],1);
      }
      
      private static function nativeTransformVector(m:Matrix3D, v:Vector3D) : Vector3D
      {
         return m.transformVector(v);
      }
      
      private static function initTransformVectorFunction(m:Matrix3D, v:Vector3D) : Vector3D
      {
         var canUseNative:Boolean = false;
         var version:Array = Capabilities.version.split(" ")[1].split(",");
         if(parseFloat(version[0]) > 10)
         {
            canUseNative = true;
         }
         else if(parseFloat(version[1]) > 0)
         {
            canUseNative = true;
         }
         else if(parseFloat(version[2]) > 22)
         {
            canUseNative = true;
         }
         else if(parseFloat(version[3]) >= 87)
         {
            canUseNative = true;
         }
         if(canUseNative)
         {
            transformVector = nativeTransformVector;
         }
         else
         {
            transformVector = pre10_0_22_87_transformVector;
         }
         return transformVector(m,v);
      }
      
      public static function build2DMatrix(m:Matrix, tx:Number, ty:Number, sx:Number, sy:Number, rz:Number, x:Number, y:Number) : void
      {
         m.translate(-tx,-ty);
         m.scale(sx,sy);
         m.rotate(rz * RADIANS_PER_DEGREES);
         m.translate(x + tx,y + ty);
      }
      
      public static function build3DMatrix(m:Matrix3D, tx:Number, ty:Number, tz:Number, sx:Number, sy:Number, sz:Number, rx:Number, ry:Number, rz:Number, x:Number, y:Number, z:Number) : void
      {
         reVR.x = rx * RADIANS_PER_DEGREES;
         reVR.y = ry * RADIANS_PER_DEGREES;
         reVR.z = rz * RADIANS_PER_DEGREES;
         m.recompose(reV);
         if(sx == 0)
         {
            sx = ZERO_REPLACEMENT_IN_3D;
         }
         if(sy == 0)
         {
            sy = ZERO_REPLACEMENT_IN_3D;
         }
         if(sz == 0)
         {
            sz = ZERO_REPLACEMENT_IN_3D;
         }
         m.prependScale(sx,sy,sz);
         m.prependTranslation(-tx,-ty,-tz);
         m.appendTranslation(tx + x,ty + y,tz + z);
      }
      
      public function set layoutX(value:Number) : void
      {
         this.layout.x = value;
         this.invalidate();
      }
      
      public function get layoutX() : Number
      {
         return this.layout.x;
      }
      
      public function set layoutY(value:Number) : void
      {
         this.layout.y = value;
         this.invalidate();
      }
      
      public function get layoutY() : Number
      {
         return this.layout.y;
      }
      
      public function set layoutZ(value:Number) : void
      {
         this.layout.z = value;
         this.invalidate();
      }
      
      public function get layoutZ() : Number
      {
         return this.layout.z;
      }
      
      public function get layoutWidth() : Number
      {
         return this._layoutWidth;
      }
      
      public function set layoutWidth(value:Number) : void
      {
         if(value == this._layoutWidth)
         {
            return;
         }
         this._layoutWidth = value;
         this.invalidate();
      }
      
      public function set transformX(value:Number) : void
      {
         this.layout.transformX = value;
         this.invalidate();
      }
      
      public function get transformX() : Number
      {
         return this.layout.transformX;
      }
      
      public function set transformY(value:Number) : void
      {
         this.layout.transformY = value;
         this.invalidate();
      }
      
      public function get transformY() : Number
      {
         return this.layout.transformY;
      }
      
      public function set transformZ(value:Number) : void
      {
         this.layout.transformZ = value;
         this.invalidate();
      }
      
      public function get transformZ() : Number
      {
         return this.layout.transformZ;
      }
      
      public function set layoutRotationX(value:Number) : void
      {
         this.layout.rotationX = value;
         this.invalidate();
      }
      
      public function get layoutRotationX() : Number
      {
         return this.layout.rotationX;
      }
      
      public function set layoutRotationY(value:Number) : void
      {
         this.layout.rotationY = value;
         this.invalidate();
      }
      
      public function get layoutRotationY() : Number
      {
         return this.layout.rotationY;
      }
      
      public function set layoutRotationZ(value:Number) : void
      {
         this.layout.rotationZ = value;
         this.invalidate();
      }
      
      public function get layoutRotationZ() : Number
      {
         return this.layout.rotationZ;
      }
      
      public function set layoutScaleX(value:Number) : void
      {
         this.layout.scaleX = value;
         this.invalidate();
      }
      
      public function get layoutScaleX() : Number
      {
         return this.layout.scaleX;
      }
      
      public function set layoutScaleY(value:Number) : void
      {
         this.layout.scaleY = value;
         this.invalidate();
      }
      
      public function get layoutScaleY() : Number
      {
         return this.layout.scaleY;
      }
      
      public function set layoutScaleZ(value:Number) : void
      {
         this.layout.scaleZ = value;
         this.invalidate();
      }
      
      public function get layoutScaleZ() : Number
      {
         return this.layout.scaleZ;
      }
      
      public function set layoutMatrix(value:Matrix) : void
      {
         this.layout.matrix = value;
         this.invalidate();
      }
      
      public function get layoutMatrix() : Matrix
      {
         return this.layout.matrix;
      }
      
      public function set layoutMatrix3D(value:Matrix3D) : void
      {
         this.layout.matrix3D = value;
         this.invalidate();
      }
      
      public function get layoutMatrix3D() : Matrix3D
      {
         return this.layout.matrix3D;
      }
      
      public function get is3D() : Boolean
      {
         return this.layout.is3D || this.postLayoutTransformOffsets != null && this.postLayoutTransformOffsets.is3D;
      }
      
      public function get layoutIs3D() : Boolean
      {
         return this.layout.is3D;
      }
      
      public function set postLayoutTransformOffsets(value:TransformOffsets) : void
      {
         if(this._postLayoutTransformOffsets != null)
         {
            this._postLayoutTransformOffsets.removeEventListener(Event.CHANGE,this.postLayoutTransformOffsetsChangedHandler);
            this._postLayoutTransformOffsets.owner = null;
         }
         this._postLayoutTransformOffsets = value;
         if(this._postLayoutTransformOffsets != null)
         {
            this._postLayoutTransformOffsets.addEventListener(Event.CHANGE,this.postLayoutTransformOffsetsChangedHandler);
            this._postLayoutTransformOffsets.owner = this;
         }
         this.invalidate();
      }
      
      public function get postLayoutTransformOffsets() : TransformOffsets
      {
         return this._postLayoutTransformOffsets;
      }
      
      private function postLayoutTransformOffsetsChangedHandler(e:Event) : void
      {
         this.invalidate();
      }
      
      public function get mirror() : Boolean
      {
         return this._mirror;
      }
      
      public function set mirror(value:Boolean) : void
      {
         this._mirror = value;
         this.invalidate();
      }
      
      public function get stretchX() : Number
      {
         return this._stretchX;
      }
      
      public function set stretchX(value:Number) : void
      {
         if(value == this._stretchX)
         {
            return;
         }
         this._stretchX = value;
         this.invalidate();
      }
      
      public function get stretchY() : Number
      {
         return this._stretchY;
      }
      
      public function set stretchY(value:Number) : void
      {
         if(value == this._stretchY)
         {
            return;
         }
         this._stretchY = value;
         this.invalidate();
      }
      
      private function invalidate() : void
      {
         this._flags &= ~COMPUTED_MATRIX_VALID;
         this._flags &= ~COMPUTED_MATRIX3D_VALID;
      }
      
      public function get computedMatrix() : Matrix
      {
         if(Boolean(this._flags & COMPUTED_MATRIX_VALID))
         {
            return this._computedMatrix;
         }
         if(!this.postLayoutTransformOffsets && !this.mirror && this.stretchX == 1 && this.stretchY == 1)
         {
            return this.layout.matrix;
         }
         var m:Matrix = this._computedMatrix;
         if(m == null)
         {
            m = this._computedMatrix = new Matrix();
         }
         else
         {
            m.identity();
         }
         var tx:Number = this.layout.transformX;
         var ty:Number = this.layout.transformY;
         var sx:Number = this.layout.scaleX;
         var sy:Number = this.layout.scaleY;
         var rz:Number = this.layout.rotationZ;
         var x:Number = this.layout.x;
         var y:Number = this.layout.y;
         if(this.mirror)
         {
            sx *= -1;
            x += this.layoutWidth;
         }
         if(Boolean(this.postLayoutTransformOffsets))
         {
            sx *= this.postLayoutTransformOffsets.scaleX;
            sy *= this.postLayoutTransformOffsets.scaleY;
            rz += this.postLayoutTransformOffsets.rotationZ;
            x += this.postLayoutTransformOffsets.x;
            y += this.postLayoutTransformOffsets.y;
         }
         if(this.stretchX != 1 || this.stretchY != 1)
         {
            m.scale(this.stretchX,this.stretchY);
         }
         build2DMatrix(m,tx,ty,sx,sy,rz,x,y);
         this._flags |= COMPUTED_MATRIX_VALID;
         return m;
      }
      
      public function get computedMatrix3D() : Matrix3D
      {
         if(Boolean(this._flags & COMPUTED_MATRIX3D_VALID))
         {
            return this._computedMatrix3D;
         }
         if(!this.postLayoutTransformOffsets && !this.mirror && this.stretchX == 1 && this.stretchY == 1)
         {
            return this.layout.matrix3D;
         }
         var m:Matrix3D = this._computedMatrix3D;
         if(m == null)
         {
            m = this._computedMatrix3D = new Matrix3D();
         }
         else
         {
            m.identity();
         }
         var tx:Number = this.layout.transformX;
         var ty:Number = this.layout.transformY;
         var tz:Number = this.layout.transformZ;
         var sx:Number = this.layout.scaleX;
         var sy:Number = this.layout.scaleY;
         var sz:Number = this.layout.scaleZ;
         var rx:Number = this.layout.rotationX;
         var ry:Number = this.layout.rotationY;
         var rz:Number = this.layout.rotationZ;
         var x:Number = this.layout.x;
         var y:Number = this.layout.y;
         var z:Number = this.layout.z;
         if(this.mirror)
         {
            sx *= -1;
            x += this.layoutWidth;
         }
         if(Boolean(this.postLayoutTransformOffsets))
         {
            sx *= this.postLayoutTransformOffsets.scaleX;
            sy *= this.postLayoutTransformOffsets.scaleY;
            sz *= this.postLayoutTransformOffsets.scaleZ;
            rx += this.postLayoutTransformOffsets.rotationX;
            ry += this.postLayoutTransformOffsets.rotationY;
            rz += this.postLayoutTransformOffsets.rotationZ;
            x += this.postLayoutTransformOffsets.x;
            y += this.postLayoutTransformOffsets.y;
            z += this.postLayoutTransformOffsets.z;
         }
         build3DMatrix(m,tx,ty,tz,sx,sy,sz,rx,ry,rz,x,y,z);
         if(this.stretchX != 1 || this.stretchY != 1)
         {
            m.prependScale(this.stretchX,this.stretchY,1);
         }
         this._flags |= COMPUTED_MATRIX3D_VALID;
         return m;
      }
      
      public function transformPointToParent(propertyIs3D:Boolean, localPosition:Vector3D, position:Vector3D, postLayoutPosition:Vector3D) : void
      {
         var transformedV:Vector3D = null;
         var transformedP:Point = null;
         var localP:Point = null;
         tempLocalPosition = Boolean(localPosition) ? localPosition.clone() : new Vector3D();
         if(this.is3D || propertyIs3D)
         {
            if(position != null)
            {
               transformedV = transformVector(this.layoutMatrix3D,tempLocalPosition);
               position.x = transformedV.x;
               position.y = transformedV.y;
               position.z = transformedV.z;
            }
            if(postLayoutPosition != null)
            {
               tempLocalPosition.x /= this.stretchX;
               tempLocalPosition.y /= this.stretchY;
               transformedV = transformVector(this.computedMatrix3D,tempLocalPosition);
               postLayoutPosition.x = transformedV.x;
               postLayoutPosition.y = transformedV.y;
               postLayoutPosition.z = transformedV.z;
            }
         }
         else
         {
            localP = new Point(tempLocalPosition.x,tempLocalPosition.y);
            if(position != null)
            {
               transformedP = this.layoutMatrix.transformPoint(localP);
               position.x = transformedP.x;
               position.y = transformedP.y;
               position.z = 0;
            }
            if(postLayoutPosition != null)
            {
               localP.x /= this.stretchX;
               localP.y /= this.stretchY;
               transformedP = this.computedMatrix.transformPoint(localP);
               postLayoutPosition.x = transformedP.x;
               postLayoutPosition.y = transformedP.y;
               postLayoutPosition.z = 0;
            }
         }
      }
      
      private function completeTransformCenterAdjustment(changeIs3D:Boolean, transformCenter:Vector3D, targetPosition:Vector3D, targetPostLayoutPosition:Vector3D) : void
      {
         var adjustedLayoutCenterV:Vector3D = null;
         var tmpPos:Vector3D = null;
         var adjustedComputedCenterV:Vector3D = null;
         var transformCenterP:Point = null;
         var currentPositionP:Point = null;
         var currentPostLayoutPosition:Point = null;
         if(this.is3D || changeIs3D)
         {
            if(targetPosition != null)
            {
               adjustedLayoutCenterV = transformVector(this.layoutMatrix3D,transformCenter);
               if(adjustedLayoutCenterV.equals(targetPosition) == false)
               {
                  this.layout.translateBy(targetPosition.x - adjustedLayoutCenterV.x,targetPosition.y - adjustedLayoutCenterV.y,targetPosition.z - adjustedLayoutCenterV.z);
                  this.invalidate();
               }
            }
            if(targetPostLayoutPosition != null && this._postLayoutTransformOffsets != null)
            {
               tmpPos = new Vector3D(transformCenter.x,transformCenter.y,transformCenter.z);
               tmpPos.x /= this.stretchX;
               tmpPos.y /= this.stretchY;
               adjustedComputedCenterV = transformVector(this.computedMatrix3D,tmpPos);
               if(adjustedComputedCenterV.equals(targetPostLayoutPosition) == false)
               {
                  this.postLayoutTransformOffsets.x += targetPostLayoutPosition.x - adjustedComputedCenterV.x;
                  this.postLayoutTransformOffsets.y += targetPostLayoutPosition.y - adjustedComputedCenterV.y;
                  this.postLayoutTransformOffsets.z += targetPostLayoutPosition.z - adjustedComputedCenterV.z;
                  this.invalidate();
               }
            }
         }
         else
         {
            transformCenterP = new Point(transformCenter.x,transformCenter.y);
            if(targetPosition != null)
            {
               currentPositionP = this.layoutMatrix.transformPoint(transformCenterP);
               if(currentPositionP.x != targetPosition.x || currentPositionP.y != targetPosition.y)
               {
                  this.layout.translateBy(targetPosition.x - currentPositionP.x,targetPosition.y - currentPositionP.y,0);
                  this.invalidate();
               }
            }
            if(targetPostLayoutPosition != null && this._postLayoutTransformOffsets != null)
            {
               transformCenterP.x /= this.stretchX;
               transformCenterP.y /= this.stretchY;
               currentPostLayoutPosition = this.computedMatrix.transformPoint(transformCenterP);
               if(currentPostLayoutPosition.x != targetPostLayoutPosition.x || currentPostLayoutPosition.y != targetPostLayoutPosition.y)
               {
                  this._postLayoutTransformOffsets.x += targetPostLayoutPosition.x - currentPostLayoutPosition.x;
                  this._postLayoutTransformOffsets.y += targetPostLayoutPosition.y - currentPostLayoutPosition.y;
                  this.invalidate();
               }
            }
         }
      }
      
      public function transformAround(transformCenter:Vector3D, scale:Vector3D, rotation:Vector3D, transformCenterPosition:Vector3D, postLayoutScale:Vector3D = null, postLayoutRotation:Vector3D = null, postLayoutTransformCenterPosition:Vector3D = null) : void
      {
         var is3D:Boolean = scale != null && scale.z != 1 || rotation != null && (rotation.x != 0 || rotation.y != 0) || transformCenterPosition != null && transformCenterPosition.z != 0 || postLayoutScale != null && postLayoutScale.z != 1 || postLayoutRotation != null && (postLayoutRotation.x != 0 || postLayoutRotation.y != 0) || postLayoutTransformCenterPosition != null && postLayoutTransformCenterPosition.z != 0;
         var needOffsets:Boolean = this._postLayoutTransformOffsets == null && (postLayoutScale != null || postLayoutRotation != null || postLayoutTransformCenterPosition != null);
         if(needOffsets)
         {
            this._postLayoutTransformOffsets = new TransformOffsets();
         }
         if(transformCenter != null && (transformCenterPosition == null || postLayoutTransformCenterPosition == null))
         {
            this.transformPointToParent(is3D,transformCenter,staticTranslation,staticOffsetTranslation);
            if(postLayoutTransformCenterPosition == null && transformCenterPosition != null)
            {
               staticOffsetTranslation.x = transformCenterPosition.x + staticOffsetTranslation.x - staticTranslation.x;
               staticOffsetTranslation.y = transformCenterPosition.y + staticOffsetTranslation.y - staticTranslation.y;
               staticOffsetTranslation.z = transformCenterPosition.z + staticOffsetTranslation.z - staticTranslation.z;
            }
         }
         var targetPosition:Vector3D = transformCenterPosition == null ? staticTranslation : transformCenterPosition;
         var postLayoutTargetPosition:Vector3D = postLayoutTransformCenterPosition == null ? staticOffsetTranslation : postLayoutTransformCenterPosition;
         if(rotation != null)
         {
            if(!isNaN(rotation.x))
            {
               this.layout.rotationX = rotation.x;
            }
            if(!isNaN(rotation.y))
            {
               this.layout.rotationY = rotation.y;
            }
            if(!isNaN(rotation.z))
            {
               this.layout.rotationZ = rotation.z;
            }
         }
         if(scale != null)
         {
            if(!isNaN(scale.x))
            {
               this.layout.scaleX = scale.x;
            }
            if(!isNaN(scale.y))
            {
               this.layout.scaleY = scale.y;
            }
            if(!isNaN(scale.z))
            {
               this.layout.scaleZ = scale.z;
            }
         }
         if(postLayoutRotation != null)
         {
            this._postLayoutTransformOffsets.rotationX = postLayoutRotation.x;
            this._postLayoutTransformOffsets.rotationY = postLayoutRotation.y;
            this._postLayoutTransformOffsets.rotationZ = postLayoutRotation.z;
         }
         if(postLayoutScale != null)
         {
            this._postLayoutTransformOffsets.scaleX = postLayoutScale.x;
            this._postLayoutTransformOffsets.scaleY = postLayoutScale.y;
            this._postLayoutTransformOffsets.scaleZ = postLayoutScale.z;
         }
         if(transformCenter == null)
         {
            if(transformCenterPosition != null)
            {
               this.layout.x = transformCenterPosition.x;
               this.layout.y = transformCenterPosition.y;
               this.layout.z = transformCenterPosition.z;
            }
            if(postLayoutTransformCenterPosition != null)
            {
               this._postLayoutTransformOffsets.x = postLayoutTransformCenterPosition.x - this.layout.x;
               this._postLayoutTransformOffsets.y = postLayoutTransformCenterPosition.y - this.layout.y;
               this._postLayoutTransformOffsets.z = postLayoutTransformCenterPosition.z - this.layout.z;
            }
         }
         this.invalidate();
         if(transformCenter != null)
         {
            this.completeTransformCenterAdjustment(is3D,transformCenter,targetPosition,postLayoutTargetPosition);
         }
      }
   }
}


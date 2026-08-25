package mx.geom
{
   import flash.geom.Matrix;
   import flash.geom.Matrix3D;
   import flash.geom.Vector3D;
   import mx.core.AdvancedLayoutFeatures;
   import mx.utils.MatrixUtil;
   
   public class CompoundTransform
   {
      
      private static const MATRIX_VALID:uint = 32;
      
      private static const MATRIX3D_VALID:uint = 64;
      
      private static const PROPERTIES_VALID:uint = 128;
      
      private static const IS_3D:uint = 512;
      
      private static const M3D_FLAGS_VALID:uint = 1024;
      
      public static const SOURCE_PROPERTIES:uint = 1;
      
      public static const SOURCE_MATRIX:uint = 2;
      
      public static const SOURCE_MATRIX3D:uint = 3;
      
      private static const INVALIDATE_FROM_NONE:uint = 0;
      
      private static const INVALIDATE_FROM_PROPERTY:uint = 4;
      
      private static const INVALIDATE_FROM_MATRIX:uint = 5;
      
      private static const INVALIDATE_FROM_MATRIX3D:uint = 6;
      
      private static var decomposition:Vector.<Number> = new Vector.<Number>();
      
      private static const RADIANS_PER_DEGREES:Number = Math.PI / 180;
      
      private static const EPSILON:Number = 0.001;
      
      decomposition.push(0);
      decomposition.push(0);
      decomposition.push(0);
      decomposition.push(0);
      decomposition.push(0);
      
      private var _rotationX:Number = 0;
      
      private var _rotationY:Number = 0;
      
      private var _rotationZ:Number = 0;
      
      private var _scaleX:Number = 1;
      
      private var _scaleY:Number = 1;
      
      private var _scaleZ:Number = 1;
      
      private var _x:Number = 0;
      
      private var _y:Number = 0;
      
      private var _z:Number = 0;
      
      private var _transformX:Number = 0;
      
      private var _transformY:Number = 0;
      
      private var _transformZ:Number = 0;
      
      private var _matrix:Matrix;
      
      private var _matrix3D:Matrix3D;
      
      public var sourceOfTruth:uint = 1;
      
      private var _flags:uint = 128;
      
      public function CompoundTransform()
      {
         super();
      }
      
      public function set x(value:Number) : void
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         if(value == this._x)
         {
            return;
         }
         this.translateBy(value - this._x,0,0);
         this.invalidate(INVALIDATE_FROM_PROPERTY,false);
      }
      
      public function get x() : Number
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         return this._x;
      }
      
      public function set y(value:Number) : void
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         if(value == this._y)
         {
            return;
         }
         this.translateBy(0,value - this._y,0);
         this.invalidate(INVALIDATE_FROM_PROPERTY,false);
      }
      
      public function get y() : Number
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         return this._y;
      }
      
      public function set z(value:Number) : void
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         if(value == this._z)
         {
            return;
         }
         this.translateBy(0,0,value - this._z);
         this.invalidate(INVALIDATE_FROM_PROPERTY,true);
      }
      
      public function get z() : Number
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         return this._z;
      }
      
      public function set rotationX(value:Number) : void
      {
         value = MatrixUtil.clampRotation(value);
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         if(value == this._rotationX)
         {
            return;
         }
         this._rotationX = value;
         this.invalidate(INVALIDATE_FROM_PROPERTY,true);
      }
      
      public function get rotationX() : Number
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         return this._rotationX;
      }
      
      public function set rotationY(value:Number) : void
      {
         value = MatrixUtil.clampRotation(value);
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         if(value == this._rotationY)
         {
            return;
         }
         this._rotationY = value;
         this.invalidate(INVALIDATE_FROM_PROPERTY,true);
      }
      
      public function get rotationY() : Number
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         return this._rotationY;
      }
      
      public function set rotationZ(value:Number) : void
      {
         value = MatrixUtil.clampRotation(value);
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         if(value == this._rotationZ)
         {
            return;
         }
         this._rotationZ = value;
         this.invalidate(INVALIDATE_FROM_PROPERTY,false);
      }
      
      public function get rotationZ() : Number
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         return this._rotationZ;
      }
      
      public function set scaleX(value:Number) : void
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         if(value == this._scaleX)
         {
            return;
         }
         this._scaleX = value;
         this.invalidate(INVALIDATE_FROM_PROPERTY,false);
      }
      
      public function get scaleX() : Number
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         return this._scaleX;
      }
      
      public function set scaleY(value:Number) : void
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         if(value == this._scaleY)
         {
            return;
         }
         this._scaleY = value;
         this.invalidate(INVALIDATE_FROM_PROPERTY,false);
      }
      
      public function get scaleY() : Number
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         return this._scaleY;
      }
      
      public function set scaleZ(value:Number) : void
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         if(value == this._scaleZ)
         {
            return;
         }
         this._scaleZ = value;
         this.invalidate(INVALIDATE_FROM_PROPERTY,true);
      }
      
      public function get scaleZ() : Number
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         return this._scaleZ;
      }
      
      public function get is3D() : Boolean
      {
         if((this._flags & M3D_FLAGS_VALID) == 0)
         {
            this.update3DFlags();
         }
         return (this._flags & IS_3D) != 0;
      }
      
      public function set transformX(value:Number) : void
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         if(value == this._transformX)
         {
            return;
         }
         this._transformX = value;
         this.invalidate(INVALIDATE_FROM_PROPERTY,true);
      }
      
      public function get transformX() : Number
      {
         return this._transformX;
      }
      
      public function set transformY(value:Number) : void
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         if(value == this._transformY)
         {
            return;
         }
         this._transformY = value;
         this.invalidate(INVALIDATE_FROM_PROPERTY,true);
      }
      
      public function get transformY() : Number
      {
         return this._transformY;
      }
      
      public function set transformZ(value:Number) : void
      {
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         if(value == this._transformZ)
         {
            return;
         }
         this._transformZ = value;
         this.invalidate(INVALIDATE_FROM_PROPERTY,true);
      }
      
      public function get transformZ() : Number
      {
         return this._transformZ;
      }
      
      private function invalidate(reason:uint, affects3D:Boolean) : void
      {
         switch(reason)
         {
            case INVALIDATE_FROM_PROPERTY:
               this.sourceOfTruth = SOURCE_PROPERTIES;
               this._flags |= PROPERTIES_VALID;
               this._flags &= ~MATRIX_VALID;
               this._flags &= ~MATRIX3D_VALID;
               break;
            case INVALIDATE_FROM_MATRIX:
               this.sourceOfTruth = SOURCE_MATRIX;
               this._flags |= MATRIX_VALID;
               this._flags &= ~PROPERTIES_VALID;
               this._flags &= ~MATRIX3D_VALID;
               break;
            case INVALIDATE_FROM_MATRIX3D:
               this.sourceOfTruth = SOURCE_MATRIX3D;
               this._flags |= MATRIX3D_VALID;
               this._flags &= ~PROPERTIES_VALID;
               this._flags &= ~MATRIX_VALID;
         }
         if(affects3D)
         {
            this._flags &= ~M3D_FLAGS_VALID;
         }
      }
      
      private function update3DFlags() : void
      {
         var matrixIs3D:Boolean = false;
         var rawData:Vector.<Number> = null;
         if((this._flags & M3D_FLAGS_VALID) == 0)
         {
            matrixIs3D = false;
            switch(this.sourceOfTruth)
            {
               case SOURCE_PROPERTIES:
                  matrixIs3D = Math.abs(this._scaleZ - 1) > EPSILON || (Math.abs(this._rotationX) + EPSILON) % 360 > 2 * EPSILON || (Math.abs(this._rotationY) + EPSILON) % 360 > 2 * EPSILON || Math.abs(this._z) > EPSILON;
                  break;
               case SOURCE_MATRIX:
                  matrixIs3D = false;
                  break;
               case SOURCE_MATRIX3D:
                  rawData = this._matrix3D.rawData;
                  matrixIs3D = rawData[2] != 0 || rawData[6] != 0 || rawData[8] != 0 || rawData[10] != 1 || rawData[14] != 0;
            }
            if(matrixIs3D)
            {
               this._flags |= IS_3D;
            }
            else
            {
               this._flags &= ~IS_3D;
            }
            this._flags |= M3D_FLAGS_VALID;
         }
      }
      
      public function translateBy(x:Number, y:Number, z:Number = 0) : void
      {
         var data:Vector.<Number> = null;
         if(Boolean(this._flags & MATRIX_VALID))
         {
            this._matrix.tx += x;
            this._matrix.ty += y;
         }
         if(Boolean(this._flags & PROPERTIES_VALID))
         {
            this._x += x;
            this._y += y;
            this._z += z;
         }
         if(Boolean(this._flags & MATRIX3D_VALID))
         {
            data = this._matrix3D.rawData;
            data[12] += x;
            data[13] += y;
            data[14] += z;
            this._matrix3D.rawData = data;
         }
         this.invalidate(INVALIDATE_FROM_NONE,z != 0);
      }
      
      public function get matrix() : Matrix
      {
         if(Boolean(this._flags & MATRIX_VALID))
         {
            return this._matrix;
         }
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         var m:Matrix = this._matrix;
         if(m == null)
         {
            m = this._matrix = new Matrix();
         }
         else
         {
            m.identity();
         }
         AdvancedLayoutFeatures.build2DMatrix(m,this._transformX,this._transformY,this._scaleX,this._scaleY,this._rotationZ,this._x,this._y);
         this._flags |= MATRIX_VALID;
         return m;
      }
      
      public function set matrix(v:Matrix) : void
      {
         if(this._matrix == null)
         {
            this._matrix = v.clone();
         }
         else
         {
            this._matrix.identity();
            this._matrix.concat(v);
         }
         this.invalidate(INVALIDATE_FROM_MATRIX,true);
      }
      
      private function validatePropertiesFromMatrix() : void
      {
         var result:Vector.<Vector3D> = null;
         var postTransformTCenter:Vector3D = null;
         if(this.sourceOfTruth == SOURCE_MATRIX3D)
         {
            result = this._matrix3D.decompose();
            this._rotationX = result[1].x / RADIANS_PER_DEGREES;
            this._rotationY = result[1].y / RADIANS_PER_DEGREES;
            this._rotationZ = result[1].z / RADIANS_PER_DEGREES;
            this._scaleX = result[2].x;
            this._scaleY = result[2].y;
            this._scaleZ = result[2].z;
            if(this._transformX != 0 || this._transformY != 0 || this._transformZ != 0)
            {
               postTransformTCenter = this._matrix3D.transformVector(new Vector3D(this._transformX,this._transformY,this._transformZ));
               this._x = postTransformTCenter.x - this._transformX;
               this._y = postTransformTCenter.y - this._transformY;
               this._z = postTransformTCenter.z - this._transformZ;
            }
            else
            {
               this._x = result[0].x;
               this._y = result[0].y;
               this._z = result[0].z;
            }
         }
         else if(this.sourceOfTruth == SOURCE_MATRIX)
         {
            MatrixUtil.decomposeMatrix(decomposition,this._matrix,this._transformX,this._transformY);
            this._x = decomposition[0];
            this._y = decomposition[1];
            this._z = 0;
            this._rotationX = 0;
            this._rotationY = 0;
            this._rotationZ = decomposition[2];
            this._scaleX = decomposition[3];
            this._scaleY = decomposition[4];
            this._scaleZ = 1;
         }
         this._flags |= PROPERTIES_VALID;
      }
      
      public function get matrix3D() : Matrix3D
      {
         if(Boolean(this._flags & MATRIX3D_VALID))
         {
            return this._matrix3D;
         }
         if((this._flags & PROPERTIES_VALID) == false)
         {
            this.validatePropertiesFromMatrix();
         }
         var m:Matrix3D = this._matrix3D;
         if(m == null)
         {
            m = this._matrix3D = new Matrix3D();
         }
         else
         {
            m.identity();
         }
         AdvancedLayoutFeatures.build3DMatrix(m,this.transformX,this.transformY,this.transformZ,this._scaleX,this._scaleY,this._scaleZ,this._rotationX,this._rotationY,this._rotationZ,this._x,this._y,this._z);
         this._flags |= MATRIX3D_VALID;
         return m;
      }
      
      public function set matrix3D(v:Matrix3D) : void
      {
         if(this._matrix3D == null)
         {
            this._matrix3D = v.clone();
         }
         else
         {
            this._matrix3D.identity();
            if(Boolean(v))
            {
               this._matrix3D.append(v);
            }
         }
         this.invalidate(INVALIDATE_FROM_MATRIX3D,true);
      }
   }
}


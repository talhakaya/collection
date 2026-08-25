package mx.geom
{
   import flash.display.DisplayObject;
   import flash.display.Shape;
   import flash.geom.ColorTransform;
   import flash.geom.Matrix;
   import flash.geom.Matrix3D;
   import flash.geom.PerspectiveProjection;
   import flash.geom.Rectangle;
   import flash.geom.Transform;
   import mx.core.ILayoutElement;
   import mx.core.IVisualElement;
   import mx.core.mx_internal;
   
   use namespace mx_internal;
   
   public class Transform extends flash.geom.Transform
   {
      
      mx_internal var applyColorTransformAlpha:Boolean = false;
      
      mx_internal var applyMatrix:Boolean = false;
      
      mx_internal var applyMatrix3D:Boolean = false;
      
      private var _target:IVisualElement;
      
      public function Transform(src:DisplayObject = null)
      {
         if(src == null)
         {
            src = new Shape();
         }
         super(src);
      }
      
      override public function set colorTransform(value:ColorTransform) : void
      {
         if(Boolean(this.target) && "$transform" in this.target)
         {
            this.target["$transform"]["colorTransform"] = value;
         }
         else if(Boolean(this.target) && "setColorTransform" in this.target)
         {
            this.target["setColorTransform"](value);
         }
         else
         {
            super.colorTransform = value;
         }
         this.applyColorTransformAlpha = true;
      }
      
      override public function get colorTransform() : ColorTransform
      {
         if(Boolean(this.target) && "$transform" in this.target)
         {
            return this.target["$transform"]["colorTransform"];
         }
         if(Boolean(this.target) && Boolean("displayObject" in this.target) && this.target["displayObject"] != null)
         {
            return this.target["displayObject"]["transform"]["colorTransform"];
         }
         return super.colorTransform;
      }
      
      override public function get concatenatedColorTransform() : ColorTransform
      {
         if(Boolean(this.target) && "$transform" in this.target)
         {
            return this.target["$transform"]["concatenatedColorTransform"];
         }
         if(Boolean(this.target) && Boolean("displayObject" in this.target) && this.target["displayObject"] != null)
         {
            return this.target["displayObject"]["transform"]["concatenatedColorTransform"];
         }
         return super.concatenatedColorTransform;
      }
      
      override public function get concatenatedMatrix() : Matrix
      {
         if(Boolean(this.target) && "$transform" in this.target)
         {
            return this.target["$transform"]["concatenatedMatrix"];
         }
         if(Boolean(this.target) && Boolean("displayObject" in this.target) && this.target["displayObject"] != null)
         {
            return this.target["displayObject"]["transform"]["concatenatedMatrix"];
         }
         return super.concatenatedMatrix;
      }
      
      override public function set matrix(value:Matrix) : void
      {
         if(this.target is ILayoutElement && value != null)
         {
            ILayoutElement(this.target).setLayoutMatrix(value,true);
         }
         else
         {
            super.matrix = value;
         }
         this.applyMatrix = value != null;
         this.applyMatrix3D = false;
      }
      
      override public function get matrix() : Matrix
      {
         if(this.target is ILayoutElement)
         {
            return ILayoutElement(this.target).getLayoutMatrix();
         }
         return super.matrix;
      }
      
      override public function set matrix3D(value:Matrix3D) : *
      {
         if(this.target is ILayoutElement && value != null)
         {
            ILayoutElement(this.target).setLayoutMatrix3D(value,true);
         }
         else
         {
            super.matrix3D = value;
         }
         this.applyMatrix3D = value != null;
         this.applyMatrix = false;
      }
      
      override public function get matrix3D() : Matrix3D
      {
         if(this.target is ILayoutElement)
         {
            return ILayoutElement(this.target).getLayoutMatrix3D();
         }
         return super.matrix3D;
      }
      
      override public function set perspectiveProjection(value:PerspectiveProjection) : void
      {
         var oldValue:PerspectiveProjection = super.perspectiveProjection;
         super.perspectiveProjection = value;
      }
      
      override public function get perspectiveProjection() : PerspectiveProjection
      {
         if(Boolean(this.target) && "$transform" in this.target)
         {
            return this.target["$transform"]["perspectiveProjection"];
         }
         if(Boolean(this.target) && Boolean("displayObject" in this.target) && this.target["displayObject"] != null)
         {
            return this.target["displayObject"]["transform"]["perspectiveProjection"];
         }
         return super.perspectiveProjection;
      }
      
      override public function get pixelBounds() : Rectangle
      {
         if(Boolean(this.target) && "$transform" in this.target)
         {
            return this.target["$transform"]["pixelBounds"];
         }
         if(Boolean(this.target) && Boolean("displayObject" in this.target) && this.target["displayObject"] != null)
         {
            return this.target["displayObject"]["transform"]["pixelBounds"];
         }
         return super.pixelBounds;
      }
      
      public function set target(value:IVisualElement) : void
      {
         if(value !== this._target)
         {
            this._target = value;
         }
      }
      
      public function get target() : IVisualElement
      {
         return this._target;
      }
      
      override public function getRelativeMatrix3D(relativeTo:DisplayObject) : Matrix3D
      {
         if(Boolean(this.target) && "$transform" in this.target)
         {
            return this.target["$transform"]["getRelativeMatrix3D"](relativeTo);
         }
         if(Boolean(this.target) && Boolean("displayObject" in this.target) && this.target["displayObject"] != null)
         {
            return this.target["displayObject"]["transform"]["getRelativeMatrix3D"](relativeTo);
         }
         return super.getRelativeMatrix3D(relativeTo);
      }
   }
}


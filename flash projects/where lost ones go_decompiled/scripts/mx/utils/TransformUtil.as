package mx.utils
{
   import flash.display.DisplayObject;
   import flash.geom.Point;
   import flash.geom.Vector3D;
   import mx.core.AdvancedLayoutFeatures;
   import mx.core.mx_internal;
   
   use namespace mx_internal;
   
   [ExcludeClass]
   public final class TransformUtil
   {
      
      private static var xformPt:Point;
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      public function TransformUtil()
      {
         super();
      }
      
      private static function needAdvancedLayout(transformCenter:Vector3D, scale:Vector3D, rotation:Vector3D, translation:Vector3D, postLayoutScale:Vector3D, postLayoutRotation:Vector3D, postLayoutTranslation:Vector3D) : Boolean
      {
         return scale != null && (!isNaN(scale.x) && scale.x != 1 || !isNaN(scale.y) && scale.y != 1 || !isNaN(scale.z) && scale.z != 1) || rotation != null && (!isNaN(rotation.x) && rotation.x != 0 || !isNaN(rotation.y) && rotation.y != 0 || !isNaN(rotation.z) && rotation.z != 0) || translation != null && translation.z != 0 && !isNaN(translation.z) || postLayoutScale != null || postLayoutRotation != null || postLayoutTranslation != null && (translation == null || postLayoutTranslation.x != translation.x || postLayoutTranslation.y != translation.y || postLayoutTranslation.z != translation.z);
      }
      
      public static function transformAround(obj:DisplayObject, transformCenter:Vector3D, scale:Vector3D = null, rotation:Vector3D = null, translation:Vector3D = null, postLayoutScale:Vector3D = null, postLayoutRotation:Vector3D = null, postLayoutTranslation:Vector3D = null, layoutFeatures:AdvancedLayoutFeatures = null, initLayoutFeatures:Function = null) : void
      {
         var xformedPt:Point = null;
         var postXFormPoint:Point = null;
         var needAdvancedLayout:Boolean = Boolean(layoutFeatures) || needAdvancedLayout(transformCenter,scale,rotation,translation,postLayoutScale,postLayoutRotation,postLayoutTranslation);
         if(needAdvancedLayout)
         {
            if(!layoutFeatures && initLayoutFeatures != null)
            {
               layoutFeatures = initLayoutFeatures();
            }
            if(Boolean(layoutFeatures))
            {
               layoutFeatures.transformAround(transformCenter,scale,rotation,translation,postLayoutScale,postLayoutRotation,postLayoutTranslation);
            }
            return;
         }
         if(translation == null && transformCenter != null)
         {
            if(xformPt == null)
            {
               xformPt = new Point();
            }
            xformPt.x = transformCenter.x;
            xformPt.y = transformCenter.y;
            xformedPt = obj.transform.matrix.transformPoint(xformPt);
         }
         if(rotation != null && !isNaN(rotation.z))
         {
            obj.rotation = rotation.z;
         }
         if(scale != null)
         {
            obj.scaleX = scale.x;
            obj.scaleY = scale.y;
         }
         if(transformCenter == null)
         {
            if(translation != null)
            {
               obj.x = translation.x;
               obj.y = translation.y;
            }
         }
         else
         {
            if(xformPt == null)
            {
               xformPt = new Point();
            }
            xformPt.x = transformCenter.x;
            xformPt.y = transformCenter.y;
            postXFormPoint = obj.transform.matrix.transformPoint(xformPt);
            if(translation != null)
            {
               obj.x += translation.x - postXFormPoint.x;
               obj.y += translation.y - postXFormPoint.y;
            }
            else
            {
               obj.x += xformedPt.x - postXFormPoint.x;
               obj.y += xformedPt.y - postXFormPoint.y;
            }
         }
      }
      
      public static function transformPointToParent(obj:DisplayObject, localPosition:Vector3D, position:Vector3D, postLayoutPosition:Vector3D, layoutFeatures:AdvancedLayoutFeatures) : void
      {
         if(Boolean(layoutFeatures))
         {
            layoutFeatures.transformPointToParent(true,localPosition,position,postLayoutPosition);
            return;
         }
         if(xformPt == null)
         {
            xformPt = new Point();
         }
         if(Boolean(localPosition))
         {
            xformPt.x = localPosition.x;
            xformPt.y = localPosition.y;
         }
         else
         {
            xformPt.x = 0;
            xformPt.y = 0;
         }
         var tmp:Point = obj.transform.matrix != null ? obj.transform.matrix.transformPoint(xformPt) : xformPt;
         if(position != null)
         {
            position.x = tmp.x;
            position.y = tmp.y;
            position.z = 0;
         }
         if(postLayoutPosition != null)
         {
            postLayoutPosition.x = tmp.x;
            postLayoutPosition.y = tmp.y;
            postLayoutPosition.z = 0;
         }
      }
   }
}


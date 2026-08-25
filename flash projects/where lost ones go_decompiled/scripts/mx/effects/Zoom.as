package mx.effects
{
   import mx.core.mx_internal;
   import mx.effects.effectClasses.ZoomInstance;
   
   use namespace mx_internal;
   
   [Alternative(replacement="spark.effects.Scale",since="4.0")]
   public class Zoom extends TweenEffect
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private static var AFFECTED_PROPERTIES:Array = ["scaleX","scaleY","x","y","width","height"];
      
      [Inspectable(category="Other",defaultValue="false")]
      public var captureRollEvents:Boolean;
      
      [Inspectable(category="General",defaultValue="NaN")]
      public var originX:Number;
      
      [Inspectable(category="General",defaultValue="NaN")]
      public var originY:Number;
      
      [Inspectable(category="General",defaultValue="0.01")]
      public var zoomHeightFrom:Number;
      
      [Inspectable(category="General",defaultValue="1")]
      public var zoomHeightTo:Number;
      
      [Inspectable(category="General",defaultValue="0.01")]
      public var zoomWidthFrom:Number;
      
      [Inspectable(category="General",defaultValue="1")]
      public var zoomWidthTo:Number;
      
      public function Zoom(target:Object = null)
      {
         super(target);
         instanceClass = ZoomInstance;
         mx_internal::applyActualDimensions = false;
         relevantProperties = ["scaleX","scaleY","width","height","visible"];
      }
      
      override public function getAffectedProperties() : Array
      {
         return AFFECTED_PROPERTIES;
      }
      
      override protected function initInstance(instance:IEffectInstance) : void
      {
         var zoomInstance:ZoomInstance = null;
         super.initInstance(instance);
         zoomInstance = ZoomInstance(instance);
         zoomInstance.zoomWidthFrom = this.zoomWidthFrom;
         zoomInstance.zoomWidthTo = this.zoomWidthTo;
         zoomInstance.zoomHeightFrom = this.zoomHeightFrom;
         zoomInstance.zoomHeightTo = this.zoomHeightTo;
         zoomInstance.originX = this.originX;
         zoomInstance.originY = this.originY;
         zoomInstance.captureRollEvents = this.captureRollEvents;
      }
   }
}


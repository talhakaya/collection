package motion.actuators
{
   import motion.easing.IEasing;
   
   public interface IGenericActuator
   {
      
      function snapping(param1:Object = undefined) : IGenericActuator;
      
      function smartRotation(param1:Object = undefined) : IGenericActuator;
      
      function reverse(param1:Object = undefined) : IGenericActuator;
      
      function repeat(param1:Object = undefined) : IGenericActuator;
      
      function reflect(param1:Object = undefined) : IGenericActuator;
      
      function onUpdate(param1:*, param2:Array = undefined) : IGenericActuator;
      
      function onRepeat(param1:*, param2:Array = undefined) : IGenericActuator;
      
      function onComplete(param1:*, param2:Array = undefined) : IGenericActuator;
      
      function ease(param1:IEasing) : IGenericActuator;
      
      function delay(param1:Number) : IGenericActuator;
      
      function autoVisible(param1:Object = undefined) : IGenericActuator;
   }
}


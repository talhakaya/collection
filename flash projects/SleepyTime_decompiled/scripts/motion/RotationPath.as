package motion
{
   import flash.Boot;
   
   public class RotationPath implements IComponentPath
   {
      
      public var step:Number;
      
      public var start:Number;
      
      public var offset:Number;
      
      public var end:Number;
      
      public var _y:ComponentPath;
      
      public var _x:ComponentPath;
      
      public function RotationPath(param1:ComponentPath = undefined, param2:ComponentPath = undefined)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         step = 0.01;
         _x = param1;
         _y = param2;
         offset = 0;
         start = calculate(0);
      }
      
      public function get_end() : Number
      {
         return calculate(1);
      }
      
      public function calculate(param1:Number) : Number
      {
         var _loc2_:Number = _x.calculate(param1) - _x.calculate(param1 + step);
         var _loc3_:Number = _y.calculate(param1) - _y.calculate(param1 + step);
         var _loc4_:Number = Math.atan2(_loc3_,_loc2_) * (180 / Math.PI);
         return (_loc4_ + offset) % 360;
      }
   }
}


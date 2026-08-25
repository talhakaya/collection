package motion
{
   import flash.Boot;
   
   public class BezierPath
   {
      
      public var strength:Number;
      
      public var end:Number;
      
      public var control:Number;
      
      public function BezierPath(param1:Number = 0, param2:Number = 0, param3:Number = 0)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         end = param1;
         control = param2;
         strength = param3;
      }
      
      public function calculate(param1:Number, param2:Number) : Number
      {
         return (1 - param2) * (1 - param2) * param1 + 2 * (1 - param2) * param2 * control + param2 * param2 * end;
      }
   }
}


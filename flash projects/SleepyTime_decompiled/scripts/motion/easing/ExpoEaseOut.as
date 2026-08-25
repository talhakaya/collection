package motion.easing
{
   public class ExpoEaseOut implements IEasing
   {
      
      public function ExpoEaseOut()
      {
      }
      
      public function ease(param1:Number, param2:Number, param3:Number, param4:Number) : Number
      {
         if(param1 == param4)
         {
            return param2 + param3;
         }
         return param3 * (1 - Math.pow(2,-10 * param1 / param4)) + param2;
      }
      
      public function calculate(param1:Number) : Number
      {
         if(param1 == 1)
         {
            return 1;
         }
         return 1 - Math.pow(2,-10 * param1);
      }
   }
}


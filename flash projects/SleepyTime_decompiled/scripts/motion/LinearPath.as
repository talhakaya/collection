package motion
{
   import flash.Boot;
   
   public class LinearPath extends BezierPath
   {
      
      public function LinearPath(param1:Number = 0, param2:Number = 0)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         super(param1,0,param2);
      }
      
      override public function calculate(param1:Number, param2:Number) : Number
      {
         return param1 + param2 * (end - param1);
      }
   }
}


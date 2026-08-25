package motion.easing
{
   public class Expo
   {
      
      public function Expo()
      {
      }
      
      public static function get_easeIn() : IEasing
      {
         return new ExpoEaseIn();
      }
      
      public static function get_easeInOut() : IEasing
      {
         return new ExpoEaseInOut();
      }
      
      public static function get_easeOut() : IEasing
      {
         return new ExpoEaseOut();
      }
   }
}


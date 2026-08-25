package
{
   import flash.Boot;
   
   public class Beat
   {
      
      public static var timingMax:int = 200;
      
      public var timing:int;
      
      public var timeInSong:int;
      
      public var lengthOfPress:int;
      
      public var isHitEnd:Boolean;
      
      public var isHit:Boolean;
      
      public function Beat(param1:int = 0, param2:int = 0, param3:Boolean = false, param4:Boolean = false, param5:int = 1000)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         timeInSong = param1;
         timing = param5;
         isHit = param3;
         lengthOfPress = param2;
         isHitEnd = param4;
      }
      
      public function hitEnd() : Boolean
      {
         var _loc1_:int = 0;
         if(!isHitEnd)
         {
            _loc1_ = GameManager.time;
            timing = timeInSong + lengthOfPress - _loc1_;
            isHitEnd = Math.abs(timing) <= Beat.timingMax;
            return isHitEnd;
         }
         return false;
      }
      
      public function hit() : Boolean
      {
         var _loc1_:int = 0;
         if(!isHit)
         {
            _loc1_ = GameManager.time;
            timing = timeInSong - _loc1_;
            isHit = Math.abs(timing) <= Beat.timingMax;
            return isHit;
         }
         return false;
      }
      
      public function getRelativePositionToEnd() : int
      {
         return timeInSong + lengthOfPress - GameManager.time;
      }
      
      public function getRelativePosition() : int
      {
         return timeInSong - GameManager.time;
      }
   }
}


package motion
{
   import flash.Boot;
   
   public class ComponentPath implements IComponentPath
   {
      
      public var totalStrength:Number;
      
      public var start:Number;
      
      public var paths:Array;
      
      public var end:Number;
      
      public function ComponentPath()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         paths = [];
         start = 0;
         totalStrength = 0;
      }
      
      public function get_end() : Number
      {
         var _loc1_:* = null as BezierPath;
         if(int(paths.length) > 0)
         {
            _loc1_ = paths[int(paths.length) - 1];
            return _loc1_.end;
         }
         return start;
      }
      
      public function calculate(param1:Number) : Number
      {
         var _loc2_:Number = NaN;
         var _loc3_:Number = NaN;
         var _loc4_:int = 0;
         var _loc5_:* = null as Array;
         var _loc6_:* = null as BezierPath;
         if(int(paths.length) == 1)
         {
            return paths[0].calculate(start,param1);
         }
         _loc2_ = param1 * totalStrength;
         _loc3_ = start;
         _loc4_ = 0;
         _loc5_ = paths;
         while(_loc4_ < int(_loc5_.length))
         {
            _loc6_ = _loc5_[_loc4_];
            _loc4_++;
            if(_loc2_ <= _loc6_.strength)
            {
               return _loc6_.calculate(_loc3_,_loc2_ / _loc6_.strength);
            }
            _loc2_ -= _loc6_.strength;
            _loc3_ = Number(_loc6_.end);
         }
         return 0;
      }
      
      public function addPath(param1:BezierPath) : void
      {
         paths.push(param1);
         totalStrength += param1.strength;
      }
   }
}


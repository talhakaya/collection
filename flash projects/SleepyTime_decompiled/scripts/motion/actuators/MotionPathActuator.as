package motion.actuators
{
   import flash.Boot;
   import motion.IComponentPath;
   
   public class MotionPathActuator extends SimpleActuator
   {
      
      public function MotionPathActuator(param1:* = undefined, param2:Number = 0, param3:* = undefined)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         super(param1,param2,param3);
      }
      
      override public function update(param1:Number) : void
      {
         var _loc3_:Number = NaN;
         var _loc4_:Number = NaN;
         var _loc5_:int = 0;
         var _loc6_:* = null as Array;
         var _loc7_:* = null as PropertyDetails;
         var _loc8_:* = null;
         var _loc10_:* = null;
         if(!paused)
         {
            _loc4_ = (param1 - timeOffset) / duration;
            if(_loc4_ > 1)
            {
               _loc4_ = 1;
            }
            if(!initialized)
            {
               initialize();
            }
            if(!special)
            {
               _loc3_ = _ease.calculate(_loc4_);
               _loc5_ = 0;
               _loc6_ = propertyDetails;
               while(_loc5_ < int(_loc6_.length))
               {
                  _loc7_ = _loc6_[_loc5_];
                  _loc5_++;
                  if(_loc7_.isField)
                  {
                     _loc8_ = _loc7_.path.calculate(_loc3_);
                     _loc7_.target[_loc7_.propertyName] = _loc8_;
                  }
                  else
                  {
                     Reflect.setProperty(_loc7_.target,_loc7_.propertyName,_loc7_.path.calculate(_loc3_));
                  }
               }
            }
            else
            {
               if(!_reverse)
               {
                  _loc3_ = _ease.calculate(_loc4_);
               }
               else
               {
                  _loc3_ = _ease.calculate(1 - _loc4_);
               }
               _loc5_ = 0;
               _loc6_ = propertyDetails;
               while(_loc5_ < int(_loc6_.length))
               {
                  _loc7_ = _loc6_[_loc5_];
                  _loc5_++;
                  if(!_snapping)
                  {
                     if(_loc7_.isField)
                     {
                        _loc8_ = _loc7_.path.calculate(_loc3_);
                        _loc7_.target[_loc7_.propertyName] = _loc8_;
                     }
                     else
                     {
                        Reflect.setProperty(_loc7_.target,_loc7_.propertyName,_loc7_.path.calculate(_loc3_));
                     }
                  }
                  else if(_loc7_.isField)
                  {
                     _loc8_ = int(Math.round(_loc7_.path.calculate(_loc3_)));
                     _loc7_.target[_loc7_.propertyName] = _loc8_;
                  }
                  else
                  {
                     Reflect.setProperty(_loc7_.target,_loc7_.propertyName,int(Math.round(_loc7_.path.calculate(_loc3_))));
                  }
               }
            }
            if(_loc4_ == 1)
            {
               if(_repeat == 0)
               {
                  active = false;
                  if(toggleVisible && _loc10_ == 0)
                  {
                     _loc8_ = target;
                     if(Reflect.hasField(_loc8_,"visible"))
                     {
                        _loc8_["visible"] = false;
                     }
                     else
                     {
                        Reflect.setProperty(_loc8_,"visible",false);
                     }
                  }
                  complete(true);
                  return;
               }
               if(_reflect)
               {
                  _reverse = !_reverse;
               }
               startTime = param1;
               timeOffset = startTime + _delay;
               if(_repeat > 0)
               {
                  --_repeat;
               }
            }
            if(sendChange)
            {
               change();
            }
         }
      }
      
      override public function initialize() : void
      {
         var _loc1_:* = null as PropertyPathDetails;
         var _loc2_:* = null as IComponentPath;
         var _loc5_:* = null as String;
         var _loc6_:Boolean = false;
         var _loc3_:int = 0;
         var _loc4_:Array = Reflect.fields(properties);
         while(_loc3_ < int(_loc4_.length))
         {
            _loc5_ = _loc4_[_loc3_];
            _loc3_++;
            _loc2_ = Reflect.field(properties,_loc5_);
            if(_loc2_ != null)
            {
               _loc6_ = true;
               _loc6_ = false;
               _loc2_.start = Reflect.getProperty(target,_loc5_);
               _loc1_ = new PropertyPathDetails(target,_loc5_,_loc2_,_loc6_);
               propertyDetails.push(_loc1_);
            }
         }
         detailsLength = int(propertyDetails.length);
         initialized = true;
      }
      
      override public function apply() : void
      {
         var _loc3_:* = null as String;
         var _loc1_:int = 0;
         var _loc2_:Array = Reflect.fields(properties);
         while(_loc1_ < int(_loc2_.length))
         {
            _loc3_ = _loc2_[_loc1_];
            _loc1_++;
            Reflect.setProperty(target,_loc3_,Number(Reflect.field(properties,_loc3_).get_end()));
         }
      }
   }
}


package motion.actuators
{
   import flash.Boot;
   import flash.Lib;
   import flash.display.DisplayObject;
   import flash.events.Event;
   import flash.utils.getTimer;
   
   public class SimpleActuator extends GenericActuator
   {
      
      public static var init__:Boolean;
      
      public static var actuators:Array;
      
      public static var actuatorsLength:int = 0;
      
      public static var addedEvent:Boolean = false;
      
      public var toggleVisible:Boolean;
      
      public var timeOffset:Number;
      
      public var startTime:Number;
      
      public var setVisible:Boolean;
      
      public var sendChange:Boolean;
      
      public var propertyDetails:Array;
      
      public var paused:Boolean;
      
      public var pauseTime:Number;
      
      public var initialized:Boolean;
      
      public var detailsLength:int;
      
      public var cacheVisible:Boolean;
      
      public var active:Boolean;
      
      public function SimpleActuator(param1:* = undefined, param2:Number = 0, param3:* = undefined)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         active = true;
         propertyDetails = [];
         sendChange = false;
         paused = false;
         cacheVisible = false;
         initialized = false;
         setVisible = false;
         toggleVisible = false;
         startTime = getTimer() / 1000;
         super(param1,param2,param3);
         if(!SimpleActuator.addedEvent)
         {
            SimpleActuator.addedEvent = true;
            Lib.current.stage.addEventListener(Event.ENTER_FRAME,SimpleActuator.stage_onEnterFrame);
         }
      }
      
      public static function stage_onEnterFrame(param1:Event) : void
      {
         var _loc3_:* = null as SimpleActuator;
         var _loc8_:int = 0;
         var _loc2_:Number = getTimer() / 1000;
         var _loc4_:int = 0;
         var _loc5_:Boolean = false;
         var _loc6_:int = 0;
         var _loc7_:int = SimpleActuator.actuatorsLength;
         while(_loc6_ < _loc7_)
         {
            _loc8_ = _loc6_++;
            _loc3_ = SimpleActuator.actuators[_loc4_];
            if(_loc3_.active)
            {
               if(_loc2_ > _loc3_.timeOffset)
               {
                  _loc3_.update(_loc2_);
               }
               _loc4_++;
            }
            else
            {
               SimpleActuator.actuators.splice(_loc4_,1);
               --SimpleActuator.actuatorsLength;
            }
         }
      }
      
      public function update(param1:Number) : void
      {
         var _loc2_:* = null as PropertyDetails;
         var _loc3_:Number = NaN;
         var _loc5_:Number = NaN;
         var _loc6_:int = 0;
         var _loc7_:int = 0;
         var _loc8_:int = 0;
         var _loc9_:* = null;
         var _loc10_:Number = NaN;
         var _loc11_:Number = NaN;
         var _loc12_:* = null;
         var _loc13_:* = null as Array;
         if(!paused)
         {
            _loc5_ = (param1 - timeOffset) / duration;
            if(_loc5_ > 1)
            {
               _loc5_ = 1;
            }
            if(!initialized)
            {
               initialize();
            }
            if(!special)
            {
               _loc3_ = _ease.calculate(_loc5_);
               _loc6_ = 0;
               _loc7_ = detailsLength;
               while(_loc6_ < _loc7_)
               {
                  _loc8_ = _loc6_++;
                  _loc2_ = propertyDetails[_loc8_];
                  _loc9_ = _loc2_.start + _loc2_.change * _loc3_;
                  if(_loc2_.isField)
                  {
                     _loc2_.target[_loc2_.propertyName] = _loc9_;
                  }
                  else
                  {
                     Reflect.setProperty(_loc2_.target,_loc2_.propertyName,_loc9_);
                  }
               }
            }
            else
            {
               if(!_reverse)
               {
                  _loc3_ = _ease.calculate(_loc5_);
               }
               else
               {
                  _loc3_ = _ease.calculate(1 - _loc5_);
               }
               _loc6_ = 0;
               _loc7_ = detailsLength;
               while(_loc6_ < _loc7_)
               {
                  _loc8_ = _loc6_++;
                  _loc2_ = propertyDetails[_loc8_];
                  if(_smartRotation && (_loc2_.propertyName == "rotation" || _loc2_.propertyName == "rotationX" || _loc2_.propertyName == "rotationY" || _loc2_.propertyName == "rotationZ"))
                  {
                     _loc11_ = _loc2_.change % 360;
                     if(_loc11_ > 180)
                     {
                        _loc11_ -= 360;
                     }
                     else if(_loc11_ < -180)
                     {
                        _loc11_ += 360;
                     }
                     _loc10_ = _loc2_.start + _loc11_ * _loc3_;
                  }
                  else
                  {
                     _loc10_ = _loc2_.start + _loc2_.change * _loc3_;
                  }
                  if(!_snapping)
                  {
                     if(_loc2_.isField)
                     {
                        _loc2_.target[_loc2_.propertyName] = _loc10_;
                     }
                     else
                     {
                        Reflect.setProperty(_loc2_.target,_loc2_.propertyName,_loc10_);
                     }
                  }
                  else
                  {
                     _loc9_ = int(Math.round(_loc10_));
                     if(_loc2_.isField)
                     {
                        _loc2_.target[_loc2_.propertyName] = _loc9_;
                     }
                     else
                     {
                        Reflect.setProperty(_loc2_.target,_loc2_.propertyName,_loc9_);
                     }
                  }
               }
            }
            if(_loc5_ == 1)
            {
               if(_repeat == 0)
               {
                  active = false;
                  if(toggleVisible && _loc12_ == 0)
                  {
                     _loc9_ = target;
                     if(Reflect.hasField(_loc9_,"visible"))
                     {
                        _loc9_["visible"] = false;
                     }
                     else
                     {
                        Reflect.setProperty(_loc9_,"visible",false);
                     }
                  }
                  complete(true);
                  return;
               }
               if(_onRepeat != null)
               {
                  _loc9_ = _onRepeat;
                  _loc13_ = _onRepeatParams;
                  if(_loc13_ == null)
                  {
                     _loc13_ = [];
                  }
                  _loc9_.apply(_loc9_,_loc13_);
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
      
      override public function stop(param1:*, param2:Boolean, param3:Boolean) : void
      {
         var _loc4_:int = 0;
         var _loc5_:* = null as Array;
         var _loc6_:* = null as String;
         if(active)
         {
            if(param1 == null)
            {
               active = false;
               if(param2)
               {
                  apply();
               }
               complete(param3);
               return;
            }
            _loc4_ = 0;
            _loc5_ = Reflect.fields(param1);
            while(_loc4_ < int(_loc5_.length))
            {
               _loc6_ = _loc5_[_loc4_];
               _loc4_++;
               if(Reflect.hasField(properties,_loc6_))
               {
                  active = false;
                  if(param2)
                  {
                     apply();
                  }
                  complete(param3);
                  return;
               }
            }
         }
      }
      
      public function setProperty(param1:PropertyDetails, param2:*) : void
      {
         if(param1.isField)
         {
            param1.target[param1.propertyName] = param2;
         }
         else
         {
            Reflect.setProperty(param1.target,param1.propertyName,param2);
         }
      }
      
      public function setField(param1:*, param2:String, param3:*) : void
      {
         if(Reflect.hasField(param1,param2))
         {
            param1[param2] = param3;
         }
         else
         {
            Reflect.setProperty(param1,param2,param3);
         }
      }
      
      override public function resume() : void
      {
         if(paused)
         {
            paused = false;
            timeOffset += (getTimer() - pauseTime) / 1000;
         }
      }
      
      override public function pause() : void
      {
         paused = true;
         pauseTime = getTimer();
      }
      
      override public function onUpdate(param1:*, param2:Array = undefined) : IGenericActuator
      {
         _onUpdate = param1;
         if(param2 == null)
         {
            _onUpdateParams = [];
         }
         else
         {
            _onUpdateParams = param2;
         }
         sendChange = true;
         return this;
      }
      
      override public function move() : void
      {
         var _loc1_:* = null;
         var _loc2_:* = null;
         toggleVisible = Reflect.hasField(properties,"alpha") && target is DisplayObject;
         if(toggleVisible && properties.alpha != 0 && !_loc2_)
         {
            setVisible = true;
            _loc1_ = target;
            _loc2_ = null;
            if(Reflect.hasField(_loc1_,"visible"))
            {
               _loc2_ = _loc1_["visible"];
            }
            else
            {
               _loc2_ = Reflect.getProperty(_loc1_,"visible");
            }
            cacheVisible = _loc2_;
            _loc1_ = target;
            if(Reflect.hasField(_loc1_,"visible"))
            {
               _loc1_["visible"] = true;
            }
            else
            {
               Reflect.setProperty(_loc1_,"visible",true);
            }
         }
         timeOffset = startTime;
         SimpleActuator.actuators.push(this);
         ++SimpleActuator.actuatorsLength;
      }
      
      public function initialize() : void
      {
         var _loc1_:* = null as PropertyDetails;
         var _loc2_:Number = NaN;
         var _loc5_:* = null as String;
         var _loc6_:Boolean = false;
         var _loc7_:* = null;
         var _loc8_:* = null;
         var _loc3_:int = 0;
         var _loc4_:Array = Reflect.fields(properties);
         while(_loc3_ < int(_loc4_.length))
         {
            _loc5_ = _loc4_[_loc3_];
            _loc3_++;
            _loc6_ = true;
            if(Reflect.hasField(target,_loc5_))
            {
               _loc2_ = Reflect.field(target,_loc5_);
            }
            else
            {
               _loc6_ = false;
               _loc2_ = Reflect.getProperty(target,_loc5_);
            }
            if(Std.§is§(_loc2_,Number))
            {
               §§push(§§findproperty(PropertyDetails));
               §§push(target);
               §§push(_loc5_);
               §§push(_loc2_);
               _loc7_ = properties;
               _loc8_ = null;
               if(Reflect.hasField(_loc7_,_loc5_))
               {
                  _loc8_ = _loc7_[_loc5_];
               }
               else
               {
                  _loc8_ = Reflect.getProperty(_loc7_,_loc5_);
               }
               _loc1_ = new §§pop().PropertyDetails(§§pop(),§§pop(),§§pop(),_loc8_ - _loc2_,_loc6_);
               propertyDetails.push(_loc1_);
            }
         }
         detailsLength = int(propertyDetails.length);
         initialized = true;
      }
      
      public function getField(param1:*, param2:String) : *
      {
         var _loc3_:* = null;
         if(Reflect.hasField(param1,param2))
         {
            _loc3_ = param1[param2];
         }
         else
         {
            _loc3_ = Reflect.getProperty(param1,param2);
         }
         return _loc3_;
      }
      
      override public function delay(param1:Number) : IGenericActuator
      {
         _delay = param1;
         timeOffset = startTime + param1;
         return this;
      }
      
      override public function autoVisible(param1:Object = undefined) : IGenericActuator
      {
         var _loc2_:* = null;
         var _loc3_:* = null;
         if(param1 == null)
         {
            param1 = true;
         }
         _autoVisible = param1;
         if(!param1)
         {
            toggleVisible = false;
            if(setVisible)
            {
               _loc2_ = target;
               _loc3_ = cacheVisible;
               if(Reflect.hasField(_loc2_,"visible"))
               {
                  _loc2_["visible"] = _loc3_;
               }
               else
               {
                  Reflect.setProperty(_loc2_,"visible",_loc3_);
               }
            }
         }
         return this;
      }
   }
}


package motion.actuators
{
   import flash.Boot;
   import flash.display.DisplayObject;
   import flash.geom.ColorTransform;
   import flash.geom.Transform;
   import flash.media.SoundTransform;
   
   public class TransformActuator extends SimpleActuator
   {
      
      public var tweenSoundTransform:SoundTransform;
      
      public var tweenColorTransform:ColorTransform;
      
      public var endSoundTransform:SoundTransform;
      
      public var endColorTransform:ColorTransform;
      
      public function TransformActuator(param1:* = undefined, param2:Number = 0, param3:* = undefined)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         super(param1,param2,param3);
      }
      
      override public function update(param1:Number) : void
      {
         var _loc2_:* = null as Transform;
         var _loc3_:* = null;
         var _loc4_:* = null;
         super.update(param1);
         if(endColorTransform != null)
         {
            _loc3_ = target;
            _loc4_ = null;
            if(Reflect.hasField(_loc3_,"transform"))
            {
               _loc4_ = _loc3_["transform"];
            }
            else
            {
               _loc4_ = Reflect.getProperty(_loc3_,"transform");
            }
            _loc2_ = _loc4_;
            _loc3_ = tweenColorTransform;
            if(Reflect.hasField(_loc2_,"colorTransform"))
            {
               _loc2_["colorTransform"] = _loc3_;
            }
            else
            {
               Reflect.setProperty(_loc2_,"colorTransform",_loc3_);
            }
         }
         if(endSoundTransform != null)
         {
            _loc3_ = target;
            _loc4_ = tweenSoundTransform;
            if(Reflect.hasField(_loc3_,"soundTransform"))
            {
               _loc3_["soundTransform"] = _loc4_;
            }
            else
            {
               Reflect.setProperty(_loc3_,"soundTransform",_loc4_);
            }
         }
      }
      
      public function initializeSound() : void
      {
         var _loc1_:* = null;
         var _loc2_:* = null;
         _loc1_ = target;
         _loc2_ = null;
         if(Reflect.hasField(_loc1_,"soundTransform"))
         {
            _loc2_ = _loc1_["soundTransform"];
         }
         else
         {
            _loc2_ = Reflect.getProperty(_loc1_,"soundTransform");
         }
         if(_loc2_ == null)
         {
            _loc1_ = target;
            _loc2_ = new SoundTransform();
            if(Reflect.hasField(_loc1_,"soundTransform"))
            {
               _loc1_["soundTransform"] = _loc2_;
            }
            else
            {
               Reflect.setProperty(_loc1_,"soundTransform",_loc2_);
            }
         }
         _loc1_ = target;
         _loc2_ = null;
         if(Reflect.hasField(_loc1_,"soundTransform"))
         {
            _loc2_ = _loc1_["soundTransform"];
         }
         else
         {
            _loc2_ = Reflect.getProperty(_loc1_,"soundTransform");
         }
         var _loc3_:SoundTransform = _loc2_;
         _loc1_ = target;
         _loc2_ = null;
         if(Reflect.hasField(_loc1_,"soundTransform"))
         {
            _loc2_ = _loc1_["soundTransform"];
         }
         else
         {
            _loc2_ = Reflect.getProperty(_loc1_,"soundTransform");
         }
         endSoundTransform = _loc2_;
         tweenSoundTransform = new SoundTransform();
         if(Reflect.hasField(properties,"soundVolume"))
         {
            endSoundTransform.volume = properties.soundVolume;
            propertyDetails.push(new PropertyDetails(tweenSoundTransform,"volume",_loc3_.volume,endSoundTransform.volume - _loc3_.volume));
         }
         if(Reflect.hasField(properties,"soundPan"))
         {
            endSoundTransform.pan = properties.soundPan;
            propertyDetails.push(new PropertyDetails(tweenSoundTransform,"pan",_loc3_.pan,endSoundTransform.pan - _loc3_.pan));
         }
      }
      
      public function initializeColor() : void
      {
         var _loc3_:Number = NaN;
         var _loc4_:Number = NaN;
         var _loc6_:* = null;
         var _loc7_:* = null;
         var _loc10_:* = null as PropertyDetails;
         var _loc12_:* = null as String;
         endColorTransform = new ColorTransform();
         var _loc1_:int = properties.colorValue;
         var _loc2_:Number = properties.colorStrength;
         if(_loc2_ < 1)
         {
            if(_loc2_ < 0.5)
            {
               _loc3_ = 1;
               _loc4_ = _loc2_ * 2;
            }
            else
            {
               _loc3_ = 1 - (_loc2_ - 0.5) * 2;
               _loc4_ = 1;
            }
            endColorTransform.redMultiplier = _loc3_;
            endColorTransform.greenMultiplier = _loc3_;
            endColorTransform.blueMultiplier = _loc3_;
            endColorTransform.redOffset = _loc4_ * (_loc1_ >> 16 & 0xFF);
            endColorTransform.greenOffset = _loc4_ * (_loc1_ >> 8 & 0xFF);
            endColorTransform.blueOffset = _loc4_ * (_loc1_ & 0xFF);
         }
         else
         {
            endColorTransform.redMultiplier = 0;
            endColorTransform.greenMultiplier = 0;
            endColorTransform.blueMultiplier = 0;
            endColorTransform.redOffset = _loc1_ >> 16 & 0xFF;
            endColorTransform.greenOffset = _loc1_ >> 8 & 0xFF;
            endColorTransform.blueOffset = _loc1_ & 0xFF;
         }
         var _loc5_:Array = ["redMultiplier","greenMultiplier","blueMultiplier","redOffset","greenOffset","blueOffset"];
         if(Reflect.hasField(properties,"colorAlpha"))
         {
            endColorTransform.alphaMultiplier = properties.colorAlpha;
            _loc5_.push("alphaMultiplier");
         }
         else
         {
            _loc6_ = target;
            _loc7_ = null;
            if(Reflect.hasField(_loc6_,"alpha"))
            {
               _loc7_ = _loc6_["alpha"];
            }
            else
            {
               _loc7_ = Reflect.getProperty(_loc6_,"alpha");
            }
            endColorTransform.alphaMultiplier = _loc7_;
         }
         _loc6_ = target;
         _loc7_ = null;
         if(Reflect.hasField(_loc6_,"transform"))
         {
            _loc7_ = _loc6_["transform"];
         }
         else
         {
            _loc7_ = Reflect.getProperty(_loc6_,"transform");
         }
         var _loc8_:Transform = _loc7_;
         _loc6_ = null;
         if(Reflect.hasField(_loc8_,"colorTransform"))
         {
            _loc6_ = _loc8_["colorTransform"];
         }
         else
         {
            _loc6_ = Reflect.getProperty(_loc8_,"colorTransform");
         }
         var _loc9_:ColorTransform = _loc6_;
         tweenColorTransform = new ColorTransform();
         var _loc11_:int = 0;
         while(_loc11_ < int(_loc5_.length))
         {
            _loc12_ = _loc5_[_loc11_];
            _loc11_++;
            _loc6_ = null;
            if(Reflect.hasField(_loc9_,_loc12_))
            {
               _loc6_ = _loc9_[_loc12_];
            }
            else
            {
               _loc6_ = Reflect.getProperty(_loc9_,_loc12_);
            }
            _loc3_ = _loc6_;
            §§push(§§findproperty(PropertyDetails));
            §§push(tweenColorTransform);
            §§push(_loc12_);
            §§push(_loc3_);
            _loc6_ = endColorTransform;
            _loc7_ = null;
            if(Reflect.hasField(_loc6_,_loc12_))
            {
               _loc7_ = _loc6_[_loc12_];
            }
            else
            {
               _loc7_ = Reflect.getProperty(_loc6_,_loc12_);
            }
            _loc10_ = new §§pop().PropertyDetails(§§pop(),§§pop(),§§pop(),_loc7_ - _loc3_);
            propertyDetails.push(_loc10_);
         }
      }
      
      override public function initialize() : void
      {
         if(Reflect.hasField(properties,"colorValue") && target is DisplayObject)
         {
            initializeColor();
         }
         if(Reflect.hasField(properties,"soundVolume") || Reflect.hasField(properties,"soundPan"))
         {
            initializeSound();
         }
         detailsLength = int(propertyDetails.length);
         initialized = true;
      }
      
      override public function apply() : void
      {
         var _loc1_:* = null as Transform;
         var _loc2_:* = null;
         var _loc3_:* = null;
         initialize();
         if(endColorTransform != null)
         {
            _loc2_ = target;
            _loc3_ = null;
            if(Reflect.hasField(_loc2_,"transform"))
            {
               _loc3_ = _loc2_["transform"];
            }
            else
            {
               _loc3_ = Reflect.getProperty(_loc2_,"transform");
            }
            _loc1_ = _loc3_;
            _loc2_ = endColorTransform;
            if(Reflect.hasField(_loc1_,"colorTransform"))
            {
               _loc1_["colorTransform"] = _loc2_;
            }
            else
            {
               Reflect.setProperty(_loc1_,"colorTransform",_loc2_);
            }
         }
         if(endSoundTransform != null)
         {
            _loc2_ = target;
            _loc3_ = endSoundTransform;
            if(Reflect.hasField(_loc2_,"soundTransform"))
            {
               _loc2_["soundTransform"] = _loc3_;
            }
            else
            {
               Reflect.setProperty(_loc2_,"soundTransform",_loc3_);
            }
         }
      }
   }
}


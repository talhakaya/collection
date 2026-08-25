package mx.effects
{
   import flash.events.Event;
   import flash.events.EventDispatcher;
   import mx.core.FlexVersion;
   import mx.core.IFlexDisplayObject;
   import mx.core.mx_internal;
   import mx.effects.effectClasses.AddRemoveEffectTargetFilter;
   import mx.effects.effectClasses.HideShowEffectTargetFilter;
   import mx.effects.effectClasses.PropertyChanges;
   import mx.events.EffectEvent;
   import mx.managers.LayoutManager;
   import mx.styles.IStyleClient;
   import mx.utils.NameUtil;
   
   use namespace mx_internal;
   
   [Event(name="effectStart",type="mx.events.EffectEvent")]
   [Event(name="effectStop",type="mx.events.EffectEvent")]
   [Event(name="effectEnd",type="mx.events.EffectEvent")]
   public class Effect extends EventDispatcher implements IEffect
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      mx_internal var applyEndValuesWhenDone:Boolean = false;
      
      private var _transitionInterruption:Boolean = false;
      
      protected var applyTransitionEndProperties:Boolean = FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0 ? false : true;
      
      private var _instances:Array = [];
      
      private var _callValidateNow:Boolean = false;
      
      private var isPaused:Boolean = false;
      
      mx_internal var filterObject:EffectTargetFilter;
      
      mx_internal var applyActualDimensions:Boolean = true;
      
      mx_internal var propertyChangesArray:Array;
      
      mx_internal var playReversed:Boolean;
      
      private var effectStopped:Boolean;
      
      mx_internal var parentCompositeEffect:Effect;
      
      private var _customFilter:EffectTargetFilter;
      
      private var _duration:Number = 500;
      
      mx_internal var durationExplicitlySet:Boolean = false;
      
      private var _effectTargetHost:IEffectTargetHost;
      
      protected var endValuesCaptured:Boolean = false;
      
      private var _filter:String;
      
      private var _hideFocusRing:Boolean = false;
      
      public var instanceClass:Class = IEffectInstance;
      
      private var _perElementOffset:Number = 0;
      
      private var _relevantProperties:Array;
      
      private var _relevantStyles:Array = [];
      
      [Inspectable(category="General",defaultValue="1",minValue="0")]
      public var repeatCount:int = 1;
      
      [Inspectable(category="General",defaultValue="0",minValue="0")]
      public var repeatDelay:int = 0;
      
      [Inspectable(category="General",defaultValue="0",minValue="0")]
      public var startDelay:int = 0;
      
      public var suspendBackgroundProcessing:Boolean = false;
      
      private var _targets:Array = [];
      
      private var _triggerEvent:Event;
      
      private var _playheadTime:Number = 0;
      
      public function Effect(target:Object = null)
      {
         super();
         this.target = target;
      }
      
      private static function mergeArrays(a1:Array, a2:Array) : Array
      {
         var i2:int = 0;
         var addIt:Boolean = false;
         var i1:int = 0;
         if(Boolean(a2))
         {
            for(i2 = 0; i2 < a2.length; i2++)
            {
               addIt = true;
               for(i1 = 0; i1 < a1.length; i1++)
               {
                  if(a1[i1] == a2[i2])
                  {
                     addIt = false;
                     break;
                  }
               }
               if(addIt)
               {
                  a1.push(a2[i2]);
               }
            }
         }
         return a1;
      }
      
      private static function stripUnchangedValues(propChanges:Array) : Array
      {
         var prop:Object = null;
         for(var i:int = 0; i < propChanges.length; i++)
         {
            if(propChanges[i].stripUnchangedValues != false)
            {
               for(prop in propChanges[i].start)
               {
                  if(propChanges[i].start[prop] == propChanges[i].end[prop] || typeof propChanges[i].start[prop] == "number" && typeof propChanges[i].end[prop] == "number" && isNaN(propChanges[i].start[prop]) && isNaN(propChanges[i].end[prop]))
                  {
                     delete propChanges[i].start[prop];
                     delete propChanges[i].end[prop];
                  }
               }
            }
         }
         return propChanges;
      }
      
      mx_internal function get transitionInterruption() : Boolean
      {
         return this._transitionInterruption;
      }
      
      mx_internal function set transitionInterruption(value:Boolean) : void
      {
         this._transitionInterruption = value;
      }
      
      public function get className() : String
      {
         return NameUtil.getUnqualifiedClassName(this);
      }
      
      public function get customFilter() : EffectTargetFilter
      {
         return this._customFilter;
      }
      
      public function set customFilter(value:EffectTargetFilter) : void
      {
         this._customFilter = value;
         this.filterObject = value;
      }
      
      [Inspectable(category="General",defaultValue="500",minValue="0.0")]
      public function get duration() : Number
      {
         if(!this.durationExplicitlySet && Boolean(this.parentCompositeEffect))
         {
            return this.parentCompositeEffect.duration;
         }
         return this._duration;
      }
      
      public function set duration(value:Number) : void
      {
         this.durationExplicitlySet = true;
         this._duration = value;
      }
      
      public function get effectTargetHost() : IEffectTargetHost
      {
         return this._effectTargetHost;
      }
      
      public function set effectTargetHost(value:IEffectTargetHost) : void
      {
         this._effectTargetHost = value;
      }
      
      [Inspectable(category="General",enumeration="add,remove,show,hide,move,resize,addItem,removeItem,replacedItem,replacementItem,none",defaultValue="none")]
      public function get filter() : String
      {
         return this._filter;
      }
      
      public function set filter(value:String) : void
      {
         if(!this.customFilter)
         {
            this._filter = value;
            switch(value)
            {
               case "add":
               case "remove":
                  this.filterObject = new AddRemoveEffectTargetFilter();
                  AddRemoveEffectTargetFilter(this.filterObject).add = value == "add";
                  break;
               case "hide":
               case "show":
                  this.filterObject = new HideShowEffectTargetFilter();
                  HideShowEffectTargetFilter(this.filterObject).show = value == "show";
                  break;
               case "move":
                  this.filterObject = new EffectTargetFilter();
                  this.filterObject.filterProperties = ["x","y"];
                  break;
               case "resize":
                  this.filterObject = new EffectTargetFilter();
                  this.filterObject.filterProperties = ["width","height"];
                  break;
               case "addItem":
                  this.filterObject = new EffectTargetFilter();
                  this.filterObject.requiredSemantics = {"added":true};
                  break;
               case "removeItem":
                  this.filterObject = new EffectTargetFilter();
                  this.filterObject.requiredSemantics = {"removed":true};
                  break;
               case "replacedItem":
                  this.filterObject = new EffectTargetFilter();
                  this.filterObject.requiredSemantics = {"replaced":true};
                  break;
               case "replacementItem":
                  this.filterObject = new EffectTargetFilter();
                  this.filterObject.requiredSemantics = {"replacement":true};
                  break;
               default:
                  this.filterObject = null;
            }
         }
      }
      
      public function get hideFocusRing() : Boolean
      {
         return this._hideFocusRing;
      }
      
      public function set hideFocusRing(value:Boolean) : void
      {
         this._hideFocusRing = value;
      }
      
      public function get isPlaying() : Boolean
      {
         return Boolean(this._instances) && this._instances.length > 0;
      }
      
      [Inspectable(defaultValue="0",category="General",verbose="0",minValue="0.0")]
      public function get perElementOffset() : Number
      {
         return this._perElementOffset;
      }
      
      public function set perElementOffset(value:Number) : void
      {
         this._perElementOffset = value;
      }
      
      public function get relevantProperties() : Array
      {
         if(Boolean(this._relevantProperties))
         {
            return this._relevantProperties;
         }
         return this.getAffectedProperties();
      }
      
      public function set relevantProperties(value:Array) : void
      {
         this._relevantProperties = value;
      }
      
      public function get relevantStyles() : Array
      {
         return this._relevantStyles;
      }
      
      public function set relevantStyles(value:Array) : void
      {
         this._relevantStyles = value;
      }
      
      public function get target() : Object
      {
         if(this._targets.length > 0)
         {
            return this._targets[0];
         }
         return null;
      }
      
      public function set target(value:Object) : void
      {
         this._targets.splice(0);
         if(Boolean(value))
         {
            this._targets[0] = value;
         }
      }
      
      [ArrayElementType("Object")]
      [Inspectable(arrayType="Object")]
      public function get targets() : Array
      {
         return this._targets;
      }
      
      public function set targets(value:Array) : void
      {
         var n:int = int(value.length);
         for(var i:int = n - 1; i >= 0; i--)
         {
            if(value[i] == null)
            {
               value.splice(i,1);
            }
         }
         this._targets = value;
      }
      
      public function get triggerEvent() : Event
      {
         return this._triggerEvent;
      }
      
      public function set triggerEvent(value:Event) : void
      {
         this._triggerEvent = value;
      }
      
      [Inspectable(minValue="0.0")]
      public function get playheadTime() : Number
      {
         for(var i:int = 0; i < this._instances.length; i++)
         {
            if(Boolean(this._instances[i]))
            {
               return IEffectInstance(this._instances[i]).playheadTime;
            }
         }
         return this._playheadTime;
      }
      
      public function set playheadTime(value:Number) : void
      {
         var started:Boolean = false;
         if(this._instances.length == 0)
         {
            this.play();
            started = true;
         }
         for(var i:int = 0; i < this._instances.length; i++)
         {
            if(Boolean(this._instances[i]))
            {
               EffectInstance(this._instances[i]).playheadTime = value;
            }
         }
         if(started)
         {
            this.pause();
         }
         this._playheadTime = value;
      }
      
      public function getAffectedProperties() : Array
      {
         return [];
      }
      
      public function createInstances(targets:Array = null) : Array
      {
         var newInstance:IEffectInstance = null;
         if(!targets)
         {
            targets = this.targets;
         }
         var newInstances:Array = [];
         var n:int = int(targets.length);
         var offsetDelay:Number = 0;
         for(var i:int = 0; i < n; i++)
         {
            newInstance = this.createInstance(targets[i]);
            if(Boolean(newInstance))
            {
               newInstance.startDelay += offsetDelay;
               offsetDelay += this.perElementOffset;
               newInstances.push(newInstance);
            }
         }
         this.triggerEvent = null;
         return newInstances;
      }
      
      public function createInstance(target:Object = null) : IEffectInstance
      {
         var n:int = 0;
         var i:int = 0;
         if(!target)
         {
            target = this.target;
         }
         var newInstance:IEffectInstance = null;
         var props:PropertyChanges = null;
         var create:Boolean = true;
         var setPropsArray:Boolean = false;
         if(Boolean(this.propertyChangesArray))
         {
            setPropsArray = true;
            create = this.filterInstance(this.propertyChangesArray,target);
         }
         if(create)
         {
            newInstance = IEffectInstance(new this.instanceClass(target));
            this.initInstance(newInstance);
            if(setPropsArray)
            {
               n = int(this.propertyChangesArray.length);
               for(i = 0; i < n; i++)
               {
                  if(this.propertyChangesArray[i].target == target)
                  {
                     newInstance.propertyChanges = this.propertyChangesArray[i];
                  }
               }
            }
            EventDispatcher(newInstance).addEventListener(EffectEvent.EFFECT_START,this.effectStartHandler);
            EventDispatcher(newInstance).addEventListener(EffectEvent.EFFECT_STOP,this.effectStopHandler);
            EventDispatcher(newInstance).addEventListener(EffectEvent.EFFECT_END,this.effectEndHandler);
            this._instances.push(newInstance);
            if(Boolean(this.triggerEvent))
            {
               newInstance.initEffect(this.triggerEvent);
            }
         }
         return newInstance;
      }
      
      protected function initInstance(instance:IEffectInstance) : void
      {
         instance.duration = this.duration;
         Object(instance).durationExplicitlySet = this.durationExplicitlySet;
         instance.effect = this;
         instance.effectTargetHost = this.effectTargetHost;
         instance.hideFocusRing = this.hideFocusRing;
         instance.repeatCount = this.repeatCount;
         instance.repeatDelay = this.repeatDelay;
         instance.startDelay = this.startDelay;
         instance.suspendBackgroundProcessing = this.suspendBackgroundProcessing;
      }
      
      public function deleteInstance(instance:IEffectInstance) : void
      {
         EventDispatcher(instance).removeEventListener(EffectEvent.EFFECT_START,this.effectStartHandler);
         EventDispatcher(instance).removeEventListener(EffectEvent.EFFECT_STOP,this.effectStopHandler);
         EventDispatcher(instance).removeEventListener(EffectEvent.EFFECT_END,this.effectEndHandler);
         var n:int = int(this._instances.length);
         for(var i:int = 0; i < n; i++)
         {
            if(this._instances[i] === instance)
            {
               this._instances.splice(i,1);
            }
         }
      }
      
      public function play(targets:Array = null, playReversedFromEnd:Boolean = false) : Array
      {
         var j:int = 0;
         var tmpStart:Object = null;
         var newInstance:IEffectInstance = null;
         this.effectStopped = false;
         this.isPaused = false;
         this.playReversed = playReversedFromEnd;
         if(targets == null && this.propertyChangesArray != null)
         {
            if(this._callValidateNow)
            {
               LayoutManager.getInstance().validateNow();
            }
            if(!this.endValuesCaptured)
            {
               this.propertyChangesArray = this.captureValues(this.propertyChangesArray,false);
            }
            this.propertyChangesArray = stripUnchangedValues(this.propertyChangesArray);
            this.applyStartValues(this.propertyChangesArray,this.targets);
            if(playReversedFromEnd)
            {
               for(j = 0; j < this.propertyChangesArray.length; j++)
               {
                  tmpStart = this.propertyChangesArray[j].start;
                  this.propertyChangesArray[j].start = this.propertyChangesArray[j].end;
                  this.propertyChangesArray[j].end = tmpStart;
               }
            }
            LayoutManager.getInstance().validateNow();
            this.applyEndValuesWhenDone = true;
         }
         var newInstances:Array = this.createInstances(targets);
         var n:int = int(newInstances.length);
         for(var i:int = 0; i < n; i++)
         {
            newInstance = IEffectInstance(newInstances[i]);
            Object(newInstance).playReversed = playReversedFromEnd;
            newInstance.startEffect();
         }
         return newInstances;
      }
      
      public function pause() : void
      {
         var n:int = 0;
         var i:int = 0;
         if(this.isPlaying && !this.isPaused)
         {
            this.isPaused = true;
            n = int(this._instances.length);
            for(i = 0; i < n; i++)
            {
               IEffectInstance(this._instances[i]).pause();
            }
         }
      }
      
      public function stop() : void
      {
         var instance:IEffectInstance = null;
         var n:int = this._instances.length - 1;
         for(var i:int = n; i >= 0; i--)
         {
            instance = IEffectInstance(this._instances[i]);
            if(Boolean(instance))
            {
               instance.stop();
            }
         }
      }
      
      public function resume() : void
      {
         var n:int = 0;
         var i:int = 0;
         if(this.isPlaying && this.isPaused)
         {
            this.isPaused = false;
            n = int(this._instances.length);
            for(i = 0; i < n; i++)
            {
               IEffectInstance(this._instances[i]).resume();
            }
         }
      }
      
      public function reverse() : void
      {
         var n:int = 0;
         var i:int = 0;
         if(this.isPlaying)
         {
            n = int(this._instances.length);
            for(i = 0; i < n; i++)
            {
               IEffectInstance(this._instances[i]).reverse();
            }
         }
      }
      
      public function end(effectInstance:IEffectInstance = null) : void
      {
         var n:int = 0;
         var i:int = 0;
         var instance:IEffectInstance = null;
         if(Boolean(effectInstance))
         {
            effectInstance.end();
         }
         else
         {
            n = int(this._instances.length);
            for(i = n - 1; i >= 0; i--)
            {
               instance = IEffectInstance(this._instances[i]);
               if(Boolean(instance))
               {
                  instance.end();
               }
            }
         }
      }
      
      protected function filterInstance(propChanges:Array, target:Object) : Boolean
      {
         if(Boolean(this.filterObject))
         {
            return this.filterObject.filterInstance(propChanges,this.effectTargetHost,target);
         }
         return true;
      }
      
      public function captureStartValues() : void
      {
         if(this.targets.length > 0)
         {
            this.propertyChangesArray = this.captureValues(null,true);
            this._callValidateNow = true;
         }
         this.endValuesCaptured = false;
      }
      
      public function captureMoreStartValues(targets:Array) : void
      {
         var additionalPropertyChangesArray:Array = null;
         if(targets.length > 0)
         {
            additionalPropertyChangesArray = this.captureValues(null,true);
            this.propertyChangesArray = this.propertyChangesArray != null ? this.propertyChangesArray.concat(additionalPropertyChangesArray) : additionalPropertyChangesArray;
         }
      }
      
      public function captureEndValues() : void
      {
         this.propertyChangesArray = this.captureValues(this.propertyChangesArray,false);
         this.endValuesCaptured = true;
      }
      
      mx_internal function captureValues(propChanges:Array, setStartValues:Boolean, targetsToCapture:Array = null) : Array
      {
         var n:int = 0;
         var i:int = 0;
         var valueMap:Object = null;
         var target:Object = null;
         var m:int = 0;
         var j:int = 0;
         var value:* = undefined;
         if(!propChanges)
         {
            propChanges = [];
            n = int(this.targets.length);
            for(i = 0; i < n; i++)
            {
               propChanges.push(new PropertyChanges(this.targets[i]));
            }
         }
         var effectProps:Array = !this.filterObject ? this.relevantProperties : mergeArrays(this.relevantProperties,this.filterObject.filterProperties);
         if(Boolean(effectProps) && effectProps.length > 0)
         {
            n = int(propChanges.length);
            for(i = 0; i < n; i++)
            {
               target = propChanges[i].target;
               if(targetsToCapture == null || targetsToCapture.length == 0 || targetsToCapture.indexOf(target) >= 0)
               {
                  valueMap = setStartValues ? propChanges[i].start : propChanges[i].end;
                  m = int(effectProps.length);
                  for(j = 0; j < m; j++)
                  {
                     if(valueMap[effectProps[j]] === undefined)
                     {
                        valueMap[effectProps[j]] = this.getValueFromTarget(target,effectProps[j]);
                     }
                  }
               }
            }
         }
         var styles:Array = !this.filterObject ? this.relevantStyles : mergeArrays(this.relevantStyles,this.filterObject.filterStyles);
         if(Boolean(styles) && styles.length > 0)
         {
            n = int(propChanges.length);
            for(i = 0; i < n; i++)
            {
               target = propChanges[i].target;
               if(targetsToCapture == null || targetsToCapture.length == 0 || targetsToCapture.indexOf(target) >= 0)
               {
                  if(target is IStyleClient)
                  {
                     valueMap = setStartValues ? propChanges[i].start : propChanges[i].end;
                     m = int(styles.length);
                     for(j = 0; j < m; j++)
                     {
                        if(valueMap[styles[j]] === undefined)
                        {
                           value = target.getStyle(styles[j]);
                           valueMap[styles[j]] = value;
                        }
                     }
                  }
               }
            }
         }
         return propChanges;
      }
      
      protected function getValueFromTarget(target:Object, property:String) : *
      {
         if(property in target)
         {
            return target[property];
         }
         return undefined;
      }
      
      mx_internal function applyStartValues(propChanges:Array, targets:Array) : void
      {
         var m:int = 0;
         var j:int = 0;
         var target:Object = null;
         var apply:Boolean = false;
         var propName:String = null;
         var startVal:* = undefined;
         var endVal:* = undefined;
         var styleName:String = null;
         var startStyle:* = undefined;
         var endStyle:* = undefined;
         var effectProps:Array = this.relevantProperties;
         var n:int = int(propChanges.length);
         for(var i:int = 0; i < n; i++)
         {
            target = propChanges[i].target;
            apply = false;
            m = int(targets.length);
            for(j = 0; j < m; j++)
            {
               if(targets[j] == target)
               {
                  apply = this.filterInstance(propChanges,target);
                  break;
               }
            }
            if(apply)
            {
               m = int(effectProps.length);
               for(j = 0; j < m; j++)
               {
                  propName = effectProps[j];
                  startVal = propChanges[i].start[propName];
                  endVal = propChanges[i].end[propName];
                  if(propName in propChanges[i].start && endVal != startVal && (!(startVal is Number) || !(isNaN(endVal) && isNaN(startVal))))
                  {
                     this.applyValueToTarget(target,effectProps[j],propChanges[i].start[effectProps[j]],propChanges[i].start);
                  }
               }
               m = int(this.relevantStyles.length);
               for(j = 0; j < m; j++)
               {
                  styleName = this.relevantStyles[j];
                  startStyle = propChanges[i].start[styleName];
                  endStyle = propChanges[i].end[styleName];
                  if(styleName in propChanges[i].start && endStyle != startStyle && (!(startStyle is Number) || !(isNaN(endStyle) && isNaN(startStyle))) && target is IStyleClient)
                  {
                     if(propChanges[i].end[this.relevantStyles[j]] !== undefined)
                     {
                        target.setStyle(this.relevantStyles[j],propChanges[i].start[this.relevantStyles[j]]);
                     }
                     else
                     {
                        target.clearStyle(this.relevantStyles[j]);
                     }
                  }
               }
            }
         }
      }
      
      mx_internal function applyEndValues(propChanges:Array, targets:Array) : void
      {
         var m:int = 0;
         var j:int = 0;
         var target:Object = null;
         var apply:Boolean = false;
         var propName:String = null;
         var startVal:* = undefined;
         var endVal:* = undefined;
         var styleName:String = null;
         var startStyle:* = undefined;
         var endStyle:* = undefined;
         if(!this.applyTransitionEndProperties)
         {
            return;
         }
         var effectProps:Array = this.relevantProperties;
         var n:int = int(propChanges.length);
         for(var i:int = 0; i < n; i++)
         {
            target = propChanges[i].target;
            apply = false;
            m = int(targets.length);
            for(j = 0; j < m; j++)
            {
               if(targets[j] == target)
               {
                  apply = this.filterInstance(propChanges,target);
                  break;
               }
            }
            if(apply)
            {
               m = int(effectProps.length);
               for(j = 0; j < m; j++)
               {
                  propName = effectProps[j];
                  startVal = propChanges[i].start[propName];
                  endVal = propChanges[i].end[propName];
                  if(propName in propChanges[i].end && (!(endVal is Number) || !(isNaN(endVal) && isNaN(startVal))))
                  {
                     this.applyValueToTarget(target,propName,propChanges[i].end[propName],propChanges[i].end);
                  }
               }
               m = int(this.relevantStyles.length);
               for(j = 0; j < m; j++)
               {
                  styleName = this.relevantStyles[j];
                  startStyle = propChanges[i].start[styleName];
                  endStyle = propChanges[i].end[styleName];
                  if(styleName in propChanges[i].end && (!(endStyle is Number) || !(isNaN(endStyle) && isNaN(startStyle))) && target is IStyleClient)
                  {
                     if(propChanges[i].end[styleName] !== undefined)
                     {
                        target.setStyle(styleName,propChanges[i].end[styleName]);
                     }
                     else
                     {
                        target.clearStyle(styleName);
                     }
                  }
               }
            }
         }
      }
      
      protected function applyValueToTarget(target:Object, property:String, value:*, props:Object) : void
      {
         if(property in target)
         {
            try
            {
               if(this.applyActualDimensions && target is IFlexDisplayObject && property == "height")
               {
                  target.setActualSize(target.width,value);
               }
               else if(this.applyActualDimensions && target is IFlexDisplayObject && property == "width")
               {
                  target.setActualSize(value,target.height);
               }
               else
               {
                  target[property] = value;
               }
            }
            catch(e:Error)
            {
            }
         }
      }
      
      protected function effectStartHandler(event:EffectEvent) : void
      {
         dispatchEvent(event);
      }
      
      protected function effectStopHandler(event:EffectEvent) : void
      {
         dispatchEvent(event);
         this.effectStopped = true;
      }
      
      protected function effectEndHandler(event:EffectEvent) : void
      {
         var j:int = 0;
         var tmpStart:Object = null;
         if(this.playReversed && this.propertyChangesArray != null)
         {
            for(j = 0; j < this.propertyChangesArray.length; j++)
            {
               tmpStart = this.propertyChangesArray[j].start;
               this.propertyChangesArray[j].start = this.propertyChangesArray[j].end;
               this.propertyChangesArray[j].end = tmpStart;
            }
         }
         var lastTime:Boolean = !this._instances || this._instances.length == 1;
         if(this.applyEndValuesWhenDone && !this.effectStopped && lastTime)
         {
            this.applyEndValues(this.propertyChangesArray,this.targets);
         }
         var instance:IEffectInstance = IEffectInstance(event.effectInstance);
         this.deleteInstance(instance);
         dispatchEvent(event);
         if(lastTime)
         {
            this.propertyChangesArray = null;
            this.applyEndValuesWhenDone = false;
         }
      }
   }
}


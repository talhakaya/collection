package mx.effects
{
   import flash.events.Event;
   import flash.events.EventDispatcher;
   import flash.events.IEventDispatcher;
   import flash.events.TimerEvent;
   import flash.utils.Timer;
   import flash.utils.getTimer;
   import mx.core.UIComponent;
   import mx.core.mx_internal;
   import mx.effects.effectClasses.PropertyChanges;
   import mx.events.EffectEvent;
   import mx.events.FlexEvent;
   import mx.utils.NameUtil;
   
   use namespace mx_internal;
   
   public class EffectInstance extends EventDispatcher implements IEffectInstance
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      mx_internal var delayTimer:Timer;
      
      private var delayStartTime:Number = 0;
      
      private var delayElapsedTime:Number = 0;
      
      mx_internal var durationExplicitlySet:Boolean = false;
      
      mx_internal var hideOnEffectEnd:Boolean = false;
      
      mx_internal var parentCompositeEffectInstance:EffectInstance;
      
      protected var playCount:int = 0;
      
      mx_internal var stopRepeat:Boolean = false;
      
      private var _duration:Number = 500;
      
      private var _effect:IEffect;
      
      private var _effectTargetHost:IEffectTargetHost;
      
      private var _hideFocusRing:Boolean;
      
      private var _playReversed:Boolean;
      
      private var _propertyChanges:PropertyChanges;
      
      private var _repeatCount:int = 0;
      
      private var _repeatDelay:int = 0;
      
      private var _startDelay:int = 0;
      
      private var _suspendBackgroundProcessing:Boolean = false;
      
      private var _target:Object;
      
      private var _triggerEvent:Event;
      
      public function EffectInstance(target:Object)
      {
         super();
         this.target = target;
      }
      
      mx_internal function get actualDuration() : Number
      {
         var value:Number = NaN;
         if(this.repeatCount > 0)
         {
            value = this.duration * this.repeatCount + this.repeatDelay * (this.repeatCount - 1) + this.startDelay;
         }
         return value;
      }
      
      public function get className() : String
      {
         return NameUtil.getUnqualifiedClassName(this);
      }
      
      [Inspectable(category="General",defaultValue="500")]
      public function get duration() : Number
      {
         if(!this.durationExplicitlySet && Boolean(this.parentCompositeEffectInstance))
         {
            return this.parentCompositeEffectInstance.duration;
         }
         return this._duration;
      }
      
      public function set duration(value:Number) : void
      {
         this.durationExplicitlySet = true;
         this._duration = value;
      }
      
      public function get effect() : IEffect
      {
         return this._effect;
      }
      
      public function set effect(value:IEffect) : void
      {
         this._effect = value;
      }
      
      public function get effectTargetHost() : IEffectTargetHost
      {
         return this._effectTargetHost;
      }
      
      public function set effectTargetHost(value:IEffectTargetHost) : void
      {
         this._effectTargetHost = value;
      }
      
      public function get hideFocusRing() : Boolean
      {
         return this._hideFocusRing;
      }
      
      public function set hideFocusRing(value:Boolean) : void
      {
         this._hideFocusRing = value;
      }
      
      public function get playheadTime() : Number
      {
         return Math.max(this.playCount - 1,0) * (this.duration + this.repeatDelay) + (this.playReversed ? 0 : this.startDelay);
      }
      
      public function set playheadTime(value:Number) : void
      {
         if(Boolean(this.delayTimer) && this.delayTimer.running)
         {
            this.delayTimer.reset();
            if(value < this.startDelay)
            {
               this.delayTimer = new Timer(this.startDelay - value,1);
               this.delayStartTime = getTimer();
               this.delayTimer.addEventListener(TimerEvent.TIMER,this.delayTimerHandler);
               this.delayTimer.start();
            }
            else
            {
               this.playCount = 0;
               this.play();
            }
         }
      }
      
      mx_internal function get playReversed() : Boolean
      {
         return this._playReversed;
      }
      
      mx_internal function set playReversed(value:Boolean) : void
      {
         this._playReversed = value;
      }
      
      public function get propertyChanges() : PropertyChanges
      {
         return this._propertyChanges;
      }
      
      public function set propertyChanges(value:PropertyChanges) : void
      {
         this._propertyChanges = value;
      }
      
      public function get repeatCount() : int
      {
         return this._repeatCount;
      }
      
      public function set repeatCount(value:int) : void
      {
         this._repeatCount = value;
      }
      
      public function get repeatDelay() : int
      {
         return this._repeatDelay;
      }
      
      public function set repeatDelay(value:int) : void
      {
         this._repeatDelay = value;
      }
      
      public function get startDelay() : int
      {
         return this._startDelay;
      }
      
      public function set startDelay(value:int) : void
      {
         this._startDelay = value;
      }
      
      public function get suspendBackgroundProcessing() : Boolean
      {
         return this._suspendBackgroundProcessing;
      }
      
      public function set suspendBackgroundProcessing(value:Boolean) : void
      {
         this._suspendBackgroundProcessing = value;
      }
      
      public function get target() : Object
      {
         return this._target;
      }
      
      public function set target(value:Object) : void
      {
         this._target = value;
      }
      
      public function get triggerEvent() : Event
      {
         return this._triggerEvent;
      }
      
      public function set triggerEvent(value:Event) : void
      {
         this._triggerEvent = value;
      }
      
      public function initEffect(event:Event) : void
      {
         this.triggerEvent = event;
         switch(event.type)
         {
            case "resizeStart":
            case "resizeEnd":
               if(!this.durationExplicitlySet)
               {
                  this.duration = 250;
               }
               break;
            case FlexEvent.HIDE:
               this.target.setVisible(true,true);
               this.hideOnEffectEnd = true;
               this.target.addEventListener(FlexEvent.SHOW,this.eventHandler);
         }
      }
      
      public function startEffect() : void
      {
         EffectManager.effectStarted(this);
         if(this.target is UIComponent)
         {
            UIComponent(this.target).effectStarted(this);
         }
         if(this.startDelay > 0 && !this.playReversed)
         {
            this.delayTimer = new Timer(this.startDelay,1);
            this.delayStartTime = getTimer();
            this.delayTimer.addEventListener(TimerEvent.TIMER,this.delayTimerHandler);
            this.delayTimer.start();
         }
         else
         {
            this.play();
         }
      }
      
      public function play() : void
      {
         ++this.playCount;
         dispatchEvent(new EffectEvent(EffectEvent.EFFECT_START,false,false,this));
         if(Boolean(this.target) && this.target is IEventDispatcher)
         {
            this.target.dispatchEvent(new EffectEvent(EffectEvent.EFFECT_START,false,false,this));
         }
      }
      
      public function pause() : void
      {
         if(Boolean(this.delayTimer) && Boolean(this.delayTimer.running) && !isNaN(this.delayStartTime))
         {
            this.delayTimer.stop();
            this.delayElapsedTime = getTimer() - this.delayStartTime;
         }
      }
      
      public function stop() : void
      {
         if(Boolean(this.delayTimer))
         {
            this.delayTimer.reset();
         }
         this.stopRepeat = true;
         dispatchEvent(new EffectEvent(EffectEvent.EFFECT_STOP,false,false,this));
         if(Boolean(this.target) && this.target is IEventDispatcher)
         {
            this.target.dispatchEvent(new EffectEvent(EffectEvent.EFFECT_STOP,false,false,this));
         }
         this.finishEffect();
      }
      
      public function resume() : void
      {
         if(Boolean(this.delayTimer) && Boolean(!this.delayTimer.running) && !isNaN(this.delayElapsedTime))
         {
            this.delayTimer.delay = !this.playReversed ? this.delayTimer.delay - this.delayElapsedTime : this.delayElapsedTime;
            this.delayStartTime = getTimer();
            this.delayTimer.start();
         }
      }
      
      public function reverse() : void
      {
         if(this.repeatCount > 0)
         {
            this.playCount = this.repeatCount - this.playCount + 1;
         }
      }
      
      public function end() : void
      {
         if(Boolean(this.delayTimer))
         {
            this.delayTimer.reset();
         }
         this.stopRepeat = true;
         this.finishEffect();
      }
      
      public function finishEffect() : void
      {
         this.playCount = 0;
         dispatchEvent(new EffectEvent(EffectEvent.EFFECT_END,false,false,this));
         if(Boolean(this.target) && this.target is IEventDispatcher)
         {
            this.target.dispatchEvent(new EffectEvent(EffectEvent.EFFECT_END,false,false,this));
         }
         if(this.target is UIComponent)
         {
            UIComponent(this.target).effectFinished(this);
         }
         EffectManager.effectFinished(this);
      }
      
      public function finishRepeat() : void
      {
         if(!this.stopRepeat && this.playCount != 0 && (this.playCount < this.repeatCount || this.repeatCount == 0))
         {
            if(this.repeatDelay > 0)
            {
               this.delayTimer = new Timer(this.repeatDelay,1);
               this.delayStartTime = getTimer();
               this.delayTimer.addEventListener(TimerEvent.TIMER,this.delayTimerHandler);
               this.delayTimer.start();
            }
            else
            {
               this.play();
            }
         }
         else
         {
            this.finishEffect();
         }
      }
      
      mx_internal function playWithNoDuration() : void
      {
         this.duration = 0;
         this.repeatCount = 1;
         this.repeatDelay = 0;
         this.startDelay = 0;
         this.startEffect();
      }
      
      mx_internal function eventHandler(event:Event) : void
      {
         if(event.type == FlexEvent.SHOW && this.hideOnEffectEnd == true)
         {
            this.hideOnEffectEnd = false;
            event.target.removeEventListener(FlexEvent.SHOW,this.eventHandler);
         }
      }
      
      private function delayTimerHandler(event:TimerEvent) : void
      {
         this.delayTimer.reset();
         this.delayStartTime = NaN;
         this.delayElapsedTime = NaN;
         this.play();
      }
   }
}


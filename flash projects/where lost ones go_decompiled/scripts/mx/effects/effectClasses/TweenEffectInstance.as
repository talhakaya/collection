package mx.effects.effectClasses
{
   import mx.core.UIComponentGlobals;
   import mx.core.mx_internal;
   import mx.effects.EffectInstance;
   import mx.effects.Tween;
   import mx.events.TweenEvent;
   
   use namespace mx_internal;
   
   public class TweenEffectInstance extends EffectInstance
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      mx_internal var needToLayout:Boolean = false;
      
      private var _seekTime:Number = 0;
      
      public var easingFunction:Function;
      
      public var tween:Tween;
      
      public function TweenEffectInstance(target:Object)
      {
         super(target);
      }
      
      override mx_internal function set playReversed(value:Boolean) : void
      {
         super.mx_internal::playReversed = value;
         if(Boolean(this.tween))
         {
            this.tween.playReversed = value;
         }
      }
      
      override public function get playheadTime() : Number
      {
         if(Boolean(this.tween))
         {
            return this.tween.playheadTime + super.playheadTime;
         }
         return 0;
      }
      
      override public function set playheadTime(value:Number) : void
      {
         if(Boolean(this.tween))
         {
            this.tween.seek(value);
         }
         else
         {
            this._seekTime = value;
         }
      }
      
      override public function pause() : void
      {
         super.pause();
         if(Boolean(this.tween))
         {
            this.tween.pause();
         }
      }
      
      override public function stop() : void
      {
         super.stop();
         if(Boolean(this.tween))
         {
            this.tween.stop();
         }
      }
      
      override public function resume() : void
      {
         super.resume();
         if(Boolean(this.tween))
         {
            this.tween.resume();
         }
      }
      
      override public function reverse() : void
      {
         super.reverse();
         if(Boolean(this.tween))
         {
            this.tween.reverse();
         }
         super.mx_internal::playReversed = !playReversed;
      }
      
      override public function end() : void
      {
         stopRepeat = true;
         if(Boolean(delayTimer))
         {
            delayTimer.reset();
         }
         if(Boolean(this.tween))
         {
            this.tween.endTween();
            this.tween = null;
         }
      }
      
      protected function createTween(listener:Object, startValue:Object, endValue:Object, duration:Number = -1, minFps:Number = -1) : Tween
      {
         var newTween:Tween = new Tween(listener,startValue,endValue,duration,minFps);
         newTween.addEventListener(TweenEvent.TWEEN_START,this.tweenEventHandler);
         newTween.addEventListener(TweenEvent.TWEEN_UPDATE,this.tweenEventHandler);
         newTween.addEventListener(TweenEvent.TWEEN_END,this.tweenEventHandler);
         if(this.easingFunction != null)
         {
            newTween.easingFunction = this.easingFunction;
         }
         if(this._seekTime > 0)
         {
            newTween.seek(this._seekTime);
         }
         newTween.playReversed = playReversed;
         return newTween;
      }
      
      mx_internal function applyTweenStartValues() : void
      {
         if(duration > 0)
         {
            this.onTweenUpdate(this.tween.getCurrentValue(0));
         }
      }
      
      private function tweenEventHandler(event:TweenEvent) : void
      {
         dispatchEvent(event);
      }
      
      public function onTweenUpdate(value:Object) : void
      {
      }
      
      public function onTweenEnd(value:Object) : void
      {
         this.onTweenUpdate(value);
         this.tween = null;
         if(this.needToLayout)
         {
            UIComponentGlobals.layoutManager.validateNow();
         }
         finishRepeat();
      }
   }
}


package mx.effects
{
   import flash.events.EventDispatcher;
   import flash.events.TimerEvent;
   import flash.utils.Timer;
   import flash.utils.getTimer;
   import mx.core.UIComponentGlobals;
   import mx.core.mx_internal;
   import mx.events.TweenEvent;
   
   use namespace mx_internal;
   
   [Alternative(replacement="spark.effects.animation.Animation",since="4.0")]
   public class Tween extends EventDispatcher
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      mx_internal static var activeTweens:Array = [];
      
      private static var interval:Number = 10;
      
      private static var timer:Timer = null;
      
      mx_internal static var intervalTime:Number = NaN;
      
      mx_internal var needToLayout:Boolean = false;
      
      private var id:int;
      
      private var maxDelay:Number = 87.5;
      
      private var arrayMode:Boolean;
      
      private var _doSeek:Boolean = false;
      
      private var _isPlaying:Boolean = true;
      
      private var _doReverse:Boolean = false;
      
      mx_internal var startTime:Number;
      
      private var previousUpdateTime:Number;
      
      private var userEquation:Function;
      
      private var updateFunction:Function;
      
      private var endFunction:Function;
      
      private var endValue:Object;
      
      private var startValue:Object;
      
      private var started:Boolean = false;
      
      public var duration:Number = 3000;
      
      public var listener:Object;
      
      private var _playheadTime:Number = 0;
      
      private var _invertValues:Boolean = false;
      
      public function Tween(listener:Object, startValue:Object, endValue:Object, duration:Number = -1, minFps:Number = -1, updateFunction:Function = null, endFunction:Function = null)
      {
         this.userEquation = this.defaultEasingFunction;
         super();
         if(!listener)
         {
            return;
         }
         if(startValue is Array)
         {
            this.arrayMode = true;
         }
         this.listener = listener;
         this.startValue = startValue;
         this.endValue = endValue;
         if(!isNaN(duration) && duration != -1)
         {
            this.duration = duration;
         }
         if(!isNaN(minFps) && minFps != -1)
         {
            this.maxDelay = 1000 / minFps;
         }
         this.updateFunction = updateFunction;
         this.endFunction = endFunction;
         if(duration == 0)
         {
            this.id = -1;
            this.endTween();
         }
         else
         {
            Tween.addTween(this);
         }
      }
      
      private static function addTween(tween:Tween) : void
      {
         tween.id = activeTweens.length;
         activeTweens.push(tween);
         if(!timer)
         {
            timer = new Timer(interval);
            timer.addEventListener(TimerEvent.TIMER,timerHandler);
            timer.start();
         }
         else
         {
            timer.start();
         }
         if(isNaN(intervalTime))
         {
            intervalTime = getTimer();
         }
         tween.startTime = tween.previousUpdateTime = intervalTime;
      }
      
      private static function removeTweenAt(index:int) : void
      {
         var curTween:Tween = null;
         if(index >= activeTweens.length || index < 0)
         {
            return;
         }
         activeTweens.splice(index,1);
         var n:int = int(activeTweens.length);
         for(var i:int = index; i < n; i++)
         {
            curTween = Tween(activeTweens[i]);
            --curTween.id;
         }
         if(n == 0)
         {
            intervalTime = NaN;
            timer.reset();
         }
      }
      
      mx_internal static function removeTween(tween:Tween) : void
      {
         removeTweenAt(tween.id);
      }
      
      private static function timerHandler(event:TimerEvent) : void
      {
         var tween:Tween = null;
         var needToLayout:Boolean = false;
         var oldTime:Number = intervalTime;
         intervalTime = getTimer();
         var n:int = int(activeTweens.length);
         for(var i:int = n; i >= 0; i--)
         {
            tween = Tween(activeTweens[i]);
            if(Boolean(tween))
            {
               tween.needToLayout = false;
               tween.doInterval();
               if(tween.needToLayout)
               {
                  needToLayout = true;
               }
            }
         }
         if(needToLayout)
         {
            UIComponentGlobals.layoutManager.validateNow();
         }
         event.updateAfterEvent();
      }
      
      mx_internal function get playheadTime() : Number
      {
         return this._playheadTime;
      }
      
      mx_internal function get playReversed() : Boolean
      {
         return this._invertValues;
      }
      
      mx_internal function set playReversed(value:Boolean) : void
      {
         this._invertValues = value;
      }
      
      public function setTweenHandlers(updateFunction:Function, endFunction:Function) : void
      {
         this.updateFunction = updateFunction;
         this.endFunction = endFunction;
      }
      
      public function set easingFunction(value:Function) : void
      {
         this.userEquation = value;
      }
      
      public function endTween() : void
      {
         var event:TweenEvent = new TweenEvent(TweenEvent.TWEEN_END);
         var value:Object = this.getCurrentValue(this.duration);
         event.value = value;
         dispatchEvent(event);
         if(this.endFunction != null)
         {
            this.endFunction(value);
         }
         else
         {
            this.listener.onTweenEnd(value);
         }
         if(this.id >= 0)
         {
            Tween.removeTweenAt(this.id);
         }
      }
      
      mx_internal function doInterval() : Boolean
      {
         var currentTime:Number = NaN;
         var currentValue:Object = null;
         var event:TweenEvent = null;
         var startEvent:TweenEvent = null;
         var tweenEnded:Boolean = false;
         this.previousUpdateTime = intervalTime;
         if(this._isPlaying || this._doSeek)
         {
            currentTime = intervalTime - this.startTime;
            this._playheadTime = currentTime;
            currentValue = this.getCurrentValue(currentTime);
            if(currentTime >= this.duration && !this._doSeek)
            {
               this.endTween();
               tweenEnded = true;
            }
            else
            {
               if(!this.started)
               {
                  startEvent = new TweenEvent(TweenEvent.TWEEN_START);
                  dispatchEvent(startEvent);
                  this.started = true;
               }
               event = new TweenEvent(TweenEvent.TWEEN_UPDATE);
               event.value = currentValue;
               dispatchEvent(event);
               if(this.updateFunction != null)
               {
                  this.updateFunction(currentValue);
               }
               else
               {
                  this.listener.onTweenUpdate(currentValue);
               }
            }
            this._doSeek = false;
         }
         return tweenEnded;
      }
      
      mx_internal function getCurrentValue(currentTime:Number) : Object
      {
         var returnArray:Array = null;
         var n:int = 0;
         var i:int = 0;
         if(this.duration == 0)
         {
            return this.endValue;
         }
         if(this._invertValues)
         {
            currentTime = this.duration - currentTime;
         }
         if(this.arrayMode)
         {
            returnArray = [];
            n = int(this.startValue.length);
            for(i = 0; i < n; i++)
            {
               returnArray[i] = this.userEquation(currentTime,this.startValue[i],this.endValue[i] - this.startValue[i],this.duration);
            }
            return returnArray;
         }
         return this.userEquation(currentTime,this.startValue,Number(this.endValue) - Number(this.startValue),this.duration);
      }
      
      private function defaultEasingFunction(t:Number, b:Number, c:Number, d:Number) : Number
      {
         return c / 2 * (Math.sin(Math.PI * (t / d - 0.5)) + 1) + b;
      }
      
      public function seek(playheadTime:Number) : void
      {
         var clockTime:Number = intervalTime;
         this.previousUpdateTime = clockTime;
         this.startTime = clockTime - playheadTime;
         this._doSeek = true;
         this.doInterval();
      }
      
      public function reverse() : void
      {
         if(this._isPlaying)
         {
            this._doReverse = false;
            this.seek(this.duration - this._playheadTime);
            this._invertValues = !this._invertValues;
         }
         else
         {
            this._doReverse = !this._doReverse;
         }
      }
      
      public function pause() : void
      {
         this._isPlaying = false;
      }
      
      public function stop() : void
      {
         if(this.id >= 0)
         {
            Tween.removeTweenAt(this.id);
         }
      }
      
      public function resume() : void
      {
         this._isPlaying = true;
         this.startTime = intervalTime - this._playheadTime;
         if(this._doReverse)
         {
            this.reverse();
            this._doReverse = false;
         }
      }
   }
}


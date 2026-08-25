package mx.effects.effectClasses
{
   import flash.events.Event;
   import flash.events.MouseEvent;
   import mx.core.mx_internal;
   import mx.effects.EffectManager;
   import mx.events.FlexEvent;
   
   use namespace mx_internal;
   
   public class ZoomInstance extends TweenEffectInstance
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private var origScaleX:Number;
      
      private var origScaleY:Number;
      
      private var origX:Number;
      
      private var origY:Number;
      
      private var newX:Number;
      
      private var newY:Number;
      
      private var scaledOriginX:Number;
      
      private var scaledOriginY:Number;
      
      private var origPercentWidth:Number;
      
      private var origPercentHeight:Number;
      
      private var _mouseHasMoved:Boolean = false;
      
      private var show:Boolean = true;
      
      public var captureRollEvents:Boolean;
      
      public var originX:Number;
      
      public var originY:Number;
      
      public var zoomHeightFrom:Number;
      
      public var zoomHeightTo:Number;
      
      public var zoomWidthFrom:Number;
      
      public var zoomWidthTo:Number;
      
      public function ZoomInstance(target:Object)
      {
         super(target);
      }
      
      override public function initEffect(event:Event) : void
      {
         super.initEffect(event);
         if(event.type == FlexEvent.HIDE || event.type == Event.REMOVED)
         {
            this.show = false;
         }
      }
      
      override public function play() : void
      {
         super.play();
         this.applyPropertyChanges();
         if(isNaN(this.zoomWidthFrom) && isNaN(this.zoomWidthTo) && isNaN(this.zoomHeightFrom) && isNaN(this.zoomHeightTo))
         {
            if(this.show)
            {
               this.zoomWidthFrom = this.zoomHeightFrom = 0;
               this.zoomWidthTo = target.scaleX;
               this.zoomHeightTo = target.scaleY;
            }
            else
            {
               this.zoomWidthFrom = target.scaleX;
               this.zoomHeightFrom = target.scaleY;
               this.zoomWidthTo = this.zoomHeightTo = 0;
            }
         }
         else
         {
            if(isNaN(this.zoomWidthFrom) && isNaN(this.zoomWidthTo))
            {
               this.zoomWidthFrom = this.zoomWidthTo = target.scaleX;
            }
            else if(isNaN(this.zoomHeightFrom) && isNaN(this.zoomHeightTo))
            {
               this.zoomHeightFrom = this.zoomHeightTo = target.scaleY;
            }
            if(isNaN(this.zoomWidthFrom))
            {
               this.zoomWidthFrom = target.scaleX;
            }
            else if(isNaN(this.zoomWidthTo))
            {
               this.zoomWidthTo = this.zoomWidthFrom == 1 ? 0 : 1;
            }
            if(isNaN(this.zoomHeightFrom))
            {
               this.zoomHeightFrom = target.scaleY;
            }
            else if(isNaN(this.zoomHeightTo))
            {
               this.zoomHeightTo = this.zoomHeightFrom == 1 ? 0 : 1;
            }
         }
         if(this.zoomWidthFrom < 0.01)
         {
            this.zoomWidthFrom = 0.01;
         }
         if(this.zoomWidthTo < 0.01)
         {
            this.zoomWidthTo = 0.01;
         }
         if(this.zoomHeightFrom < 0.01)
         {
            this.zoomHeightFrom = 0.01;
         }
         if(this.zoomHeightTo < 0.01)
         {
            this.zoomHeightTo = 0.01;
         }
         this.origScaleX = target.scaleX;
         this.origScaleY = target.scaleY;
         this.newX = this.origX = target.x;
         this.newY = this.origY = target.y;
         if(isNaN(this.originX))
         {
            this.scaledOriginX = target.width / 2;
         }
         else
         {
            this.scaledOriginX = this.originX * this.origScaleX;
         }
         if(isNaN(this.originY))
         {
            this.scaledOriginY = target.height / 2;
         }
         else
         {
            this.scaledOriginY = this.originY * this.origScaleY;
         }
         this.scaledOriginX = Number(this.scaledOriginX.toFixed(1));
         this.scaledOriginY = Number(this.scaledOriginY.toFixed(1));
         this.origPercentWidth = target.percentWidth;
         if(!isNaN(this.origPercentWidth))
         {
            target.width = target.width;
         }
         this.origPercentHeight = target.percentHeight;
         if(!isNaN(this.origPercentHeight))
         {
            target.height = target.height;
         }
         tween = createTween(this,[this.zoomWidthFrom,this.zoomHeightFrom],[this.zoomWidthTo,this.zoomHeightTo],duration);
         if(this.captureRollEvents)
         {
            target.addEventListener(MouseEvent.ROLL_OVER,this.mouseEventHandler,false);
            target.addEventListener(MouseEvent.ROLL_OUT,this.mouseEventHandler,false);
            target.addEventListener(MouseEvent.MOUSE_MOVE,this.mouseEventHandler,false);
         }
      }
      
      override public function onTweenUpdate(value:Object) : void
      {
         EffectManager.suspendEventHandling();
         if(Math.abs(this.newX - target.x) > 0.1)
         {
            this.origX += Number(target.x.toFixed(1)) - this.newX;
         }
         if(Math.abs(this.newY - target.y) > 0.1)
         {
            this.origY += Number(target.y.toFixed(1)) - this.newY;
         }
         target.scaleX = value[0];
         target.scaleY = value[1];
         var ratioX:Number = value[0] / this.origScaleX;
         var ratioY:Number = value[1] / this.origScaleY;
         var newOriginX:Number = this.scaledOriginX * ratioX;
         var newOriginY:Number = this.scaledOriginY * ratioY;
         this.newX = this.scaledOriginX - newOriginX + this.origX;
         this.newY = this.scaledOriginY - newOriginY + this.origY;
         this.newX = Number(this.newX.toFixed(1));
         this.newY = Number(this.newY.toFixed(1));
         target.move(this.newX,this.newY);
         if(Boolean(tween))
         {
            tween.needToLayout = true;
         }
         else
         {
            needToLayout = true;
         }
         EffectManager.resumeEventHandling();
      }
      
      override public function onTweenEnd(value:Object) : void
      {
         var curWidth:Number = NaN;
         var curHeight:Number = NaN;
         if(!isNaN(this.origPercentWidth))
         {
            curWidth = Number(target.width);
            target.percentWidth = this.origPercentWidth;
            if(Boolean(target.parent) && target.parent.autoLayout == false)
            {
               target._width = curWidth;
            }
         }
         if(!isNaN(this.origPercentHeight))
         {
            curHeight = Number(target.height);
            target.percentHeight = this.origPercentHeight;
            if(Boolean(target.parent) && target.parent.autoLayout == false)
            {
               target._height = curHeight;
            }
         }
         super.onTweenEnd(value);
         if(hideOnEffectEnd)
         {
            EffectManager.suspendEventHandling();
            target.scaleX = this.origScaleX;
            target.scaleY = this.origScaleY;
            target.move(this.origX,this.origY);
            EffectManager.resumeEventHandling();
         }
      }
      
      override public function finishEffect() : void
      {
         if(this.captureRollEvents)
         {
            target.removeEventListener(MouseEvent.ROLL_OVER,this.mouseEventHandler,false);
            target.removeEventListener(MouseEvent.ROLL_OUT,this.mouseEventHandler,false);
            target.removeEventListener(MouseEvent.MOUSE_MOVE,this.mouseEventHandler,false);
         }
         super.finishEffect();
      }
      
      private function applyPropertyChanges() : void
      {
         var useSize:Boolean = false;
         var useScale:Boolean = false;
         var values:PropertyChanges = propertyChanges;
         if(Boolean(values))
         {
            useSize = false;
            useScale = false;
            if(values.end["scaleX"] !== undefined)
            {
               this.zoomWidthFrom = isNaN(this.zoomWidthFrom) ? Number(target.scaleX) : this.zoomWidthFrom;
               this.zoomWidthTo = isNaN(this.zoomWidthTo) ? Number(values.end["scaleX"]) : this.zoomWidthTo;
               useScale = true;
            }
            if(values.end["scaleY"] !== undefined)
            {
               this.zoomHeightFrom = isNaN(this.zoomHeightFrom) ? Number(target.scaleY) : this.zoomHeightFrom;
               this.zoomHeightTo = isNaN(this.zoomHeightTo) ? Number(values.end["scaleY"]) : this.zoomHeightTo;
               useScale = true;
            }
            if(useScale)
            {
               return;
            }
            if(values.end["width"] !== undefined)
            {
               this.zoomWidthFrom = isNaN(this.zoomWidthFrom) ? this.getScaleFromWidth(target.width) : this.zoomWidthFrom;
               this.zoomWidthTo = isNaN(this.zoomWidthTo) ? this.getScaleFromWidth(values.end["width"]) : this.zoomWidthTo;
               useSize = true;
            }
            if(values.end["height"] !== undefined)
            {
               this.zoomHeightFrom = isNaN(this.zoomHeightFrom) ? this.getScaleFromHeight(target.height) : this.zoomHeightFrom;
               this.zoomHeightTo = isNaN(this.zoomHeightTo) ? this.getScaleFromHeight(values.end["height"]) : this.zoomHeightTo;
               useSize = true;
            }
            if(useSize)
            {
               return;
            }
            if(values.end["visible"] !== undefined)
            {
               this.show = values.end["visible"];
            }
         }
      }
      
      private function getScaleFromWidth(value:Number) : Number
      {
         return value / (target.width / Math.abs(target.scaleX));
      }
      
      private function getScaleFromHeight(value:Number) : Number
      {
         return value / (target.height / Math.abs(target.scaleY));
      }
      
      private function mouseEventHandler(event:MouseEvent) : void
      {
         if(event.type == MouseEvent.MOUSE_MOVE)
         {
            this._mouseHasMoved = true;
         }
         else if(event.type == MouseEvent.ROLL_OUT || event.type == MouseEvent.ROLL_OVER)
         {
            if(!this._mouseHasMoved)
            {
               event.stopImmediatePropagation();
            }
            this._mouseHasMoved = false;
         }
      }
   }
}


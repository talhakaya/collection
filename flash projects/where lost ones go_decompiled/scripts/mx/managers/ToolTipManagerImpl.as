package mx.managers
{
   import flash.display.DisplayObject;
   import flash.events.Event;
   import flash.events.EventDispatcher;
   import flash.events.MouseEvent;
   import flash.events.TimerEvent;
   import flash.geom.Point;
   import flash.geom.Rectangle;
   import flash.utils.Timer;
   import mx.controls.ToolTip;
   import mx.core.FlexGlobals;
   import mx.core.FlexVersion;
   import mx.core.IFlexModule;
   import mx.core.IInvalidating;
   import mx.core.ILayoutDirectionElement;
   import mx.core.IToolTip;
   import mx.core.IUIComponent;
   import mx.core.LayoutDirection;
   import mx.core.mx_internal;
   import mx.effects.EffectManager;
   import mx.effects.IAbstractEffect;
   import mx.events.DynamicEvent;
   import mx.events.EffectEvent;
   import mx.events.ToolTipEvent;
   import mx.styles.IStyleClient;
   import mx.validators.IValidatorListener;
   
   use namespace mx_internal;
   
   [ExcludeClass]
   public class ToolTipManagerImpl extends EventDispatcher implements IToolTipManager2
   {
      
      private static var instance:IToolTipManager2;
      
      public static var mixins:Array;
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      mx_internal var initialized:Boolean = false;
      
      mx_internal var showTimer:Timer;
      
      mx_internal var hideTimer:Timer;
      
      mx_internal var scrubTimer:Timer;
      
      mx_internal var currentText:String;
      
      mx_internal var isError:Boolean;
      
      mx_internal var previousTarget:DisplayObject;
      
      private var _currentTarget:DisplayObject;
      
      mx_internal var _currentToolTip:DisplayObject;
      
      private var _enabled:Boolean = true;
      
      private var _hideDelay:Number = 10000;
      
      private var _hideEffect:IAbstractEffect;
      
      private var _scrubDelay:Number = 100;
      
      private var _showDelay:Number = 500;
      
      private var _showEffect:IAbstractEffect;
      
      private var _toolTipClass:Class;
      
      public function ToolTipManagerImpl()
      {
         var n:int = 0;
         var i:int = 0;
         this._toolTipClass = ToolTip;
         super();
         if(Boolean(instance))
         {
            throw new Error("Instance already exists.");
         }
         if(Boolean(mixins))
         {
            n = int(mixins.length);
            for(i = 0; i < n; i++)
            {
               new mixins[i](this);
            }
         }
         if(hasEventListener("initialize"))
         {
            dispatchEvent(new Event("initialize"));
         }
      }
      
      public static function getInstance() : IToolTipManager2
      {
         if(!instance)
         {
            instance = new ToolTipManagerImpl();
         }
         return instance;
      }
      
      public function get currentTarget() : DisplayObject
      {
         return this._currentTarget;
      }
      
      public function set currentTarget(value:DisplayObject) : void
      {
         this._currentTarget = value;
      }
      
      public function get currentToolTip() : IToolTip
      {
         return this._currentToolTip as IToolTip;
      }
      
      public function set currentToolTip(value:IToolTip) : void
      {
         this._currentToolTip = value as DisplayObject;
         if(hasEventListener("currentToolTip"))
         {
            dispatchEvent(new Event("currentToolTip"));
         }
      }
      
      public function get enabled() : Boolean
      {
         return this._enabled;
      }
      
      public function set enabled(value:Boolean) : void
      {
         this._enabled = value;
      }
      
      public function get hideDelay() : Number
      {
         return this._hideDelay;
      }
      
      public function set hideDelay(value:Number) : void
      {
         this._hideDelay = value;
      }
      
      public function get hideEffect() : IAbstractEffect
      {
         return this._hideEffect;
      }
      
      public function set hideEffect(value:IAbstractEffect) : void
      {
         this._hideEffect = value as IAbstractEffect;
      }
      
      public function get scrubDelay() : Number
      {
         return this._scrubDelay;
      }
      
      public function set scrubDelay(value:Number) : void
      {
         this._scrubDelay = value;
      }
      
      public function get showDelay() : Number
      {
         return this._showDelay;
      }
      
      public function set showDelay(value:Number) : void
      {
         this._showDelay = value;
      }
      
      public function get showEffect() : IAbstractEffect
      {
         return this._showEffect;
      }
      
      public function set showEffect(value:IAbstractEffect) : void
      {
         this._showEffect = value as IAbstractEffect;
      }
      
      public function get toolTipClass() : Class
      {
         return this._toolTipClass;
      }
      
      public function set toolTipClass(value:Class) : void
      {
         this._toolTipClass = value;
      }
      
      mx_internal function initialize() : void
      {
         if(!this.showTimer)
         {
            this.showTimer = new Timer(0,1);
            this.showTimer.addEventListener(TimerEvent.TIMER,this.showTimer_timerHandler);
         }
         if(!this.hideTimer)
         {
            this.hideTimer = new Timer(0,1);
            this.hideTimer.addEventListener(TimerEvent.TIMER,this.hideTimer_timerHandler);
         }
         if(!this.scrubTimer)
         {
            this.scrubTimer = new Timer(0,1);
         }
         this.initialized = true;
      }
      
      public function registerToolTip(target:DisplayObject, oldToolTip:String, newToolTip:String) : void
      {
         if(!oldToolTip && Boolean(newToolTip))
         {
            target.addEventListener(MouseEvent.MOUSE_OVER,this.toolTipMouseOverHandler);
            target.addEventListener(MouseEvent.MOUSE_OUT,this.toolTipMouseOutHandler);
            if(this.mouseIsOver(target))
            {
               this.showImmediately(target);
            }
         }
         else if(Boolean(oldToolTip) && !newToolTip)
         {
            target.removeEventListener(MouseEvent.MOUSE_OVER,this.toolTipMouseOverHandler);
            target.removeEventListener(MouseEvent.MOUSE_OUT,this.toolTipMouseOutHandler);
            if(this.mouseIsOver(target))
            {
               this.hideImmediately(target);
            }
         }
      }
      
      public function registerErrorString(target:DisplayObject, oldErrorString:String, newErrorString:String) : void
      {
         if(!oldErrorString && Boolean(newErrorString))
         {
            target.addEventListener(MouseEvent.MOUSE_OVER,this.errorTipMouseOverHandler);
            target.addEventListener(MouseEvent.MOUSE_OUT,this.errorTipMouseOutHandler);
            if(this.mouseIsOver(target))
            {
               this.showImmediately(target);
            }
         }
         else if(Boolean(oldErrorString) && !newErrorString)
         {
            target.removeEventListener(MouseEvent.MOUSE_OVER,this.errorTipMouseOverHandler);
            target.removeEventListener(MouseEvent.MOUSE_OUT,this.errorTipMouseOutHandler);
            if(this.mouseIsOver(target))
            {
               this.hideImmediately(target);
            }
         }
      }
      
      private function mouseIsOver(target:DisplayObject) : Boolean
      {
         if(!target || !target.stage)
         {
            return false;
         }
         if(target.stage.mouseX == 0 && target.stage.mouseY == 0)
         {
            return false;
         }
         if(target is ILayoutManagerClient && !ILayoutManagerClient(target).initialized)
         {
            return false;
         }
         return target.hitTestPoint(target.stage.mouseX,target.stage.mouseY,true);
      }
      
      private function showImmediately(target:DisplayObject) : void
      {
         var oldShowDelay:Number = ToolTipManager.showDelay;
         ToolTipManager.showDelay = 0;
         this.checkIfTargetChanged(target);
         ToolTipManager.showDelay = oldShowDelay;
      }
      
      private function hideImmediately(target:DisplayObject) : void
      {
         this.checkIfTargetChanged(null);
      }
      
      mx_internal function checkIfTargetChanged(displayObject:DisplayObject) : void
      {
         if(!this.enabled)
         {
            return;
         }
         this.findTarget(displayObject);
         if(this.currentTarget != this.previousTarget)
         {
            this.targetChanged();
            this.previousTarget = this.currentTarget;
         }
      }
      
      mx_internal function findTarget(displayObject:DisplayObject) : void
      {
         var showErrorTip:Boolean = false;
         while(Boolean(displayObject))
         {
            if(displayObject is IValidatorListener)
            {
               this.currentText = IValidatorListener(displayObject).errorString;
               if(displayObject is IStyleClient)
               {
                  showErrorTip = FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0 || Boolean(IStyleClient(displayObject).getStyle("showErrorTip"));
               }
               if(this.currentText != null && this.currentText != "" && showErrorTip)
               {
                  this.currentTarget = displayObject;
                  this.isError = true;
                  return;
               }
            }
            if(displayObject is IToolTipManagerClient)
            {
               this.currentText = IToolTipManagerClient(displayObject).toolTip;
               if(this.currentText != null)
               {
                  this.currentTarget = displayObject;
                  this.isError = false;
                  return;
               }
            }
            displayObject = displayObject.parent;
         }
         this.currentText = null;
         this.currentTarget = null;
      }
      
      mx_internal function targetChanged() : void
      {
         var event:ToolTipEvent = null;
         if(!this.initialized)
         {
            this.initialize();
         }
         if(Boolean(this.previousTarget) && Boolean(this.currentToolTip))
         {
            if(this.currentToolTip is IToolTip)
            {
               event = new ToolTipEvent(ToolTipEvent.TOOL_TIP_HIDE);
               event.toolTip = this.currentToolTip;
               this.previousTarget.dispatchEvent(event);
            }
            else if(hasEventListener(ToolTipEvent.TOOL_TIP_HIDE))
            {
               dispatchEvent(new Event(ToolTipEvent.TOOL_TIP_HIDE));
            }
         }
         this.reset();
         if(Boolean(this.currentTarget))
         {
            if(this.currentText == "")
            {
               return;
            }
            event = new ToolTipEvent(ToolTipEvent.TOOL_TIP_START);
            this.currentTarget.dispatchEvent(event);
            if(this.showDelay == 0 || this.scrubTimer.running)
            {
               this.createTip();
               this.initializeTip();
               this.positionTip();
               this.showTip();
            }
            else
            {
               this.showTimer.delay = this.showDelay;
               this.showTimer.start();
            }
         }
      }
      
      mx_internal function createTip() : void
      {
         var event:ToolTipEvent = new ToolTipEvent(ToolTipEvent.TOOL_TIP_CREATE);
         this.currentTarget.dispatchEvent(event);
         if(Boolean(event.toolTip))
         {
            this.currentToolTip = event.toolTip;
         }
         else
         {
            this.currentToolTip = new this.toolTipClass();
         }
         this.currentToolTip.visible = false;
         if(this.currentToolTip is IFlexModule && IFlexModule(this.currentToolTip).moduleFactory == null && this.currentTarget is IFlexModule)
         {
            IFlexModule(this.currentToolTip).moduleFactory = IFlexModule(this.currentTarget).moduleFactory;
         }
         if(hasEventListener("createTip"))
         {
            if(!dispatchEvent(new Event("createTip",false,true)))
            {
               return;
            }
         }
         var sm:ISystemManager = this.getSystemManager(this.currentTarget) as ISystemManager;
         sm.topLevelSystemManager.toolTipChildren.addChild(this.currentToolTip as DisplayObject);
      }
      
      mx_internal function initializeTip() : void
      {
         if(this.currentToolTip is IToolTip)
         {
            IToolTip(this.currentToolTip).text = this.currentText;
         }
         if(this.isError && this.currentToolTip is IStyleClient)
         {
            IStyleClient(this.currentToolTip).setStyle("styleName","errorTip");
         }
         this.sizeTip(this.currentToolTip);
         if(this.currentToolTip is IStyleClient)
         {
            if(Boolean(this.showEffect))
            {
               IStyleClient(this.currentToolTip).setStyle("showEffect",this.showEffect);
            }
            if(Boolean(this.hideEffect))
            {
               IStyleClient(this.currentToolTip).setStyle("hideEffect",this.hideEffect);
            }
         }
         if(Boolean(this.showEffect) || Boolean(this.hideEffect))
         {
            this.currentToolTip.addEventListener(EffectEvent.EFFECT_END,this.effectEndHandler);
         }
      }
      
      public function sizeTip(toolTip:IToolTip) : void
      {
         if(toolTip is IInvalidating)
         {
            IInvalidating(toolTip).validateNow();
         }
         toolTip.setActualSize(toolTip.getExplicitOrMeasuredWidth(),toolTip.getExplicitOrMeasuredHeight());
      }
      
      mx_internal function positionTip() : void
      {
         var layoutDirection:String = null;
         var x:Number = NaN;
         var y:Number = NaN;
         var tipElt:ILayoutDirectionElement = null;
         var targetGlobalBounds:Rectangle = null;
         var noRoom:Boolean = false;
         var newWidth:Number = NaN;
         var oldWidth:Number = NaN;
         var sm:ISystemManager = null;
         var toolTipWidth:Number = NaN;
         var toolTipHeight:Number = NaN;
         var pos:Point = null;
         if(this.currentTarget is ILayoutDirectionElement)
         {
            layoutDirection = ILayoutDirectionElement(this.currentTarget).layoutDirection;
         }
         else
         {
            layoutDirection = LayoutDirection.LTR;
         }
         var mirror:Boolean = layoutDirection == LayoutDirection.RTL;
         var screenWidth:Number = this.currentToolTip.screen.width;
         var screenHeight:Number = this.currentToolTip.screen.height;
         if(this.isError)
         {
            tipElt = this.currentToolTip as ILayoutDirectionElement;
            if(Boolean(tipElt) && tipElt.layoutDirection != layoutDirection)
            {
               tipElt.layoutDirection = layoutDirection;
               tipElt.invalidateLayoutDirection();
            }
            targetGlobalBounds = this.getGlobalBounds(this.currentTarget,this.currentToolTip.root,mirror);
            x = mirror ? targetGlobalBounds.left - 4 : targetGlobalBounds.right + 4;
            y = targetGlobalBounds.top - 1;
            noRoom = mirror ? x < this.currentToolTip.width : x + this.currentToolTip.width > screenWidth;
            if(noRoom)
            {
               newWidth = NaN;
               oldWidth = NaN;
               x = mirror ? targetGlobalBounds.right + 2 - this.currentToolTip.width : targetGlobalBounds.left - 2;
               if(mirror)
               {
                  if(x < this.currentToolTip.width + 4)
                  {
                     x = 4;
                     newWidth = targetGlobalBounds.right - 2;
                  }
               }
               else if(x + this.currentToolTip.width + 4 > screenWidth)
               {
                  newWidth = screenWidth - x - 4;
               }
               if(!isNaN(newWidth))
               {
                  oldWidth = Number(Object(this.toolTipClass).maxWidth);
                  Object(this.toolTipClass).maxWidth = newWidth;
                  if(this.currentToolTip is IStyleClient)
                  {
                     IStyleClient(this.currentToolTip).setStyle("borderStyle","errorTipAbove");
                  }
                  this.currentToolTip["text"] = this.currentToolTip["text"];
               }
               else
               {
                  if(this.currentToolTip is IStyleClient)
                  {
                     IStyleClient(this.currentToolTip).setStyle("borderStyle","errorTipAbove");
                  }
                  this.currentToolTip["text"] = this.currentToolTip["text"];
               }
               if(this.currentToolTip.height + 2 < targetGlobalBounds.top)
               {
                  y = targetGlobalBounds.top - (this.currentToolTip.height + 2);
               }
               else
               {
                  y = targetGlobalBounds.bottom + 2;
                  if(!isNaN(newWidth))
                  {
                     Object(this.toolTipClass).maxWidth = newWidth;
                  }
                  if(this.currentToolTip is IStyleClient)
                  {
                     IStyleClient(this.currentToolTip).setStyle("borderStyle","errorTipBelow");
                  }
                  this.currentToolTip["text"] = this.currentToolTip["text"];
               }
            }
            this.sizeTip(this.currentToolTip);
            if(!isNaN(oldWidth))
            {
               Object(this.toolTipClass).maxWidth = oldWidth;
            }
            else if(mirror)
            {
               x = targetGlobalBounds.right + 2 - this.currentToolTip.width;
            }
         }
         else
         {
            sm = this.getSystemManager(this.currentTarget);
            x = DisplayObject(sm).mouseX + 11;
            if(mirror)
            {
               x -= this.currentToolTip.width;
            }
            y = DisplayObject(sm).mouseY + 22;
            toolTipWidth = Number(this.currentToolTip.width);
            if(mirror)
            {
               if(x < 2)
               {
                  x = 2;
               }
            }
            else if(x + toolTipWidth > screenWidth)
            {
               x = screenWidth - toolTipWidth;
            }
            toolTipHeight = Number(this.currentToolTip.height);
            if(y + toolTipHeight > screenHeight)
            {
               y = screenHeight - toolTipHeight;
            }
            pos = new Point(x,y);
            pos = DisplayObject(sm).localToGlobal(pos);
            pos = DisplayObject(sm.getSandboxRoot()).globalToLocal(pos);
            x = pos.x;
            y = pos.y;
         }
         this.currentToolTip.move(x,y);
      }
      
      mx_internal function showTip() : void
      {
         var sm:ISystemManager = null;
         var event:ToolTipEvent = new ToolTipEvent(ToolTipEvent.TOOL_TIP_SHOW);
         event.toolTip = this.currentToolTip;
         this.currentTarget.dispatchEvent(event);
         if(this.isError)
         {
            this.currentTarget.addEventListener(Event.CHANGE,this.changeHandler);
         }
         else
         {
            sm = this.getSystemManager(this.currentTarget);
            sm.addEventListener(MouseEvent.MOUSE_DOWN,this.systemManager_mouseDownHandler);
         }
         this.currentToolTip.visible = true;
         if(!this.showEffect)
         {
            this.showEffectEnded();
         }
      }
      
      mx_internal function hideTip() : void
      {
         var event:ToolTipEvent = null;
         var sm:ISystemManager = null;
         if(Boolean(this.previousTarget))
         {
            event = new ToolTipEvent(ToolTipEvent.TOOL_TIP_HIDE);
            event.toolTip = this.currentToolTip;
            this.previousTarget.dispatchEvent(event);
         }
         if(Boolean(this.currentToolTip))
         {
            this.currentToolTip.visible = false;
         }
         if(this.isError)
         {
            if(Boolean(this.currentTarget))
            {
               this.currentTarget.removeEventListener(Event.CHANGE,this.changeHandler);
            }
         }
         else if(Boolean(this.previousTarget))
         {
            sm = this.getSystemManager(this.previousTarget);
            sm.removeEventListener(MouseEvent.MOUSE_DOWN,this.systemManager_mouseDownHandler);
         }
         if(!this.hideEffect)
         {
            this.hideEffectEnded();
         }
      }
      
      mx_internal function reset() : void
      {
         var e:DynamicEvent = null;
         var sm:ISystemManager = null;
         this.showTimer.reset();
         this.hideTimer.reset();
         if(Boolean(this.currentToolTip))
         {
            if(Boolean(this.showEffect) || Boolean(this.hideEffect))
            {
               this.currentToolTip.removeEventListener(EffectEvent.EFFECT_END,this.effectEndHandler);
            }
            EffectManager.endEffectsForTarget(this.currentToolTip);
            if(hasEventListener("removeChild"))
            {
               e = new DynamicEvent("removeChild",false,true);
               e.sm = this.currentToolTip.systemManager;
               e.toolTip = this.currentToolTip;
            }
            if(!e || dispatchEvent(e))
            {
               sm = this.currentToolTip.systemManager as ISystemManager;
               sm.topLevelSystemManager.toolTipChildren.removeChild(this.currentToolTip as DisplayObject);
            }
            this.currentToolTip = null;
            this.scrubTimer.delay = this.scrubDelay;
            this.scrubTimer.reset();
            if(this.scrubDelay > 0)
            {
               this.scrubTimer.delay = this.scrubDelay;
               this.scrubTimer.start();
            }
         }
      }
      
      public function createToolTip(text:String, x:Number, y:Number, errorTipBorderStyle:String = null, context:IUIComponent = null) : IToolTip
      {
         var e:DynamicEvent = null;
         var toolTip:ToolTip = new ToolTip();
         var sm:ISystemManager = Boolean(context) ? context.systemManager as ISystemManager : FlexGlobals.topLevelApplication.systemManager as ISystemManager;
         if(context is IFlexModule)
         {
            toolTip.moduleFactory = IFlexModule(context).moduleFactory;
         }
         else
         {
            toolTip.moduleFactory = sm;
         }
         if(hasEventListener("addChild"))
         {
            e = new DynamicEvent("addChild",false,true);
            e.sm = sm;
            e.toolTip = toolTip;
         }
         if(!e || dispatchEvent(e))
         {
            sm.topLevelSystemManager.toolTipChildren.addChild(toolTip as DisplayObject);
         }
         if(Boolean(errorTipBorderStyle))
         {
            toolTip.setStyle("styleName","errorTip");
            toolTip.setStyle("borderStyle",errorTipBorderStyle);
         }
         toolTip.text = text;
         this.sizeTip(toolTip);
         toolTip.move(x,y);
         return toolTip as IToolTip;
      }
      
      public function destroyToolTip(toolTip:IToolTip) : void
      {
         var e:DynamicEvent = null;
         var sm:ISystemManager = null;
         if(hasEventListener("removeChild"))
         {
            e = new DynamicEvent("removeChild",false,true);
            e.sm = toolTip.systemManager;
            e.toolTip = toolTip;
         }
         if(!e || dispatchEvent(e))
         {
            sm = toolTip.systemManager as ISystemManager;
            sm.topLevelSystemManager.toolTipChildren.removeChild(toolTip as DisplayObject);
         }
      }
      
      mx_internal function showEffectEnded() : void
      {
         var event:ToolTipEvent = null;
         if(this.hideDelay == 0)
         {
            this.hideTip();
         }
         else if(this.hideDelay < Infinity)
         {
            this.hideTimer.delay = this.hideDelay;
            this.hideTimer.start();
         }
         if(Boolean(this.currentTarget))
         {
            event = new ToolTipEvent(ToolTipEvent.TOOL_TIP_SHOWN);
            event.toolTip = this.currentToolTip;
            this.currentTarget.dispatchEvent(event);
         }
      }
      
      mx_internal function hideEffectEnded() : void
      {
         var event:ToolTipEvent = null;
         this.reset();
         if(Boolean(this.previousTarget))
         {
            event = new ToolTipEvent(ToolTipEvent.TOOL_TIP_END);
            event.toolTip = this.currentToolTip;
            this.previousTarget.dispatchEvent(event);
         }
      }
      
      mx_internal function getSystemManager(target:DisplayObject) : ISystemManager
      {
         return target is IUIComponent ? IUIComponent(target).systemManager : null;
      }
      
      private function getGlobalBounds(obj:DisplayObject, parent:DisplayObject, mirror:Boolean) : Rectangle
      {
         var upperLeft:Point = new Point(0,0);
         upperLeft = obj.localToGlobal(upperLeft);
         if(mirror)
         {
            upperLeft.x -= obj.width;
         }
         upperLeft = parent.globalToLocal(upperLeft);
         return new Rectangle(upperLeft.x,upperLeft.y,obj.width,obj.height);
      }
      
      mx_internal function toolTipMouseOverHandler(event:MouseEvent) : void
      {
         this.checkIfTargetChanged(DisplayObject(event.target));
      }
      
      mx_internal function toolTipMouseOutHandler(event:MouseEvent) : void
      {
         this.checkIfTargetChanged(event.relatedObject);
      }
      
      mx_internal function errorTipMouseOverHandler(event:MouseEvent) : void
      {
         this.checkIfTargetChanged(DisplayObject(event.target));
      }
      
      mx_internal function errorTipMouseOutHandler(event:MouseEvent) : void
      {
         this.checkIfTargetChanged(event.relatedObject);
      }
      
      mx_internal function showTimer_timerHandler(event:TimerEvent) : void
      {
         if(Boolean(this.currentTarget))
         {
            this.createTip();
            this.initializeTip();
            this.positionTip();
            this.showTip();
         }
      }
      
      mx_internal function hideTimer_timerHandler(event:TimerEvent) : void
      {
         this.hideTip();
      }
      
      mx_internal function effectEndHandler(event:EffectEvent) : void
      {
         if(event.effectInstance.effect == this.showEffect)
         {
            this.showEffectEnded();
         }
         else if(event.effectInstance.effect == this.hideEffect)
         {
            this.hideEffectEnded();
         }
      }
      
      mx_internal function systemManager_mouseDownHandler(event:MouseEvent) : void
      {
         this.reset();
      }
      
      mx_internal function changeHandler(event:Event) : void
      {
         this.reset();
      }
   }
}


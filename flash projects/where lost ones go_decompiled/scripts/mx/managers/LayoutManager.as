package mx.managers
{
   import flash.display.Stage;
   import flash.events.Event;
   import flash.events.EventDispatcher;
   import mx.core.UIComponentGlobals;
   import mx.core.mx_internal;
   import mx.events.DynamicEvent;
   import mx.events.FlexEvent;
   import mx.managers.layoutClasses.PriorityQueue;
   
   use namespace mx_internal;
   
   public class LayoutManager extends EventDispatcher implements ILayoutManager
   {
      
      private static var instance:LayoutManager;
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private var updateCompleteQueue:PriorityQueue = new PriorityQueue();
      
      private var invalidatePropertiesQueue:PriorityQueue = new PriorityQueue();
      
      private var invalidatePropertiesFlag:Boolean = false;
      
      private var invalidateClientPropertiesFlag:Boolean = false;
      
      private var invalidateSizeQueue:PriorityQueue = new PriorityQueue();
      
      private var invalidateSizeFlag:Boolean = false;
      
      private var invalidateClientSizeFlag:Boolean = false;
      
      private var invalidateDisplayListQueue:PriorityQueue = new PriorityQueue();
      
      private var invalidateDisplayListFlag:Boolean = false;
      
      private var waitedAFrame:Boolean = false;
      
      private var listenersAttached:Boolean = false;
      
      private var originalFrameRate:Number;
      
      private var targetLevel:int = 2147483647;
      
      private var systemManager:ISystemManager;
      
      private var currentObject:ILayoutManagerClient;
      
      private var _usePhasedInstantiation:Boolean = false;
      
      private var _usingBridge:int = -1;
      
      public function LayoutManager()
      {
         super();
         this.systemManager = SystemManagerGlobals.topLevelSystemManagers[0];
      }
      
      public static function getInstance() : LayoutManager
      {
         if(!instance)
         {
            instance = new LayoutManager();
         }
         return instance;
      }
      
      public function get usePhasedInstantiation() : Boolean
      {
         return this._usePhasedInstantiation;
      }
      
      public function set usePhasedInstantiation(value:Boolean) : void
      {
         var stage:Stage = null;
         if(this._usePhasedInstantiation != value)
         {
            this._usePhasedInstantiation = value;
            try
            {
               stage = this.systemManager.stage;
               if(Boolean(stage))
               {
                  if(value)
                  {
                     this.originalFrameRate = stage.frameRate;
                     stage.frameRate = 1000;
                  }
                  else
                  {
                     stage.frameRate = this.originalFrameRate;
                  }
               }
            }
            catch(e:SecurityError)
            {
            }
         }
      }
      
      public function invalidateProperties(obj:ILayoutManagerClient) : void
      {
         if(!this.invalidatePropertiesFlag && Boolean(this.systemManager))
         {
            this.invalidatePropertiesFlag = true;
            if(!this.listenersAttached)
            {
               this.attachListeners(this.systemManager);
            }
         }
         if(this.targetLevel <= obj.nestLevel)
         {
            this.invalidateClientPropertiesFlag = true;
         }
         this.invalidatePropertiesQueue.addObject(obj,obj.nestLevel);
      }
      
      public function invalidateSize(obj:ILayoutManagerClient) : void
      {
         if(!this.invalidateSizeFlag && Boolean(this.systemManager))
         {
            this.invalidateSizeFlag = true;
            if(!this.listenersAttached)
            {
               this.attachListeners(this.systemManager);
            }
         }
         if(this.targetLevel <= obj.nestLevel)
         {
            this.invalidateClientSizeFlag = true;
         }
         this.invalidateSizeQueue.addObject(obj,obj.nestLevel);
      }
      
      public function invalidateDisplayList(obj:ILayoutManagerClient) : void
      {
         if(!this.invalidateDisplayListFlag && Boolean(this.systemManager))
         {
            this.invalidateDisplayListFlag = true;
            if(!this.listenersAttached)
            {
               this.attachListeners(this.systemManager);
            }
         }
         else if(!this.invalidateDisplayListFlag && !this.systemManager)
         {
         }
         this.invalidateDisplayListQueue.addObject(obj,obj.nestLevel);
      }
      
      private function validateProperties() : void
      {
         var obj:ILayoutManagerClient = ILayoutManagerClient(this.invalidatePropertiesQueue.removeSmallest());
         while(Boolean(obj))
         {
            if(Boolean(obj.nestLevel))
            {
               this.currentObject = obj;
               obj.validateProperties();
               if(!obj.updateCompletePendingFlag)
               {
                  this.updateCompleteQueue.addObject(obj,obj.nestLevel);
                  obj.updateCompletePendingFlag = true;
               }
            }
            obj = ILayoutManagerClient(this.invalidatePropertiesQueue.removeSmallest());
         }
         if(this.invalidatePropertiesQueue.isEmpty())
         {
            this.invalidatePropertiesFlag = false;
         }
      }
      
      private function validateSize() : void
      {
         var obj:ILayoutManagerClient = ILayoutManagerClient(this.invalidateSizeQueue.removeLargest());
         while(Boolean(obj))
         {
            if(Boolean(obj.nestLevel))
            {
               this.currentObject = obj;
               obj.validateSize();
               if(!obj.updateCompletePendingFlag)
               {
                  this.updateCompleteQueue.addObject(obj,obj.nestLevel);
                  obj.updateCompletePendingFlag = true;
               }
            }
            obj = ILayoutManagerClient(this.invalidateSizeQueue.removeLargest());
         }
         if(this.invalidateSizeQueue.isEmpty())
         {
            this.invalidateSizeFlag = false;
         }
      }
      
      private function validateDisplayList() : void
      {
         var obj:ILayoutManagerClient = ILayoutManagerClient(this.invalidateDisplayListQueue.removeSmallest());
         while(Boolean(obj))
         {
            if(Boolean(obj.nestLevel))
            {
               this.currentObject = obj;
               obj.validateDisplayList();
               if(!obj.updateCompletePendingFlag)
               {
                  this.updateCompleteQueue.addObject(obj,obj.nestLevel);
                  obj.updateCompletePendingFlag = true;
               }
            }
            obj = ILayoutManagerClient(this.invalidateDisplayListQueue.removeSmallest());
         }
         if(this.invalidateDisplayListQueue.isEmpty())
         {
            this.invalidateDisplayListFlag = false;
         }
      }
      
      private function doPhasedInstantiation() : void
      {
         var obj:ILayoutManagerClient = null;
         if(this.usePhasedInstantiation)
         {
            if(this.invalidatePropertiesFlag)
            {
               this.validateProperties();
               this.systemManager.document.dispatchEvent(new Event("validatePropertiesComplete"));
            }
            else if(this.invalidateSizeFlag)
            {
               this.validateSize();
               this.systemManager.document.dispatchEvent(new Event("validateSizeComplete"));
            }
            else if(this.invalidateDisplayListFlag)
            {
               this.validateDisplayList();
               this.systemManager.document.dispatchEvent(new Event("validateDisplayListComplete"));
            }
         }
         else
         {
            if(this.invalidatePropertiesFlag)
            {
               this.validateProperties();
            }
            if(this.invalidateSizeFlag)
            {
               this.validateSize();
            }
            if(this.invalidateDisplayListFlag)
            {
               this.validateDisplayList();
            }
         }
         if(this.invalidatePropertiesFlag || this.invalidateSizeFlag || this.invalidateDisplayListFlag)
         {
            this.attachListeners(this.systemManager);
         }
         else
         {
            this.usePhasedInstantiation = false;
            this.listenersAttached = false;
            obj = ILayoutManagerClient(this.updateCompleteQueue.removeLargest());
            while(Boolean(obj))
            {
               if(!obj.initialized && obj.processedDescriptors)
               {
                  obj.initialized = true;
               }
               if(obj.hasEventListener(FlexEvent.UPDATE_COMPLETE))
               {
                  obj.dispatchEvent(new FlexEvent(FlexEvent.UPDATE_COMPLETE));
               }
               obj.updateCompletePendingFlag = false;
               obj = ILayoutManagerClient(this.updateCompleteQueue.removeLargest());
            }
            dispatchEvent(new FlexEvent(FlexEvent.UPDATE_COMPLETE));
         }
      }
      
      public function validateNow() : void
      {
         var infiniteLoopGuard:int = 0;
         if(!this.usePhasedInstantiation)
         {
            infiniteLoopGuard = 0;
            while(this.listenersAttached && infiniteLoopGuard++ < 100)
            {
               this.doPhasedInstantiation();
            }
         }
      }
      
      public function validateClient(target:ILayoutManagerClient, skipDisplayList:Boolean = false) : void
      {
         var obj:ILayoutManagerClient = null;
         var lastCurrentObject:ILayoutManagerClient = this.currentObject;
         var i:int = 0;
         var done:Boolean = false;
         var oldTargetLevel:int = this.targetLevel;
         if(this.targetLevel == int.MAX_VALUE)
         {
            this.targetLevel = target.nestLevel;
         }
         while(!done)
         {
            done = true;
            obj = ILayoutManagerClient(this.invalidatePropertiesQueue.removeSmallestChild(target));
            while(Boolean(obj))
            {
               if(Boolean(obj.nestLevel))
               {
                  this.currentObject = obj;
                  obj.validateProperties();
                  if(!obj.updateCompletePendingFlag)
                  {
                     this.updateCompleteQueue.addObject(obj,obj.nestLevel);
                     obj.updateCompletePendingFlag = true;
                  }
               }
               obj = ILayoutManagerClient(this.invalidatePropertiesQueue.removeSmallestChild(target));
            }
            if(this.invalidatePropertiesQueue.isEmpty())
            {
               this.invalidatePropertiesFlag = false;
               this.invalidateClientPropertiesFlag = false;
            }
            obj = ILayoutManagerClient(this.invalidateSizeQueue.removeLargestChild(target));
            while(Boolean(obj))
            {
               if(Boolean(obj.nestLevel))
               {
                  this.currentObject = obj;
                  obj.validateSize();
                  if(!obj.updateCompletePendingFlag)
                  {
                     this.updateCompleteQueue.addObject(obj,obj.nestLevel);
                     obj.updateCompletePendingFlag = true;
                  }
               }
               if(this.invalidateClientPropertiesFlag)
               {
                  obj = ILayoutManagerClient(this.invalidatePropertiesQueue.removeSmallestChild(target));
                  if(Boolean(obj))
                  {
                     this.invalidatePropertiesQueue.addObject(obj,obj.nestLevel);
                     done = false;
                     break;
                  }
               }
               obj = ILayoutManagerClient(this.invalidateSizeQueue.removeLargestChild(target));
            }
            if(this.invalidateSizeQueue.isEmpty())
            {
               this.invalidateSizeFlag = false;
               this.invalidateClientSizeFlag = false;
            }
            if(!skipDisplayList)
            {
               obj = ILayoutManagerClient(this.invalidateDisplayListQueue.removeSmallestChild(target));
               while(Boolean(obj))
               {
                  if(Boolean(obj.nestLevel))
                  {
                     this.currentObject = obj;
                     obj.validateDisplayList();
                     if(!obj.updateCompletePendingFlag)
                     {
                        this.updateCompleteQueue.addObject(obj,obj.nestLevel);
                        obj.updateCompletePendingFlag = true;
                     }
                  }
                  if(this.invalidateClientPropertiesFlag)
                  {
                     obj = ILayoutManagerClient(this.invalidatePropertiesQueue.removeSmallestChild(target));
                     if(Boolean(obj))
                     {
                        this.invalidatePropertiesQueue.addObject(obj,obj.nestLevel);
                        done = false;
                        break;
                     }
                  }
                  if(this.invalidateClientSizeFlag)
                  {
                     obj = ILayoutManagerClient(this.invalidateSizeQueue.removeLargestChild(target));
                     if(Boolean(obj))
                     {
                        this.invalidateSizeQueue.addObject(obj,obj.nestLevel);
                        done = false;
                        break;
                     }
                  }
                  obj = ILayoutManagerClient(this.invalidateDisplayListQueue.removeSmallestChild(target));
               }
               if(this.invalidateDisplayListQueue.isEmpty())
               {
                  this.invalidateDisplayListFlag = false;
               }
            }
         }
         if(oldTargetLevel == int.MAX_VALUE)
         {
            this.targetLevel = int.MAX_VALUE;
            if(!skipDisplayList)
            {
               obj = ILayoutManagerClient(this.updateCompleteQueue.removeLargestChild(target));
               while(Boolean(obj))
               {
                  if(!obj.initialized)
                  {
                     obj.initialized = true;
                  }
                  if(obj.hasEventListener(FlexEvent.UPDATE_COMPLETE))
                  {
                     obj.dispatchEvent(new FlexEvent(FlexEvent.UPDATE_COMPLETE));
                  }
                  obj.updateCompletePendingFlag = false;
                  obj = ILayoutManagerClient(this.updateCompleteQueue.removeLargestChild(target));
               }
            }
         }
         this.currentObject = lastCurrentObject;
      }
      
      public function isInvalid() : Boolean
      {
         return this.invalidatePropertiesFlag || this.invalidateSizeFlag || this.invalidateDisplayListFlag;
      }
      
      private function waitAFrame(event:Event) : void
      {
         this.systemManager.removeEventListener(Event.ENTER_FRAME,this.waitAFrame);
         this.systemManager.addEventListener(Event.ENTER_FRAME,this.doPhasedInstantiationCallback);
         this.waitedAFrame = true;
      }
      
      public function attachListeners(systemManager:ISystemManager) : void
      {
         if(!this.waitedAFrame)
         {
            systemManager.addEventListener(Event.ENTER_FRAME,this.waitAFrame);
         }
         else
         {
            systemManager.addEventListener(Event.ENTER_FRAME,this.doPhasedInstantiationCallback);
            if(!this.usePhasedInstantiation)
            {
               if(Boolean(systemManager) && (Boolean(systemManager.stage) || Boolean(this.usingBridge(systemManager))))
               {
                  systemManager.addEventListener(Event.RENDER,this.doPhasedInstantiationCallback);
                  if(Boolean(systemManager.stage))
                  {
                     systemManager.stage.invalidate();
                  }
               }
            }
         }
         this.listenersAttached = true;
      }
      
      private function doPhasedInstantiationCallback(event:Event) : void
      {
         var callLaterErrorEvent:DynamicEvent = null;
         if(UIComponentGlobals.callLaterSuspendCount > 0)
         {
            return;
         }
         this.systemManager.removeEventListener(Event.ENTER_FRAME,this.doPhasedInstantiationCallback);
         this.systemManager.removeEventListener(Event.RENDER,this.doPhasedInstantiationCallback);
         if(!UIComponentGlobals.catchCallLaterExceptions)
         {
            this.doPhasedInstantiation();
         }
         else
         {
            try
            {
               this.doPhasedInstantiation();
            }
            catch(e:Error)
            {
               callLaterErrorEvent = new DynamicEvent("callLaterError");
               callLaterErrorEvent.error = e;
               callLaterErrorEvent.source = this;
               callLaterErrorEvent.object = currentObject;
               systemManager.dispatchEvent(callLaterErrorEvent);
            }
         }
         this.currentObject = null;
      }
      
      private function usingBridge(sm:ISystemManager) : Boolean
      {
         if(this._usingBridge == 0)
         {
            return false;
         }
         if(this._usingBridge == 1)
         {
            return true;
         }
         if(!sm)
         {
            return false;
         }
         var mp:Object = sm.getImplementation("mx.managers::IMarshalSystemManager");
         if(!mp)
         {
            this._usingBridge = 0;
            return false;
         }
         if(Boolean(mp.useSWFBridge()))
         {
            this._usingBridge = 1;
            return true;
         }
         this._usingBridge = 0;
         return false;
      }
   }
}


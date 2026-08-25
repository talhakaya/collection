package mx.binding
{
   import flash.utils.Dictionary;
   import mx.collections.errors.ItemPendingError;
   import mx.core.mx_internal;
   
   use namespace mx_internal;
   
   [ExcludeClass]
   public class Binding
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      mx_internal static var allowedErrors:Object = generateAllowedErrors();
      
      mx_internal var _isEnabled:Boolean;
      
      mx_internal var isExecuting:Boolean;
      
      mx_internal var isHandlingEvent:Boolean;
      
      mx_internal var disabledRequests:Dictionary;
      
      private var hasHadValue:Boolean;
      
      public var uiComponentWatcher:int;
      
      public var twoWayCounterpart:Binding;
      
      public var isTwoWayPrimary:Boolean;
      
      private var wrappedFunctionSuccessful:Boolean;
      
      mx_internal var document:Object;
      
      mx_internal var srcFunc:Function;
      
      mx_internal var destFunc:Function;
      
      mx_internal var destString:String;
      
      mx_internal var srcString:String;
      
      private var lastValue:Object;
      
      public function Binding(document:Object, srcFunc:Function, destFunc:Function, destString:String, srcString:String = null)
      {
         super();
         this.document = document;
         this.srcFunc = srcFunc;
         this.destFunc = destFunc;
         this.destString = destString;
         this.srcString = srcString;
         if(this.srcFunc == null)
         {
            this.srcFunc = this.defaultSrcFunc;
         }
         if(this.destFunc == null)
         {
            this.destFunc = this.defaultDestFunc;
         }
         this._isEnabled = true;
         this.isExecuting = false;
         this.isHandlingEvent = false;
         this.hasHadValue = false;
         this.uiComponentWatcher = -1;
         BindingManager.addBinding(document,destString,this);
      }
      
      mx_internal static function generateAllowedErrors() : Object
      {
         var o:Object = {};
         o[1507] = 1;
         o[2005] = 1;
         return o;
      }
      
      mx_internal function get isEnabled() : Boolean
      {
         return this._isEnabled;
      }
      
      mx_internal function set isEnabled(value:Boolean) : void
      {
         this._isEnabled = value;
         if(value)
         {
            this.processDisabledRequests();
         }
      }
      
      private function defaultDestFunc(value:Object) : void
      {
         var chain:Array = this.destString.split(".");
         var element:Object = this.document;
         var i:uint = 0;
         if(chain[0] == "this")
         {
            i++;
         }
         while(i < chain.length - 1)
         {
            element = element[chain[i++]];
         }
         element[chain[i]] = value;
      }
      
      private function defaultSrcFunc() : Object
      {
         return this.document[this.srcString];
      }
      
      public function execute(o:Object = null) : void
      {
         if(!this.isEnabled)
         {
            if(o != null)
            {
               this.registerDisabledExecute(o);
            }
            return;
         }
         if(Boolean(this.twoWayCounterpart) && Boolean(!this.twoWayCounterpart.hasHadValue) && this.twoWayCounterpart.isTwoWayPrimary)
         {
            this.twoWayCounterpart.execute();
            this.hasHadValue = true;
            return;
         }
         if(this.isExecuting || Boolean(this.twoWayCounterpart) && Boolean(this.twoWayCounterpart.isExecuting))
         {
            this.hasHadValue = true;
            return;
         }
         try
         {
            this.isExecuting = true;
            this.wrapFunctionCall(this,this.innerExecute,o);
         }
         catch(error:Error)
         {
            if(allowedErrors[error.errorID] == null)
            {
               throw error;
            }
         }
         finally
         {
            this.isExecuting = false;
         }
      }
      
      private function registerDisabledExecute(o:Object) : void
      {
         if(o != null)
         {
            this.disabledRequests = this.disabledRequests != null ? this.disabledRequests : new Dictionary(true);
            this.disabledRequests[o] = true;
         }
      }
      
      private function processDisabledRequests() : void
      {
         var key:Object = null;
         if(this.disabledRequests != null)
         {
            for(key in this.disabledRequests)
            {
               this.execute(key);
            }
            this.disabledRequests = null;
         }
      }
      
      protected function wrapFunctionCall(thisArg:Object, wrappedFunction:Function, object:Object = null, ... args) : Object
      {
         var result:Object = null;
         this.wrappedFunctionSuccessful = false;
         try
         {
            result = wrappedFunction.apply(thisArg,args);
            this.wrappedFunctionSuccessful = true;
            return result;
         }
         catch(itemPendingError:ItemPendingError)
         {
            itemPendingError.addResponder(new EvalBindingResponder(this,object));
            if(Boolean(BindingManager.debugDestinationStrings[destString]))
            {
               trace("Binding: destString = " + destString + ", error = " + itemPendingError);
            }
         }
         catch(rangeError:RangeError)
         {
            if(Boolean(BindingManager.debugDestinationStrings[destString]))
            {
               trace("Binding: destString = " + destString + ", error = " + rangeError);
            }
         }
         catch(error:Error)
         {
            if(error.errorID != 1006 && error.errorID != 1009 && error.errorID != 1010 && error.errorID != 1055 && error.errorID != 1069)
            {
               throw error;
            }
            if(Boolean(BindingManager.debugDestinationStrings[destString]))
            {
               trace("Binding: destString = " + destString + ", error = " + error);
            }
         }
         return null;
      }
      
      private function nodeSeqEqual(x:XMLList, y:XMLList) : Boolean
      {
         var i:uint = 0;
         var n:uint = uint(x.length());
         if(n == y.length())
         {
            i = 0;
            while(i < n && x[i] === y[i])
            {
               i++;
            }
            return i == n;
         }
         return false;
      }
      
      private function innerExecute() : void
      {
         var value:Object = this.wrapFunctionCall(this.document,this.srcFunc);
         if(Boolean(BindingManager.debugDestinationStrings[this.destString]))
         {
            trace("Binding: destString = " + this.destString + ", srcFunc result = " + value);
         }
         if(this.hasHadValue || this.wrappedFunctionSuccessful)
         {
            if(!(this.lastValue is XML && Boolean(this.lastValue.hasComplexContent()) && this.lastValue === value) && !(this.lastValue is XMLList && this.lastValue.hasComplexContent() && value is XMLList && this.nodeSeqEqual(this.lastValue as XMLList,value as XMLList)))
            {
               this.destFunc.call(this.document,value);
               this.lastValue = value;
               this.hasHadValue = true;
            }
         }
      }
      
      public function watcherFired(commitEvent:Boolean, cloneIndex:int) : void
      {
         if(this.isHandlingEvent)
         {
            return;
         }
         try
         {
            this.isHandlingEvent = true;
            this.execute(cloneIndex);
         }
         finally
         {
            this.isHandlingEvent = false;
         }
      }
   }
}


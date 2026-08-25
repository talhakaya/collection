package mx.states
{
   import flash.events.EventDispatcher;
   import mx.core.mx_internal;
   import mx.events.FlexEvent;
   
   use namespace mx_internal;
   
   [DefaultProperty("overrides")]
   [Event(name="exitState",type="mx.events.FlexEvent")]
   [Event(name="enterState",type="mx.events.FlexEvent")]
   public class State extends EventDispatcher
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private var initialized:Boolean = false;
      
      [Inspectable(category="General")]
      public var basedOn:String;
      
      [Inspectable(category="General")]
      public var name:String;
      
      [Inspectable(category="General")]
      [ArrayElementType("mx.states.IOverride")]
      public var overrides:Array;
      
      [Inspectable(category="General")]
      [ArrayElementType("String")]
      public var stateGroups:Array;
      
      public function State(properties:Object = null)
      {
         var p:String = null;
         this.overrides = [];
         this.stateGroups = [];
         super();
         for(p in properties)
         {
            this[p] = properties[p];
         }
      }
      
      mx_internal function initialize() : void
      {
         var i:int = 0;
         if(!this.initialized)
         {
            this.initialized = true;
            for(i = 0; i < this.overrides.length; i++)
            {
               IOverride(this.overrides[i]).initialize();
            }
         }
      }
      
      mx_internal function dispatchEnterState() : void
      {
         if(hasEventListener(FlexEvent.ENTER_STATE))
         {
            dispatchEvent(new FlexEvent(FlexEvent.ENTER_STATE));
         }
      }
      
      mx_internal function dispatchExitState() : void
      {
         if(hasEventListener(FlexEvent.EXIT_STATE))
         {
            dispatchEvent(new FlexEvent(FlexEvent.EXIT_STATE));
         }
      }
   }
}


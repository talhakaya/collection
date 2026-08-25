package mx.events
{
   import flash.events.Event;
   
   public class TouchInteractionEvent extends Event
   {
      
      public static const TOUCH_INTERACTION_STARTING:String = "touchInteractionStarting";
      
      public static const TOUCH_INTERACTION_START:String = "touchInteractionStart";
      
      public static const TOUCH_INTERACTION_END:String = "touchInteractionEnd";
      
      public var reason:String;
      
      public var relatedObject:Object;
      
      public function TouchInteractionEvent(type:String, bubbles:Boolean = false, cancelable:Boolean = false)
      {
         super(type,bubbles,cancelable);
      }
      
      override public function clone() : Event
      {
         var clonedEvent:TouchInteractionEvent = new TouchInteractionEvent(type,bubbles,cancelable);
         clonedEvent.reason = this.reason;
         clonedEvent.relatedObject = this.relatedObject;
         return clonedEvent;
      }
   }
}


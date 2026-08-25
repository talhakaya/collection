package mx.events
{
   import flash.events.Event;
   import mx.core.mx_internal;
   import mx.effects.IEffectInstance;
   
   use namespace mx_internal;
   
   public class EffectEvent extends Event
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      public static const EFFECT_END:String = "effectEnd";
      
      public static const EFFECT_STOP:String = "effectStop";
      
      public static const EFFECT_START:String = "effectStart";
      
      public static const EFFECT_REPEAT:String = "effectRepeat";
      
      public static const EFFECT_UPDATE:String = "effectUpdate";
      
      public var effectInstance:IEffectInstance;
      
      public function EffectEvent(eventType:String, bubbles:Boolean = false, cancelable:Boolean = false, effectInstance:IEffectInstance = null)
      {
         super(eventType,bubbles,cancelable);
         this.effectInstance = effectInstance;
      }
      
      override public function clone() : Event
      {
         return new EffectEvent(type,bubbles,cancelable,this.effectInstance);
      }
   }
}


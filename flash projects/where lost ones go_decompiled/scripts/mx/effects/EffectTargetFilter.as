package mx.effects
{
   import mx.core.mx_internal;
   import mx.effects.effectClasses.PropertyChanges;
   
   use namespace mx_internal;
   
   public class EffectTargetFilter
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      public var filterFunction:Function;
      
      public var filterProperties:Array = [];
      
      public var filterStyles:Array = [];
      
      public var requiredSemantics:Object = null;
      
      public function EffectTargetFilter()
      {
         this.filterFunction = this.defaultFilterFunctionEx;
         super();
      }
      
      public function filterInstance(propChanges:Array, semanticsProvider:IEffectTargetHost, target:Object) : Boolean
      {
         if(this.filterFunction.length == 2)
         {
            return this.filterFunction(propChanges,target);
         }
         return this.filterFunction(propChanges,semanticsProvider,target);
      }
      
      protected function defaultFilterFunctionEx(propChanges:Array, semanticsProvider:IEffectTargetHost, target:Object) : Boolean
      {
         var prop:String = null;
         if(Boolean(this.requiredSemantics))
         {
            for(prop in this.requiredSemantics)
            {
               if(!semanticsProvider)
               {
                  return false;
               }
               if(semanticsProvider.getRendererSemanticValue(target,prop) != this.requiredSemantics[prop])
               {
                  return false;
               }
            }
            return true;
         }
         return this.defaultFilterFunction(propChanges,target);
      }
      
      protected function defaultFilterFunction(propChanges:Array, instanceTarget:Object) : Boolean
      {
         var props:PropertyChanges = null;
         var triggers:Array = null;
         var m:int = 0;
         var j:int = 0;
         var n:int = int(propChanges.length);
         for(var i:int = 0; i < n; i++)
         {
            props = propChanges[i];
            if(props.target == instanceTarget)
            {
               triggers = this.filterProperties.concat(this.filterStyles);
               m = int(triggers.length);
               for(j = 0; j < m; j++)
               {
                  if(props.start[triggers[j]] !== undefined && props.end[triggers[j]] != props.start[triggers[j]])
                  {
                     return true;
                  }
               }
            }
         }
         return false;
      }
   }
}


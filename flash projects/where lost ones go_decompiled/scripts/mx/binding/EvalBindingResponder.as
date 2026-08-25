package mx.binding
{
   import mx.core.mx_internal;
   import mx.rpc.IResponder;
   
   use namespace mx_internal;
   
   [ExcludeClass]
   public class EvalBindingResponder implements IResponder
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private var binding:Binding;
      
      private var object:Object;
      
      public function EvalBindingResponder(binding:Binding, object:Object)
      {
         super();
         this.binding = binding;
         this.object = object;
      }
      
      public function result(data:Object) : void
      {
         this.binding.execute(this.object);
      }
      
      public function fault(data:Object) : void
      {
      }
   }
}


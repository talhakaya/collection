package mx.core
{
   use namespace mx_internal;
   
   public class ComponentDescriptor
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      public var document:Object;
      
      public var events:Object;
      
      public var id:String;
      
      private var _properties:Object;
      
      public var propertiesFactory:Function;
      
      public var type:Class;
      
      public function ComponentDescriptor(descriptorProperties:Object)
      {
         var p:String = null;
         super();
         for(p in descriptorProperties)
         {
            this[p] = descriptorProperties[p];
         }
      }
      
      public function get properties() : Object
      {
         var cd:Array = null;
         var n:int = 0;
         var i:int = 0;
         if(Boolean(this._properties))
         {
            return this._properties;
         }
         if(this.propertiesFactory != null)
         {
            this._properties = this.propertiesFactory.call(this.document);
         }
         if(Boolean(this._properties))
         {
            cd = this._properties.childDescriptors;
            if(Boolean(cd))
            {
               n = int(cd.length);
               for(i = 0; i < n; i++)
               {
                  cd[i].document = this.document;
               }
            }
         }
         else
         {
            this._properties = {};
         }
         return this._properties;
      }
      
      public function invalidateProperties() : void
      {
         this._properties = null;
      }
      
      public function toString() : String
      {
         return "ComponentDescriptor_" + this.id;
      }
   }
}


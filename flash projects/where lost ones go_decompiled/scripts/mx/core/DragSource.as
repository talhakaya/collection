package mx.core
{
   use namespace mx_internal;
   
   public class DragSource
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private var dataHolder:Object = {};
      
      private var formatHandlers:Object = {};
      
      private var _formats:Array = [];
      
      public function DragSource()
      {
         super();
      }
      
      public function get formats() : Array
      {
         return this._formats;
      }
      
      public function addData(data:Object, format:String) : void
      {
         this._formats.push(format);
         this.dataHolder[format] = data;
      }
      
      public function addHandler(handler:Function, format:String) : void
      {
         this._formats.push(format);
         this.formatHandlers[format] = handler;
      }
      
      public function dataForFormat(format:String) : Object
      {
         var data:Object = this.dataHolder[format];
         if(Boolean(data))
         {
            return data;
         }
         if(Boolean(this.formatHandlers[format]))
         {
            return this.formatHandlers[format]();
         }
         return null;
      }
      
      public function hasFormat(format:String) : Boolean
      {
         var n:int = int(this._formats.length);
         for(var i:int = 0; i < n; i++)
         {
            if(this._formats[i] == format)
            {
               return true;
            }
         }
         return false;
      }
   }
}


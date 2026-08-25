package mx.managers.layoutClasses
{
   import flash.display.DisplayObject;
   import flash.display.DisplayObjectContainer;
   import mx.core.IChildList;
   import mx.core.IRawChildrenContainer;
   import mx.core.mx_internal;
   import mx.managers.ILayoutManagerClient;
   
   use namespace mx_internal;
   
   [ExcludeClass]
   public class PriorityQueue
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private var priorityBins:Array = [];
      
      private var minPriority:int = 0;
      
      private var maxPriority:int = -1;
      
      public function PriorityQueue()
      {
         super();
      }
      
      public function addObject(obj:Object, priority:int) : void
      {
         if(this.maxPriority < this.minPriority)
         {
            this.minPriority = this.maxPriority = priority;
         }
         else
         {
            if(priority < this.minPriority)
            {
               this.minPriority = priority;
            }
            if(priority > this.maxPriority)
            {
               this.maxPriority = priority;
            }
         }
         var bin:PriorityBin = this.priorityBins[priority];
         if(!bin)
         {
            bin = new PriorityBin();
            this.priorityBins[priority] = bin;
            bin.items[obj] = true;
            ++bin.length;
         }
         else if(bin.items[obj] == null)
         {
            bin.items[obj] = true;
            ++bin.length;
         }
      }
      
      public function removeLargest() : Object
      {
         var bin:PriorityBin = null;
         var key:Object = null;
         var obj:Object = null;
         if(this.minPriority <= this.maxPriority)
         {
            bin = this.priorityBins[this.maxPriority];
            while(!bin || bin.length == 0)
            {
               --this.maxPriority;
               if(this.maxPriority < this.minPriority)
               {
                  return null;
               }
               bin = this.priorityBins[this.maxPriority];
            }
            var _loc4_:int = 0;
            var _loc5_:* = bin.items;
            for(key in _loc5_)
            {
               obj = key;
               this.removeChild(ILayoutManagerClient(key),this.maxPriority);
            }
            while(!bin || bin.length == 0)
            {
               --this.maxPriority;
               if(this.maxPriority < this.minPriority)
               {
                  break;
               }
               bin = this.priorityBins[this.maxPriority];
            }
         }
         return obj;
      }
      
      public function removeLargestChild(client:ILayoutManagerClient) : Object
      {
         var bin:PriorityBin = null;
         var key:Object = null;
         var max:int = this.maxPriority;
         var min:int = client.nestLevel;
         while(min <= max)
         {
            bin = this.priorityBins[max];
            if(Boolean(bin) && bin.length > 0)
            {
               if(max == client.nestLevel)
               {
                  if(Boolean(bin.items[client]))
                  {
                     this.removeChild(ILayoutManagerClient(client),max);
                     return client;
                  }
               }
               else
               {
                  for(key in bin.items)
                  {
                     if(key is DisplayObject && this.contains(DisplayObject(client),DisplayObject(key)))
                     {
                        this.removeChild(ILayoutManagerClient(key),max);
                        return key;
                     }
                  }
               }
               max--;
            }
            else
            {
               if(max == this.maxPriority)
               {
                  --this.maxPriority;
               }
               max--;
               if(max < min)
               {
                  break;
               }
            }
         }
         return null;
      }
      
      public function removeSmallest() : Object
      {
         var bin:PriorityBin = null;
         var key:Object = null;
         var obj:Object = null;
         if(this.minPriority <= this.maxPriority)
         {
            bin = this.priorityBins[this.minPriority];
            while(!bin || bin.length == 0)
            {
               ++this.minPriority;
               if(this.minPriority > this.maxPriority)
               {
                  return null;
               }
               bin = this.priorityBins[this.minPriority];
            }
            var _loc4_:int = 0;
            var _loc5_:* = bin.items;
            for(key in _loc5_)
            {
               obj = key;
               this.removeChild(ILayoutManagerClient(key),this.minPriority);
            }
            while(!bin || bin.length == 0)
            {
               ++this.minPriority;
               if(this.minPriority > this.maxPriority)
               {
                  break;
               }
               bin = this.priorityBins[this.minPriority];
            }
         }
         return obj;
      }
      
      public function removeSmallestChild(client:ILayoutManagerClient) : Object
      {
         var bin:PriorityBin = null;
         var key:Object = null;
         var min:int = client.nestLevel;
         while(min <= this.maxPriority)
         {
            bin = this.priorityBins[min];
            if(Boolean(bin) && bin.length > 0)
            {
               if(min == client.nestLevel)
               {
                  if(Boolean(bin.items[client]))
                  {
                     this.removeChild(ILayoutManagerClient(client),min);
                     return client;
                  }
               }
               else
               {
                  for(key in bin.items)
                  {
                     if(key is DisplayObject && this.contains(DisplayObject(client),DisplayObject(key)))
                     {
                        this.removeChild(ILayoutManagerClient(key),min);
                        return key;
                     }
                  }
               }
               min++;
            }
            else
            {
               if(min == this.minPriority)
               {
                  ++this.minPriority;
               }
               min++;
               if(min > this.maxPriority)
               {
                  break;
               }
            }
         }
         return null;
      }
      
      public function removeChild(client:ILayoutManagerClient, level:int = -1) : Object
      {
         var priority:int = level >= 0 ? level : client.nestLevel;
         var bin:PriorityBin = this.priorityBins[priority];
         if(Boolean(bin) && bin.items[client] != null)
         {
            delete bin.items[client];
            --bin.length;
            return client;
         }
         return null;
      }
      
      public function removeAll() : void
      {
         this.priorityBins.length = 0;
         this.minPriority = 0;
         this.maxPriority = -1;
      }
      
      public function isEmpty() : Boolean
      {
         return this.minPriority > this.maxPriority;
      }
      
      private function contains(parent:DisplayObject, child:DisplayObject) : Boolean
      {
         var rawChildren:IChildList = null;
         if(parent is IRawChildrenContainer)
         {
            rawChildren = IRawChildrenContainer(parent).rawChildren;
            return rawChildren.contains(child);
         }
         if(parent is DisplayObjectContainer)
         {
            return DisplayObjectContainer(parent).contains(child);
         }
         return parent == child;
      }
   }
}

import flash.utils.Dictionary;

class PriorityBin
{
   
   public var length:int;
   
   public var items:Dictionary = new Dictionary();
   
   public function PriorityBin()
   {
      super();
   }
}

package
{
   import org.flixel.FlxBasic;
   
   public class TTimer extends FlxBasic
   {
      
      public var count:int;
      
      private var upTo:int;
      
      public var started:Boolean;
      
      public var complete:Boolean;
      
      public function TTimer()
      {
         super();
         this.reset();
      }
      
      public function start(_countUpTo:int) : void
      {
         if(!this.started)
         {
            this.reset();
            this.started = true;
            this.upTo = _countUpTo;
         }
      }
      
      public function reset() : void
      {
         this.count = 0;
         this.started = false;
         this.complete = false;
         this.upTo = 0;
      }
      
      override public function update() : void
      {
         super.update();
         if(this.started)
         {
            if(this.count < this.upTo)
            {
               ++this.count;
            }
            else
            {
               this.reset();
               this.complete = true;
            }
         }
         if(this.complete)
         {
            if(this.count == 0)
            {
               this.count = -1;
            }
            else if(this.count == -1)
            {
               this.reset();
            }
         }
      }
   }
}


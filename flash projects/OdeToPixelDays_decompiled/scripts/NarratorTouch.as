package
{
   import org.flixel.FlxSprite;
   
   public class NarratorTouch extends FlxSprite
   {
      
      private var touched:Boolean;
      
      private var narrator:Narrator;
      
      public var lineIncrementer:Boolean;
      
      public function NarratorTouch(_X:Number, _Y:Number, _narrator:Narrator)
      {
         super();
         x = _X;
         y = _Y;
         this.narrator = _narrator;
         width = 32;
         height = 240;
         this.touched = false;
         alpha = 0;
         this.lineIncrementer = false;
      }
      
      override public function kill() : void
      {
         if(!this.touched)
         {
            this.touched = true;
            if(this.lineIncrementer)
            {
               ++this.narrator.line;
            }
            else
            {
               this.narrator.kill();
            }
         }
      }
   }
}


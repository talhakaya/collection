package
{
   import org.flixel.FlxSprite;
   
   public class SoyutTekme extends FlxSprite
   {
      
      public var byNaz:Boolean;
      
      private var count:int = 15;
      
      public function SoyutTekme(_X:Number, _Y:Number, naz:Boolean)
      {
         super();
         x = _X;
         y = _Y;
         this.byNaz = naz;
         makeGraphic(40,8,4294901760);
         alpha = 0;
      }
      
      override public function update() : void
      {
         super.update();
         --this.count;
         if(this.count < 0)
         {
            kill();
         }
      }
   }
}


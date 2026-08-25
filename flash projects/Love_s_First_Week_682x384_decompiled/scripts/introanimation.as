package
{
   import flash.display.MovieClip;
   
   [Embed(source="/_assets/assets.swf", symbol="symbol199")]
   public class introanimation extends MovieClip
   {
      
      public var addGame:Boolean;
      
      public function introanimation()
      {
         super();
         addFrameScript(144,this.frame145);
      }
      
      internal function frame145() : *
      {
         this.addGame = true;
         stop();
      }
   }
}


package MaxGames_fla
{
   import flash.display.MovieClip;
   import flash.events.Event;
   
   [Embed(source="/_assets/assets.swf", symbol="symbol68")]
   public dynamic class MaxGames_MAXLOGOINTRO1_1 extends MovieClip
   {
      
      public function MaxGames_MAXLOGOINTRO1_1()
      {
         super();
         addFrameScript(0,frame1,185,frame186);
      }
      
      internal function frame1() : *
      {
      }
      
      internal function frame186() : *
      {
         this.dispatchEvent(new Event("LOGO_FINISHED"));
      }
   }
}


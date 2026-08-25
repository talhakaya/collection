package
{
   import flash.Boot;
   import flash.Lib;
   import flash.display.Stage;
   
   public class DocumentClass extends Main
   {
      
      public function DocumentClass()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         super();
      }
      
      override public function get stage() : Stage
      {
         return Lib.current.stage;
      }
   }
}


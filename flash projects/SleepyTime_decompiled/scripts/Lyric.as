package
{
   import flash.Boot;
   
   public class Lyric
   {
      
      public var time:Number;
      
      public var text:String;
      
      public function Lyric(param1:Number = 0, param2:String = undefined)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         time = param1;
         text = param2;
      }
   }
}


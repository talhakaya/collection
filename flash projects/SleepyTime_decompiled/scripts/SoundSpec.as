package
{
   import flash.Boot;
   import flash.media.Sound;
   import flash.media.SoundTransform;
   
   public class SoundSpec
   {
      
      public var soundTransform:SoundTransform;
      
      public var sound:Sound;
      
      public var panning:Number;
      
      public function SoundSpec(param1:Sound = undefined, param2:SoundTransform = undefined, param3:Number = 0)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         sound = param1;
         soundTransform = param2;
         panning = param3;
      }
   }
}


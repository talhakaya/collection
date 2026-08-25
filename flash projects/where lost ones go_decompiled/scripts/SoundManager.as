package
{
   import flash.display.Sprite;
   import flash.events.Event;
   import flash.media.Sound;
   import flash.media.SoundChannel;
   import flash.media.SoundTransform;
   
   public class SoundManager extends Sprite
   {
      
      private static var soundBeach:Sound;
      
      private static var soundChannelBeach:SoundChannel;
      
      private static var soundTransformBeach:SoundTransform;
      
      private static var soundVolumeUp:Boolean;
      
      public static var instance:SoundManager;
      
      public static var music:Class = SoundManager_music;
      
      public static var beach:Class = SoundManager_beach;
      
      public function SoundManager()
      {
         super();
         instance = this;
      }
      
      public static function enterFrameHandler(e:Event) : void
      {
         soundChannelBeach.soundTransform = soundTransformBeach;
         if(!soundVolumeUp)
         {
            soundTransformBeach.volume -= 0.004;
            if(soundTransformBeach.volume <= 0)
            {
               instance.removeEventListener(Event.ENTER_FRAME,enterFrameHandler);
            }
         }
         else
         {
            soundTransformBeach.volume += 0.004;
            if(soundTransformBeach.volume >= 1)
            {
               instance.removeEventListener(Event.ENTER_FRAME,enterFrameHandler);
            }
         }
      }
      
      public static function playBeach() : void
      {
         soundBeach = new beach();
         soundTransformBeach = new SoundTransform(1,0);
         soundChannelBeach = soundBeach.play(0,99);
      }
      
      public static function stopBeach() : void
      {
         soundVolumeUp = false;
         instance.addEventListener(Event.ENTER_FRAME,enterFrameHandler);
      }
      
      public static function continueBeach() : void
      {
         soundVolumeUp = true;
         soundTransformBeach = new SoundTransform(0,0);
         instance.addEventListener(Event.ENTER_FRAME,enterFrameHandler);
      }
      
      public static function playMusic() : void
      {
         var sound:Sound = new music();
         sound.play();
      }
   }
}


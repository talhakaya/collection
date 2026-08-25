package
{
   import flash.Boot;
   import flash.media.Sound;
   import flash.media.SoundLoaderContext;
   import flash.net.URLRequest;
   
   [Embed(source="/_assets/1076___ASSET__snd_menu2_mp3.mp3")]
   public class __ASSET__snd_menu2_mp3 extends Sound
   {
      
      public function __ASSET__snd_menu2_mp3(param1:URLRequest = undefined, param2:SoundLoaderContext = undefined)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         super(param1,param2);
      }
   }
}


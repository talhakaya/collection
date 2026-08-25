package
{
   import flash.Boot;
   import flash.display.Bitmap;
   import flash.display.BitmapData;
   import flash.display.Loader;
   import flash.events.Event;
   import flash.media.Sound;
   import flash.net.URLLoader;
   import flash.net.URLRequest;
   import flash.text.Font;
   import flash.utils.ByteArray;
   import openfl.AssetLibrary;
   import openfl.AssetType;
   
   public class DefaultAssetLibrary extends AssetLibrary
   {
      
      public static var init__:Boolean;
      
      public static var className:IMap;
      
      public static var path:IMap;
      
      public static var type:IMap;
      
      public function DefaultAssetLibrary()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         super();
         DefaultAssetLibrary.className.set("img/bald.png",__ASSET__img_bald_png);
         var _loc1_:AssetType = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/bald.png",_loc1_);
         DefaultAssetLibrary.className.set("img/bereket.png",__ASSET__img_bereket_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/bereket.png",_loc1_);
         DefaultAssetLibrary.className.set("img/bg.jpg",__ASSET__img_bg_jpg);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/bg.jpg",_loc1_);
         DefaultAssetLibrary.className.set("img/blind.png",__ASSET__img_blind_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/blind.png",_loc1_);
         DefaultAssetLibrary.className.set("img/buda.png",__ASSET__img_buda_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/buda.png",_loc1_);
         DefaultAssetLibrary.className.set("img/ceasar.png",__ASSET__img_ceasar_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/ceasar.png",_loc1_);
         DefaultAssetLibrary.className.set("img/crosshair1.png",__ASSET__img_crosshair1_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/crosshair1.png",_loc1_);
         DefaultAssetLibrary.className.set("img/freaky.png",__ASSET__img_freaky_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/freaky.png",_loc1_);
         DefaultAssetLibrary.className.set("img/headphones.png",__ASSET__img_headphones_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/headphones.png",_loc1_);
         DefaultAssetLibrary.className.set("img/kayabros.png",__ASSET__img_kayabros_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/kayabros.png",_loc1_);
         DefaultAssetLibrary.className.set("img/line.png",__ASSET__img_line_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/line.png",_loc1_);
         DefaultAssetLibrary.className.set("img/lover.png",__ASSET__img_lover_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/lover.png",_loc1_);
         DefaultAssetLibrary.className.set("img/mother.png",__ASSET__img_mother_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/mother.png",_loc1_);
         DefaultAssetLibrary.className.set("img/particle.png",__ASSET__img_particle_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/particle.png",_loc1_);
         DefaultAssetLibrary.className.set("img/politician.png",__ASSET__img_politician_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/politician.png",_loc1_);
         DefaultAssetLibrary.className.set("img/sad.png",__ASSET__img_sad_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/sad.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene1/color1.png",__ASSET__img_scene1_color1_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene1/color1.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene1/color2.png",__ASSET__img_scene1_color2_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene1/color2.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene1/hand.png",__ASSET__img_scene1_hand_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene1/hand.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene1/head.png",__ASSET__img_scene1_head_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene1/head.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene1/line1.png",__ASSET__img_scene1_line1_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene1/line1.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene1/line2.png",__ASSET__img_scene1_line2_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene1/line2.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene1/penis.png",__ASSET__img_scene1_penis_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene1/penis.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene2/color1.png",__ASSET__img_scene2_color1_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene2/color1.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene2/color2.png",__ASSET__img_scene2_color2_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene2/color2.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene2/hand.png",__ASSET__img_scene2_hand_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene2/hand.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene2/head.png",__ASSET__img_scene2_head_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene2/head.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene2/line1.png",__ASSET__img_scene2_line1_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene2/line1.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene2/line2.png",__ASSET__img_scene2_line2_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene2/line2.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene2/penis.png",__ASSET__img_scene2_penis_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene2/penis.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene3/color1.png",__ASSET__img_scene3_color1_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene3/color1.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene3/color2.png",__ASSET__img_scene3_color2_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene3/color2.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene3/hand.png",__ASSET__img_scene3_hand_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene3/hand.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene3/head.png",__ASSET__img_scene3_head_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene3/head.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene3/line1.png",__ASSET__img_scene3_line1_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene3/line1.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene3/line2.png",__ASSET__img_scene3_line2_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene3/line2.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene3/penis.png",__ASSET__img_scene3_penis_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene3/penis.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene4/color1.png",__ASSET__img_scene4_color1_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene4/color1.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene4/color2.png",__ASSET__img_scene4_color2_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene4/color2.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene4/hand.png",__ASSET__img_scene4_hand_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene4/hand.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene4/head.png",__ASSET__img_scene4_head_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene4/head.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene4/line1.png",__ASSET__img_scene4_line1_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene4/line1.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene4/line2.png",__ASSET__img_scene4_line2_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene4/line2.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene4/penis.png",__ASSET__img_scene4_penis_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene4/penis.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene5/color1.png",__ASSET__img_scene5_color1_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene5/color1.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene5/color2.png",__ASSET__img_scene5_color2_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene5/color2.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene5/hand.png",__ASSET__img_scene5_hand_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene5/hand.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene5/head.png",__ASSET__img_scene5_head_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene5/head.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene5/line1.png",__ASSET__img_scene5_line1_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene5/line1.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene5/line2.png",__ASSET__img_scene5_line2_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene5/line2.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene5/penis.png",__ASSET__img_scene5_penis_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene5/penis.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene6/line1.png",__ASSET__img_scene6_line1_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene6/line1.png",_loc1_);
         DefaultAssetLibrary.className.set("img/scene6/line2.png",__ASSET__img_scene6_line2_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/scene6/line2.png",_loc1_);
         DefaultAssetLibrary.className.set("img/sleepy time.png",__ASSET__img_sleepy_time_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/sleepy time.png",_loc1_);
         DefaultAssetLibrary.className.set("img/spike.png",__ASSET__img_spike_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/spike.png",_loc1_);
         DefaultAssetLibrary.className.set("img/star_empty.png",__ASSET__img_star_empty_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/star_empty.png",_loc1_);
         DefaultAssetLibrary.className.set("img/star_full.png",__ASSET__img_star_full_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/star_full.png",_loc1_);
         DefaultAssetLibrary.className.set("img/tache.png",__ASSET__img_tache_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/tache.png",_loc1_);
         DefaultAssetLibrary.className.set("img/tutorialMouse.png",__ASSET__img_tutorialmouse_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/tutorialMouse.png",_loc1_);
         DefaultAssetLibrary.className.set("img/tutorialSpace.png",__ASSET__img_tutorialspace_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/tutorialSpace.png",_loc1_);
         DefaultAssetLibrary.className.set("img/tutorialSpace2.png",__ASSET__img_tutorialspace2_png);
         _loc1_ = Reflect.field(AssetType,"image".toUpperCase());
         DefaultAssetLibrary.type.set("img/tutorialSpace2.png",_loc1_);
         DefaultAssetLibrary.className.set("snd/1.mp3",__ASSET__snd_1_mp3);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/1.mp3",_loc1_);
         DefaultAssetLibrary.className.set("snd/1.ogg",__ASSET__snd_1_ogg);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/1.ogg",_loc1_);
         DefaultAssetLibrary.className.set("snd/2.mp3",__ASSET__snd_2_mp3);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/2.mp3",_loc1_);
         DefaultAssetLibrary.className.set("snd/2.ogg",__ASSET__snd_2_ogg);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/2.ogg",_loc1_);
         DefaultAssetLibrary.className.set("snd/3.mp3",__ASSET__snd_3_mp3);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/3.mp3",_loc1_);
         DefaultAssetLibrary.className.set("snd/3.ogg",__ASSET__snd_3_ogg);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/3.ogg",_loc1_);
         DefaultAssetLibrary.className.set("snd/4.mp3",__ASSET__snd_4_mp3);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/4.mp3",_loc1_);
         DefaultAssetLibrary.className.set("snd/4.ogg",__ASSET__snd_4_ogg);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/4.ogg",_loc1_);
         DefaultAssetLibrary.className.set("snd/5.mp3",__ASSET__snd_5_mp3);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/5.mp3",_loc1_);
         DefaultAssetLibrary.className.set("snd/5.ogg",__ASSET__snd_5_ogg);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/5.ogg",_loc1_);
         DefaultAssetLibrary.className.set("snd/6.mp3",__ASSET__snd_6_mp3);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/6.mp3",_loc1_);
         DefaultAssetLibrary.className.set("snd/6.ogg",__ASSET__snd_6_ogg);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/6.ogg",_loc1_);
         DefaultAssetLibrary.className.set("snd/explosion.wav",__ASSET__snd_explosion_wav);
         _loc1_ = Reflect.field(AssetType,"sound".toUpperCase());
         DefaultAssetLibrary.type.set("snd/explosion.wav",_loc1_);
         DefaultAssetLibrary.className.set("snd/menu1.mp3",__ASSET__snd_menu1_mp3);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/menu1.mp3",_loc1_);
         DefaultAssetLibrary.className.set("snd/menu1.ogg",__ASSET__snd_menu1_ogg);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/menu1.ogg",_loc1_);
         DefaultAssetLibrary.className.set("snd/menu2.mp3",__ASSET__snd_menu2_mp3);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/menu2.mp3",_loc1_);
         DefaultAssetLibrary.className.set("snd/menu2.ogg",__ASSET__snd_menu2_ogg);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/menu2.ogg",_loc1_);
         DefaultAssetLibrary.className.set("snd/menu3.mp3",__ASSET__snd_menu3_mp3);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/menu3.mp3",_loc1_);
         DefaultAssetLibrary.className.set("snd/menu3.ogg",__ASSET__snd_menu3_ogg);
         _loc1_ = Reflect.field(AssetType,"music".toUpperCase());
         DefaultAssetLibrary.type.set("snd/menu3.ogg",_loc1_);
         DefaultAssetLibrary.className.set("snd/rate good.wav",__ASSET__snd_rate_good_wav);
         _loc1_ = Reflect.field(AssetType,"sound".toUpperCase());
         DefaultAssetLibrary.type.set("snd/rate good.wav",_loc1_);
         DefaultAssetLibrary.className.set("snd/rate great.wav",__ASSET__snd_rate_great_wav);
         _loc1_ = Reflect.field(AssetType,"sound".toUpperCase());
         DefaultAssetLibrary.type.set("snd/rate great.wav",_loc1_);
         DefaultAssetLibrary.className.set("snd/rate ok.wav",__ASSET__snd_rate_ok_wav);
         _loc1_ = Reflect.field(AssetType,"sound".toUpperCase());
         DefaultAssetLibrary.type.set("snd/rate ok.wav",_loc1_);
         DefaultAssetLibrary.className.set("snd/rate perfect.wav",__ASSET__snd_rate_perfect_wav);
         _loc1_ = Reflect.field(AssetType,"sound".toUpperCase());
         DefaultAssetLibrary.type.set("snd/rate perfect.wav",_loc1_);
         DefaultAssetLibrary.className.set("snd/rate sad.wav",__ASSET__snd_rate_sad_wav);
         _loc1_ = Reflect.field(AssetType,"sound".toUpperCase());
         DefaultAssetLibrary.type.set("snd/rate sad.wav",_loc1_);
      }
      
      override public function loadText(param1:String, param2:Function) : void
      {
         var handler:Function = param2;
         var _loc3_:Function = function(param1:ByteArray):void
         {
            if(param1 == null)
            {
               handler(null);
            }
            else
            {
               handler(param1.readUTFBytes(param1.length));
            }
         };
         loadBytes(param1,_loc3_);
      }
      
      override public function loadSound(param1:String, param2:Function) : void
      {
         param2(getSound(param1));
      }
      
      override public function loadMusic(param1:String, param2:Function) : void
      {
         param2(getMusic(param1));
      }
      
      override public function loadFont(param1:String, param2:Function) : void
      {
         param2(getFont(param1));
      }
      
      override public function loadBytes(param1:String, param2:Function) : void
      {
         var handler:Function;
         var _loc3_:* = null as URLLoader;
         handler = param2;
         if("$" + param1 in DefaultAssetLibrary.path.h)
         {
            _loc3_ = new URLLoader();
            _loc3_.addEventListener(Event.COMPLETE,function(param1:Event):void
            {
               var _loc2_:ByteArray = new ByteArray();
               _loc2_.writeUTFBytes(param1.currentTarget.data);
               _loc2_.position = 0;
               handler(_loc2_);
            });
            _loc3_.load(new URLRequest(DefaultAssetLibrary.path.get(param1)));
         }
         else
         {
            handler(getBytes(param1));
         }
      }
      
      override public function loadBitmapData(param1:String, param2:Function) : void
      {
         var handler:Function;
         var _loc3_:* = null as Loader;
         handler = param2;
         if("$" + param1 in DefaultAssetLibrary.path.h)
         {
            _loc3_ = new Loader();
            _loc3_.contentLoaderInfo.addEventListener(Event.COMPLETE,function(param1:Event):void
            {
               handler(param1.currentTarget.content.bitmapData);
            });
            _loc3_.load(new URLRequest(DefaultAssetLibrary.path.get(param1)));
         }
         else
         {
            handler(getBitmapData(param1));
         }
      }
      
      override public function isLocal(param1:String, param2:AssetType) : Boolean
      {
         if(param2 != AssetType.MUSIC && param2 != AssetType.SOUND)
         {
            return "$" + param1 in DefaultAssetLibrary.className.h;
         }
         return true;
      }
      
      override public function getText(param1:String) : String
      {
         var _loc2_:ByteArray = getBytes(param1);
         if(_loc2_ == null)
         {
            return null;
         }
         return _loc2_.readUTFBytes(_loc2_.length);
      }
      
      override public function getSound(param1:String) : Sound
      {
         return Type.createInstance(DefaultAssetLibrary.className.get(param1),[]);
      }
      
      override public function getPath(param1:String) : String
      {
         return DefaultAssetLibrary.path.get(param1);
      }
      
      override public function getMusic(param1:String) : Sound
      {
         return Type.createInstance(DefaultAssetLibrary.className.get(param1),[]);
      }
      
      override public function getFont(param1:String) : Font
      {
         return Type.createInstance(DefaultAssetLibrary.className.get(param1),[]);
      }
      
      override public function getBytes(param1:String) : ByteArray
      {
         return Type.createInstance(DefaultAssetLibrary.className.get(param1),[]);
      }
      
      override public function getBitmapData(param1:String) : BitmapData
      {
         return Type.createInstance(DefaultAssetLibrary.className.get(param1),[]);
      }
      
      override public function exists(param1:String, param2:AssetType) : Boolean
      {
         var _loc3_:AssetType = DefaultAssetLibrary.type.get(param1);
         if(_loc3_ != null)
         {
            if(_loc3_ == param2 || (param2 == AssetType.SOUND || param2 == AssetType.MUSIC) && (_loc3_ == AssetType.MUSIC || _loc3_ == AssetType.SOUND))
            {
               return true;
            }
            if((_loc3_ == AssetType.BINARY || _loc3_ == AssetType.TEXT) && param2 == AssetType.BINARY)
            {
               return true;
            }
            if("$" + param1 in DefaultAssetLibrary.path.h)
            {
               return true;
            }
         }
         return false;
      }
   }
}


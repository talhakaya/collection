package
{
   import flash.Boot;
   import flash.media.Sound;
   import flash.media.SoundChannel;
   import flash.media.SoundTransform;
   import haxe.Log;
   import openfl.Assets;
   
   public class SoundManager
   {
      
      public static var instance:SoundManager;
      
      public static var music1:Sound;
      
      public static var music2:Sound;
      
      public static var music3:Sound;
      
      public static var music4:Sound;
      
      public static var music5:Sound;
      
      public static var music6:Sound;
      
      public static var musicMenu1:Sound;
      
      public static var musicMenu2:Sound;
      
      public static var musicMenu3:Sound;
      
      public static var rate_perfect:Sound;
      
      public static var rate_great:Sound;
      
      public static var rate_good:Sound;
      
      public static var rate_ok:Sound;
      
      public static var rate_sad:Sound;
      
      public static var explosion:Sound;
      
      public static var soundTransformMain:SoundTransform;
      
      public static var soundTransformReplay:SoundTransform;
      
      public static var MinSoundRhythmRatio:Number = 0.25;
      
      public static var currentRate:String = "";
      
      public static var currentExplosion:Boolean = false;
      
      public static var musicOn:Boolean = true;
      
      public var soundRhythmCountOld:Number;
      
      public var soundRhythmCount:Number;
      
      public var soundQueue:Array;
      
      public var position:Number;
      
      public var playing:Boolean;
      
      public var currentMusic:Sound;
      
      public var channelSound:SoundChannel;
      
      public var channelMusic:SoundChannel;
      
      public function SoundManager()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         soundQueue = [];
         soundRhythmCount = 0;
         soundRhythmCountOld = 0;
         position = 0;
         playing = false;
         SoundManager.instance = this;
         SoundManager.music1 = Assets.getSound("snd/1.mp3");
         SoundManager.music2 = Assets.getSound("snd/2.mp3");
         SoundManager.music3 = Assets.getSound("snd/3.mp3");
         SoundManager.music4 = Assets.getSound("snd/4.mp3");
         SoundManager.music5 = Assets.getSound("snd/5.mp3");
         SoundManager.music6 = Assets.getSound("snd/6.mp3");
         SoundManager.musicMenu1 = Assets.getSound("snd/menu1.mp3");
         SoundManager.musicMenu2 = Assets.getSound("snd/menu2.mp3");
         SoundManager.musicMenu3 = Assets.getSound("snd/menu3.mp3");
         SoundManager.rate_perfect = Assets.getSound("snd/rate perfect.wav");
         SoundManager.rate_great = Assets.getSound("snd/rate great.wav");
         SoundManager.rate_good = Assets.getSound("snd/rate good.wav");
         SoundManager.rate_ok = Assets.getSound("snd/rate ok.wav");
         SoundManager.rate_sad = Assets.getSound("snd/rate sad.wav");
         SoundManager.explosion = Assets.getSound("snd/explosion.wav");
         SoundManager.soundTransformMain = new SoundTransform(0.05,0);
         SoundManager.soundTransformReplay = new SoundTransform(0.02,0);
      }
      
      public static function update() : void
      {
         SoundManager.instance._update();
      }
      
      public static function changeMusic(param1:String = undefined) : void
      {
         if(param1 == null)
         {
            param1 = "menu1";
         }
         SoundManager.instance._changeMusic(param1);
      }
      
      public static function pauseMusic() : void
      {
         SoundManager.instance._pauseMusic();
      }
      
      public static function playMusic() : void
      {
         SoundManager.instance._playMusic();
      }
      
      public static function skipToMusic(param1:Number) : void
      {
         SoundManager.instance._skipToMusic(param1);
      }
      
      public static function playSound(param1:String, param2:Boolean, param3:Number = 0) : void
      {
         SoundManager.instance._playSound(param1,param2,param3);
      }
      
      public function _update() : void
      {
         var _loc1_:Boolean = false;
         var _loc2_:int = 0;
         var _loc3_:int = 0;
         var _loc4_:int = 0;
         soundRhythmCount = GameManager.time % (GameManager.rhythm * 0.25);
         if(soundRhythmCount < GameManager.rhythm * 0.25 / 2 && soundRhythmCountOld > GameManager.rhythm * 0.25 / 2)
         {
            if(int(soundQueue.length) > 0)
            {
               _loc1_ = false;
               _loc2_ = 0;
               _loc3_ = int(soundQueue.length);
               while(_loc2_ < _loc3_)
               {
                  _loc4_ = _loc2_++;
                  if(soundQueue[_loc4_].sound != SoundManager.explosion)
                  {
                     _loc1_ = true;
                     break;
                  }
               }
               _loc2_ = 0;
               _loc3_ = int(soundQueue.length);
               while(_loc2_ < _loc3_)
               {
                  _loc4_ = _loc2_++;
                  if(soundQueue[_loc4_].sound != SoundManager.explosion || !_loc1_)
                  {
                     soundQueue[_loc4_].soundTransform.pan = soundQueue[_loc4_].panning;
                     soundQueue[_loc4_].sound.play(0,0,soundQueue[_loc4_].soundTransform);
                  }
               }
               soundQueue = [];
            }
         }
         soundRhythmCountOld = soundRhythmCount;
      }
      
      public function _skipToMusic(param1:Number) : void
      {
         _pauseMusic();
         position = param1;
      }
      
      public function _playSound(param1:String, param2:Boolean, param3:Number = 0) : void
      {
         var _loc4_:* = null as SoundTransform;
         if(param2)
         {
            _loc4_ = SoundManager.soundTransformMain;
         }
         else
         {
            _loc4_ = SoundManager.soundTransformReplay;
         }
         var _loc5_:String = param1;
         if(_loc5_ == "perfect")
         {
            soundQueue.push(new SoundSpec(SoundManager.rate_perfect,_loc4_,param3));
         }
         else if(_loc5_ == "great")
         {
            soundQueue.push(new SoundSpec(SoundManager.rate_great,_loc4_,param3));
         }
         else if(_loc5_ == "good")
         {
            soundQueue.push(new SoundSpec(SoundManager.rate_good,_loc4_,param3));
         }
         else if(_loc5_ == "ok")
         {
            soundQueue.push(new SoundSpec(SoundManager.rate_ok,_loc4_,param3));
         }
         else if(_loc5_ == "sad")
         {
            soundQueue.push(new SoundSpec(SoundManager.rate_sad,_loc4_,param3));
         }
         else if(_loc5_ == "explosion")
         {
            soundQueue.push(new SoundSpec(SoundManager.explosion,_loc4_,param3));
         }
      }
      
      public function _playMusic() : void
      {
         if(currentMusic == null)
         {
            Log.trace("no music selected",{
               "fileName":"SoundManager.hx",
               "lineNumber":202,
               "className":"SoundManager",
               "methodName":"_playMusic"
            });
         }
         else
         {
            playing = true;
            channelMusic = currentMusic.play(position);
         }
      }
      
      public function _pauseMusic() : void
      {
         if(playing && channelMusic != null)
         {
            playing = false;
            position = GameManager.time;
            channelMusic.stop();
            channelMusic = null;
         }
      }
      
      public function _changeMusic(param1:String) : void
      {
         if(playing)
         {
            _pauseMusic();
         }
         var _loc2_:String = param1;
         if(_loc2_ == "menu1")
         {
            currentMusic = SoundManager.musicMenu1;
            GameManager.rhythm = 500;
         }
         else if(_loc2_ == "menu2")
         {
            currentMusic = SoundManager.musicMenu2;
            GameManager.rhythm = 500;
         }
         else if(_loc2_ == "menu3")
         {
            currentMusic = SoundManager.musicMenu3;
            GameManager.rhythm = 400;
         }
         else if(_loc2_ == "1")
         {
            currentMusic = SoundManager.music1;
            GameManager.rhythm = 500;
            Scene.numberOfSections = 12;
         }
         else if(_loc2_ == "2")
         {
            currentMusic = SoundManager.music2;
            GameManager.rhythm = 556;
            Scene.numberOfSections = 11;
         }
         else if(_loc2_ == "3")
         {
            currentMusic = SoundManager.music3;
            GameManager.rhythm = 526;
            Scene.numberOfSections = 10;
         }
         else if(_loc2_ == "4")
         {
            currentMusic = SoundManager.music4;
            GameManager.rhythm = 500;
            Scene.numberOfSections = 10;
         }
         else if(_loc2_ == "5")
         {
            currentMusic = SoundManager.music5;
            GameManager.rhythm = 577;
            Scene.numberOfSections = 9;
         }
         else if(_loc2_ == "6")
         {
            currentMusic = SoundManager.music6;
            GameManager.rhythm = 606;
            Scene.numberOfSections = 7;
         }
         position = 0;
         GameManager.time = 0;
      }
   }
}


package
{
   import flash.Boot;
   import flash.net.SharedObject;
   import flash.net.SharedObjectFlushStatus;
   
   public class SaveManager
   {
      
      public static var instance:SaveManager;
      
      public static var playedBefore:Boolean = false;
      
      public var so:SharedObject;
      
      public function SaveManager()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         SaveManager.instance = this;
         so = SharedObject.getLocal("0");
         SaveManager.playedBefore = SaveManager.instance.so.data.playedBefore != null;
         if(so.data.playedBefore == null)
         {
            _save();
         }
         else
         {
            _load();
         }
      }
      
      public static function save() : void
      {
         SaveManager.instance._save();
      }
      
      public static function load() : void
      {
         SaveManager.instance._load();
      }
      
      public function _save() : void
      {
         var _loc3_:* = null;
         var _loc4_:* = null as String;
         so.data.playedBefore = true;
         so.data.scores0 = int(SceneManager.scores[0]);
         so.data.scores1 = int(SceneManager.scores[1]);
         so.data.scores2 = int(SceneManager.scores[2]);
         so.data.scores3 = int(SceneManager.scores[3]);
         so.data.scores4 = int(SceneManager.scores[4]);
         so.data.scores5 = int(SceneManager.scores[5]);
         var _loc2_:String = null;
         try
         {
            _loc2_ = so.flush();
         }
         catch(_loc_e_:*)
         {
            if(_loc2_ != null)
            {
               _loc4_ = _loc2_;
               if(_loc4_ != SharedObjectFlushStatus.PENDING)
               {
                  if(_loc4_ == SharedObjectFlushStatus.FLUSHED)
                  {
                  }
               }
            }
            return;
         }
      }
      
      public function _load() : void
      {
         SceneManager.scores[0] = so.data.scores0;
         SceneManager.scores[1] = so.data.scores1;
         SceneManager.scores[2] = so.data.scores2;
         SceneManager.scores[3] = so.data.scores3;
         SceneManager.scores[4] = so.data.scores4;
         SceneManager.scores[5] = so.data.scores5;
      }
   }
}


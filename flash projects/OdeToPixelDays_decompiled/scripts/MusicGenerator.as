package
{
   import org.flixel.*;
   
   public class MusicGenerator extends FlxSprite
   {
      
      private static var Sfx1:Class = MusicGenerator_Sfx1;
      
      private static var Sfx2:Class = MusicGenerator_Sfx2;
      
      private static var Sfx3:Class = MusicGenerator_Sfx3;
      
      private static var Sfx4:Class = MusicGenerator_Sfx4;
      
      private static var Sfx5:Class = MusicGenerator_Sfx5;
      
      private static var Sfx6:Class = MusicGenerator_Sfx6;
      
      private var hans:Hans;
      
      private var count:int;
      
      private var volume:Number;
      
      public var HIZ:int;
      
      private var yer:int;
      
      private var erey:Array;
      
      public function MusicGenerator(_h:Hans, _Y:Number, hiz:int)
      {
         super();
         this.hans = _h;
         y = _Y;
         this.HIZ = hiz;
         alpha = 0;
      }
      
      override public function update() : void
      {
         super.update();
         if(this.count == 0)
         {
            this.erey = this.createArray();
            this.playNote(this.erey[0]);
         }
         else if(this.count == this.HIZ)
         {
            this.playNote(this.erey[1]);
         }
         else if(this.count == this.HIZ * 2)
         {
            this.playNote(this.erey[2]);
         }
         else if(this.count == this.HIZ * 3)
         {
            this.playNote(this.erey[3]);
         }
         else if(this.count == this.HIZ * 4)
         {
            this.playNote(this.erey[4]);
         }
         else if(this.count == this.HIZ * 5)
         {
            this.playNote(this.erey[5]);
         }
         else if(this.count == this.HIZ * 6 - 1)
         {
            this.count = -1;
         }
         ++this.count;
      }
      
      private function createArray() : Array
      {
         var i:int = 0;
         var j:int = 0;
         var turn:int = 0;
         var random:Number = NaN;
         var temp:Array = new Array(1,2,3,4,5,6);
         var erey:Array = new Array();
         for(turn = 0; turn < 6; turn++)
         {
            i = Math.floor(Math.random() * temp.length);
            erey.push(temp[i]);
            temp.splice(i,1);
         }
         return erey;
      }
      
      private function playNote(i:int) : void
      {
         if(this.hans.y - y > 88)
         {
            this.volume = 0;
         }
         else if(this.hans.y - y > 0)
         {
            this.volume = 1 - (this.hans.y - y) / 88;
         }
         else
         {
            this.volume = 1;
         }
         if(i == 1)
         {
            FlxG.play(Sfx1,this.volume);
         }
         else if(i == 2)
         {
            FlxG.play(Sfx2,this.volume);
         }
         else if(i == 3)
         {
            FlxG.play(Sfx3,this.volume);
         }
         else if(i == 4)
         {
            FlxG.play(Sfx4,this.volume);
         }
         else if(i == 5)
         {
            FlxG.play(Sfx5,this.volume);
         }
         else if(i == 6)
         {
            FlxG.play(Sfx6,this.volume);
         }
      }
   }
}


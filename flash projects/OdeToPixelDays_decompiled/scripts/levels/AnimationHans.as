package levels
{
   import org.flixel.FlxG;
   import org.flixel.FlxGroup;
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   import org.flixel.FlxState;
   
   public class AnimationHans extends FlxState
   {
      
      private static var music:Class = AnimationHans_music;
      
      private static var music2:Class = AnimationHans_music2;
      
      public var hans:HansGetSmaller;
      
      public var hans2:HansGetSmaller2;
      
      public var scale:FlxPoint;
      
      public var smaller:Boolean;
      
      public var timer1:TTimer;
      
      public var timer2:TTimer;
      
      public function AnimationHans(_scale:FlxPoint, _smaller:Boolean)
      {
         super();
         this.scale = _scale;
         this.smaller = _smaller;
      }
      
      override public function create() : void
      {
         var i:int = 0;
         var j:int = 0;
         super.create();
         if(FlxG.music != null)
         {
            FlxG.music.stop();
            FlxG.music = null;
         }
         if(this.smaller)
         {
            FlxG.play(music);
         }
         else
         {
            FlxG.play(music2);
         }
         var bg:FlxGroup = new FlxGroup();
         add(bg);
         if(this.smaller)
         {
            if(this.scale.x == 1)
            {
               FlxG.bgColor = 4289357414;
            }
            else if(this.scale.x == 2)
            {
               FlxG.bgColor = 4288900388;
            }
            else if(this.scale.x == 4)
            {
               FlxG.bgColor = 4285499319;
            }
            else if(this.scale.x == 8)
            {
               FlxG.bgColor = 4290199552;
            }
         }
         else if(this.scale.x == 2)
         {
            FlxG.bgColor = 4288900388;
         }
         else if(this.scale.x == 4)
         {
            FlxG.bgColor = 4285499319;
         }
         else if(this.scale.x == 8)
         {
            FlxG.bgColor = 4290199552;
         }
         else if(this.scale.x == 16)
         {
            FlxG.bgColor = 4281241856;
         }
         var floor:FlxSprite = new FlxSprite(0,200);
         floor.makeGraphic(320,80,4278190080,false);
         add(floor);
         if(this.smaller)
         {
            this.hans = new HansGetSmaller(0,0,this.scale,this.smaller);
         }
         else
         {
            this.hans = new HansGetSmaller(0,0,this.scale,this.smaller);
         }
         add(this.hans);
         var a:int = Math.ceil(40 / 8 / this.hans.visualScale.x) + 1;
         var b:int = Math.ceil(30 / 8 / this.hans.visualScale.x) + 1;
         for(i = 0; i < a; i++)
         {
            for(j = 0; j < b; j++)
            {
               bg.add(new Bg(64 * this.hans.visualScale.x * i,64 * this.hans.visualScale.x * j,this.hans.visualScale));
            }
         }
         this.timer1 = new TTimer();
         add(this.timer1);
         this.timer1.start(30);
      }
      
      override public function update() : void
      {
         super.update();
         this.hans.x = 160 - this.hans.width * 0.5;
         if(this.hans.count < 100)
         {
            if(this.smaller)
            {
               this.hans.y = 200 - this.hans.height + this.hans.count * 0.48;
            }
            else
            {
               this.hans.y = 200 - this.hans.height - this.hans.count * 0.24;
            }
         }
         if(this.hans.putHansGetSmaller2)
         {
            if(this.smaller)
            {
               this.hans2 = new HansGetSmaller2(160 - this.hans.width * 0.25,200 - this.hans.height * 0.5,this.scale,this.smaller);
            }
            else
            {
               this.hans2 = new HansGetSmaller2(160 - this.hans.width,200 - this.hans.height * 2,this.scale,this.smaller);
            }
            add(this.hans2);
         }
         if(this.hans.goToNextLevel)
         {
            if(this.smaller)
            {
               if(this.scale.x == 1)
               {
                  FlxG.switchState(new Level5());
               }
               else if(this.scale.x == 2)
               {
                  FlxG.switchState(new Level9());
               }
               else if(this.scale.x == 4)
               {
                  FlxG.switchState(new Level13());
               }
               else if(this.scale.x == 8)
               {
                  FlxG.switchState(new Level17());
               }
            }
            else if(this.scale.x == 16)
            {
               FlxG.switchState(new Level24());
            }
            else if(this.scale.x == 8)
            {
               FlxG.switchState(new Level31());
            }
            else if(this.scale.x == 4)
            {
               FlxG.switchState(new Level35());
            }
            else if(this.scale.x == 2)
            {
               FlxG.switchState(new Level41());
            }
         }
         if(this.timer1.complete)
         {
            this.hans.startGettingSmaller();
         }
      }
   }
}


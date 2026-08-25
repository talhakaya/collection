package
{
   import org.flixel.*;
   
   public class PatternMatcher extends FlxSprite
   {
      
      private static var Sfxdogru:Class = PatternMatcher_Sfxdogru;
      
      private static var Sfxyanlis:Class = PatternMatcher_Sfxyanlis;
      
      private var pattern1Complete:Boolean;
      
      private var pattern2Complete:Boolean;
      
      private var pattern3Complete:Boolean;
      
      private var pattern4Complete:Boolean;
      
      private var pattern5Complete:Boolean;
      
      private var pattern6Complete:Boolean;
      
      private const TIME:Number = 1.5;
      
      private var timer1:FlxTimer;
      
      private var timer2:FlxTimer;
      
      private var timer3:FlxTimer;
      
      private var timer4:FlxTimer;
      
      private var timer5:FlxTimer;
      
      private var timer6:FlxTimer;
      
      private var timer7:FlxTimer;
      
      private var timer8:FlxTimer;
      
      private var lightbulb1:Lightbulb;
      
      private var lightbulb2:Lightbulb;
      
      private var lightbulb3:Lightbulb;
      
      private var lightbulb4:Lightbulb;
      
      public var asking:Boolean;
      
      public var answering:Boolean;
      
      public var timer7set:Boolean;
      
      private var patternAsked:Array;
      
      private var answer:Array;
      
      private var rightAnswerGiven:Boolean;
      
      public var kutuFirlat:Boolean;
      
      public var canavarGonder:Boolean;
      
      public var yanlisBildin:Boolean;
      
      public var dogruBildin:Boolean;
      
      public var cevapver:Boolean;
      
      private var boss:MonsterHans;
      
      public function PatternMatcher(_1:Boolean, _2:Boolean, _3:Boolean, _4:Boolean, _5:Boolean, _6:Boolean, _bulb1:Lightbulb, _bulb2:Lightbulb, _bulb3:Lightbulb, _bulb4:Lightbulb, _b:MonsterHans)
      {
         super();
         alpha = 0;
         this.pattern1Complete = _1;
         this.pattern2Complete = _2;
         this.pattern3Complete = _3;
         this.pattern4Complete = _4;
         this.pattern5Complete = _5;
         this.pattern6Complete = _6;
         this.lightbulb1 = _bulb1;
         this.lightbulb2 = _bulb2;
         this.lightbulb3 = _bulb3;
         this.lightbulb4 = _bulb4;
         this.boss = _b;
         this.timer1 = new FlxTimer();
         this.timer2 = new FlxTimer();
         this.timer3 = new FlxTimer();
         this.timer4 = new FlxTimer();
         this.timer5 = new FlxTimer();
         this.timer6 = new FlxTimer();
         this.timer7 = new FlxTimer();
         this.timer8 = new FlxTimer();
         this.timer7set = false;
         this.asking = false;
         this.answering = false;
         this.yanlisBildin = false;
         this.dogruBildin = false;
         this.canavarGonder = false;
         this.cevapver = false;
         this.answer = new Array();
         this.patternAsked = new Array();
      }
      
      override public function update() : void
      {
         super.update();
         if(!this.asking && this.answering)
         {
            if(this.lightbulb1.justOpened)
            {
               this.answer.push(1);
            }
            else if(this.lightbulb2.justOpened)
            {
               this.answer.push(2);
            }
            else if(this.lightbulb3.justOpened)
            {
               this.answer.push(3);
            }
            else if(this.lightbulb4.justOpened)
            {
               this.answer.push(4);
            }
            if(this.answer.length > 0 && this.answer.length <= this.patternAsked.length)
            {
               this.timer8.start(this.TIME,1,this.checkAnswer);
               if(this.answer.length == this.patternAsked.length)
               {
                  this.answering = false;
               }
            }
         }
      }
      
      private function checkAnswer(_t:FlxTimer) : void
      {
         var i:int = 0;
         this.rightAnswerGiven = true;
         for(i = 0; i < this.answer.length; i++)
         {
            if(this.answer[i] != this.patternAsked[i])
            {
               this.rightAnswerGiven = false;
            }
         }
         if(this.rightAnswerGiven)
         {
            if(this.answer.length == this.patternAsked.length)
            {
               FlxG.play(Sfxdogru);
               this.dogruBildin = true;
               this.canavarGonder = true;
               this.answering = false;
               this.kutuFirlat = true;
               if(!this.pattern1Complete)
               {
                  this.pattern1Complete = true;
               }
               else if(!this.pattern2Complete)
               {
                  this.pattern2Complete = true;
               }
               else if(!this.pattern3Complete)
               {
                  this.pattern3Complete = true;
               }
               else if(!this.pattern4Complete)
               {
                  this.pattern4Complete = true;
               }
               else if(!this.pattern5Complete)
               {
                  this.pattern5Complete = true;
               }
               else if(!this.pattern6Complete)
               {
                  this.pattern6Complete = true;
                  this.canavarGonder = false;
               }
               this.timer8 = null;
               this.timer8 = new FlxTimer();
               this.timer8.start(this.TIME * 1,1,this.goOn);
               this.answer = new Array();
            }
         }
         else
         {
            FlxG.play(Sfxyanlis);
            this.yanlisBildin = true;
            this.answering = false;
            this.answer = new Array();
         }
      }
      
      private function goOn(_t:FlxTimer) : void
      {
         this.start();
      }
      
      public function start() : void
      {
         if(!this.asking)
         {
            this.asking = true;
            this.answering = false;
            if(!this.pattern1Complete)
            {
               this.pattern(this.patternAsked = [2]);
            }
            else if(!this.pattern2Complete)
            {
               this.pattern(this.patternAsked = [1,3]);
            }
            else if(!this.pattern3Complete)
            {
               this.pattern(this.patternAsked = [2,3,4]);
            }
            else if(!this.pattern4Complete)
            {
               this.pattern(this.patternAsked = [1,3,2,4]);
            }
            else if(!this.pattern5Complete)
            {
               this.pattern(this.patternAsked = [1,2,1,3,4]);
            }
            else if(!this.pattern6Complete)
            {
               this.pattern(this.patternAsked = [3,4,1,2,4,2]);
            }
         }
      }
      
      private function pattern(_array:Array) : void
      {
         this.timer7set = false;
         if(_array.length > 0)
         {
            if(_array[0] == 1)
            {
               this.timer1.start(this.TIME,1,this.light1);
            }
            else if(_array[0] == 2)
            {
               this.timer1.start(this.TIME,1,this.light2);
            }
            else if(_array[0] == 3)
            {
               this.timer1.start(this.TIME,1,this.light3);
            }
            else if(_array[0] == 4)
            {
               this.timer1.start(this.TIME,1,this.light4);
            }
         }
         else if(!this.timer7set)
         {
            this.timer7set = true;
            this.timer7.start(1,1,this.asked);
         }
         if(_array.length > 1)
         {
            if(_array[1] == 1)
            {
               this.timer2.start(this.TIME * 2,1,this.light1);
            }
            else if(_array[1] == 2)
            {
               this.timer2.start(this.TIME * 2,1,this.light2);
            }
            else if(_array[1] == 3)
            {
               this.timer2.start(this.TIME * 2,1,this.light3);
            }
            else if(_array[1] == 4)
            {
               this.timer2.start(this.TIME * 2,1,this.light4);
            }
         }
         else if(!this.timer7set)
         {
            this.timer7set = true;
            this.timer7.start(this.TIME + 1,1,this.asked);
         }
         if(_array.length > 2)
         {
            if(_array[2] == 1)
            {
               this.timer3.start(this.TIME * 3,1,this.light1);
            }
            else if(_array[2] == 2)
            {
               this.timer3.start(this.TIME * 3,1,this.light2);
            }
            else if(_array[2] == 3)
            {
               this.timer3.start(this.TIME * 3,1,this.light3);
            }
            else if(_array[2] == 4)
            {
               this.timer3.start(this.TIME * 3,1,this.light4);
            }
         }
         else if(!this.timer7set)
         {
            this.timer7set = true;
            this.timer7.start(this.TIME * 2 + 1,1,this.asked);
         }
         if(_array.length > 3)
         {
            if(_array[3] == 1)
            {
               this.timer4.start(this.TIME * 4,1,this.light1);
            }
            else if(_array[3] == 2)
            {
               this.timer4.start(this.TIME * 4,1,this.light2);
            }
            else if(_array[3] == 3)
            {
               this.timer4.start(this.TIME * 4,1,this.light3);
            }
            else if(_array[3] == 4)
            {
               this.timer4.start(this.TIME * 4,1,this.light4);
            }
         }
         else if(!this.timer7set)
         {
            this.timer7set = true;
            this.timer7.start(this.TIME * 3 + 1,1,this.asked);
         }
         if(_array.length > 4)
         {
            if(_array[4] == 1)
            {
               this.timer5.start(this.TIME * 5,1,this.light1);
            }
            else if(_array[4] == 2)
            {
               this.timer5.start(this.TIME * 5,1,this.light2);
            }
            else if(_array[4] == 3)
            {
               this.timer5.start(this.TIME * 5,1,this.light3);
            }
            else if(_array[4] == 4)
            {
               this.timer5.start(this.TIME * 5,1,this.light4);
            }
         }
         else if(!this.timer7set)
         {
            this.timer7set = true;
            this.timer7.start(this.TIME * 4 + 1,1,this.asked);
         }
         if(_array.length > 5)
         {
            if(_array[5] == 1)
            {
               this.timer6.start(this.TIME * 6,1,this.light1);
            }
            else if(_array[5] == 2)
            {
               this.timer6.start(this.TIME * 6,1,this.light2);
            }
            else if(_array[5] == 3)
            {
               this.timer6.start(this.TIME * 6,1,this.light3);
            }
            else if(_array[5] == 4)
            {
               this.timer6.start(this.TIME * 5,1,this.light4);
            }
         }
         else if(!this.timer7set)
         {
            this.timer7set = true;
            this.timer7.start(this.TIME * 5 + 1,1,this.asked);
         }
         if(!this.timer7set)
         {
            this.timer7set = true;
            this.timer7.start(this.TIME * 6 + 1,1,this.asked);
         }
      }
      
      private function light1(_t:FlxTimer) : void
      {
         this.lightbulb1.light();
      }
      
      private function light2(_t:FlxTimer) : void
      {
         this.lightbulb2.light();
      }
      
      private function light3(_t:FlxTimer) : void
      {
         this.lightbulb3.light();
      }
      
      private function light4(_t:FlxTimer) : void
      {
         this.lightbulb4.light();
      }
      
      private function asked(_t:FlxTimer) : void
      {
         this.asking = false;
         this.answering = true;
         this.cevapver = true;
      }
   }
}


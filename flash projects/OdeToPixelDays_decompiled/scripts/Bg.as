package
{
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   import org.flixel.FlxTimer;
   
   public class Bg extends FlxSprite
   {
      
      private static var S_1:Class = Bg_S_1;
      
      private static var S_2:Class = Bg_S_2;
      
      private static var S_3:Class = Bg_S_3;
      
      private static var S_4:Class = Bg_S_4;
      
      private static var S_5:Class = Bg_S_5;
      
      private static var S_6:Class = Bg_S_6;
      
      private static var S_7:Class = Bg_S_7;
      
      private static var S_8:Class = Bg_S_8;
      
      private static var S_9:Class = Bg_S_9;
      
      private static var S_10:Class = Bg_S_10;
      
      private static var S_11:Class = Bg_S_11;
      
      private static var S_12:Class = Bg_S_12;
      
      private static var S_13:Class = Bg_S_13;
      
      private static var S_14:Class = Bg_S_14;
      
      private static var S_15:Class = Bg_S_15;
      
      private static var S_16:Class = Bg_S_16;
      
      private static var S_hans:Class = Bg_S_hans;
      
      private static var S_17:Class = Bg_S_17;
      
      public var birdFlied:Boolean;
      
      public var windEsti:Boolean;
      
      public var timer:FlxTimer;
      
      public var timerSet:Boolean;
      
      public function Bg(_X:Number, _Y:Number, _scale:FlxPoint)
      {
         super();
         x = _X;
         y = _Y;
         scale = _scale;
         loadGraphic(S_1,false,false,64,64,false);
         this.timerSet = false;
      }
      
      public function torch() : void
      {
         loadGraphic(S_2,true,false,64,64,false);
         var random:Number = Math.random();
         if(random < 0.25)
         {
            addAnimation("0",[0,0,0,0,1,1,1,1],9,true);
            play("0");
         }
         else if(random < 0.5)
         {
            addAnimation("1",[0,0,0,1,1,1,1,0],10,true);
            play("1");
         }
         else if(random < 0.75)
         {
            addAnimation("2",[0,0,1,1,1,1,0,0],11,true);
            play("2");
         }
         else
         {
            addAnimation("3",[0,1,1,1,1,0,0,0],12,true);
            play("3");
         }
      }
      
      public function windowPalm() : void
      {
         loadGraphic(S_4,true,false,64,64,false);
         addAnimation("4",[0,1,2,1],1,true);
         play("4");
      }
      
      public function damaged() : void
      {
         loadGraphic(S_3,true,false,64,64,false);
         var random:Number = Math.random();
         if(random < 0.25)
         {
            addAnimation("0",[0],0,false);
            play("0");
         }
         else if(random < 0.5)
         {
            addAnimation("1",[1],0,false);
            play("1");
         }
         else if(random < 0.75)
         {
            addAnimation("2",[2],0,false);
            play("2");
         }
         else
         {
            addAnimation("3",[3],0,false);
            play("3");
         }
      }
      
      public function sun00() : void
      {
         loadGraphic(S_5,true,false,64,64,false);
         addAnimation("00",[0,4],0.5,true);
         play("00");
      }
      
      public function sun01() : void
      {
         loadGraphic(S_5,true,false,64,64,false);
         addAnimation("01",[2,6],0.5,true);
         play("01");
      }
      
      public function sun10() : void
      {
         loadGraphic(S_5,true,false,64,64,false);
         addAnimation("10",[1,5],0.5,true);
         play("10");
      }
      
      public function sun11() : void
      {
         loadGraphic(S_5,true,false,64,64,false);
         addAnimation("11",[3,7],0.5,true);
         play("11");
      }
      
      public function bird() : void
      {
         loadGraphic(S_6,true,false,64,64,false);
         addAnimation("bird",[0,1,2,3,4,5,6,7,7,7,7,7,7,7,7,7,7,7,7,7],2,true);
         play("bird");
      }
      
      public function windowSmall() : void
      {
         loadGraphic(S_7,true,false,64,64,false);
         var random:Number = Math.random();
         if(random < 0.25)
         {
            addAnimation("0",[0],0,false);
            play("0");
         }
         else if(random < 0.5)
         {
            addAnimation("1",[1],0,false);
            play("1");
         }
         else if(random < 0.75)
         {
            addAnimation("2",[2],0,false);
            play("2");
         }
         else
         {
            addAnimation("3",[3],0,false);
            play("3");
         }
      }
      
      public function bird2() : void
      {
         loadGraphic(S_15,true,false,64,64,false);
         var random:Number = Math.random();
         addAnimation("0",[0],0,false);
         addAnimation("fly",[1,2,3,4,5],3,false);
         this.birdFlied = false;
         play("0");
      }
      
      public function windowBig00() : void
      {
         loadGraphic(S_13,true,false,64,64,false);
         addAnimation("00",[0],0,false);
         play("00");
      }
      
      public function windowBig02() : void
      {
         loadGraphic(S_13,true,false,64,64,false);
         addAnimation("02",[2],0,false);
         play("02");
      }
      
      public function windowBig20() : void
      {
         loadGraphic(S_13,true,false,64,64,false);
         addAnimation("20",[1],0,false);
         play("20");
      }
      
      public function windowBig22() : void
      {
         loadGraphic(S_13,true,false,64,64,false);
         addAnimation("22",[3],0,false);
         play("22");
      }
      
      public function windowBig10() : void
      {
         loadGraphic(S_11,true,false,64,64,false);
         var random:Number = Math.random();
         if(random < 0.25)
         {
            addAnimation("0",[0],0,false);
            play("0");
         }
         else if(random < 0.5)
         {
            addAnimation("1",[1],0,false);
            play("1");
         }
         else if(random < 0.75)
         {
            addAnimation("2",[2],0,false);
            play("2");
         }
         else
         {
            addAnimation("3",[3],0,false);
            play("3");
         }
      }
      
      public function windowBig01() : void
      {
         loadGraphic(S_10,true,false,64,64,false);
         var random:Number = Math.random();
         if(random < 0.25)
         {
            addAnimation("0",[0],0,false);
            play("0");
         }
         else if(random < 0.5)
         {
            addAnimation("1",[1],0,false);
            play("1");
         }
         else if(random < 0.75)
         {
            addAnimation("2",[2],0,false);
            play("2");
         }
         else
         {
            addAnimation("3",[3],0,false);
            play("3");
         }
      }
      
      public function windowBig12() : void
      {
         loadGraphic(S_9,true,false,64,64,false);
         var random:Number = Math.random();
         if(random < 0.25)
         {
            addAnimation("0",[0],0,false);
            play("0");
         }
         else if(random < 0.5)
         {
            addAnimation("1",[1],0,false);
            play("1");
         }
         else if(random < 0.75)
         {
            addAnimation("2",[2],0,false);
            play("2");
         }
         else
         {
            addAnimation("3",[3],0,false);
            play("3");
         }
      }
      
      public function windowBig21() : void
      {
         loadGraphic(S_12,true,false,64,64,false);
         var random:Number = Math.random();
         if(random < 0.25)
         {
            addAnimation("0",[0],0,false);
            play("0");
         }
         else if(random < 0.5)
         {
            addAnimation("1",[1],0,false);
            play("1");
         }
         else if(random < 0.75)
         {
            addAnimation("2",[2],0,false);
            play("2");
         }
         else
         {
            addAnimation("3",[3],0,false);
            play("3");
         }
      }
      
      public function windowBig11() : void
      {
         loadGraphic(S_14,true,false,64,64,false);
         var random:Number = Math.random();
         if(random < 0.25)
         {
            addAnimation("0",[0],0,false);
            play("0");
         }
         else if(random < 0.5)
         {
            addAnimation("1",[1],0,false);
            play("1");
         }
         else if(random < 0.75)
         {
            addAnimation("2",[2],0,false);
            play("2");
         }
         else
         {
            addAnimation("3",[3],0,false);
            play("3");
         }
      }
      
      public function greyCloud() : void
      {
         loadGraphic(S_17,true,false,64,64,false);
         var random:Number = Math.random();
         if(random < 0.25)
         {
            addAnimation("0",[0],0,false);
            play("0");
         }
         else if(random < 0.5)
         {
            addAnimation("1",[1],0,false);
            play("1");
         }
         else if(random < 0.75)
         {
            addAnimation("2",[2],0,false);
            play("2");
         }
         else
         {
            addAnimation("3",[3],0,false);
            play("3");
         }
      }
      
      public function corner() : void
      {
         loadGraphic(S_16,true,false,64,64,false);
         addAnimation("0",[0],1,true);
         play("0");
      }
      
      public function wind() : void
      {
         if(!this.timerSet)
         {
            this.timerSet = true;
            this.timer = new FlxTimer();
         }
         this.windEsti = true;
         this.timer.start(4,1,this.windEsmedi);
      }
      
      private function windEsmedi(a:FlxTimer) : void
      {
         this.windEsti = false;
      }
      
      public function hans() : void
      {
         loadGraphic(S_hans,true,false,64,64,false);
         addAnimation("0",[0],1,true);
         play("0");
      }
   }
}


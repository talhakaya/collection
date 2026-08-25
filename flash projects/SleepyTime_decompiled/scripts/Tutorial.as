package
{
   import flash.Boot;
   import flash.display.BitmapData;
   import openfl.Assets;
   
   public class Tutorial extends Citmap
   {
      
      public var whiteLine:Citmap;
      
      public var spike3:Citmap;
      
      public var spike2:Citmap;
      
      public var spike1:Citmap;
      
      public var spaceBar2:Citmap;
      
      public var spaceBar:Citmap;
      
      public var mouse:Citmap;
      
      public var crosshair:Citmap;
      
      public function Tutorial()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         super(Assets.getBitmapData("img/scene1/line1.png"),50,2);
         rotating = false;
         bitmap.scaleX = 2;
         spike1 = new Citmap(Assets.getBitmapData("img/spike.png"),4,30);
         spike1.alpha = 0;
         addChild(spike1);
         spike2 = new Citmap(Assets.getBitmapData("img/spike.png"),4,30);
         spike2.alpha = 0;
         addChild(spike2);
         spike3 = new Citmap(Assets.getBitmapData("img/spike.png"),4,30);
         spike3.alpha = 0;
         addChild(spike3);
         whiteLine = new Citmap(Assets.getBitmapData("img/line.png"),50,8);
         whiteLine.y = -4;
         whiteLine.scaleY = 0.5;
         whiteLine.alpha = 0;
         addChild(whiteLine);
         spaceBar = new Citmap(Assets.getBitmapData("img/tutorialSpace.png"),0,0);
         spaceBar.x = -40;
         spaceBar.y = 40;
         spaceBar.scaleX = spaceBar.scaleY = 4;
         spaceBar.alpha = 0;
         addChild(spaceBar);
         spaceBar2 = new Citmap(Assets.getBitmapData("img/tutorialSpace2.png"),0,0);
         spaceBar2.x = -40;
         spaceBar2.y = 40;
         spaceBar2.scaleX = spaceBar2.scaleY = 4;
         spaceBar2.alpha = 0;
         addChild(spaceBar2);
         mouse = new Citmap(Assets.getBitmapData("img/tutorialMouse.png"),0,0);
         mouse.y = 50;
         mouse.scaleX = mouse.scaleY = 4;
         mouse.alpha = 0;
         addChild(mouse);
         crosshair = new Citmap(Assets.getBitmapData("img/crosshair1.png"),25,25);
         addChild(crosshair);
         GameManager.time = 0;
      }
      
      override public function update() : void
      {
         var _loc3_:Number = NaN;
         var _loc1_:int = int(GameManager.time % 4000);
         var _loc2_:Boolean = false;
         if(_loc1_ < 1000)
         {
            _loc2_ = false;
            spike1.alpha = 1;
            spike2.alpha = 0;
            spike3.alpha = 0;
            whiteLine.alpha = 0;
            spike1.x = 125 - _loc1_ / 8;
            spike1.scaleX = spike1.scaleY = 1;
            spike2.scaleX = spike2.scaleY = 1;
            spike3.scaleX = spike3.scaleY = 1;
            whiteLine.scaleY = 0.5;
         }
         else if(_loc1_ < 1250)
         {
            _loc2_ = true;
            spike1.alpha = 1;
            spike2.alpha = 0;
            spike3.alpha = 0;
            whiteLine.alpha = 0;
            spike1.x = 125 - _loc1_ / 8;
            spike1.scaleX = spike1.scaleY = 4 * ((1250 - _loc1_) / 250);
         }
         else if(_loc1_ < 2000)
         {
            _loc2_ = false;
            spike1.alpha = 0;
            spike2.alpha = 0;
            spike3.alpha = 0;
            whiteLine.alpha = 0;
         }
         else if(_loc1_ < 3000)
         {
            _loc2_ = false;
            spike1.alpha = 0;
            spike2.alpha = 1;
            spike2.x = 375 - _loc1_ / 8;
            spike3.alpha = 1;
            spike3.x = 437.5 - _loc1_ / 8;
            whiteLine.alpha = 1;
            whiteLine.x = 375 - _loc1_ / 8 + 31.25;
            whiteLine.scaleX = 0.625;
         }
         else if(_loc1_ < 3250)
         {
            _loc2_ = true;
            spike1.alpha = 0;
            spike2.alpha = 1;
            spike2.x = 375 - _loc1_ / 8;
            spike2.scaleX = spike2.scaleY = 4 * ((3250 - _loc1_) / 250);
            spike3.alpha = 1;
            spike3.x = 437.5 - _loc1_ / 8;
            whiteLine.alpha = 1;
            whiteLine.x = 375 - _loc1_ / 8 + 31.25;
            whiteLine.scaleY = 2 * ((3500 - _loc1_) / 500);
            whiteLine.scaleX = 0.625;
         }
         else if(_loc1_ < 3500)
         {
            _loc2_ = true;
            spike1.alpha = 0;
            spike2.alpha = 0;
            spike3.alpha = 1;
            spike3.x = 437.5 - _loc1_ / 8;
            whiteLine.alpha = 1;
            whiteLine.x = 375 - _loc1_ / 8 + 31.25;
            whiteLine.scaleY = 2 * ((3500 - _loc1_) / 500);
            whiteLine.scaleX = 0.625;
         }
         else if(_loc1_ < 3750)
         {
            _loc2_ = false;
            spike1.alpha = 0;
            spike2.alpha = 0;
            spike3.alpha = 1;
            spike3.x = 437.5 - _loc1_ / 8;
            spike3.scaleX = spike3.scaleY = 4 * ((3750 - _loc1_) / 250);
            whiteLine.alpha = 0;
         }
         else
         {
            _loc2_ = false;
            spike1.alpha = 0;
            spike2.alpha = 0;
            spike3.alpha = 0;
            whiteLine.alpha = 0;
         }
         if(_loc2_)
         {
            mouse.alpha = 1;
            spaceBar2.alpha = 1;
            spaceBar.alpha = 0;
            crosshair.scaleX = crosshair.scaleY = 2;
         }
         else
         {
            mouse.alpha = 0;
            spaceBar2.alpha = 0;
            spaceBar.alpha = 1;
            crosshair.scaleX = crosshair.scaleY = 1;
         }
      }
   }
}


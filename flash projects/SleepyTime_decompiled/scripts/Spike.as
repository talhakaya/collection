package
{
   import flash.Boot;
   import flash.display.BitmapData;
   import flash.display.Sprite;
   import openfl.Assets;
   
   public class Spike extends Sprite
   {
      
      public var secondSpike:Citmap;
      
      public var sceneId:int;
      
      public var line:Citmap;
      
      public var lengthOfPress:int;
      
      public var isOnePress:Boolean;
      
      public var idInTrack:int;
      
      public var firstSpike:Citmap;
      
      public function Spike(param1:int = 0, param2:int = 0, param3:int = 0)
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         super();
         firstSpike = new Citmap(Assets.getBitmapData("img/spike.png"),4,30);
         addChild(firstSpike);
         lengthOfPress = param2;
         sceneId = param3;
         if(param2 != 0)
         {
            secondSpike = new Citmap(Assets.getBitmapData("img/spike.png"),4,30);
            secondSpike.x = firstSpike.x + lengthOfPress / GameManager.rhythm * Scene.lengthOfSection;
            addChild(secondSpike);
            line = new Citmap(Assets.getBitmapData("img/line.png"),0,7);
            line.scaleX = lengthOfPress / 100 / GameManager.rhythm * Scene.lengthOfSection;
            line.scaleY = 0.5;
            addChild(line);
         }
         idInTrack = param1;
      }
   }
}


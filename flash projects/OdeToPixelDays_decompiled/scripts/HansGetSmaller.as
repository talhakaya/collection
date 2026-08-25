package
{
   import org.flixel.FlxG;
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   
   public class HansGetSmaller extends FlxSprite
   {
      
      private static var S_hans:Class = HansGetSmaller_S_hans;
      
      private static var S_hans16:Class = HansGetSmaller_S_hans16;
      
      private static var S_hans8:Class = HansGetSmaller_S_hans8;
      
      private static var S_hans4:Class = HansGetSmaller_S_hans4;
      
      private static var S_hans2:Class = HansGetSmaller_S_hans2;
      
      private static var S_hans1:Class = HansGetSmaller_S_hans1;
      
      public var goToNextLevel:Boolean;
      
      public var putHansGetSmaller2:Boolean;
      
      private var animate:Boolean;
      
      public var smaller:Boolean;
      
      public var count:int;
      
      private var countSlower:int;
      
      public var initialScale:FlxPoint;
      
      public var visualScale:FlxPoint;
      
      public function HansGetSmaller(_X:Number, _Y:Number, _scale:FlxPoint, _smaller:Boolean)
      {
         super();
         x = _X;
         y = _Y;
         if(_smaller)
         {
            this.visualScale = new FlxPoint(_scale.x * 6,_scale.x * 6);
         }
         else
         {
            this.visualScale = new FlxPoint(_scale.x * 3,_scale.x * 3);
         }
         scale = this.visualScale;
         this.smaller = _smaller;
         this.initialScale = _scale;
         this.count = 0;
         this.countSlower = 0;
         if(this.initialScale.x == 1)
         {
            loadGraphic(S_hans,false,true,16,32,false);
         }
         else if(this.initialScale.x == 2)
         {
            loadGraphic(S_hans16,false,true,10,16,false);
         }
         else if(this.initialScale.x == 4)
         {
            loadGraphic(S_hans8,false,true,5,8,false);
         }
         else if(this.initialScale.x == 8)
         {
            loadGraphic(S_hans4,false,true,4,4,false);
         }
         else if(this.initialScale.x == 16)
         {
            loadGraphic(S_hans2,false,true,1,2,false);
         }
         if(this.smaller)
         {
            height = 192;
            width = 96;
         }
         else
         {
            height = 96;
            width = 48;
         }
         centerOffsets();
      }
      
      override public function update() : void
      {
         super.update();
         if(FlxG.keys.justPressed("ENTER"))
         {
            this.goToNextLevel = true;
         }
         if(FlxG.keys.justPressed("SPACE"))
         {
            this.goToNextLevel = true;
         }
         if(this.animate)
         {
            ++this.countSlower;
            if(this.countSlower > 1)
            {
               ++this.count;
               this.getSmaller();
               this.countSlower = 0;
            }
         }
      }
      
      public function getSmaller() : void
      {
         if(this.count < 101)
         {
            if(this.smaller)
            {
               scale = new FlxPoint(this.visualScale.x * (1 - this.count * 0.005),this.visualScale.y * (1 - this.count * 0.005));
            }
            else
            {
               scale = new FlxPoint(this.visualScale.x * (1 + this.count * 0.005),this.visualScale.y * (1 + this.count * 0.005));
            }
            if(this.count == 90)
            {
               this.putHansGetSmaller2 = true;
            }
            else if(this.count > 90)
            {
               alpha -= 0.1;
               this.putHansGetSmaller2 = false;
            }
         }
         else if(this.count == 200)
         {
            this.goToNextLevel = true;
         }
      }
      
      public function startGettingSmaller() : void
      {
         this.animate = true;
      }
   }
}


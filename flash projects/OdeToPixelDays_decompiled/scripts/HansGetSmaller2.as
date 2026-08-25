package
{
   import org.flixel.FlxPoint;
   import org.flixel.FlxSprite;
   
   public class HansGetSmaller2 extends FlxSprite
   {
      
      private static var S_hans:Class = HansGetSmaller2_S_hans;
      
      private static var S_hans16:Class = HansGetSmaller2_S_hans16;
      
      private static var S_hans8:Class = HansGetSmaller2_S_hans8;
      
      private static var S_hans4:Class = HansGetSmaller2_S_hans4;
      
      private static var S_hans2:Class = HansGetSmaller2_S_hans2;
      
      private static var S_hans1:Class = HansGetSmaller2_S_hans1;
      
      public var goToNextLevel:Boolean;
      
      private var animate:Boolean;
      
      public var smaller:Boolean;
      
      public var count:int;
      
      private var countSlower:int;
      
      public var initialScale:FlxPoint;
      
      public var visualScale:FlxPoint;
      
      public function HansGetSmaller2(_X:Number, _Y:Number, _scale:FlxPoint, _smaller:Boolean)
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
         alpha = 0;
         scale = this.visualScale;
         if(this.smaller)
         {
            if(this.initialScale.x == 1)
            {
               loadGraphic(S_hans16,false,true,10,16,false);
            }
            else if(this.initialScale.x == 2)
            {
               loadGraphic(S_hans8,false,true,5,8,false);
            }
            else if(this.initialScale.x == 4)
            {
               loadGraphic(S_hans4,false,true,4,4,false);
            }
            else if(this.initialScale.x == 8)
            {
               loadGraphic(S_hans2,false,true,1,2,false);
            }
         }
         else if(this.initialScale.x == 2)
         {
            loadGraphic(S_hans,false,true,16,32,false);
         }
         else if(this.initialScale.x == 4)
         {
            loadGraphic(S_hans16,false,true,10,16,false);
         }
         else if(this.initialScale.x == 8)
         {
            loadGraphic(S_hans8,false,true,5,8,false);
         }
         else if(this.initialScale.x == 16)
         {
            loadGraphic(S_hans4,false,false,4,4,false);
         }
         if(this.smaller)
         {
            height = 96;
            width = 48;
         }
         else
         {
            height = 192;
            width = 96;
         }
         centerOffsets();
      }
      
      override public function update() : void
      {
         super.update();
         if(alpha < 1)
         {
            ++this.countSlower;
            if(this.countSlower > 1)
            {
               this.countSlower = 0;
               alpha += 0.05;
            }
         }
      }
   }
}


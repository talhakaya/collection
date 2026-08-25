package
{
   import org.flixel.FlxSprite;
   
   public class Monster extends FlxSprite
   {
      
      public var isDead:Boolean;
      
      public var isDeadOnce:Boolean;
      
      public var isWalkin:Boolean;
      
      public var isLeft:Boolean;
      
      public var isJumpin:Boolean;
      
      public var randomNumber:Number;
      
      public function Monster()
      {
         super();
         this.isDead = false;
      }
   }
}


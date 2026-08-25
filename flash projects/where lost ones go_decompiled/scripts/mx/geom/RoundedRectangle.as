package mx.geom
{
   import flash.geom.Rectangle;
   import mx.core.mx_internal;
   
   use namespace mx_internal;
   
   public class RoundedRectangle extends Rectangle
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      [Inspectable]
      public var cornerRadius:Number = 0;
      
      public function RoundedRectangle(x:Number = 0, y:Number = 0, width:Number = 0, height:Number = 0, cornerRadius:Number = 0)
      {
         super(x,y,width,height);
         this.cornerRadius = cornerRadius;
      }
   }
}


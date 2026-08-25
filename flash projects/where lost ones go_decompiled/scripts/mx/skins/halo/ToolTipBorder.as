package mx.skins.halo
{
   import flash.display.Graphics;
   import flash.filters.DropShadowFilter;
   import mx.core.EdgeMetrics;
   import mx.core.mx_internal;
   import mx.graphics.RectangularDropShadow;
   import mx.skins.RectangularBorder;
   
   use namespace mx_internal;
   
   public class ToolTipBorder extends RectangularBorder
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private var dropShadow:RectangularDropShadow;
      
      private var _borderMetrics:EdgeMetrics;
      
      public function ToolTipBorder()
      {
         super();
      }
      
      override public function get borderMetrics() : EdgeMetrics
      {
         if(Boolean(this._borderMetrics))
         {
            return this._borderMetrics;
         }
         var borderStyle:String = getStyle("borderStyle");
         switch(borderStyle)
         {
            case "errorTipRight":
               this._borderMetrics = new EdgeMetrics(15,1,3,3);
               break;
            case "errorTipAbove":
               this._borderMetrics = new EdgeMetrics(3,1,3,15);
               break;
            case "errorTipBelow":
               this._borderMetrics = new EdgeMetrics(3,13,3,3);
               break;
            default:
               this._borderMetrics = new EdgeMetrics(3,1,3,3);
         }
         return this._borderMetrics;
      }
      
      override public function styleChanged(styleProp:String) : void
      {
         super.styleChanged(styleProp);
         if(styleProp == "borderStyle" || styleProp == "styleName" || styleProp == null)
         {
            this._borderMetrics = null;
         }
      }
      
      override protected function updateDisplayList(w:Number, h:Number) : void
      {
         super.updateDisplayList(w,h);
         var borderStyle:String = getStyle("borderStyle");
         var backgroundColor:uint = getStyle("backgroundColor");
         var backgroundAlpha:Number = getStyle("backgroundAlpha");
         var borderColor:uint = getStyle("borderColor");
         var cornerRadius:Number = getStyle("cornerRadius");
         var g:Graphics = graphics;
         g.clear();
         filters = [];
         switch(borderStyle)
         {
            case "none":
               break;
            case "errorTipRight":
               drawRoundRect(11,0,w - 11,h - 2,3,borderColor,backgroundAlpha);
               g.beginFill(borderColor,backgroundAlpha);
               g.moveTo(11,7);
               g.lineTo(0,13);
               g.lineTo(11,19);
               g.moveTo(11,7);
               g.endFill();
               filters = [new DropShadowFilter(2,90,0,0.4)];
               break;
            case "errorTipAbove":
               drawRoundRect(0,0,w,h - 13,3,borderColor,backgroundAlpha);
               g.beginFill(borderColor,backgroundAlpha);
               g.moveTo(9,h - 13);
               g.lineTo(15,h - 2);
               g.lineTo(21,h - 13);
               g.moveTo(9,h - 13);
               g.endFill();
               filters = [new DropShadowFilter(2,90,0,0.4)];
               break;
            case "errorTipBelow":
               drawRoundRect(0,11,w,h - 13,3,borderColor,backgroundAlpha);
               g.beginFill(borderColor,backgroundAlpha);
               g.moveTo(9,11);
               g.lineTo(15,0);
               g.lineTo(21,11);
               g.moveTo(10,11);
               g.endFill();
               filters = [new DropShadowFilter(2,90,0,0.4)];
               break;
            default:
               drawRoundRect(3,1,w - 6,h - 4,cornerRadius,backgroundColor,backgroundAlpha);
               if(!this.dropShadow)
               {
                  this.dropShadow = new RectangularDropShadow();
               }
               this.dropShadow.distance = 3;
               this.dropShadow.angle = 90;
               this.dropShadow.color = 0;
               this.dropShadow.alpha = 0.4;
               this.dropShadow.tlRadius = cornerRadius + 2;
               this.dropShadow.trRadius = cornerRadius + 2;
               this.dropShadow.blRadius = cornerRadius + 2;
               this.dropShadow.brRadius = cornerRadius + 2;
               this.dropShadow.drawShadow(graphics,3,0,w - 6,h - 4);
         }
      }
   }
}


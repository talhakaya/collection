package mx.skins.halo
{
   import flash.display.DisplayObject;
   import flash.display.Graphics;
   import flash.display.InteractiveObject;
   import flash.display.Shape;
   import flash.events.Event;
   import mx.core.FlexShape;
   import mx.core.FlexSprite;
   import mx.core.mx_internal;
   import mx.styles.CSSStyleDeclaration;
   import mx.styles.IStyleManager2;
   import mx.styles.StyleManager;
   
   use namespace mx_internal;
   
   public class BusyCursor extends FlexSprite
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private var minuteHand:Shape;
      
      private var hourHand:Shape;
      
      public function BusyCursor(styleManager:IStyleManager2 = null)
      {
         var g:Graphics = null;
         super();
         if(!styleManager)
         {
            styleManager = StyleManager.getStyleManager(null);
         }
         var cursorManagerStyleDeclaration:CSSStyleDeclaration = styleManager.getMergedStyleDeclaration("mx.managers.CursorManager");
         var cursorClass:Class = cursorManagerStyleDeclaration.getStyle("busyCursorBackground");
         var cursorHolder:DisplayObject = new cursorClass();
         if(cursorHolder is InteractiveObject)
         {
            InteractiveObject(cursorHolder).mouseEnabled = false;
         }
         addChild(cursorHolder);
         var xOff:Number = -0.5;
         var yOff:Number = -0.5;
         this.minuteHand = new FlexShape();
         this.minuteHand.name = "minuteHand";
         g = this.minuteHand.graphics;
         g.beginFill(0);
         g.moveTo(xOff,yOff);
         g.lineTo(1 + xOff,0 + yOff);
         g.lineTo(1 + xOff,5 + yOff);
         g.lineTo(0 + xOff,5 + yOff);
         g.lineTo(0 + xOff,0 + yOff);
         g.endFill();
         addChild(this.minuteHand);
         this.hourHand = new FlexShape();
         this.hourHand.name = "hourHand";
         g = this.hourHand.graphics;
         g.beginFill(0);
         g.moveTo(xOff,yOff);
         g.lineTo(4 + xOff,0 + yOff);
         g.lineTo(4 + xOff,1 + yOff);
         g.lineTo(0 + xOff,1 + yOff);
         g.lineTo(0 + xOff,0 + yOff);
         g.endFill();
         addChild(this.hourHand);
         addEventListener(Event.ADDED,this.handleAdded);
         addEventListener(Event.REMOVED,this.handleRemoved);
      }
      
      private function handleAdded(event:Event) : void
      {
         addEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
      }
      
      private function handleRemoved(event:Event) : void
      {
         removeEventListener(Event.ENTER_FRAME,this.enterFrameHandler);
      }
      
      private function enterFrameHandler(event:Event) : void
      {
         this.minuteHand.rotation += 12;
         this.hourHand.rotation += 1;
      }
   }
}


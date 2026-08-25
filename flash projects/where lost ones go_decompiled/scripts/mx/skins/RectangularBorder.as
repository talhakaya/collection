package mx.skins
{
   import flash.display.DisplayObject;
   import flash.display.Graphics;
   import flash.display.Loader;
   import flash.display.LoaderInfo;
   import flash.display.Shape;
   import flash.events.ErrorEvent;
   import flash.events.Event;
   import flash.events.IOErrorEvent;
   import flash.geom.Rectangle;
   import flash.net.URLRequest;
   import flash.system.ApplicationDomain;
   import flash.system.LoaderContext;
   import flash.utils.getDefinitionByName;
   import mx.core.EdgeMetrics;
   import mx.core.FlexLoader;
   import mx.core.FlexShape;
   import mx.core.IChildList;
   import mx.core.IContainer;
   import mx.core.IRawChildrenContainer;
   import mx.core.IRectangularBorder;
   import mx.core.mx_internal;
   import mx.resources.IResourceManager;
   import mx.resources.ResourceManager;
   import mx.styles.ISimpleStyleClient;
   
   use namespace mx_internal;
   
   [ResourceBundle("skins")]
   public class RectangularBorder extends Border implements IRectangularBorder
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      private var backgroundImageStyle:Object;
      
      private var backgroundImageWidth:Number;
      
      private var backgroundImageHeight:Number;
      
      private var resourceManager:IResourceManager = ResourceManager.getInstance();
      
      private var backgroundImage:DisplayObject;
      
      private var _backgroundImageBounds:Rectangle;
      
      public function RectangularBorder()
      {
         super();
         addEventListener(Event.REMOVED,this.removedHandler);
      }
      
      public function get hasBackgroundImage() : Boolean
      {
         return this.backgroundImage != null;
      }
      
      public function get backgroundImageBounds() : Rectangle
      {
         return this._backgroundImageBounds;
      }
      
      public function set backgroundImageBounds(value:Rectangle) : void
      {
         if(Boolean(this._backgroundImageBounds) && Boolean(value) && this._backgroundImageBounds.equals(value))
         {
            return;
         }
         this._backgroundImageBounds = value;
         invalidateDisplayList();
      }
      
      override protected function updateDisplayList(unscaledWidth:Number, unscaledHeight:Number) : void
      {
         var cls:Class = null;
         var newStyleObj:DisplayObject = null;
         var loader:Loader = null;
         var loaderContext:LoaderContext = null;
         var message:String = null;
         if(!parent)
         {
            return;
         }
         var newStyle:Object = getStyle("backgroundImage");
         if(newStyle != this.backgroundImageStyle)
         {
            this.removedHandler(null);
            this.backgroundImageStyle = newStyle;
            if(Boolean(newStyle) && Boolean(newStyle as Class))
            {
               cls = Class(newStyle);
               this.initBackgroundImage(new cls());
            }
            else if(Boolean(newStyle) && newStyle is String)
            {
               try
               {
                  cls = Class(getDefinitionByName(String(newStyle)));
               }
               catch(e:Error)
               {
               }
               if(Boolean(cls))
               {
                  newStyleObj = new cls();
                  this.initBackgroundImage(newStyleObj);
               }
               else
               {
                  loader = new FlexLoader();
                  loader.contentLoaderInfo.addEventListener(Event.COMPLETE,this.completeEventHandler);
                  loader.contentLoaderInfo.addEventListener(IOErrorEvent.IO_ERROR,this.errorEventHandler);
                  loader.contentLoaderInfo.addEventListener(ErrorEvent.ERROR,this.errorEventHandler);
                  loaderContext = new LoaderContext();
                  loaderContext.applicationDomain = new ApplicationDomain(ApplicationDomain.currentDomain);
                  loader.load(new URLRequest(String(newStyle)),loaderContext);
               }
            }
            else if(Boolean(newStyle))
            {
               message = this.resourceManager.getString("skins","notLoaded",[newStyle]);
               throw new Error(message);
            }
         }
         if(Boolean(this.backgroundImage))
         {
            this.layoutBackgroundImage();
         }
      }
      
      private function initBackgroundImage(image:DisplayObject) : void
      {
         this.backgroundImage = image;
         if(image is Loader)
         {
            this.backgroundImageWidth = Loader(image).contentLoaderInfo.width;
            this.backgroundImageHeight = Loader(image).contentLoaderInfo.height;
         }
         else
         {
            this.backgroundImageWidth = this.backgroundImage.width;
            this.backgroundImageHeight = this.backgroundImage.height;
            if(image is ISimpleStyleClient)
            {
               ISimpleStyleClient(image).styleName = styleName;
            }
         }
         var childrenList:IChildList = parent is IRawChildrenContainer ? IRawChildrenContainer(parent).rawChildren : IChildList(parent);
         var backgroundMask:Shape = new FlexShape();
         backgroundMask.name = "backgroundMask";
         backgroundMask.x = 0;
         backgroundMask.y = 0;
         childrenList.addChild(backgroundMask);
         var myIndex:int = childrenList.getChildIndex(this);
         childrenList.addChildAt(this.backgroundImage,myIndex + 1);
         this.backgroundImage.mask = backgroundMask;
      }
      
      public function layoutBackgroundImage() : void
      {
         var sW:Number = NaN;
         var sH:Number = NaN;
         var sX:Number = NaN;
         var sY:Number = NaN;
         var scale:Number = NaN;
         var g:Graphics = null;
         var p:DisplayObject = parent;
         var bm:EdgeMetrics = p is IContainer ? IContainer(p).viewMetrics : borderMetrics;
         var scrollableBk:Boolean = getStyle("backgroundAttachment") != "fixed";
         if(Boolean(this._backgroundImageBounds))
         {
            sW = this._backgroundImageBounds.width;
            sH = this._backgroundImageBounds.height;
         }
         else
         {
            sW = width - bm.left - bm.right;
            sH = height - bm.top - bm.bottom;
         }
         var percentage:Number = this.getBackgroundSize();
         if(isNaN(percentage))
         {
            sX = 1;
            sY = 1;
         }
         else
         {
            scale = percentage * 0.01;
            sX = scale * sW / this.backgroundImageWidth;
            sY = scale * sH / this.backgroundImageHeight;
         }
         this.backgroundImage.scaleX = sX;
         this.backgroundImage.scaleY = sY;
         var offsetX:Number = Math.round(0.5 * (sW - this.backgroundImageWidth * sX));
         var offsetY:Number = Math.round(0.5 * (sH - this.backgroundImageHeight * sY));
         this.backgroundImage.x = bm.left;
         this.backgroundImage.y = bm.top;
         var backgroundMask:Shape = Shape(this.backgroundImage.mask);
         backgroundMask.x = bm.left;
         backgroundMask.y = bm.top;
         if(scrollableBk && p is IContainer)
         {
            offsetX -= IContainer(p).horizontalScrollPosition;
            offsetY -= IContainer(p).verticalScrollPosition;
         }
         this.backgroundImage.alpha = getStyle("backgroundAlpha");
         this.backgroundImage.x += offsetX;
         this.backgroundImage.y += offsetY;
         var maskWidth:Number = width - bm.left - bm.right;
         var maskHeight:Number = height - bm.top - bm.bottom;
         if(backgroundMask.width != maskWidth || backgroundMask.height != maskHeight)
         {
            g = backgroundMask.graphics;
            g.clear();
            g.beginFill(16777215);
            g.drawRect(0,0,maskWidth,maskHeight);
            g.endFill();
         }
      }
      
      private function getBackgroundSize() : Number
      {
         var index:int = 0;
         var percentage:Number = NaN;
         var backgroundSize:Object = getStyle("backgroundSize");
         if(Boolean(backgroundSize) && backgroundSize is String)
         {
            index = int(backgroundSize.indexOf("%"));
            if(index != -1)
            {
               percentage = Number(backgroundSize.substr(0,index));
            }
         }
         return percentage;
      }
      
      private function errorEventHandler(event:Event) : void
      {
      }
      
      private function completeEventHandler(event:Event) : void
      {
         if(!parent)
         {
            return;
         }
         var target:DisplayObject = DisplayObject(LoaderInfo(event.target).loader);
         this.initBackgroundImage(target);
         this.layoutBackgroundImage();
         dispatchEvent(event.clone());
      }
      
      private function removedHandler(event:Event) : void
      {
         var childrenList:IChildList = null;
         if(Boolean(this.backgroundImage))
         {
            childrenList = parent is IRawChildrenContainer ? IRawChildrenContainer(parent).rawChildren : IChildList(parent);
            childrenList.removeChild(this.backgroundImage.mask);
            childrenList.removeChild(this.backgroundImage);
            this.backgroundImage = null;
         }
      }
   }
}


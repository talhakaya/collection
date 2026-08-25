package mx.core
{
   import flash.events.EventDispatcher;
   import mx.events.PropertyChangeEvent;
   import mx.events.PropertyChangeEventKind;
   
   [Event(name="layerPropertyChange",type="mx.events.PropertyChangeEvent")]
   public class DesignLayer extends EventDispatcher implements IMXMLObject
   {
      
      private var _id:String;
      
      private var _parent:DesignLayer;
      
      private var layerChildren:Array = new Array();
      
      private var _visible:Boolean = true;
      
      private var _alpha:Number = 1;
      
      public function DesignLayer()
      {
         super();
      }
      
      public function get id() : String
      {
         return this._id;
      }
      
      public function set id(value:String) : void
      {
         this._id = value;
      }
      
      public function get parent() : DesignLayer
      {
         return this._parent;
      }
      
      protected function parentChanged(value:DesignLayer) : void
      {
         if(Boolean(this._parent) && Boolean(value))
         {
            this._parent.removeLayer(this);
         }
         this._parent = value;
         this.effectiveVisibilityChanged(this._visible);
         this.effectiveAlphaChanged(this._alpha);
      }
      
      public function get visible() : Boolean
      {
         return this._visible;
      }
      
      public function set visible(value:Boolean) : void
      {
         if(this._visible != value)
         {
            this._visible = value;
            this.effectiveVisibilityChanged(this.effectiveVisibility);
         }
      }
      
      public function get effectiveVisibility() : Boolean
      {
         var isVisible:Boolean = this._visible;
         var currentLayer:DesignLayer = this;
         while(isVisible && Boolean(currentLayer.parent))
         {
            currentLayer = currentLayer.parent;
            isVisible = currentLayer.visible;
         }
         return isVisible;
      }
      
      protected function effectiveVisibilityChanged(value:Boolean) : void
      {
         var layerChild:DesignLayer = null;
         dispatchEvent(new PropertyChangeEvent("layerPropertyChange",false,false,PropertyChangeEventKind.UPDATE,"effectiveVisibility",!this.effectiveVisibility,this.effectiveVisibility));
         for(var i:int = 0; i < this.layerChildren.length; i++)
         {
            layerChild = this.layerChildren[i];
            if(layerChild.visible)
            {
               layerChild.effectiveVisibilityChanged(value);
            }
         }
      }
      
      public function get alpha() : Number
      {
         return this._alpha;
      }
      
      public function set alpha(value:Number) : void
      {
         var oldAlpha:Number = NaN;
         if(this._alpha != value)
         {
            oldAlpha = this._alpha;
            this._alpha = value;
            this.effectiveAlphaChanged(oldAlpha);
         }
      }
      
      public function get effectiveAlpha() : Number
      {
         var currentAlpha:Number = this._alpha;
         var currentLayer:DesignLayer = this;
         while(Boolean(currentLayer.parent))
         {
            currentLayer = currentLayer.parent;
            currentAlpha *= currentLayer.alpha;
         }
         return currentAlpha;
      }
      
      protected function effectiveAlphaChanged(oldAlpha:Number) : void
      {
         var layerChild:DesignLayer = null;
         dispatchEvent(new PropertyChangeEvent("layerPropertyChange",false,false,PropertyChangeEventKind.UPDATE,"effectiveAlpha",oldAlpha,this.effectiveAlpha));
         for(var i:int = 0; i < this.layerChildren.length; i++)
         {
            layerChild = this.layerChildren[i];
            layerChild.effectiveAlphaChanged(layerChild.alpha);
         }
      }
      
      public function get numLayers() : int
      {
         return this.layerChildren.length;
      }
      
      public function addLayer(value:DesignLayer) : void
      {
         value.parentChanged(this);
         this.layerChildren.push(value);
      }
      
      public function getLayerAt(index:int) : DesignLayer
      {
         return index < this.layerChildren.length && index >= 0 ? this.layerChildren[index] : null;
      }
      
      public function initialized(document:Object, id:String) : void
      {
         this.id = id;
      }
      
      public function removeLayer(value:DesignLayer) : void
      {
         for(var i:int = 0; i < this.layerChildren.length; i++)
         {
            if(this.layerChildren[i] == value)
            {
               value.parentChanged(null);
               this.layerChildren.splice(i,1);
               return;
            }
         }
      }
   }
}


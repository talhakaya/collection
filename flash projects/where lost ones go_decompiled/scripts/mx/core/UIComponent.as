package mx.core
{
   import flash.accessibility.Accessibility;
   import flash.accessibility.AccessibilityProperties;
   import flash.display.BlendMode;
   import flash.display.DisplayObject;
   import flash.display.DisplayObjectContainer;
   import flash.display.GradientType;
   import flash.display.Graphics;
   import flash.display.InteractiveObject;
   import flash.display.Loader;
   import flash.display.Shader;
   import flash.display.Sprite;
   import flash.display.Stage;
   import flash.events.Event;
   import flash.events.EventPhase;
   import flash.events.FocusEvent;
   import flash.events.IEventDispatcher;
   import flash.events.KeyboardEvent;
   import flash.geom.ColorTransform;
   import flash.geom.Matrix;
   import flash.geom.Matrix3D;
   import flash.geom.PerspectiveProjection;
   import flash.geom.Point;
   import flash.geom.Rectangle;
   import flash.geom.Transform;
   import flash.geom.Vector3D;
   import flash.system.ApplicationDomain;
   import flash.system.Capabilities;
   import flash.text.TextFormatAlign;
   import flash.text.TextLineMetrics;
   import flash.ui.Keyboard;
   import flash.utils.Dictionary;
   import flash.utils.getQualifiedClassName;
   import mx.automation.IAutomationObject;
   import mx.binding.BindingManager;
   import mx.controls.IFlexContextMenu;
   import mx.effects.EffectManager;
   import mx.effects.IEffect;
   import mx.effects.IEffectInstance;
   import mx.events.ChildExistenceChangedEvent;
   import mx.events.DynamicEvent;
   import mx.events.EffectEvent;
   import mx.events.FlexEvent;
   import mx.events.MoveEvent;
   import mx.events.PropertyChangeEvent;
   import mx.events.ResizeEvent;
   import mx.events.StateChangeEvent;
   import mx.events.ValidationResultEvent;
   import mx.filters.BaseFilter;
   import mx.filters.IBitmapFilter;
   import mx.geom.RoundedRectangle;
   import mx.geom.Transform;
   import mx.geom.TransformOffsets;
   import mx.graphics.shaderClasses.ColorBurnShader;
   import mx.graphics.shaderClasses.ColorDodgeShader;
   import mx.graphics.shaderClasses.ColorShader;
   import mx.graphics.shaderClasses.ExclusionShader;
   import mx.graphics.shaderClasses.HueShader;
   import mx.graphics.shaderClasses.LuminosityShader;
   import mx.graphics.shaderClasses.SaturationShader;
   import mx.graphics.shaderClasses.SoftLightShader;
   import mx.managers.CursorManager;
   import mx.managers.ICursorManager;
   import mx.managers.IFocusManager;
   import mx.managers.IFocusManagerComponent;
   import mx.managers.IFocusManagerContainer;
   import mx.managers.ILayoutManagerClient;
   import mx.managers.ISystemManager;
   import mx.managers.IToolTipManagerClient;
   import mx.managers.SystemManager;
   import mx.managers.SystemManagerGlobals;
   import mx.managers.ToolTipManager;
   import mx.resources.IResourceManager;
   import mx.resources.ResourceManager;
   import mx.states.State;
   import mx.states.Transition;
   import mx.styles.CSSStyleDeclaration;
   import mx.styles.IAdvancedStyleClient;
   import mx.styles.ISimpleStyleClient;
   import mx.styles.IStyleClient;
   import mx.styles.IStyleManager2;
   import mx.styles.StyleManager;
   import mx.styles.StyleProtoChain;
   import mx.utils.ColorUtil;
   import mx.utils.GraphicsUtil;
   import mx.utils.MatrixUtil;
   import mx.utils.NameUtil;
   import mx.utils.StringUtil;
   import mx.utils.TransformUtil;
   import mx.validators.IValidatorListener;
   import mx.validators.ValidationResult;
   
   use namespace mx_internal;
   
   [ResourceBundle("skins")]
   [ResourceBundle("core")]
   [AccessibilityClass(implementation="mx.accessibility.UIComponentAccProps")]
   [Effect(name="removedEffect",event="removed")]
   [Effect(name="addedEffect",event="added")]
   [Effect(name="focusOutEffect",event="focusOut")]
   [Effect(name="focusInEffect",event="focusIn")]
   [Effect(name="rollOutEffect",event="rollOut")]
   [Effect(name="rollOverEffect",event="rollOver")]
   [Effect(name="mouseUpEffect",event="mouseUp")]
   [Effect(name="mouseDownEffect",event="mouseDown")]
   [Effect(name="hideEffect",event="hide")]
   [Effect(name="showEffect",event="show")]
   [Effect(name="resizeEffect",event="resize")]
   [Effect(name="moveEffect",event="move")]
   [Effect(name="creationCompleteEffect",event="creationComplete")]
   [Style(name="themeColor",type="uint",format="Color",inherit="yes",theme="halo")]
   [Style(name="showErrorTip",type="Boolean",inherit="yes")]
   [Style(name="showErrorSkin",type="Boolean",inherit="yes")]
   [Style(name="layoutDirection",type="String",enumeration="ltr,rtl",inherit="yes")]
   [Style(name="focusThickness",type="Number",format="Length",inherit="no",minValue="0.0")]
   [Style(name="focusSkin",type="Class",inherit="no")]
   [Style(name="focusBlendMode",type="String",inherit="no")]
   [Style(name="interactionMode",type="String",enumeration="mouse,touch",inherit="yes")]
   [Style(name="errorColor",type="uint",format="Color",inherit="yes")]
   [Style(name="chromeColor",type="uint",format="Color",inherit="yes",theme="spark")]
   [Style(name="verticalCenter",type="String",inherit="no")]
   [Style(name="top",type="String",inherit="no")]
   [Style(name="right",type="String",inherit="no")]
   [Style(name="left",type="String",inherit="no")]
   [Style(name="horizontalCenter",type="String",inherit="no")]
   [Style(name="bottom",type="String",inherit="no")]
   [Style(name="baseline",type="String",inherit="no")]
   [Event(name="toolTipStart",type="mx.events.ToolTipEvent")]
   [Event(name="toolTipShown",type="mx.events.ToolTipEvent")]
   [Event(name="toolTipShow",type="mx.events.ToolTipEvent")]
   [Event(name="toolTipHide",type="mx.events.ToolTipEvent")]
   [Event(name="toolTipEnd",type="mx.events.ToolTipEvent")]
   [Event(name="toolTipCreate",type="mx.events.ToolTipEvent")]
   [Event(name="touchInteractionEnd",type="mx.events.TouchInteractionEvent")]
   [Event(name="touchInteractionStart",type="mx.events.TouchInteractionEvent")]
   [Event(name="touchInteractionStarting",type="mx.events.TouchInteractionEvent")]
   [Event(name="stateChangeInterrupted",type="mx.events.FlexEvent")]
   [Event(name="stateChangeComplete",type="mx.events.FlexEvent")]
   [Event(name="exitState",type="mx.events.FlexEvent")]
   [Event(name="enterState",type="mx.events.FlexEvent")]
   [Event(name="currentStateChange",type="mx.events.StateChangeEvent")]
   [Event(name="currentStateChanging",type="mx.events.StateChangeEvent")]
   [Event(name="effectEnd",type="mx.events.EffectEvent")]
   [Event(name="effectStop",type="mx.events.EffectEvent")]
   [Event(name="effectStart",type="mx.events.EffectEvent")]
   [Event(name="dragStart",type="mx.events.DragEvent")]
   [Event(name="dragComplete",type="mx.events.DragEvent")]
   [Event(name="dragDrop",type="mx.events.DragEvent")]
   [Event(name="dragExit",type="mx.events.DragEvent")]
   [Event(name="dragOver",type="mx.events.DragEvent")]
   [Event(name="dragEnter",type="mx.events.DragEvent")]
   [Event(name="valid",type="mx.events.FlexEvent")]
   [Event(name="invalid",type="mx.events.FlexEvent")]
   [Event(name="valueCommit",type="mx.events.FlexEvent")]
   [Event(name="mouseWheelOutside",type="mx.events.FlexMouseEvent")]
   [Event(name="mouseDownOutside",type="mx.events.FlexMouseEvent")]
   [Event(name="show",type="mx.events.FlexEvent")]
   [Event(name="resize",type="mx.events.ResizeEvent")]
   [Event(name="remove",type="mx.events.FlexEvent")]
   [Event(name="preinitialize",type="mx.events.FlexEvent")]
   [Event(name="move",type="mx.events.MoveEvent")]
   [Event(name="initialize",type="mx.events.FlexEvent")]
   [Event(name="hide",type="mx.events.FlexEvent")]
   [Event(name="updateComplete",type="mx.events.FlexEvent")]
   [Event(name="creationComplete",type="mx.events.FlexEvent")]
   [Event(name="add",type="mx.events.FlexEvent")]
   [Exclude(name="layoutDirection",kind="property")]
   public class UIComponent extends FlexSprite implements IAutomationObject, IChildList, IConstraintClient, IDeferredInstantiationUIComponent, IFlexDisplayObject, IFlexModule, IInvalidating, ILayoutManagerClient, IPropertyChangeNotifier, IRepeaterClient, IStateClient, IAdvancedStyleClient, IToolTipManagerClient, IUIComponent, IValidatorListener, IVisualElement
   {
      
      mx_internal static var createAccessibilityImplementation:Function;
      
      private static var noEmbeddedFonts:Boolean;
      
      private static var _embeddedFontRegistry:IEmbeddedFontRegistry;
      
      private static var effectType:Class;
      
      private static var compositeEffectType:Class;
      
      mx_internal static var dispatchEventHook:Function;
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      public static const DEFAULT_MEASURED_WIDTH:Number = 160;
      
      public static const DEFAULT_MEASURED_MIN_WIDTH:Number = 40;
      
      public static const DEFAULT_MEASURED_HEIGHT:Number = 22;
      
      public static const DEFAULT_MEASURED_MIN_HEIGHT:Number = 22;
      
      public static const DEFAULT_MAX_WIDTH:Number = 10000;
      
      public static const DEFAULT_MAX_HEIGHT:Number = 10000;
      
      private static const LAYOUT_DIRECTION_CACHE_UNSET:String = "layoutDirectionCacheUnset";
      
      private static var effectLoaded:Boolean = false;
      
      private static var compositeEffectLoaded:Boolean = false;
      
      private static var fakeMouseX:QName = new QName(mx_internal,"_mouseX");
      
      private static var fakeMouseY:QName = new QName(mx_internal,"_mouseY");
      
      private var deferredSetStyles:Object;
      
      private var listeningForRender:Boolean = false;
      
      private var methodQueue:Array = [];
      
      private var hasFocusRect:Boolean = false;
      
      private var transitionFromState:String;
      
      private var transitionToState:String;
      
      private var parentChangedFlag:Boolean = false;
      
      private var layoutDirectionCachedValue:String = "layoutDirectionCacheUnset";
      
      private var _initialized:Boolean = false;
      
      private var _processedDescriptors:Boolean = false;
      
      private var _updateCompletePendingFlag:Boolean = false;
      
      mx_internal var invalidatePropertiesFlag:Boolean = false;
      
      mx_internal var invalidateSizeFlag:Boolean = false;
      
      mx_internal var invalidateDisplayListFlag:Boolean = false;
      
      mx_internal var setActualSizeCalled:Boolean = false;
      
      private var oldX:Number = 0;
      
      private var oldY:Number = 0;
      
      private var oldWidth:Number = 0;
      
      private var oldHeight:Number = 0;
      
      private var oldMinWidth:Number;
      
      private var oldMinHeight:Number;
      
      private var oldExplicitWidth:Number;
      
      private var oldExplicitHeight:Number;
      
      private var oldScaleX:Number = 1;
      
      private var oldScaleY:Number = 1;
      
      private var hasFontContextBeenSaved:Boolean = false;
      
      private var oldEmbeddedFontContext:IFlexModuleFactory = null;
      
      mx_internal var _layoutFeatures:AdvancedLayoutFeatures;
      
      private var _transform:flash.geom.Transform;
      
      private var cachedTextFormat:UITextFormat;
      
      mx_internal var effectOverlay:UIComponent;
      
      mx_internal var effectOverlayColor:uint;
      
      mx_internal var effectOverlayReferenceCount:int = 0;
      
      mx_internal var saveBorderColor:Boolean = true;
      
      mx_internal var origBorderColor:Number;
      
      mx_internal var automaticRadioButtonGroups:Object;
      
      private var _usingBridge:int = -1;
      
      mx_internal var _owner:DisplayObjectContainer;
      
      mx_internal var _parent:DisplayObjectContainer;
      
      mx_internal var _width:Number;
      
      mx_internal var _height:Number;
      
      private var _scaleX:Number = 1;
      
      private var _scaleY:Number = 1;
      
      private var _visible:Boolean = true;
      
      private var _alpha:Number = 1;
      
      private var _blendMode:String = "normal";
      
      private var blendShaderChanged:Boolean;
      
      private var blendModeChanged:Boolean;
      
      private var _enabled:Boolean = false;
      
      private var _filters:Array;
      
      private var _designLayer:DesignLayer;
      
      private var _tweeningProperties:Array;
      
      private var _focusManager:IFocusManager;
      
      private var _resourceManager:IResourceManager = ResourceManager.getInstance();
      
      private var _styleManager:IStyleManager2;
      
      private var _systemManager:ISystemManager;
      
      private var _systemManagerDirty:Boolean = false;
      
      private var _nestLevel:int = 0;
      
      mx_internal var _descriptor:UIComponentDescriptor;
      
      mx_internal var _document:Object;
      
      mx_internal var _documentDescriptor:UIComponentDescriptor;
      
      private var _id:String;
      
      private var _moduleFactory:IFlexModuleFactory;
      
      private var _inheritingStyles:Object = StyleProtoChain.STYLE_UNINITIALIZED;
      
      private var _nonInheritingStyles:Object = StyleProtoChain.STYLE_UNINITIALIZED;
      
      private var _styleDeclaration:CSSStyleDeclaration;
      
      private var _cachePolicy:String = "auto";
      
      private var cacheAsBitmapCount:int = 0;
      
      private var _focusPane:Sprite;
      
      private var _focusEnabled:Boolean = true;
      
      private var _hasFocusableChildren:Boolean = false;
      
      private var _mouseFocusEnabled:Boolean = true;
      
      private var _tabFocusEnabled:Boolean = true;
      
      private var _measuredMinWidth:Number = 0;
      
      private var _measuredMinHeight:Number = 0;
      
      private var _measuredWidth:Number = 0;
      
      private var _measuredHeight:Number = 0;
      
      private var _percentWidth:Number;
      
      private var _percentHeight:Number;
      
      mx_internal var _explicitMinWidth:Number;
      
      mx_internal var _explicitMinHeight:Number;
      
      mx_internal var _explicitMaxWidth:Number;
      
      mx_internal var _explicitMaxHeight:Number;
      
      private var _explicitWidth:Number;
      
      private var _explicitHeight:Number;
      
      private var _hasComplexLayoutMatrix:Boolean = false;
      
      private var _includeInLayout:Boolean = true;
      
      mx_internal var oldLayoutDirection:String = "ltr";
      
      private var _instanceIndices:Array;
      
      private var _repeaters:Array;
      
      private var _repeaterIndices:Array;
      
      private var _currentState:String;
      
      private var requestedCurrentState:String;
      
      private var playStateTransition:Boolean = true;
      
      private var _currentStateChanged:Boolean;
      
      private var _currentStateDeferred:String;
      
      private var _states:Array = [];
      
      private var _currentTransition:Transition;
      
      private var _transitions:Array = [];
      
      private var _flexContextMenu:IFlexContextMenu;
      
      private var _styleName:Object;
      
      mx_internal var _toolTip:String;
      
      private var _uid:String;
      
      private var _isPopUp:Boolean;
      
      private var _automationDelegate:IAutomationObject;
      
      private var _automationName:String = null;
      
      private var _showInAutomationHierarchy:Boolean = true;
      
      mx_internal var _errorString:String = "";
      
      private var oldErrorString:String = "";
      
      private var errorArray:Array;
      
      private var errorObjectArray:Array;
      
      private var errorStringChanged:Boolean = false;
      
      private var _validationSubField:String;
      
      private var lastUnscaledWidth:Number;
      
      private var lastUnscaledHeight:Number;
      
      mx_internal var advanceStyleClientChildren:Dictionary = null;
      
      mx_internal var _effectsStarted:Array = [];
      
      mx_internal var _affectedProperties:Object = {};
      
      private var _isEffectStarted:Boolean = false;
      
      private var preventDrawFocus:Boolean = false;
      
      private var _endingEffectInstances:Array = [];
      
      private var _maintainProjectionCenter:Boolean = false;
      
      public function UIComponent()
      {
         super();
         focusRect = false;
         tabEnabled = this is IFocusManagerComponent;
         this.tabFocusEnabled = this is IFocusManagerComponent;
         this.enabled = true;
         this.$visible = false;
         addEventListener(Event.ADDED,this.addedHandler);
         addEventListener(Event.REMOVED,this.removedHandler);
         addEventListener(Event.REMOVED_FROM_STAGE,this.removedFromStageHandler);
         if(this is IFocusManagerComponent)
         {
            addEventListener(FocusEvent.FOCUS_IN,this.focusInHandler);
            addEventListener(FocusEvent.FOCUS_OUT,this.focusOutHandler);
            addEventListener(KeyboardEvent.KEY_DOWN,this.keyDownHandler);
            addEventListener(KeyboardEvent.KEY_UP,this.keyUpHandler);
         }
         this.resourcesChanged();
         this.resourceManager.addEventListener(Event.CHANGE,this.resourceManager_changeHandler,false,0,true);
         this._width = super.width;
         this._height = super.height;
      }
      
      mx_internal static function get embeddedFontRegistry() : IEmbeddedFontRegistry
      {
         if(!_embeddedFontRegistry && !noEmbeddedFonts)
         {
            try
            {
               _embeddedFontRegistry = IEmbeddedFontRegistry(Singleton.getInstance("mx.core::IEmbeddedFontRegistry"));
            }
            catch(e:Error)
            {
               noEmbeddedFonts = true;
            }
         }
         return _embeddedFontRegistry;
      }
      
      public static function suspendBackgroundProcessing() : void
      {
         ++UIComponentGlobals.callLaterSuspendCount;
      }
      
      public static function resumeBackgroundProcessing() : void
      {
         var sm:ISystemManager = null;
         if(UIComponentGlobals.callLaterSuspendCount > 0)
         {
            --UIComponentGlobals.callLaterSuspendCount;
            if(UIComponentGlobals.callLaterSuspendCount == 0)
            {
               sm = SystemManagerGlobals.topLevelSystemManagers[0];
               if(Boolean(sm) && Boolean(sm.stage))
               {
                  sm.stage.invalidate();
               }
            }
         }
      }
      
      [Inspectable(environment="none")]
      public function get initialized() : Boolean
      {
         return this._initialized;
      }
      
      public function set initialized(value:Boolean) : void
      {
         this._initialized = value;
         if(value)
         {
            this.setVisible(this._visible,true);
            this.dispatchEvent(new FlexEvent(FlexEvent.CREATION_COMPLETE));
         }
      }
      
      [Inspectable(environment="none")]
      public function get processedDescriptors() : Boolean
      {
         return this._processedDescriptors;
      }
      
      public function set processedDescriptors(value:Boolean) : void
      {
         this._processedDescriptors = value;
         if(value)
         {
            this.dispatchEvent(new FlexEvent(FlexEvent.INITIALIZE));
         }
      }
      
      [Inspectable(environment="none")]
      public function get updateCompletePendingFlag() : Boolean
      {
         return this._updateCompletePendingFlag;
      }
      
      public function set updateCompletePendingFlag(value:Boolean) : void
      {
         this._updateCompletePendingFlag = value;
      }
      
      public function get accessibilityEnabled() : Boolean
      {
         return Boolean(accessibilityProperties) ? !accessibilityProperties.silent : true;
      }
      
      public function set accessibilityEnabled(value:Boolean) : void
      {
         if(!Capabilities.hasAccessibility)
         {
            return;
         }
         if(!accessibilityProperties)
         {
            accessibilityProperties = new AccessibilityProperties();
         }
         accessibilityProperties.silent = !value;
         Accessibility.updateProperties();
      }
      
      public function get accessibilityName() : String
      {
         return Boolean(accessibilityProperties) ? accessibilityProperties.name : "";
      }
      
      public function set accessibilityName(value:String) : void
      {
         if(!Capabilities.hasAccessibility)
         {
            return;
         }
         if(!accessibilityProperties)
         {
            accessibilityProperties = new AccessibilityProperties();
         }
         accessibilityProperties.name = value;
         Accessibility.updateProperties();
      }
      
      public function get accessibilityDescription() : String
      {
         return Boolean(accessibilityProperties) ? accessibilityProperties.description : "";
      }
      
      public function set accessibilityDescription(value:String) : void
      {
         if(!Capabilities.hasAccessibility)
         {
            return;
         }
         if(!accessibilityProperties)
         {
            accessibilityProperties = new AccessibilityProperties();
         }
         accessibilityProperties.description = value;
         Accessibility.updateProperties();
      }
      
      public function get accessibilityShortcut() : String
      {
         return Boolean(accessibilityProperties) ? accessibilityProperties.shortcut : "";
      }
      
      public function set accessibilityShortcut(value:String) : void
      {
         if(!Capabilities.hasAccessibility)
         {
            return;
         }
         if(!accessibilityProperties)
         {
            accessibilityProperties = new AccessibilityProperties();
         }
         accessibilityProperties.shortcut = value;
         Accessibility.updateProperties();
      }
      
      private function get usingBridge() : Boolean
      {
         if(this._usingBridge == 0)
         {
            return false;
         }
         if(this._usingBridge == 1)
         {
            return true;
         }
         if(!this._systemManager)
         {
            return false;
         }
         var mp:Object = this._systemManager.getImplementation("mx.managers::IMarshalSystemManager");
         if(!mp)
         {
            this._usingBridge = 0;
            return false;
         }
         if(Boolean(mp.useSWFBridge()))
         {
            this._usingBridge = 1;
            return true;
         }
         this._usingBridge = 0;
         return false;
      }
      
      public function get owner() : DisplayObjectContainer
      {
         return Boolean(this._owner) ? this._owner : this.parent;
      }
      
      public function set owner(value:DisplayObjectContainer) : void
      {
         this._owner = value;
      }
      
      override public function get parent() : DisplayObjectContainer
      {
         try
         {
            return Boolean(this._parent) ? this._parent : super.parent;
         }
         catch(e:SecurityError)
         {
         }
         return null;
      }
      
      [Inspectable(category="General")]
      [Bindable("xChanged")]
      override public function get x() : Number
      {
         return this._layoutFeatures == null ? super.x : this._layoutFeatures.layoutX;
      }
      
      override public function set x(value:Number) : void
      {
         if(this.x == value)
         {
            return;
         }
         if(this._layoutFeatures == null)
         {
            super.x = value;
         }
         else
         {
            this._layoutFeatures.layoutX = value;
            this.invalidateTransform();
         }
         this.invalidateProperties();
         if(Boolean(this.parent) && this.parent is UIComponent)
         {
            UIComponent(this.parent).childXYChanged();
         }
         if(hasEventListener("xChanged"))
         {
            this.dispatchEvent(new Event("xChanged"));
         }
      }
      
      [Inspectable(category="General")]
      [Bindable("zChanged")]
      override public function get z() : Number
      {
         return this._layoutFeatures == null ? super.z : this._layoutFeatures.layoutZ;
      }
      
      override public function set z(value:Number) : void
      {
         if(this.z == value)
         {
            return;
         }
         var was3D:Boolean = this.is3D;
         if(this._layoutFeatures == null)
         {
            this.initAdvancedLayoutFeatures();
         }
         this._layoutFeatures.layoutZ = value;
         this.invalidateTransform();
         this.invalidateProperties();
         if(was3D != this.is3D)
         {
            this.validateMatrix();
         }
         if(hasEventListener("zChanged"))
         {
            this.dispatchEvent(new Event("zChanged"));
         }
      }
      
      public function get transformX() : Number
      {
         return this._layoutFeatures == null ? 0 : this._layoutFeatures.transformX;
      }
      
      public function set transformX(value:Number) : void
      {
         if(this.transformX == value)
         {
            return;
         }
         if(this._layoutFeatures == null)
         {
            this.initAdvancedLayoutFeatures();
         }
         this._layoutFeatures.transformX = value;
         this.invalidateTransform();
         this.invalidateProperties();
         this.invalidateParentSizeAndDisplayList();
      }
      
      public function get transformY() : Number
      {
         return this._layoutFeatures == null ? 0 : this._layoutFeatures.transformY;
      }
      
      public function set transformY(value:Number) : void
      {
         if(this.transformY == value)
         {
            return;
         }
         if(this._layoutFeatures == null)
         {
            this.initAdvancedLayoutFeatures();
         }
         this._layoutFeatures.transformY = value;
         this.invalidateTransform();
         this.invalidateProperties();
         this.invalidateParentSizeAndDisplayList();
      }
      
      public function get transformZ() : Number
      {
         return this._layoutFeatures == null ? 0 : this._layoutFeatures.transformZ;
      }
      
      public function set transformZ(value:Number) : void
      {
         if(this.transformZ == value)
         {
            return;
         }
         if(this._layoutFeatures == null)
         {
            this.initAdvancedLayoutFeatures();
         }
         this._layoutFeatures.transformZ = value;
         this.invalidateTransform();
         this.invalidateProperties();
         this.invalidateParentSizeAndDisplayList();
      }
      
      override public function get rotation() : Number
      {
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            return super.rotation;
         }
         return this._layoutFeatures == null ? super.rotation : this._layoutFeatures.layoutRotationZ;
      }
      
      override public function set rotation(value:Number) : void
      {
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            super.rotation = value;
            return;
         }
         if(this.rotation == value)
         {
            return;
         }
         this._hasComplexLayoutMatrix = true;
         if(this._layoutFeatures == null)
         {
            super.rotation = MatrixUtil.clampRotation(value);
         }
         else
         {
            this._layoutFeatures.layoutRotationZ = value;
         }
         this.invalidateTransform();
         this.invalidateProperties();
         this.invalidateParentSizeAndDisplayList();
      }
      
      override public function get rotationZ() : Number
      {
         return this.rotation;
      }
      
      override public function set rotationZ(value:Number) : void
      {
         this.rotation = value;
      }
      
      override public function get rotationX() : Number
      {
         return this._layoutFeatures == null ? super.rotationX : this._layoutFeatures.layoutRotationX;
      }
      
      override public function set rotationX(value:Number) : void
      {
         if(this.rotationX == value)
         {
            return;
         }
         var was3D:Boolean = this.is3D;
         if(this._layoutFeatures == null)
         {
            this.initAdvancedLayoutFeatures();
         }
         this._layoutFeatures.layoutRotationX = value;
         this.invalidateTransform();
         this.invalidateProperties();
         this.invalidateParentSizeAndDisplayList();
         if(was3D != this.is3D)
         {
            this.validateMatrix();
         }
      }
      
      override public function get rotationY() : Number
      {
         return this._layoutFeatures == null ? super.rotationY : this._layoutFeatures.layoutRotationY;
      }
      
      override public function set rotationY(value:Number) : void
      {
         if(this.rotationY == value)
         {
            return;
         }
         var was3D:Boolean = this.is3D;
         if(this._layoutFeatures == null)
         {
            this.initAdvancedLayoutFeatures();
         }
         this._layoutFeatures.layoutRotationY = value;
         this.invalidateTransform();
         this.invalidateProperties();
         this.invalidateParentSizeAndDisplayList();
         if(was3D != this.is3D)
         {
            this.validateMatrix();
         }
      }
      
      [Inspectable(category="General")]
      [Bindable("yChanged")]
      override public function get y() : Number
      {
         return this._layoutFeatures == null ? super.y : this._layoutFeatures.layoutY;
      }
      
      override public function set y(value:Number) : void
      {
         if(this.y == value)
         {
            return;
         }
         if(this._layoutFeatures == null)
         {
            super.y = value;
         }
         else
         {
            this._layoutFeatures.layoutY = value;
            this.invalidateTransform();
         }
         this.invalidateProperties();
         if(Boolean(this.parent) && this.parent is UIComponent)
         {
            UIComponent(this.parent).childXYChanged();
         }
         if(hasEventListener("yChanged"))
         {
            this.dispatchEvent(new Event("yChanged"));
         }
      }
      
      [PercentProxy("percentWidth")]
      [Inspectable(category="General")]
      [Bindable("widthChanged")]
      override public function get width() : Number
      {
         return this._width;
      }
      
      override public function set width(value:Number) : void
      {
         if(this.explicitWidth != value)
         {
            this.explicitWidth = value;
            this.invalidateSize();
         }
         if(this._width != value)
         {
            this.invalidateProperties();
            this.invalidateDisplayList();
            this.invalidateParentSizeAndDisplayList();
            this._width = value;
            if(Boolean(this._layoutFeatures))
            {
               this._layoutFeatures.layoutWidth = this._width;
               this.invalidateTransform();
            }
            if(hasEventListener("widthChanged"))
            {
               this.dispatchEvent(new Event("widthChanged"));
            }
         }
      }
      
      [PercentProxy("percentHeight")]
      [Inspectable(category="General")]
      [Bindable("heightChanged")]
      override public function get height() : Number
      {
         return this._height;
      }
      
      override public function set height(value:Number) : void
      {
         if(this.explicitHeight != value)
         {
            this.explicitHeight = value;
            this.invalidateSize();
         }
         if(this._height != value)
         {
            this.invalidateProperties();
            this.invalidateDisplayList();
            this.invalidateParentSizeAndDisplayList();
            this._height = value;
            if(hasEventListener("heightChanged"))
            {
               this.dispatchEvent(new Event("heightChanged"));
            }
         }
      }
      
      [Inspectable(category="Size",defaultValue="1.0")]
      [Bindable("scaleXChanged")]
      override public function get scaleX() : Number
      {
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            return this._scaleX;
         }
         return this._layoutFeatures == null ? super.scaleX : this._layoutFeatures.layoutScaleX;
      }
      
      override public function set scaleX(value:Number) : void
      {
         var prevValue:Number = NaN;
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            if(this._scaleX == value)
            {
               return;
            }
            this._scaleX = value;
            this.invalidateProperties();
            this.invalidateSize();
         }
         else
         {
            prevValue = this._layoutFeatures == null ? this.scaleX : this._layoutFeatures.layoutScaleX;
            if(prevValue == value)
            {
               return;
            }
            this._hasComplexLayoutMatrix = true;
            if(this._layoutFeatures == null)
            {
               super.scaleX = value;
            }
            else
            {
               this._layoutFeatures.layoutScaleX = value;
            }
            this.invalidateTransform();
            this.invalidateProperties();
            this.invalidateParentSizeAndDisplayList();
         }
         this.dispatchEvent(new Event("scaleXChanged"));
      }
      
      [Inspectable(category="Size",defaultValue="1.0")]
      [Bindable("scaleYChanged")]
      override public function get scaleY() : Number
      {
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            return this._scaleY;
         }
         return this._layoutFeatures == null ? super.scaleY : this._layoutFeatures.layoutScaleY;
      }
      
      override public function set scaleY(value:Number) : void
      {
         var prevValue:Number = NaN;
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            if(this._scaleY == value)
            {
               return;
            }
            this._scaleY = value;
            this.invalidateProperties();
            this.invalidateSize();
         }
         else
         {
            prevValue = this._layoutFeatures == null ? this.scaleY : this._layoutFeatures.layoutScaleY;
            if(prevValue == value)
            {
               return;
            }
            this._hasComplexLayoutMatrix = true;
            if(this._layoutFeatures == null)
            {
               super.scaleY = value;
            }
            else
            {
               this._layoutFeatures.layoutScaleY = value;
            }
            this.invalidateTransform();
            this.invalidateProperties();
            this.invalidateParentSizeAndDisplayList();
         }
         this.dispatchEvent(new Event("scaleYChanged"));
      }
      
      [Inspectable(category="Size",defaultValue="1.0")]
      [Bindable("scaleZChanged")]
      override public function get scaleZ() : Number
      {
         return this._layoutFeatures == null ? super.scaleZ : this._layoutFeatures.layoutScaleZ;
      }
      
      override public function set scaleZ(value:Number) : void
      {
         if(this.scaleZ == value)
         {
            return;
         }
         var was3D:Boolean = this.is3D;
         if(this._layoutFeatures == null)
         {
            this.initAdvancedLayoutFeatures();
         }
         this._hasComplexLayoutMatrix = true;
         this._layoutFeatures.layoutScaleZ = value;
         this.invalidateTransform();
         this.invalidateProperties();
         this.invalidateParentSizeAndDisplayList();
         if(was3D != this.is3D)
         {
            this.validateMatrix();
         }
         this.dispatchEvent(new Event("scaleZChanged"));
      }
      
      final mx_internal function get $scaleX() : Number
      {
         return super.scaleX;
      }
      
      final mx_internal function set $scaleX(value:Number) : void
      {
         super.scaleX = value;
      }
      
      final mx_internal function get $scaleY() : Number
      {
         return super.scaleY;
      }
      
      final mx_internal function set $scaleY(value:Number) : void
      {
         super.scaleY = value;
      }
      
      [Inspectable(category="General",defaultValue="true")]
      [Bindable("show")]
      [Bindable("hide")]
      override public function get visible() : Boolean
      {
         return this._visible;
      }
      
      override public function set visible(value:Boolean) : void
      {
         this.setVisible(value);
      }
      
      public function setVisible(value:Boolean, noEvent:Boolean = false) : void
      {
         var eventType:String = null;
         this._visible = value;
         if(!this.initialized)
         {
            return;
         }
         if(Boolean(this.designLayer) && !this.designLayer.effectiveVisibility)
         {
            value = false;
         }
         if(this.$visible == value)
         {
            return;
         }
         this.$visible = value;
         if(!noEvent)
         {
            eventType = value ? FlexEvent.SHOW : FlexEvent.HIDE;
            if(hasEventListener(eventType))
            {
               this.dispatchEvent(new FlexEvent(eventType));
            }
         }
      }
      
      [Inspectable(defaultValue="1.0",category="General",verbose="1",minValue="0.0",maxValue="1.0")]
      [Bindable("alphaChanged")]
      override public function get alpha() : Number
      {
         return int(this._alpha * 256) / 256;
      }
      
      override public function set alpha(value:Number) : void
      {
         if(this._alpha != value)
         {
            this._alpha = value;
            if(Boolean(this.designLayer))
            {
               value *= this.designLayer.effectiveAlpha;
            }
            this.$alpha = value;
            this.dispatchEvent(new Event("alphaChanged"));
         }
      }
      
      [Inspectable(category="General",enumeration="add,alpha,darken,difference,erase,hardlight,invert,layer,lighten,multiply,normal,subtract,screen,overlay,colordodge,colorburn,exclusion,softlight,hue,saturation,color,luminosity",defaultValue="normal")]
      override public function get blendMode() : String
      {
         return this._blendMode;
      }
      
      override public function set blendMode(value:String) : void
      {
         if(this._blendMode != value)
         {
            this._blendMode = value;
            this.blendModeChanged = true;
            if(value == "colordodge" || value == "colorburn" || value == "exclusion" || value == "softlight" || value == "hue" || value == "saturation" || value == "color" || value == "luminosity")
            {
               this.blendShaderChanged = true;
            }
            this.invalidateProperties();
         }
      }
      
      [Inspectable(enumeration="true,false",defaultValue="true")]
      override public function get doubleClickEnabled() : Boolean
      {
         return super.doubleClickEnabled;
      }
      
      override public function set doubleClickEnabled(value:Boolean) : void
      {
         var childList:IChildList = null;
         var child:InteractiveObject = null;
         super.doubleClickEnabled = value;
         if(this is IRawChildrenContainer)
         {
            childList = IRawChildrenContainer(this).rawChildren;
         }
         else
         {
            childList = IChildList(this);
         }
         for(var i:int = 0; i < childList.numChildren; i++)
         {
            child = childList.getChildAt(i) as InteractiveObject;
            if(Boolean(child))
            {
               child.doubleClickEnabled = value;
            }
         }
      }
      
      [Bindable("enabledChanged")]
      [Inspectable(category="General",enumeration="true,false",defaultValue="true")]
      public function get enabled() : Boolean
      {
         return this._enabled;
      }
      
      public function set enabled(value:Boolean) : void
      {
         this._enabled = value;
         this.cachedTextFormat = null;
         this.invalidateDisplayList();
         this.dispatchEvent(new Event("enabledChanged"));
      }
      
      override public function set cacheAsBitmap(value:Boolean) : void
      {
         super.cacheAsBitmap = value;
         this.cacheAsBitmapCount = value ? 1 : 0;
      }
      
      override public function get filters() : Array
      {
         return Boolean(this._filters) ? this._filters : super.filters;
      }
      
      override public function set filters(value:Array) : void
      {
         var n:int = 0;
         var i:int = 0;
         var e:IEventDispatcher = null;
         if(Boolean(this._filters))
         {
            n = int(this._filters.length);
            for(i = 0; i < n; i++)
            {
               e = this._filters[i] as IEventDispatcher;
               if(Boolean(e))
               {
                  e.removeEventListener(BaseFilter.CHANGE,this.filterChangeHandler);
               }
            }
         }
         this._filters = value;
         var clonedFilters:Array = [];
         if(Boolean(this._filters))
         {
            n = int(this._filters.length);
            for(i = 0; i < n; i++)
            {
               if(this._filters[i] is IBitmapFilter)
               {
                  e = this._filters[i] as IEventDispatcher;
                  if(Boolean(e))
                  {
                     e.addEventListener(BaseFilter.CHANGE,this.filterChangeHandler);
                  }
                  clonedFilters.push(IBitmapFilter(this._filters[i]).clone());
               }
               else
               {
                  clonedFilters.push(this._filters[i]);
               }
            }
         }
         super.filters = clonedFilters;
      }
      
      [Inspectable(environment="none")]
      public function get designLayer() : DesignLayer
      {
         return this._designLayer;
      }
      
      public function set designLayer(value:DesignLayer) : void
      {
         if(Boolean(this._designLayer))
         {
            this._designLayer.removeEventListener("layerPropertyChange",this.layer_PropertyChange,false);
         }
         this._designLayer = value;
         if(Boolean(this._designLayer))
         {
            this._designLayer.addEventListener("layerPropertyChange",this.layer_PropertyChange,false,0,true);
         }
         this.$alpha = Boolean(this._designLayer) ? this._alpha * this._designLayer.effectiveAlpha : this._alpha;
         this.$visible = Boolean(this.designLayer) ? this._visible && this._designLayer.effectiveVisibility : this._visible;
      }
      
      final mx_internal function get $alpha() : Number
      {
         return super.alpha;
      }
      
      final mx_internal function set $alpha(value:Number) : void
      {
         super.alpha = value;
      }
      
      final mx_internal function get $blendMode() : String
      {
         return super.blendMode;
      }
      
      final mx_internal function set $blendMode(value:String) : void
      {
         super.blendMode = value;
      }
      
      final mx_internal function set $blendShader(value:Shader) : void
      {
         super.blendShader = value;
      }
      
      final mx_internal function get $parent() : DisplayObjectContainer
      {
         return super.parent;
      }
      
      final mx_internal function get $x() : Number
      {
         return super.x;
      }
      
      final mx_internal function set $x(value:Number) : void
      {
         super.x = value;
      }
      
      final mx_internal function get $y() : Number
      {
         return super.y;
      }
      
      final mx_internal function set $y(value:Number) : void
      {
         super.y = value;
      }
      
      final mx_internal function get $width() : Number
      {
         return super.width;
      }
      
      final mx_internal function set $width(value:Number) : void
      {
         super.width = value;
      }
      
      final mx_internal function get $height() : Number
      {
         return super.height;
      }
      
      final mx_internal function set $height(value:Number) : void
      {
         super.height = value;
      }
      
      final mx_internal function get $visible() : Boolean
      {
         return super.visible;
      }
      
      final mx_internal function set $visible(value:Boolean) : void
      {
         super.visible = value;
      }
      
      public function get contentMouseX() : Number
      {
         return this.mouseX;
      }
      
      public function get contentMouseY() : Number
      {
         return this.mouseY;
      }
      
      [Inspectable(environment="none")]
      public function get tweeningProperties() : Array
      {
         return this._tweeningProperties;
      }
      
      public function set tweeningProperties(value:Array) : void
      {
         this._tweeningProperties = value;
      }
      
      public function get cursorManager() : ICursorManager
      {
         var cm:ICursorManager = null;
         var o:DisplayObject = this.parent;
         while(Boolean(o))
         {
            if(o is IUIComponent && "cursorManager" in o)
            {
               return o["cursorManager"];
            }
            o = o.parent;
         }
         return CursorManager.getInstance();
      }
      
      [Inspectable(environment="none")]
      public function get focusManager() : IFocusManager
      {
         if(Boolean(this._focusManager))
         {
            return this._focusManager;
         }
         var o:DisplayObject = this.parent;
         while(Boolean(o))
         {
            if(o is IFocusManagerContainer)
            {
               return IFocusManagerContainer(o).focusManager;
            }
            o = o.parent;
         }
         return null;
      }
      
      public function set focusManager(value:IFocusManager) : void
      {
         this._focusManager = value;
         this.dispatchEvent(new FlexEvent(FlexEvent.ADD_FOCUS_MANAGER));
      }
      
      [Bindable("unused")]
      protected function get resourceManager() : IResourceManager
      {
         return this._resourceManager;
      }
      
      public function get styleManager() : IStyleManager2
      {
         if(!this._styleManager)
         {
            this._styleManager = StyleManager.getStyleManager(this.moduleFactory);
         }
         return this._styleManager;
      }
      
      [Inspectable(environment="none")]
      public function get systemManager() : ISystemManager
      {
         var r:DisplayObject = null;
         var o:DisplayObjectContainer = null;
         var ui:IUIComponent = null;
         if(!this._systemManager || this._systemManagerDirty)
         {
            r = root;
            if(!(Boolean(this._systemManager) && this._systemManager.isProxy))
            {
               if(Boolean(r) && !(r is Stage))
               {
                  this._systemManager = r as ISystemManager;
               }
               else if(Boolean(r))
               {
                  this._systemManager = Stage(r).getChildAt(0) as ISystemManager;
               }
               else
               {
                  o = this.parent;
                  while(Boolean(o))
                  {
                     ui = o as IUIComponent;
                     if(Boolean(ui))
                     {
                        this._systemManager = ui.systemManager;
                        break;
                     }
                     if(o is ISystemManager)
                     {
                        this._systemManager = o as ISystemManager;
                        break;
                     }
                     o = o.parent;
                  }
               }
            }
            this._systemManagerDirty = false;
         }
         return this._systemManager;
      }
      
      public function set systemManager(value:ISystemManager) : void
      {
         this._systemManager = value;
         this._systemManagerDirty = false;
      }
      
      mx_internal function getNonNullSystemManager() : ISystemManager
      {
         var sm:ISystemManager = this.systemManager;
         if(!sm)
         {
            sm = ISystemManager(SystemManager.getSWFRoot(this));
         }
         if(!sm)
         {
            return SystemManagerGlobals.topLevelSystemManagers[0];
         }
         return sm;
      }
      
      protected function invalidateSystemManager() : void
      {
         var child:UIComponent = null;
         var childList:IChildList = this is IRawChildrenContainer ? IRawChildrenContainer(this).rawChildren : IChildList(this);
         var n:int = childList.numChildren;
         for(var i:int = 0; i < n; i++)
         {
            child = childList.getChildAt(i) as UIComponent;
            if(Boolean(child))
            {
               child.invalidateSystemManager();
            }
         }
         this._systemManagerDirty = true;
      }
      
      [Inspectable(environment="none")]
      public function get nestLevel() : int
      {
         return this._nestLevel;
      }
      
      public function set nestLevel(value:int) : void
      {
         var ui:ILayoutManagerClient = null;
         var textField:IUITextField = null;
         if(value == 1)
         {
            return;
         }
         if(value > 1 && this._nestLevel != value)
         {
            this._nestLevel = value;
            this.updateCallbacks();
            value++;
         }
         else if(value == 0)
         {
            this._nestLevel = value = 0;
         }
         else
         {
            value++;
         }
         var childList:IChildList = this is IRawChildrenContainer ? IRawChildrenContainer(this).rawChildren : IChildList(this);
         var n:int = childList.numChildren;
         for(var i:int = 0; i < n; i++)
         {
            ui = childList.getChildAt(i) as ILayoutManagerClient;
            if(Boolean(ui))
            {
               ui.nestLevel = value;
            }
            else
            {
               textField = childList.getChildAt(i) as IUITextField;
               if(Boolean(textField))
               {
                  textField.nestLevel = value;
               }
            }
         }
      }
      
      [Inspectable(environment="none")]
      public function get descriptor() : UIComponentDescriptor
      {
         return this._descriptor;
      }
      
      public function set descriptor(value:UIComponentDescriptor) : void
      {
         this._descriptor = value;
      }
      
      [Inspectable(environment="none")]
      public function get document() : Object
      {
         return this._document;
      }
      
      public function set document(value:Object) : void
      {
         var child:IUIComponent = null;
         var n:int = numChildren;
         for(var i:int = 0; i < n; i++)
         {
            child = getChildAt(i) as IUIComponent;
            if(child)
            {
               if(child.document == this._document || child.document == FlexGlobals.topLevelApplication)
               {
                  child.document = value;
               }
            }
         }
         this._document = value;
      }
      
      mx_internal function get documentDescriptor() : UIComponentDescriptor
      {
         return this._documentDescriptor;
      }
      
      public function get id() : String
      {
         return this._id;
      }
      
      public function set id(value:String) : void
      {
         this._id = value;
      }
      
      public function get isDocument() : Boolean
      {
         return this.document == this;
      }
      
      [Bindable("initialize")]
      public function get parentApplication() : Object
      {
         var p:UIComponent = null;
         var o:Object = this.systemManager.document;
         if(o == this)
         {
            p = o.systemManager.parent as UIComponent;
            o = Boolean(p) ? p.systemManager.document : null;
         }
         return o;
      }
      
      [Bindable("initialize")]
      public function get parentDocument() : Object
      {
         var p:IUIComponent = null;
         var sm:ISystemManager = null;
         if(this.document == this)
         {
            p = this.parent as IUIComponent;
            if(Boolean(p))
            {
               return p.document;
            }
            sm = this.parent as ISystemManager;
            if(Boolean(sm))
            {
               return sm.document;
            }
            return null;
         }
         return this.document;
      }
      
      public function get screen() : Rectangle
      {
         var sm:ISystemManager = this.systemManager;
         return Boolean(sm) ? sm.screen : null;
      }
      
      [Inspectable(environment="none")]
      public function get moduleFactory() : IFlexModuleFactory
      {
         return this._moduleFactory;
      }
      
      public function set moduleFactory(factory:IFlexModuleFactory) : void
      {
         var child:IFlexModule = null;
         var styleClient:Object = null;
         var iAdvanceStyleClientChild:IFlexModule = null;
         this._styleManager = null;
         var n:int = numChildren;
         for(var i:int = 0; i < n; i++)
         {
            child = getChildAt(i) as IFlexModule;
            if(child)
            {
               if(child.moduleFactory == null || child.moduleFactory == this._moduleFactory)
               {
                  child.moduleFactory = factory;
               }
            }
         }
         if(this.advanceStyleClientChildren != null)
         {
            for(styleClient in this.advanceStyleClientChildren)
            {
               iAdvanceStyleClientChild = styleClient as IFlexModule;
               if(Boolean(iAdvanceStyleClientChild) && (iAdvanceStyleClientChild.moduleFactory == null || iAdvanceStyleClientChild.moduleFactory == this._moduleFactory))
               {
                  iAdvanceStyleClientChild.moduleFactory = factory;
               }
            }
         }
         this._moduleFactory = factory;
         this.setDeferredStyles();
      }
      
      [Inspectable(environment="none")]
      public function get inheritingStyles() : Object
      {
         return this._inheritingStyles;
      }
      
      public function set inheritingStyles(value:Object) : void
      {
         this._inheritingStyles = value;
      }
      
      [Inspectable(environment="none")]
      public function get nonInheritingStyles() : Object
      {
         return this._nonInheritingStyles;
      }
      
      public function set nonInheritingStyles(value:Object) : void
      {
         this._nonInheritingStyles = value;
      }
      
      [Inspectable(environment="none")]
      public function get styleDeclaration() : CSSStyleDeclaration
      {
         return this._styleDeclaration;
      }
      
      public function set styleDeclaration(value:CSSStyleDeclaration) : void
      {
         this._styleDeclaration = value;
      }
      
      [Inspectable(enumeration="on,off,auto",defaultValue="auto")]
      public function get cachePolicy() : String
      {
         return this._cachePolicy;
      }
      
      public function set cachePolicy(value:String) : void
      {
         if(this._cachePolicy != value)
         {
            this._cachePolicy = value;
            if(value == UIComponentCachePolicy.OFF)
            {
               this.cacheAsBitmap = false;
            }
            else if(value == UIComponentCachePolicy.ON)
            {
               this.cacheAsBitmap = true;
            }
            else
            {
               this.cacheAsBitmap = this.cacheAsBitmapCount > 0;
            }
         }
      }
      
      [Inspectable(environment="none")]
      public function set cacheHeuristic(value:Boolean) : void
      {
         if(this._cachePolicy == UIComponentCachePolicy.AUTO)
         {
            if(value)
            {
               ++this.cacheAsBitmapCount;
            }
            else if(this.cacheAsBitmapCount != 0)
            {
               --this.cacheAsBitmapCount;
            }
            super.cacheAsBitmap = this.cacheAsBitmapCount != 0;
         }
      }
      
      [Inspectable(environment="none")]
      public function get focusPane() : Sprite
      {
         return this._focusPane;
      }
      
      public function set focusPane(value:Sprite) : void
      {
         if(Boolean(value))
         {
            this.addChild(value);
            value.x = 0;
            value.y = 0;
            value.scrollRect = null;
            this._focusPane = value;
         }
         else
         {
            this.removeChild(this._focusPane);
            this._focusPane.mask = null;
            this._focusPane = null;
         }
      }
      
      [Inspectable(defaultValue="true")]
      public function get focusEnabled() : Boolean
      {
         return this._focusEnabled;
      }
      
      public function set focusEnabled(value:Boolean) : void
      {
         this._focusEnabled = value;
      }
      
      [Inspectable(defaultValue="false")]
      [Bindable("hasFocusableChildrenChange")]
      public function get hasFocusableChildren() : Boolean
      {
         return this._hasFocusableChildren;
      }
      
      public function set hasFocusableChildren(value:Boolean) : void
      {
         if(value != this._hasFocusableChildren)
         {
            this._hasFocusableChildren = value;
            this.dispatchEvent(new Event("hasFocusableChildrenChange"));
         }
      }
      
      [Inspectable(defaultValue="true")]
      public function get mouseFocusEnabled() : Boolean
      {
         return this._mouseFocusEnabled;
      }
      
      public function set mouseFocusEnabled(value:Boolean) : void
      {
         this._mouseFocusEnabled = value;
      }
      
      [Inspectable(defaultValue="true")]
      [Bindable("tabFocusEnabledChange")]
      public function get tabFocusEnabled() : Boolean
      {
         return this._tabFocusEnabled;
      }
      
      public function set tabFocusEnabled(value:Boolean) : void
      {
         if(value != this._tabFocusEnabled)
         {
            this._tabFocusEnabled = value;
            this.dispatchEvent(new Event("tabFocusEnabledChange"));
         }
      }
      
      [Inspectable(environment="none")]
      public function get measuredMinWidth() : Number
      {
         return this._measuredMinWidth;
      }
      
      public function set measuredMinWidth(value:Number) : void
      {
         this._measuredMinWidth = value;
      }
      
      [Inspectable(environment="none")]
      public function get measuredMinHeight() : Number
      {
         return this._measuredMinHeight;
      }
      
      public function set measuredMinHeight(value:Number) : void
      {
         this._measuredMinHeight = value;
      }
      
      [Inspectable(environment="none")]
      public function get measuredWidth() : Number
      {
         return this._measuredWidth;
      }
      
      public function set measuredWidth(value:Number) : void
      {
         this._measuredWidth = value;
      }
      
      [Inspectable(environment="none")]
      public function get measuredHeight() : Number
      {
         return this._measuredHeight;
      }
      
      public function set measuredHeight(value:Number) : void
      {
         this._measuredHeight = value;
      }
      
      [Inspectable(environment="none")]
      [Bindable("resize")]
      public function get percentWidth() : Number
      {
         return this._percentWidth;
      }
      
      public function set percentWidth(value:Number) : void
      {
         if(this._percentWidth == value)
         {
            return;
         }
         if(!isNaN(value))
         {
            this._explicitWidth = NaN;
         }
         this._percentWidth = value;
         this.invalidateParentSizeAndDisplayList();
      }
      
      [Inspectable(environment="none")]
      [Bindable("resize")]
      public function get percentHeight() : Number
      {
         return this._percentHeight;
      }
      
      public function set percentHeight(value:Number) : void
      {
         if(this._percentHeight == value)
         {
            return;
         }
         if(!isNaN(value))
         {
            this._explicitHeight = NaN;
         }
         this._percentHeight = value;
         this.invalidateParentSizeAndDisplayList();
      }
      
      [Inspectable(category="Size",defaultValue="0")]
      [Bindable("explicitMinWidthChanged")]
      public function get minWidth() : Number
      {
         if(!isNaN(this.explicitMinWidth))
         {
            return this.explicitMinWidth;
         }
         return this.measuredMinWidth;
      }
      
      public function set minWidth(value:Number) : void
      {
         if(this.explicitMinWidth == value)
         {
            return;
         }
         this.explicitMinWidth = value;
      }
      
      [Inspectable(category="Size",defaultValue="0")]
      [Bindable("explicitMinHeightChanged")]
      public function get minHeight() : Number
      {
         if(!isNaN(this.explicitMinHeight))
         {
            return this.explicitMinHeight;
         }
         return this.measuredMinHeight;
      }
      
      public function set minHeight(value:Number) : void
      {
         if(this.explicitMinHeight == value)
         {
            return;
         }
         this.explicitMinHeight = value;
      }
      
      [Inspectable(category="Size",defaultValue="10000")]
      [Bindable("explicitMaxWidthChanged")]
      public function get maxWidth() : Number
      {
         return !isNaN(this.explicitMaxWidth) ? this.explicitMaxWidth : DEFAULT_MAX_WIDTH;
      }
      
      public function set maxWidth(value:Number) : void
      {
         if(this.explicitMaxWidth == value)
         {
            return;
         }
         this.explicitMaxWidth = value;
      }
      
      [Inspectable(category="Size",defaultValue="10000")]
      [Bindable("explicitMaxHeightChanged")]
      public function get maxHeight() : Number
      {
         return !isNaN(this.explicitMaxHeight) ? this.explicitMaxHeight : DEFAULT_MAX_HEIGHT;
      }
      
      public function set maxHeight(value:Number) : void
      {
         if(this.explicitMaxHeight == value)
         {
            return;
         }
         this.explicitMaxHeight = value;
      }
      
      [Inspectable(environment="none")]
      [Bindable("explicitMinWidthChanged")]
      public function get explicitMinWidth() : Number
      {
         return this._explicitMinWidth;
      }
      
      public function set explicitMinWidth(value:Number) : void
      {
         if(this._explicitMinWidth == value)
         {
            return;
         }
         this._explicitMinWidth = value;
         this.invalidateSize();
         this.invalidateParentSizeAndDisplayList();
         this.dispatchEvent(new Event("explicitMinWidthChanged"));
      }
      
      [Inspectable(environment="none")]
      [Bindable("explictMinHeightChanged")]
      public function get explicitMinHeight() : Number
      {
         return this._explicitMinHeight;
      }
      
      public function set explicitMinHeight(value:Number) : void
      {
         if(this._explicitMinHeight == value)
         {
            return;
         }
         this._explicitMinHeight = value;
         this.invalidateSize();
         this.invalidateParentSizeAndDisplayList();
         this.dispatchEvent(new Event("explicitMinHeightChanged"));
      }
      
      [Inspectable(environment="none")]
      [Bindable("explicitMaxWidthChanged")]
      public function get explicitMaxWidth() : Number
      {
         return this._explicitMaxWidth;
      }
      
      public function set explicitMaxWidth(value:Number) : void
      {
         if(this._explicitMaxWidth == value)
         {
            return;
         }
         this._explicitMaxWidth = value;
         this.invalidateSize();
         this.invalidateParentSizeAndDisplayList();
         this.dispatchEvent(new Event("explicitMaxWidthChanged"));
      }
      
      [Inspectable(environment="none")]
      [Bindable("explicitMaxHeightChanged")]
      public function get explicitMaxHeight() : Number
      {
         return this._explicitMaxHeight;
      }
      
      public function set explicitMaxHeight(value:Number) : void
      {
         if(this._explicitMaxHeight == value)
         {
            return;
         }
         this._explicitMaxHeight = value;
         this.invalidateSize();
         this.invalidateParentSizeAndDisplayList();
         this.dispatchEvent(new Event("explicitMaxHeightChanged"));
      }
      
      [Inspectable(environment="none")]
      [Bindable("explicitWidthChanged")]
      public function get explicitWidth() : Number
      {
         return this._explicitWidth;
      }
      
      public function set explicitWidth(value:Number) : void
      {
         if(this._explicitWidth == value)
         {
            return;
         }
         if(!isNaN(value))
         {
            this._percentWidth = NaN;
         }
         this._explicitWidth = value;
         this.invalidateSize();
         this.invalidateParentSizeAndDisplayList();
         this.dispatchEvent(new Event("explicitWidthChanged"));
      }
      
      [Inspectable(environment="none")]
      [Bindable("explicitHeightChanged")]
      public function get explicitHeight() : Number
      {
         return this._explicitHeight;
      }
      
      public function set explicitHeight(value:Number) : void
      {
         if(this._explicitHeight == value)
         {
            return;
         }
         if(!isNaN(value))
         {
            this._percentHeight = NaN;
         }
         this._explicitHeight = value;
         this.invalidateSize();
         this.invalidateParentSizeAndDisplayList();
         this.dispatchEvent(new Event("explicitHeightChanged"));
      }
      
      protected function get hasComplexLayoutMatrix() : Boolean
      {
         if(!this._hasComplexLayoutMatrix)
         {
            return false;
         }
         if(this._layoutFeatures == null)
         {
            this._hasComplexLayoutMatrix = !MatrixUtil.isDeltaIdentity(super.transform.matrix);
            return this._hasComplexLayoutMatrix;
         }
         return !MatrixUtil.isDeltaIdentity(this._layoutFeatures.layoutMatrix);
      }
      
      [Inspectable(category="General",defaultValue="true")]
      [Bindable("includeInLayoutChanged")]
      public function get includeInLayout() : Boolean
      {
         return this._includeInLayout;
      }
      
      public function set includeInLayout(value:Boolean) : void
      {
         var p:IInvalidating = null;
         if(this._includeInLayout != value)
         {
            this._includeInLayout = value;
            p = this.parent as IInvalidating;
            if(Boolean(p))
            {
               p.invalidateSize();
               p.invalidateDisplayList();
            }
            this.dispatchEvent(new Event("includeInLayoutChanged"));
         }
      }
      
      public function get layoutDirection() : String
      {
         if(this.layoutDirectionCachedValue == LAYOUT_DIRECTION_CACHE_UNSET)
         {
            this.layoutDirectionCachedValue = this.getStyle("layoutDirection");
         }
         return this.layoutDirectionCachedValue;
      }
      
      public function set layoutDirection(value:String) : void
      {
         if(value == null)
         {
            this.setStyle("layoutDirection",undefined);
         }
         else
         {
            this.setStyle("layoutDirection",value);
         }
      }
      
      public function get instanceIndex() : int
      {
         return Boolean(this._instanceIndices) ? int(this._instanceIndices[this._instanceIndices.length - 1]) : -1;
      }
      
      [Inspectable(environment="none")]
      public function get instanceIndices() : Array
      {
         return Boolean(this._instanceIndices) ? this._instanceIndices.slice(0) : null;
      }
      
      public function set instanceIndices(value:Array) : void
      {
         this._instanceIndices = value;
      }
      
      public function get repeater() : IRepeater
      {
         return Boolean(this._repeaters) ? this._repeaters[this._repeaters.length - 1] : null;
      }
      
      [Inspectable(environment="none")]
      public function get repeaters() : Array
      {
         return Boolean(this._repeaters) ? this._repeaters.slice(0) : [];
      }
      
      public function set repeaters(value:Array) : void
      {
         this._repeaters = value;
      }
      
      public function get repeaterIndex() : int
      {
         return Boolean(this._repeaterIndices) ? int(this._repeaterIndices[this._repeaterIndices.length - 1]) : -1;
      }
      
      [Inspectable(environment="none")]
      public function get repeaterIndices() : Array
      {
         return Boolean(this._repeaterIndices) ? this._repeaterIndices.slice() : [];
      }
      
      public function set repeaterIndices(value:Array) : void
      {
         this._repeaterIndices = value;
      }
      
      [Bindable("currentStateChange")]
      public function get currentState() : String
      {
         return this._currentStateChanged ? this.requestedCurrentState : this._currentState;
      }
      
      public function set currentState(value:String) : void
      {
         if(this._currentStateDeferred != null)
         {
            this._currentStateDeferred = value;
         }
         else
         {
            this.setCurrentState(value,true);
         }
      }
      
      mx_internal function get currentStateDeferred() : String
      {
         return this._currentStateDeferred != null ? this._currentStateDeferred : this.currentState;
      }
      
      mx_internal function set currentStateDeferred(value:String) : void
      {
         this._currentStateDeferred = value;
         if(value != null)
         {
            this.invalidateProperties();
         }
      }
      
      [ArrayElementType("mx.states.State")]
      [Inspectable(arrayType="mx.states.State")]
      public function get states() : Array
      {
         return this._states;
      }
      
      public function set states(value:Array) : void
      {
         this._states = value;
      }
      
      [ArrayElementType("mx.states.Transition")]
      [Inspectable(arrayType="mx.states.Transition")]
      public function get transitions() : Array
      {
         return this._transitions;
      }
      
      public function set transitions(value:Array) : void
      {
         this._transitions = value;
      }
      
      public function get baselinePosition() : Number
      {
         if(!this.validateBaselinePosition())
         {
            return NaN;
         }
         var lineMetrics:TextLineMetrics = this.measureText("Wj");
         if(this.height < 2 + lineMetrics.ascent + 2)
         {
            return int(this.height + (lineMetrics.ascent - this.height) / 2);
         }
         return 2 + lineMetrics.ascent;
      }
      
      public function get className() : String
      {
         return NameUtil.getUnqualifiedClassName(this);
      }
      
      public function get activeEffects() : Array
      {
         return this._effectsStarted;
      }
      
      public function get flexContextMenu() : IFlexContextMenu
      {
         return this._flexContextMenu;
      }
      
      public function set flexContextMenu(value:IFlexContextMenu) : void
      {
         if(Boolean(this._flexContextMenu))
         {
            this._flexContextMenu.unsetContextMenu(this);
         }
         this._flexContextMenu = value;
         if(value != null)
         {
            this._flexContextMenu.setContextMenu(this);
         }
      }
      
      [Inspectable(category="General")]
      public function get styleName() : Object
      {
         return this._styleName;
      }
      
      public function set styleName(value:Object) : void
      {
         if(this._styleName === value)
         {
            return;
         }
         this._styleName = value;
         if(this.inheritingStyles == StyleProtoChain.STYLE_UNINITIALIZED)
         {
            return;
         }
         this.regenerateStyleCache(true);
         this.initThemeColor();
         this.styleChanged("styleName");
         this.notifyStyleChangeInChildren("styleName",true);
      }
      
      [Inspectable(category="General",defaultValue="null")]
      [Bindable("toolTipChanged")]
      public function get toolTip() : String
      {
         return this._toolTip;
      }
      
      public function set toolTip(value:String) : void
      {
         var oldValue:String = this._toolTip;
         this._toolTip = value;
         ToolTipManager.registerToolTip(this,oldValue,value);
         this.dispatchEvent(new Event("toolTipChanged"));
      }
      
      public function get uid() : String
      {
         if(!this._uid)
         {
            this._uid = toString();
         }
         return this._uid;
      }
      
      public function set uid(uid:String) : void
      {
         this._uid = uid;
      }
      
      private function get indexedID() : String
      {
         var s:String = this.id;
         var indices:Array = this.instanceIndices;
         if(Boolean(indices))
         {
            s += "[" + indices.join("][") + "]";
         }
         return s;
      }
      
      [Inspectable(environment="none")]
      public function get isPopUp() : Boolean
      {
         return this._isPopUp;
      }
      
      public function set isPopUp(value:Boolean) : void
      {
         this._isPopUp = value;
      }
      
      public function get automationDelegate() : Object
      {
         return this._automationDelegate;
      }
      
      public function set automationDelegate(value:Object) : void
      {
         this._automationDelegate = value as IAutomationObject;
      }
      
      public function get automationName() : String
      {
         if(Boolean(this._automationName))
         {
            return this._automationName;
         }
         if(Boolean(this.automationDelegate))
         {
            return this.automationDelegate.automationName;
         }
         return "";
      }
      
      public function set automationName(value:String) : void
      {
         this._automationName = value;
      }
      
      public function get automationValue() : Array
      {
         if(Boolean(this.automationDelegate))
         {
            return this.automationDelegate.automationValue;
         }
         return [];
      }
      
      public function get showInAutomationHierarchy() : Boolean
      {
         return this._showInAutomationHierarchy;
      }
      
      public function set showInAutomationHierarchy(value:Boolean) : void
      {
         this._showInAutomationHierarchy = value;
      }
      
      [Bindable("errorStringChanged")]
      public function get errorString() : String
      {
         return this._errorString;
      }
      
      public function set errorString(value:String) : void
      {
         if(value == this._errorString)
         {
            return;
         }
         this.oldErrorString = this._errorString;
         this._errorString = value;
         this.errorStringChanged = true;
         this.invalidateProperties();
         this.dispatchEvent(new Event("errorStringChanged"));
      }
      
      private function setBorderColorForErrorString() : void
      {
         var focusManager:IFocusManager = null;
         var focusObj:DisplayObject = null;
         var showErrorSkin:Boolean = FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0 || Boolean(this.getStyle("showErrorSkin"));
         if(showErrorSkin)
         {
            if(!this._errorString || this._errorString.length == 0)
            {
               if(!isNaN(this.origBorderColor))
               {
                  this.setStyle("borderColor",this.origBorderColor);
                  this.saveBorderColor = true;
               }
            }
            else
            {
               if(this.saveBorderColor)
               {
                  this.saveBorderColor = false;
                  this.origBorderColor = this.getStyle("borderColor");
               }
               this.setStyle("borderColor",this.getStyle("errorColor"));
            }
            this.styleChanged("themeColor");
            focusManager = this.focusManager;
            focusObj = Boolean(focusManager) ? DisplayObject(focusManager.getFocus()) : null;
            if(Boolean(focusManager) && Boolean(focusManager.showFocusIndicator) && focusObj == this)
            {
               this.drawFocus(true);
            }
         }
      }
      
      public function get validationSubField() : String
      {
         return this._validationSubField;
      }
      
      public function set validationSubField(value:String) : void
      {
         this._validationSubField = value;
      }
      
      override public function addChild(child:DisplayObject) : DisplayObject
      {
         var formerParent:DisplayObjectContainer = child.parent;
         if(Boolean(formerParent) && !(formerParent is Loader))
         {
            formerParent.removeChild(child);
         }
         var index:int = Boolean(this.effectOverlayReferenceCount) && child != this.effectOverlay ? int(Math.max(0,super.numChildren - 1)) : super.numChildren;
         this.addingChild(child);
         this.$addChildAt(child,index);
         this.childAdded(child);
         return child;
      }
      
      override public function addChildAt(child:DisplayObject, index:int) : DisplayObject
      {
         var formerParent:DisplayObjectContainer = child.parent;
         if(Boolean(formerParent) && !(formerParent is Loader))
         {
            formerParent.removeChild(child);
         }
         if(Boolean(this.effectOverlayReferenceCount) && child != this.effectOverlay)
         {
            index = Math.min(index,Math.max(0,super.numChildren - 1));
         }
         this.addingChild(child);
         this.$addChildAt(child,index);
         this.childAdded(child);
         return child;
      }
      
      override public function removeChild(child:DisplayObject) : DisplayObject
      {
         this.removingChild(child);
         this.$removeChild(child);
         this.childRemoved(child);
         return child;
      }
      
      override public function removeChildAt(index:int) : DisplayObject
      {
         var child:DisplayObject = getChildAt(index);
         this.removingChild(child);
         this.$removeChild(child);
         this.childRemoved(child);
         return child;
      }
      
      override public function setChildIndex(child:DisplayObject, newIndex:int) : void
      {
         if(Boolean(this.effectOverlayReferenceCount) && child != this.effectOverlay)
         {
            newIndex = Math.min(newIndex,Math.max(0,super.numChildren - 2));
         }
         super.setChildIndex(child,newIndex);
      }
      
      override public function stopDrag() : void
      {
         super.stopDrag();
         this.invalidateProperties();
         this.dispatchEvent(new Event("xChanged"));
         this.dispatchEvent(new Event("yChanged"));
      }
      
      final mx_internal function $addChild(child:DisplayObject) : DisplayObject
      {
         return super.addChild(child);
      }
      
      final mx_internal function $addChildAt(child:DisplayObject, index:int) : DisplayObject
      {
         return super.addChildAt(child,index);
      }
      
      final mx_internal function $removeChild(child:DisplayObject) : DisplayObject
      {
         return super.removeChild(child);
      }
      
      final mx_internal function $removeChildAt(index:int) : DisplayObject
      {
         return super.removeChildAt(index);
      }
      
      final mx_internal function $setChildIndex(child:DisplayObject, index:int) : void
      {
         super.setChildIndex(child,index);
      }
      
      mx_internal function updateCallbacks() : void
      {
         if(this.invalidateDisplayListFlag)
         {
            UIComponentGlobals.layoutManager.invalidateDisplayList(this);
         }
         if(this.invalidateSizeFlag)
         {
            UIComponentGlobals.layoutManager.invalidateSize(this);
         }
         if(this.invalidatePropertiesFlag)
         {
            UIComponentGlobals.layoutManager.invalidateProperties(this);
         }
         if(Boolean(this.systemManager) && (Boolean(this._systemManager.stage) || Boolean(this.usingBridge)))
         {
            if(this.methodQueue.length > 0 && !this.listeningForRender)
            {
               this._systemManager.addEventListener(FlexEvent.RENDER,this.callLaterDispatcher);
               this._systemManager.addEventListener(FlexEvent.ENTER_FRAME,this.callLaterDispatcher);
               this.listeningForRender = true;
            }
            if(Boolean(this._systemManager.stage))
            {
               this._systemManager.stage.invalidate();
            }
         }
      }
      
      public function parentChanged(p:DisplayObjectContainer) : void
      {
         if(!p)
         {
            this._parent = null;
            this.nestLevel = 0;
         }
         else if(p is IStyleClient)
         {
            this._parent = p;
         }
         else if(p is ISystemManager)
         {
            this._parent = p;
         }
         else
         {
            this._parent = p.parent;
         }
         this.parentChangedFlag = true;
      }
      
      mx_internal function addingChild(child:DisplayObject) : void
      {
         if(child is IUIComponent && !IUIComponent(child).document)
         {
            IUIComponent(child).document = Boolean(this.document) ? this.document : FlexGlobals.topLevelApplication;
         }
         if(child is IFlexModule && IFlexModule(child).moduleFactory == null)
         {
            if(this.moduleFactory != null)
            {
               IFlexModule(child).moduleFactory = this.moduleFactory;
            }
            else if(this.document is IFlexModule && this.document.moduleFactory != null)
            {
               IFlexModule(child).moduleFactory = this.document.moduleFactory;
            }
            else if(this.parent is IFlexModule && IFlexModule(this.parent).moduleFactory != null)
            {
               IFlexModule(child).moduleFactory = IFlexModule(this.parent).moduleFactory;
            }
         }
         if(child is IFontContextComponent && !(child is UIComponent) && IFontContextComponent(child).fontContext == null)
         {
            IFontContextComponent(child).fontContext = this.moduleFactory;
         }
         if(child is IUIComponent)
         {
            IUIComponent(child).parentChanged(this);
         }
         if(child is ILayoutManagerClient)
         {
            ILayoutManagerClient(child).nestLevel = this.nestLevel + 1;
         }
         else if(child is IUITextField)
         {
            IUITextField(child).nestLevel = this.nestLevel + 1;
         }
         if(child is InteractiveObject)
         {
            if(this.doubleClickEnabled)
            {
               InteractiveObject(child).doubleClickEnabled = true;
            }
         }
         if(child is IStyleClient)
         {
            IStyleClient(child).regenerateStyleCache(true);
         }
         else if(child is IUITextField && Boolean(IUITextField(child).inheritingStyles))
         {
            StyleProtoChain.initTextField(IUITextField(child));
         }
         if(child is ISimpleStyleClient)
         {
            ISimpleStyleClient(child).styleChanged(null);
         }
         if(child is IStyleClient)
         {
            IStyleClient(child).notifyStyleChangeInChildren(null,true);
         }
         if(child is UIComponent)
         {
            UIComponent(child).initThemeColor();
         }
         if(child is UIComponent)
         {
            UIComponent(child).stylesInitialized();
         }
      }
      
      mx_internal function childAdded(child:DisplayObject) : void
      {
         var initializeErrorEvent:DynamicEvent = null;
         if(!UIComponentGlobals.designMode)
         {
            if(child is UIComponent)
            {
               if(!UIComponent(child).initialized)
               {
                  UIComponent(child).initialize();
               }
            }
            else if(child is IUIComponent)
            {
               IUIComponent(child).initialize();
            }
         }
         else
         {
            try
            {
               if(child is UIComponent)
               {
                  if(!UIComponent(child).initialized)
                  {
                     UIComponent(child).initialize();
                  }
               }
               else if(child is IUIComponent)
               {
                  IUIComponent(child).initialize();
               }
            }
            catch(e:Error)
            {
               initializeErrorEvent = new DynamicEvent("initializeError");
               initializeErrorEvent.error = e;
               initializeErrorEvent.source = child;
               systemManager.dispatchEvent(initializeErrorEvent);
            }
         }
      }
      
      mx_internal function removingChild(child:DisplayObject) : void
      {
      }
      
      mx_internal function childRemoved(child:DisplayObject) : void
      {
         if(child is IUIComponent)
         {
            if(IUIComponent(child).document != child)
            {
               IUIComponent(child).document = null;
            }
            IUIComponent(child).parentChanged(null);
         }
      }
      
      public function initialize() : void
      {
         if(this.initialized)
         {
            return;
         }
         this.dispatchEvent(new FlexEvent(FlexEvent.PREINITIALIZE));
         this.createChildren();
         this.childrenCreated();
         this.initializeAccessibility();
         this.initializationComplete();
      }
      
      protected function initializationComplete() : void
      {
         this.processedDescriptors = true;
      }
      
      protected function initializeAccessibility() : void
      {
         if(UIComponent.createAccessibilityImplementation != null)
         {
            UIComponent.createAccessibilityImplementation(this);
         }
      }
      
      public function initializeRepeaterArrays(parent:IRepeaterClient) : void
      {
         if(Boolean(parent && parent.instanceIndices) && (Boolean(!parent.isDocument || parent != this.descriptor.document)) && !this._instanceIndices)
         {
            this._instanceIndices = parent.instanceIndices;
            this._repeaters = parent.repeaters;
            this._repeaterIndices = parent.repeaterIndices;
         }
      }
      
      protected function createChildren() : void
      {
      }
      
      protected function childrenCreated() : void
      {
         this.invalidateProperties();
         this.invalidateSize();
         this.invalidateDisplayList();
      }
      
      public function invalidateProperties() : void
      {
         if(!this.invalidatePropertiesFlag)
         {
            this.invalidatePropertiesFlag = true;
            if(Boolean(this.nestLevel) && Boolean(UIComponentGlobals.layoutManager))
            {
               UIComponentGlobals.layoutManager.invalidateProperties(this);
            }
         }
      }
      
      public function invalidateSize() : void
      {
         if(!this.invalidateSizeFlag)
         {
            this.invalidateSizeFlag = true;
            if(Boolean(this.nestLevel) && Boolean(UIComponentGlobals.layoutManager))
            {
               UIComponentGlobals.layoutManager.invalidateSize(this);
            }
         }
      }
      
      protected function invalidateParentSizeAndDisplayList() : void
      {
         if(!this.includeInLayout)
         {
            return;
         }
         var p:IInvalidating = this.parent as IInvalidating;
         if(!p)
         {
            return;
         }
         p.invalidateSize();
         p.invalidateDisplayList();
      }
      
      public function invalidateDisplayList() : void
      {
         if(!this.invalidateDisplayListFlag)
         {
            this.invalidateDisplayListFlag = true;
            if(Boolean(this.nestLevel) && Boolean(UIComponentGlobals.layoutManager))
            {
               UIComponentGlobals.layoutManager.invalidateDisplayList(this);
            }
         }
      }
      
      private function invalidateTransform() : void
      {
         if(Boolean(this._layoutFeatures) && this._layoutFeatures.updatePending == false)
         {
            this._layoutFeatures.updatePending = true;
            if(Boolean(this.nestLevel) && Boolean(UIComponentGlobals.layoutManager) && this.invalidateDisplayListFlag == false)
            {
               UIComponentGlobals.layoutManager.invalidateDisplayList(this);
            }
         }
      }
      
      public function invalidateLayoutDirection() : void
      {
         var parentElt:ILayoutDirectionElement = null;
         var i:int = 0;
         var thisContainer:IVisualElementContainer = null;
         var thisContainerNumElements:int = 0;
         var elt:IVisualElement = null;
         var thisNumChildren:int = 0;
         var child:DisplayObject = null;
         parentElt = this.parent as ILayoutDirectionElement;
         var thisLayoutDirection:String = this.layoutDirection;
         var mirror:Boolean = Boolean(parentElt) ? parentElt.layoutDirection != thisLayoutDirection : LayoutDirection.LTR != thisLayoutDirection;
         if(Boolean(this._layoutFeatures) ? mirror != this._layoutFeatures.mirror : mirror)
         {
            if(this._layoutFeatures == null)
            {
               this.initAdvancedLayoutFeatures();
            }
            this._layoutFeatures.mirror = mirror;
            this._layoutFeatures.layoutWidth = this._width;
            this.invalidateTransform();
         }
         if(this.oldLayoutDirection != this.layoutDirection)
         {
            if(this is IVisualElementContainer)
            {
               thisContainer = IVisualElementContainer(this);
               thisContainerNumElements = thisContainer.numElements;
               for(i = 0; i < thisContainerNumElements; i++)
               {
                  elt = thisContainer.getElementAt(i);
                  if(Boolean(elt) && !(elt is IStyleClient))
                  {
                     elt.invalidateLayoutDirection();
                  }
               }
            }
            else
            {
               thisNumChildren = numChildren;
               for(i = 0; i < thisNumChildren; i++)
               {
                  child = getChildAt(i);
                  if(!(child is IStyleClient) && child is ILayoutDirectionElement)
                  {
                     ILayoutDirectionElement(child).invalidateLayoutDirection();
                  }
               }
            }
         }
      }
      
      private function transformOffsetsChangedHandler(e:Event) : void
      {
         this.invalidateTransform();
      }
      
      public function stylesInitialized() : void
      {
      }
      
      public function styleChanged(styleProp:String) : void
      {
         var allStyles:Boolean = !styleProp || styleProp == "styleName";
         StyleProtoChain.styleChanged(this,styleProp);
         if(!allStyles)
         {
            if(hasEventListener(styleProp + "Changed"))
            {
               this.dispatchEvent(new Event(styleProp + "Changed"));
            }
         }
         else if(hasEventListener("allStylesChanged"))
         {
            this.dispatchEvent(new Event("allStylesChanged"));
         }
         if(allStyles || styleProp == "layoutDirection")
         {
            this.layoutDirectionCachedValue = LAYOUT_DIRECTION_CACHE_UNSET;
         }
      }
      
      public function validateNow() : void
      {
         UIComponentGlobals.layoutManager.validateClient(this);
      }
      
      mx_internal function validateBaselinePosition() : Boolean
      {
         var w:Number = NaN;
         var h:Number = NaN;
         if(!this.parent)
         {
            return false;
         }
         if(!this.setActualSizeCalled && (this.width == 0 || this.height == 0))
         {
            this.validateNow();
            w = this.getExplicitOrMeasuredWidth();
            h = this.getExplicitOrMeasuredHeight();
            this.setActualSize(w,h);
         }
         this.validateNow();
         return true;
      }
      
      public function callLater(method:Function, args:Array = null) : void
      {
         this.methodQueue.push(new MethodQueueElement(method,args));
         var sm:ISystemManager = this.systemManager;
         if(Boolean(sm) && (Boolean(sm.stage) || Boolean(this.usingBridge)))
         {
            if(!this.listeningForRender)
            {
               sm.addEventListener(FlexEvent.RENDER,this.callLaterDispatcher);
               sm.addEventListener(FlexEvent.ENTER_FRAME,this.callLaterDispatcher);
               this.listeningForRender = true;
            }
            if(Boolean(sm.stage))
            {
               sm.stage.invalidate();
            }
         }
      }
      
      mx_internal function cancelAllCallLaters() : void
      {
         var sm:ISystemManager = this.systemManager;
         if(Boolean(sm) && (Boolean(sm.stage) || Boolean(this.usingBridge)))
         {
            if(this.listeningForRender)
            {
               sm.removeEventListener(FlexEvent.RENDER,this.callLaterDispatcher);
               sm.removeEventListener(FlexEvent.ENTER_FRAME,this.callLaterDispatcher);
               this.listeningForRender = false;
            }
         }
         this.methodQueue.splice(0);
      }
      
      public function validateProperties() : void
      {
         if(this.invalidatePropertiesFlag)
         {
            this.commitProperties();
            this.invalidatePropertiesFlag = false;
         }
      }
      
      protected function commitProperties() : void
      {
         var scalingFactorX:Number = NaN;
         var scalingFactorY:Number = NaN;
         var newState:String = null;
         var parentUIC:UIComponent = null;
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            if(this._scaleX != this.oldScaleX)
            {
               scalingFactorX = Math.abs(this._scaleX / this.oldScaleX);
               if(!isNaN(this.explicitMinWidth))
               {
                  this.explicitMinWidth *= scalingFactorX;
               }
               if(!isNaN(this.explicitWidth))
               {
                  this.explicitWidth *= scalingFactorX;
               }
               if(!isNaN(this.explicitMaxWidth))
               {
                  this.explicitMaxWidth *= scalingFactorX;
               }
               this._width *= scalingFactorX;
               super.scaleX = this.oldScaleX = this._scaleX;
            }
            if(this._scaleY != this.oldScaleY)
            {
               scalingFactorY = Math.abs(this._scaleY / this.oldScaleY);
               if(!isNaN(this.explicitMinHeight))
               {
                  this.explicitMinHeight *= scalingFactorY;
               }
               if(!isNaN(this.explicitHeight))
               {
                  this.explicitHeight *= scalingFactorY;
               }
               if(!isNaN(this.explicitMaxHeight))
               {
                  this.explicitMaxHeight *= scalingFactorY;
               }
               this._height *= scalingFactorY;
               super.scaleY = this.oldScaleY = this._scaleY;
            }
         }
         else
         {
            if(this._currentStateDeferred != null)
            {
               newState = this._currentStateDeferred;
               this._currentStateDeferred = null;
               this.currentState = newState;
            }
            this.oldScaleX = this.scaleX;
            this.oldScaleY = this.scaleY;
         }
         if(this._currentStateChanged && !this.initialized)
         {
            this._currentStateChanged = false;
            this.commitCurrentState();
         }
         if(FlexVersion.compatibilityVersion >= FlexVersion.VERSION_4_0)
         {
            parentUIC = this.parent as UIComponent;
            if(this.oldLayoutDirection != this.layoutDirection || this.parentChangedFlag || Boolean(parentUIC) && Boolean(parentUIC.layoutDirection != parentUIC.oldLayoutDirection))
            {
               this.invalidateLayoutDirection();
            }
         }
         if(this.x != this.oldX || this.y != this.oldY)
         {
            this.dispatchMoveEvent();
         }
         if(this.width != this.oldWidth || this.height != this.oldHeight)
         {
            this.dispatchResizeEvent();
         }
         if(this.errorStringChanged)
         {
            this.errorStringChanged = false;
            if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0 || Boolean(this.getStyle("showErrorTip")))
            {
               ToolTipManager.registerErrorString(this,this.oldErrorString,this.errorString);
            }
            this.setBorderColorForErrorString();
         }
         if(this.blendModeChanged)
         {
            this.blendModeChanged = false;
            if(!this.blendShaderChanged)
            {
               this.$blendMode = this._blendMode;
            }
            else
            {
               this.blendShaderChanged = false;
               this.$blendMode = BlendMode.NORMAL;
               switch(this._blendMode)
               {
                  case "color":
                     this.$blendShader = new ColorShader();
                     break;
                  case "colordodge":
                     this.$blendShader = new ColorDodgeShader();
                     break;
                  case "colorburn":
                     this.$blendShader = new ColorBurnShader();
                     break;
                  case "exclusion":
                     this.$blendShader = new ExclusionShader();
                     break;
                  case "hue":
                     this.$blendShader = new HueShader();
                     break;
                  case "luminosity":
                     this.$blendShader = new LuminosityShader();
                     break;
                  case "saturation":
                     this.$blendShader = new SaturationShader();
                     break;
                  case "softlight":
                     this.$blendShader = new SoftLightShader();
               }
            }
         }
         this.parentChangedFlag = false;
      }
      
      public function validateSize(recursive:Boolean = false) : void
      {
         var i:int = 0;
         var child:DisplayObject = null;
         var sizeChanging:Boolean = false;
         if(recursive)
         {
            for(i = 0; i < numChildren; i++)
            {
               child = getChildAt(i);
               if(child is ILayoutManagerClient)
               {
                  (child as ILayoutManagerClient).validateSize(true);
               }
            }
         }
         if(this.invalidateSizeFlag)
         {
            sizeChanging = this.measureSizes();
            if(sizeChanging && this.includeInLayout)
            {
               this.invalidateDisplayList();
               this.invalidateParentSizeAndDisplayList();
            }
         }
      }
      
      protected function canSkipMeasurement() : Boolean
      {
         return !isNaN(this.explicitWidth) && !isNaN(this.explicitHeight);
      }
      
      mx_internal function measureSizes() : Boolean
      {
         var scalingFactor:Number = NaN;
         var newValue:Number = NaN;
         var xScale:Number = NaN;
         var yScale:Number = NaN;
         var changed:Boolean = false;
         if(!this.invalidateSizeFlag)
         {
            return changed;
         }
         if(this.canSkipMeasurement())
         {
            this.invalidateSizeFlag = false;
            this._measuredMinWidth = 0;
            this._measuredMinHeight = 0;
         }
         else
         {
            xScale = Math.abs(this.scaleX);
            yScale = Math.abs(this.scaleY);
            if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
            {
               if(xScale != 1)
               {
                  this._measuredMinWidth /= xScale;
                  this._measuredWidth /= xScale;
               }
               if(yScale != 1)
               {
                  this._measuredMinHeight /= yScale;
                  this._measuredHeight /= yScale;
               }
            }
            this.measure();
            this.invalidateSizeFlag = false;
            if(!isNaN(this.explicitMinWidth) && this.measuredWidth < this.explicitMinWidth)
            {
               this.measuredWidth = this.explicitMinWidth;
            }
            if(!isNaN(this.explicitMaxWidth) && this.measuredWidth > this.explicitMaxWidth)
            {
               this.measuredWidth = this.explicitMaxWidth;
            }
            if(!isNaN(this.explicitMinHeight) && this.measuredHeight < this.explicitMinHeight)
            {
               this.measuredHeight = this.explicitMinHeight;
            }
            if(!isNaN(this.explicitMaxHeight) && this.measuredHeight > this.explicitMaxHeight)
            {
               this.measuredHeight = this.explicitMaxHeight;
            }
            if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
            {
               if(xScale != 1)
               {
                  this._measuredMinWidth *= xScale;
                  this._measuredWidth *= xScale;
               }
               if(yScale != 1)
               {
                  this._measuredMinHeight *= yScale;
                  this._measuredHeight *= yScale;
               }
            }
         }
         this.adjustSizesForScaleChanges();
         if(isNaN(this.oldMinWidth))
         {
            this.oldMinWidth = !isNaN(this.explicitMinWidth) ? this.explicitMinWidth : this.measuredMinWidth;
            this.oldMinHeight = !isNaN(this.explicitMinHeight) ? this.explicitMinHeight : this.measuredMinHeight;
            this.oldExplicitWidth = !isNaN(this.explicitWidth) ? this.explicitWidth : this.measuredWidth;
            this.oldExplicitHeight = !isNaN(this.explicitHeight) ? this.explicitHeight : this.measuredHeight;
            changed = true;
         }
         else
         {
            newValue = !isNaN(this.explicitMinWidth) ? this.explicitMinWidth : this.measuredMinWidth;
            if(newValue != this.oldMinWidth)
            {
               this.oldMinWidth = newValue;
               changed = true;
            }
            newValue = !isNaN(this.explicitMinHeight) ? this.explicitMinHeight : this.measuredMinHeight;
            if(newValue != this.oldMinHeight)
            {
               this.oldMinHeight = newValue;
               changed = true;
            }
            newValue = !isNaN(this.explicitWidth) ? this.explicitWidth : this.measuredWidth;
            if(newValue != this.oldExplicitWidth)
            {
               this.oldExplicitWidth = newValue;
               changed = true;
            }
            newValue = !isNaN(this.explicitHeight) ? this.explicitHeight : this.measuredHeight;
            if(newValue != this.oldExplicitHeight)
            {
               this.oldExplicitHeight = newValue;
               changed = true;
            }
         }
         return changed;
      }
      
      protected function measure() : void
      {
         this.measuredMinWidth = 0;
         this.measuredMinHeight = 0;
         this.measuredWidth = 0;
         this.measuredHeight = 0;
      }
      
      mx_internal function adjustSizesForScaleChanges() : void
      {
         var scalingFactor:Number = NaN;
         var xScale:Number = this.scaleX;
         var yScale:Number = this.scaleY;
         if(xScale != this.oldScaleX)
         {
            if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
            {
               scalingFactor = Math.abs(xScale / this.oldScaleX);
               if(Boolean(this.explicitMinWidth))
               {
                  this.explicitMinWidth *= scalingFactor;
               }
               if(!isNaN(this.explicitWidth))
               {
                  this.explicitWidth *= scalingFactor;
               }
               if(Boolean(this.explicitMaxWidth))
               {
                  this.explicitMaxWidth *= scalingFactor;
               }
            }
            this.oldScaleX = xScale;
         }
         if(yScale != this.oldScaleY)
         {
            if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
            {
               scalingFactor = Math.abs(yScale / this.oldScaleY);
               if(Boolean(this.explicitMinHeight))
               {
                  this.explicitMinHeight *= scalingFactor;
               }
               if(Boolean(this.explicitHeight))
               {
                  this.explicitHeight *= scalingFactor;
               }
               if(Boolean(this.explicitMaxHeight))
               {
                  this.explicitMaxHeight *= scalingFactor;
               }
            }
            this.oldScaleY = yScale;
         }
      }
      
      public function getExplicitOrMeasuredWidth() : Number
      {
         return !isNaN(this.explicitWidth) ? this.explicitWidth : this.measuredWidth;
      }
      
      public function getExplicitOrMeasuredHeight() : Number
      {
         return !isNaN(this.explicitHeight) ? this.explicitHeight : this.measuredHeight;
      }
      
      protected function get unscaledWidth() : Number
      {
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            return this.width / Math.abs(this.scaleX);
         }
         return this.width;
      }
      
      mx_internal function getUnscaledWidth() : Number
      {
         return this.unscaledWidth;
      }
      
      mx_internal function setUnscaledWidth(value:Number) : void
      {
         var newValue:Number = value;
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            newValue *= Math.abs(this.oldScaleX);
         }
         if(this._explicitWidth == newValue)
         {
            return;
         }
         if(!isNaN(newValue))
         {
            this._percentWidth = NaN;
         }
         this._explicitWidth = newValue;
         this.invalidateSize();
         this.invalidateParentSizeAndDisplayList();
      }
      
      protected function get unscaledHeight() : Number
      {
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            return this.height / Math.abs(this.scaleY);
         }
         return this.height;
      }
      
      mx_internal function getUnscaledHeight() : Number
      {
         return this.unscaledHeight;
      }
      
      mx_internal function setUnscaledHeight(value:Number) : void
      {
         var newValue:Number = value;
         if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            newValue *= Math.abs(this.oldScaleY);
         }
         if(this._explicitHeight == newValue)
         {
            return;
         }
         if(!isNaN(newValue))
         {
            this._percentHeight = NaN;
         }
         this._explicitHeight = newValue;
         this.invalidateSize();
         this.invalidateParentSizeAndDisplayList();
      }
      
      public function measureText(text:String) : TextLineMetrics
      {
         return this.determineTextFormatFromStyles().measureText(text);
      }
      
      public function measureHTMLText(htmlText:String) : TextLineMetrics
      {
         return this.determineTextFormatFromStyles().measureHTMLText(htmlText);
      }
      
      protected function validateMatrix() : void
      {
         var pmatrix:PerspectiveProjection = null;
         if(this._layoutFeatures != null && this._layoutFeatures.updatePending == true)
         {
            this.applyComputedMatrix();
         }
         if(this._maintainProjectionCenter)
         {
            pmatrix = super.transform.perspectiveProjection;
            if(pmatrix != null)
            {
               pmatrix.projectionCenter = new Point(this.unscaledWidth / 2,this.unscaledHeight / 2);
            }
         }
      }
      
      public function validateDisplayList() : void
      {
         var sm:ISystemManager = null;
         var unscaledWidth:Number = NaN;
         var unscaledHeight:Number = NaN;
         this.oldLayoutDirection = this.layoutDirection;
         if(this.invalidateDisplayListFlag)
         {
            sm = this.parent as ISystemManager;
            if(Boolean(sm))
            {
               if(sm.isProxy || sm == this.systemManager.topLevelSystemManager && sm.document != this)
               {
                  this.setActualSize(this.getExplicitOrMeasuredWidth(),this.getExplicitOrMeasuredHeight());
               }
            }
            this.validateMatrix();
            unscaledWidth = this.width;
            unscaledHeight = this.height;
            if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
            {
               unscaledWidth = this.scaleX == 0 ? 0 : this.width / this.scaleX;
               unscaledHeight = this.scaleY == 0 ? 0 : this.height / this.scaleY;
               if(Math.abs(unscaledWidth - this.lastUnscaledWidth) < 0.00001)
               {
                  unscaledWidth = this.lastUnscaledWidth;
               }
               if(Math.abs(unscaledHeight - this.lastUnscaledHeight) < 0.00001)
               {
                  unscaledHeight = this.lastUnscaledHeight;
               }
            }
            this.updateDisplayList(unscaledWidth,unscaledHeight);
            this.lastUnscaledWidth = unscaledWidth;
            this.lastUnscaledHeight = unscaledHeight;
            this.invalidateDisplayListFlag = false;
         }
         else
         {
            this.validateMatrix();
         }
      }
      
      protected function updateDisplayList(unscaledWidth:Number, unscaledHeight:Number) : void
      {
      }
      
      public function getConstraintValue(constraintName:String) : *
      {
         return this.getStyle(constraintName);
      }
      
      public function setConstraintValue(constraintName:String, value:*) : void
      {
         this.setStyle(constraintName,value);
      }
      
      [Inspectable(category="General")]
      public function get left() : Object
      {
         return this.getConstraintValue("left");
      }
      
      public function set left(value:Object) : void
      {
         this.setConstraintValue("left",value != null ? value : undefined);
      }
      
      [Inspectable(category="General")]
      public function get right() : Object
      {
         return this.getConstraintValue("right");
      }
      
      public function set right(value:Object) : void
      {
         this.setConstraintValue("right",value != null ? value : undefined);
      }
      
      [Inspectable(category="General")]
      public function get top() : Object
      {
         return this.getConstraintValue("top");
      }
      
      public function set top(value:Object) : void
      {
         this.setConstraintValue("top",value != null ? value : undefined);
      }
      
      [Inspectable(category="General")]
      public function get bottom() : Object
      {
         return this.getConstraintValue("bottom");
      }
      
      public function set bottom(value:Object) : void
      {
         this.setConstraintValue("bottom",value != null ? value : undefined);
      }
      
      [Inspectable(category="General")]
      public function get horizontalCenter() : Object
      {
         return this.getConstraintValue("horizontalCenter");
      }
      
      public function set horizontalCenter(value:Object) : void
      {
         this.setConstraintValue("horizontalCenter",value != null ? value : undefined);
      }
      
      [Inspectable(category="General")]
      public function get verticalCenter() : Object
      {
         return this.getConstraintValue("verticalCenter");
      }
      
      public function set verticalCenter(value:Object) : void
      {
         this.setConstraintValue("verticalCenter",value != null ? value : undefined);
      }
      
      [Inspectable(category="General")]
      public function get baseline() : Object
      {
         return this.getConstraintValue("baseline");
      }
      
      public function set baseline(value:Object) : void
      {
         this.setConstraintValue("baseline",value != null ? value : undefined);
      }
      
      public function horizontalGradientMatrix(x:Number, y:Number, width:Number, height:Number) : Matrix
      {
         UIComponentGlobals.tempMatrix.createGradientBox(width,height,0,x,y);
         return UIComponentGlobals.tempMatrix;
      }
      
      public function verticalGradientMatrix(x:Number, y:Number, width:Number, height:Number) : Matrix
      {
         UIComponentGlobals.tempMatrix.createGradientBox(width,height,Math.PI / 2,x,y);
         return UIComponentGlobals.tempMatrix;
      }
      
      public function drawRoundRect(x:Number, y:Number, w:Number, h:Number, r:Object = null, c:Object = null, alpha:Object = null, rot:Object = null, gradient:String = null, ratios:Array = null, hole:Object = null) : void
      {
         var ellipseSize:Number = NaN;
         var alphas:Array = null;
         var matrix:Matrix = null;
         var holeR:Object = null;
         var g:Graphics = graphics;
         if(!w || !h)
         {
            return;
         }
         if(c !== null)
         {
            if(c is Array)
            {
               if(alpha is Array)
               {
                  alphas = alpha as Array;
               }
               else
               {
                  alphas = [alpha,alpha];
               }
               if(!ratios)
               {
                  ratios = [0,255];
               }
               matrix = null;
               if(Boolean(rot))
               {
                  if(rot is Matrix)
                  {
                     matrix = Matrix(rot);
                  }
                  else
                  {
                     matrix = new Matrix();
                     if(rot is Number)
                     {
                        matrix.createGradientBox(w,h,Number(rot) * Math.PI / 180,x,y);
                     }
                     else
                     {
                        matrix.createGradientBox(rot.w,rot.h,rot.r,rot.x,rot.y);
                     }
                  }
               }
               if(gradient == GradientType.RADIAL)
               {
                  g.beginGradientFill(GradientType.RADIAL,c as Array,alphas,ratios,matrix);
               }
               else
               {
                  g.beginGradientFill(GradientType.LINEAR,c as Array,alphas,ratios,matrix);
               }
            }
            else
            {
               g.beginFill(Number(c),Number(alpha));
            }
         }
         if(!r)
         {
            g.drawRect(x,y,w,h);
         }
         else if(r is Number)
         {
            ellipseSize = Number(r) * 2;
            g.drawRoundRect(x,y,w,h,ellipseSize,ellipseSize);
         }
         else
         {
            GraphicsUtil.drawRoundRectComplex(g,x,y,w,h,r.tl,r.tr,r.bl,r.br);
         }
         if(Boolean(hole))
         {
            holeR = hole.r;
            if(holeR is Number)
            {
               ellipseSize = Number(holeR) * 2;
               g.drawRoundRect(hole.x,hole.y,hole.w,hole.h,ellipseSize,ellipseSize);
            }
            else
            {
               GraphicsUtil.drawRoundRectComplex(g,hole.x,hole.y,hole.w,hole.h,holeR.tl,holeR.tr,holeR.bl,holeR.br);
            }
         }
         if(c !== null)
         {
            g.endFill();
         }
      }
      
      public function move(x:Number, y:Number) : void
      {
         var changed:Boolean = false;
         if(x != this.x)
         {
            if(this._layoutFeatures == null)
            {
               super.x = x;
            }
            else
            {
               this._layoutFeatures.layoutX = x;
            }
            if(hasEventListener("xChanged"))
            {
               this.dispatchEvent(new Event("xChanged"));
            }
            changed = true;
         }
         if(y != this.y)
         {
            if(this._layoutFeatures == null)
            {
               super.y = y;
            }
            else
            {
               this._layoutFeatures.layoutY = y;
            }
            if(hasEventListener("yChanged"))
            {
               this.dispatchEvent(new Event("yChanged"));
            }
            changed = true;
         }
         if(changed)
         {
            this.invalidateTransform();
            this.dispatchMoveEvent();
         }
      }
      
      public function setActualSize(w:Number, h:Number) : void
      {
         var changed:Boolean = false;
         if(this._width != w)
         {
            this._width = w;
            if(Boolean(this._layoutFeatures))
            {
               this._layoutFeatures.layoutWidth = w;
               this.invalidateTransform();
            }
            if(hasEventListener("widthChanged"))
            {
               this.dispatchEvent(new Event("widthChanged"));
            }
            changed = true;
         }
         if(this._height != h)
         {
            this._height = h;
            if(hasEventListener("heightChanged"))
            {
               this.dispatchEvent(new Event("heightChanged"));
            }
            changed = true;
         }
         if(changed)
         {
            this.invalidateDisplayList();
            this.dispatchResizeEvent();
         }
         this.setActualSizeCalled = true;
      }
      
      public function contentToGlobal(point:Point) : Point
      {
         return localToGlobal(point);
      }
      
      public function globalToContent(point:Point) : Point
      {
         return globalToLocal(point);
      }
      
      public function contentToLocal(point:Point) : Point
      {
         return point;
      }
      
      public function localToContent(point:Point) : Point
      {
         return point;
      }
      
      public function getFocus() : InteractiveObject
      {
         var sm:ISystemManager = this.systemManager;
         if(!sm)
         {
            return null;
         }
         if(Boolean(UIComponentGlobals.nextFocusObject))
         {
            return UIComponentGlobals.nextFocusObject;
         }
         if(Boolean(sm.stage))
         {
            return sm.stage.focus;
         }
         return null;
      }
      
      public function setFocus() : void
      {
         var sm:ISystemManager = this.systemManager;
         if(Boolean(sm) && (Boolean(sm.stage) || Boolean(this.usingBridge)))
         {
            if(UIComponentGlobals.callLaterDispatcherCount == 0)
            {
               sm.stage.focus = this;
               UIComponentGlobals.nextFocusObject = null;
            }
            else
            {
               UIComponentGlobals.nextFocusObject = this;
               sm.addEventListener(FlexEvent.ENTER_FRAME,this.setFocusLater);
            }
         }
         else
         {
            UIComponentGlobals.nextFocusObject = this;
            this.callLater(this.setFocusLater);
         }
      }
      
      mx_internal function getFocusObject() : DisplayObject
      {
         var fm:IFocusManager = this.focusManager;
         if(!fm || !fm.focusPane)
         {
            return null;
         }
         return fm.focusPane.numChildren == 0 ? null : fm.focusPane.getChildAt(0);
      }
      
      public function drawFocus(isFocused:Boolean) : void
      {
         var focusOwner:DisplayObjectContainer = null;
         var focusClass:Class = null;
         if(!this.parent)
         {
            return;
         }
         var focusObj:DisplayObject = this.getFocusObject();
         var focusPane:Sprite = Boolean(this.focusManager) ? this.focusManager.focusPane : null;
         if(isFocused && !this.preventDrawFocus)
         {
            focusOwner = focusPane.parent;
            if(focusOwner != this.parent)
            {
               if(Boolean(focusOwner))
               {
                  if(focusOwner is ISystemManager)
                  {
                     ISystemManager(focusOwner).focusPane = null;
                  }
                  else
                  {
                     IUIComponent(focusOwner).focusPane = null;
                  }
               }
               if(this.parent is ISystemManager)
               {
                  ISystemManager(this.parent).focusPane = focusPane;
               }
               else
               {
                  IUIComponent(this.parent).focusPane = focusPane;
               }
            }
            focusClass = this.getStyle("focusSkin");
            if(!focusClass)
            {
               return;
            }
            if(Boolean(focusObj) && !(focusObj is focusClass))
            {
               focusPane.removeChild(focusObj);
               focusObj = null;
            }
            if(!focusObj)
            {
               focusObj = new focusClass();
               focusObj.name = "focus";
               focusPane.addChild(focusObj);
            }
            if(focusObj is ILayoutManagerClient)
            {
               ILayoutManagerClient(focusObj).nestLevel = this.nestLevel;
            }
            if(focusObj is ISimpleStyleClient)
            {
               ISimpleStyleClient(focusObj).styleName = this;
            }
            addEventListener(MoveEvent.MOVE,this.focusObj_moveHandler,true);
            addEventListener(MoveEvent.MOVE,this.focusObj_moveHandler);
            addEventListener(ResizeEvent.RESIZE,this.focusObj_resizeHandler,true);
            addEventListener(ResizeEvent.RESIZE,this.focusObj_resizeHandler);
            addEventListener(Event.REMOVED,this.focusObj_removedHandler,true);
            focusObj.visible = true;
            this.hasFocusRect = true;
            this.adjustFocusRect();
         }
         else if(this.hasFocusRect)
         {
            this.hasFocusRect = false;
            if(Boolean(focusObj))
            {
               focusObj.visible = false;
               if(focusObj is ISimpleStyleClient)
               {
                  ISimpleStyleClient(focusObj).styleName = null;
               }
            }
            removeEventListener(MoveEvent.MOVE,this.focusObj_moveHandler);
            removeEventListener(MoveEvent.MOVE,this.focusObj_moveHandler,true);
            removeEventListener(ResizeEvent.RESIZE,this.focusObj_resizeHandler,true);
            removeEventListener(ResizeEvent.RESIZE,this.focusObj_resizeHandler);
            removeEventListener(Event.REMOVED,this.focusObj_removedHandler,true);
         }
      }
      
      protected function adjustFocusRect(obj:DisplayObject = null) : void
      {
         var width:Number = NaN;
         var height:Number = NaN;
         var rectCol:Number = NaN;
         var showErrorSkin:Boolean = false;
         var thickness:Number = NaN;
         var pt:Point = null;
         var rotRad:Number = NaN;
         if(!obj)
         {
            obj = this;
         }
         if(obj is UIComponent)
         {
            width = UIComponent(obj).unscaledWidth * Math.abs(obj.scaleX);
            height = UIComponent(obj).unscaledHeight * Math.abs(obj.scaleY);
         }
         else
         {
            width = obj.width;
            height = obj.height;
         }
         if(isNaN(width) || isNaN(height))
         {
            return;
         }
         var fm:IFocusManager = this.focusManager;
         if(!fm)
         {
            return;
         }
         var focusObj:IFlexDisplayObject = IFlexDisplayObject(this.getFocusObject());
         if(Boolean(focusObj))
         {
            showErrorSkin = FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0 || Boolean(this.getStyle("showErrorSkin"));
            if(Boolean(this.errorString) && Boolean(this.errorString != "") && showErrorSkin)
            {
               rectCol = this.getStyle("errorColor");
            }
            else if(FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
            {
               rectCol = this.getStyle("themeColor");
            }
            else
            {
               rectCol = this.getStyle("focusColor");
            }
            thickness = this.getStyle("focusThickness");
            if(focusObj is IStyleClient)
            {
               IStyleClient(focusObj).setStyle("focusColor",rectCol);
            }
            focusObj.setActualSize(width + 2 * thickness,height + 2 * thickness);
            if(Boolean(this.rotation))
            {
               rotRad = this.rotation * Math.PI / 180;
               pt = new Point(obj.x - thickness * (Math.cos(rotRad) - Math.sin(rotRad)),obj.y - thickness * (Math.cos(rotRad) + Math.sin(rotRad)));
               DisplayObject(focusObj).rotation = this.rotation;
            }
            else
            {
               pt = new Point(obj.x - thickness,obj.y - thickness);
               DisplayObject(focusObj).rotation = 0;
            }
            if(obj.parent == this)
            {
               pt.x += this.x;
               pt.y += this.y;
            }
            if(obj != this)
            {
               if(Boolean(this._layoutFeatures) && this._layoutFeatures.mirror)
               {
                  pt.x += this.width - obj.width;
               }
            }
            pt = this.parent.localToGlobal(pt);
            pt = this.parent.globalToLocal(pt);
            focusObj.move(pt.x,pt.y);
            if(focusObj is IInvalidating)
            {
               IInvalidating(focusObj).validateNow();
            }
            else if(focusObj is IProgrammaticSkin)
            {
               IProgrammaticSkin(focusObj).validateNow();
            }
         }
      }
      
      protected function dispatchPropertyChangeEvent(prop:String, oldValue:*, value:*) : void
      {
         if(hasEventListener("propertyChange"))
         {
            this.dispatchEvent(PropertyChangeEvent.createUpdateEvent(this,prop,oldValue,value));
         }
      }
      
      private function dispatchMoveEvent() : void
      {
         var moveEvent:MoveEvent = null;
         if(hasEventListener(MoveEvent.MOVE))
         {
            moveEvent = new MoveEvent(MoveEvent.MOVE);
            moveEvent.oldX = this.oldX;
            moveEvent.oldY = this.oldY;
            this.dispatchEvent(moveEvent);
         }
         this.oldX = this.x;
         this.oldY = this.y;
      }
      
      private function dispatchResizeEvent() : void
      {
         var resizeEvent:ResizeEvent = null;
         if(hasEventListener(ResizeEvent.RESIZE))
         {
            resizeEvent = new ResizeEvent(ResizeEvent.RESIZE);
            resizeEvent.oldWidth = this.oldWidth;
            resizeEvent.oldHeight = this.oldHeight;
            this.dispatchEvent(resizeEvent);
         }
         this.oldWidth = this.width;
         this.oldHeight = this.height;
      }
      
      mx_internal function childXYChanged() : void
      {
      }
      
      mx_internal function mapKeycodeForLayoutDirection(event:KeyboardEvent, mapUpDown:Boolean = false) : uint
      {
         var keyCode:uint = event.keyCode;
         switch(keyCode)
         {
            case Keyboard.DOWN:
               if(mapUpDown && this.layoutDirection == LayoutDirection.RTL)
               {
                  keyCode = Keyboard.LEFT;
               }
               break;
            case Keyboard.RIGHT:
               if(this.layoutDirection == LayoutDirection.RTL)
               {
                  keyCode = Keyboard.LEFT;
               }
               break;
            case Keyboard.UP:
               if(mapUpDown && this.layoutDirection == LayoutDirection.RTL)
               {
                  keyCode = Keyboard.RIGHT;
               }
               break;
            case Keyboard.LEFT:
               if(this.layoutDirection == LayoutDirection.RTL)
               {
                  keyCode = Keyboard.RIGHT;
               }
         }
         return keyCode;
      }
      
      public function setCurrentState(stateName:String, playTransition:Boolean = true) : void
      {
         stateName = this.isBaseState(stateName) ? this.getDefaultState() : stateName;
         if(stateName != this.currentState && !(this.isBaseState(stateName) && this.isBaseState(this.currentState)))
         {
            this.requestedCurrentState = stateName;
            this.playStateTransition = this is IStateClient2 && this.isBaseState(this.currentState) ? false : playTransition;
            if(this.initialized)
            {
               this.commitCurrentState();
            }
            else
            {
               this._currentStateChanged = true;
               this.invalidateProperties();
            }
         }
      }
      
      public function hasState(stateName:String) : Boolean
      {
         return this.getState(stateName,false) != null;
      }
      
      private function isBaseState(stateName:String) : Boolean
      {
         return !stateName || stateName == "";
      }
      
      private function getDefaultState() : String
      {
         return this is IStateClient2 && this.states.length > 0 ? this.states[0].name : null;
      }
      
      private function commitCurrentState() : void
      {
         var event:StateChangeEvent = null;
         var prevTransitionEffect:Object = null;
         var tmpPropertyChanges:Array = null;
         var prevTransitionFraction:Number = NaN;
         var reverseTransition:Boolean = false;
         var nextTransition:Transition = this.playStateTransition ? this.getTransition(this._currentState,this.requestedCurrentState) : null;
         var commonBaseState:String = this.findCommonBaseState(this._currentState,this.requestedCurrentState);
         var oldState:String = Boolean(this._currentState) ? this._currentState : "";
         var destination:State = this.getState(this.requestedCurrentState);
         if(Boolean(nextTransition) && !effectLoaded)
         {
            effectLoaded = true;
            if(ApplicationDomain.currentDomain.hasDefinition("mx.effects.Effect"))
            {
               effectType = Class(ApplicationDomain.currentDomain.getDefinition("mx.effects.Effect"));
            }
         }
         if(Boolean(this._currentTransition))
         {
            this._currentTransition.effect.removeEventListener(EffectEvent.EFFECT_END,this.transition_effectEndHandler);
            if(Boolean(nextTransition) && this._currentTransition.interruptionBehavior == "stop")
            {
               prevTransitionEffect = this._currentTransition.effect;
               prevTransitionEffect.transitionInterruption = true;
               tmpPropertyChanges = prevTransitionEffect.propertyChangesArray;
               prevTransitionEffect.applyEndValuesWhenDone = false;
               prevTransitionEffect.stop();
               prevTransitionEffect.applyEndValuesWhenDone = true;
            }
            else
            {
               if(this._currentTransition.autoReverse && this.transitionFromState == this.requestedCurrentState && this.transitionToState == this._currentState)
               {
                  if(this._currentTransition.effect.duration == 0)
                  {
                     prevTransitionFraction = 0;
                  }
                  else
                  {
                     prevTransitionFraction = this._currentTransition.effect.playheadTime / this.getTotalDuration(this._currentTransition.effect);
                  }
               }
               this._currentTransition.effect.end();
            }
            if(hasEventListener(FlexEvent.STATE_CHANGE_INTERRUPTED))
            {
               this.dispatchEvent(new FlexEvent(FlexEvent.STATE_CHANGE_INTERRUPTED));
            }
            this._currentTransition = null;
         }
         this.initializeState(this.requestedCurrentState);
         if(Boolean(nextTransition))
         {
            nextTransition.effect.captureStartValues();
         }
         if(Boolean(tmpPropertyChanges))
         {
            prevTransitionEffect.applyEndValues(tmpPropertyChanges,prevTransitionEffect.targets);
         }
         if(hasEventListener(StateChangeEvent.CURRENT_STATE_CHANGING))
         {
            event = new StateChangeEvent(StateChangeEvent.CURRENT_STATE_CHANGING);
            event.oldState = oldState;
            event.newState = Boolean(this.requestedCurrentState) ? this.requestedCurrentState : "";
            this.dispatchEvent(event);
         }
         if(this.isBaseState(this._currentState) && hasEventListener(FlexEvent.EXIT_STATE))
         {
            this.dispatchEvent(new FlexEvent(FlexEvent.EXIT_STATE));
         }
         this.removeState(this._currentState,commonBaseState);
         this._currentState = this.requestedCurrentState;
         this.stateChanged(oldState,this._currentState,true);
         if(this.isBaseState(this.currentState))
         {
            if(hasEventListener(FlexEvent.ENTER_STATE))
            {
               this.dispatchEvent(new FlexEvent(FlexEvent.ENTER_STATE));
            }
         }
         else
         {
            this.applyState(this._currentState,commonBaseState);
         }
         if(hasEventListener(StateChangeEvent.CURRENT_STATE_CHANGE))
         {
            event = new StateChangeEvent(StateChangeEvent.CURRENT_STATE_CHANGE);
            event.oldState = oldState;
            event.newState = Boolean(this._currentState) ? this._currentState : "";
            this.dispatchEvent(event);
         }
         if(Boolean(nextTransition))
         {
            reverseTransition = Boolean(nextTransition) && nextTransition.autoReverse && (nextTransition.toState == oldState || nextTransition.fromState == this._currentState);
            UIComponentGlobals.layoutManager.validateNow();
            this._currentTransition = nextTransition;
            this.transitionFromState = oldState;
            this.transitionToState = this._currentState;
            Object(nextTransition.effect).transitionInterruption = prevTransitionEffect != null;
            nextTransition.effect.addEventListener(EffectEvent.EFFECT_END,this.transition_effectEndHandler);
            nextTransition.effect.play(null,reverseTransition);
            if(!isNaN(prevTransitionFraction) && nextTransition.effect.duration != 0)
            {
               nextTransition.effect.playheadTime = (1 - prevTransitionFraction) * this.getTotalDuration(nextTransition.effect);
            }
         }
         else if(hasEventListener(FlexEvent.STATE_CHANGE_COMPLETE))
         {
            this.dispatchEvent(new FlexEvent(FlexEvent.STATE_CHANGE_COMPLETE));
         }
      }
      
      private function getTotalDuration(effect:IEffect) : Number
      {
         var duration:Number = 0;
         var effectObj:Object = Object(effect);
         if(!compositeEffectLoaded)
         {
            compositeEffectLoaded = true;
            if(ApplicationDomain.currentDomain.hasDefinition("mx.effects.CompositeEffect"))
            {
               compositeEffectType = Class(ApplicationDomain.currentDomain.getDefinition("mx.effects.CompositeEffect"));
            }
         }
         if(Boolean(compositeEffectType) && effect is compositeEffectType)
         {
            duration = Number(effectObj.compositeDuration);
         }
         else
         {
            duration = effect.duration;
         }
         var repeatDelay:int = "repeatDelay" in effect ? int(effectObj.repeatDelay) : 0;
         var repeatCount:int = "repeatCount" in effect ? int(effectObj.repeatCount) : 0;
         var startDelay:int = "startDelay" in effect ? int(effectObj.startDelay) : 0;
         return duration * repeatCount + repeatDelay * (repeatCount - 1) + startDelay;
      }
      
      private function transition_effectEndHandler(event:EffectEvent) : void
      {
         this._currentTransition = null;
         if(hasEventListener(FlexEvent.STATE_CHANGE_COMPLETE))
         {
            this.dispatchEvent(new FlexEvent(FlexEvent.STATE_CHANGE_COMPLETE));
         }
      }
      
      private function getState(stateName:String, throwOnUndefined:Boolean = true) : State
      {
         var message:String = null;
         if(!this.states || this.isBaseState(stateName))
         {
            return null;
         }
         for(var i:int = 0; i < this.states.length; i++)
         {
            if(this.states[i].name == stateName)
            {
               return this.states[i];
            }
         }
         if(throwOnUndefined)
         {
            message = this.resourceManager.getString("core","stateUndefined",[stateName]);
            throw new ArgumentError(message);
         }
         return null;
      }
      
      private function findCommonBaseState(state1:String, state2:String) : String
      {
         var firstState:State = this.getState(state1);
         var secondState:State = this.getState(state2);
         if(!firstState || !secondState)
         {
            return "";
         }
         if(this.isBaseState(firstState.basedOn) && this.isBaseState(secondState.basedOn))
         {
            return "";
         }
         var firstBaseStates:Array = this.getBaseStates(firstState);
         var secondBaseStates:Array = this.getBaseStates(secondState);
         var commonBase:String = "";
         while(firstBaseStates[firstBaseStates.length - 1] == secondBaseStates[secondBaseStates.length - 1])
         {
            commonBase = firstBaseStates.pop();
            secondBaseStates.pop();
            if(!firstBaseStates.length || !secondBaseStates.length)
            {
               break;
            }
         }
         if(Boolean(firstBaseStates.length) && firstBaseStates[firstBaseStates.length - 1] == secondState.name)
         {
            commonBase = secondState.name;
         }
         else if(Boolean(secondBaseStates.length) && secondBaseStates[secondBaseStates.length - 1] == firstState.name)
         {
            commonBase = firstState.name;
         }
         return commonBase;
      }
      
      private function getBaseStates(state:State) : Array
      {
         var baseStates:Array = [];
         while(Boolean(state) && Boolean(state.basedOn))
         {
            baseStates.push(state.basedOn);
            state = this.getState(state.basedOn);
         }
         return baseStates;
      }
      
      private function removeState(stateName:String, lastState:String) : void
      {
         var overrides:Array = null;
         var i:int = 0;
         var state:State = this.getState(stateName);
         if(stateName == lastState)
         {
            return;
         }
         if(Boolean(state))
         {
            state.dispatchExitState();
            overrides = state.overrides;
            for(i = int(overrides.length); Boolean(i); i--)
            {
               overrides[i - 1].remove(this);
            }
            if(state.basedOn != lastState)
            {
               this.removeState(state.basedOn,lastState);
            }
         }
      }
      
      private function applyState(stateName:String, lastState:String) : void
      {
         var overrides:Array = null;
         var i:int = 0;
         var state:State = this.getState(stateName);
         if(stateName == lastState)
         {
            return;
         }
         if(Boolean(state))
         {
            if(state.basedOn != lastState)
            {
               this.applyState(state.basedOn,lastState);
            }
            overrides = state.overrides;
            for(i = 0; i < overrides.length; i++)
            {
               overrides[i].apply(this);
            }
            state.dispatchEnterState();
         }
      }
      
      private function initializeState(stateName:String) : void
      {
         var state:State = this.getState(stateName);
         while(Boolean(state))
         {
            state.initialize();
            state = this.getState(state.basedOn);
         }
      }
      
      private function getTransition(oldState:String, newState:String) : Transition
      {
         var t:Transition = null;
         var result:Transition = null;
         var priority:int = 0;
         if(!this.transitions)
         {
            return null;
         }
         if(!oldState)
         {
            oldState = "";
         }
         if(!newState)
         {
            newState = "";
         }
         for(var i:int = 0; i < this.transitions.length; i++)
         {
            t = this.transitions[i];
            if(t.fromState == "*" && t.toState == "*" && priority < 1)
            {
               result = t;
               priority = 1;
            }
            else if(t.toState == oldState && t.fromState == "*" && t.autoReverse && priority < 2)
            {
               result = t;
               priority = 2;
            }
            else if(t.toState == "*" && t.fromState == newState && t.autoReverse && priority < 3)
            {
               result = t;
               priority = 3;
            }
            else if(t.toState == oldState && t.fromState == newState && t.autoReverse && priority < 4)
            {
               result = t;
               priority = 4;
            }
            else if(t.fromState == oldState && t.toState == "*" && priority < 5)
            {
               result = t;
               priority = 5;
            }
            else if(t.fromState == "*" && t.toState == newState && priority < 6)
            {
               result = t;
               priority = 6;
            }
            else if(t.fromState == oldState && t.toState == newState && priority < 7)
            {
               result = t;
               priority = 7;
               break;
            }
         }
         if(Boolean(result) && !result.effect)
         {
            result = null;
         }
         return result;
      }
      
      protected function get currentCSSState() : String
      {
         return this.currentState;
      }
      
      public function get styleParent() : IAdvancedStyleClient
      {
         return this.parent as IAdvancedStyleClient;
      }
      
      public function set styleParent(parent:IAdvancedStyleClient) : void
      {
      }
      
      public function matchesCSSState(cssState:String) : Boolean
      {
         return this.currentCSSState == cssState;
      }
      
      public function matchesCSSType(cssType:String) : Boolean
      {
         return StyleProtoChain.matchesCSSType(this,cssType);
      }
      
      public function hasCSSState() : Boolean
      {
         return this.currentCSSState != null;
      }
      
      mx_internal function initProtoChain() : void
      {
         StyleProtoChain.initProtoChain(this);
      }
      
      public function getClassStyleDeclarations() : Array
      {
         return StyleProtoChain.getClassStyleDeclarations(this);
      }
      
      public function regenerateStyleCache(recursive:Boolean) : void
      {
         var child:Object = null;
         var styleClient:Object = null;
         var iAdvanceStyleClientChild:IAdvancedStyleClient = null;
         this.initProtoChain();
         var childList:IChildList = this is IRawChildrenContainer ? IRawChildrenContainer(this).rawChildren : IChildList(this);
         var n:int = childList.numChildren;
         for(var i:int = 0; i < n; i++)
         {
            child = childList.getChildAt(i);
            if(child is IStyleClient)
            {
               if(IStyleClient(child).inheritingStyles != StyleProtoChain.STYLE_UNINITIALIZED)
               {
                  IStyleClient(child).regenerateStyleCache(recursive);
               }
            }
            else if(child is IUITextField)
            {
               if(Boolean(IUITextField(child).inheritingStyles))
               {
                  StyleProtoChain.initTextField(IUITextField(child));
               }
            }
         }
         if(this.advanceStyleClientChildren != null)
         {
            for(styleClient in this.advanceStyleClientChildren)
            {
               iAdvanceStyleClientChild = styleClient as IAdvancedStyleClient;
               if(Boolean(iAdvanceStyleClientChild) && iAdvanceStyleClientChild.inheritingStyles != StyleProtoChain.STYLE_UNINITIALIZED)
               {
                  iAdvanceStyleClientChild.regenerateStyleCache(recursive);
               }
            }
         }
      }
      
      protected function stateChanged(oldState:String, newState:String, recursive:Boolean) : void
      {
         if(Boolean(this.currentCSSState) && Boolean(oldState != newState) && (this.styleManager.hasPseudoCondition(oldState) || this.styleManager.hasPseudoCondition(newState)))
         {
            this.regenerateStyleCache(recursive);
            this.initThemeColor();
            this.styleChanged(null);
            this.notifyStyleChangeInChildren(null,recursive);
         }
      }
      
      [Bindable(style="true")]
      public function getStyle(styleProp:String) : *
      {
         if(!this.moduleFactory)
         {
            if(Boolean(this.deferredSetStyles) && this.deferredSetStyles[styleProp] !== undefined)
            {
               return this.deferredSetStyles[styleProp];
            }
         }
         return Boolean(this.styleManager.inheritingStyles[styleProp]) ? this._inheritingStyles[styleProp] : this._nonInheritingStyles[styleProp];
      }
      
      public function setStyle(styleProp:String, newValue:*) : void
      {
         if(Boolean(this.moduleFactory))
         {
            StyleProtoChain.setStyle(this,styleProp,newValue);
         }
         else
         {
            if(!this.deferredSetStyles)
            {
               this.deferredSetStyles = new Object();
            }
            this.deferredSetStyles[styleProp] = newValue;
         }
      }
      
      private function setDeferredStyles() : void
      {
         var styleProp:String = null;
         if(!this.deferredSetStyles)
         {
            return;
         }
         for(styleProp in this.deferredSetStyles)
         {
            StyleProtoChain.setStyle(this,styleProp,this.deferredSetStyles[styleProp]);
         }
         this.deferredSetStyles = null;
      }
      
      public function clearStyle(styleProp:String) : void
      {
         this.setStyle(styleProp,undefined);
      }
      
      public function addStyleClient(styleClient:IAdvancedStyleClient) : void
      {
         var parentComponent:UIComponent = null;
         var message:String = null;
         if(!(styleClient is DisplayObject))
         {
            if(styleClient.styleParent != null)
            {
               parentComponent = styleClient.styleParent as UIComponent;
               if(Boolean(parentComponent))
               {
                  parentComponent.removeStyleClient(styleClient);
               }
            }
            if(this.advanceStyleClientChildren == null)
            {
               this.advanceStyleClientChildren = new Dictionary(true);
            }
            this.advanceStyleClientChildren[styleClient] = true;
            styleClient.styleParent = this;
            styleClient.regenerateStyleCache(true);
            styleClient.styleChanged(null);
            return;
         }
         message = this.resourceManager.getString("core","badParameter",[styleClient]);
         throw new ArgumentError(message);
      }
      
      public function removeStyleClient(styleClient:IAdvancedStyleClient) : void
      {
         if(Boolean(this.advanceStyleClientChildren) && Boolean(this.advanceStyleClientChildren[styleClient]))
         {
            delete this.advanceStyleClientChildren[styleClient];
            styleClient.styleParent = null;
            styleClient.regenerateStyleCache(true);
            styleClient.styleChanged(null);
         }
      }
      
      public function notifyStyleChangeInChildren(styleProp:String, recursive:Boolean) : void
      {
         var child:ISimpleStyleClient = null;
         var styleClient:Object = null;
         var iAdvanceStyleClientChild:IAdvancedStyleClient = null;
         this.cachedTextFormat = null;
         var n:int = numChildren;
         for(var i:int = 0; i < n; i++)
         {
            child = getChildAt(i) as ISimpleStyleClient;
            if(Boolean(child))
            {
               child.styleChanged(styleProp);
               if(child is IStyleClient)
               {
                  IStyleClient(child).notifyStyleChangeInChildren(styleProp,recursive);
               }
            }
         }
         if(this.advanceStyleClientChildren != null)
         {
            for(styleClient in this.advanceStyleClientChildren)
            {
               iAdvanceStyleClientChild = styleClient as IAdvancedStyleClient;
               if(Boolean(iAdvanceStyleClientChild))
               {
                  iAdvanceStyleClientChild.styleChanged(styleProp);
               }
            }
         }
      }
      
      mx_internal function initThemeColor() : Boolean
      {
         var tc:Object = null;
         var rc:Number = NaN;
         var sc:Number = NaN;
         var i:int = 0;
         var styleDeclarations:Array = null;
         var decl:CSSStyleDeclaration = null;
         var classSelector:Object = null;
         var typeSelectors:Array = null;
         var typeSelector:CSSStyleDeclaration = null;
         if(FlexVersion.compatibilityVersion >= FlexVersion.VERSION_4_0)
         {
            return true;
         }
         var styleName:Object = this._styleName;
         if(Boolean(this._styleDeclaration))
         {
            tc = this._styleDeclaration.getStyle("themeColor");
            rc = this._styleDeclaration.getStyle("rollOverColor");
            sc = this._styleDeclaration.getStyle("selectionColor");
         }
         if(this.styleManager.hasAdvancedSelectors())
         {
            if(tc === null || !this.styleManager.isValidStyleValue(tc))
            {
               styleDeclarations = StyleProtoChain.getMatchingStyleDeclarations(this);
               for(i = styleDeclarations.length - 1; i >= 0; i--)
               {
                  decl = styleDeclarations[i];
                  if(Boolean(decl))
                  {
                     tc = decl.getStyle("themeColor");
                     rc = decl.getStyle("rollOverColor");
                     sc = decl.getStyle("selectionColor");
                  }
                  if(tc !== null && Boolean(this.styleManager.isValidStyleValue(tc)))
                  {
                     break;
                  }
               }
            }
         }
         else
         {
            if((tc === null || !this.styleManager.isValidStyleValue(tc)) && (Boolean(styleName) && Boolean(!(styleName is ISimpleStyleClient))))
            {
               classSelector = styleName is String ? this.styleManager.getMergedStyleDeclaration("." + styleName) : styleName;
               if(Boolean(classSelector))
               {
                  tc = classSelector.getStyle("themeColor");
                  rc = Number(classSelector.getStyle("rollOverColor"));
                  sc = Number(classSelector.getStyle("selectionColor"));
               }
            }
            if(tc === null || !this.styleManager.isValidStyleValue(tc))
            {
               typeSelectors = this.getClassStyleDeclarations();
               for(i = 0; i < typeSelectors.length; i++)
               {
                  typeSelector = typeSelectors[i];
                  if(Boolean(typeSelector))
                  {
                     tc = typeSelector.getStyle("themeColor");
                     rc = typeSelector.getStyle("rollOverColor");
                     sc = typeSelector.getStyle("selectionColor");
                  }
                  if(tc !== null && Boolean(this.styleManager.isValidStyleValue(tc)))
                  {
                     break;
                  }
               }
            }
         }
         if(tc !== null && Boolean(this.styleManager.isValidStyleValue(tc)) && isNaN(rc) && isNaN(sc))
         {
            this.setThemeColor(tc);
            return true;
         }
         return tc !== null && Boolean(this.styleManager.isValidStyleValue(tc)) && !isNaN(rc) && !isNaN(sc);
      }
      
      mx_internal function setThemeColor(value:Object) : void
      {
         var newValue:Number = NaN;
         if(newValue is String)
         {
            newValue = parseInt(String(value));
         }
         else
         {
            newValue = Number(value);
         }
         if(isNaN(newValue))
         {
            newValue = Number(this.styleManager.getColorName(value));
         }
         var newValueS:Number = ColorUtil.adjustBrightness2(newValue,50);
         var newValueR:Number = ColorUtil.adjustBrightness2(newValue,70);
         this.setStyle("selectionColor",newValueS);
         this.setStyle("rollOverColor",newValueR);
      }
      
      public function determineTextFormatFromStyles() : UITextFormat
      {
         var font:String = null;
         var align:String = null;
         var textFormat:UITextFormat = this.cachedTextFormat;
         if(!textFormat)
         {
            font = StringUtil.trimArrayElements(this._inheritingStyles.fontFamily,",");
            textFormat = new UITextFormat(this.getNonNullSystemManager(),font);
            textFormat.moduleFactory = this.moduleFactory;
            align = this._inheritingStyles.textAlign;
            if(align == "start")
            {
               align = TextFormatAlign.LEFT;
            }
            else if(align == "end")
            {
               align = TextFormatAlign.RIGHT;
            }
            textFormat.align = align;
            textFormat.bold = this._inheritingStyles.fontWeight == "bold";
            textFormat.color = this.enabled ? this._inheritingStyles.color : this._inheritingStyles.disabledColor;
            textFormat.font = font;
            textFormat.indent = this._inheritingStyles.textIndent;
            textFormat.italic = this._inheritingStyles.fontStyle == "italic";
            textFormat.kerning = this._inheritingStyles.kerning;
            textFormat.leading = this._nonInheritingStyles.leading;
            textFormat.leftMargin = this._nonInheritingStyles.paddingLeft;
            textFormat.letterSpacing = this._inheritingStyles.letterSpacing;
            textFormat.rightMargin = this._nonInheritingStyles.paddingRight;
            textFormat.size = this._inheritingStyles.fontSize;
            textFormat.underline = this._nonInheritingStyles.textDecoration == "underline";
            textFormat.antiAliasType = this._inheritingStyles.fontAntiAliasType;
            textFormat.gridFitType = this._inheritingStyles.fontGridFitType;
            textFormat.sharpness = this._inheritingStyles.fontSharpness;
            textFormat.thickness = this._inheritingStyles.fontThickness;
            textFormat.useFTE = this.getTextFieldClassName() == "mx.core::UIFTETextField" || this.getTextInputClassName() == "mx.controls::MXFTETextInput";
            if(textFormat.useFTE)
            {
               textFormat.direction = this._inheritingStyles.direction;
               textFormat.locale = this._inheritingStyles.locale;
            }
            this.cachedTextFormat = textFormat;
         }
         return textFormat;
      }
      
      public function executeBindings(recurse:Boolean = false) : void
      {
         var bindingsHost:Object = Boolean(this.descriptor) && Boolean(this.descriptor.document) ? this.descriptor.document : this.parentDocument;
         BindingManager.executeBindings(bindingsHost,this.id,this);
      }
      
      public function registerEffects(effects:Array) : void
      {
         var event:String = null;
         var n:int = int(effects.length);
         for(var i:int = 0; i < n; i++)
         {
            event = EffectManager.getEventForEffectTrigger(effects[i]);
            if(event != null && event != "")
            {
               addEventListener(event,EffectManager.eventHandler,false,EventPriority.EFFECT);
            }
         }
      }
      
      mx_internal function addOverlay(color:uint, targetArea:RoundedRectangle = null) : void
      {
         if(!this.effectOverlay)
         {
            this.effectOverlayColor = color;
            this.effectOverlay = new UIComponent();
            this.effectOverlay.name = "overlay";
            this.effectOverlay.$visible = true;
            this.fillOverlay(this.effectOverlay,color,targetArea);
            this.attachOverlay();
            if(!targetArea)
            {
               addEventListener(ResizeEvent.RESIZE,this.overlay_resizeHandler);
            }
            this.effectOverlay.x = 0;
            this.effectOverlay.y = 0;
            this.invalidateDisplayList();
            this.effectOverlayReferenceCount = 1;
         }
         else
         {
            ++this.effectOverlayReferenceCount;
         }
         this.dispatchEvent(new ChildExistenceChangedEvent(ChildExistenceChangedEvent.OVERLAY_CREATED,true,false,this.effectOverlay));
      }
      
      protected function attachOverlay() : void
      {
         this.addChild(this.effectOverlay);
      }
      
      mx_internal function fillOverlay(overlay:UIComponent, color:uint, targetArea:RoundedRectangle = null) : void
      {
         if(!targetArea)
         {
            targetArea = new RoundedRectangle(0,0,this.unscaledWidth,this.unscaledHeight,0);
         }
         var g:Graphics = overlay.graphics;
         g.clear();
         g.beginFill(color);
         g.drawRoundRect(targetArea.x,targetArea.y,targetArea.width,targetArea.height,targetArea.cornerRadius * 2,targetArea.cornerRadius * 2);
         g.endFill();
      }
      
      mx_internal function removeOverlay() : void
      {
         if(this.effectOverlayReferenceCount > 0 && --this.effectOverlayReferenceCount == 0 && Boolean(this.effectOverlay))
         {
            removeEventListener(ResizeEvent.RESIZE,this.overlay_resizeHandler);
            if(Boolean(super.getChildByName("overlay")))
            {
               this.$removeChild(this.effectOverlay);
            }
            this.effectOverlay = null;
         }
      }
      
      private function overlay_resizeHandler(event:Event) : void
      {
         this.fillOverlay(this.effectOverlay,this.effectOverlayColor,null);
      }
      
      mx_internal function get isEffectStarted() : Boolean
      {
         return this._isEffectStarted;
      }
      
      mx_internal function set isEffectStarted(value:Boolean) : void
      {
         this._isEffectStarted = value;
      }
      
      public function effectStarted(effectInst:IEffectInstance) : void
      {
         var propName:String = null;
         this._effectsStarted.push(effectInst);
         var aProps:Array = effectInst.effect.getAffectedProperties();
         for(var j:int = 0; j < aProps.length; j++)
         {
            propName = aProps[j];
            if(this._affectedProperties[propName] == undefined)
            {
               this._affectedProperties[propName] = [];
            }
            this._affectedProperties[propName].push(effectInst);
         }
         this.isEffectStarted = true;
         if(effectInst.hideFocusRing)
         {
            this.preventDrawFocus = true;
            this.drawFocus(false);
         }
      }
      
      public function effectFinished(effectInst:IEffectInstance) : void
      {
         this._endingEffectInstances.push(effectInst);
         this.invalidateProperties();
         UIComponentGlobals.layoutManager.addEventListener(FlexEvent.UPDATE_COMPLETE,this.updateCompleteHandler,false,0,true);
      }
      
      public function endEffectsStarted() : void
      {
         var len:int = int(this._effectsStarted.length);
         for(var i:int = 0; i < len; i++)
         {
            this._effectsStarted[i].end();
         }
      }
      
      private function updateCompleteHandler(event:FlexEvent) : void
      {
         UIComponentGlobals.layoutManager.removeEventListener(FlexEvent.UPDATE_COMPLETE,this.updateCompleteHandler);
         this.processEffectFinished(this._endingEffectInstances);
         this._endingEffectInstances = [];
      }
      
      private function processEffectFinished(effectInsts:Array) : void
      {
         var j:int = 0;
         var effectInst:IEffectInstance = null;
         var removedInst:IEffectInstance = null;
         var aProps:Array = null;
         var k:int = 0;
         var propName:String = null;
         var l:int = 0;
         for(var i:int = this._effectsStarted.length - 1; i >= 0; i--)
         {
            for(j = 0; j < effectInsts.length; j++)
            {
               effectInst = effectInsts[j];
               if(effectInst == this._effectsStarted[i])
               {
                  removedInst = this._effectsStarted[i];
                  this._effectsStarted.splice(i,1);
                  aProps = removedInst.effect.getAffectedProperties();
                  for(k = 0; k < aProps.length; k++)
                  {
                     propName = aProps[k];
                     if(this._affectedProperties[propName] != undefined)
                     {
                        for(l = 0; l < this._affectedProperties[propName].length; l++)
                        {
                           if(this._affectedProperties[propName][l] == effectInst)
                           {
                              this._affectedProperties[propName].splice(l,1);
                              break;
                           }
                        }
                        if(this._affectedProperties[propName].length == 0)
                        {
                           delete this._affectedProperties[propName];
                        }
                     }
                  }
                  break;
               }
            }
         }
         this.isEffectStarted = this._effectsStarted.length > 0 ? true : false;
         if(Boolean(effectInst) && effectInst.hideFocusRing)
         {
            this.preventDrawFocus = false;
         }
      }
      
      mx_internal function getEffectsForProperty(propertyName:String) : Array
      {
         return this._affectedProperties[propertyName] != undefined ? this._affectedProperties[propertyName] : [];
      }
      
      public function createReferenceOnParentDocument(parentDocument:IFlexDisplayObject) : void
      {
         var indices:Array = null;
         var r:Object = null;
         var n:int = 0;
         var i:int = 0;
         var s:Object = null;
         var event:PropertyChangeEvent = null;
         if(Boolean(this.id) && this.id != "")
         {
            indices = this._instanceIndices;
            if(!indices)
            {
               parentDocument[this.id] = this;
            }
            else
            {
               r = parentDocument[this.id];
               if(!(r is Array))
               {
                  r = parentDocument[this.id] = [];
               }
               n = int(indices.length);
               for(i = 0; i < n - 1; i++)
               {
                  s = r[indices[i]];
                  if(!(s is Array))
                  {
                     s = r[indices[i]] = [];
                  }
                  r = s;
               }
               r[indices[n - 1]] = this;
               if(parentDocument.hasEventListener("propertyChange"))
               {
                  event = PropertyChangeEvent.createUpdateEvent(parentDocument,this.id,parentDocument[this.id],parentDocument[this.id]);
                  parentDocument.dispatchEvent(event);
               }
            }
         }
      }
      
      public function deleteReferenceOnParentDocument(parentDocument:IFlexDisplayObject) : void
      {
         var indices:Array = null;
         var r:Object = null;
         var stack:Array = null;
         var n:int = 0;
         var i:int = 0;
         var j:int = 0;
         var s:Object = null;
         var event:PropertyChangeEvent = null;
         if(Boolean(this.id) && this.id != "")
         {
            indices = this._instanceIndices;
            if(!indices)
            {
               parentDocument[this.id] = null;
            }
            else
            {
               r = parentDocument[this.id];
               if(!r)
               {
                  return;
               }
               stack = [];
               stack.push(r);
               n = int(indices.length);
               for(i = 0; i < n - 1; i++)
               {
                  s = r[indices[i]];
                  if(!s)
                  {
                     return;
                  }
                  r = s;
                  stack.push(r);
               }
               r.splice(indices[n - 1],1);
               for(j = stack.length - 1; j > 0; j--)
               {
                  if(stack[j].length == 0)
                  {
                     stack[j - 1].splice(indices[j],1);
                  }
               }
               if(stack.length > 0 && stack[0].length == 0)
               {
                  parentDocument[this.id] = null;
               }
               else if(parentDocument.hasEventListener("propertyChange"))
               {
                  event = PropertyChangeEvent.createUpdateEvent(parentDocument,this.id,parentDocument[this.id],parentDocument[this.id]);
                  parentDocument.dispatchEvent(event);
               }
            }
         }
      }
      
      public function getRepeaterItem(whichRepeater:int = -1) : Object
      {
         var repeaterArray:Array = this.repeaters;
         if(repeaterArray.length == 0)
         {
            return null;
         }
         if(whichRepeater == -1)
         {
            whichRepeater = repeaterArray.length - 1;
         }
         return repeaterArray[whichRepeater].getItemAt(this.repeaterIndices[whichRepeater]);
      }
      
      protected function resourcesChanged() : void
      {
      }
      
      public function prepareToPrint(target:IFlexDisplayObject) : Object
      {
         return null;
      }
      
      public function finishPrint(obj:Object, target:IFlexDisplayObject) : void
      {
      }
      
      private function callLaterDispatcher(event:Event) : void
      {
         var callLaterErrorEvent:DynamicEvent = null;
         ++UIComponentGlobals.callLaterDispatcherCount;
         if(!UIComponentGlobals.catchCallLaterExceptions)
         {
            this.callLaterDispatcher2(event);
         }
         else
         {
            try
            {
               this.callLaterDispatcher2(event);
            }
            catch(e:Error)
            {
               callLaterErrorEvent = new DynamicEvent("callLaterError");
               callLaterErrorEvent.error = e;
               callLaterErrorEvent.source = this;
               systemManager.dispatchEvent(callLaterErrorEvent);
            }
         }
         --UIComponentGlobals.callLaterDispatcherCount;
      }
      
      private function callLaterDispatcher2(event:Event) : void
      {
         var mqe:MethodQueueElement = null;
         if(UIComponentGlobals.callLaterSuspendCount > 0)
         {
            return;
         }
         var sm:ISystemManager = this.systemManager;
         if(Boolean(sm) && (Boolean(sm.stage || this.usingBridge)) && this.listeningForRender)
         {
            sm.removeEventListener(FlexEvent.RENDER,this.callLaterDispatcher);
            sm.removeEventListener(FlexEvent.ENTER_FRAME,this.callLaterDispatcher);
            this.listeningForRender = false;
         }
         var queue:Array = this.methodQueue;
         this.methodQueue = [];
         var n:int = int(queue.length);
         for(var i:int = 0; i < n; i++)
         {
            mqe = MethodQueueElement(queue[i]);
            mqe.method.apply(null,mqe.args);
         }
      }
      
      protected function keyDownHandler(event:KeyboardEvent) : void
      {
      }
      
      protected function keyUpHandler(event:KeyboardEvent) : void
      {
      }
      
      protected function isOurFocus(target:DisplayObject) : Boolean
      {
         return target == this;
      }
      
      protected function focusInHandler(event:FocusEvent) : void
      {
         var fm:IFocusManager = null;
         if(this.isOurFocus(DisplayObject(event.target)))
         {
            fm = this.focusManager;
            if(Boolean(fm) && fm.showFocusIndicator)
            {
               this.drawFocus(true);
            }
            ContainerGlobals.checkFocus(event.relatedObject,this);
         }
      }
      
      protected function focusOutHandler(event:FocusEvent) : void
      {
         if(this.isOurFocus(DisplayObject(event.target)))
         {
            this.drawFocus(false);
         }
      }
      
      private function addedHandler(event:Event) : void
      {
         if(event.eventPhase != EventPhase.AT_TARGET)
         {
            return;
         }
         try
         {
            if(this.parent is IContainer && IContainer(this.parent).creatingContentPane)
            {
               event.stopImmediatePropagation();
               return;
            }
         }
         catch(error:SecurityError)
         {
         }
      }
      
      private function removedHandler(event:Event) : void
      {
         if(event.eventPhase != EventPhase.AT_TARGET)
         {
            return;
         }
         try
         {
            if(this.parent is IContainer && IContainer(this.parent).creatingContentPane)
            {
               event.stopImmediatePropagation();
               return;
            }
         }
         catch(error:SecurityError)
         {
         }
      }
      
      private function removedFromStageHandler(event:Event) : void
      {
         this._systemManagerDirty = true;
      }
      
      private function setFocusLater(event:Event = null) : void
      {
         var sm:ISystemManager = this.systemManager;
         if(Boolean(sm) && Boolean(sm.stage))
         {
            sm.stage.removeEventListener(Event.ENTER_FRAME,this.setFocusLater);
            if(Boolean(UIComponentGlobals.nextFocusObject))
            {
               sm.stage.focus = UIComponentGlobals.nextFocusObject;
            }
            UIComponentGlobals.nextFocusObject = null;
         }
      }
      
      private function focusObj_scrollHandler(event:Event) : void
      {
         this.adjustFocusRect();
      }
      
      private function focusObj_moveHandler(event:MoveEvent) : void
      {
         this.adjustFocusRect();
      }
      
      private function focusObj_resizeHandler(event:Event) : void
      {
         if(event is ResizeEvent)
         {
            this.adjustFocusRect();
         }
      }
      
      private function focusObj_removedHandler(event:Event) : void
      {
         if(event.target != this)
         {
            return;
         }
         var focusObject:DisplayObject = this.getFocusObject();
         if(Boolean(focusObject))
         {
            focusObject.visible = false;
         }
      }
      
      protected function layer_PropertyChange(event:PropertyChangeEvent) : void
      {
         var newValue:Boolean = false;
         var newAlpha:Number = NaN;
         switch(event.property)
         {
            case "effectiveVisibility":
               newValue = Boolean(event.newValue) && this._visible;
               if(newValue != this.$visible)
               {
                  this.$visible = newValue;
               }
               break;
            case "effectiveAlpha":
               newAlpha = Number(event.newValue) * this._alpha;
               if(newAlpha != this.$alpha)
               {
                  this.$alpha = newAlpha;
               }
         }
      }
      
      public function validationResultHandler(event:ValidationResultEvent) : void
      {
         var msg:String = null;
         var result:ValidationResult = null;
         var i:int = 0;
         if(this.errorObjectArray === null)
         {
            this.errorObjectArray = new Array();
            this.errorArray = new Array();
         }
         var validatorIndex:int = this.errorObjectArray.indexOf(event.target);
         if(event.type == ValidationResultEvent.VALID)
         {
            if(validatorIndex != -1)
            {
               this.errorObjectArray.splice(validatorIndex,1);
               this.errorArray.splice(validatorIndex,1);
               this.errorString = this.errorArray.join("\n");
               if(this.errorArray.length == 0)
               {
                  this.dispatchEvent(new FlexEvent(FlexEvent.VALID));
               }
            }
         }
         else
         {
            if(this.validationSubField != null && this.validationSubField != "" && Boolean(event.results))
            {
               for(i = 0; i < event.results.length; i++)
               {
                  result = event.results[i];
                  if(result.subField == this.validationSubField)
                  {
                     if(result.isError)
                     {
                        msg = result.errorMessage;
                     }
                     else if(validatorIndex != -1)
                     {
                        this.errorObjectArray.splice(validatorIndex,1);
                        this.errorArray.splice(validatorIndex,1);
                        this.errorString = this.errorArray.join("\n");
                        if(this.errorArray.length == 0)
                        {
                           this.dispatchEvent(new FlexEvent(FlexEvent.VALID));
                        }
                     }
                     break;
                  }
               }
            }
            else if(Boolean(event.results) && event.results.length > 0)
            {
               msg = event.results[0].errorMessage;
            }
            if(Boolean(msg) && validatorIndex != -1)
            {
               this.errorArray[validatorIndex] = msg;
               this.errorString = this.errorArray.join("\n");
               this.dispatchEvent(new FlexEvent(FlexEvent.INVALID));
            }
            else if(Boolean(msg) && validatorIndex == -1)
            {
               this.errorObjectArray.push(event.target);
               this.errorArray.push(msg);
               this.errorString = this.errorArray.join("\n");
               this.dispatchEvent(new FlexEvent(FlexEvent.INVALID));
            }
         }
      }
      
      private function resourceManager_changeHandler(event:Event) : void
      {
         this.resourcesChanged();
      }
      
      private function filterChangeHandler(event:Event) : void
      {
         this.filters = this._filters;
      }
      
      public function owns(child:DisplayObject) : Boolean
      {
         var childList:IChildList = this is IRawChildrenContainer ? IRawChildrenContainer(this).rawChildren : IChildList(this);
         if(childList.contains(child))
         {
            return true;
         }
         try
         {
            while(Boolean(child) && child != this)
            {
               if(child is IUIComponent)
               {
                  child = IUIComponent(child).owner;
               }
               else
               {
                  child = child.parent;
               }
            }
         }
         catch(e:SecurityError)
         {
            return false;
         }
         return child == this;
      }
      
      mx_internal function getFontContext(fontName:String, bold:Boolean, italic:Boolean, embeddedCff:* = undefined) : IFlexModuleFactory
      {
         if(noEmbeddedFonts)
         {
            return null;
         }
         var registry:IEmbeddedFontRegistry = embeddedFontRegistry;
         return Boolean(registry) ? registry.getAssociatedModuleFactory(fontName,bold,italic,this,this.moduleFactory,this.systemManager,embeddedCff) : null;
      }
      
      protected function createInFontContext(classObj:Class) : Object
      {
         this.hasFontContextBeenSaved = true;
         var fontName:String = StringUtil.trimArrayElements(this.getStyle("fontFamily"),",");
         var fontWeight:String = this.getStyle("fontWeight");
         var fontStyle:String = this.getStyle("fontStyle");
         var bold:Boolean = fontWeight == "bold";
         var italic:Boolean = fontStyle == "italic";
         var className:String = getQualifiedClassName(classObj);
         if(className == "mx.core::UITextField")
         {
            className = this.getTextFieldClassName();
            if(className == "mx.core::UIFTETextField")
            {
               classObj = Class(ApplicationDomain.currentDomain.getDefinition(className));
            }
         }
         this.oldEmbeddedFontContext = this.getFontContext(fontName,bold,italic,className == "mx.core::UIFTETextField");
         var moduleContext:IFlexModuleFactory = Boolean(this.oldEmbeddedFontContext) ? this.oldEmbeddedFontContext : this.moduleFactory;
         var obj:Object = this.createInModuleContext(moduleContext,className);
         if(obj == null)
         {
            obj = new classObj();
         }
         if(className == "mx.core::UIFTETextField")
         {
            obj.fontContext = moduleContext;
         }
         return obj;
      }
      
      private function getTextFieldClassName() : String
      {
         var c:Class = this.getStyle("textFieldClass");
         if(!c || FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            return "mx.core::UITextField";
         }
         return getQualifiedClassName(c);
      }
      
      private function getTextInputClassName() : String
      {
         var c:Class = this.getStyle("textInputClass");
         if(!c || FlexVersion.compatibilityVersion < FlexVersion.VERSION_4_0)
         {
            return "mx.core::TextInput";
         }
         return getQualifiedClassName(c);
      }
      
      protected function createInModuleContext(moduleFactory:IFlexModuleFactory, className:String) : Object
      {
         var newObject:Object = null;
         if(Boolean(moduleFactory))
         {
            newObject = moduleFactory.create(className);
         }
         return newObject;
      }
      
      public function hasFontContextChanged() : Boolean
      {
         if(!this.hasFontContextBeenSaved)
         {
            return false;
         }
         var fontName:String = StringUtil.trimArrayElements(this.getStyle("fontFamily"),",");
         var fontWeight:String = this.getStyle("fontWeight");
         var fontStyle:String = this.getStyle("fontStyle");
         var bold:Boolean = fontWeight == "bold";
         var italic:Boolean = fontStyle == "italic";
         var fontContext:IFlexModuleFactory = noEmbeddedFonts ? null : embeddedFontRegistry.getAssociatedModuleFactory(fontName,bold,italic,this,this.moduleFactory,this.systemManager);
         return fontContext != this.oldEmbeddedFontContext;
      }
      
      public function createAutomationIDPart(child:IAutomationObject) : Object
      {
         if(Boolean(this.automationDelegate))
         {
            return this.automationDelegate.createAutomationIDPart(child);
         }
         return null;
      }
      
      public function createAutomationIDPartWithRequiredProperties(child:IAutomationObject, properties:Array) : Object
      {
         if(Boolean(this.automationDelegate))
         {
            return this.automationDelegate.createAutomationIDPartWithRequiredProperties(child,properties);
         }
         return null;
      }
      
      public function resolveAutomationIDPart(criteria:Object) : Array
      {
         if(Boolean(this.automationDelegate))
         {
            return this.automationDelegate.resolveAutomationIDPart(criteria);
         }
         return [];
      }
      
      public function getAutomationChildAt(index:int) : IAutomationObject
      {
         if(Boolean(this.automationDelegate))
         {
            return this.automationDelegate.getAutomationChildAt(index);
         }
         return null;
      }
      
      public function getAutomationChildren() : Array
      {
         if(Boolean(this.automationDelegate))
         {
            return this.automationDelegate.getAutomationChildren();
         }
         return null;
      }
      
      public function get numAutomationChildren() : int
      {
         if(Boolean(this.automationDelegate))
         {
            return this.automationDelegate.numAutomationChildren;
         }
         return 0;
      }
      
      public function get automationTabularData() : Object
      {
         if(Boolean(this.automationDelegate))
         {
            return this.automationDelegate.automationTabularData;
         }
         return null;
      }
      
      public function get automationOwner() : DisplayObjectContainer
      {
         return this.owner;
      }
      
      public function get automationParent() : DisplayObjectContainer
      {
         return this.parent;
      }
      
      public function get automationEnabled() : Boolean
      {
         return this.enabled;
      }
      
      public function get automationVisible() : Boolean
      {
         return this.visible;
      }
      
      public function replayAutomatableEvent(event:Event) : Boolean
      {
         if(Boolean(this.automationDelegate))
         {
            return this.automationDelegate.replayAutomatableEvent(event);
         }
         return false;
      }
      
      public function getVisibleRect(targetParent:DisplayObject = null) : Rectangle
      {
         if(!targetParent)
         {
            targetParent = DisplayObject(this.systemManager);
         }
         var thisParent:DisplayObject = Boolean(this.$parent) ? this.$parent : this.parent;
         if(!thisParent)
         {
            return new Rectangle();
         }
         var pt:Point = new Point(this.x,this.y);
         pt = thisParent.localToGlobal(pt);
         var bounds:Rectangle = new Rectangle(pt.x,pt.y,this.width,this.height);
         var current:DisplayObject = this;
         var currentRect:Rectangle = new Rectangle();
         do
         {
            if(current is UIComponent)
            {
               if(Boolean(UIComponent(current).$parent))
               {
                  current = UIComponent(current).$parent;
               }
               else
               {
                  current = UIComponent(current).parent;
               }
            }
            else
            {
               current = current.parent;
            }
            if(Boolean(current) && Boolean(current.scrollRect))
            {
               currentRect = current.scrollRect.clone();
               pt = current.localToGlobal(currentRect.topLeft);
               currentRect.x = pt.x;
               currentRect.y = pt.y;
               bounds = bounds.intersection(currentRect);
            }
         }
         while(Boolean(current) && current != targetParent);
         return bounds;
      }
      
      override public function dispatchEvent(event:Event) : Boolean
      {
         if(dispatchEventHook != null)
         {
            dispatchEventHook(event,this);
         }
         return super.dispatchEvent(event);
      }
      
      override public function get mouseX() : Number
      {
         if(!root || root is Stage || root[fakeMouseX] === undefined)
         {
            return super.mouseX;
         }
         return globalToLocal(new Point(root[fakeMouseX],0)).x;
      }
      
      override public function get mouseY() : Number
      {
         if(!root || root is Stage || root[fakeMouseY] === undefined)
         {
            return super.mouseY;
         }
         return globalToLocal(new Point(0,root[fakeMouseY])).y;
      }
      
      protected function initAdvancedLayoutFeatures() : void
      {
         this.internal_initAdvancedLayoutFeatures();
      }
      
      mx_internal function transformRequiresValidations() : Boolean
      {
         return this._layoutFeatures != null;
      }
      
      mx_internal function clearAdvancedLayoutFeatures() : void
      {
         if(Boolean(this._layoutFeatures))
         {
            this.validateMatrix();
            this._layoutFeatures = null;
         }
      }
      
      private function internal_initAdvancedLayoutFeatures() : AdvancedLayoutFeatures
      {
         var features:AdvancedLayoutFeatures = new AdvancedLayoutFeatures();
         this._hasComplexLayoutMatrix = true;
         features.layoutScaleX = this.scaleX;
         features.layoutScaleY = this.scaleY;
         features.layoutScaleZ = this.scaleZ;
         features.layoutRotationX = this.rotationX;
         features.layoutRotationY = this.rotationY;
         features.layoutRotationZ = this.rotation;
         features.layoutX = this.x;
         features.layoutY = this.y;
         features.layoutZ = this.z;
         features.layoutWidth = this.width;
         this._layoutFeatures = features;
         this.invalidateTransform();
         return features;
      }
      
      private function setTransform(value:flash.geom.Transform) : void
      {
         var oldTransform:mx.geom.Transform = this._transform as Transform;
         if(Boolean(oldTransform))
         {
            oldTransform.target = null;
         }
         var newTransform:mx.geom.Transform = value as Transform;
         if(Boolean(newTransform))
         {
            newTransform.target = this;
         }
         this._transform = value;
      }
      
      mx_internal function get $transform() : flash.geom.Transform
      {
         return super.transform;
      }
      
      override public function get transform() : flash.geom.Transform
      {
         if(this._transform == null)
         {
            this.setTransform(new mx.geom.Transform(this));
         }
         return this._transform;
      }
      
      override public function set transform(value:flash.geom.Transform) : void
      {
         var m:Matrix = value.matrix;
         var m3:Matrix3D = value.matrix3D;
         var ct:ColorTransform = value.colorTransform;
         var pp:PerspectiveProjection = value.perspectiveProjection;
         var was3D:Boolean = this.is3D;
         var mxTransform:mx.geom.Transform = value as Transform;
         if(Boolean(mxTransform))
         {
            if(!mxTransform.applyMatrix)
            {
               m = null;
            }
            if(!mxTransform.applyMatrix3D)
            {
               m3 = null;
            }
         }
         this.setTransform(value);
         if(m != null)
         {
            this.setLayoutMatrix(m.clone(),true);
         }
         else if(m3 != null)
         {
            this.setLayoutMatrix3D(m3.clone(),true);
         }
         super.transform.colorTransform = ct;
         super.transform.perspectiveProjection = pp;
         if(this.maintainProjectionCenter)
         {
            this.invalidateDisplayList();
         }
         if(was3D != this.is3D)
         {
            this.validateMatrix();
         }
      }
      
      public function get postLayoutTransformOffsets() : TransformOffsets
      {
         return this._layoutFeatures != null ? this._layoutFeatures.postLayoutTransformOffsets : null;
      }
      
      public function set postLayoutTransformOffsets(value:TransformOffsets) : void
      {
         var was3D:Boolean = this.is3D;
         if(this._layoutFeatures == null)
         {
            this.initAdvancedLayoutFeatures();
         }
         if(this._layoutFeatures.postLayoutTransformOffsets != null)
         {
            this._layoutFeatures.postLayoutTransformOffsets.removeEventListener(Event.CHANGE,this.transformOffsetsChangedHandler);
         }
         this._layoutFeatures.postLayoutTransformOffsets = value;
         if(this._layoutFeatures.postLayoutTransformOffsets != null)
         {
            this._layoutFeatures.postLayoutTransformOffsets.addEventListener(Event.CHANGE,this.transformOffsetsChangedHandler);
         }
         if(was3D != this.is3D)
         {
            this.validateMatrix();
         }
         this.invalidateTransform();
      }
      
      public function set maintainProjectionCenter(value:Boolean) : void
      {
         this._maintainProjectionCenter = value;
         if(value && super.transform.perspectiveProjection == null)
         {
            super.transform.perspectiveProjection = new PerspectiveProjection();
         }
         this.invalidateDisplayList();
      }
      
      public function get maintainProjectionCenter() : Boolean
      {
         return this._maintainProjectionCenter;
      }
      
      public function setLayoutMatrix(value:Matrix, invalidateLayout:Boolean) : void
      {
         var previousMatrix:Matrix = Boolean(this._layoutFeatures) ? this._layoutFeatures.layoutMatrix : super.transform.matrix;
         var was3D:Boolean = this.is3D;
         this._hasComplexLayoutMatrix = true;
         if(this._layoutFeatures == null)
         {
            super.transform.matrix = value;
         }
         else
         {
            this._layoutFeatures.layoutMatrix = value;
            this.invalidateTransform();
         }
         if(MatrixUtil.isEqual(previousMatrix,Boolean(this._layoutFeatures) ? this._layoutFeatures.layoutMatrix : super.transform.matrix))
         {
            return;
         }
         this.invalidateProperties();
         if(invalidateLayout)
         {
            this.invalidateParentSizeAndDisplayList();
         }
         if(was3D != this.is3D)
         {
            this.validateMatrix();
         }
      }
      
      public function setLayoutMatrix3D(value:Matrix3D, invalidateLayout:Boolean) : void
      {
         if(Boolean(this._layoutFeatures) && MatrixUtil.isEqual3D(this._layoutFeatures.layoutMatrix3D,value))
         {
            return;
         }
         var was3D:Boolean = this.is3D;
         if(this._layoutFeatures == null)
         {
            this.initAdvancedLayoutFeatures();
         }
         this._layoutFeatures.layoutMatrix3D = value;
         this.invalidateTransform();
         this.invalidateProperties();
         if(invalidateLayout)
         {
            this.invalidateParentSizeAndDisplayList();
         }
         if(was3D != this.is3D)
         {
            this.validateMatrix();
         }
      }
      
      public function transformAround(transformCenter:Vector3D, scale:Vector3D = null, rotation:Vector3D = null, translation:Vector3D = null, postLayoutScale:Vector3D = null, postLayoutRotation:Vector3D = null, postLayoutTranslation:Vector3D = null, invalidateLayout:Boolean = true) : void
      {
         var oldIncludeInLayout:Boolean = false;
         if(!invalidateLayout)
         {
            oldIncludeInLayout = this._includeInLayout;
            this._includeInLayout = false;
         }
         var prevX:Number = this.x;
         var prevY:Number = this.y;
         var prevZ:Number = this.z;
         TransformUtil.transformAround(this,transformCenter,scale,rotation,translation,postLayoutScale,postLayoutRotation,postLayoutTranslation,this._layoutFeatures,this.internal_initAdvancedLayoutFeatures);
         if(this._layoutFeatures != null)
         {
            this.invalidateTransform();
            this.invalidateParentSizeAndDisplayList();
            if(prevX != this._layoutFeatures.layoutX)
            {
               this.dispatchEvent(new Event("xChanged"));
            }
            if(prevY != this._layoutFeatures.layoutY)
            {
               this.dispatchEvent(new Event("yChanged"));
            }
            if(prevZ != this._layoutFeatures.layoutZ)
            {
               this.dispatchEvent(new Event("zChanged"));
            }
         }
         if(!invalidateLayout)
         {
            this._includeInLayout = oldIncludeInLayout;
         }
      }
      
      public function transformPointToParent(localPosition:Vector3D, position:Vector3D, postLayoutPosition:Vector3D) : void
      {
         TransformUtil.transformPointToParent(this,localPosition,position,postLayoutPosition,this._layoutFeatures);
      }
      
      public function set layoutMatrix3D(value:Matrix3D) : void
      {
         this.setLayoutMatrix3D(value,true);
      }
      
      public function get depth() : Number
      {
         return this._layoutFeatures == null ? 0 : this._layoutFeatures.depth;
      }
      
      public function set depth(value:Number) : void
      {
         if(value == this.depth)
         {
            return;
         }
         if(this._layoutFeatures == null)
         {
            this.initAdvancedLayoutFeatures();
         }
         this._layoutFeatures.depth = value;
         if(this.parent is UIComponent)
         {
            UIComponent(this.parent).invalidateLayering();
         }
      }
      
      public function invalidateLayering() : void
      {
      }
      
      protected function applyComputedMatrix() : void
      {
         this._layoutFeatures.updatePending = false;
         if(this._layoutFeatures.is3D)
         {
            super.transform.matrix3D = this._layoutFeatures.computedMatrix3D;
         }
         else
         {
            super.transform.matrix = this._layoutFeatures.computedMatrix;
         }
      }
      
      mx_internal function get computedMatrix() : Matrix
      {
         return Boolean(this._layoutFeatures) ? this._layoutFeatures.computedMatrix : this.transform.matrix;
      }
      
      protected function setStretchXY(stretchX:Number, stretchY:Number) : void
      {
         if(this._layoutFeatures == null)
         {
            this.initAdvancedLayoutFeatures();
         }
         if(stretchX != this._layoutFeatures.stretchX || stretchY != this._layoutFeatures.stretchY)
         {
            this._layoutFeatures.stretchX = stretchX;
            this._layoutFeatures.stretchY = stretchY;
            this.invalidateTransform();
         }
      }
      
      public function getPreferredBoundsWidth(postLayoutTransform:Boolean = true) : Number
      {
         return LayoutElementUIComponentUtils.getPreferredBoundsWidth(this,postLayoutTransform ? this.nonDeltaLayoutMatrix() : null);
      }
      
      public function getPreferredBoundsHeight(postLayoutTransform:Boolean = true) : Number
      {
         return LayoutElementUIComponentUtils.getPreferredBoundsHeight(this,postLayoutTransform ? this.nonDeltaLayoutMatrix() : null);
      }
      
      public function getMinBoundsWidth(postLayoutTransform:Boolean = true) : Number
      {
         return LayoutElementUIComponentUtils.getMinBoundsWidth(this,postLayoutTransform ? this.nonDeltaLayoutMatrix() : null);
      }
      
      public function getMinBoundsHeight(postLayoutTransform:Boolean = true) : Number
      {
         return LayoutElementUIComponentUtils.getMinBoundsHeight(this,postLayoutTransform ? this.nonDeltaLayoutMatrix() : null);
      }
      
      public function getMaxBoundsWidth(postLayoutTransform:Boolean = true) : Number
      {
         return LayoutElementUIComponentUtils.getMaxBoundsWidth(this,postLayoutTransform ? this.nonDeltaLayoutMatrix() : null);
      }
      
      public function getMaxBoundsHeight(postLayoutTransform:Boolean = true) : Number
      {
         return LayoutElementUIComponentUtils.getMaxBoundsHeight(this,postLayoutTransform ? this.nonDeltaLayoutMatrix() : null);
      }
      
      public function getBoundsXAtSize(width:Number, height:Number, postLayoutTransform:Boolean = true) : Number
      {
         return LayoutElementUIComponentUtils.getBoundsXAtSize(this,width,height,postLayoutTransform ? this.nonDeltaLayoutMatrix() : null);
      }
      
      public function getBoundsYAtSize(width:Number, height:Number, postLayoutTransform:Boolean = true) : Number
      {
         return LayoutElementUIComponentUtils.getBoundsYAtSize(this,width,height,postLayoutTransform ? this.nonDeltaLayoutMatrix() : null);
      }
      
      public function getLayoutBoundsWidth(postLayoutTransform:Boolean = true) : Number
      {
         return LayoutElementUIComponentUtils.getLayoutBoundsWidth(this,postLayoutTransform ? this.nonDeltaLayoutMatrix() : null);
      }
      
      public function getLayoutBoundsHeight(postLayoutTransform:Boolean = true) : Number
      {
         return LayoutElementUIComponentUtils.getLayoutBoundsHeight(this,postLayoutTransform ? this.nonDeltaLayoutMatrix() : null);
      }
      
      public function getLayoutBoundsX(postLayoutTransform:Boolean = true) : Number
      {
         return LayoutElementUIComponentUtils.getLayoutBoundsX(this,postLayoutTransform ? this.nonDeltaLayoutMatrix() : null);
      }
      
      public function getLayoutBoundsY(postLayoutTransform:Boolean = true) : Number
      {
         return LayoutElementUIComponentUtils.getLayoutBoundsY(this,postLayoutTransform ? this.nonDeltaLayoutMatrix() : null);
      }
      
      public function setLayoutBoundsPosition(x:Number, y:Number, postLayoutTransform:Boolean = true) : void
      {
         LayoutElementUIComponentUtils.setLayoutBoundsPosition(this,x,y,postLayoutTransform ? this.nonDeltaLayoutMatrix() : null);
      }
      
      public function setLayoutBoundsSize(width:Number, height:Number, postLayoutTransform:Boolean = true) : void
      {
         LayoutElementUIComponentUtils.setLayoutBoundsSize(this,width,height,postLayoutTransform ? this.nonDeltaLayoutMatrix() : null);
      }
      
      public function getLayoutMatrix() : Matrix
      {
         if(this._layoutFeatures != null || super.transform.matrix == null)
         {
            if(this._layoutFeatures == null)
            {
               this.initAdvancedLayoutFeatures();
            }
            return this._layoutFeatures.layoutMatrix.clone();
         }
         return super.transform.matrix;
      }
      
      public function get hasLayoutMatrix3D() : Boolean
      {
         return Boolean(this._layoutFeatures) ? this._layoutFeatures.layoutIs3D : false;
      }
      
      public function get is3D() : Boolean
      {
         return Boolean(this._layoutFeatures) ? this._layoutFeatures.is3D : false;
      }
      
      public function getLayoutMatrix3D() : Matrix3D
      {
         if(this._layoutFeatures == null)
         {
            this.initAdvancedLayoutFeatures();
         }
         return this._layoutFeatures.layoutMatrix3D.clone();
      }
      
      protected function nonDeltaLayoutMatrix() : Matrix
      {
         if(!this.hasComplexLayoutMatrix)
         {
            return null;
         }
         if(this._layoutFeatures != null)
         {
            return this._layoutFeatures.layoutMatrix;
         }
         return super.transform.matrix;
      }
   }
}

class MethodQueueElement
{
   
   public var method:Function;
   
   public var args:Array;
   
   public function MethodQueueElement(method:Function, args:Array = null)
   {
      super();
      this.method = method;
      this.args = args;
   }
}

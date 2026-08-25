package mx.styles
{
   import flash.display.DisplayObject;
   import flash.display.DisplayObjectContainer;
   import flash.system.ApplicationDomain;
   import flash.utils.getQualifiedClassName;
   import flash.utils.getQualifiedSuperclassName;
   import mx.core.FlexGlobals;
   import mx.core.IFlexDisplayObject;
   import mx.core.IFlexModule;
   import mx.core.IFlexModuleFactory;
   import mx.core.IFontContextComponent;
   import mx.core.IInvalidating;
   import mx.core.IUITextField;
   import mx.core.IVisualElement;
   import mx.core.UIComponent;
   import mx.core.mx_internal;
   import mx.effects.EffectManager;
   import mx.managers.SystemManager;
   import mx.modules.IModule;
   import mx.modules.ModuleManager;
   import mx.utils.NameUtil;
   import mx.utils.OrderedObject;
   import mx.utils.object_proxy;
   
   use namespace mx_internal;
   use namespace object_proxy;
   
   [ExcludeClass]
   public class StyleProtoChain
   {
      
      mx_internal static const VERSION:String = "4.6.0.23201";
      
      public static var STYLE_UNINITIALIZED:Object = {};
      
      public function StyleProtoChain()
      {
         super();
      }
      
      public static function getClassStyleDeclarations(object:IStyleClient) : Array
      {
         var type:String = null;
         var decls:Object = null;
         var matchingDecls:Array = null;
         var decl:CSSStyleDeclaration = null;
         var styleManager:IStyleManager2 = getStyleManager(object);
         var qualified:Boolean = styleManager.qualifiedTypeSelectors;
         var className:String = qualified ? getQualifiedClassName(object) : object.className;
         var advancedObject:IAdvancedStyleClient = object as IAdvancedStyleClient;
         var typeHierarchy:OrderedObject = getTypeHierarchy(object,qualified);
         var types:Array = typeHierarchy.propertyList;
         var typeCount:int = int(types.length);
         var classDecls:Array = null;
         if(!styleManager.hasAdvancedSelectors())
         {
            classDecls = styleManager.typeSelectorCache[className];
            if(Boolean(classDecls))
            {
               return classDecls;
            }
         }
         classDecls = [];
         for(var i:int = typeCount - 1; i >= 0; i--)
         {
            type = types[i].toString();
            if(styleManager.hasAdvancedSelectors() && advancedObject != null)
            {
               decls = styleManager.getStyleDeclarations(type);
               if(Boolean(decls))
               {
                  matchingDecls = matchStyleDeclarations(decls,advancedObject);
                  classDecls = classDecls.concat(matchingDecls);
               }
            }
            else
            {
               decl = styleManager.getMergedStyleDeclaration(type);
               if(Boolean(decl))
               {
                  classDecls.push(decl);
               }
            }
         }
         if(styleManager.hasAdvancedSelectors() && advancedObject != null)
         {
            classDecls = sortOnSpecificity(classDecls);
         }
         else
         {
            styleManager.typeSelectorCache[className] = classDecls;
         }
         return classDecls;
      }
      
      public static function initProtoChain(object:IStyleClient, inheritPopUpStylesFromOwner:Boolean = true) : void
      {
         var n:int = 0;
         var i:int = 0;
         var inheritChain:Object = null;
         var owner:DisplayObjectContainer = null;
         var styleNames:Array = null;
         var styleManager:IStyleManager2 = getStyleManager(object);
         var uicObject:UIComponent = object as UIComponent;
         var advancedObject:IAdvancedStyleClient = object as IAdvancedStyleClient;
         var styleDeclaration:CSSStyleDeclaration = null;
         var universalSelectors:Array = [];
         var hasStyleName:Boolean = false;
         var styleName:Object = object.styleName;
         if(Boolean(styleName))
         {
            if(styleName is CSSStyleDeclaration)
            {
               universalSelectors.push(CSSStyleDeclaration(styleName));
            }
            else
            {
               if(styleName is IFlexDisplayObject || styleName is IStyleClient)
               {
                  StyleProtoChain.initProtoChainForUIComponentStyleName(object);
                  return;
               }
               if(styleName is String)
               {
                  hasStyleName = true;
               }
            }
         }
         var nonInheritChain:Object = styleManager.stylesRoot;
         if(Boolean(nonInheritChain) && Boolean(nonInheritChain.effects))
         {
            object.registerEffects(nonInheritChain.effects);
         }
         var p:IStyleClient = null;
         if(object is IVisualElement)
         {
            p = IVisualElement(object).parent as IStyleClient;
         }
         else if(object is IAdvancedStyleClient)
         {
            p = IAdvancedStyleClient(object).styleParent as IStyleClient;
         }
         if(Boolean(p))
         {
            inheritChain = p.inheritingStyles;
            if(inheritChain == StyleProtoChain.STYLE_UNINITIALIZED)
            {
               inheritChain = nonInheritChain;
            }
            if(object is IModule)
            {
               styleDeclaration = styleManager.getStyleDeclaration("global");
               if(Boolean(styleDeclaration))
               {
                  inheritChain = styleDeclaration.addStyleToProtoChain(inheritChain,DisplayObject(object));
               }
            }
         }
         else if(Boolean(uicObject) && uicObject.isPopUp)
         {
            owner = uicObject._owner;
            if(Boolean(inheritPopUpStylesFromOwner) && Boolean(owner) && owner is IStyleClient)
            {
               inheritChain = IStyleClient(owner).inheritingStyles;
            }
            else
            {
               inheritChain = FlexGlobals.topLevelApplication.inheritingStyles;
            }
         }
         else
         {
            inheritChain = styleManager.stylesRoot;
         }
         var styleDeclarations:Array = null;
         if(styleManager.hasAdvancedSelectors() && advancedObject != null)
         {
            styleDeclarations = getMatchingStyleDeclarations(advancedObject,universalSelectors);
            n = styleDeclarations != null ? int(styleDeclarations.length) : 0;
            for(i = 0; i < n; i++)
            {
               styleDeclaration = styleDeclarations[i];
               inheritChain = styleDeclaration.addStyleToProtoChain(inheritChain,uicObject);
               nonInheritChain = styleDeclaration.addStyleToProtoChain(nonInheritChain,uicObject);
               if(Boolean(styleDeclaration.effects))
               {
                  advancedObject.registerEffects(styleDeclaration.effects);
               }
            }
         }
         else
         {
            if(hasStyleName)
            {
               styleNames = styleName.split(/\s+/);
               n = int(styleNames.length);
               for(i = 0; i < n; i++)
               {
                  if(Boolean(styleNames[i].length))
                  {
                     styleDeclaration = styleManager.getMergedStyleDeclaration("." + styleNames[i]);
                     if(Boolean(styleDeclaration))
                     {
                        universalSelectors.push(styleDeclaration);
                     }
                  }
               }
            }
            styleDeclarations = object.getClassStyleDeclarations();
            n = styleDeclarations != null ? int(styleDeclarations.length) : 0;
            for(i = 0; i < n; i++)
            {
               styleDeclaration = styleDeclarations[i];
               inheritChain = styleDeclaration.addStyleToProtoChain(inheritChain,uicObject);
               nonInheritChain = styleDeclaration.addStyleToProtoChain(nonInheritChain,uicObject);
               if(Boolean(styleDeclaration.effects))
               {
                  object.registerEffects(styleDeclaration.effects);
               }
            }
            n = int(universalSelectors.length);
            for(i = 0; i < n; i++)
            {
               styleDeclaration = universalSelectors[i];
               if(Boolean(styleDeclaration))
               {
                  inheritChain = styleDeclaration.addStyleToProtoChain(inheritChain,uicObject);
                  nonInheritChain = styleDeclaration.addStyleToProtoChain(nonInheritChain,uicObject);
                  if(Boolean(styleDeclaration.effects))
                  {
                     object.registerEffects(styleDeclaration.effects);
                  }
               }
            }
         }
         styleDeclaration = object.styleDeclaration;
         object.inheritingStyles = Boolean(styleDeclaration) ? styleDeclaration.addStyleToProtoChain(inheritChain,uicObject) : inheritChain;
         object.nonInheritingStyles = Boolean(styleDeclaration) ? styleDeclaration.addStyleToProtoChain(nonInheritChain,uicObject) : nonInheritChain;
      }
      
      public static function initProtoChainForUIComponentStyleName(obj:IStyleClient) : void
      {
         var typeSelector:CSSStyleDeclaration = null;
         var styleManager:IStyleManager2 = getStyleManager(obj);
         var styleName:IStyleClient = IStyleClient(obj.styleName);
         var target:DisplayObject = obj as DisplayObject;
         var nonInheritChain:Object = styleName.nonInheritingStyles;
         if(!nonInheritChain || nonInheritChain == StyleProtoChain.STYLE_UNINITIALIZED)
         {
            nonInheritChain = styleManager.stylesRoot;
            if(Boolean(nonInheritChain.effects))
            {
               obj.registerEffects(nonInheritChain.effects);
            }
         }
         var inheritChain:Object = styleName.inheritingStyles;
         if(!inheritChain || inheritChain == StyleProtoChain.STYLE_UNINITIALIZED)
         {
            inheritChain = styleManager.stylesRoot;
         }
         var typeSelectors:Array = obj.getClassStyleDeclarations();
         var n:int = int(typeSelectors.length);
         if(styleName is StyleProxy)
         {
            if(n == 0)
            {
               nonInheritChain = addProperties(nonInheritChain,styleName,false);
            }
            target = StyleProxy(styleName).source as DisplayObject;
         }
         for(var i:int = 0; i < n; i++)
         {
            typeSelector = typeSelectors[i];
            inheritChain = typeSelector.addStyleToProtoChain(inheritChain,target);
            inheritChain = addProperties(inheritChain,styleName,true);
            nonInheritChain = typeSelector.addStyleToProtoChain(nonInheritChain,target);
            nonInheritChain = addProperties(nonInheritChain,styleName,false);
            if(Boolean(typeSelector.effects))
            {
               obj.registerEffects(typeSelector.effects);
            }
         }
         obj.inheritingStyles = Boolean(obj.styleDeclaration) ? obj.styleDeclaration.addStyleToProtoChain(inheritChain,target) : inheritChain;
         obj.nonInheritingStyles = Boolean(obj.styleDeclaration) ? obj.styleDeclaration.addStyleToProtoChain(nonInheritChain,target) : nonInheritChain;
      }
      
      private static function addProperties(chain:Object, obj:IStyleClient, bInheriting:Boolean) : Object
      {
         var styleDeclarations:Array = null;
         var decl:CSSStyleDeclaration = null;
         var n:int = 0;
         var i:int = 0;
         var styleNames:Array = null;
         var c:int = 0;
         var filterMap:Object = obj is StyleProxy && !bInheriting ? StyleProxy(obj).filterMap : null;
         var curObj:IStyleClient = obj;
         while(curObj is StyleProxy)
         {
            curObj = StyleProxy(curObj).source;
         }
         var target:DisplayObject = curObj as DisplayObject;
         var advancedObject:IAdvancedStyleClient = obj as IAdvancedStyleClient;
         var styleName:Object = obj.styleName;
         var styleManager:IStyleManager2 = getStyleManager(target);
         if(advancedObject != null && styleManager.hasAdvancedSelectors())
         {
            if(styleName is CSSStyleDeclaration)
            {
               styleDeclarations = [CSSStyleDeclaration(styleName)];
            }
            styleDeclarations = getMatchingStyleDeclarations(advancedObject,styleDeclarations);
            for(i = 0; i < styleDeclarations.length; i++)
            {
               decl = styleDeclarations[i];
               if(Boolean(decl))
               {
                  chain = decl.addStyleToProtoChain(chain,target,filterMap);
                  if(Boolean(decl.effects))
                  {
                     obj.registerEffects(decl.effects);
                  }
               }
            }
            if(styleName is IStyleClient)
            {
               chain = addProperties(chain,IStyleClient(styleName),bInheriting);
            }
         }
         else
         {
            styleDeclarations = obj.getClassStyleDeclarations();
            n = int(styleDeclarations.length);
            for(i = 0; i < n; i++)
            {
               decl = styleDeclarations[i];
               chain = decl.addStyleToProtoChain(chain,target,filterMap);
               if(Boolean(decl.effects))
               {
                  obj.registerEffects(decl.effects);
               }
            }
            if(Boolean(styleName))
            {
               styleDeclarations = [];
               if(typeof styleName == "object")
               {
                  if(styleName is CSSStyleDeclaration)
                  {
                     styleDeclarations.push(CSSStyleDeclaration(styleName));
                  }
                  else
                  {
                     chain = addProperties(chain,IStyleClient(styleName),bInheriting);
                  }
               }
               else
               {
                  styleNames = styleName.split(/\s+/);
                  for(c = 0; c < styleNames.length; c++)
                  {
                     if(Boolean(styleNames[c].length))
                     {
                        styleDeclarations.push(styleManager.getMergedStyleDeclaration("." + styleNames[c]));
                     }
                  }
               }
               for(i = 0; i < styleDeclarations.length; i++)
               {
                  decl = styleDeclarations[i];
                  if(Boolean(decl))
                  {
                     chain = decl.addStyleToProtoChain(chain,target,filterMap);
                     if(Boolean(decl.effects))
                     {
                        obj.registerEffects(decl.effects);
                     }
                  }
               }
            }
         }
         if(Boolean(obj.styleDeclaration))
         {
            chain = obj.styleDeclaration.addStyleToProtoChain(chain,target,filterMap);
         }
         return chain;
      }
      
      public static function initTextField(obj:IUITextField) : void
      {
         var styleNames:Array = null;
         var c:int = 0;
         var classSelector:CSSStyleDeclaration = null;
         var styleManager:IStyleManager2 = StyleManager.getStyleManager(obj.moduleFactory);
         var styleName:Object = obj.styleName;
         var classSelectors:Array = [];
         if(Boolean(styleName))
         {
            if(typeof styleName == "object")
            {
               if(!(styleName is CSSStyleDeclaration))
               {
                  if(styleName is StyleProxy)
                  {
                     obj.inheritingStyles = IStyleClient(styleName).inheritingStyles;
                     obj.nonInheritingStyles = addProperties(styleManager.stylesRoot,IStyleClient(styleName),false);
                     return;
                  }
                  obj.inheritingStyles = IStyleClient(styleName).inheritingStyles;
                  obj.nonInheritingStyles = IStyleClient(styleName).nonInheritingStyles;
                  return;
               }
               classSelectors.push(CSSStyleDeclaration(styleName));
            }
            else
            {
               styleNames = styleName.split(/\s+/);
               for(c = 0; c < styleNames.length; c++)
               {
                  if(Boolean(styleNames[c].length))
                  {
                     classSelectors.push(styleManager.getMergedStyleDeclaration("." + styleNames[c]));
                  }
               }
            }
         }
         var inheritChain:Object = IStyleClient(obj.parent).inheritingStyles;
         var nonInheritChain:Object = styleManager.stylesRoot;
         if(!inheritChain)
         {
            inheritChain = styleManager.stylesRoot;
         }
         for(var i:int = 0; i < classSelectors.length; i++)
         {
            classSelector = classSelectors[i];
            if(Boolean(classSelector))
            {
               inheritChain = classSelector.addStyleToProtoChain(inheritChain,DisplayObject(obj));
               nonInheritChain = classSelector.addStyleToProtoChain(nonInheritChain,DisplayObject(obj));
            }
         }
         obj.inheritingStyles = inheritChain;
         obj.nonInheritingStyles = nonInheritChain;
      }
      
      public static function setStyle(object:IStyleClient, styleProp:String, newValue:*) : void
      {
         var styleManager:IStyleManager2 = getStyleManager(object);
         if(styleProp == "styleName")
         {
            object.styleName = newValue;
            return;
         }
         if(EffectManager.getEventForEffectTrigger(styleProp) != "")
         {
            EffectManager.setStyle(styleProp,object);
         }
         var isInheritingStyle:Boolean = Boolean(styleManager.isInheritingStyle(styleProp));
         var isProtoChainInitialized:Boolean = object.inheritingStyles != StyleProtoChain.STYLE_UNINITIALIZED;
         var valueChanged:Boolean = object.getStyle(styleProp) != newValue;
         if(!object.styleDeclaration)
         {
            object.styleDeclaration = new CSSStyleDeclaration(null,styleManager);
            object.styleDeclaration.setLocalStyle(styleProp,newValue);
            if(isProtoChainInitialized)
            {
               object.regenerateStyleCache(isInheritingStyle);
            }
         }
         else
         {
            object.styleDeclaration.setLocalStyle(styleProp,newValue);
         }
         if(isProtoChainInitialized && valueChanged)
         {
            object.styleChanged(styleProp);
            object.notifyStyleChangeInChildren(styleProp,isInheritingStyle);
         }
      }
      
      public static function styleChanged(object:IInvalidating, styleProp:String) : void
      {
         var parent:IInvalidating = null;
         var styleManager:IStyleManager2 = getStyleManager(object);
         if(object is IFontContextComponent && "hasFontContextChanged" in object && Boolean(object["hasFontContextChanged"]()))
         {
            object.invalidateProperties();
         }
         if(!styleProp || styleProp == "styleName" || styleProp == "layoutDirection")
         {
            object.invalidateProperties();
         }
         if(!styleProp || styleProp == "styleName" || Boolean(styleManager.isSizeInvalidatingStyle(styleProp)))
         {
            object.invalidateSize();
         }
         if(!styleProp || styleProp == "styleName" || styleProp == "themeColor")
         {
            if(object is UIComponent)
            {
               object["initThemeColor"]();
            }
         }
         object.invalidateDisplayList();
         if(object is IVisualElement)
         {
            parent = IVisualElement(object).parent as IInvalidating;
         }
         if(Boolean(parent))
         {
            if(styleProp == "styleName" || Boolean(styleManager.isParentSizeInvalidatingStyle(styleProp)))
            {
               parent.invalidateSize();
            }
            if(styleProp == "styleName" || Boolean(styleManager.isParentDisplayListInvalidatingStyle(styleProp)))
            {
               parent.invalidateDisplayList();
            }
         }
      }
      
      public static function matchesCSSType(object:IAdvancedStyleClient, cssType:String) : Boolean
      {
         var styleManager:IStyleManager2 = getStyleManager(object);
         var qualified:Boolean = styleManager.qualifiedTypeSelectors;
         var typeHierarchy:OrderedObject = getTypeHierarchy(object,qualified);
         return typeHierarchy.getObjectProperty(cssType) != null;
      }
      
      public static function getMatchingStyleDeclarations(object:IAdvancedStyleClient, styleDeclarations:Array = null) : Array
      {
         var styleManager:IStyleManager2 = getStyleManager(object);
         if(styleDeclarations == null)
         {
            styleDeclarations = [];
         }
         var universalDecls:Object = styleManager.getStyleDeclarations("*");
         styleDeclarations = matchStyleDeclarations(universalDecls,object).concat(styleDeclarations);
         if(styleDeclarations.length > 0)
         {
            styleDeclarations = object.getClassStyleDeclarations().concat(styleDeclarations);
            styleDeclarations = sortOnSpecificity(styleDeclarations);
         }
         else
         {
            styleDeclarations = object.getClassStyleDeclarations();
         }
         return styleDeclarations;
      }
      
      private static function getTypeHierarchy(object:IStyleClient, qualified:Boolean = true) : OrderedObject
      {
         var myApplicationDomain:ApplicationDomain = null;
         var factory:IFlexModuleFactory = null;
         var myRoot:DisplayObject = null;
         var type:String = null;
         var styleManager:IStyleManager2 = getStyleManager(object);
         var className:String = getQualifiedClassName(object);
         var hierarchy:OrderedObject = styleManager.typeHierarchyCache[className] as OrderedObject;
         if(hierarchy == null)
         {
            hierarchy = new OrderedObject();
            factory = ModuleManager.getAssociatedFactory(object);
            if(factory != null)
            {
               myApplicationDomain = ApplicationDomain(factory.info()["currentDomain"]);
            }
            else
            {
               myRoot = SystemManager.getSWFRoot(object);
               if(!myRoot)
               {
                  return hierarchy;
               }
               myApplicationDomain = myRoot.loaderInfo.applicationDomain;
            }
            styleManager.typeHierarchyCache[className] = hierarchy;
            while(!isStopClass(className))
            {
               try
               {
                  if(qualified)
                  {
                     type = className.replace("::",".");
                  }
                  else
                  {
                     type = NameUtil.getUnqualifiedClassName(className);
                  }
                  hierarchy.setObjectProperty(type,true);
                  className = getQualifiedSuperclassName(myApplicationDomain.getDefinition(className));
               }
               catch(e:ReferenceError)
               {
                  className = null;
               }
            }
         }
         return hierarchy;
      }
      
      private static function isStopClass(value:String) : Boolean
      {
         return value == null || value == "mx.core::UIComponent" || value == "mx.core::UITextField" || value == "mx.graphics.baseClasses::GraphicElement";
      }
      
      private static function matchStyleDeclarations(declarations:Object, object:IAdvancedStyleClient) : Array
      {
         var decl:CSSStyleDeclaration = null;
         var matchingDecls:Array = [];
         var pseudos:Array = declarations["pseudo"];
         var classes:Array = declarations["class"];
         var ids:Array = declarations["id"];
         var unconditionals:Array = declarations["unconditional"];
         for each(decl in unconditionals)
         {
            if(decl.matchesStyleClient(object))
            {
               matchingDecls.push(decl);
            }
         }
         if(object.styleName is String)
         {
            for each(decl in classes)
            {
               if(decl.matchesStyleClient(object))
               {
                  matchingDecls.push(decl);
               }
            }
         }
         if(object.hasCSSState())
         {
            for each(decl in pseudos)
            {
               if(decl.matchesStyleClient(object))
               {
                  matchingDecls.push(decl);
               }
            }
         }
         if(Boolean(object.id))
         {
            for each(decl in ids)
            {
               if(decl.matchesStyleClient(object))
               {
                  matchingDecls.push(decl);
               }
            }
         }
         if(matchingDecls.length > 1)
         {
            matchingDecls.sortOn("selectorIndex",Array.NUMERIC);
         }
         if(Boolean(declarations.parent))
         {
            matchingDecls = matchStyleDeclarations(declarations.parent,object).concat(matchingDecls);
         }
         return matchingDecls;
      }
      
      private static function sortOnSpecificity(decls:Array) : Array
      {
         var tmp:CSSStyleDeclaration = null;
         var j:int = 0;
         var len:Number = decls.length;
         if(len <= 1)
         {
            return decls;
         }
         for(var i:int = 1; i < len; i++)
         {
            for(j = i; j > 0; j--)
            {
               if(decls[j].specificity >= decls[j - 1].specificity)
               {
                  break;
               }
               tmp = decls[j];
               decls[j] = decls[j - 1];
               decls[j - 1] = tmp;
            }
         }
         return decls;
      }
      
      private static function getStyleManager(object:Object) : IStyleManager2
      {
         if(object is IFlexModule)
         {
            return StyleManager.getStyleManager(IFlexModule(object).moduleFactory);
         }
         if(object is StyleProxy)
         {
            return getStyleManagerFromStyleProxy(StyleProxy(object));
         }
         return StyleManager.getStyleManager(null);
      }
      
      private static function getStyleManagerFromStyleProxy(obj:StyleProxy) : IStyleManager2
      {
         var curObj:IStyleClient = obj;
         while(curObj is StyleProxy)
         {
            curObj = StyleProxy(curObj).source;
         }
         if(curObj is IFlexModule)
         {
            return StyleManager.getStyleManager(IFlexModule(curObj).moduleFactory);
         }
         return StyleManager.getStyleManager(null);
      }
   }
}


package mx.core
{
   import flash.display.InteractiveObject;
   import mx.managers.IFocusManager;
   import mx.managers.IFocusManagerContainer;
   
   public class ContainerGlobals
   {
      
      public static var focusedContainer:InteractiveObject;
      
      public function ContainerGlobals()
      {
         super();
      }
      
      public static function checkFocus(oldObj:InteractiveObject, newObj:InteractiveObject) : void
      {
         var fm:IFocusManager = null;
         var defButton:IButton = null;
         var objParent:InteractiveObject = newObj;
         var currObj:InteractiveObject = newObj;
         var lastUIComp:IUIComponent = null;
         if(newObj != null && oldObj == newObj)
         {
            return;
         }
         while(Boolean(currObj))
         {
            if(Boolean(currObj.parent))
            {
               objParent = currObj.parent;
            }
            else
            {
               objParent = null;
            }
            if(currObj is IUIComponent)
            {
               lastUIComp = IUIComponent(currObj);
            }
            currObj = objParent;
            if(Boolean(currObj) && Boolean(currObj is IFocusManagerContainer) && Boolean(IFocusManagerContainer(currObj).defaultButton))
            {
               break;
            }
         }
         if(ContainerGlobals.focusedContainer != currObj || ContainerGlobals.focusedContainer == null && currObj == null)
         {
            if(!currObj)
            {
               currObj = InteractiveObject(lastUIComp);
            }
            if(Boolean(currObj) && currObj is IFocusManagerContainer)
            {
               fm = IFocusManagerContainer(currObj).focusManager;
               if(!fm)
               {
                  return;
               }
               defButton = IFocusManagerContainer(currObj).defaultButton as IButton;
               if(Boolean(defButton))
               {
                  ContainerGlobals.focusedContainer = InteractiveObject(currObj);
                  fm.defaultButton = defButton as IButton;
               }
               else
               {
                  ContainerGlobals.focusedContainer = InteractiveObject(currObj);
                  fm.defaultButton = null;
               }
            }
         }
      }
   }
}


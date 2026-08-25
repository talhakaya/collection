package org.flixel
{
   import flash.events.MouseEvent;
   
   public class FlxButton extends FlxSprite
   {
      
      public static var NORMAL:uint = 0;
      
      public static var HIGHLIGHT:uint = 1;
      
      public static var PRESSED:uint = 2;
      
      protected var ImgDefaultButton:Class = FlxButton_ImgDefaultButton;
      
      protected var SndBeep:Class = FlxButton_SndBeep;
      
      public var label:FlxText;
      
      public var labelOffset:FlxPoint;
      
      public var onUp:Function;
      
      public var onDown:Function;
      
      public var onOver:Function;
      
      public var onOut:Function;
      
      public var status:uint;
      
      public var soundOver:FlxSound;
      
      public var soundOut:FlxSound;
      
      public var soundDown:FlxSound;
      
      public var soundUp:FlxSound;
      
      protected var _onToggle:Boolean;
      
      protected var _pressed:Boolean;
      
      protected var _initialized:Boolean;
      
      public function FlxButton(X:Number = 0, Y:Number = 0, Label:String = null, OnClick:Function = null)
      {
         super(X,Y);
         if(Label != null)
         {
            this.label = new FlxText(0,0,80,Label);
            this.label.setFormat(null,8,3355443,"center");
            this.labelOffset = new FlxPoint(-1,3);
         }
         loadGraphic(this.ImgDefaultButton,true,false,80,20);
         this.onUp = OnClick;
         this.onDown = null;
         this.onOut = null;
         this.onOver = null;
         this.soundOver = null;
         this.soundOut = null;
         this.soundDown = null;
         this.soundUp = null;
         this.status = NORMAL;
         this._onToggle = false;
         this._pressed = false;
         this._initialized = false;
      }
      
      override public function destroy() : void
      {
         if(FlxG.stage != null)
         {
            FlxG.stage.removeEventListener(MouseEvent.MOUSE_UP,this.onMouseUp);
         }
         if(this.label != null)
         {
            this.label.destroy();
            this.label = null;
         }
         this.onUp = null;
         this.onDown = null;
         this.onOut = null;
         this.onOver = null;
         if(this.soundOver != null)
         {
            this.soundOver.destroy();
         }
         if(this.soundOut != null)
         {
            this.soundOut.destroy();
         }
         if(this.soundDown != null)
         {
            this.soundDown.destroy();
         }
         if(this.soundUp != null)
         {
            this.soundUp.destroy();
         }
         super.destroy();
      }
      
      override public function preUpdate() : void
      {
         super.preUpdate();
         if(!this._initialized)
         {
            if(FlxG.stage != null)
            {
               FlxG.stage.addEventListener(MouseEvent.MOUSE_UP,this.onMouseUp);
               this._initialized = true;
            }
         }
      }
      
      override public function update() : void
      {
         this.updateButton();
         if(this.label == null)
         {
            return;
         }
         switch(frame)
         {
            case HIGHLIGHT:
               this.label.alpha = 1;
               break;
            case PRESSED:
               this.label.alpha = 0.5;
               ++this.label.y;
               break;
            case NORMAL:
            default:
               this.label.alpha = 0.8;
         }
      }
      
      protected function updateButton() : void
      {
         var camera:FlxCamera = null;
         var i:uint = 0;
         var l:uint = 0;
         var offAll:Boolean = false;
         if(FlxG.mouse.visible)
         {
            if(cameras == null)
            {
               cameras = FlxG.cameras;
            }
            i = 0;
            l = cameras.length;
            offAll = true;
            while(i < l)
            {
               camera = cameras[i++] as FlxCamera;
               FlxG.mouse.getWorldPosition(camera,_point);
               if(overlapsPoint(_point,true,camera))
               {
                  offAll = false;
                  if(FlxG.mouse.justPressed())
                  {
                     this.status = PRESSED;
                     if(this.onDown != null)
                     {
                        this.onDown();
                     }
                     if(this.soundDown != null)
                     {
                        this.soundDown.play(true);
                     }
                  }
                  if(this.status == NORMAL)
                  {
                     this.status = HIGHLIGHT;
                     if(this.onOver != null)
                     {
                        this.onOver();
                     }
                     if(this.soundOver != null)
                     {
                        this.soundOver.play(true);
                     }
                  }
               }
            }
            if(offAll)
            {
               if(this.status != NORMAL)
               {
                  if(this.onOut != null)
                  {
                     this.onOut();
                  }
                  if(this.soundOut != null)
                  {
                     this.soundOut.play(true);
                  }
               }
               this.status = NORMAL;
            }
         }
         if(this.label != null)
         {
            this.label.x = x;
            this.label.y = y;
         }
         if(this.labelOffset != null)
         {
            this.label.x += this.labelOffset.x;
            this.label.y += this.labelOffset.y;
         }
         if(this.status == HIGHLIGHT && this._onToggle)
         {
            frame = NORMAL;
         }
         else
         {
            frame = this.status;
         }
      }
      
      override public function draw() : void
      {
         super.draw();
         if(this.label != null)
         {
            this.label.scrollFactor = scrollFactor;
            this.label.cameras = cameras;
            this.label.draw();
         }
      }
      
      override protected function resetHelpers() : void
      {
         super.resetHelpers();
         if(this.label != null)
         {
            this.label.width = width;
         }
      }
      
      public function setSounds(SoundOver:Class = null, SoundOverVolume:Number = 1, SoundOut:Class = null, SoundOutVolume:Number = 1, SoundDown:Class = null, SoundDownVolume:Number = 1, SoundUp:Class = null, SoundUpVolume:Number = 1) : void
      {
         if(SoundOver != null)
         {
            this.soundOver = FlxG.loadSound(SoundOver,SoundOverVolume);
         }
         if(SoundOut != null)
         {
            this.soundOut = FlxG.loadSound(SoundOut,SoundOutVolume);
         }
         if(SoundDown != null)
         {
            this.soundDown = FlxG.loadSound(SoundDown,SoundDownVolume);
         }
         if(SoundUp != null)
         {
            this.soundUp = FlxG.loadSound(SoundUp,SoundUpVolume);
         }
      }
      
      public function get on() : Boolean
      {
         return this._onToggle;
      }
      
      public function set on(On:Boolean) : void
      {
         this._onToggle = On;
      }
      
      protected function onMouseUp(event:MouseEvent) : void
      {
         if(!exists || !visible || !active || this.status != PRESSED)
         {
            return;
         }
         if(this.onUp != null)
         {
            this.onUp();
         }
         if(this.soundUp != null)
         {
            this.soundUp.play(true);
         }
      }
   }
}


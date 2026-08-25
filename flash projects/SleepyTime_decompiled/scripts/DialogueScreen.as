package
{
   import flash.Boot;
   import flash.display.BitmapData;
   import flash.display.Sprite;
   import motion.Actuate;
   import openfl.Assets;
   
   public class DialogueScreen extends Sprite
   {
      
      public static var PressConst:int = 1000;
      
      public var textRight3:TextTalha;
      
      public var textRight2:TextTalha;
      
      public var textRight1:TextTalha;
      
      public var textLeft3:TextTalha;
      
      public var textLeft2:TextTalha;
      
      public var textLeft1:TextTalha;
      
      public var pressCounter:int;
      
      public var linesThird:Array;
      
      public var linesSecond:Array;
      
      public var linesFirst:Array;
      
      public var isCharacterRight:Boolean;
      
      public var id:int;
      
      public var dialoguePointer:int;
      
      public var destroyMePleaseMessageTaken:Boolean;
      
      public var destroyMePlease:Boolean;
      
      public var characters:Array;
      
      public var characterRight:Citmap;
      
      public var characterLeft:Citmap;
      
      public var characterContainerRight:Sprite;
      
      public var characterContainerLeft:Sprite;
      
      public var backgroundGriddyBackground:GriddyBackground;
      
      public var background:Sprite;
      
      public function DialogueScreen()
      {
         if(Boot.skip_constructor)
         {
            return;
         }
         pressCounter = 0;
         dialoguePointer = 0;
         isCharacterRight = true;
         destroyMePleaseMessageTaken = false;
         destroyMePlease = false;
         super();
         characters = [];
         linesFirst = [];
         linesSecond = [];
         linesThird = [];
         background = new Sprite();
         background.x = Main.stageWidth / 2;
         background.y = Main.stageHeight / 2;
         addChild(background);
         backgroundGriddyBackground = new GriddyBackground();
         backgroundGriddyBackground.x = -Main.stageWidth / 2;
         backgroundGriddyBackground.y = -Main.stageHeight / 2;
         background.addChild(backgroundGriddyBackground);
         characterContainerRight = new Sprite();
         characterContainerLeft = new Sprite();
         characterContainerRight.x = Main.stageWidth * 4 / 2;
         characterContainerLeft.x = -Main.stageWidth * 4 / 2;
         addChild(characterContainerRight);
         addChild(characterContainerLeft);
         characterRight = new Citmap(Assets.getBitmapData("img/bereket.png"),200,250);
         characterLeft = new Citmap(Assets.getBitmapData("img/bereket.png"),200,250);
         characterRight.x = Main.stageWidth / 2;
         characterRight.y = Main.stageHeight * 2.5 / 8;
         characterLeft.x = Main.stageWidth / 2;
         characterLeft.y = Main.stageHeight * 2.5 / 8;
         characterRight.scaleX = characterRight.scaleY = 0.5;
         characterLeft.scaleX = characterLeft.scaleY = 0.5;
         characterContainerRight.addChild(characterRight);
         characterContainerLeft.addChild(characterLeft);
         textLeft1 = new TextTalha("textLeft1");
         textLeft1.x = Main.stageWidth / 2;
         textLeft1.y = Main.stageHeight * 5.5 / 8;
         characterContainerLeft.addChild(textLeft1);
         textLeft2 = new TextTalha("textLeft2");
         textLeft2.x = Main.stageWidth / 2;
         textLeft2.y = Main.stageHeight * 6.5 / 8;
         characterContainerLeft.addChild(textLeft2);
         textLeft3 = new TextTalha("textLeft3");
         textLeft3.x = Main.stageWidth / 2;
         textLeft3.y = Main.stageHeight * 7.5 / 8;
         characterContainerLeft.addChild(textLeft3);
         textRight1 = new TextTalha("textRight1");
         textRight1.x = Main.stageWidth / 2;
         textRight1.y = Main.stageHeight * 5.5 / 8;
         characterContainerRight.addChild(textRight1);
         textRight2 = new TextTalha("textRight2");
         textRight2.x = Main.stageWidth / 2;
         textRight2.y = Main.stageHeight * 6.5 / 8;
         characterContainerRight.addChild(textRight2);
         textRight3 = new TextTalha("textRight3");
         textRight3.x = Main.stageWidth / 2;
         textRight3.y = Main.stageHeight * 7.5 / 8;
         characterContainerRight.addChild(textRight3);
         id = GameManager.id;
         var _loc2_:int = id;
         var _loc3_:int = _loc2_;
         if(_loc3_ == -2)
         {
            characters.push("sleepy time");
            linesFirst.push("");
            linesSecond.push("fullscreen might not work on flash");
            linesThird.push("");
         }
         else if(_loc3_ == -1)
         {
            characters.push("tutorial");
            linesFirst.push("press \'space\' or any button to play");
            linesSecond.push("single press when on a white thing");
            linesThird.push("long press if the white thing goes on");
         }
         else if(_loc3_ == 0)
         {
            characters.push("kayabros");
            linesFirst.push("Awkwardly Presents");
            linesSecond.push("");
            linesThird.push("Press any button or click");
            characters.push("sleepy time");
            linesFirst.push("");
            linesSecond.push("sleepy time");
            linesThird.push("");
            characters.push("headphones");
            linesFirst.push("Headphones are recommended");
            linesSecond.push("Just hear the thing, would ya?");
            linesThird.push("");
            characters.push("sleepy time");
            linesFirst.push("");
            linesSecond.push("press \'f\' for fullscreen on/off");
            linesThird.push("");
            characters.push("bereket");
            linesFirst.push("nothing will ever be the same");
            linesSecond.push("after you play this game");
            linesThird.push("are you sure you want to play it?");
            characters.push("sad");
            linesFirst.push("I am a super hero");
            linesSecond.push("at day I\'m a regular man");
            linesThird.push("at night I fight with depression");
            characters.push("bereket");
            linesFirst.push("listen to me");
            linesSecond.push("you can press any button");
            linesThird.push("for example \'space\'");
            characters.push("sad");
            linesFirst.push("");
            linesSecond.push("...");
            linesThird.push("");
            characters.push("bereket");
            linesFirst.push("this is a rhythm game like guitar hero");
            linesSecond.push("press when you are on a white thing");
            linesThird.push("keep pressing until the white thing ends");
            characters.push("sad");
            linesFirst.push("this is stupid");
            linesSecond.push("you are stupid");
            linesThird.push("");
            characters.push("tutorial");
            linesFirst.push("press \'space\' or any button to play");
            linesSecond.push("single press when on a white thing");
            linesThird.push("long press if the white thing goes on");
         }
         else if(_loc3_ == 1)
         {
            characters.push("blind");
            linesFirst.push("confusion is your friend");
            linesSecond.push("you should not be afraid");
            linesThird.push("be human");
            characters.push("bereket");
            linesFirst.push("when you can\'t sleep");
            linesSecond.push("you don\'t count sheep, do you?");
            linesThird.push("what do you do?");
            characters.push("bald");
            linesFirst.push("what\'s the meaning of all this?");
            linesSecond.push("is this supposed to be fun?");
            linesThird.push("");
            characters.push("bereket");
            linesFirst.push("this is a game");
            linesSecond.push("you will have to play it to sleep");
            linesThird.push("you wanna sleep, don\'t ya?");
            characters.push("sad");
            linesFirst.push("I want to sleep");
            linesSecond.push("sleep forever");
            linesThird.push("until the end of times");
            characters.push("bereket");
            linesFirst.push("then play it");
            linesSecond.push("you are feeling bad");
            linesThird.push("you must be a spoiled child");
         }
         else if(_loc3_ == 2)
         {
            characters.push("mother");
            linesFirst.push("where\'s my baby");
            linesSecond.push("where\'s my love");
            linesThird.push("why is this happening");
            characters.push("bereket");
            linesFirst.push("there is no reason");
            linesSecond.push("there is no love");
            linesThird.push("there is nothing");
            characters.push("mother");
            linesFirst.push("where are my sons");
            linesSecond.push("are they okay");
            linesThird.push("help me");
            characters.push("bereket");
            linesFirst.push("there is no pain");
            linesSecond.push("nothing to worry");
            linesThird.push("nothing to suffer");
            characters.push("mother");
            linesFirst.push("");
            linesSecond.push("...");
            linesThird.push("");
            characters.push("bereket");
            linesFirst.push("trust me, just sleep");
            linesSecond.push("it\'s all gone");
            linesThird.push("it was always gone");
         }
         else if(_loc3_ == 3)
         {
            characters.push("freaky");
            linesFirst.push("what are you doing?");
            linesSecond.push("who are you fooling again?");
            linesThird.push("this ends now");
            characters.push("bereket");
            linesFirst.push("it\'s not over yet");
            linesSecond.push("we have much to see");
            linesThird.push("they have to see");
            characters.push("buda");
            linesFirst.push("it\'s okay");
            linesSecond.push("they know they are depressed");
            linesThird.push("so they will get better");
            characters.push("bereket");
            linesFirst.push("don\'t hold your breathe");
            linesSecond.push("that won\'t happen");
            linesThird.push("");
            characters.push("buda");
            linesFirst.push("");
            linesSecond.push("how do you know that?");
            linesThird.push("");
            characters.push("bereket");
            linesFirst.push("these guys are losers");
            linesSecond.push("big fucking losers");
            linesThird.push("they are useless pricks");
            characters.push("buda");
            linesFirst.push("I think there must be");
            linesSecond.push("another way for these people");
            linesThird.push("");
            characters.push("freaky");
            linesFirst.push("when the time comes");
            linesSecond.push("they\'ll be free");
            linesThird.push("until then, they are yours");
            characters.push("bereket");
            linesFirst.push("");
            linesSecond.push("thank you");
            linesThird.push("");
         }
         else if(_loc3_ == 4)
         {
            characters.push("lover");
            linesFirst.push("wow, what a mess");
            linesSecond.push("this won\'t get better");
            linesThird.push("I guess I left just in time");
            characters.push("mother");
            linesFirst.push("my son, is he ok?");
            linesSecond.push("what did you do to him?");
            linesThird.push("fix it!");
            characters.push("lover");
            linesFirst.push("I didn\'t do anything");
            linesSecond.push("I was just passing by");
            linesThird.push("he hurt me more than I hurt him");
            characters.push("bereket");
            linesFirst.push("I think he looks okay");
            linesSecond.push("look at what he is doing");
            linesThird.push("he is having fun. fun!");
         }
         else if(_loc3_ == 5)
         {
            characters.push("politician");
            linesFirst.push("ladies and gentleman");
            linesSecond.push("we can cure depression");
            linesThird.push("once and for all");
            characters.push("tache");
            linesFirst.push("you have to fix it");
            linesSecond.push("these people");
            linesThird.push("they are suffering");
            characters.push("politician");
            linesFirst.push("so as I said");
            linesSecond.push("and it shall be done");
            linesThird.push("");
            characters.push("tache");
            linesFirst.push("do you even believe yourself?");
            linesSecond.push("you\'re horrible");
            linesThird.push("you can\'t even fix yourself");
            characters.push("politician");
            linesFirst.push("I don\'t worry");
            linesSecond.push("I don\'t suffer");
            linesThird.push("it shall be done");
            characters.push("ceasar");
            linesFirst.push("you have nothing to do with this");
            linesSecond.push("leave it alone");
            linesThird.push("let us, real humans, deal with it");
         }
         else if(_loc3_ == 6)
         {
            characters.push("bereket");
            linesFirst.push("there\'s a pattern to everything");
            linesSecond.push("everything humans do");
            linesThird.push("");
            characters.push("ceasar");
            linesFirst.push("everything they worry about");
            linesSecond.push("everything they desire");
            linesThird.push("everything they suffer for");
            characters.push("sad");
            linesFirst.push("humans are after the very same thing");
            linesSecond.push("as if they are one");
            linesThird.push("as if there is only one person");
            characters.push("ceasar");
            linesFirst.push("and there\'s a pattern");
            linesSecond.push("to everything");
            linesThird.push("to life and death");
            characters.push("mother");
            linesFirst.push("it\'s ok");
            linesSecond.push("I have died");
            linesThird.push("they will go on");
            characters.push("sad");
            linesFirst.push("we are all super heroes");
            linesSecond.push("fighting with depression");
            linesThird.push("fighting with ourselves");
            characters.push("bereket");
            linesFirst.push("");
            linesSecond.push("you are all fucked up");
            linesThird.push("");
            characters.push("sad");
            linesFirst.push("we are fucked up");
            linesSecond.push("and we are proud");
            linesThird.push("");
            characters.push("bereket");
            linesFirst.push("");
            linesSecond.push("...");
            linesThird.push("");
            characters.push("sad");
            linesFirst.push("");
            linesSecond.push("...");
            linesThird.push("");
            characters.push("bereket");
            linesFirst.push("");
            linesSecond.push("...");
            linesThird.push("");
            characters.push("sad");
            linesFirst.push("");
            linesSecond.push("...");
            linesThird.push("");
            characters.push("bereket");
            linesFirst.push("");
            linesSecond.push("fuck you all");
            linesThird.push("");
         }
         updateCharacter();
      }
      
      public function updateCharacter() : void
      {
         var _loc2_:Number = NaN;
         var _loc3_:Number = NaN;
         var _loc4_:* = null as String;
         var _loc5_:* = null as String;
         var _loc6_:Number = NaN;
         if(dialoguePointer < int(characters.length))
         {
            _loc2_ = 0;
            _loc3_ = 0;
            if(isCharacterRight)
            {
               _loc2_ = characterRight.x;
               _loc3_ = characterRight.y;
               characterContainerRight.removeChild(characterRight);
            }
            else
            {
               _loc2_ = characterLeft.x;
               _loc3_ = characterLeft.y;
               characterContainerLeft.removeChild(characterLeft);
            }
            _loc4_ = characters[dialoguePointer];
            _loc5_ = _loc4_;
            if(_loc5_ == "bereket")
            {
               if(isCharacterRight)
               {
                  characterRight = new Citmap(Assets.getBitmapData("img/bereket.png"),200,250);
                  characterRight.scaleX = characterRight.scaleY = 0.5;
               }
               else
               {
                  characterLeft = new Citmap(Assets.getBitmapData("img/bereket.png"),200,250);
                  characterLeft.scaleX = characterLeft.scaleY = 0.5;
               }
            }
            else if(_loc5_ == "kayabros")
            {
               if(isCharacterRight)
               {
                  characterRight = new Citmap(Assets.getBitmapData("img/kayabros.png"),160,173);
                  characterRight.scaleX = characterRight.scaleY = 0.8;
               }
               else
               {
                  characterLeft = new Citmap(Assets.getBitmapData("img/kayabros.png"),160,173);
                  characterLeft.scaleX = characterLeft.scaleY = 0.8;
               }
            }
            else if(_loc5_ == "headphones")
            {
               if(isCharacterRight)
               {
                  characterRight = new Citmap(Assets.getBitmapData("img/headphones.png"),99,90);
                  characterRight.scaleX = characterRight.scaleY = 1;
               }
               else
               {
                  characterLeft = new Citmap(Assets.getBitmapData("img/headphones.png"),99,90);
                  characterLeft.scaleX = characterLeft.scaleY = 1;
               }
            }
            else if(_loc5_ == "tache")
            {
               if(isCharacterRight)
               {
                  characterRight = new Citmap(Assets.getBitmapData("img/tache.png"),128,221);
                  characterRight.scaleX = characterRight.scaleY = 0.55;
               }
               else
               {
                  characterLeft = new Citmap(Assets.getBitmapData("img/tache.png"),128,221);
                  characterLeft.scaleX = characterLeft.scaleY = 0.55;
               }
            }
            else if(_loc5_ == "politician")
            {
               if(isCharacterRight)
               {
                  characterRight = new Citmap(Assets.getBitmapData("img/politician.png"),155,250);
                  characterRight.scaleX = characterRight.scaleY = 0.5;
               }
               else
               {
                  characterLeft = new Citmap(Assets.getBitmapData("img/politician.png"),155,250);
                  characterLeft.scaleX = characterLeft.scaleY = 0.5;
               }
            }
            else if(_loc5_ == "mother")
            {
               if(isCharacterRight)
               {
                  characterRight = new Citmap(Assets.getBitmapData("img/mother.png"),173,250);
                  characterRight.scaleX = characterRight.scaleY = 0.5;
               }
               else
               {
                  characterLeft = new Citmap(Assets.getBitmapData("img/mother.png"),173,250);
                  characterLeft.scaleX = characterLeft.scaleY = 0.5;
               }
            }
            else if(_loc5_ == "lover")
            {
               if(isCharacterRight)
               {
                  characterRight = new Citmap(Assets.getBitmapData("img/lover.png"),194,250);
                  characterRight.scaleX = characterRight.scaleY = 0.5;
               }
               else
               {
                  characterLeft = new Citmap(Assets.getBitmapData("img/lover.png"),194,250);
                  characterLeft.scaleX = characterLeft.scaleY = 0.5;
               }
            }
            else if(_loc5_ == "freaky")
            {
               if(isCharacterRight)
               {
                  characterRight = new Citmap(Assets.getBitmapData("img/freaky.png"),140,172);
                  characterRight.scaleX = characterRight.scaleY = 0.7;
               }
               else
               {
                  characterLeft = new Citmap(Assets.getBitmapData("img/freaky.png"),140,172);
                  characterLeft.scaleX = characterLeft.scaleY = 0.7;
               }
            }
            else if(_loc5_ == "ceasar")
            {
               if(isCharacterRight)
               {
                  characterRight = new Citmap(Assets.getBitmapData("img/ceasar.png"),139,176);
                  characterRight.scaleX = characterRight.scaleY = 0.7;
               }
               else
               {
                  characterLeft = new Citmap(Assets.getBitmapData("img/ceasar.png"),139,176);
                  characterLeft.scaleX = characterLeft.scaleY = 0.7;
               }
            }
            else if(_loc5_ == "buda")
            {
               if(isCharacterRight)
               {
                  characterRight = new Citmap(Assets.getBitmapData("img/buda.png"),221,181);
                  characterRight.scaleX = characterRight.scaleY = 0.65;
               }
               else
               {
                  characterLeft = new Citmap(Assets.getBitmapData("img/buda.png"),221,181);
                  characterLeft.scaleX = characterLeft.scaleY = 0.65;
               }
            }
            else if(_loc5_ == "blind")
            {
               if(isCharacterRight)
               {
                  characterRight = new Citmap(Assets.getBitmapData("img/blind.png"),196,250);
                  characterRight.scaleX = characterRight.scaleY = 0.5;
               }
               else
               {
                  characterLeft = new Citmap(Assets.getBitmapData("img/blind.png"),196,250);
                  characterLeft.scaleX = characterLeft.scaleY = 0.5;
               }
            }
            else if(_loc5_ == "bald")
            {
               if(isCharacterRight)
               {
                  characterRight = new Citmap(Assets.getBitmapData("img/bald.png"),177,250);
                  characterRight.scaleX = characterRight.scaleY = 0.5;
               }
               else
               {
                  characterLeft = new Citmap(Assets.getBitmapData("img/bald.png"),177,250);
                  characterLeft.scaleX = characterLeft.scaleY = 0.5;
               }
            }
            else if(_loc5_ == "sad")
            {
               if(isCharacterRight)
               {
                  characterRight = new Citmap(Assets.getBitmapData("img/sad.png"),132,213);
                  characterRight.scaleX = characterRight.scaleY = 0.6;
               }
               else
               {
                  characterLeft = new Citmap(Assets.getBitmapData("img/sad.png"),132,213);
                  characterLeft.scaleX = characterLeft.scaleY = 0.6;
               }
            }
            else if(_loc5_ == "sleepy time")
            {
               if(isCharacterRight)
               {
                  characterRight = new Citmap(Assets.getBitmapData("img/sleepy time.png"),50,38);
                  characterRight.scaleX = characterRight.scaleY = 3;
               }
               else
               {
                  characterLeft = new Citmap(Assets.getBitmapData("img/sleepy time.png"),50,38);
                  characterLeft.scaleX = characterLeft.scaleY = 3;
               }
            }
            else if(_loc5_ == "tutorial")
            {
               if(isCharacterRight)
               {
                  characterRight = new Tutorial();
                  characterRight.scaleX = characterRight.scaleY = 1;
               }
               else
               {
                  characterLeft = new Tutorial();
                  characterLeft.scaleX = characterLeft.scaleY = 1;
               }
            }
            if(isCharacterRight)
            {
               characterRight.x = _loc2_;
               characterRight.y = _loc3_;
               characterContainerRight.addChild(characterRight);
               textRight1.text = linesFirst[dialoguePointer];
               textRight2.text = linesSecond[dialoguePointer];
               textRight3.text = linesThird[dialoguePointer];
               Actuate.tween(characterContainerRight,1,{"x":0});
               Actuate.tween(characterContainerLeft,2,{"x":-Main.stageWidth});
            }
            else
            {
               characterLeft.x = _loc2_;
               characterLeft.y = _loc3_;
               characterContainerLeft.addChild(characterLeft);
               textLeft1.text = linesFirst[dialoguePointer];
               textLeft2.text = linesSecond[dialoguePointer];
               textLeft3.text = linesThird[dialoguePointer];
               Actuate.tween(characterContainerLeft,1,{"x":0});
               Actuate.tween(characterContainerRight,2,{"x":Main.stageWidth});
            }
            isCharacterRight = !isCharacterRight;
            ++dialoguePointer;
         }
         else
         {
            destroyMePlease = true;
         }
      }
      
      public function update() : void
      {
         inputHandler();
         backgroundGriddyBackground.update();
         textLeft1.update();
         textLeft2.update();
         textLeft3.update();
         textRight1.update();
         textRight2.update();
         textRight3.update();
         characterLeft.update();
         characterRight.update();
      }
      
      public function inputHandler() : void
      {
         if(pressCounter < 1000)
         {
            pressCounter += GameManager.dt;
         }
         else if(GameManager.getKeyDown())
         {
            pressCounter = 0;
            updateCharacter();
         }
      }
   }
}


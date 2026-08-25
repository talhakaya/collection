package
{
   import org.flixel.FlxText;
   
   public class Narrator extends FlxText
   {
      
      public var workByTouch:Boolean;
      
      private var written:Boolean = false;
      
      public var erase:Boolean;
      
      public var line:uint;
      
      private var oldline:uint;
      
      public function Narrator(_X:Number, _Y:Number, _width:uint, _line:uint, _workByTouch:Boolean)
      {
         this.workByTouch = _workByTouch;
         this.erase;
         this.line = _line;
         this.oldline = _line;
         alpha = 0;
         super(_X,_Y,_width,"",true);
         if(!this.workByTouch)
         {
            this.kill();
         }
      }
      
      override public function kill() : void
      {
         if(!this.written)
         {
            this.written = true;
            alpha = 0;
            if(this.line == 1)
            {
               text = "Hans liked a cheerleader. Because she was pretty.";
               size = 8;
            }
            else if(this.line == 2)
            {
               text = "The cheerleader didn’t like Hans. Because he was ugly.";
               size = 8;
            }
            else if(this.line == 3)
            {
               text = "Hans wished to be a better-looking person.";
               size = 8;
            }
            else if(this.line == 4)
            {
               text = "But that wasn\'t gonna happen.";
               size = 8;
            }
            else if(this.line == 5)
            {
               text = "He cried and cried over the girl";
               size = 8;
            }
            else if(this.line == 6)
            {
               text = "- the idealized female he created in his mind.";
               size = 8;
            }
            else if(this.line == 7)
            {
               text = "The months of crying led to an escape plan. He thought it might just work.";
               size = 8;
            }
            else if(this.line == 8)
            {
               text = "What if things were simpler? What if everyone was even?";
               size = 8;
            }
            else if(this.line == 9)
            {
               text = "What if everyone looked just the same?";
               size = 8;
            }
            else if(this.line == 10)
            {
               text = "Hans spent his nights building a magical machine that would make his wish come true.";
               size = 8;
            }
            else if(this.line == 11)
            {
               text = "He pulled the lever.";
               size = 8;
            }
            else if(this.line == 12)
            {
               text = "As Hans became smaller, he thought he looked more like a normal person!";
               size = 8;
            }
            else if(this.line == 1213)
            {
               text = "Hans was proud with his machine. If everyone got smaller, then everyone would look just the same. Thank god, we live in a world of pixels!";
               size = 8;
            }
            else if(this.line == 13)
            {
               text = "He saw her, so calm and pretty. His heart beat faster.";
               size = 8;
            }
            else if(this.line == 14)
            {
               text = "That didn’t go well at all.";
               size = 8;
            }
            else if(this.line == 15)
            {
               text = "But he could try once more.";
               size = 8;
            }
            else if(this.line == 16)
            {
               text = "Hans thought he looked more acceptable now.";
               size = 8;
            }
            else if(this.line == 17)
            {
               text = "Hans may be the last ugly boy on earth. What a rewarding journey, he thought.";
               size = 8;
            }
            else if(this.line == 18)
            {
               text = "And Hans failed one more time.";
               size = 8;
            }
            else if(this.line == 19)
            {
               text = "Well, what can you do... Try again?";
               size = 8;
            }
            else if(this.line == 20)
            {
               text = "This is getting boring and hard.            Hans started missing his old life.";
               size = 8;
            }
            else if(this.line == 2021)
            {
               text = "But Hans smiled, climbing was bearable if love awaits on the top of the mountain.";
               size = 8;
            }
            else if(this.line == 21)
            {
               text = "And again, he thought maybe, he could finally have her.";
               size = 8;
            }
            else if(this.line == 22)
            {
               text = "Hans started hating this journey.";
               size = 8;
            }
            else if(this.line == 23)
            {
               text = "Hans is the first member of a new breed. A utopia, slowly coming together.";
               size = 8;
            }
            else if(this.line == 2324)
            {
               text = "Two pixels of loneliness; impatient, and depressed.";
               size = 8;
            }
            else if(this.line == 24)
            {
               text = "Just a few more steps... Here she comes.";
               size = 8;
            }
            else if(this.line == 25)
            {
               text = "God, look at that pixel!";
               size = 8;
            }
            else if(this.line == 26)
            {
               text = "Heeeey, you made it! You have the girlfriend of your dreams!";
               size = 8;
            }
            else if(this.line == 27)
            {
               text = "Wow, you must be really happy right now! Congrats!";
               size = 8;
            }
            else if(this.line == 28)
            {
               text = "You look perfect as a couple.";
               size = 8;
            }
            else if(this.line == 29)
            {
               text = "It’s every guy’s dream to have a girlfriend like her.";
               size = 8;
            }
            else if(this.line == 30)
            {
               text = "Not that every guy deserves one, though.";
               size = 8;
            }
            else if(this.line == 31)
            {
               text = "Who deserves who, anyway?";
               size = 8;
            }
            else if(this.line == 32)
            {
               text = "Well, aren’t you guys adorable!";
               size = 8;
            }
            else if(this.line == 3232)
            {
               text = "Hans can\'t suppress the feeling that she\'s a burden on him more than a lover.";
               size = 8;
            }
            else if(this.line == 3233)
            {
               text = "";
               size = 8;
            }
            else if(this.line == 3234)
            {
               text = "But ain\'t she so sweet, jumping like a bunny.";
               size = 8;
            }
            else if(this.line == 3235)
            {
               text = "And now Hans understands that he can go on loving her, forever.";
               size = 8;
            }
            else if(this.line == 3236)
            {
               text = "Even if that means she will limit his life in every aspect.";
               size = 8;
            }
            else if(this.line == 33)
            {
               text = "Hmm, there’s something weird...";
               size = 8;
            }
            else if(this.line == 34)
            {
               text = "Why does she seem displeased?";
               size = 8;
            }
            else if(this.line == 35)
            {
               text = "Is she afraid of something?";
               size = 8;
            }
            else if(this.line == 36)
            {
               text = "Does she miss her old life?";
               size = 8;
            }
            else if(this.line == 37)
            {
               text = "She was so popular and pretty back in the old life.";
               size = 8;
            }
            else if(this.line == 38)
            {
               text = "Now she’s just two pixels, like everyone else.";
               size = 8;
            }
            else if(this.line == 39)
            {
               text = "She just wants to be different...";
               size = 8;
            }
            else if(this.line == 40)
            {
               text = "...Just like you want to be equal.";
               size = 8;
            }
            else if(this.line == 41)
            {
               text = "She feels trapped.";
               size = 8;
            }
            else if(this.line == 42)
            {
               text = "She wants to run away from all these.";
               size = 8;
            }
            else if(this.line == 43)
            {
               text = "Don’t let her go!";
               size = 8;
            }
            else if(this.line == 44)
            {
               text = "Where is she?";
               size = 8;
            }
            else if(this.line == 45)
            {
               text = "Is that your machine?";
               size = 8;
            }
            else if(this.line == 46)
            {
               text = "Oh there she is!";
               size = 8;
            }
            else if(this.line == 47)
            {
               text = "This can\'t be good.";
               size = 8;
            }
            else if(this.line == 48)
            {
               text = "";
               size = 8;
            }
            else if(this.line == 49)
            {
               text = "I\'ll just pretend all the weird stuff never happened! It would be great if we knew where she was though!";
               size = 8;
            }
            else if(this.line == 50)
            {
               text = "Hans, I feel something dangerous. Just keep walking no matter what.";
               size = 8;
            }
            else if(this.line == 5051)
            {
               text = "Ahh, that was hard! Maybe now we can breathe easily.";
               size = 8;
            }
            else if(this.line == 51)
            {
               text = "Hans, be careful.";
               size = 8;
            }
            else if(this.line == 52)
            {
               text = "It\'s the cheerleader!";
               size = 8;
            }
            else if(this.line == 53)
            {
               text = "Stop her, you worked hard for all these!";
               size = 8;
            }
            else if(this.line == 54)
            {
               text = "Damn it! We lost the girl again.                             And another box puzzle, how lovely!";
               size = 8;
            }
            else if(this.line == 55)
            {
               text = "I think you shouldn\'t lose the box on this one.";
               size = 8;
            }
            else if(this.line == 56)
            {
               text = "Why are we here, anyway? Is she really worth all this trouble? ";
               size = 8;
            }
            else if(this.line == 57)
            {
               text = "There she is again...";
               size = 8;
            }
            else if(this.line == 58)
            {
               text = "Well, we saw that coming anyway. Now, I think we\'ve acted naive long enough. We have to figure this out.";
               size = 8;
            }
            else if(this.line == 5859)
            {
               text = "Hans, maybe the question is, why do you need the cheerleader so much?";
               size = 8;
            }
            else if(this.line == 59)
            {
               text = "Maybe the reason is you, Hans. It\'s not about the cheerleader.";
               size = 8;
            }
            else if(this.line == 5960)
            {
               text = "She\'s just a confused child, just like you.";
               size = 8;
            }
            else if(this.line == 5961)
            {
               text = "But this is your story. Maybe we\'re here, talking, because you feel ugly, and you want to be handsome, and cool, and charismatic...";
               size = 8;
            }
            else if(this.line == 5962)
            {
               text = "You wanted to be special, but look at yourself now.";
               size = 8;
            }
            else if(this.line == 60)
            {
               text = "It\'s time to face your mistakes, Hans.";
               size = 8;
            }
            else if(this.line == 61)
            {
               text = "It\'s the hard times, that teach you. It\'s the hard times, that get you closer to the person you want to be.";
               size = 8;
            }
            else if(this.line == 62)
            {
               text = "Hans, you haven\'t been in a battle with yourself before. Just be strong. And believe me, this too shall pass.";
               size = 8;
            }
            else if(this.line == 63)
            {
               text = "KNOW YOUR INSTINCTS";
               size = 8;
            }
            else if(this.line == 64)
            {
               text = "TAKE CONTROL";
               size = 8;
            }
            else if(this.line == 65)
            {
               text = "YOU\'RE WHAT YOU ARE";
               size = 8;
            }
            else if(this.line == 66)
            {
               text = "FIND YOUR WAY";
               size = 8;
            }
            else if(this.line == 67)
            {
               text = "DON\'T EXPECT FROM OTHERS";
               size = 8;
            }
            else if(this.line == 68)
            {
               text = "YOU\'RE ONE AND ONLY";
               size = 8;
            }
            else if(this.line == 69)
            {
               text = "EXPLORE YOURSELF";
               size = 8;
            }
            else if(this.line == 70)
            {
               text = "You did it, Hans. It\'s over.";
               size = 8;
            }
            else if(this.line == 71)
            {
               text = "Did you miss the old Hans?";
               size = 8;
            }
            else if(this.line == 72)
            {
               text = "I don\'t think this castle can hold your dreams anymore.";
               size = 8;
            }
            else if(this.line == 73)
            {
               text = "Have you ever wanted to fly, Hans?";
               size = 8;
            }
            else if(this.line == 74)
            {
               text = "Now is your chance.";
               size = 8;
            }
            else if(this.line == 75)
            {
               text = "Thanks for playing.";
               size = 8;
            }
            else if(this.line == 76)
            {
               text = "A game by Talha Kaya";
               size = 8;
            }
            else if(this.line == 77)
            {
               text = "Game Design,";
               size = 8;
            }
            else if(this.line == 78)
            {
               text = "Graphics, Animations,";
               size = 8;
            }
            else if(this.line == 79)
            {
               text = "Programming,";
               size = 8;
            }
            else if(this.line == 80)
            {
               text = "Music, Sfx: Talha Kaya";
               size = 8;
            }
            else if(this.line == 84)
            {
               text = "With pixel-art graphics and original soundtrack";
               size = 8;
            }
            else if(this.line == 85)
            {
               text = "Puzzles to solve";
               size = 8;
            }
            else if(this.line == 86)
            {
               text = "5 different worlds to explore";
               size = 8;
            }
            else if(this.line == 87)
            {
               text = "This is the story of a young, imaginative boy. Witness his effort to create an utopia.";
               size = 8;
            }
            else if(this.line == 88)
            {
               text = "If everyone was smaller, then everyone would look just the same!";
               size = 8;
            }
            else if(this.line == 100)
            {
               text = "Hans made a plan to make her like him. Why not? Doesn\'t he deserve someone like her?";
               size = 8;
            }
            else if(this.line == 9999)
            {
               text = "She yells: \"Don\'t leave me here!\"";
               size = 8;
            }
            else if(this.line == 101)
            {
               text = "These lovely people made it all possible:";
               size = 8;
            }
            else if(this.line == 102)
            {
               text = "Tarik Kaya,";
               size = 8;
            }
            else if(this.line == 103)
            {
               text = "Nazire Aslan,";
               size = 8;
            }
            else if(this.line == 104)
            {
               text = "My Family,";
               size = 8;
            }
            else if(this.line == 105)
            {
               text = "Nowhere Studios";
               size = 8;
            }
            else if(this.line == 106)
            {
               text = "Without your help, support and motivation, this game wouldn\'t be able to come to life.";
               size = 8;
            }
            else if(this.line == 107)
            {
               text = "Very Special Thanks To:";
               size = 8;
            }
            else if(this.line == 108)
            {
               text = "Orcun Nisli,";
               size = 8;
            }
            else if(this.line == 109)
            {
               text = "Burak Tezateser,";
               size = 8;
            }
            else if(this.line == 110)
            {
               text = "Refik Toksoy,";
               size = 8;
            }
            else if(this.line == 111)
            {
               text = "and the whole Nowhere Studios team.";
               size = 8;
            }
         }
      }
      
      override public function update() : void
      {
         if(this.written && alpha < 1 && !this.erase)
         {
            alpha += 0.02;
         }
         else if(this.erase && alpha > 0)
         {
            alpha -= 0.02;
         }
         if(this.line != this.oldline)
         {
            this.erase = true;
            if(alpha <= 0)
            {
               this.oldline = this.line;
               this.erase = false;
               this.written = false;
               this.kill();
               if(this.line == 67)
               {
                  x = 816;
               }
               else if(this.line == 66)
               {
                  x = 840;
               }
               else if(this.line >= 65 && this.line <= 68)
               {
                  x = 832;
               }
               else if(this.line == 64)
               {
                  x = 848;
               }
               else if(this.line >= 63 && this.line <= 69)
               {
                  x = 840;
               }
            }
         }
      }
   }
}


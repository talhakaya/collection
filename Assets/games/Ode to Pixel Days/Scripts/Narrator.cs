namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The story text. Ported from Narrator.as.
	///
	/// A Narrator is one line of the narration, chosen by number. It starts invisible and
	/// fades in once it has been "written" - immediately, or when the player walks into the
	/// NarratorTouch that owns it. Changing line fades the old text out and the new one in.
	///
	/// "kill" does not kill it: here it means "write yourself", because the trigger that
	/// reveals a narrator is the same overlap callback that kills everything else Hans
	/// touches. That is the source's naming.
	///
	/// The source spells the lines out as one long if-chain in kill(), each setting the text
	/// and the size (always 8). The chain is a table here, in the same order.
	/// </summary>
	public class Narrator : FlxText
	{
		public bool workByTouch;
		private bool written = false;
		public bool erase;
		public uint line;
		private uint oldline;

		public Narrator(double _X, double _Y, int _width, uint _line, bool _workByTouch) : base(_X, _Y, _width, "", true)
		{
			workByTouch = _workByTouch;
			line = _line;
			oldline = _line;
			alpha = 0;
			if (!workByTouch)
			{
				kill();
			}
		}

		public override void kill()
		{
			if (!written)
			{
				written = true;
				alpha = 0;
				string lineText;
				if (Lines.TryGetValue(line, out lineText))
				{
					text = lineText;
					size = 8;
				}
			}
		}

		public override void update()
		{
			if (written && alpha < 1 && !erase)
			{
				alpha += 0.02;
			}
			else if (erase && alpha > 0)
			{
				alpha -= 0.02;
			}

			if (line != oldline)
			{
				erase = true;
				if (alpha <= 0)
				{
					oldline = line;
					erase = false;
					written = false;
					kill();
					if (line == 67)
					{
						x = 816;
					}
					else if (line == 66)
					{
						x = 840;
					}
					else if (line >= 65 && line <= 68)
					{
						x = 832;
					}
					else if (line == 64)
					{
						x = 848;
					}
					else if (line >= 63 && line <= 69)
					{
						x = 840;
					}
				}
			}
		}

		private static readonly System.Collections.Generic.Dictionary<uint, string> Lines = new System.Collections.Generic.Dictionary<uint, string>
		{
			{ 1, "Hans liked a cheerleader. Because she was pretty." },
			{ 2, "The cheerleader didn’t like Hans. Because he was ugly." },
			{ 3, "Hans wished to be a better-looking person." },
			{ 4, "But that wasn't gonna happen." },
			{ 5, "He cried and cried over the girl" },
			{ 6, "- the idealized female he created in his mind." },
			{ 7, "The months of crying led to an escape plan. He thought it might just work." },
			{ 8, "What if things were simpler? What if everyone was even?" },
			{ 9, "What if everyone looked just the same?" },
			{ 10, "Hans spent his nights building a magical machine that would make his wish come true." },
			{ 11, "He pulled the lever." },
			{ 12, "As Hans became smaller, he thought he looked more like a normal person!" },
			{ 1213, "Hans was proud with his machine. If everyone got smaller, then everyone would look just the same. Thank god, we live in a world of pixels!" },
			{ 13, "He saw her, so calm and pretty. His heart beat faster." },
			{ 14, "That didn’t go well at all." },
			{ 15, "But he could try once more." },
			{ 16, "Hans thought he looked more acceptable now." },
			{ 17, "Hans may be the last ugly boy on earth. What a rewarding journey, he thought." },
			{ 18, "And Hans failed one more time." },
			{ 19, "Well, what can you do... Try again?" },
			{ 20, "This is getting boring and hard.            Hans started missing his old life." },
			{ 2021, "But Hans smiled, climbing was bearable if love awaits on the top of the mountain." },
			{ 21, "And again, he thought maybe, he could finally have her." },
			{ 22, "Hans started hating this journey." },
			{ 23, "Hans is the first member of a new breed. A utopia, slowly coming together." },
			{ 2324, "Two pixels of loneliness; impatient, and depressed." },
			{ 24, "Just a few more steps... Here she comes." },
			{ 25, "God, look at that pixel!" },
			{ 26, "Heeeey, you made it! You have the girlfriend of your dreams!" },
			{ 27, "Wow, you must be really happy right now! Congrats!" },
			{ 28, "You look perfect as a couple." },
			{ 29, "It’s every guy’s dream to have a girlfriend like her." },
			{ 30, "Not that every guy deserves one, though." },
			{ 31, "Who deserves who, anyway?" },
			{ 32, "Well, aren’t you guys adorable!" },
			{ 3232, "Hans can't suppress the feeling that she's a burden on him more than a lover." },
			{ 3233, "" },
			{ 3234, "But ain't she so sweet, jumping like a bunny." },
			{ 3235, "And now Hans understands that he can go on loving her, forever." },
			{ 3236, "Even if that means she will limit his life in every aspect." },
			{ 33, "Hmm, there’s something weird..." },
			{ 34, "Why does she seem displeased?" },
			{ 35, "Is she afraid of something?" },
			{ 36, "Does she miss her old life?" },
			{ 37, "She was so popular and pretty back in the old life." },
			{ 38, "Now she’s just two pixels, like everyone else." },
			{ 39, "She just wants to be different..." },
			{ 40, "...Just like you want to be equal." },
			{ 41, "She feels trapped." },
			{ 42, "She wants to run away from all these." },
			{ 43, "Don’t let her go!" },
			{ 44, "Where is she?" },
			{ 45, "Is that your machine?" },
			{ 46, "Oh there she is!" },
			{ 47, "This can't be good." },
			{ 48, "" },
			{ 49, "I'll just pretend all the weird stuff never happened! It would be great if we knew where she was though!" },
			{ 50, "Hans, I feel something dangerous. Just keep walking no matter what." },
			{ 5051, "Ahh, that was hard! Maybe now we can breathe easily." },
			{ 51, "Hans, be careful." },
			{ 52, "It's the cheerleader!" },
			{ 53, "Stop her, you worked hard for all these!" },
			{ 54, "Damn it! We lost the girl again.                             And another box puzzle, how lovely!" },
			{ 55, "I think you shouldn't lose the box on this one." },
			{ 56, "Why are we here, anyway? Is she really worth all this trouble? " },
			{ 57, "There she is again..." },
			{ 58, "Well, we saw that coming anyway. Now, I think we've acted naive long enough. We have to figure this out." },
			{ 5859, "Hans, maybe the question is, why do you need the cheerleader so much?" },
			{ 59, "Maybe the reason is you, Hans. It's not about the cheerleader." },
			{ 5960, "She's just a confused child, just like you." },
			{ 5961, "But this is your story. Maybe we're here, talking, because you feel ugly, and you want to be handsome, and cool, and charismatic..." },
			{ 5962, "You wanted to be special, but look at yourself now." },
			{ 60, "It's time to face your mistakes, Hans." },
			{ 61, "It's the hard times, that teach you. It's the hard times, that get you closer to the person you want to be." },
			{ 62, "Hans, you haven't been in a battle with yourself before. Just be strong. And believe me, this too shall pass." },
			{ 63, "KNOW YOUR INSTINCTS" },
			{ 64, "TAKE CONTROL" },
			{ 65, "YOU'RE WHAT YOU ARE" },
			{ 66, "FIND YOUR WAY" },
			{ 67, "DON'T EXPECT FROM OTHERS" },
			{ 68, "YOU'RE ONE AND ONLY" },
			{ 69, "EXPLORE YOURSELF" },
			{ 70, "You did it, Hans. It's over." },
			{ 71, "Did you miss the old Hans?" },
			{ 72, "I don't think this castle can hold your dreams anymore." },
			{ 73, "Have you ever wanted to fly, Hans?" },
			{ 74, "Now is your chance." },
			{ 75, "Thanks for playing." },
			{ 76, "A game by Talha Kaya" },
			{ 77, "Game Design," },
			{ 78, "Graphics, Animations," },
			{ 79, "Programming," },
			{ 80, "Music, Sfx: Talha Kaya" },
			{ 84, "With pixel-art graphics and original soundtrack" },
			{ 85, "Puzzles to solve" },
			{ 86, "5 different worlds to explore" },
			{ 87, "This is the story of a young, imaginative boy. Witness his effort to create an utopia." },
			{ 88, "If everyone was smaller, then everyone would look just the same!" },
			{ 100, "Hans made a plan to make her like him. Why not? Doesn't he deserve someone like her?" },
			{ 9999, "She yells: \"Don't leave me here!\"" },
			{ 101, "These lovely people made it all possible:" },
			{ 102, "Tarik Kaya," },
			{ 103, "Nazire Aslan," },
			{ 104, "My Family," },
			{ 105, "Nowhere Studios" },
			{ 106, "Without your help, support and motivation, this game wouldn't be able to come to life." },
			{ 107, "Very Special Thanks To:" },
			{ 108, "Orcun Nisli," },
			{ 109, "Burak Tezateser," },
			{ 110, "Refik Toksoy," },
			{ 111, "and the whole Nowhere Studios team." },
		};
	}
}

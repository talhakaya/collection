using System.Collections.Generic;

namespace Games.SleepyTime
{
	/// <summary>
	/// The dialogue, transcribed from DialogueScreen.as.
	///
	/// One entry per exchange: who is speaking, and three lines of text shown together.
	/// Keyed by GameManager.id, which is -2 for the fullscreen note, -1 for the tutorial,
	/// 0..5 for the songs and 6 for the ending.
	/// </summary>
	public static class SleepyDialogue
	{
		public class Entry
		{
			public string character;
			public string first;
			public string second;
			public string third;

			public Entry(string character, string first, string second, string third)
			{
				this.character = character;
				this.first = first;
				this.second = second;
				this.third = third;
			}
		}

		public static List<Entry> forId(int id)
		{
			List<Entry> entries = new List<Entry>();
			switch (id)
			{
				case -2:
					entries.Add(new Entry("sleepy time", "", "fullscreen might not work on flash", ""));
					break;
				case -1:
					entries.Add(new Entry("tutorial", "press 'space' or any button to play", "single press when on a white thing", "long press if the white thing goes on"));
					break;
				case 0:
					entries.Add(new Entry("kayabros", "Awkwardly Presents", "", "Press any button or click"));
					entries.Add(new Entry("sleepy time", "", "sleepy time", ""));
					entries.Add(new Entry("headphones", "Headphones are recommended", "Just hear the thing, would ya?", ""));
					entries.Add(new Entry("sleepy time", "", "press 'f' for fullscreen on/off", ""));
					entries.Add(new Entry("bereket", "nothing will ever be the same", "after you play this game", "are you sure you want to play it?"));
					entries.Add(new Entry("sad", "I am a super hero", "at day I'm a regular man", "at night I fight with depression"));
					entries.Add(new Entry("bereket", "listen to me", "you can press any button", "for example 'space'"));
					entries.Add(new Entry("sad", "", "...", ""));
					entries.Add(new Entry("bereket", "this is a rhythm game like guitar hero", "press when you are on a white thing", "keep pressing until the white thing ends"));
					entries.Add(new Entry("sad", "this is stupid", "you are stupid", ""));
					entries.Add(new Entry("tutorial", "press 'space' or any button to play", "single press when on a white thing", "long press if the white thing goes on"));
					break;
				case 1:
					entries.Add(new Entry("blind", "confusion is your friend", "you should not be afraid", "be human"));
					entries.Add(new Entry("bereket", "when you can't sleep", "you don't count sheep, do you?", "what do you do?"));
					entries.Add(new Entry("bald", "what's the meaning of all this?", "is this supposed to be fun?", ""));
					entries.Add(new Entry("bereket", "this is a game", "you will have to play it to sleep", "you wanna sleep, don't ya?"));
					entries.Add(new Entry("sad", "I want to sleep", "sleep forever", "until the end of times"));
					entries.Add(new Entry("bereket", "then play it", "you are feeling bad", "you must be a spoiled child"));
					break;
				case 2:
					entries.Add(new Entry("mother", "where's my baby", "where's my love", "why is this happening"));
					entries.Add(new Entry("bereket", "there is no reason", "there is no love", "there is nothing"));
					entries.Add(new Entry("mother", "where are my sons", "are they okay", "help me"));
					entries.Add(new Entry("bereket", "there is no pain", "nothing to worry", "nothing to suffer"));
					entries.Add(new Entry("mother", "", "...", ""));
					entries.Add(new Entry("bereket", "trust me, just sleep", "it's all gone", "it was always gone"));
					break;
				case 3:
					entries.Add(new Entry("freaky", "what are you doing?", "who are you fooling again?", "this ends now"));
					entries.Add(new Entry("bereket", "it's not over yet", "we have much to see", "they have to see"));
					entries.Add(new Entry("buda", "it's okay", "they know they are depressed", "so they will get better"));
					entries.Add(new Entry("bereket", "don't hold your breathe", "that won't happen", ""));
					entries.Add(new Entry("buda", "", "how do you know that?", ""));
					entries.Add(new Entry("bereket", "these guys are losers", "big fucking losers", "they are useless pricks"));
					entries.Add(new Entry("buda", "I think there must be", "another way for these people", ""));
					entries.Add(new Entry("freaky", "when the time comes", "they'll be free", "until then, they are yours"));
					entries.Add(new Entry("bereket", "", "thank you", ""));
					break;
				case 4:
					entries.Add(new Entry("lover", "wow, what a mess", "this won't get better", "I guess I left just in time"));
					entries.Add(new Entry("mother", "my son, is he ok?", "what did you do to him?", "fix it!"));
					entries.Add(new Entry("lover", "I didn't do anything", "I was just passing by", "he hurt me more than I hurt him"));
					entries.Add(new Entry("bereket", "I think he looks okay", "look at what he is doing", "he is having fun. fun!"));
					break;
				case 5:
					entries.Add(new Entry("politician", "ladies and gentleman", "we can cure depression", "once and for all"));
					entries.Add(new Entry("tache", "you have to fix it", "these people", "they are suffering"));
					entries.Add(new Entry("politician", "so as I said", "and it shall be done", ""));
					entries.Add(new Entry("tache", "do you even believe yourself?", "you're horrible", "you can't even fix yourself"));
					entries.Add(new Entry("politician", "I don't worry", "I don't suffer", "it shall be done"));
					entries.Add(new Entry("ceasar", "you have nothing to do with this", "leave it alone", "let us, real humans, deal with it"));
					break;
				case 6:
					entries.Add(new Entry("bereket", "there's a pattern to everything", "everything humans do", ""));
					entries.Add(new Entry("ceasar", "everything they worry about", "everything they desire", "everything they suffer for"));
					entries.Add(new Entry("sad", "humans are after the very same thing", "as if they are one", "as if there is only one person"));
					entries.Add(new Entry("ceasar", "and there's a pattern", "to everything", "to life and death"));
					entries.Add(new Entry("mother", "it's ok", "I have died", "they will go on"));
					entries.Add(new Entry("sad", "we are all super heroes", "fighting with depression", "fighting with ourselves"));
					entries.Add(new Entry("bereket", "", "you are all fucked up", ""));
					entries.Add(new Entry("sad", "we are fucked up", "and we are proud", ""));
					entries.Add(new Entry("bereket", "", "...", ""));
					entries.Add(new Entry("sad", "", "...", ""));
					entries.Add(new Entry("bereket", "", "...", ""));
					entries.Add(new Entry("sad", "", "...", ""));
					entries.Add(new Entry("bereket", "", "fuck you all", ""));
					break;
			}

			return entries;
		}
	}
}

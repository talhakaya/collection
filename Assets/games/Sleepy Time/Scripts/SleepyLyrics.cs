using System.Collections.Generic;

namespace Games.SleepyTime
{
	/// <summary>
	/// The lyrics, transcribed from Scene.as's putInLyrics.
	///
	/// Times are written as per-line deltas in beats and accumulated into absolute beats at
	/// the end, exactly as the original does - updateLyrics then compares them against
	/// GameManager.time / GameManager.rhythm. Empty lines are not padding: they are what
	/// clears the previous line off the screen.
	/// </summary>
	public static class SleepyLyrics
	{
		public static List<Lyric> putInLyrics(int id)
		{
			List<Lyric> lyrics = new List<Lyric>();
			switch (id)
			{
				case 1:
					lyrics.Add(new Lyric(5f, "Everyday you wake up at seven"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(2f, "Your shift starts at eight"));
					lyrics.Add(new Lyric(6f, ""));
					lyrics.Add(new Lyric(2f, "You work thirteen hours a day"));
					lyrics.Add(new Lyric(6f, ""));
					lyrics.Add(new Lyric(2f, "Don't you deserve some rest?"));
					lyrics.Add(new Lyric(8f, ""));
					lyrics.Add(new Lyric(7f, "Oh how you'd like a job that is"));
					lyrics.Add(new Lyric(7f, "Nine to five"));
					lyrics.Add(new Lyric(8f, ""));
					lyrics.Add(new Lyric(7f, "You'd have all the time in the world"));
					lyrics.Add(new Lyric(9f, "Just work nine to five"));
					lyrics.Add(new Lyric(8f, ""));
					lyrics.Add(new Lyric(7f, "Every night you get home at eleven"));
					lyrics.Add(new Lyric(7f, "You're so tired you can't watch a movie"));
					lyrics.Add(new Lyric(8f, "Without your head falling"));
					lyrics.Add(new Lyric(6f, ""));
					lyrics.Add(new Lyric(4f, "How you'd watch everything you find"));
					lyrics.Add(new Lyric(4f, "When you lived with your mom"));
					lyrics.Add(new Lyric(8f, ""));
					lyrics.Add(new Lyric(5f, "But now how you'd like a job that is"));
					lyrics.Add(new Lyric(7f, "Nine to five"));
					lyrics.Add(new Lyric(8f, ""));
					lyrics.Add(new Lyric(7f, "You'd have all the time in the world"));
					lyrics.Add(new Lyric(7f, "Just work nine to five"));
					lyrics.Add(new Lyric(10f, ""));
					break;
				case 2:
					lyrics.Add(new Lyric(5f, "Everyday, you wake up at eleven"));
					lyrics.Add(new Lyric(13f, ""));
					lyrics.Add(new Lyric(2f, "With booze near you, some cigarette fallen"));
					lyrics.Add(new Lyric(14f, ""));
					lyrics.Add(new Lyric(2f, "To the floor where there is nothing"));
					lyrics.Add(new Lyric(14f, ""));
					lyrics.Add(new Lyric(2f, "But your whole messy life in a nutshell"));
					lyrics.Add(new Lyric(10f, ""));
					lyrics.Add(new Lyric(8f, "Some say you are a loser"));
					lyrics.Add(new Lyric(12f, ""));
					lyrics.Add(new Lyric(4f, "Your friends look at you with pity"));
					lyrics.Add(new Lyric(8f, ""));
					lyrics.Add(new Lyric(4f, "And you,"));
					lyrics.Add(new Lyric(8f, "You know"));
					lyrics.Add(new Lyric(8f, "There is"));
					lyrics.Add(new Lyric(8f, "No hope"));
					lyrics.Add(new Lyric(6f, "For you"));
					lyrics.Add(new Lyric(12f, ""));
					lyrics.Add(new Lyric(8f, "Since your mother had cancer"));
					lyrics.Add(new Lyric(13f, ""));
					lyrics.Add(new Lyric(2f, "You've been living off of your father"));
					lyrics.Add(new Lyric(14f, ""));
					lyrics.Add(new Lyric(2f, "Even if he didn't hate you"));
					lyrics.Add(new Lyric(14f, ""));
					lyrics.Add(new Lyric(2f, "You would feel bad about the money"));
					lyrics.Add(new Lyric(6f, "He's giving you"));
					lyrics.Add(new Lyric(4f, ""));
					lyrics.Add(new Lyric(8f, "Some say you are a loser"));
					lyrics.Add(new Lyric(12f, ""));
					lyrics.Add(new Lyric(4f, "Your friends look at you with pity"));
					lyrics.Add(new Lyric(8f, ""));
					lyrics.Add(new Lyric(4f, "And you,"));
					lyrics.Add(new Lyric(8f, "You know"));
					lyrics.Add(new Lyric(8f, "There is"));
					lyrics.Add(new Lyric(8f, "No hope"));
					lyrics.Add(new Lyric(6f, "For you"));
					lyrics.Add(new Lyric(6f, ""));
					break;
				case 3:
					lyrics.Add(new Lyric(4f, "Everyday you wake up at eight"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "With power and struggle you hate"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "Nausea keeps coming back for more"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "Oh you'd rather just seeing your boys"));
					lyrics.Add(new Lyric(7f, "Your boys"));
					lyrics.Add(new Lyric(8f, "One is lost and one is high"));
					lyrics.Add(new Lyric(8f, "Your boys"));
					lyrics.Add(new Lyric(8f, "One is dying and one has tried"));
					lyrics.Add(new Lyric(8f, "Your boys"));
					lyrics.Add(new Lyric(4f, ""));
					lyrics.Add(new Lyric(4f, "Since you lost your wife"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "You can't sleep before five AM"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "You'd die for one more moment"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "With your love"));
					lyrics.Add(new Lyric(7f, "Your love"));
					lyrics.Add(new Lyric(8f, "Cancer took her she is gone"));
					lyrics.Add(new Lyric(8f, "Your love"));
					lyrics.Add(new Lyric(8f, "She is missed and she is loved"));
					lyrics.Add(new Lyric(8f, "Your love"));
					lyrics.Add(new Lyric(8f, ""));
					break;
				case 4:
					lyrics.Add(new Lyric(5f, "Everyday you wake up at whatever"));
					lyrics.Add(new Lyric(7.5f, ""));
					lyrics.Add(new Lyric(0.5f, "It's hard, from your bed, to see the clock"));
					lyrics.Add(new Lyric(15f, ""));
					lyrics.Add(new Lyric(1f, "Not that you'd miss out on a lot"));
					lyrics.Add(new Lyric(7.5f, ""));
					lyrics.Add(new Lyric(0.5f, "Without love, love, love"));
					lyrics.Add(new Lyric(15f, ""));
					lyrics.Add(new Lyric(14f, "Like the wind once here it's gone"));
					lyrics.Add(new Lyric(12f, ""));
					lyrics.Add(new Lyric(4f, "Their eyes looking from far"));
					lyrics.Add(new Lyric(12f, ""));
					lyrics.Add(new Lyric(4f, "So far that you can't see"));
					lyrics.Add(new Lyric(12f, ""));
					lyrics.Add(new Lyric(7f, "Two women that you cared about"));
					lyrics.Add(new Lyric(7.5f, ""));
					lyrics.Add(new Lyric(0.5f, "One was your mother one was your lover"));
					lyrics.Add(new Lyric(13f, ""));
					lyrics.Add(new Lyric(0.5f, "All the times you felt like one another"));
					lyrics.Add(new Lyric(9.5f, ""));
					lyrics.Add(new Lyric(0.5f, "But both have gone, gone away"));
					lyrics.Add(new Lyric(15.5f, ""));
					lyrics.Add(new Lyric(14f, "Like the wind once here it's gone"));
					lyrics.Add(new Lyric(12f, ""));
					lyrics.Add(new Lyric(4f, "Now the doctor's saying"));
					lyrics.Add(new Lyric(6f, "You don't have much time"));
					lyrics.Add(new Lyric(6f, ""));
					lyrics.Add(new Lyric(4f, "As if you had much to live for"));
					lyrics.Add(new Lyric(12f, ""));
					lyrics.Add(new Lyric(15f, "All the times you felt like you've had it"));
					lyrics.Add(new Lyric(7.5f, ""));
					lyrics.Add(new Lyric(0.5f, "You should be gone, gone away"));
					lyrics.Add(new Lyric(15f, ""));
					break;
				case 5:
					lyrics.Add(new Lyric(5f, "Everyday you wake up again"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "Wish you didn't have to begin"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "Every new day is a new black page"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "Shouldn't have happened, happened anyway"));
					lyrics.Add(new Lyric(8f, ""));
					lyrics.Add(new Lyric(4f, "Pathetic"));
					lyrics.Add(new Lyric(15f, ""));
					lyrics.Add(new Lyric(1f, "What is this sound in your head"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "Hitting walls and finding its way"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(2f, "Pathetic"));
					lyrics.Add(new Lyric(15f, ""));
					lyrics.Add(new Lyric(3f, "The time you tried to end it all"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "Accepting that everything was your fault"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "Tried to be at peace, and let it go"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "The only way you knew how to"));
					lyrics.Add(new Lyric(8f, ""));
					lyrics.Add(new Lyric(14f, "Pathetic"));
					lyrics.Add(new Lyric(15f, ""));
					lyrics.Add(new Lyric(1f, "What is this sound in your head"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "Hitting walls and finding its way"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(2f, "Pathetic"));
					lyrics.Add(new Lyric(53f, ""));
					lyrics.Add(new Lyric(9f, "What is this sound in your head"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "Hitting walls and finding its way"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(6f, "Pathetic"));
					lyrics.Add(new Lyric(30f, ""));
					break;
				case 6:
					lyrics.Add(new Lyric(4f, "Everyday you wake up together"));
					lyrics.Add(new Lyric(6f, "You don't know, you're not getting better"));
					lyrics.Add(new Lyric(6f, "At doing things, connecting with others"));
					lyrics.Add(new Lyric(6f, "All sad, since you lost your mother"));
					lyrics.Add(new Lyric(7f, ""));
					lyrics.Add(new Lyric(1f, "Now and here"));
					lyrics.Add(new Lyric(6f, "To share what you feel"));
					lyrics.Add(new Lyric(6f, "No matter how bad"));
					lyrics.Add(new Lyric(6f, "We are all that there is"));
					lyrics.Add(new Lyric(6f, ""));
					lyrics.Add(new Lyric(2f, "From the sun, from the wind"));
					lyrics.Add(new Lyric(11f, ""));
					lyrics.Add(new Lyric(2f, "Although it's hard"));
					lyrics.Add(new Lyric(6f, "It's the only thing we can try"));
					lyrics.Add(new Lyric(6f, "It's just us, nothing to hide"));
					lyrics.Add(new Lyric(6f, "We're fucked up, we should be proud"));
					lyrics.Add(new Lyric(11f, ""));
					lyrics.Add(new Lyric(1f, "Now and here"));
					lyrics.Add(new Lyric(6f, "To share what you feel"));
					lyrics.Add(new Lyric(6f, "No matter how bad"));
					lyrics.Add(new Lyric(6f, "We are all that there is"));
					lyrics.Add(new Lyric(6f, ""));
					lyrics.Add(new Lyric(5f, "From the sun, from the wind"));
					lyrics.Add(new Lyric(48f, ""));
					break;
			}

			float runningTime = 0f;
			for (int i = 0; i < lyrics.Count; i++)
			{
				runningTime += lyrics[i].time;
				lyrics[i].time = runningTime;
			}

			return lyrics;
		}
	}
}

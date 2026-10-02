using System;
using System.Collections.Generic;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The tune of Level 41: six notes, played in a freshly shuffled order each time round,
	/// louder the higher Hans climbs. Ported from MusicGenerator.as.
	///
	/// It is a sprite only so that the level updates it; it draws nothing.
	/// </summary>
	public class MusicGenerator : FlxSprite
	{
		private static string Sfx1 = "MusicGenerator_Sfx1";
		private static string Sfx2 = "MusicGenerator_Sfx2";
		private static string Sfx3 = "MusicGenerator_Sfx3";
		private static string Sfx4 = "MusicGenerator_Sfx4";
		private static string Sfx5 = "MusicGenerator_Sfx5";
		private static string Sfx6 = "MusicGenerator_Sfx6";

		private Hans hans;
		private int count;
		private double volume;
		public int HIZ;
		private int yer;
		private List<int> erey;

		public MusicGenerator(Hans _h, double _Y, int hiz)
		{
			hans = _h;
			y = _Y;
			HIZ = hiz;
			alpha = 0;
		}

		public override void update()
		{
			base.update();
			if (count == 0)
			{
				erey = createArray();
				playNote(erey[0]);
			}
			else if (count == HIZ)
			{
				playNote(erey[1]);
			}
			else if (count == HIZ * 2)
			{
				playNote(erey[2]);
			}
			else if (count == HIZ * 3)
			{
				playNote(erey[3]);
			}
			else if (count == HIZ * 4)
			{
				playNote(erey[4]);
			}
			else if (count == HIZ * 5)
			{
				playNote(erey[5]);
			}
			else if (count == HIZ * 6 - 1)
			{
				count = -1;
			}

			++count;
		}

		private List<int> createArray()
		{
			int i = 0;
			int j = 0;
			int turn = 0;
			double random = double.NaN;
			List<int> temp = new List<int> { 1, 2, 3, 4, 5, 6 };
			List<int> erey = new List<int>();
			for (turn = 0; turn < 6; turn++)
			{
				i = (int)Math.Floor(FlxG.random() * temp.Count);
				erey.Add(temp[i]);
				temp.RemoveAt(i);
			}

			return erey;
		}

		private void playNote(int i)
		{
			if (hans.y - y > 88)
			{
				volume = 0;
			}
			else if (hans.y - y > 0)
			{
				volume = 1 - (hans.y - y) / 88;
			}
			else
			{
				volume = 1;
			}

			if (i == 1)
			{
				FlxG.play(Sfx1, volume);
			}
			else if (i == 2)
			{
				FlxG.play(Sfx2, volume);
			}
			else if (i == 3)
			{
				FlxG.play(Sfx3, volume);
			}
			else if (i == 4)
			{
				FlxG.play(Sfx4, volume);
			}
			else if (i == 5)
			{
				FlxG.play(Sfx5, volume);
			}
			else if (i == 6)
			{
				FlxG.play(Sfx6, volume);
			}
		}
	}
}

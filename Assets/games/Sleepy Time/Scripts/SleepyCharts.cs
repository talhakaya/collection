using System.Collections.Generic;

namespace Games.SleepyTime
{
	/// <summary>
	/// The beat charts, transcribed from Scene.as's getTrackArray.
	///
	/// The numbers are the song's rhythm in beats, read as alternating (gap, hold-length)
	/// pairs: putInBeats walks them keeping a running time, emitting a Beat on every second
	/// value. A hold length of 0 is a tap rather than a hold.
	///
	/// Deliberately kept as literal code, loops and all, in the shape the original wrote it.
	/// No new charts will ever be authored, so a ScriptableObject pipeline would buy nothing
	/// and cost a second source of truth.
	///
	/// putInBeats reads GameManager.rhythm, so a chart is only correct for the song that was
	/// selected when the Scene was built. SceneManager.changeMusic runs before every Scene is
	/// constructed, which is what makes that hold - including for the ghost replays, which
	/// are therefore laid out against the active song's rhythm, as in the original.
	/// </summary>
	public static class SleepyCharts
	{
		public static List<Beat> getTrackArray(int id)
		{
			List<float> track = new List<float>();
			switch (id)
			{
				case 1:
					track.Add(4f);
					for (int i = 0; i < 8; i++)
					{
						track.Add(2f);
						track.Add(2f);
					}
					for (int i = 0; i < 6; i++)
					{
						track.Add(2f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(2f);
						track.Add(2f);
					}
					for (int i = 0; i < 4; i++)
					{
						track.Add(2f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(2f);
						track.Add(2f);
					}
					for (int i = 0; i < 8; i++)
					{
						track.Add(0f);
						track.Add(1f);
						track.Add(1f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
					}
					track.Add(7f);
					track.Add(1f);
					for (int i = 0; i < 4; i++)
					{
						track.Add(2f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(2f);
						track.Add(2f);
					}
					for (int i = 0; i < 6; i++)
					{
						track.Add(2f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(2f);
						track.Add(2f);
					}
					break;
				case 2:
					track.Add(4f);
					for (int i = 0; i < 8; i++)
					{
						track.Add(1f);
						track.Add(1f);
						track.Add(1f);
						track.Add(1f);
						track.Add(1f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
					}
					for (int i = 0; i < 4; i++)
					{
						track.Add(2f);
						track.Add(3f);
						track.Add(0f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(1f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(1f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
					}
					track.Add(15f);
					track.Add(1f);
					for (int i = 0; i < 8; i++)
					{
						track.Add(1f);
						track.Add(1f);
						track.Add(1f);
						track.Add(1f);
						track.Add(1f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
					}
					for (int i = 0; i < 4; i++)
					{
						track.Add(2f);
						track.Add(3f);
						track.Add(0f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(1f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(1f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
					}
					track.Add(15f);
					track.Add(1f);
					break;
				case 3:
					track.Add(4f);
					for (int i = 0; i < 8; i++)
					{
						track.Add(1.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0f);
						track.Add(1f);
					}
					for (int i = 0; i < 4; i++)
					{
						track.Add(1f);
						track.Add(1f);
						track.Add(1f);
						track.Add(1f);
					}
					for (int i = 0; i < 4; i++)
					{
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(1f);
						track.Add(1f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(0.5f);
						track.Add(1f);
						track.Add(0.5f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
					}
					for (int i = 0; i < 8; i++)
					{
						track.Add(1.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0f);
						track.Add(1f);
					}
					for (int i = 0; i < 4; i++)
					{
						track.Add(1f);
						track.Add(1f);
						track.Add(1f);
						track.Add(1f);
					}
					for (int i = 0; i < 4; i++)
					{
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(1f);
						track.Add(1f);
					}
					for (int i = 0; i < 8; i++)
					{
						track.Add(1f);
						track.Add(1f);
						track.Add(1f);
						track.Add(1f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(0.5f);
						track.Add(1f);
						track.Add(0.5f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
					}
					track.Add(0f);
					break;
				case 4:
					track.Add(4f);
					for (int i = 0; i < 12; i++)
					{
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(1f);
						track.Add(1f);
					}
					for (int i = 0; i < 4; i++)
					{
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(1.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(1f);
						track.Add(2f);
						track.Add(0f);
						track.Add(1f);
						track.Add(6.5f);
						track.Add(0.5f);
					}
					for (int i = 0; i < 12; i++)
					{
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(1f);
						track.Add(1f);
					}
					for (int i = 0; i < 3; i++)
					{
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(1.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(1f);
						track.Add(2f);
						track.Add(0f);
						track.Add(1f);
						track.Add(6.5f);
						track.Add(0.5f);
					}
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(1.5f);
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(2f);
					track.Add(0f);
					track.Add(1f);
					track.Add(14.5f);
					track.Add(0.5f);
					for (int i = 0; i < 5; i++)
					{
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(1f);
						track.Add(1f);
					}
					track.Add(4f);
					break;
				case 5:
					track.Add(4f);
					for (int i = 0; i < 4; i++)
					{
						track.Add(0f);
						track.Add(1f);
						track.Add(0f);
						track.Add(0.5f);
						track.Add(0f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0f);
						track.Add(1f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
					}
					for (int i = 0; i < 2; i++)
					{
						track.Add(0f);
						track.Add(1f);
						track.Add(0f);
						track.Add(1f);
						track.Add(0f);
						track.Add(0.5f);
						track.Add(0f);
						track.Add(1f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(1f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
					}
					track.Add(0.25f);
					track.Add(0.75f);
					track.Add(0.25f);
					track.Add(0.75f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.75f);
					track.Add(0.25f);
					track.Add(0.75f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(0.5f);
					for (int i = 0; i < 3; i++)
					{
						track.Add(0.25f);
						track.Add(0.75f);
						track.Add(0.25f);
						track.Add(0.75f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.75f);
						track.Add(0.25f);
						track.Add(0.75f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(1f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
					}
					for (int i = 0; i < 4; i++)
					{
						track.Add(0f);
						track.Add(1f);
						track.Add(0f);
						track.Add(0.5f);
						track.Add(0f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0f);
						track.Add(1f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
					}
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(1f);
					track.Add(1.5f);
					track.Add(0.5f);
					track.Add(2f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(1.5f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(1f);
					track.Add(1.5f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(0.5f);
					for (int i = 0; i < 9; i++)
					{
						track.Add(0.25f);
						track.Add(0.75f);
						track.Add(0.25f);
						track.Add(0.75f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.75f);
						track.Add(0.25f);
						track.Add(0.75f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(1f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
					}
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(2f);
					track.Add(0f);
					track.Add(3f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(4.5f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(2f);
					track.Add(0f);
					track.Add(3f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(8.5f);
					for (int i = 0; i < 3; i++)
					{
						track.Add(0.25f);
						track.Add(0.75f);
						track.Add(0.25f);
						track.Add(0.75f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.75f);
						track.Add(0.25f);
						track.Add(0.75f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(1f);
						track.Add(0.5f);
						track.Add(0.5f);
						track.Add(0.5f);
					}
					track.Add(0.25f);
					track.Add(0.75f);
					track.Add(0.25f);
					track.Add(0.75f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.75f);
					track.Add(0f);
					break;
				case 6:
					track.Add(3f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.75f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(1.5f);
					track.Add(1.5f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(1.25f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(0.75f);
					track.Add(0f);
					track.Add(0.75f);
					track.Add(0f);
					track.Add(0.75f);
					track.Add(0f);
					track.Add(0.75f);
					track.Add(0f);
					track.Add(0.75f);
					track.Add(0f);
					track.Add(1.5f);
					track.Add(0f);
					track.Add(0.75f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(1.25f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(0.75f);
					track.Add(0f);
					track.Add(0.75f);
					track.Add(0f);
					track.Add(0.75f);
					track.Add(0f);
					track.Add(0.75f);
					track.Add(0f);
					track.Add(0.75f);
					track.Add(0f);
					track.Add(1.5f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(0.5f);
					for (int i = 0; i < 3; i++)
					{
						track.Add(0f);
						track.Add(0.5f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
					}
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(0.5f);
					for (int i = 0; i < 3; i++)
					{
						track.Add(0f);
						track.Add(0.5f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
					}
					track.Add(0f);
					track.Add(1.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.75f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(1f);
					track.Add(0.5f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.5f);
					track.Add(0.5f);
					track.Add(1.5f);
					track.Add(1.5f);
					track.Add(0f);
					track.Add(1.5f);
					track.Add(0f);
					track.Add(1.5f);
					for (int i = 0; i < 4; i++)
					{
						track.Add(0f);
						track.Add(0.5f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(1f);
						track.Add(0.5f);
						track.Add(0f);
						track.Add(0.5f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(0.25f);
						track.Add(1.25f);
						track.Add(0f);
						track.Add(0.5f);
					}
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0.25f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(0.5f);
					track.Add(0f);
					track.Add(1f);
					track.Add(0f);
					track.Add(0.5f);
					for (int i = 0; i < 8; i++)
					{
						for (int i1 = 0; i1 < 3; i1++)
						{
							track.Add(0f);
							track.Add(0.5f);
							track.Add(0.25f);
							track.Add(0.25f);
							track.Add(0.25f);
							track.Add(0.25f);
						}
						track.Add(0f);
						track.Add(1f);
						track.Add(0f);
						track.Add(0.5f);
					}
					for (int i = 0; i < 13; i++)
					{
						track.Add(0f);
						track.Add(1.5f);
						track.Add(0f);
						track.Add(1.5f);
						track.Add(0f);
						track.Add(1.5f);
						track.Add(0f);
						track.Add(1.5f);
					}
					for (int i = 0; i < 4; i++)
					{
						for (int i1 = 0; i1 < 3; i1++)
						{
							track.Add(0.25f);
							track.Add(0.25f);
							track.Add(0.25f);
							track.Add(0.25f);
							track.Add(0.25f);
							track.Add(0.25f);
						}
						track.Add(1f);
						track.Add(0.5f);
					}
					for (int i = 0; i < 4; i++)
					{
						for (int i1 = 0; i1 < 3; i1++)
						{
							track.Add(0f);
							track.Add(0.5f);
							track.Add(0.25f);
							track.Add(0.25f);
							track.Add(0.25f);
							track.Add(0.25f);
						}
						track.Add(0f);
						track.Add(1f);
						track.Add(0f);
						track.Add(0.5f);
					}
					track.Add(0f);
					break;
				default:
					for (int i = 0; i < 6; i++)
					{
						track.Add(1f);
					}
					break;
			}

			return putInBeats(track);
		}

		/// <summary>
		/// Ported from Scene.as's putInBeats. The state machine reads -2 -> -1 -> emit, so a
		/// Beat comes out of every second value, timed at the running total *before* its own
		/// length is added - and the running total accumulates gaps and hold lengths alike.
		/// </summary>
		public static List<Beat> putInBeats(List<float> track)
		{
			List<Beat> beats = new List<Beat>();
			int runningTime = 0;
			int state = -2;
			for (int i = 0; i < track.Count; i++)
			{
				if (state == -2)
				{
					state = -1;
				}
				else if (state == -1)
				{
					state = (int)System.Math.Round(track[i] * GameManager.rhythm, System.MidpointRounding.AwayFromZero);
					beats.Add(new Beat(runningTime, state));
					state = -2;
				}

				runningTime += (int)System.Math.Round(track[i] * GameManager.rhythm, System.MidpointRounding.AwayFromZero);
			}

			return beats;
		}
	}
}

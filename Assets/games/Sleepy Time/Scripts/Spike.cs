namespace Games.SleepyTime
{
	/// <summary>
	/// The visual for one Beat: a head spike, and for a hold, a tail spike with a line
	/// stretched between them. Ported from Spike.as.
	///
	/// A Sprite rather than a Citmap in the original - it has no artwork of its own, only
	/// Citmap children - so it is a bare FlashObject here.
	///
	/// Every division below was floating point in ActionScript, where int / int still yields
	/// a Number. lengthOfPress and rhythm are both ints, so the casts are load-bearing: in
	/// C# the same expression would truncate and every hold would come out the wrong length.
	/// </summary>
	public class Spike : FlashObject
	{
		public Citmap secondSpike;
		public int sceneId;
		public Citmap line;
		public int lengthOfPress;
		public bool isOnePress;
		public int idInTrack;
		public Citmap firstSpike;

		public static Spike New(int idInTrack = 0, int lengthOfPress = 0, int sceneId = 0)
		{
			Spike spike = NewNode("Spike").AddComponent<Spike>();

			spike.firstSpike = Citmap.New(SleepyAssets.GetSprite("img/spike.png"), 4, 30);
			spike.addChild(spike.firstSpike);
			spike.lengthOfPress = lengthOfPress;
			spike.sceneId = sceneId;

			if (lengthOfPress != 0)
			{
				spike.secondSpike = Citmap.New(SleepyAssets.GetSprite("img/spike.png"), 4, 30);
				spike.secondSpike.x = spike.firstSpike.x + (float)lengthOfPress / GameManager.rhythm * Scene.lengthOfSection;
				spike.addChild(spike.secondSpike);

				spike.line = Citmap.New(SleepyAssets.GetSprite("img/line.png"), 0, 7);
				spike.line.scaleX = (float)lengthOfPress / 100f / GameManager.rhythm * Scene.lengthOfSection;
				spike.line.scaleY = 0.5f;
				spike.addChild(spike.line);
			}

			spike.idInTrack = idInTrack;
			return spike;
		}
	}
}

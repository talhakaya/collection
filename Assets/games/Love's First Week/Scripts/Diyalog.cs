namespace Games.LovesFirstWeek
{
	/// <summary>
	/// One line of dialogue and who says it. Ported from Diyalog.as.
	/// </summary>
	public class Diyalog
	{
		public bool isNaz;
		public string text;

		public Diyalog(bool byNaz, string txt)
		{
			isNaz = byNaz;
			text = txt;
		}
	}
}

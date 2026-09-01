using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// ActionScript's Math.round rounds halves toward positive infinity. Mathf.RoundToInt
	/// and System.Math.Round round halves to even, so 2.5 comes out 2 rather than 3.
	///
	/// That is not academic here: addScore rounds max(amount, amount * combo / 10), which
	/// lands exactly on a half whenever the combo is an odd multiple of 5, so scores would
	/// quietly drift from the original's - and ScoreTable's star thresholds are tuned against
	/// those.
	/// </summary>
	public static class FlashMath
	{
		public static int round(float value)
		{
			return Mathf.FloorToInt(value + 0.5f);
		}
	}
}

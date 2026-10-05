using UnityEngine;

namespace Collection
{
	/// What an AudioSource plays.
	public enum AudioKind
	{
		Sound,
		Music
	}

	/// <summary>
	/// Put next to an AudioSource to say outright whether it is music, where the guess in
	/// CollectionSettings (a long clip, or a looping one of some length) gets it wrong. The
	/// music volume of the settings applies to music only.
	/// </summary>
	public class AudioKindOverride : MonoBehaviour
	{
		public AudioKind kind;
	}
}

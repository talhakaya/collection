using UnityEditor;
using UnityEngine;

namespace Collection.EditorTools
{
	/// <summary>
	/// Switches the project between Gamma and Linear colour space, to compare how the
	/// games look in each.
	///
	/// The ported Flash games were authored for Flash's blending, which matches Gamma:
	/// in Linear, everything semi-transparent (fades, overlays, shadows) comes out lighter
	/// than it was. Older Unity games were most likely made in Gamma too, Unity's default
	/// before 2019.
	///
	/// A colour space can only be chosen in the editor - a built player cannot change it.
	/// Switching re-imports the project's textures, which takes a few minutes, and changes
	/// ProjectSettings.asset: commit that only once the choice is made.
	/// </summary>
	public static class ColorSpaceToggle
	{
		private const string GammaItem = "Tools/Color Space/Gamma";
		private const string LinearItem = "Tools/Color Space/Linear";

		[MenuItem(GammaItem)]
		private static void UseGamma()
		{
			Switch(ColorSpace.Gamma);
		}

		[MenuItem(LinearItem)]
		private static void UseLinear()
		{
			Switch(ColorSpace.Linear);
		}

		[MenuItem(GammaItem, true)]
		private static bool CanUseGamma()
		{
			Menu.SetChecked(GammaItem, PlayerSettings.colorSpace == ColorSpace.Gamma);
			Menu.SetChecked(LinearItem, PlayerSettings.colorSpace == ColorSpace.Linear);
			return !EditorApplication.isPlayingOrWillChangePlaymode;
		}

		[MenuItem(LinearItem, true)]
		private static bool CanUseLinear()
		{
			return CanUseGamma();
		}

		private static void Switch(ColorSpace colorSpace)
		{
			if (PlayerSettings.colorSpace == colorSpace)
			{
				return;
			}

			PlayerSettings.colorSpace = colorSpace;
			Debug.Log($"Color space is now {colorSpace}. Don't commit ProjectSettings.asset until you've decided which to keep.");
		}
	}
}

using UnityEngine;

namespace Collection
{
	/// What kind of build this is.
	public enum BuildType
	{
		/// What players get: no cheats.
		Release,

		/// For testing: the cheats are there.
		Debug
	}

	/// <summary>
	/// Settings of the build as a whole, in one asset: Resources/BuildSettings. Chosen by hand
	/// in the Inspector before building; nothing sets it by itself.
	///
	/// For now only the build type, which decides whether the cheats are there (taking a
	/// story artifact without playing its game through).
	/// </summary>
	[CreateAssetMenu(fileName = "BuildSettings", menuName = "Collection/Build Settings")]
	public class BuildSettings : ScriptableObject
	{
		private const string ResourcePath = "BuildSettings";

		public BuildType buildType = BuildType.Release;

		private static BuildSettings instance;

		/// The asset. With none in Resources, a Release one: a build that has lost its
		/// settings must not come out with the cheats.
		public static BuildSettings Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Resources.Load<BuildSettings>(ResourcePath);
					if (instance == null)
					{
						instance = CreateInstance<BuildSettings>();
					}
				}

				return instance;
			}
		}

		public static bool IsDebug => Instance.buildType == BuildType.Debug;

		/// Whether the cheats are there.
		public static bool Cheats => IsDebug;
	}
}

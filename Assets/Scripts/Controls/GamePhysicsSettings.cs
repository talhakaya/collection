using System;
using UnityEngine;

namespace Collection.Controls
{
	/// <summary>
	/// Per-game Physics2D.gravity overrides, applied automatically by GamePhysicsManager on
	/// scene load. Games not listed here keep whatever gravity is configured project-wide
	/// (Edit &gt; Project Settings &gt; Physics 2D) - this only exists for games whose physics
	/// don't want that shared setting (e.g. a top-down shooter's bullets shouldn't fall).
	/// </summary>
	[CreateAssetMenu(fileName = "GamePhysicsSettings", menuName = "Collection/Game Physics Settings")]
	public class GamePhysicsSettings : ScriptableObject
	{
		[Serializable]
		public struct Entry
		{
			public string gameName;
			public Vector2 gravity;
		}

		public Entry[] overrides = Array.Empty<Entry>();

		public bool TryGetGravity(string gameName, out Vector2 gravity)
		{
			foreach (Entry entry in overrides)
			{
				if (string.Equals(entry.gameName, gameName, StringComparison.OrdinalIgnoreCase))
				{
					gravity = entry.gravity;
					return true;
				}
			}

			gravity = default;
			return false;
		}
	}
}

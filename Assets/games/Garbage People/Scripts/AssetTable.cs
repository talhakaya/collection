using System.Collections.Generic;

namespace Games.GarbagePeople
{
	/// <summary>
	/// The names the game loads its images and sounds under (Phaser's texture and audio
	/// keys), and where each file is, under Resources/GarbagePeople.
	///
	/// Generated from the preload functions of the original scenes. Phaser's caches are
	/// game-wide, so a name loaded by one scene is used by later ones; here every name is
	/// available from the start.
	/// </summary>
	public static class AssetTable
	{
		public static readonly Dictionary<string, string> Images = new Dictionary<string, string>
		{
			{ "arm0", "images/bathroom0/arm" },
			{ "arm1", "images/bathroom1/arm" },
			{ "bathroom0bg", "images/bathroom0/bg" },
			{ "bg", "images/bathroom1/bg" },
			{ "bg_broken", "images/bathroom1/bg_broken" },
			{ "black", "images/black" },
			{ "building0", "images/bathroom1/building0" },
			{ "building1", "images/bathroom1/building1" },
			{ "building2", "images/bathroom1/building2" },
			{ "building3", "images/bathroom1/building3" },
			{ "car", "images/city/car" },
			{ "coin", "images/coin" },
			{ "creep", "images/bathroom1/creep" },
			{ "curtain0", "images/bathroom1/curtain0" },
			{ "curtain1", "images/bathroom1/curtain1" },
			{ "curtain2", "images/bathroom1/curtain2" },
			{ "curtain3", "images/bathroom1/curtain3" },
			{ "detail0", "images/forest/detail0" },
			{ "detail1", "images/forest/detail1" },
			{ "detail2", "images/forest/detail2" },
			{ "detail3", "images/forest/detail3" },
			{ "detail4", "images/forest/detail4" },
			{ "detail5", "images/forest/detail5" },
			{ "detail6", "images/forest/detail6" },
			{ "dirt", "images/forest/dirt" },
			{ "eyes", "images/bathroom0/eyes" },
			{ "junk", "images/city/junk" },
			{ "kid", "images/bathroom0/kid" },
			{ "kid_attack0", "images/player/kid_attack0" },
			{ "kid_attack1", "images/player/kid_attack1" },
			{ "kid_attack2", "images/player/kid_attack2" },
			{ "kid_attack3", "images/player/kid_attack3" },
			{ "kid_brush", "images/bathroom1/kid_brush" },
			{ "kid_idle0", "images/player/kid_idle0" },
			{ "kid_scared", "images/player/kid_scared" },
			{ "kid_walk0", "images/player/kid_walk0" },
			{ "kid_walk1", "images/player/kid_walk1" },
			{ "kid_walk2", "images/player/kid_walk2" },
			{ "kid_walk3", "images/player/kid_walk3" },
			{ "kid_walk4", "images/player/kid_walk4" },
			{ "kid_walk5", "images/player/kid_walk5" },
			{ "light", "images/bathroom0/light" },
			{ "lightbulb0", "images/bathroom0/lightbulb" },
			{ "lightbulb1", "images/bathroom1/lightbulb" },
			{ "logo", "images/logo" },
			{ "mess", "images/road/mess" },
			{ "money", "images/money" },
			{ "radio", "images/city/radio" },
			{ "road", "images/bathroom1/road" },
			{ "rock0", "images/forest/rock0" },
			{ "rock1", "images/forest/rock1" },
			{ "rock2", "images/forest/rock2" },
			{ "shine", "images/shine" },
			{ "sign", "images/city/sign" },
			{ "sky", "images/sky" },
			{ "sunshine", "images/bathroom1/sunshine" },
			{ "toilet", "images/bathroom1/toilet" },
			{ "tree0", "images/forest/tree0" },
			{ "tree1", "images/forest/tree1" },
			{ "tree2", "images/forest/tree2" },
			{ "trunk", "images/forest/trunk" },
			{ "wallSide", "images/wallSide" },
			{ "white", "images/white" },
			{ "yellow", "images/yellow" },
		};

		public static readonly Dictionary<string, string> Sounds = new Dictionary<string, string>
		{
			{ "Bathroom0", "sounds/Bathroom0" },
			{ "Bathroom1", "sounds/Bathroom1" },
			{ "City", "sounds/City" },
			{ "Dialogue", "sounds/Dialogue" },
			{ "Forest", "sounds/Forest" },
			{ "HitStrong0", "sounds/HitStrong0" },
			{ "HitStrong1", "sounds/HitStrong1" },
			{ "HitWeak0", "sounds/HitWeak0" },
			{ "HitWeak1", "sounds/HitWeak1" },
			{ "HitWeak2", "sounds/HitWeak2" },
			{ "Outside", "sounds/Outside" },
			{ "Skyscraper", "sounds/Skyscraper" },
			{ "Victory", "sounds/Victory" },
		};
	}
}

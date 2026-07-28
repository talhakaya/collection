using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Golfinity
{
	public class Terrain : MonoBehaviour
	{
	    public enum Type
	    {
	        Normal,
	        Short,
	        High,
	    }
	    public enum Pack
	    {
	        Standard,
	        Varied,
	        Moving,
	        Muddy
	    }
	    public enum Size
	    {
	        OneXOne,
	        ThreeXThree
	    }
	    public Type type;
	    public Pack pack;
	    public Size size;
	    public GameObject[] muds;

	    private void Start()
	    {
	        if (transform.eulerAngles.z > 45f) foreach (GameObject mud in muds) mud.SetActive(false);
	    }

	    public bool IsUnlocked(List<Pack> packs)
	    {
	        switch (pack)
	        {
	            case Pack.Standard:
	                return true;
	            default:
	                return packs.Contains(pack);
	        }
	    }
	}

}

using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class FollowObject : MonoBehaviour
	{
	    public Transform followObject;
		void Update ()
	    {
	        if (Game.level == 26)
	        {
	            transform.position = new Vector3(followObject.transform.position.x, followObject.transform.position.y, transform.position.z);
	        }
		}
	}

}

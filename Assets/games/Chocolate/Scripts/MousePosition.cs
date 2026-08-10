using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.Chocolate
{
	public class MousePosition : MonoBehaviour {

		public static float x()
		{
			return get ().x;
		}

		public static float y()
		{
			return get ().y;
		}

		public static Vector3 get()
		{
			return Camera.main.ScreenToWorldPoint (TaloketoInputManager.mousePosition) + Vector3.forward;
		}
	}
}

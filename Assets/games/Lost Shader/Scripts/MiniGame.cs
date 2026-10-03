using UnityEngine;
using System.Collections;

namespace Games.LostShader
{
	public class MiniGame : MonoBehaviour
	{
	    public const float MaxX = 12f;
	    public float enabledTimer;

		void OnEnable ()
	    {
	        transform.localPosition = new Vector3(-MaxX, 0f, transform.localPosition.z);
	        enabledTimer = 1f;
		}

		void Update ()
	    {
		    if (enabledTimer > 0f)
	        {
	            enabledTimer -= Game.dt;
	            if (enabledTimer <= 0f)
	            {
	                enabledTimer = 0f;
	                transform.localPosition = new Vector3(0f, 0f, transform.localPosition.z);
	            }
	            else
	            {
	                transform.localPosition = new Vector3(Easing.SineEaseIn(1f - enabledTimer, -MaxX, MaxX, 1f), 0f, transform.localPosition.z);
	            }
	        }
	        else if (enabledTimer < 0f)
	        {
	            enabledTimer += Game.dt;
	            if (enabledTimer >= 0f)
	            {
	                enabledTimer = 0f;
	                transform.localPosition = new Vector3(MaxX, 0f, transform.localPosition.z);
	                Destroy(gameObject);
	            }
	            else
	            {
	                transform.localPosition = new Vector3(Easing.SineEaseIn(1f + enabledTimer, 0f, MaxX, 1f), 0f, transform.localPosition.z);
	            }
	        }
	        else
	        {
	#if UNITY_EDITOR
	            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.kKey.wasPressedThisFrame)
	            {
	                end();
	            }
	#endif
	        }
		}

	    public void end()
	    {
	        // In the collection: a fix. Ending a game that was already sliding out started
	        // the slide again, so a player who kept feeding the kid never got out of it.
	        if (enabledTimer < 0f)
	        {
	            return;
	        }
	        enabledTimer = -1f;
	        CameraScript.PlanesDown();
	    }
	}
}

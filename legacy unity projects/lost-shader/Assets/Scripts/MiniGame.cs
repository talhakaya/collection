using UnityEngine;
using System.Collections;

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
            if (Input.GetKeyDown(KeyCode.K))
            {
                end();
            }
#endif
        }
	}

    public void end()
    {
        enabledTimer = -1f;
        CameraScript.PlanesDown();
    }
}

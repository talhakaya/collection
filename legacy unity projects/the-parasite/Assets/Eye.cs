using UnityEngine;
using System.Collections;

public class Eye : MonoBehaviour {

    public static bool blinkingLeft;
    public static bool blinkingRight;
    private Vector3 localScale;
    public bool isLeftEye;
    public bool realSprite;

    void Start()
    {
        localScale = transform.localScale;
    }

    void Update()
    {
        if (realSprite == Game.realSprites)
        {
            if ((isLeftEye && blinkingLeft) || (!isLeftEye && blinkingRight))
            {
                transform.localScale = new Vector3(localScale.x, localScale.y * Random.Range(0.1f, 0.3f), localScale.z);
            }
            else
            {
                transform.localScale = localScale;
            }
        }
        else
        {
            transform.localScale = Vector3.zero;
        }
    }
}

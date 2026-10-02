using UnityEngine;
using System.Collections;

public class ColorPicker : MonoBehaviour
{
    public int colorId;

    void OnValidate()
    {
        setColor();
    }

	void Start ()
    {
        setColor();
	}

    public void setColor()
    {
        GetComponent<TintScript>().selfColor = Game.colors[colorId];
        GetComponent<SpriteRenderer>().color = Game.colors[colorId];
    }
}

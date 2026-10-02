using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorInputField : MonoBehaviour
{
    private Image image;
    private InputField inputField;
    public int colorId;
    public bool isSkyColor;

	void Start ()
    {
        inputField = GetComponent<InputField>();
        image = transform.parent.GetComponent<Image>();
        if (isSkyColor)
        {
            inputField.text = ToHex(LevelGenerator.skyColors[colorId]);
        }
        else
        {
            inputField.text = ToHex(LevelGenerator.terrainColors[colorId]);
        }
	}
	
	void Update ()
    {
        image.color = FromHex(inputField.text);
        if (isSkyColor)
        {
            LevelGenerator.skyColors[colorId] = image.color;
        }
        else
        {
            LevelGenerator.terrainColors[colorId] = image.color;
        }
	}

    public static Color FromHex(string hex)
    {
        if (hex.StartsWith("#"))
            hex = hex.Substring(1);

        if (hex.Length < 6)
        {
            return Color.black;
        }

        return new Color(
            int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f,
            int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f,
            int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f);
    }

    public static string ToHex(Color c)
    {
        return "#" + Mathf.RoundToInt(c.r * 255).ToString("X") + Mathf.RoundToInt(c.g * 255).ToString("X") + Mathf.RoundToInt(c.b * 255).ToString("X");
    }
}

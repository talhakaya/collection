using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorPicker : MonoBehaviour
{
    public static ColorPicker instance;
    public InputField inputField;

    void Awake()
    {
        instance = this;
    }

    void OnDisable()
    {
        instance = this;
    }
	
	void Update ()
    {
        inputField.text = LevelGenerator.instance.newColors();
	}

    public void pressSave()
    {
        for (int i = 0; i < LevelGenerator.skyColors.Length; i++)
        {
            PlayerPrefs.SetString("skyColor" + i, ColorInputField.ToHex(LevelGenerator.skyColors[i]));
            PlayerPrefs.SetString("terrainColor" + i, ColorInputField.ToHex(LevelGenerator.terrainColors[i]));
        }
    }

    public void pressLoad()
    {
        if (PlayerPrefs.GetString("skyColor0", "hehehe") == "hehehe")
        {
            pressSave();
        }
        else
        {
            for (int i = 0; i < LevelGenerator.skyColors.Length; i++)
            {
                LevelGenerator.skyColors[i] = ColorInputField.FromHex(PlayerPrefs.GetString("skyColor" + i));
                LevelGenerator.terrainColors[i] = ColorInputField.FromHex(PlayerPrefs.GetString("terrainColor" + i));
            }
        }
    }

    public void pressCopy()
    {
        GUIUtility.systemCopyBuffer = inputField.text;
    }

    public void pressClose()
    {
        gameObject.SetActive(false);
    }
}

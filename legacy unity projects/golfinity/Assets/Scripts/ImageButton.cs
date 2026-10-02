using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageButton : MonoBehaviour
{
    public bool pressed;
    public bool pressing;
    private bool mouseOver;
    public bool interactable;
    private Image image;
    public float alpha = 1f;
    private Color cNotInteractable;
    private Color cPressing;
    private Color cOver;
    private RectTransform rect;
    public string textEN;
    public string textTR;
    private Text text;
    private float buttonPressedTime;

    void OnDisable()
    {
        mouseOver = false;
        pressed = false;
        pressing = false;
    }

    void Start()
    {
        cNotInteractable = new Color(0.5f, 0.5f, 0.5f, alpha * 0.5f);
        cPressing = new Color(0.3f, 0.3f, 0.3f, alpha);
        cOver = new Color(0.6f, 0.6f, 0.6f, alpha);
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        text = GetComponentInChildren<Text>();
        if (Game.lang == Lang.EN)
        {
            text.text = textEN;
        }
        else
        {
            text.text = textTR;
        }
    }

    void Update()
    {
        if (buttonPressedTime > 0f)
        {
            buttonPressedTime -= Game.dt;
        }
        if (Game.lang == Lang.EN)
        {
            text.text = textEN;
        }
        else
        {
            text.text = textTR;
        }
#if !UNITY_EDITOR && UNITY_ANDROID
        mouseOver = false;
        foreach (Touch touch in Input.touches)
        {
            if (!mouseOver)
            {
                mouseOver = isOnUIElement(touch.position.x, touch.position.y, rect);
            }
        }
#else
        mouseOver = isOnUIElement(Input.mousePosition.x, Input.mousePosition.y, rect);
#endif
        pressed = interactable && mouseOver && Input.GetMouseButtonDown(0);
        if (pressed && Game.soundOn)
        {
            GetComponent<AudioSource>().Play();
        }
#if !UNITY_EDITOR && UNITY_ANDROID
        if (pressed)
        {
            buttonPressedTime = 0.5f;
        }
#endif
        pressing = interactable && mouseOver && Input.GetMouseButton(0);
        if (!interactable)
        {
            image.color = cNotInteractable;
        }
        else if (pressing)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            if (buttonPressedTime > 0)
            {
                image.color = cPressing;
            }
            else
            {
                image.color = new Color(1f, 1f, 1f, alpha);
            }
#else
            image.color = cPressing;
#endif
        }
        else if (mouseOver)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            image.color = new Color(1f, 1f, 1f, alpha);
#else
            image.color = cOver;
#endif
        }
        else
        {
            image.color = new Color(1f, 1f, 1f, alpha);
        }
    }

    public static bool isOnUIElement(float xScreen, float yScreen, RectTransform t, float buttonScale = 1.4f)
    {
        return xScreen >= t.position.x - (t.rect.width * t.lossyScale.x / 2f) * buttonScale
            && xScreen <= t.position.x + (t.rect.width * t.lossyScale.x / 2f) * buttonScale
            && yScreen >= t.position.y - (t.rect.height * t.lossyScale.y / 2f) * buttonScale
            && yScreen <= t.position.y + (t.rect.height * t.lossyScale.y / 2f) * buttonScale;
    }
}

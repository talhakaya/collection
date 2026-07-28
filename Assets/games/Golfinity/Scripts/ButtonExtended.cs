using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Games.Golfinity
{
	public class ButtonExtended : MonoBehaviour {
	    public Image icon;
	    public TextMeshProUGUI text;
	    public string textKey;
	    public AudioClip audioClip;
	    [HideInInspector] public RectTransform rectTransform;
	    [HideInInspector] public Button button;
	    [Space]
	    [Header("Icon Color")]
	    public bool colorIcon;
	    private bool _colorIconFlashing;
	    public bool colorIconFlashing
	    {
	        get
	        {
	            return _colorIconFlashing;
	        }
	        set
	        {
	            _colorIconFlashing = value;
	            if (!value) icon.color = iconColorDefault;
	        }
	    }
	    public Color iconColorDefault;
	    public Color iconColorFlash0;
	    public Color iconColorFlash1;

	    private void Awake()
	    {
	        rectTransform = GetComponent<RectTransform>();
	        button = GetComponent<Button>();
	    }

	    void Start() {
	        if (!string.IsNullOrEmpty(textKey)) {
	            text.text = Local.Get(textKey);
	            Local.OnLanguageChange += OnLanguageChange;
	        }
	    }

	    private void Update()
	    {
	        if (colorIcon && colorIconFlashing)
	        {
	            float p = Time.realtimeSinceStartup % 1f;
	            if (p <= 0.1f) icon.color = iconColorFlash0;
	            else if (p <= 0.5f)
	            {
	                float animRatio = (p - 0.1f) / 0.4f;
	                icon.color = new Color(Mathf.Lerp(iconColorFlash0.r, iconColorFlash1.r, animRatio), Mathf.Lerp(iconColorFlash0.g, iconColorFlash1.g, animRatio), Mathf.Lerp(iconColorFlash0.b, iconColorFlash1.b, animRatio), Mathf.Lerp(iconColorFlash0.a, iconColorFlash1.a, animRatio));
	            }
	            else if (p <= 0.6f) icon.color = iconColorFlash1;
	            else
	            {
	                float animRatio = (p - 0.6f) / 0.4f;
	                icon.color = new Color(Mathf.Lerp(iconColorFlash1.r, iconColorFlash0.r, animRatio), Mathf.Lerp(iconColorFlash1.g, iconColorFlash0.g, animRatio), Mathf.Lerp(iconColorFlash1.b, iconColorFlash0.b, animRatio), Mathf.Lerp(iconColorFlash1.a, iconColorFlash0.a, animRatio));
	            }
	        }
	    }

	    private void OnDestroy() {
	        if (!string.IsNullOrEmpty(textKey)) {
	            Local.OnLanguageChange -= OnLanguageChange;
	        }
	    }

	    private void OnLanguageChange() {
	        if (!string.IsNullOrEmpty(textKey)) text.text = Local.Get(textKey);
	    }

	    public void OnClick() {
	        if (Game.soundOn) AudioSource.PlayClipAtPoint(audioClip, Game.instance.transform.position);
	    }
	}

}

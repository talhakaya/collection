using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Games.Golfinity
{
	public class Popup : MonoBehaviour
	{
	    public float fadeTime = 0.2f;
	    public CanvasGroup canvasGroup;
	    private Coroutine fade;

	#if UNITY_EDITOR
	    void OnValidate()
	    {
	        canvasGroup = GetComponent<CanvasGroup>();
	    }
	#endif

	    protected virtual void Awake()
	    {
	        canvasGroup.alpha = 0f;
	    }

	    public virtual void Show()
	    {
	        gameObject.SetActive(true);
	        canvasGroup.interactable = true;
	        if (fade != null) StopCoroutine(fade);
	        fade = StartCoroutine(FadeIn());
	    }

	    public virtual void Hide()
	    {
	        canvasGroup.interactable = false;
	        if (fade != null) StopCoroutine(fade);
	        fade = StartCoroutine(FadeOut());
	    }

	    IEnumerator FadeIn()
	    {
	        while (canvasGroup.alpha < 1f)
	        {
	            canvasGroup.alpha += Time.deltaTime / fadeTime;
	            yield return null;
	        }
	        fade = null;
	    }

	    IEnumerator FadeOut()
	    {
	        while (canvasGroup.alpha > 0f)
	        {
	            canvasGroup.alpha -= Time.deltaTime / fadeTime;
	            yield return null;
	        }
	        gameObject.SetActive(false);
	        fade = null;
	    }
	}

}

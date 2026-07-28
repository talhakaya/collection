using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Games.Golfinity
{
	public class TextLang : MonoBehaviour
	{
	    private TextMeshProUGUI text;
	    public string textKey;

		void Start ()
	    {
	        text = GetComponent<TextMeshProUGUI>();
	        if (!string.IsNullOrEmpty(textKey)) text.text = Local.Get(textKey);
	        Local.OnLanguageChange += OnLanguageChange;
	    }

	    private void OnDestroy() {
	        Local.OnLanguageChange -= OnLanguageChange;
	    }

	    private void OnLanguageChange() {
	        if (!string.IsNullOrEmpty(textKey)) text.text = Local.Get(textKey);
	    }
	}

}

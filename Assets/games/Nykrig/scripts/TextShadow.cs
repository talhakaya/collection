using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Games.Nykrig
{
	public class TextShadow : MonoBehaviour {
	    // In the collection: any UI graphic rather than a UI Text, so that the texts that became
	    // TextMesh Pro texts (for button prompts) keep their shadow.
	    private Graphic text;
	    private RectTransform rectTransform;
	    
	    void Start () {
	        text = GetComponent<Graphic>();
	        rectTransform = GetComponent<RectTransform>();
	    }
		
		void Update () {
	        rectTransform.anchoredPosition = new Vector2(Game.shadowVector.x, Game.shadowVector.y) * 8f;
	        text.color = new Color(text.color.r, text.color.g, text.color.b, Geometry.lengthOfVector3(Game.shadowVector));
	    }
	}

}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Games.LetsNeverDoThatAgain
{
	public class Textt : MonoBehaviour {

	    public static Textt instance;
	    public string text;
	    // In the collection: a TextMesh Pro text (it was a UI Text), so that the {tokens} in the
	    // lines - button prompts, see InputPrompts - can be drawn as the collection's glyphs.
	    public TMPro.TMP_Text textObject;
	    private float period = 1f;

	    void Awake()
	    {
	        instance = this;
	    }

		void Start ()
	    {
	        showText();
		}

		void Update ()
	    {
	        showText();
	        if (period > 0f)
	        {
	            period -= Game.dt;
	            textObject.transform.rotation = Quaternion.identity;
	            textObject.transform.Rotate(Vector3.forward * Random.Range(-10f, 10f) * period);
	            textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 1f - period);
	        }
	        else if (period > -2f)
	        {
	            period -= Game.dt;
	            textObject.transform.rotation = Quaternion.identity;
	            textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 1f);
	        }
	        else if (period > -2.5f)
	        {
	            period -= Game.dt;
	            textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 2 * (period + 2.5f));
	        }
	        else
	        {
	            textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 0f);
	        }
		}

	    private string shownFor;
	    private bool animatedPrompt;
	    private bool shownBlink;

	    // In the collection: formatted only when the line or the device changes, or when a
	    // prompt that swaps sprites is due to swap.
	    private void showText()
	    {
	        bool blink = Collection.Controls.InputPrompts.Blink;
	        string key = text + "|" + Collection.Controls.InputPrompts.Scheme + "|" + Collection.Controls.InputPrompts.PadKind;
	        if (key == shownFor && !(animatedPrompt && blink != shownBlink))
	        {
	            return;
	        }

	        shownFor = key;
	        shownBlink = blink;
	        textObject.spriteAsset = Collection.Controls.InputPrompts.SpriteAsset();
	        textObject.text = Collection.Controls.InputPrompts.Format(text, out animatedPrompt);
	    }

	    public static void updateText(string text)
	    {
	        instance.period = 1f;
	        instance.text = text;
	        Game.instance.aText.pitch = Random.Range(0.9f, 1.1f);
	        Game.instance.aText.volume = 0.5f;
	        Game.instance.aText.Play();
	    }
	}
}

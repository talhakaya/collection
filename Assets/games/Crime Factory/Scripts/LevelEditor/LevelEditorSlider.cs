using UnityEngine;
using UnityEngine.UI;

namespace Games.CrimeFactory
{
	public class LevelEditorSlider : MonoBehaviour
	{
	    public Text label;
	    public Slider slider;

	    public void OnValueChanged() {
	        if (Mathf.Round(slider.value) == slider.value) {
	            label.text = string.Format("{0}: {1}", label.text.Split(':')[0], Mathf.RoundToInt(slider.value));
	        }
	        else {
	            label.text = string.Format("{0}: {1}", label.text.Split(':')[0], slider.value.ToString("F2"));
	        }
	    }
	}
}

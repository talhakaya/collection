using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Games.Nykrig
{
	public class MarbleNoise : MonoBehaviour {
	    public Sprite[] sprites;
	    private Image image;
	    private bool scaleXUp;
	    private bool scaleYUp;
	    private float timer;
	    public float period = 0.3f;
	    public float scale = 0.3f;
	    public float alphaMax = 0.4f;

	    void Start () {
	        image = GetComponent<Image>();
	        SetNoise();
	    }
		
		void Update () {
	        transform.localScale += scale * Game.dt * new Vector3(scaleXUp ? 1f : -1f, scaleYUp ? 1f : -1f, 0f);
	        timer += Game.dt;
	        if (timer >= period) {
	            timer = 0f;
	            SetNoise();
	        }
	    }

	    void SetNoise() {
	        transform.localScale = new Vector3(1f, 1f, 1f) * Random.Range(1f, 1.5f);
	        image.color = new Color(1f, 1f, 1f, Random.Range(0f, alphaMax));
	        if (Random.value < 0.5f) {
	            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
	        }
	        if (Random.value < 0.5f) {
	            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
	        }
	        scaleXUp = !(transform.localScale.x > 1.25f || transform.localScale.x < -1.25f);
	        scaleYUp = !(transform.localScale.y > 1.25f || transform.localScale.y < -1.25f);
	        image.sprite = sprites[Random.Range(0, sprites.Length)];
	    }
	}

}

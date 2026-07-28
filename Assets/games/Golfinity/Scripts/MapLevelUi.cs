using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Golfinity
{
	public class MapLevelUi : MapUi
	{
	    public bool interactable;
	    public Color enabledColor;
	    public Color disabledColor;
	    public SpriteRenderer whiteDot;
	    public Animator animator;
	    public GameObject[] stars;
	    public GameObject[] starsDisabled;
	    public TMPro.TextMeshPro text;
	    public int holeNo;
	    public bool isOver;
	    public GameObject hand;
	    public AudioClip audioClip;


	    private void Enable()
	    {
	        isOver = false;
	        animator.speed = UnityEngine.Random.Range(0.8f, 1.2f);
	    }

	    public void Appear()
	    {
	        animator.Play("Appear");
	    }

	    public void SetValues(int holeNo, int numStars, Vector3 position, bool interactable)
	    {
	        if (holeNo < 0)
	        {
	            gameObject.SetActive(false);
	            return;
	        }
	        gameObject.SetActive(true);
	        name = $"mapLevelUi ({holeNo})";
	        this.holeNo = holeNo;
	        transform.localPosition = new Vector3(position.x, position.y, transform.localPosition.z);
	        text.text = (holeNo + 1).ToString();
	        for (int i = 0; i < 3; i++)
	        {
	            GameObject star = stars[i];
	            GameObject starDisabled = starsDisabled[i];
	            if (numStars == 0)
	            {
	                star.SetActive(false);
	            }
	            else if (i < numStars)
	            {
	                star.SetActive(true);
	                starDisabled.SetActive(false);
	            }
	            else
	            {
	                star.SetActive(true);
	                starDisabled.SetActive(true);
	            }
	        }
	        this.interactable = interactable;
	        whiteDot.color = interactable ? enabledColor : disabledColor;
	        text.color = interactable ? enabledColor : disabledColor;
	        hand.SetActive(numStars == 0 && interactable);
	    }

	    public override void OnOver(bool isOver)
	    {
	        if (this.isOver == isOver) return;
	        this.isOver = isOver;
	        if (isOver) animator.SetTrigger("In");
	        else animator.SetTrigger("Out");
	    }

	    public override void OnClick()
	    {
	        if (Game.soundOn) AudioSource.PlayClipAtPoint(audioClip, Game.instance.transform.position);
	        animator.SetTrigger("Click");
	        Invoke(nameof(Play), 0.2f);
	    }

	    private void Play()
	    {
	        Game.instance.SetState(GameState.Play);
	        Game.OpenLevel(holeNo);
	    }
	}

}

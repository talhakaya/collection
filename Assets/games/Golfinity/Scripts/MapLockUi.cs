using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Golfinity
{
	public class MapLockUi : MapUi
	{
	    public bool interactable;
	    public Color enabledColor;
	    public Color disabledColor;
	    public Animator animator;
	    public TMPro.TextMeshPro text;
	    public GameObject barParent;
	    public Transform barScale;
	    public SpriteRenderer[] spritesToColor;
	    public GameObject hand;
	    public AudioSource audioUnlock;
	    public AudioClip audioUnlocking;
	    public AudioClip audioUnlocked;
	    [HideInInspector] public bool isPaying;
	    private int lockIndex;
	    private int numGoldLeft;
	    private int numGoldTotal;

	    public void Init(bool showValues, bool interactable, int lockIndex, int numGoldLeft, int numGoldTotal)
	    {
	        this.lockIndex = lockIndex;
	        this.numGoldLeft = numGoldLeft;
	        this.numGoldTotal = numGoldTotal;
	        this.interactable = interactable;
	        animator.Play("Locked");
	        barParent.SetActive(showValues);
	        text.gameObject.SetActive(showValues);
	        hand.SetActive(interactable);
	        text.text = numGoldLeft.ToString();
	        float scale = 1f * (numGoldTotal - numGoldLeft) / numGoldTotal;
	        barScale.localScale = new Vector3(scale, 1f, 1f);
	        if (interactable) foreach (var sr in spritesToColor) sr.color = enabledColor;
	        else foreach (var sr in spritesToColor) sr.color = disabledColor;
	        audioUnlock.volume = Game.soundOn ? 1f : 0f;
	        audioUnlock.pitch = 1f + scale * 2f;
	    }

	    private void Update()
	    {
	        if (isPaying)
	        {
	            if (Game.instance.level.PayGoldToUnlock(lockIndex))
	            {
	                Init(true, true, lockIndex, numGoldLeft - 1, numGoldTotal);
	                hand.SetActive(false);
	            }
	            else
	            {
	                isPaying = false;
	                audioUnlock.Stop();
	                interactable = false;
	                if (Game.instance.level.GetNumGoldToUnlock(lockIndex) == 0)
	                {
	                    animator.Play("Unlocked");
	                    if (Game.soundOn) AudioSource.PlayClipAtPoint(audioUnlocking, Game.instance.transform.position);
	                    Invoke(nameof(PlayAudioUnlocked), 1.83f);
	                    Invoke(nameof(OnUnlocked), 2.67f);
	                }
	                else foreach (var sr in spritesToColor) sr.color = disabledColor;
	            }
	        }
	    }

	    private void PlayAudioUnlocked()
	    {
	        if (Game.soundOn) AudioSource.PlayClipAtPoint(audioUnlocked, Game.instance.transform.position);
	    }

	    private void OnUnlocked()
	    {
	        Game.instance.map.Refresh(true);
	    }

	    public override void OnOver(bool isOver)
	    {
	        
	    }

	    public override void OnClick()
	    {
	        isPaying = true;
	        audioUnlock.Play();
	    }
	}

}

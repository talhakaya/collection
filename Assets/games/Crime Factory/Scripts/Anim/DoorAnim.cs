using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public class DoorAnim : AnimBehavior {
	    public Sprite[] anim;
	    public Sprite[] animClosed;
	    private PhysBox physBox;
	    private Vector2 visualPosOffset;
	    [HideInInspector] public Vector2 localScale;
	    public float timer;
	    public bool isOpen;

	    private new void Start() {
	        sprites = anim;
	        period = 0.2f;
	        physBox = GetComponent<PhysBox>();
	        visualPosOffset = physBox.visualPosOffset;
	        localScale = transform.localScale;
	        base.Start();
	    }

	    private new void Update() {
	        base.Update();
	        if (Simulator.IsPaused) return;
	        sprites = isOpen ? anim : animClosed;
	        if (timer <= 0f) {
	            transform.localScale = new Vector3(localScale.x, localScale.y, 1f);
	            physBox.visualPosOffset = visualPosOffset;
	        }
	        else if (timer < 0.25f) {
	            physBox.visualPosOffset = new Vector2(visualPosOffset.x + Random.Range(-0.1f, 0.1f), visualPosOffset.y + Random.Range(-0.1f, 0.1f));
	        }
	        else if (timer < 0.5f) {
	            float animRatio = (0.5f - timer) * 4f;
	            transform.localScale = new Vector3(localScale.x, localScale.y * animRatio, 1f);
	            physBox.visualPosOffset = visualPosOffset;
	        }
	        else {
	            transform.localScale = new Vector3(localScale.x, 0f, 1f);
	            physBox.visualPosOffset = visualPosOffset;
	        }
	    }
	}
}

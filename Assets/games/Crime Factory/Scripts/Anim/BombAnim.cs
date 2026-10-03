using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public class BombAnim : AnimBehavior {
	    public Sprite[] anim;
	    public float[] angles;
	    public float timer;

	    private void OnEnable() {
	        timer = 4f;
	    }

	    private new void Start() {
	        sprites = anim;
	        period = 0.2f;
	        base.Start();
	    }

	    private new void Update() {
	        base.Update();
	        if (Simulator.IsPaused) return;
	        if (angles.Length > 0) {
	            int i = Mathf.FloorToInt((animTimer % (period * angles.Length)) / period);
	            transform.localEulerAngles = new Vector3(0f, 0f, angles[i]);
	            animTimer += Time.deltaTime;
	        }
	        timer -= Time.deltaTime;
	        if (timer > 2f) {
	            period = 0.5f;
	        }
	        else if (timer > 1f) {
	            period = 0.25f;
	        }
	        else if (timer > 0f) {
	            period = 0.1f;
	        }
	        else {
	            Vector3 pos = transform.position;
	            ObjectPool.explosionPool.getPhys(pos + new Vector3(0f, 0f, 0f));
	            ObjectPool.explosionPool.getPhys(pos + new Vector3(1f, 0f, 0f));
	            ObjectPool.explosionPool.getPhys(pos + new Vector3(2f, 0f, 0f));
	            ObjectPool.explosionPool.getPhys(pos + new Vector3(-1f, 0f, 0f));
	            ObjectPool.explosionPool.getPhys(pos + new Vector3(-2f, 0f, 0f));
	            ObjectPool.explosionPool.getPhys(pos + new Vector3(0f, 1f, 0f));
	            ObjectPool.explosionPool.getPhys(pos + new Vector3(1f, 1f, 0f));
	            ObjectPool.explosionPool.getPhys(pos + new Vector3(-1f, 1f, 0f));
	            ObjectPool.explosionPool.getPhys(pos + new Vector3(0f, -1f, 0f));
	            ObjectPool.explosionPool.getPhys(pos + new Vector3(1f, -1f, 0f));
	            ObjectPool.explosionPool.getPhys(pos + new Vector3(-1f, -1f, 0f));
	            ObjectPool.explosionPool.getPhys(pos + new Vector3(0f, 2f, 0f));
	            ObjectPool.explosionPool.getPhys(pos + new Vector3(0f, -2f, 0f));
	            AudioPlayer.instance.Play(AudioPlayer.instance.clipExplosion);
	            gameObject.SetActive(false);
	        }
	    }
	}
}

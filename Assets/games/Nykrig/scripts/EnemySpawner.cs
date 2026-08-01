using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class EnemySpawner : MonoBehaviour {
	    private float timer;
	    public float period = 2f;
	    private ObjectPool objectPool;
	    private bool isEnemy;

	    void Start () {
	        timer = 0f;
	        objectPool = GetComponent<ObjectPool>();
	        isEnemy = objectPool.objectPrefab.GetComponent<Enemy>() != null;
	    }
		
		void Update () {
	        if (Game.instance.endless && !Game.instance.menu.gameObject.activeSelf && !Game.instance.result.activeSelf) {
	            timer += Game.dt;
	            if (timer >= period * (isEnemy ? Mathf.Clamp((100f - Game.endlessTimer) / 100f, 0.2f, 1f) : 1f)) {
	                timer = 0f;
	                GameObject e = objectPool.get(transform.position);
	                e.transform.position = new Vector3(Random.Range(Game.levelMinX, Game.levelMaxX), Random.Range(Game.levelMinY, Game.levelMaxY), 0f);
	                while (Geometry.lengthOfVector3(e.transform.position - Game.instance.transform.position) < 4f) {
	                    e.transform.position = new Vector3(Random.Range(Game.levelMinX, Game.levelMaxX), Random.Range(Game.levelMinY, Game.levelMaxY), 0f);
	                }
	                if (isEnemy) {
	                    e.GetComponent<Enemy>().Set();
	                    TalhaAudioSource.PlayInstance(AudioResources.instance.spring, AudioType.Effect);
	                }
	            }
	        }
	        else {
	            timer = 0f;
	        }
	    }
	}

}

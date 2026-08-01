using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class Spawn : MonoBehaviour {
	    public ObjectPool.PoolType poolType;
	    public GameObject go;

	    public void Create() {
	        ObjectPool objectPool = null;

	        if (poolType == ObjectPool.PoolType.Fire) {
	            objectPool = ObjectPool.firePool;
	        }
	        else if (poolType == ObjectPool.PoolType.Light) {
	            objectPool = ObjectPool.lightPool;
	        }
	        else if (poolType == ObjectPool.PoolType.Audio) {
	            objectPool = ObjectPool.audioPool;
	        }
	        else if (poolType == ObjectPool.PoolType.EnemyScale) {
	            Game.enemyCount++;
	            objectPool = ObjectPool.enemyScalePool;
	        }
	        else if (poolType == ObjectPool.PoolType.EnemySimple) {
	            Game.enemyCount++;
	            objectPool = ObjectPool.enemySimplePool;
	        }
	        else if (poolType == ObjectPool.PoolType.EnemyPortal) {
	            Game.enemyCount++;
	            objectPool = ObjectPool.enemyPortalPool;
	        }
	        else if (poolType == ObjectPool.PoolType.EnemySimpleSmall) {
	            objectPool = ObjectPool.enemySimpleSmallPool;
	        }
	        else if (poolType == ObjectPool.PoolType.LightPermanent) {
	            objectPool = ObjectPool.lightPermanentPool;
	        }
	        else if (poolType == ObjectPool.PoolType.EnemyPlus) {
	            Game.enemyCount++;
	            objectPool = ObjectPool.enemyPlusPool;
	        }
	        else if (poolType == ObjectPool.PoolType.EnemyX) {
	            Game.enemyCount++;
	            objectPool = ObjectPool.enemyXPool;
	        }
	        else if (poolType == ObjectPool.PoolType.NightPill) {
	            objectPool = ObjectPool.nightPillPool;
	        }
	        else if (poolType == ObjectPool.PoolType.NextLevel) {
	            Game.enemyCount++;
	            objectPool = ObjectPool.nextLevelPool;
	        }
	        else if (poolType == ObjectPool.PoolType.Boss) {
	            Game.enemyCount = System.Int32.MaxValue;
	            objectPool = ObjectPool.bossPool;
	        }
	        if (objectPool != null) {
	            go = objectPool.get(transform.position);
	            if (go.GetComponent<Enemy>() != null) {
	                go.GetComponent<Enemy>().Set();
	            }
	            if (go.GetComponent<Lightt>() != null) {
	                go.GetComponent<Lightt>().Set(transform.localScale.x, false, 0f);
	            }
	            else {
	                go.transform.localScale = new Vector3(go.transform.localScale.x * transform.localScale.x, go.transform.localScale.y * transform.localScale.y, 1f);
	            }
	        }
		}
	}

}

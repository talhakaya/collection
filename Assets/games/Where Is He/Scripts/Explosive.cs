using UnityEngine;
using System.Collections;

namespace Games.WhereIsHe
{
	public class Explosive : MonoBehaviour
	{
	    private Enemy enemy;

		void Start ()
	    {
	        enemy = GetComponent<Enemy>();
		}

	    public void explode()
	    {
	        if (enemy == null)
	        {
	            Explosion.create(gameObject);
	            Game.screenShakeSmall();
	            Destroy(gameObject);
	        }
	        else
	        {
	            Explosion.create(gameObject);
	            gameObject.SetActive(false);
	            Game.screenShakeSmall();
	        }
	    }
	}
}

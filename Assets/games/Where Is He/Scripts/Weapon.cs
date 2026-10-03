using UnityEngine;
using System.Collections;

namespace Games.WhereIsHe
{
	public class Weapon : MonoBehaviour
	{
	    public bool pickedUp;
	    private PlayerScript player;
	    public GameObject[] enableWhenPickedUp;
	    public float period = 0.2f;
	    public float lineHeightChange = 0.2f;
	    public float lineRGBChange = 0.3f;
	    public float bulletSpeed = 100f;
	    public GameObject bulletPrefab;
	    public Vector3 bulletSpawnLocation;
	    public AudioClip audio;

		void Start ()
	    {
		    for (int i = 0; i < enableWhenPickedUp.Length; i++)
	        {
	            enableWhenPickedUp[i].SetActive(false);
	        }
		}

		public void shoot (float angle)
	    {
	        LineManager.Sheight += lineHeightChange;
	        LineManager.SrgbSplit += lineRGBChange;
	        Rigidbody2D b = (Instantiate(bulletPrefab, transform.TransformPoint(bulletSpawnLocation), Quaternion.identity) as GameObject).GetComponent<Rigidbody2D>();
	        b.GetComponent<Bullet>().vel = Geometry.createVector3(angle, bulletSpeed);
	        b.AddTorque(Random.Range(-90f, 90f));
	        Game.screenShakeSmall();
		}

	    void OnTriggerEnter2D(Collider2D other)
	    {
	        if (other.tag == "Player")
	        {
	            if (!pickedUp)
	            {
	                AudioSource.PlayClipAtPoint(audio, Camera.main.transform.position);
	                for (int i = 0; i < enableWhenPickedUp.Length; i++)
	                {
	                    enableWhenPickedUp[i].SetActive(true);
	                }
	                player = other.GetComponent<PlayerScript>();
	                player.getWeapon(this);
	                pickedUp = true;
	            }
	        }
	    }
	}
}

using UnityEngine;
using System.Collections;

namespace Games.Gridsplosion
{
	public enum WeaponType
	{
	    None,
	    Pistol,
	    Uzi,
	    MachineGun
	}

	public class Weapon : MonoBehaviour
	{
	    public WeaponType type;
	    public Sprite[] sprites;
	    public Bullet[] bullets;
	    public Color[] bulletExplosions;
	    public bool[] continuingClicks;
	    public float[] periods;
	    private SpriteRenderer sprite;
	    public Bullet bullet;
	    private float period;
	    private Color bulletExplosion;
	    private bool continuingClick;
	    private float timer;

		void Start ()
	    {
	        sprite = GetComponent<SpriteRenderer>();
	        changeWeapon(type);
		}

		void Update ()
	    {
		    if (timer < period)
	        {
	            timer += Game.dt;
	        }
	        else if (Game.inputDown || (Game.input && continuingClick))
	        {
	            timer -= period;
	            if (bullet != null)
	            {
	                Instantiate(bullet, transform.position, transform.rotation);
	                Game.explosion(new ExplosionData(transform.position, bulletExplosion, 5f, period * 2f, 2f));
	                CameraScript.zoom();
	            }
	        }
		}

	    public void changeWeapon(WeaponType _type)
	    {
	        type = _type;
	        sprite.enabled = true;
	        switch (type)
	        {
	            case WeaponType.None:
	                sprite.enabled = false;
	                break;
	        }
	        bullet = bullets[(int)type];
	        sprite.sprite = sprites[(int)type];
	        period = periods[(int)type];
	        timer = period;
	        bulletExplosion = bulletExplosions[(int)type];
	        continuingClick = continuingClicks[(int)type];
	        if (type != WeaponType.None)
	        {
	            //make change weapon sound
	        }
	    }
	}
}

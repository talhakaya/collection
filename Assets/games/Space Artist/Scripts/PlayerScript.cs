using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class PlayerScript : MonoBehaviour
	{
	    public static PlayerScript instance;
	    public float maxSpeed;
	    public float acceleration;
	    public float rotateSpeed;
	    public Rigidbody2D body;
	    public Transform particleCreate;
	    private float particleTimer;
	    private AudioSource audioSource;
	    public float jetpackVolume = 0.8f;
	    public float jetpackAddPitch = 0.8f;
	    public float resetTimer;
	    private float resetPeriod = 1f;
	    private const float MaxResetPeriod = 1f;
	    private Vector3 resetPositionTo;
	    private Vector3 resetPositionFrom;
	    private float resetRotationTo;
	    private float resetRotationFrom;

		void Start ()
	    {
	        instance = this;
	        body = GetComponent<Rigidbody2D>();
	        audioSource = GetComponent<AudioSource>();
	        resetPositionTo = transform.position;
	        resetRotationTo = transform.eulerAngles.z;
		}
		
		void Update ()
	    {
	        if (resetTimer > 0f)
	        {
	            resetTimer -= Game.dt;
	            float x = Easing.CircEaseOut(resetPeriod - resetTimer, resetPositionFrom.x, resetPositionTo.x - resetPositionFrom.x, resetPeriod);
	            float y = Easing.CircEaseOut(resetPeriod - resetTimer, resetPositionFrom.y, resetPositionTo.y - resetPositionFrom.y, resetPeriod);
	            float z = Easing.CircEaseOut(resetPeriod - resetTimer, resetPositionFrom.z, resetPositionTo.z - resetPositionFrom.z, resetPeriod);
	            float r = Easing.CircEaseOut(resetPeriod - resetTimer, resetRotationFrom, resetRotationTo - resetRotationFrom, resetPeriod);
	            transform.position = new Vector3(x, y, z);
	            transform.eulerAngles = Vector3.forward * r;
	            body.linearVelocity = Vector2.zero;
	            body.angularVelocity = 0f;
	            jetpackVolumeDown();
	        }
		    else if (Game.input)
	        {
	            body.AddForce(Geometry.normalizeVector3(MousePosition.get - transform.position, acceleration * Game.dt));
	            if (Geometry.lengthOfVector2(body.linearVelocity) > maxSpeed)
	            {
	                body.linearVelocity = Geometry.normalizeVector2(body.linearVelocity, maxSpeed);
	            }
	            float angle = Geometry.differenceOfAnglesNegative(Geometry.angleOfVector3(MousePosition.get - transform.position), transform.eulerAngles.z + 90f);
	            body.angularVelocity = angle;

	            particleTimer += Game.dt;
	            if (particleTimer >= 0.1f)
	            {
	                particleTimer = 0f;
	                ParticlePool.get(particleCreate.position, particleCreate.eulerAngles.z, 2, 2f);
	            }
	            jetpackVolumeUp();
	        }
	        else
	        {
	            jetpackVolumeDown();
	        }
		}

	    public void Reset()
	    {
	        resetPositionFrom = transform.position;
	        resetRotationFrom = transform.eulerAngles.z;
	        resetPeriod = MaxResetPeriod * Geometry.lengthOfVector3(resetPositionFrom - resetPositionTo) / 10f;
	        resetTimer = resetPeriod;
	    }

	    void jetpackVolumeDown()
	    {
	        if (audioSource.volume > 0f)
	        {
	            audioSource.volume -= Game.dt * 3f * jetpackVolume;
	            if (audioSource.volume < 0f)
	            {
	                audioSource.volume = 0f;
	            }
	            audioSource.pitch = 1f + audioSource.volume * jetpackAddPitch / jetpackVolume;
	        }
	    }

	    void jetpackVolumeUp()
	    {
	        if (audioSource.volume < jetpackVolume)
	        {
	            audioSource.volume += Game.dt * 3f * jetpackVolume;
	            if (audioSource.volume > jetpackVolume)
	            {
	                audioSource.volume = jetpackVolume;
	            }
	            audioSource.pitch = 1f + audioSource.volume * jetpackAddPitch / jetpackVolume;
	        }
	    }
	}

}

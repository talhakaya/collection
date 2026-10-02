using UnityEngine;
using System.Collections;

public enum AnimState
{
	Idle,
	Walk
};
	
public enum SpriteAlphaState
{
	Normal,
	Flickering,
	Free
}

public class PlayerScript : MonoBehaviour
{
	public static PlayerScript instance;
	
	private bool walkInputTaken;
	private bool isLeft;
	private bool isLeftOld;
	private const float turnAnimTime = 0.25f;
	private float turnAnimCounter = turnAnimTime;
	private PlayerAnim currentAnimOld;
	private GameArcadeKind spriteTypeOld;
	private tk2dSpriteAnimator animator;
	private tk2dSprite sprite;
	
	public GameArcadeKind spriteType;
	public Vector3 walkPosition;
	public float speed;//600 maybe
	public float drag;
	public PlayerAnim currentAnim;
	public float zOffset;
	public bool canWalk;
	public SpriteAlphaState spriteAlphaState;
	public Vector3 directionForCamera;
	
	public enum PlayerAnim
	{
		Idle,
		WalkDown,
		TurnDown,
		Arcade1
	}
	
	void Awake ()
	{
		instance = this;
		animator = gameObject.GetComponent<tk2dSpriteAnimator> ();
		sprite = gameObject.GetComponent<tk2dSprite> ();
	}
	
	void Start ()
	{
		spriteType = 0;
		canWalk = true;
		ShadowScript.createShadow(transform, Vector2.one * 5, Vector2.up * (-1));
		//ArcadeGameManager.createNewArcadeGame(GameArcadeKind.Crying);
	}
	
	void Update ()
	{
		directionForCamera = Vector3.zero;
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / StaticObjectZScript.zDivider - zOffset);
		
		walkInputTaken = false;
		
		if (spriteAlphaState == SpriteAlphaState.Normal)
		{
			if (sprite.color.a != 1f)
			{
				sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
			}
		}
		else if (spriteAlphaState == SpriteAlphaState.Flickering)
		{
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Random.Range (0.2f, 0.5f));
		}
		
		/*if (Input.GetKey(KeyCode.Y))
		{
			CameraScript.Shake (90, 1f);
		}*/
		if (Input.GetKey(KeyCode.T))
		{
			CameraScript.ShakeEaseIn (90, 1f);
		}
		if (Input.GetKey(KeyCode.U))
		{
			CameraScript.ShakeEaseOut (90, 1f);
		}
		if (Input.GetKey(KeyCode.B))
		{
			ArcadeGameManager.createNewArcadeGame(GameArcadeKind.AvoidEnemies);
		}
		if (Input.GetKey(KeyCode.N))
		{
			ArcadeGameManager.createNewArcadeGame(GameArcadeKind.Crying);
		}
		if (Input.GetKey(KeyCode.M))
		{
			ArcadeGameManager.createNewArcadeGame(GameArcadeKind.SwallowPill);
		}
		if (Input.GetKey(KeyCode.I))
		{
			CameraScript.zoomInOut(0.75f, 0.1f);
		}
		if (Input.GetKey(KeyCode.O))
		{
			CameraScript.zoomInOut(1f, 0.1f);
		}
		if (Input.GetKey(KeyCode.P))
		{
			CameraScript.zoomInOut(1.5f, 0.1f);
		}
		if (Input.GetKeyDown(KeyCode.G))
		{
			CameraScript.instance.glitchEffect = !CameraScript.instance.glitchEffect;
		}
		walkPosition = transform.position;
		if (Input.GetButton("Left"))
		{
			walkInputTaken = true;
			walkPosition += Vector3.right * (-1);
		}
		if (Input.GetButton("Right"))
		{
			walkInputTaken = true;
			walkPosition += Vector3.right;
		}
		if (Input.GetButton("Down"))
		{
			walkInputTaken = true;
			walkPosition += Vector3.up * (-1);
		}
		if (Input.GetButton("Up"))
		{
			walkInputTaken = true;
			walkPosition += Vector3.up;
		}
		if (Input.GetButton("Fire"))
		{
			walkInputTaken = true;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			walkPosition = ray.origin + (ray.direction * 10f);
		}
		
		
		bool willWalk = canWalk && walkInputTaken 
			&& !(ArcadeGameManager.instance != null && ArcadeGameManager.instance.isPaused) 
				&& DialogueManager.instance == null && DialoguePreparer.instance == null;
			
		if (willWalk)
		{
			rigidbody.velocity = Vector3.zero;
			rigidbody.drag = 0;
			
			Vector3 direction = new Vector3(walkPosition.x - transform.position.x,
											walkPosition.y - transform.position.y, 0);
			float newSpeed = Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y);
			
			if (newSpeed > 0f)
			{
				float speedRatio = speed * Time.deltaTime * 60f / newSpeed;
				if (currentAnim == PlayerAnim.TurnDown)
				{
					direction.Set(direction.x / 2, direction.y, direction.z);
				}
				
				/*if (spriteType == GameArcadeState.NonArcade)
				{
					speedRatio /= 1 + Mathf.Sqrt(Mathf.Abs (direction.y) / newSpeed);
				}*/
				
				directionForCamera = direction * speedRatio;
				rigidbody.AddForce(directionForCamera);
			}
			
			isLeft = (direction.x < 0) || (direction.x == 0 && isLeftOld);
		}
		else
		{
			rigidbody.drag = drag;
		}
		
		if (currentAnim != PlayerAnim.TurnDown)
		{
			if (willWalk)
			{
				currentAnim = PlayerAnim.WalkDown;
			}
			else
			{
				currentAnim = PlayerAnim.Idle;
			}
		}
		else
		{
			turnAnimCounter += Time.deltaTime;
			if (turnAnimCounter > turnAnimTime)
			{
				currentAnim = PlayerAnim.Idle;
			}
		}
		
		if (isLeft != isLeftOld)
		{
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
			currentAnim = PlayerAnim.TurnDown;
			turnAnimCounter = 0f;
		}
		
		if (!canWalk || (ArcadeGameManager.instance != null && ArcadeGameManager.instance.isPaused))
		{
			currentAnim = PlayerAnim.Idle;
		}
		
		if (currentAnim != currentAnimOld || spriteTypeOld != spriteType)
		{
			if (spriteType == GameArcadeKind.NonArcade)
			{
				if (currentAnim == PlayerAnim.Idle)
				{
					animator.Play("idle");
				}
				else if (currentAnim == PlayerAnim.TurnDown)
				{
					animator.Play("turnDown");
				}
				else if (currentAnim == PlayerAnim.WalkDown)
				{
					animator.Play("walkDown");
				}
			}
			else if (spriteType == GameArcadeKind.AvoidEnemies)
			{
				animator.Play("arcadeIdle");
			}
		}
		
		isLeftOld = isLeft;
		currentAnimOld = currentAnim;
		spriteTypeOld = spriteType;
	}
}

using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.Orphan
{
	public class CameraScript : MonoBehaviour
	{
		public static CameraScript instance;
		public static int fadeInOutLength = 60;
		public static int fadeInOutRadius = 2;

		private int shakeCount;
		private float shakeRadius;
		private Vector3 realPosition;
		private int shakeEaseOutCount;
		private int shakeEaseInCount;
		private float shakeStateRadius;
		private int z = 0;
		private int zBound = 60;
		private float zMax = 0.001f;
		private bool zIncreasing;
		private Vector3 cameraPoint;
		private float zoomSpeed;
		private bool glitchEffectWasTrueBefore;
		private GameObject glitchParent;

		public tk2dCamera cameraTk2d;
		public float zoomFactor;
		public bool hotlineMiamiCameraEffect;
		public bool glitchEffect;
		public GameObject Foreground;
		public tk2dSprite ForegroundSprite;
		public bool followingPlayer;
		public bool obeyingBounds;
		public Vector2 minBound;
		public Vector2 maxBound;
		public float drag;
		public float speed;

		void Awake()
		{
			instance = this;

		}

		void Start ()
		{
			foreach (Transform child in transform)
			{
				if (child.name == "Camera")
				{
					cameraTk2d = child.gameObject.GetComponent<tk2dCamera>();
				}
				else if (child.name == "Foreground")
				{
					Foreground = child.gameObject;
					ForegroundSprite = Foreground.GetComponent<tk2dSprite>();
				}
			}
			if (PlayerScript.instance != null)
			{
				drag = PlayerScript.instance.drag / 20f;
			}
		}

		void Update ()
		{
			if (PlayerScript.instance != null && followingPlayer)
			{
				realPosition = new Vector3(PlayerScript.instance.transform.position.x, PlayerScript.instance.transform.position.y + 5, -10);
				Ray ray = Camera.main.ScreenPointToRay(TaloketoInputManager.mousePosition);
				Vector3 mousePosition = ray.origin + (ray.direction * 10f);
				Vector3 deltaPosition2 = mousePosition - transform.position;
				float newRadius = Mathf.Sqrt(deltaPosition2.x * deltaPosition2.x + deltaPosition2.y * deltaPosition2.y);
				if (newRadius > 24f / CameraScript.instance.cameraTk2d.ZoomFactor)
				{
					deltaPosition2 = deltaPosition2 * 24f / CameraScript.instance.cameraTk2d.ZoomFactor / newRadius;
				}
				transform.position = realPosition + new Vector3(deltaPosition2.x, deltaPosition2.y, 0f) / 4;
				/*realPosition = new Vector3(PlayerScript.instance.transform.position.x, PlayerScript.instance.transform.position.y + 5, -10);
				cameraPoint = PlayerScript.instance.transform.position + Vector3.up * 5;
				Vector3 deltaPosition2 = cameraPoint - transform.position;
				float newRadius = Mathf.Sqrt(deltaPosition2.x * deltaPosition2.x + deltaPosition2.y * deltaPosition2.y);
				if (PlayerScript.instance.directionForCamera != Vector3.zero)
				{
					cameraPoint += 0.125f * PlayerScript.instance.directionForCamera;
				}

				if (newRadius < 14f / CameraScript.instance.cameraTk2d.ZoomFactor)
				{
					speed = PlayerScript.instance.speed * 1.1f;
				}
				else if (newRadius > 21f / CameraScript.instance.cameraTk2d.ZoomFactor)
				{
					if (PlayerScript.instance.directionForCamera == Vector3.zero)
					{
						speed = PlayerScript.instance.speed * 0.8f;
					}
					else
					{
						speed = PlayerScript.instance.speed * 0.8f;
					}
				}
				else
				{
					speed = PlayerScript.instance.speed;
				}

				Vector3 deltaPosition = cameraPoint - transform.position;
				float newSpeed = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);

				if (newSpeed > 5f) //will walk
				{
					GetComponent<Rigidbody>().velocity = Vector3.zero;
					GetComponent<Rigidbody>().drag = 0;

					Vector3 direction = new Vector3(deltaPosition.x, deltaPosition.y, 0);

					float speedRatio = speed / newSpeed;

					direction = direction * speedRatio;
					GetComponent<Rigidbody>().AddForce(direction);
				}
				else
				{
					GetComponent<Rigidbody>().drag = drag;
				}*/
			}


			if (shakeCount > 0)
			{
				if (shakeEaseOutCount > 0)
				{
					shakeRadius = shakeStateRadius * shakeCount / shakeEaseOutCount;
				}
				else if (shakeEaseInCount > 0)
				{
					shakeRadius = shakeStateRadius - shakeStateRadius * shakeCount / shakeEaseInCount;
				}
				shakeCount--;
				transform.position += new Vector3(Random.Range(-shakeRadius, +shakeRadius), Random.Range(-shakeRadius, +shakeRadius), 0f);
			}
			else
			{
				//transform.position = realPosition;
			}


			if (cameraTk2d.ZoomFactor != zoomFactor)
			{
				float zoomSpeedTimed = zoomSpeed * Time.deltaTime;
				if (cameraTk2d.ZoomFactor - zoomSpeedTimed > zoomFactor)
				{
					changeZoom(cameraTk2d.ZoomFactor - zoomSpeedTimed);
				}
				else if (cameraTk2d.ZoomFactor + zoomSpeedTimed < zoomFactor)
				{
					changeZoom(cameraTk2d.ZoomFactor + zoomSpeedTimed);
				}
				else
				{
					changeZoom(zoomFactor);
				}
			}



			if (hotlineMiamiCameraEffect)
			{
				if (zIncreasing)
				{
					z++;
					if (z >= zBound)
					{
						zIncreasing = false;
					}
					transform.Rotate(new Vector3(0, 0, zMax - zMax * (zBound - Mathf.Abs (z))));
				}
				else
				{
					z--;
					if (z <= -zBound)
					{
						zIncreasing = true;
					}
					transform.Rotate(new Vector3(0, 0, -(zMax - zMax * (zBound - Mathf.Abs (z)))));
				}
			}
			else if (transform.rotation != Quaternion.identity)
			{
				transform.rotation = Quaternion.identity;
			}

			if (glitchEffect)
			{
				if (!glitchEffectWasTrueBefore)
				{
					glitchEffectWasTrueBefore = true;
					CreateGlitchEffect();
				}
				else if (!glitchParent.activeSelf)
				{
					glitchParent.SetActive(true);
				}
			}
			else
			{
				if (glitchEffectWasTrueBefore && glitchParent.activeSelf)
				{
					glitchParent.SetActive(false);
				}
			}

			//BOUNDARY CONTROL
			if (obeyingBounds)
			{
				float xOffset = 48f / cameraTk2d.ZoomFactor;
				float yOffset = 27f / cameraTk2d.ZoomFactor;
				ForegroundSprite.color = new Color(ForegroundSprite.color.r, ForegroundSprite.color.g, ForegroundSprite.color.b, 1f);
				if (transform.position.x < minBound.x + xOffset)
				{
					/*ForegroundSprite.color = new Color(ForegroundSprite.color.r, ForegroundSprite.color.g, ForegroundSprite.color.b, 
						ForegroundSprite.color.a - (Mathf.Abs(transform.position.x - minBound.x - xOffset) / xOffset / 3f));*/
					transform.position = new Vector3(minBound.x + xOffset, transform.position.y, transform.position.z);
				}
				if (transform.position.y < minBound.y + yOffset)
				{
					/*ForegroundSprite.color = new Color(ForegroundSprite.color.r, ForegroundSprite.color.g, ForegroundSprite.color.b, 
						ForegroundSprite.color.a - (Mathf.Abs(transform.position.y - minBound.y - yOffset) / yOffset / 3f));*/
					transform.position = new Vector3(transform.position.x, minBound.y + yOffset, transform.position.z);
				}
				if (transform.position.x > maxBound.x - xOffset)
				{
					/*ForegroundSprite.color = new Color(ForegroundSprite.color.r, ForegroundSprite.color.g, ForegroundSprite.color.b, 
						ForegroundSprite.color.a - (Mathf.Abs(transform.position.x - minBound.x + xOffset) / xOffset / 3f));*/
					transform.position = new Vector3(maxBound.x - xOffset, transform.position.y, transform.position.z);
				}
				if (transform.position.y > maxBound.y - yOffset)
				{
					/*ForegroundSprite.color = new Color(ForegroundSprite.color.r, ForegroundSprite.color.g, ForegroundSprite.color.b, 
						ForegroundSprite.color.a - (Mathf.Abs(transform.position.y - minBound.y + yOffset) / yOffset / 3f));*/
					transform.position = new Vector3(transform.position.x, maxBound.y - yOffset, transform.position.z);
				}
			}

			Foreground.transform.position = transform.position + new Vector3(0, 0, 1);
		}

		void CreateGlitchEffect()
		{
			glitchParent = Instantiate(Resources.Load ("Orphan/CameraRelated/GlitchParent") as GameObject, transform.position + Vector3.forward, transform.rotation) as GameObject;
			glitchParent.transform.parent = transform;
			glitchParent.name = "GlitchParent";
		}

		public static void Shake(int count, float radius)
		{
			instance.shakeEaseOutCount = 0;
			instance.shakeEaseInCount = 0;
			instance.shakeCount = count;
			instance.shakeRadius = radius;
			instance.shakeStateRadius = 0;
			if (instance.shakeCount < 0)
			{
				instance.realPosition = instance.transform.position;
			}
		}

		public static void ShakeEaseOut(int count, float radius)
		{
			instance.shakeEaseOutCount = count;
			instance.shakeEaseInCount = 0;
			instance.shakeCount = count;
			instance.shakeRadius = radius;
			instance.shakeStateRadius = radius;
			if (instance.shakeCount < 0)
			{
				instance.realPosition = instance.transform.position;
			}
		}

		public static void ShakeEaseIn(int count, float radius)
		{
			instance.shakeEaseInCount = count;
			instance.shakeEaseOutCount = 0;
			instance.shakeCount = count;
			instance.shakeRadius = radius;
			instance.shakeStateRadius = radius;
			if (instance.shakeCount < 0)
			{
				instance.realPosition = instance.transform.position;
			}
		}

		public static void changeZoom(float _zoomFactor)
		{
			if (instance.cameraTk2d != null)
			{
				instance.cameraTk2d.ZoomFactor = _zoomFactor;
				instance.transform.localScale = Vector3.one / _zoomFactor;
				instance.cameraTk2d.transform.localScale = Vector3.one * _zoomFactor;
			}
		}

		public static void zoomInOut(float _zoomFactor, float _zoomSpeed)
		{
			instance.zoomFactor = _zoomFactor;
			instance.zoomSpeed = _zoomSpeed;
		}
	}
				/*cameraPoint = PlayerScript.instance.transform.position;
				if (PlayerScript.instance.directionForCamera != Vector3.zero)
				{
					cameraPoint += 0.025f * PlayerScript.instance.directionForCamera;
				}

				speed = PlayerScript.instance.speed * 1.2f;

				Vector3 deltaPosition = cameraPoint - transform.position;
				float newSpeed = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);

				if (newSpeed > 5f) //will walk
				{
					GetComponent<Rigidbody>().velocity = Vector3.zero;
					GetComponent<Rigidbody>().drag = 0;

					Vector3 direction = new Vector3(deltaPosition.x, deltaPosition.y, 0);

					float speedRatio = speed / newSpeed;

					direction = direction * speedRatio;
					GetComponent<Rigidbody>().AddForce(direction);
				}
				else
				{
					GetComponent<Rigidbody>().drag = drag;
				}*/
}

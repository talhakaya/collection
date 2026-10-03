using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Collection.Controls;

namespace Games.KayabrosPrototype4
{
	public class Background : MonoBehaviour {
	    public static Background instance;
	    [SerializeField] private GameObject circlePrefab;
	    [SerializeField] private Transform[] circles;
	    [SerializeField] private SpriteRenderer[] circleRenderers;
	    [SerializeField] private Vector2[] circleForces;
	    [SerializeField] private int numCircles;
	    private float MaxX;
	    private float MaxY;
	    const float MaxDistance = 5f;

	    void Start () {
			if (instance == null) {
	            instance = this;
	            Collection.Controls.PortHelpers.KeepWithinGame(gameObject); // In the collection: was DontDestroyOnLoad
	            MaxY = Camera.main.orthographicSize;
	            MaxX = MaxY * Screen.width / Screen.height;
	            MaxY -= 0.5f;
	            MaxX -= 0.5f;
	            circles = new Transform[numCircles];
	            circleForces = new Vector2[numCircles];
	            circleRenderers = new SpriteRenderer[numCircles];
	            for (int i = 0; i < numCircles; i++) {
	                circles[i] = Instantiate(circlePrefab, transform).transform;
	                circleRenderers[i] = circles[i].GetComponent<SpriteRenderer>();
	                circleRenderers[i].color = GetRandomColor();
	                circleForces[i] = new Vector2(0f, 0f);
	                circles[i].position = new Vector3(Random.Range(-MaxX, MaxX), Random.Range(-MaxY, MaxY), 0f);
	            }
	        }
	        else {
	            Destroy(gameObject);
	        }
		}

		void Update () {
	        List<Vector3> touchPos = new List<Vector3>();
	// In the collection: the original read the mouse in the editor and touches in a build.
	        // The mouse (or the collection's gamepad pointer) is used everywhere.
	        if (Collection.Controls.TaloketoInputManager.GetMouseButton(0)) {
	            touchPos.Add(Camera.main.ScreenToWorldPoint(Collection.Controls.TaloketoInputManager.mousePosition));
	        }
	        for (int i = 0; i < numCircles; i++) {
	            Vector2 force = new Vector2(0f, 0f);
	            for (int j = 0; j < numCircles; j++) {
	                if (i != j) {
	                    force += calculateForce(i, circles[j].position);
	                }
	            }
	            if (circles[i].position.x < -MaxX + MaxDistance) {
	                force += calculateForce(i, new Vector3(Mathf.Min(-MaxX, circles[i].position.x - 0.01f), circles[i].position.y, 0f));
	            }
	            if (circles[i].position.x > MaxX - MaxDistance) {
	                force += calculateForce(i, new Vector3(Mathf.Max(MaxX, circles[i].position.x + 0.01f), circles[i].position.y, 0f));
	            }
	            if (circles[i].position.y < -MaxY + MaxDistance) {
	                force += calculateForce(i, new Vector3(circles[i].position.x, Mathf.Min(-MaxY, circles[i].position.y - 0.01f), 0f));
	            }
	            if (circles[i].position.y > MaxY - MaxDistance) {
	                force += calculateForce(i, new Vector3(circles[i].position.x, Mathf.Max(MaxY, circles[i].position.y + 0.01f), 0f));
	            }
	            foreach (Vector3 tp in touchPos) {
	                force += calculateForce(i, tp, 5f, 1.5f);
	            }

	            circleForces[i] += force;
	            circleForces[i] *= 1f - Time.deltaTime;
	            circleForces[i] = Geometry.normalizeVector2(circleForces[i], Mathf.Max(0f, Geometry.lengthOfVector2(circleForces[i]) - Time.deltaTime));
	            circles[i].position += new Vector3(circleForces[i].x, circleForces[i].y, 0f) * Time.deltaTime;
	            circleRenderers[i].color = new Color(circleRenderers[i].color.r, circleRenderers[i].color.g, circleRenderers[i].color.b, 0.2f + 0.1f * Geometry.lengthOfVector2(circleForces[i]));
	        }
	    }

	    Color GetRandomColor() {
	        return new Color(Random.value, Random.value, Random.value, 0.2f);
	    }

	    Vector2 calculateForce(int i, Vector3 forcePos, float force = 1f, float forceLength = 1f) {
	        float d = Geometry.lengthOfVector3(circles[i].position - forcePos);
	        if (d < MaxDistance * forceLength) {
	            float a = Geometry.angleOfVector3(circles[i].position - forcePos);
	            return Geometry.createVector2(a, Time.deltaTime * force * (MaxDistance * forceLength - d) / d);
	        }
	        return new Vector2(0f, 0f);
	    }
	}
}

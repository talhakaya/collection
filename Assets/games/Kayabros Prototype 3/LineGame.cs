using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Collection.Controls;

namespace Games.KayabrosPrototype3
{
	public class LineGame : MonoBehaviour {
	    private LineRenderer lrFirst;
	    private LineRenderer[] lr;
	    private const int lPoolSize = 1000;
	    private int lPoolIndex = 0;
	    public Transform player;
	    public Transform[] enemies;
	    public Transform[] points;
	    public Transform[] enemyBullets;
	    private int enemyBulletPoolIndex = 0;
	    public Transform[] playerBullets;
	    private int playerBulletPoolIndex = 0;
	    private float screenRatio = 16f / 9f;

	    private int playerPowerup;
	    private float playerSpeed;
	    private float playerTurnTimer;
	    private float[] enemiesSpeed;
	    private float screenshake;

	    void Start () {
	        lrFirst = GetComponent<LineRenderer>();
	        lr = new LineRenderer[lPoolSize];
	        lr[0] = lrFirst;
	        for (int i = 1; i < lPoolSize; i++) {
	            lr[i] = new GameObject("RenderObject").AddComponent<LineRenderer>();
	            lr[i].materials = lrFirst.materials;
	            lr[i].startWidth = lr[i].endWidth = lrFirst.startWidth;
	            lr[i].startColor = lr[i].endColor = lrFirst.startColor;
	            lr[i].positionCount = 0;
	        }
	        enemiesSpeed = new float[enemies.Length];

	        ResetGame();
	    }

	    void ResetGame() {
	        player.gameObject.SetActive(true);
	        playerPowerup = 0;
	        playerSpeed = 0f;
	        player.position = new Vector3(0f, 0f, 0f);
	        player.eulerAngles = new Vector3(0f, 0f, 0f);
	        int numPoints = Random.Range(3, points.Length);
	        int numEnemies = Random.Range(3, enemies.Length);

	        for (int i = 0, len = points.Length; i < len; i++) {
	            if (i < numPoints) {
	                SetRandomPos(points[i]);
	                points[i].gameObject.SetActive(true);
	            }
	            else {
	                points[i].gameObject.SetActive(false);
	            }
	        }

	        for (int i = 0, len = enemies.Length; i < len; i++) {
	            if (i < numPoints) {
	                SetRandomPos(enemies[i]);
	                enemies[i].eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
	                enemies[i].gameObject.SetActive(true);
	            }
	            else {
	                enemies[i].gameObject.SetActive(false);
	            }
	            enemiesSpeed[0] = 0f;
	        }

	        foreach (Transform t in enemyBullets) {
	            t.gameObject.SetActive(false);
	        }
	        foreach (Transform t in playerBullets) {
	            t.gameObject.SetActive(false);
	        }
	    }

	    private void SetRandomPos(Transform t) {
	        float ort = Camera.main.orthographicSize;
	        Vector3 p = new Vector3(Random.Range(-ort, ort) * screenRatio, Random.Range(-ort, ort), 0f);
	        while (Geometry.lengthOfVector3(p) < 2f) {
	            p = new Vector3(Random.Range(-ort, ort) * screenRatio, Random.Range(-ort, ort), 0f);
	        }
	        t.position = p;
	    }

	    void Update() {
	        if (TaloketoInputManager.GetButton("Reset")) {
	            ResetGame();
	        }

	        //player controls
	        if (player.gameObject.activeSelf) {
	            if (TaloketoInputManager.GetAxisRaw("Horizontal") != 0f) {
	                playerTurnTimer += Time.deltaTime * 3f;
	                player.eulerAngles += new Vector3(0f, 0f, -TaloketoInputManager.GetAxisRaw("Horizontal") * 240f * Time.deltaTime * Mathf.Min(1f, playerTurnTimer));
	            }
	            else {
	                playerTurnTimer = 0f;
	            }
	            if (TaloketoInputManager.GetAxisRaw("Vertical") > 0f || playerSpeed < 0) {
	                playerSpeed += TaloketoInputManager.GetAxisRaw("Vertical") * 3f * Time.deltaTime;
	            }
	            else if (TaloketoInputManager.GetAxisRaw("Vertical") < 0f) {
	                playerSpeed += TaloketoInputManager.GetAxisRaw("Vertical") * 10f * Time.deltaTime;
	            }
	            player.position += Geometry.createVector3(player.eulerAngles.z, playerSpeed * Time.deltaTime);
	            ScreenWrap(player);
	            if (TaloketoInputManager.GetButtonDown("Fire1") || TaloketoInputManager.GetButtonDown("Jump")) {
	                float maxAngle = (playerPowerup == 0 ? 0 : (10 + playerPowerup * 10f));
	                float deltaAngle = (playerPowerup == 0 ? 0 : 2f * maxAngle / playerPowerup);

	                for (int i = 0; i <= playerPowerup; i++) {
	                    playerBullets[playerBulletPoolIndex].gameObject.SetActive(true);
	                    playerBullets[playerBulletPoolIndex].eulerAngles = player.eulerAngles + new Vector3(0f, 0f, -maxAngle + deltaAngle * i);
	                    playerBullets[playerBulletPoolIndex].position = player.position;
	                    playerBulletPoolIndex++;
	                    if (playerBulletPoolIndex >= playerBullets.Length) {
	                        playerBulletPoolIndex = 0;
	                    }
	                }

	                //MULTIPLE FIRE PLS
	            }
	        }
	        //point-player collision
	        foreach (Transform p in points) {
	            if (p.gameObject.activeSelf && Geometry.lengthOfVector3(player.position - p.position) < 0.2f) {
	                player.localScale += new Vector3(1f, 1f, 0f) * 0.2f;
	                p.gameObject.SetActive(false);
	                screenshake = 0.6f;
	                playerPowerup++;
	            }
	        }
	        //enemy logic
	        for (int i = 0, len = enemies.Length; i < len; i++) {
	            Transform e = enemies[i];
	            if (e.gameObject.activeSelf) {
	                float deltaAngle = Geometry.differenceOfAnglesNegative(Geometry.angleOfVector3(player.position - e.position), e.eulerAngles.z);
	                e.eulerAngles += new Vector3(0f, 0f, 5f * Time.deltaTime * deltaAngle);
	                enemiesSpeed[i] += (90f - Mathf.Abs(deltaAngle)) / 90f * Time.deltaTime * 0.2f;
	                e.position += Geometry.createVector3(e.eulerAngles.z, enemiesSpeed[i] * Time.deltaTime);
	                ScreenWrap(e);
	                if (Random.value < Time.deltaTime) {
	                    enemyBullets[enemyBulletPoolIndex].gameObject.SetActive(true);
	                    enemyBullets[enemyBulletPoolIndex].eulerAngles = new Vector3(0f, 0f, Geometry.angleOfVector3(player.position - e.position));
	                    enemyBullets[enemyBulletPoolIndex].position = e.position;
	                    enemyBulletPoolIndex++;
	                    if (enemyBulletPoolIndex >= enemyBullets.Length) {
	                        enemyBulletPoolIndex = 0;
	                    }
	                }
	                //enemy-player collision
	                if (player.gameObject.activeSelf && Geometry.lengthOfVector3(player.position - e.position) < 0.3f) {
	                    e.gameObject.SetActive(true);
	                    player.gameObject.SetActive(false);
	                    playerSpeed = 0f;
	                    screenshake += 2.5f;
	                }
	            }
	        }
	        //playerBullets
	        foreach (Transform pb in playerBullets) {
	            if (pb.gameObject.activeSelf) {
	                pb.position += Geometry.createVector3(pb.eulerAngles.z, 15f * Time.deltaTime);
	                //playerBullet-enemy collision
	                foreach (Transform e in enemies) {
	                    if (e.gameObject.activeSelf && Geometry.lengthOfVector3(e.position - pb.position) < 0.2f) {
	                        e.gameObject.SetActive(false);
	                        pb.gameObject.SetActive(false);
	                        screenshake += 0.5f;
	                    }
	                }
	            }
	        }
	        //enemyBullets
	        foreach (Transform eb in enemyBullets) {
	            if (eb.gameObject.activeSelf) {
	                eb.position += Geometry.createVector3(eb.eulerAngles.z, 15f * Time.deltaTime);
	                //enemyBullet-player collision
	                if (player.gameObject.activeSelf && Geometry.lengthOfVector3(player.position - eb.position) < 0.2f) {
	                    player.gameObject.SetActive(false);
	                    playerSpeed = 0f;
	                    screenshake += 2.5f;
	                }
	            }
	        }

	        Render();
	    }

		void Render() {
			foreach (LineRenderer l in lr) {
	            l.startColor = l.endColor = new Color(l.startColor.r, l.startColor.g, l.startColor.b, Mathf.Min(0.5f, l.startColor.a - Time.deltaTime));
	            for (int i = 0, len = l.positionCount; i < len; i++) {
	                Vector3 p = l.GetPosition(i);
	                l.SetPosition(i, new Vector3(p.x, p.y, 10f));
	            }
	        }

	        screenshake = Mathf.Max(0f, screenshake - Mathf.Max(1f, screenshake) * Time.deltaTime);

	        Render(transform);
	    }

	    void Render(Transform ts) {
	        if (!ts.gameObject.activeSelf) return;
	        foreach (Transform t in ts) {
	            if (t.GetComponent<RenderData>()) {
	                Render(t.GetComponent<RenderData>());
	            }
	            Render(t);
	        }
	    }

	    void Render(RenderData r) {
	        if (!r.gameObject.activeSelf) return;
	        Color c = r.color;
	        c.r = Mathf.Min(1f, c.r * Random.Range(1f, 2f));
	        c.g = Mathf.Min(1f, c.g * Random.Range(1f, 2f));
	        c.b = Mathf.Min(1f, c.b * Random.Range(1f, 2f));

	        LineRenderer l = GetLr();
	        l.startColor = l.endColor = c;

	        l.positionCount = r.points.Length + 1;
	        for (int i = 0, len = r.points.Length; i < len; i++) {
	            l.SetPosition(i, DistortPoint(r.transform.TransformPoint(r.points[i])));
	        }
	        l.SetPosition(r.points.Length, l.GetPosition(0));
	    }

	    LineRenderer GetLr() {
	        LineRenderer toReturn = lr[lPoolIndex];
	        lPoolIndex++;
	        if (lPoolIndex >= lPoolSize) {
	            lPoolIndex = 0;
	        }
	        return toReturn;
	    }

	    Vector3 DistortPoint(Vector3 p) {
	        return p + Geometry.createVector3(player.eulerAngles.z, -playerSpeed * 0.1f * Random.value) + Geometry.createVector3(Random.Range(0f, 360f), Random.Range(-screenshake, screenshake));
	    }

	    void ScreenWrap(Transform t) {
	        if (t.position.x < -Camera.main.orthographicSize * screenRatio) {
	            t.position = new Vector3(t.position.x + 2f * Camera.main.orthographicSize * screenRatio, t.position.y, t.position.z);
	        }
	        if (t.position.x > Camera.main.orthographicSize * screenRatio) {
	            t.position = new Vector3(t.position.x - 2f * Camera.main.orthographicSize * screenRatio, t.position.y, t.position.z);
	        }
	        if (t.position.y < -Camera.main.orthographicSize) {
	            t.position = new Vector3(t.position.x, t.position.y + 2f * Camera.main.orthographicSize, t.position.z);
	        }
	        if (t.position.y > Camera.main.orthographicSize) {
	            t.position = new Vector3(t.position.x, t.position.y - 2f * Camera.main.orthographicSize, t.position.z);
	        }
	    }
	}
}

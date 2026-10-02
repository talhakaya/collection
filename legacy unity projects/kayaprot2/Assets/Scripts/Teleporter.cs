using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : Gadget {
    private float timer;
    public float period = 1f;
    public float posPeriod = 2f;
    public SpriteRenderer telPoint;
    public GameObject trigger;
    private LineRenderer lineRenderer;
    public Bar bar;
    private bool canTeleport;

    void Start () {
        lineRenderer = GetComponent<LineRenderer>();
    }
	
	void Update () {
        if (telPoint.gameObject.activeSelf) {
            timer = Mathf.Min(timer + Game.dt, period);
        }
        float posRatio = Game.time % (posPeriod * 2f);
        if (posRatio > posPeriod) {
            posRatio = (posPeriod * 2f) - posRatio;
        }
        posRatio = posRatio / posPeriod;
        telPoint.transform.localPosition = new Vector3(4f + Easing.Linear(posRatio, 0f, 3f, 1f), 0.6f, 0f);
        lineRenderer.SetPosition(0, transform.position + transform.up * 0.6f * transform.localScale.y);
        lineRenderer.SetPosition(1, telPoint.transform.position);
        bar.animRatio = timer / period;

        canTeleport = false;
        const float rayLength = 0.6f;
        Vector3 rayPos = telPoint.transform.position - new Vector3(-rayLength / 2f, 0f, 0f);
        RaycastHit2D hit = Physics2D.Raycast(rayPos, new Vector3(1f, 0f, 0f));
        if (hit.collider == null || hit.collider.isTrigger || (hit.collider != null && !hit.collider.isTrigger && Geometry.lengthOfVector3(new Vector3(hit.point.x, hit.point.y, 0f) - rayPos) > rayLength)) {
            canTeleport = true;
        }
        telPoint.color = lineRenderer.startColor = lineRenderer.endColor = canTeleport ? new Color(1f, 1f, 1f) : new Color(0.3f, 0.3f, 0.3f);
    }

    public void Trigger(Collider2D col) {
        if (canTeleport && timer >= period) {
            if (col.GetComponent<Rigidbody2D>() != null) {
                Rigidbody2D body = col.GetComponent<Rigidbody2D>();
                if (!col.isTrigger && !body.isKinematic) {
                    timer = 0f;
                    body.position = telPoint.transform.position;
                }
            }
        }
    }
}

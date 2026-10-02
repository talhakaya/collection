using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {
    private Camera mainCamera;
    
	void Start () {
        mainCamera = Camera.main;
	}
	
	void Update () {
        float scale = 5f / mainCamera.orthographicSize;
        transform.localScale = new Vector3(scale, scale, 1f);
        float scaleSqrt = Mathf.Sqrt(scale);
        Vector2 levelMiddlePoint = new Vector2(LevelEditor.level.GetLength(0) - 1f, LevelEditor.level.GetLength(1) - 1f) * 0.5f;
        transform.position = new Vector3(levelMiddlePoint.x + (transform.parent.position.x - levelMiddlePoint.x) * 0.5f * scaleSqrt,
            levelMiddlePoint.y + (transform.parent.position.y - levelMiddlePoint.y) * 0.2f * scaleSqrt,
            transform.position.z);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background3D : MonoBehaviour
{
    private Camera mainCamera;
    private Transform model;
    private bool isOutside;

    public void Create(string levelName)
    {
        if (model != null) Destroy(model.gameObject);
        isOutside = levelName.Contains("outside") || levelName.Contains("final0") || levelName.Contains("final2");
        if (!levelName.Contains("mom") && !levelName.Contains("police") && !levelName.Contains("final1") && !levelName.Contains("final3")) {
            model = (Instantiate(Resources.Load(string.Format("3D/{0}", isOutside ? "outside" : (Platformer.LevelIndex % 16).ToString())), transform) as GameObject).transform;
            model.localPosition = new Vector3(0f, 0f, 0f);
        }
        if (isOutside) {
            transform.SetParent(null);
            transform.localPosition = new Vector3(LevelEditor.WIDTH * 0.5f, LevelEditor.HEIGHT * 0.5f, transform.localPosition.z);
        }
        else {
            transform.SetParent(mainCamera.transform);
            transform.localPosition = new Vector3(0f, 0f, transform.localPosition.z);
        }
    }

    void Awake() {
        mainCamera = Camera.main;
    }

    void Update() {
        float scale = mainCamera.orthographicSize / 10f;
        transform.localScale = new Vector3(scale, scale, scale);
        if (!isOutside) transform.localPosition = new Vector3(-transform.parent.position.x * 0.3f, -transform.parent.position.y * 0.1f, transform.localPosition.z);
    }
}

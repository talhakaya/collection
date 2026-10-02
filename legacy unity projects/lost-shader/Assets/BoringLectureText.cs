using UnityEngine;
using System.Collections;

public class BoringLectureText : MonoBehaviour
{
    private TextMesh textMesh;
    public float timer;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
    }

    void Update()
    {
        timer -= Game.dt;
        if (timer <= 0f)
        {
            timer = 0.2f;
            textMesh.text = "";
            while (Random.value > 0.3f)
            {
                float r = Random.value;
                if (r < 0.4f)
                {
                    textMesh.text += "boring ";
                }
                else if (r < 0.8f)
                {
                    textMesh.text += "lecture ";
                }
                else
                {
                    textMesh.text += "\n";
                }
            }
            
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    public SpriteRenderer sr;
    public Renderer r;
    public float speed;
    public List<Color> colors;
    public float timer;

    void Update()
    {
        timer += speed * Time.deltaTime;
        if (timer >= colors.Count) timer -= colors.Count;
        int colorIndex0 = (int)timer;
        int colorIndex1 = (int)timer + 1;
        if (colorIndex1 >= colors.Count) colorIndex1 = 0;
        Color newColor = Color.Lerp(colors[colorIndex0], colors[colorIndex1], timer % 1f);
        if (sr != null) sr.color = new Color(newColor.r, newColor.g, newColor.b, sr.color.a);
        if (r != null) r.sharedMaterial.SetColor("_BaseColor", new Color(newColor.r, newColor.g, newColor.b, r.sharedMaterial.GetColor("_BaseColor").a));
    }
}

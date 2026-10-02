using UnityEngine;
using System.Collections;

public class ExplosionData
{
    public float period = 1.6f;
    public float diameter = 20f;
    public float thickness = 2f;
    public float time;
    public Vector3 pos;
    public Color color = Color.red;

    public ExplosionData(Vector3 _pos, Color _color, float _diameter = 20f, float _period = 1.6f, float _thickness = 2f)
    {
        time = Game.time;
        pos = _pos;
        diameter = _diameter;
        period = _period;
        thickness = _thickness;
        color = _color;
    }

    public Color r(Vector3 location)
    {
        float toReturn = 0f;
        float distance = Geometry.lengthOfVector3(pos - location);
        float T = Game.time - time;
        float currentLength = diameter * T / period;
        float thick = thickness;// *(period * 1f + T * 1f) / period;
        if (distance > currentLength - thick && distance <= currentLength + thick)
        {
            if (distance < currentLength)
            {
                toReturn += (distance - currentLength + thick) * (period - T) / period;
            }
            else
            {
                toReturn += (currentLength + thick - distance) * (period - T) / period;
            }
        }
        return new Color(color.r * toReturn, color.g * toReturn, color.b * toReturn);
    }
}

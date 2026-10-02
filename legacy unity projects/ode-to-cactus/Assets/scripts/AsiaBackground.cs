using UnityEngine;
using System.Collections;

public class AsiaBackground : MonoBehaviour {

    public GameObject bgTile;
    public float tileCreatePeriod;
    private float tileCreateCounter;
    private float[] tileOffsets = new float[] { 0f, 0f, 0f, 0f };
    private float[] tileOffsets2;


	void Start ()
    {
        tileCreateCounter = 0f;
        tileOffsets2 = new float[] { Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f) };
	}
	
	void Update ()
    {
        tileCreateCounter += Game.dt;
        if (tileCreateCounter >= tileCreatePeriod)
        {
            tileCreateCounter -= tileCreatePeriod;
            for (int i = 0; i < 4; i++)
            {
                if (Random.value < 0.8f)
                {
                    tileOffsets[i] += Random.Range(-0.3f, 0.3f);
                    GameObject newTile = Instantiate(bgTile, new Vector3((i - 1.5f) * 4f + tileOffsets[i], 8f + tileOffsets2[i], 10f), Quaternion.identity) as GameObject;
                    newTile.transform.Rotate(Vector3.forward * Random.Range(-30f, 30f));
                }
                
            }
        }
	}
}

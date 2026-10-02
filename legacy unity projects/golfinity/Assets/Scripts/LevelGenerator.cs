using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;
    public GameObject[] holesUp;
    public GameObject[] holesLeft;
    public GameObject[] terrainAll;
    public GameObject[] terrainShort;
    public GameObject[] terrainVeryShort;
    private const float tileWidth = 8f;

    public Color[] skyColorz = new Color[] {
            new Color(0.6470588f, 0.9411765f, 1f),
            new Color(0.854902f, 0.3882353f, 0.3882353f),
            new Color(0.1921569f, 0.1921569f, 0.4196078f),
            new Color(0.3529412f, 0.7058824f, 0.3215686f),
            new Color(0.2784314f, 0.2784314f, 0.2784314f)
        };
    public Color[] terrainColorz = new Color[] {
            new Color(0.5882353f, 0.7803922f, 0.3333333f),
            new Color(0.4823529f, 0.2941177f, 0.5372549f),
            new Color(0.2470588f, 0.6313726f, 0.3686275f),
            new Color(0.6862745f, 0.3803922f, 0.6901961f),
            new Color(0.572549f, 0.572549f, 0.572549f)
        };
    public static Color[] skyColors = new Color[] {
            new Color(0.6470588f, 0.9411765f, 1f),
            new Color(0.854902f, 0.3882353f, 0.3882353f),
            new Color(0.1921569f, 0.1921569f, 0.4196078f),
            new Color(98f / 256f, 203f / 256f, 94f / 256f),
            new Color(0.2784314f, 0.2784314f, 0.2784314f)
        };
    public static Color[] terrainColors = new Color[] {
            new Color(0.5882353f, 0.7803922f, 0.3333333f),
            new Color(0.4823529f, 0.2941177f, 0.5372549f),
            new Color(0.2470588f, 0.6313726f, 0.3686275f),
            new Color(213f / 256f, 119f / 256f, 214f / 256f),
            new Color(0.572549f, 0.572549f, 0.572549f)
        };
    public Color skyColor;
    public Color terrainColor;

    //public Color[] skyColors = new Color[] {
    //        new Color(0.6176471f, 0.9367142f, 1f),
    //        new Color(0.8529412f, 0.3888408f, 0.3888408f),
    //        new Color(0.1219723f, 0.1235652f, 0.3529412f),
    //        new Color(0.6156863f, 0.827451f, 0.5960785f),
    //        new Color(0.2794118f, 0.2794118f, 0.2794118f)
    //    };
    //public Color[] terrainColors = new Color[] {
    //        new Color(0.5882741f, 0.7794118f, 0.3323962f),
    //        new Color(0.4960475f, 0.1964749f, 0.5808823f),
    //        new Color(0.1849049f, 0.5588235f, 0.3021675f),
    //        new Color(0.7529412f, 0.509804f, 0.7568628f),
    //        new Color(0.5735294f, 0.5735294f, 0.5735294f)
    //    };

    void Awake()
    {
        instance = this;
    }

    public string newColors()
    {
        string s = "public Color[] skyColors = " + logColors(skyColors);
        s += "\npublic Color[] terrainColors = " + logColors(terrainColors);
        return s;
    }

    public string logColors(Color[] colors)
    {
        string s = "new Color[] {";
        for (int i = 0; i < colors.Length; i++)
        {
            s += "\nnew Color(" + colors[i].r + "f, " + colors[i].g + "f, " + colors[i].b + "f),";
        }
        s = s.Substring(0, s.Length - 1);
        s += "\n};";
        return s;
    }

    void Start()
    {
        //ColorPicker.instance.pressLoad();
        ResetLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetLevel();
        }
    }

    public void ResetLevel()
    {
        SetColors();
        DeleteLevel();
        GenerateLevel();
    }

    void DeleteLevel()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void GenerateLevel()
    {
        int noOfColumns = 4 + 2 * Random.Range(0, 3 + Game.noOfHoles / 100);
        int noOfRows = 4 + 2 * Random.Range(0, 2 + Game.noOfHoles / 100);

        if (Game.noOfHoles < 11)
        {
            noOfRows = 2;
        }
        else if (Game.noOfHoles < 31)
        {
            noOfRows = 2 + 2 * Random.Range(0, 2);
        }
        else if (Game.noOfHoles < 51)
        {
            noOfRows = 2 + 2 * Random.Range(0, 3);
        }

        int holeTile = Random.Range(0, noOfColumns);

        if (Game.noOfHoles > 100 && Game.holesOnWalls)
        {
            holeTile = Random.Range(-1, noOfColumns + 1);
        }
        while (noOfRows == 2 && holeTile == 0)
        {
            holeTile = Random.Range(0, noOfColumns);
        }

        bool createdHole = false;

        for (int j = -1; j < noOfRows + 1; j++)
        {
            int emptyTile = Random.Range(0, noOfColumns);
            int emptyTile2 = Random.Range(0, noOfColumns);

            if (holeTile == -1 || holeTile == noOfColumns)
            {
                while (j == noOfRows - 2 && (emptyTile == holeTile - 1 || emptyTile == holeTile + 1))
                {
                    emptyTile = Random.Range(0, noOfColumns);
                }
                while (j == noOfRows - 2 && (emptyTile2 == holeTile - 1 || emptyTile2 == holeTile + 1))
                {
                    emptyTile2 = Random.Range(0, noOfColumns);
                }
            }
            else
            {
                while (j == noOfRows - 2 && emptyTile == holeTile)
                {
                    emptyTile = Random.Range(0, noOfColumns);
                }
                while (j == noOfRows - 2 && emptyTile2 == holeTile)
                {
                    emptyTile2 = Random.Range(0, noOfColumns);
                }
            }

            for (int i = -1; i < noOfColumns + 1; i++)
            {
                if (i == -1 || i == noOfColumns || j == -1 || j == noOfRows)
                {
                    if (j == noOfRows - 1 && holeTile == i)
                    {
                        if (i == -1)
                        {
                            GameObject go = create(holesLeft[Random.Range(0, holesLeft.Length)], i, j);
                            go.transform.localScale = new Vector3(-go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);
                        }
                        else
                        {
                            create(holesLeft[Random.Range(0, holesLeft.Length)], i, j);
                        }
                        createdHole = true;
                    }
                    else if (j % 2 == 1)
                    {
                        if (i == -1)
                        {
                            create(terrainVeryShort[Random.Range(0, terrainVeryShort.Length)], i, j, -90);
                        }
                        else
                        {
                            create(terrainVeryShort[Random.Range(0, terrainVeryShort.Length)], i, j, 90);
                        }
                    }
                    else
                    {
                        create(terrainAll[0], i, j);
                    }
                }
                else if (i == 0 && j == 1)
                {
                    GolfBall.instance.transform.position = pos(i, j);
                    GolfBall.instance.transform.position = new Vector3(GolfBall.instance.transform.position.x, GolfBall.instance.transform.position.y, -2f);
                    GolfBall.instance.startPos = GolfBall.instance.transform.position;
                }
                else if (j % 2 == 0)
                {
                    if (i == emptyTile || i == emptyTile2)
                    {

                    }
                    else if (j == noOfRows - 2 && i == holeTile)
                    {
                        create(holesUp[Random.Range(0, holesUp.Length)], i, j);
                        createdHole = true;
                    }
                    else if (i == 0 || i == noOfColumns - 1)
                    {
                        create(terrainVeryShort[Random.Range(0, terrainVeryShort.Length)], i, j);
                    }
                    else
                    {
                        if (j > 0 && Random.value < 0.3f)
                        {
                            create(terrainVeryShort[Random.Range(0, terrainVeryShort.Length)], i, j, 180);
                        }
                        else if (i == emptyTile - 1 || i == emptyTile + 1 || i == emptyTile2 - 1 || i == emptyTile2 + 1)
                        {
                            if ((i == emptyTile + 1 && i != emptyTile2 - 1) || (i == emptyTile2 + 1 && i != emptyTile - 1))
                            {
                                if (Random.value < 0.3f)
                                {
                                    create(terrainVeryShort[Random.Range(0, terrainVeryShort.Length)], i, j, 90);
                                }
                                else
                                {
                                    create(terrainVeryShort[Random.Range(0, terrainVeryShort.Length)], i, j);
                                }
                            }
                            else if ((i == emptyTile - 1 && i != emptyTile2 + 1) || (i == emptyTile2 - 1 && i != emptyTile + 1))
                            {
                                if (Random.value < 0.3f)
                                {
                                    create(terrainVeryShort[Random.Range(0, terrainVeryShort.Length)], i, j, -90);
                                }
                                else
                                {
                                    create(terrainVeryShort[Random.Range(0, terrainVeryShort.Length)], i, j);
                                }
                            }
                            else
                            {
                                create(terrainVeryShort[Random.Range(0, terrainVeryShort.Length)], i, j);
                            }
                        }
                        else
                        {
                            create(terrainAll[Random.Range(0, terrainAll.Length)], i, j);
                        }
                    }
                }
            }
        }

        create(terrainAll[0], -6, noOfRows / 2).transform.localScale = new Vector3(9, noOfRows + 10, 1);
        create(terrainAll[0], noOfColumns + 5, noOfRows / 2).transform.localScale = new Vector3(9, noOfRows + 10, 1);
        create(terrainAll[0], noOfColumns / 2, -6).transform.localScale = new Vector3(noOfColumns + 10, 9, 1);
        create(terrainAll[0], noOfColumns / 2, noOfRows + 5).transform.localScale = new Vector3(noOfColumns + 10, 9, 1);

        if (!createdHole)
        {
            Debug.Log("didn't created hole: " + holeTile);
        }
    }

    GameObject create(GameObject prefab, int i, int j, float angle = 0)
    {
        GameObject go = Instantiate(prefab, pos(i, j), Quaternion.identity) as GameObject;
        go.transform.parent = transform;
        go.transform.eulerAngles = Vector3.forward * angle;
        return go;
    }

    Vector3 pos(int i, int j)
    {
        return new Vector3(i * tileWidth, j * tileWidth, 0f);
    }

    public void SetColors()
    {
        skyColor = skyColors[(Game.noOfHoles / 50) % skyColors.Length];
        terrainColor = terrainColors[(Game.noOfHoles / 50) % terrainColors.Length];
        Camera.main.backgroundColor = skyColor;
    }
}

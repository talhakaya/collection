using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public static float time;
	public static float dt;
	public static Color color0 = new Color(183 / 255f, 162 / 255f, 130 / 255f);
	public static Color color1 = new Color(193 / 255f, 106 / 255f, 68 / 255f);
	public static Color color2 = new Color(170 / 255f, 68 / 255f, 68 / 255f);
	public static Color color3 = new Color(189 / 255f, 68 / 255f, 193 / 255f);
	public static Color color4 = new Color(110 / 255f, 64 / 255f, 183 / 255f);
	public static Color[] colors = new Color[]{color0, color1, color2, color3, color4};
    public static Transform cameraTransform;
    public static Game instance;
    public AudioSource dus;
    public AudioSource dus2;
    public AudioSource yen;
    public AudioSource yen1;
    public AudioSource yen2;
    public AudioSource tasak;
    public GameObject win;
    public SpriteRenderer p1Name;
    public SpriteRenderer p2Name;
    public SpriteRenderer p1;
    public SpriteRenderer p2;
    public Sprite[] players;
    public Sprite[] playerNames;

    private bool done;

    void Awake()
    {
        PlayerScript.list = new PlayerScript[] { null, null, null, null };
        cameraTransform = transform;
        instance = this;
    }

    void Start()
    {
        time = 0f;
        int ran = Random.Range(0, 12);
        int ran2 = Random.Range(0, 12);
        while (ran == ran2)
        {
            ran2 = Random.Range(0, 12);
        }
        p1.sprite = players[ran];
        p2.sprite = players[ran2];
        p1Name.sprite = playerNames[ran];
        p2Name.sprite = playerNames[ran2];
    }
	
	void Update ()
	{
		dt = Time.deltaTime;
		time += dt;

        if (p1 != null)
        {
            p1Name.transform.position = p1.transform.position;
        }
        if (p2 != null)
        {
            p2Name.transform.position = p2.transform.position;
        }
        
        if (time < 5f)
        {
            p1Name.color = new Color(1f, 1f, 1f, 1f);
            p2Name.color = new Color(1f, 1f, 1f, 1f);
        }
        else if (time < 6f)
        {
            p1Name.color = new Color(1f, 1f, 1f, 6f - time);
            p2Name.color = new Color(1f, 1f, 1f, 6f - time);
        }
        else
        {
            p1Name.color = new Color(1f, 1f, 1f, 0f);
            p2Name.color = new Color(1f, 1f, 1f, 0f);
        }

		MousePosition.get = Camera.main.ScreenToWorldPoint (Input.mousePosition) + Vector3.forward;
		MousePosition.x = MousePosition.get.x;
        MousePosition.y = MousePosition.get.y;

        if (Input.GetKey(KeyCode.Return))
        {
            Application.LoadLevel(1);
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        for (int i = 0; i < PlayerScript.list.Length; i++)
        {
            if (PlayerScript.list[i] != null)
            {
                if (PlayerScript.list[i].transform.position.y < -20f)
                {
                    if (PlayerScript.list[i].transform.position.x > 0)
                    {
                        Camera.main.transform.Rotate(Vector3.forward * Random.Range(10f, 30f));
                    }
                    else
                    {
                        Camera.main.transform.Rotate(-Vector3.forward * Random.Range(10f, 30f));
                    }
                    Destroy(PlayerScript.list[i].transform.gameObject);

                    if (Random.value < 0.5f)
                    {
                        dus.Play();
                    }
                    else
                    {
                        dus2.Play();
                    }
                }
            }
        }

        if (!done)
        {
            int aliveGuy = 0;
            for (int j = 0; j < PlayerScript.list.Length; j++)
            {
                if (PlayerScript.list[j] != null)
                {
                    aliveGuy++;
                }
            }
            if (aliveGuy <= 1)
            {
                float ran = Random.value;
                if (ran < 0.33f)
                {
                    yen.Play();
                }
                else if (ran < 0.67f)
                {
                    yen1.Play();
                }
                else
                {
                    yen2.Play();
                }
                done = true;
                win.SetActive(true);
            }
        }

        if (SpriteEffect.blurConst > 0f)
        {
            SpriteEffect.blurConst -= 10f * Game.dt;
            if (SpriteEffect.blurConst < 0f)
            {
                SpriteEffect.blurConst = 0f;
            }
        }
	}
}

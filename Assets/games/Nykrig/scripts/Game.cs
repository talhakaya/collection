using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Games.Nykrig
{
	public class Game : MonoBehaviour {
	    public static Game instance;
		public static float time;
		public static float dt;
		public static float dtPhysics = 0.0166667f;
	    public static Color color0 = new Color(183 / 255f, 162 / 255f, 130 / 255f);
		public static Color color1 = new Color(193 / 255f, 106 / 255f, 68 / 255f);
		public static Color color2 = new Color(170 / 255f, 68 / 255f, 68 / 255f);
		public static Color color3 = new Color(189 / 255f, 68 / 255f, 193 / 255f);
		public static Color color4 = new Color(110 / 255f, 64 / 255f, 183 / 255f);
		public static Color[] colors = new Color[]{color0, color1, color2, color3, color4};
		public static bool input;
		private static bool inputOld;
		public static bool inputDown;
		public static bool inputUp;
	    public static Vector3 shadowVector;
	    public static float DefaultShadowDistance = 0.3f;
	    public const float levelUnitX = 5f * 16f / 9f;
	    public const float levelUnitY = 5f;
	    public static float levelMinX = -2f * levelUnitX;
	    public static float levelMinY = -2f * levelUnitY;
	    public static float levelMaxX = 2f * levelUnitX;
	    public static float levelMaxY = 2f * levelUnitY;
	    public Camera[] renderTextureCameras;
	    public RawImage[] renderTextureImages;
	    private static Texture2D tex;
	    public Menu menu;
	    public GameObject result;
	    public GameObject imageLighting;
	    public GameObject imageNightVision;
	    public Player player0;
	    public Player player1;
	    public bool endless;
	    public static float endlessTimer;
	    private float endlessMusicTimer;
	    private float endlessMusicPeriod = 10f;
	    public bool twoPlayers;
	    public int levelNo;
	    public static int score;
	    public Text textScore0;
	    public Text textScore1;
	    public Transform levelParent;
	    public List<Level> levels;
	    public static int enemyCount;
	    private float nextLevelTimer;
	    private static int enemyCountOld;
	    public SpriteRenderer circleTransition;
	    private float circleTransitionScale;
	    private int screenWidth;
	    private int screenHeight;

	    public static bool gamepad;
	    public static float mouseAngle;
	    public static float mouseDistance;
	    public static float mouseDistanceClamped;

	    void Awake()
		{
	        instance = this;
	        shadowVector = Vector3.left;
	        tex = new Texture2D(renderTextureCameras[0].targetTexture.width, renderTextureCameras[0].targetTexture.height, TextureFormat.RGB24, true);
	        levels = new List<Level>();
	        foreach (Transform child in levelParent) {
	            levels.Add(child.GetComponent<Level>());
	            child.position = new Vector3(0f, 0f, 0f);
	            child.gameObject.SetActive(false);
	        }
	        circleTransitionScale = circleTransition.transform.localScale.x;
	        circleTransition.transform.localScale = new Vector3(0f, 0f, 1f);
	        screenWidth = 0;
	        screenHeight = 0;
	    }
		
		void Update ()
		{
			if (Input.GetButtonDown("Escape"))
	        {
	            if (menu.gameObject.activeSelf) {
	                Application.Quit();
	            }
	            else {
	                menu.gameObject.SetActive(true);
	                player0.gameObject.SetActive(false);
	                player1.gameObject.SetActive(false);
	            }
	        }
			
			dt = Time.deltaTime;
			time += dt;

			MousePosition.get = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	        MousePosition.get.z = 0f;
	        if (MousePosition.get.x > transform.position.x + levelUnitX) {
	            MousePosition.get.x = transform.position.x + levelUnitX;
	        }
	        if (MousePosition.get.y > transform.position.y + levelUnitY) {
	            MousePosition.get.y = transform.position.y + levelUnitY;
	        }
	        if (MousePosition.get.x < transform.position.x - levelUnitX) {
	            MousePosition.get.x = transform.position.x - levelUnitX;
	        }
	        if (MousePosition.get.y < transform.position.y - levelUnitY) {
	            MousePosition.get.y = transform.position.y - levelUnitY;
	        }
	        MousePosition.x = MousePosition.get.x;
			MousePosition.y = MousePosition.get.y;

			input = Input.GetMouseButton (0);
			inputDown = input && !inputOld;
			inputUp = !input && inputOld;

			inputOld = input;

	        //shadowVector
	        if (!player0.gameObject.activeSelf && !player1.gameObject.activeSelf) {
	            if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f || Input.GetButton("Fire")) {
	                gamepad = false;
	            }
	            if (Input.GetAxisRaw("Horizontal0") != 0f || Input.GetAxisRaw("Vertical0") != 0f || Input.GetAxisRaw("Horizontal1") != 0f || Input.GetAxisRaw("Vertical1") != 0f || Input.GetAxisRaw("FireAxis") > 0.5f || Input.GetAxisRaw("FireAxisMac") > 0.5f) {
	                gamepad = true;
	            }

	            if (gamepad) {
	                Vector2 gamepadVector = new Vector2(Input.GetAxis("Horizontal0"), Input.GetAxis("Vertical0")) + new Vector2(Input.GetAxis("Horizontal1"), Input.GetAxis("Vertical1"));
	                float gamepadVectorLength = Geometry.lengthOfVector2(gamepadVector);
	                //if (gamepadVectorLength >= 0.2f) {
	                //}
	                mouseAngle = Geometry.angleOfVector2(gamepadVector);
	                mouseDistance = gamepadVectorLength * 5f;
	            }
	            else {
	                mouseAngle = Geometry.angleOfVector3(MousePosition.get - Game.instance.transform.position);
	                mouseDistance = Geometry.lengthOfVector3(MousePosition.get - Game.instance.transform.position);
	            }
	            mouseDistanceClamped = Mathf.Clamp(mouseDistance, 1f, 5f);
	            Game.shadowVector = Geometry.createVector3(mouseAngle, 0.1f * mouseDistanceClamped);
	        }

	        //camera
	        if (!menu.gameObject.activeSelf && !result.activeSelf) {
	            Vector3 cameraPos = new Vector3(0f, 0f, 0f);
	            if (player0.gameObject.activeSelf && player1.gameObject.activeSelf) {
	                cameraPos = (player0.transform.position + player1.transform.position) * 0.5f;
	            }
	            else if (player0.gameObject.activeSelf) {
	                cameraPos = new Vector3(player0.transform.position.x, player0.transform.position.y, 0f) + 0.1f * (MousePosition.get - player0.transform.position);
	            }
	            else if (player1.gameObject.activeSelf) {
	                cameraPos = new Vector3(player1.transform.position.x, player1.transform.position.y, 0f) + 0.1f * (MousePosition.get - player1.transform.position);
	            }
	            else {
	                cameraPos = transform.position;
	            }
	            transform.position = new Vector3(cameraPos.x, cameraPos.y, transform.position.z);
	            if (transform.position.x < levelMinX + levelUnitX) {
	                transform.position = new Vector3(levelMinX + levelUnitX, transform.position.y, transform.position.z);
	            }
	            if (transform.position.x > levelMaxX - levelUnitX) {
	                transform.position = new Vector3(levelMaxX - levelUnitX, transform.position.y, transform.position.z);
	            }
	            if (transform.position.y < levelMinY + levelUnitY) {
	                transform.position = new Vector3(transform.position.x, levelMinY + levelUnitY, transform.position.z);
	            }
	            if (transform.position.y > levelMaxY - levelUnitY) {
	                transform.position = new Vector3(transform.position.x, levelMaxY - levelUnitY, transform.position.z);
	            }
	        }
	        else {
	            transform.position = new Vector3(0f, 0f, transform.position.z);
	        }
	        //state check
	        if (!menu.gameObject.activeSelf && !result.activeSelf) {
	            if (!player0.gameObject.activeSelf && !player1.gameObject.activeSelf) {
	                if (endless) {
	                    result.SetActive(true);
	                }
	                else {
	                    bool fireButton = (Input.GetButton("Fire") || Input.GetAxisRaw("FireAxis") > 0.5f || Input.GetAxisRaw("FireAxisMac") > 0.5f);
	                    if (fireButton) {
	                        StartGame(endless, twoPlayers, levelNo);
	                    }
	                }
	            }
	            if (!endless) {
	                if (enemyCount <= 0 && enemyCountOld <= 0) {
	                    nextLevelTimer += Game.dt * 1.2f;
	                    circleTransition.color = new Color(circleTransition.color.r, circleTransition.color.g, circleTransition.color.b, 1f);
	                    float scale = Mathf.Max(0f, 1f / 0.3f * (nextLevelTimer - 0.7f)) * circleTransitionScale;
	                    circleTransition.transform.localScale = new Vector3(scale, scale, 1f);
	                    if (nextLevelTimer > 1f) {
	                        if (levels.Count > levelNo + 1) {
	                            StartGame(endless, twoPlayers, levelNo + 1);
	                        }
	                        else {
	                            player0.gameObject.SetActive(false);
	                            player1.gameObject.SetActive(false);
	                            menu.gameObject.SetActive(true);
	                        }
	                    }
	                }
	            }
	            else {
	                endlessTimer += Game.dt;
	            }
	            enemyCountOld = enemyCount;
	        }
	        imageLighting.SetActive(!menu.gameObject.activeSelf);
	        imageNightVision.SetActive(!menu.gameObject.activeSelf);

	        if (endless && !menu.gameObject.activeSelf && !result.activeSelf) {
	            textScore0.text = "SCORE\n" + score;
	            if (endlessMusicTimer >= endlessMusicPeriod) {
	                endlessMusicTimer = 0f;
	                List<int> channels = new List<int>();
	                channels.Add(Random.Range(0, Music.instance.audios.Length));
	                channels.Add(Random.Range(0, Music.instance.audios.Length));
	                channels.Add(Random.Range(0, Music.instance.audios.Length));
	                List<int> channelResults = new List<int>();
	                for (int i = 0; i < channels.Count; i++) {
	                    if (channels[i] == 1 || channels[i] == 2 || channels[i] == 5) {
	                        channelResults.Add(channels[i]);
	                        break;
	                    }
	                    else {
	                        channelResults.Add(channels[i]);
	                    }
	                }
	                Music.instance.Set(channelResults);
	            }
	            else {
	                endlessMusicTimer += Game.dt;
	            }
	        }
	        else {
	            textScore0.text = "";
	        }
	        textScore1.text = textScore0.text;

	        if (circleTransition.transform.localScale.x >= circleTransitionScale && circleTransition.color.a > 0f) {
	            circleTransition.color = new Color(circleTransition.color.r, circleTransition.color.g, circleTransition.color.b, circleTransition.color.a - Game.dt * 2f);
	        }

	        Wall.lightAlpha = Mathf.Min(1f, Wall.lightAlpha + 1f * Game.dt);

	        if (screenWidth != Screen.width || screenHeight != Screen.height) {
	            screenWidth = Screen.width;
	            screenHeight = Screen.height;

	            const int textureWidth = 800;
	            int textureHeight = textureWidth * screenHeight / screenWidth;

	            for (int i = 0, len = renderTextureCameras.Length; i < len; i++) {
	                Camera c = renderTextureCameras[i];
	                RenderTexture rt = new RenderTexture(textureWidth, textureHeight, 24);
	                c.targetTexture = rt;
	                renderTextureImages[i].texture = rt;
	            }
	        }
	    }

	    public void StartGame(bool endless, bool twoPlayers, int levelNo = 0) {
	        score = 0;
	        enemyCount = 0;
	        enemyCountOld = 1;
	        nextLevelTimer = 0f;
	        ObjectPool.ResetAll();
	        player0.gameObject.SetActive(true);
	        player1.gameObject.SetActive(twoPlayers);
	        player0.transform.position = new Vector3(twoPlayers  ? - 1.5f : 0f, 0f, 0f);
	        player1.transform.position = new Vector3(1.5f, 0f, 0f);
	        player0.transform.localScale = new Vector3(1f, 1f, 1f);
	        player1.transform.localScale = new Vector3(1f, 1f, 1f);
	        this.endless = endless;
	        this.twoPlayers = twoPlayers;
	        this.levelNo = levelNo;
	        endlessMusicTimer = endlessMusicPeriod;
	        endlessTimer = 0f;
	        if (endless) {
	            levelMinX = -2f * levelUnitX;
	            levelMaxX = 2f * levelUnitX;
	            levelMinY = -2f * levelUnitY;
	            levelMaxY = 2f * levelUnitY;
	        }
	        for (int i = 0, len = levels.Count; i < len; i++) {
	            if (!endless && levelNo == i) {
	                Music.instance.Set(levels[i].musicChannels);
	                levelMinX = (levels[i].minX - 1) * levelUnitX;
	                levelMaxX = (levels[i].maxX + 1) * levelUnitX;
	                levelMinY = (levels[i].minY - 1) * levelUnitY;
	                levelMaxY = (levels[i].maxY + 1) * levelUnitY;
	                levels[i].gameObject.SetActive(true);
	                levels[i].Spawn();
	            }
	            else {
	                levels[i].gameObject.SetActive(false);
	            }
	        }
	    }

	    void OnPostRender() {
	        RenderTexture rt = RenderTexture.active;
	        RenderTexture.active = renderTextureCameras[0].targetTexture;
	        tex.ReadPixels(new Rect(0, 0, renderTextureCameras[0].targetTexture.width, renderTextureCameras[0].targetTexture.height), 0, 0);
	        tex.Apply();
	        RenderTexture.active = rt;
	    }

	    public static bool isPosLit(Vector2 pos) {
	        return arePosLit(new Vector2[1] { pos });
	    }

	    public static bool arePosLit(Vector2[] pos, bool oneIsEnough = true) {
	        for (int i = 0, len = pos.Length; i < len; i++) {
	            pos[i] = Camera.main.WorldToScreenPoint(pos[i]);
	            pos[i].x /= Screen.width;
	            pos[i].y /= Screen.height;
	            if (pos[i].x < 0f || pos[i].y < 0f || pos[i].x >= 1f || pos[i].y >= 1f) {
	                if (!oneIsEnough) return false;
	            }
	            else {
	                pos[i].x *= instance.renderTextureCameras[0].targetTexture.width;
	                pos[i].y *= instance.renderTextureCameras[0].targetTexture.height;
	                Color c = tex.GetPixel(Mathf.RoundToInt(pos[i].x), Mathf.RoundToInt(pos[i].y));
	                if (c.r == 0f && c.g == 0f && c.b == 0f) {
	                    if (!oneIsEnough) return false;
	                }
	                else {
	                    if (oneIsEnough) return true;
	                }
	            }
	        }
	        return !oneIsEnough;
	    }

	    public static Player getPlayer(Vector3 pos) {
	        if (instance.twoPlayers) {
	            if (Geometry.lengthOfVector3(instance.player0.transform.position - pos) < Geometry.lengthOfVector3(instance.player1.transform.position - pos)) {
	                return instance.player0;
	            }
	            else {
	                return instance.player1;
	            }
	        }
	        else {
	            return instance.player0;
	        }
	    }
	}

}

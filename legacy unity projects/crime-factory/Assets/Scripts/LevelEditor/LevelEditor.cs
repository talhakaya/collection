using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ClimbableTile {
    None,
    L,
    LD,
    LR,
    R,
    RD,
    D,
    LRD,
}

public enum TileCategory {
    Other,
    Wall,
    WallIndest,
    Enemy,
    MovingPlatform,
    NPC,
    Tutorial,
    NumCategories
}

public class LevelEditor : MonoBehaviour {
    public Background3D background3D;
    public AudioSource musicPlayer;
    public AudioClip[] musicList;
    private int musicIndex;
    public bool isMK;
    public static bool isMKStatic;
    private static LevelEditor instance;
    public static bool On {
        get { return instance != null && instance.isActiveAndEnabled && instance.isOn; }
    }
    private bool isOn;
    [SerializeField] private Transform cursor;
    [SerializeField] private GameObject uiParent;
    private int mouseXOld;
    private int mouseYOld;
    private bool canPutObject;
    public static GameObject[,] level;
    private List<GameObject> elements;
    private List<ClimbableTile> elementClimbables;
    private List<TileCategory> elementCategories;
    private int categorySelected = -1;
    private int elementSelected;
    private int instanceSelectedX;
    private int instanceSelectedY;
    private int instanceSelectedDeltaX = 0;
    private int instanceSelectedDeltaY = 0;
    public static int WIDTH = 49;
    public static int HEIGHT = 28;
    public Button[] buttons;
    public Text textFile;
    public InputField inputWidth;
    public InputField inputHeight;
    public InputField inputLevelLoad;
    public InputField inputLevelSave;
    public EventSystem eventSystem;
    public GraphicRaycaster graphicRaycaster;
    public Transform[] blackSides;
    public GameObject inspector;
    public Text inspectorTitle;
    public LevelEditorInputField[] inspectorInputFieldNumbers;
    public LevelEditorInputField[] inspectorInputFields;
    public LevelEditorSlider[] inspectorSliders;
    public LevelEditorDropdown[] inspectorDropdowns;
    public static bool updateAlsoPlatforms;
    public Sprite[] spriteClimbables;
    public static System.Action<string> OnLoad;

    private void Awake() {
        isMKStatic = isMK;
        Platformer.isMK = isMK;
        instance = this;
        DeselectTile();
        musicIndex = -1;
        if (elements == null) {
            string mk = isMK ? "mk/" : "";
            elements = new List<GameObject>()
            {
                Resources.Load<GameObject>(mk + "player"),
                Resources.Load<GameObject>(mk + "door"),
                Resources.Load<GameObject>(mk + "wall"),
                Resources.Load<GameObject>(mk + "wall"),
                Resources.Load<GameObject>(mk + "wall"),
                Resources.Load<GameObject>(mk + "wall"),
                Resources.Load<GameObject>(mk + "wall"),
                Resources.Load<GameObject>(mk + "wall"),
                Resources.Load<GameObject>(mk + "wall"),
                Resources.Load<GameObject>(mk + "wall"),
                Resources.Load<GameObject>(mk + "wallIndest"),
                Resources.Load<GameObject>(mk + "wallIndest"),
                Resources.Load<GameObject>(mk + "wallIndest"),
                Resources.Load<GameObject>(mk + "wallIndest"),
                Resources.Load<GameObject>(mk + "wallIndest"),
                Resources.Load<GameObject>(mk + "wallIndest"),
                Resources.Load<GameObject>(mk + "wallIndest"),
                Resources.Load<GameObject>(mk + "wallIndest"),
                Resources.Load<GameObject>(mk + "movingPlatform"),
                Resources.Load<GameObject>(mk + "movingPlatform"),
                Resources.Load<GameObject>(mk + "movingPlatform"),
                Resources.Load<GameObject>(mk + "movingPlatform"),
                Resources.Load<GameObject>(mk + "movingPlatform"),
                Resources.Load<GameObject>(mk + "movingPlatform"),
                Resources.Load<GameObject>(mk + "movingPlatform"),
                Resources.Load<GameObject>(mk + "movingPlatform"),
                Resources.Load<GameObject>(mk + "ladder"),
                Resources.Load<GameObject>(mk + "enemyLaser"),
                Resources.Load<GameObject>(mk + "enemyCritter"),
                Resources.Load<GameObject>(mk + "enemyJumper"),
                Resources.Load<GameObject>(mk + "enemyPistol"),
                Resources.Load<GameObject>(mk + "enemyDiagonal"),
                Resources.Load<GameObject>(mk + "enemyBat"),
                Resources.Load<GameObject>(mk + "spike"),
                Resources.Load<GameObject>(mk + "boss"),
                Resources.Load<GameObject>(mk + "bossSpider"),
                Resources.Load<GameObject>(mk + "bombBox"),
                Resources.Load<GameObject>(mk + "hanger"),
                Resources.Load<GameObject>(mk + "npc/npc"),
                Resources.Load<GameObject>(mk + "wallIndest2"),
                Resources.Load<GameObject>(mk + "wallIndest2"),
                Resources.Load<GameObject>(mk + "wallIndest2"),
                Resources.Load<GameObject>(mk + "wallIndest2"),
                Resources.Load<GameObject>(mk + "wallIndest2"),
                Resources.Load<GameObject>(mk + "wallIndest2"),
                Resources.Load<GameObject>(mk + "wallIndest2"),
                Resources.Load<GameObject>(mk + "wallIndest2"),
                Resources.Load<GameObject>(mk + "npc/mother0"),
                Resources.Load<GameObject>(mk + "tutoJump"),
                Resources.Load<GameObject>(mk + "tutoFire"),
                Resources.Load<GameObject>(mk + "tutoBomb"),
                Resources.Load<GameObject>(mk + "tutoRestart"),
                Resources.Load<GameObject>(mk + "tutoUp"),
                Resources.Load<GameObject>(mk + "barrel"),
                Resources.Load<GameObject>(mk + "npc/police0"),
                Resources.Load<GameObject>(mk + "npc/police1"),
                Resources.Load<GameObject>(mk + "npc/police2"),
                Resources.Load<GameObject>(mk + "npc/mother1"),
                Resources.Load<GameObject>(mk + "npc/cat0"),
                Resources.Load<GameObject>(mk + "npc/cat1"),
                Resources.Load<GameObject>(mk + "npc/salesperson0"),
                Resources.Load<GameObject>(mk + "npc/salesperson1"),
                Resources.Load<GameObject>(mk + "tutoLeftRight"),
                Resources.Load<GameObject>(mk + "npc/police3"),
                Resources.Load<GameObject>(mk + "npc/police4"),
                Resources.Load<GameObject>(mk + "npc/mother2"),
                Resources.Load<GameObject>(mk + "npc/girl0"),
                Resources.Load<GameObject>(mk + "npc/girl1"),
                Resources.Load<GameObject>(mk + "bossBall"),
                Resources.Load<GameObject>(mk + "npc/police5"),
                Resources.Load<GameObject>(mk + "npc/girl2"),
                Resources.Load<GameObject>(mk + "npc/mother3"),
            };
            elementClimbables = new List<ClimbableTile>()
            {
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.L,
                ClimbableTile.LD,
                ClimbableTile.LR,
                ClimbableTile.R,
                ClimbableTile.RD,
                ClimbableTile.D,
                ClimbableTile.LRD,
                ClimbableTile.None,
                ClimbableTile.L,
                ClimbableTile.LD,
                ClimbableTile.LR,
                ClimbableTile.R,
                ClimbableTile.RD,
                ClimbableTile.D,
                ClimbableTile.LRD,
                ClimbableTile.None,
                ClimbableTile.L,
                ClimbableTile.LD,
                ClimbableTile.LR,
                ClimbableTile.R,
                ClimbableTile.RD,
                ClimbableTile.D,
                ClimbableTile.LRD,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.L,
                ClimbableTile.LD,
                ClimbableTile.LR,
                ClimbableTile.R,
                ClimbableTile.RD,
                ClimbableTile.D,
                ClimbableTile.LRD,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
                ClimbableTile.None,
            };
            elementCategories = new List<TileCategory>()
            {
                TileCategory.Other,
                TileCategory.Other,
                TileCategory.Wall,
                TileCategory.Wall,
                TileCategory.Wall,
                TileCategory.Wall,
                TileCategory.Wall,
                TileCategory.Wall,
                TileCategory.Wall,
                TileCategory.Wall,
                TileCategory.WallIndest,
                TileCategory.WallIndest,
                TileCategory.WallIndest,
                TileCategory.WallIndest,
                TileCategory.WallIndest,
                TileCategory.WallIndest,
                TileCategory.WallIndest,
                TileCategory.WallIndest,
                TileCategory.MovingPlatform,
                TileCategory.MovingPlatform,
                TileCategory.MovingPlatform,
                TileCategory.MovingPlatform,
                TileCategory.MovingPlatform,
                TileCategory.MovingPlatform,
                TileCategory.MovingPlatform,
                TileCategory.MovingPlatform,
                TileCategory.Other,
                TileCategory.Enemy,
                TileCategory.Enemy,
                TileCategory.Enemy,
                TileCategory.Enemy,
                TileCategory.Enemy,
                TileCategory.Enemy,
                TileCategory.Enemy,
                TileCategory.Enemy,
                TileCategory.Enemy,
                TileCategory.Other,
                TileCategory.Other,
                TileCategory.NPC,
                TileCategory.WallIndest,
                TileCategory.WallIndest,
                TileCategory.WallIndest,
                TileCategory.WallIndest,
                TileCategory.WallIndest,
                TileCategory.WallIndest,
                TileCategory.WallIndest,
                TileCategory.WallIndest,
                TileCategory.NPC,
                TileCategory.Tutorial,
                TileCategory.Tutorial,
                TileCategory.Tutorial,
                TileCategory.Tutorial,
                TileCategory.Tutorial,
                TileCategory.Other,
                TileCategory.NPC,
                TileCategory.NPC,
                TileCategory.NPC,
                TileCategory.NPC,
                TileCategory.NPC,
                TileCategory.NPC,
                TileCategory.NPC,
                TileCategory.NPC,
                TileCategory.Tutorial,
                TileCategory.NPC,
                TileCategory.NPC,
                TileCategory.NPC,
                TileCategory.NPC,
                TileCategory.NPC,
                TileCategory.Enemy,
                TileCategory.NPC,
                TileCategory.NPC,
                TileCategory.NPC,
            };
        }
        level = new GameObject[WIDTH, HEIGHT];
        ClimbablePhysBox.spriteClimbables = spriteClimbables;
        UpdateButtons();
    }

    private void Update () {
//#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L)) {
            isOn = !isOn;
            canPutObject = false;
            cursor.gameObject.SetActive(isOn);
            uiParent.SetActive(isOn);
            Simulator.shouldInitPhys = true;
        }
//#endif

        if (isOn) {
            int mouseX = Mathf.Clamp(Mathf.RoundToInt(MousePosition.x), 0, WIDTH - 1);
            int mouseY = Mathf.Clamp(Mathf.RoundToInt(MousePosition.y), 0, HEIGHT - 1);
            int mouseDeltaX = 0;
            int mouseDeltaY = 0;

            cursor.position = new Vector3(mouseX, mouseY, cursor.position.z);

            if (mouseXOld != mouseX || mouseYOld != mouseY) {
                canPutObject = true;
            }

            if (!eventSystem.IsPointerOverGameObject()) {
                cursor.gameObject.SetActive(true);
                if (Input.GetMouseButton(0) && canPutObject) {
                    bool canCreateTile = (level[mouseX, mouseY] == null);
                    if (canCreateTile) {
                        for (int col = mouseX; col >= 0; col--) {
                            for (int row = mouseY; row < level.GetLength(1); row++) {
                                if (col == mouseX && row == mouseY) continue;
                                if (level[col, row] != null && level[col, row].GetComponent<ScalablePhysBox>() != null) {
                                    ScalablePhysBox scalablePhysBox = level[col, row].GetComponent<ScalablePhysBox>();
                                    if (scalablePhysBox.scaleX >= mouseX - col + 1 && scalablePhysBox.scaleY >= row - mouseY + 1) {
                                        canCreateTile = false;
                                        mouseDeltaX = mouseX - col;
                                        mouseDeltaY = -mouseY + row;
                                        mouseX = col;
                                        mouseY = row;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (canCreateTile) {
                        CreateTile(mouseX, mouseY, elementSelected);
                    }
                    SelectTile(mouseX, mouseY, mouseDeltaX, mouseDeltaY);
                    canPutObject = false;
                }
                if (Input.GetMouseButton(1) && canPutObject) {
                    if (level[mouseX, mouseY] != null) {
                        Destroy(level[mouseX, mouseY]);
                        level[mouseX, mouseY] = null;
                        if (mouseY > 0 && level[mouseX, mouseY - 1] != null && level[mouseX, mouseY - 1].GetComponent<PhysBox>() != null) {
                            PhysBox physBox = level[mouseX, mouseY - 1].GetComponent<PhysBox>();
                            if (physBox.gameElement == PhysBox.GameElement.Ladder) {
                                (physBox as HoldablePhysBox).isAlsoPlatform = true;
                            }
                            updateAlsoPlatforms = true;
                        }
                    }
                    DeselectTile();
                    canPutObject = false;
                }
            }
            else {
                cursor.gameObject.SetActive(false);
            }

            mouseXOld = mouseX;
            mouseYOld = mouseY;
        }
	}

    public void OnClickNew() {
        DestroyEverything();
        if (inputWidth.text.Length > 0 && int.Parse(inputWidth.text) > 0) {
            WIDTH = int.Parse(inputWidth.text);
        }
        if (inputHeight.text.Length > 0 && int.Parse(inputHeight.text) > 0) {
            HEIGHT = int.Parse(inputHeight.text);
        }
        level = new GameObject[WIDTH, HEIGHT];
        transform.position = new Vector3(WIDTH / 2f, HEIGHT / 2f, transform.position.z);
        CreateIndestructables();
    }

    public void OnClickSave() {
        string path = "";
#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(inputLevelSave.text)) {
            path = string.Format("{0}/Levels/{1}.txt", Application.streamingAssetsPath, inputLevelSave.text);
        }
        else {
            path = EditorUtility.SaveFilePanel(
                "Save level",
                Application.streamingAssetsPath + "/Levels",
                "0.txt",
                "txt");
        }
#else
        if (!string.IsNullOrEmpty(inputLevelSave.text)) {
            path = string.Format("{0}/Levels/{1}.txt", Application.streamingAssetsPath, inputLevelSave.text);
        }
#endif

        if (!string.IsNullOrEmpty(path)) {
            File.WriteAllBytes(path, Encoding.ASCII.GetBytes(GetLevelString()));
        }
    }

    public void OnClickLoad() {
        string path = "";
#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(inputLevelLoad.text)) {
            path = string.Format("{0}/Levels/{1}.txt", Application.streamingAssetsPath, inputLevelLoad.text);
        }
        else {
            path = EditorUtility.OpenFilePanel(
                "Load level",
                Application.streamingAssetsPath + "/Levels",
                "txt");
        }
#else
        if (!string.IsNullOrEmpty(inputLevelLoad.text)) {
            path = string.Format("{0}/Levels/{1}.txt", Application.streamingAssetsPath, inputLevelLoad.text);
        }
#endif

        if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
            string[] parts = path.Split('/', '\\');
            string levelName = parts[parts.Length - 1].Split('.')[0];
            LoadInternal(File.ReadAllText(path), levelName);
            SetTextFileText(parts[parts.Length - 1]);
        }
    }

    public void OnClickLoadPrev() {
        string path = string.Format("{0}/Levels/{1}.txt", Application.streamingAssetsPath, Platformer.LevelIndex - 1);
        if (File.Exists(path)) {
            string[] parts = path.Split('/', '\\');
            string levelName = parts[parts.Length - 1].Split('.')[0];
            LoadInternal(File.ReadAllText(path), levelName);
            SetTextFileText(parts[parts.Length - 1]);
        }
        else {
            Debug.LogErrorFormat("Level doesn't exist: {0}", Platformer.LevelIndex - 1);
        }
    }

    public void OnClickLoadNext() {
        string path = string.Format("{0}/Levels/{1}.txt", Application.streamingAssetsPath, Platformer.LevelIndex + 1);
        if (File.Exists(path)) {
            string[] parts = path.Split('/', '\\');
            string levelName = parts[parts.Length - 1].Split('.')[0];
            LoadInternal(File.ReadAllText(path), levelName);
            SetTextFileText(parts[parts.Length - 1]);
        }
        else {
            Debug.LogErrorFormat("Level doesn't exist: {0}", Platformer.LevelIndex + 1);
        }
    }

    public static string GetLevelString() {
        List<string> properties = new List<string>();
        string res = WIDTH.ToString() + "," + HEIGHT + ",";
        for (int j = 0; j < HEIGHT; j++) {
            for (int i = 0; i < WIDTH; i++) {
                if (level[i, j] != null) {
                    PhysBox physBox = level[i, j].GetComponent<PhysBox>();
                    if (physBox == null) {
                        for (int k = 0, len = instance.elements.Count; k < len; k++) {
                            if (level[i, j].name == instance.elements[k].name) {
                                res += (char)(k + ((int)'0'));
                                break;
                            }
                        }
                    }
                    else {
                        res += physBox.GetChar();
                        if (physBox.hasProperties) {
                            foreach (var p in physBox.properties) {
                                if (p is PhysBox.PropertyDropdown) {
                                    PhysBox.PropertyDropdown pCasted = (p as PhysBox.PropertyDropdown);
                                    if (pCasted.pValue != 0) {
                                        if (p.iDelta == 0 && p.jDelta == 0) {
                                            properties.Add(string.Format("{0},{1},{2},{3}", i, j, pCasted.name, pCasted.pValue));
                                        }
                                        else {
                                            properties.Add(string.Format("{0},{1},{2},{3},{4},{5}", i, j, pCasted.name, pCasted.pValue, p.iDelta, p.jDelta));
                                        }
                                    }
                                }
                                else if (p is PhysBox.PropertyFloat) {
                                    PhysBox.PropertyFloat pCasted = (p as PhysBox.PropertyFloat);
                                    if (!pCasted.defaultValues.Contains(pCasted.pValue)) {
                                        if (p.iDelta == 0 && p.jDelta == 0) {
                                            properties.Add(string.Format("{0},{1},{2},{3}", i, j, pCasted.name, pCasted.pValue.ToString("F2")));
                                        }
                                        else {
                                            properties.Add(string.Format("{0},{1},{2},{3},{4},{5}", i, j, pCasted.name, pCasted.pValue.ToString("F2"), p.iDelta, p.jDelta));
                                        }
                                    }
                                }
                                else if (p is PhysBox.PropertyInt) {
                                    PhysBox.PropertyInt pCasted = (p as PhysBox.PropertyInt);
                                    if (!pCasted.defaultValues.Contains(pCasted.pValue)) {
                                        if (p.iDelta == 0 && p.jDelta == 0) {
                                            properties.Add(string.Format("{0},{1},{2},{3}", i, j, pCasted.name, pCasted.pValue));
                                        }
                                        else {
                                            properties.Add(string.Format("{0},{1},{2},{3},{4},{5}", i, j, pCasted.name, pCasted.pValue, p.iDelta, p.jDelta));
                                        }
                                    }
                                }
                                else if (p is PhysBox.PropertyString) {
                                    PhysBox.PropertyString pCasted = (p as PhysBox.PropertyString);
                                    if (!pCasted.defaultValues.Contains(pCasted.pValue)) {
                                        if (p.iDelta == 0 && p.jDelta == 0) {
                                            properties.Add(string.Format("{0},{1},{2},{3}", i, j, pCasted.name, pCasted.pValue));
                                        }
                                        else {
                                            properties.Add(string.Format("{0},{1},{2},{3},{4},{5}", i, j, pCasted.name, pCasted.pValue, p.iDelta, p.jDelta));
                                        }
                                    }
                                }
                                else throw new System.NotImplementedException();
                            }
                        }
                    }
                }
                else {
                    res += ".";
                }
            }
        }
        res += "\nproperties";
        foreach (string s in properties) {
            res += "\n";
            res += s;
        }
        return res;
    }

    public static void Load(string levelString, string levelName) {
        try {
            instance.LoadInternal(levelString, levelName);
        }
        catch (System.Exception e) {
            Debug.LogError(e);
        }
    }

    private void LoadInternal(string levelString, string levelName) {
        bool propertiesExist = levelString.Contains("properties");
        string tileTypes = propertiesExist ? levelString.Substring(0, levelString.IndexOf("properties")) : levelString;
        string[] tileValues = tileTypes.Split(',');
        if (tileValues.Length == 3) {
            DestroyEverything();
            int width = int.Parse(tileValues[0]);
            int height = int.Parse(tileValues[1]);
            string lvl = tileValues[2];
            WIDTH = width;
            HEIGHT = height;
            transform.position = new Vector3(WIDTH / 2f, HEIGHT / 2f, transform.position.z);
            level = new GameObject[WIDTH, HEIGHT];
            for (int j = 0; j < HEIGHT; j++) {
                for (int i = 0; i < WIDTH; i++) {
                    int index = i + j * WIDTH;
                    level[i, j] = null;
                    char c = lvl[index];
                    int cInt = (int)c - ((int)'0');
                    if (cInt >= 0 && cInt < elements.Count)
                    {
                        CreateTile(i, j, cInt);
                    }
                }
            }
            for (int j = 0; j < HEIGHT - 1; j++) {
                for (int i = 0; i < WIDTH; i++) {
                    if (level[i, j] != null && level[i, j].GetComponent<PhysBox>() != null && level[i, j].GetComponent<PhysBox>().gameElement == PhysBox.GameElement.Ladder) {
                        level[i, j].GetComponent<HoldablePhysBox>().isAlsoPlatform = (level[i, j + 1] == null || level[i, j + 1].GetComponent<PhysBox>() == null || (level[i, j + 1].GetComponent<PhysBox>().kind != PhysBox.Kind.Static && level[i, j + 1].GetComponent<PhysBox>().gameElement != PhysBox.GameElement.Ladder));
                    }
                }
            }
            CreateIndestructables();
            if (propertiesExist) {
                string[] properties = levelString.Substring(levelString.IndexOf("properties") + 10).Split('\n');
                foreach (string p in properties) {
                    string[] pValues = p.Split(',');
                    if (pValues.Length == 4) {
                        int i = int.Parse(pValues[0]);
                        int j = int.Parse(pValues[1]);
                        level[i, j].GetComponent<PhysBox>().SetPropertyGeneral(pValues[2], pValues[3]);
                    }
                    else if (pValues.Length == 6) {
                        int i = int.Parse(pValues[0]);
                        int j = int.Parse(pValues[1]);
                        int iDelta = int.Parse(pValues[4]);
                        int jDelta = int.Parse(pValues[5]);
                        level[i, j].GetComponent<PhysBox>().SetPropertyGeneral(pValues[2], pValues[3], iDelta, jDelta);
                    }
                }
            }
            if (background3D != null) background3D.Create(levelName);
            int musicIndex = -1;
            if (levelName.Contains("mom")) {
                musicIndex = 0;
            }
            else if (levelName.Contains("outside") || levelName.Contains("police")) {
                musicIndex = 1;
            }
            else if (levelName.Contains("final")) {
                musicIndex = 6;
            }

            if (musicIndex == -1) {
                int levelIndex = int.MaxValue;
                int.TryParse(levelName, out levelIndex);
                if (levelIndex == 22 || levelIndex == 39 || levelIndex == 55) {
                    //boss music
                    musicIndex = 5;
                }
                else if (levelIndex < 22) {
                    musicIndex = 2;
                }
                else if (levelIndex < 39) {
                    musicIndex = 3;
                }
                else if (levelIndex < 55) {
                    musicIndex = 4;
                }
            }
            if (this.musicIndex != musicIndex) {
                this.musicIndex = musicIndex;
                if (this.musicIndex >= 0 && this.musicIndex < musicList.Length) {
                    musicPlayer.clip = musicList[this.musicIndex];
                    musicPlayer.Play();
                }
                else {
                    musicPlayer.Stop();
                }
            }
            SetTextFileText(levelName + ".txt");
            if (OnLoad != null) OnLoad(levelName);
        }
    }

    private void CreateTile(int i, int j, int elementIndex)
    {
        level[i, j] = Instantiate(elements[elementIndex], new Vector3(i, j, elements[elementIndex].transform.position.z), elements[elementIndex].transform.rotation);
        level[i, j].name = elements[elementIndex].name;
        if (level[i, j].GetComponent<PhysBox>() != null)
        {
            PhysBox physBox = level[i, j].GetComponent<PhysBox>();
            int scaleX = 1;
            if (GetComponent<ScalablePhysBox>() != null) {
                ScalablePhysBox scalable = GetComponent<ScalablePhysBox>();
                scaleX = scalable.scaleX;
            }
            physBox.col = i;
            physBox.row = j;
            physBox.tileChar = (char)(elementIndex + ((int)'0'));
            if (elementIndex < elementClimbables.Count)
            {
                if (physBox is ClimbablePhysBox) {
                    ClimbablePhysBox climbablePhys = (physBox as ClimbablePhysBox);
                    SetPhysBoxClimbable(climbablePhys, elementClimbables[elementIndex]);
                    if (i == 0) {
                        climbablePhys.isClimbableL = false;
                    }
                    else if (level[i - 1, j] != null) {
                        PhysBox physBox0 = level[i - 1, j].GetComponent<PhysBox>();
                        if (physBox0.kind == PhysBox.Kind.Static) {
                            climbablePhys.isClimbableL = false;
                        }
                        if (physBox0 is ClimbablePhysBox) {
                            ClimbablePhysBox climbablePhys0 = (physBox0 as ClimbablePhysBox);
                            climbablePhys0.isClimbableR = false;
                            SetPhysBoxClimbable(climbablePhys0, climbablePhys0.isClimbableL, climbablePhys0.isClimbableD, climbablePhys0.isClimbableR);
                            climbablePhys0.SetClimbable();
                        }
                    }

                    if (i == level.GetLength(0) - 1) {
                        climbablePhys.isClimbableR = false;
                    }
                    else if (level[i + 1, j] != null) {
                        PhysBox physBox0 = level[i + scaleX, j].GetComponent<PhysBox>();
                        if (physBox0.kind == PhysBox.Kind.Static) {
                            climbablePhys.isClimbableR = false;
                        }
                        if (physBox0 is ClimbablePhysBox) {
                            ClimbablePhysBox climbablePhys0 = (physBox0 as ClimbablePhysBox);
                            climbablePhys0.isClimbableL = false;
                            SetPhysBoxClimbable(climbablePhys0, climbablePhys0.isClimbableL, climbablePhys0.isClimbableD, climbablePhys0.isClimbableR);
                            climbablePhys0.SetClimbable();
                        }
                    }

                    if (j == 0) {
                        climbablePhys.isClimbableD = false;
                    }
                    else if (level[i, j - 1] != null) {
                        PhysBox physBox0 = level[i, j - 1].GetComponent<PhysBox>();
                        if (physBox0.kind == PhysBox.Kind.Static) {
                            climbablePhys.isClimbableD = false;
                        }
                    }

                    if (j == level.GetLength(1) - 1) {

                    }
                    else if (level[i, j + 1] != null) {
                        PhysBox physBox0 = level[i, j + 1].GetComponent<PhysBox>();
                        if (physBox0 is ClimbablePhysBox) {
                            ClimbablePhysBox climbablePhys0 = (physBox0 as ClimbablePhysBox);
                            climbablePhys0.isClimbableD = false;
                            SetPhysBoxClimbable(climbablePhys0, climbablePhys0.isClimbableL, climbablePhys0.isClimbableD, climbablePhys0.isClimbableR);
                            climbablePhys0.SetClimbable();
                        }
                    }

                    climbablePhys.SetClimbable();
                }
                else if (physBox is HoldablePhysBox) {
                    if (j > 0 && level[i, j - 1] != null) {
                        (physBox as HoldablePhysBox).isAlsoPlatform = false;
                        updateAlsoPlatforms = true;
                    }
                    
                    if (j < level.GetLength(1) - 1 && level[i, j + 1] == null) {
                        (physBox as HoldablePhysBox).isAlsoPlatform = (physBox.gameElement == PhysBox.GameElement.Ladder);
                        updateAlsoPlatforms = true;
                    }
                }
            }
        }
    }

    private void CreateIndestructables() {
        string mk = isMK ? "mk/" : "";
        GameObject prefabWallIndestructable = Resources.Load<GameObject>(mk + "wallIndestOut");
        ScalablePhysBox physBox = Instantiate(prefabWallIndestructable, new Vector3(-1, -1, 4f), Quaternion.identity).GetComponent<ScalablePhysBox>();
        physBox.name = prefabWallIndestructable.name;
        physBox.col = -1;
        physBox.row = HEIGHT;
        physBox.scaleY = HEIGHT + 2;
        physBox = Instantiate(prefabWallIndestructable, new Vector3(WIDTH, -1, 4f), Quaternion.identity).GetComponent<ScalablePhysBox>();
        physBox.name = prefabWallIndestructable.name;
        physBox.col = WIDTH;
        physBox.row = HEIGHT;
        physBox.scaleY = HEIGHT + 2;
        physBox = Instantiate(prefabWallIndestructable, new Vector3(0, -1, 4f), Quaternion.identity).GetComponent<ScalablePhysBox>();
        physBox.name = prefabWallIndestructable.name;
        physBox.col = 0;
        physBox.row = -1;
        physBox.scaleX = WIDTH;
        physBox = Instantiate(prefabWallIndestructable, new Vector3(0, HEIGHT, 4f), Quaternion.identity).GetComponent<ScalablePhysBox>();
        physBox.name = prefabWallIndestructable.name;
        physBox.col = 0;
        physBox.row = HEIGHT;
        physBox.scaleX = WIDTH;

        if (blackSides.Length > 0) {
            blackSides[0].position = new Vector3(WIDTH + 0.5f, HEIGHT * 0.5f, 5f);
            blackSides[0].eulerAngles = new Vector3(0f, 0f, 0f);
            blackSides[0].localScale = new Vector3(WIDTH * 3f, HEIGHT * 3f, 1f);
            blackSides[1].position = new Vector3(-1.5f, HEIGHT * 0.5f, 5f);
            blackSides[1].eulerAngles = new Vector3(0f, 0f, 180f);
            blackSides[1].localScale = new Vector3(WIDTH * 3f, HEIGHT * 3f, 1f);
            blackSides[2].position = new Vector3(WIDTH * 0.5f, HEIGHT + 0.5f, 5f);
            blackSides[2].eulerAngles = new Vector3(0f, 0f, 90f);
            blackSides[2].localScale = new Vector3(HEIGHT * 3f, WIDTH * 3f, 1f);
            blackSides[3].position = new Vector3(WIDTH * 0.5f, -1.5f, 5f);
            blackSides[3].eulerAngles = new Vector3(0f, 0f, -90f);
            blackSides[3].localScale = new Vector3(HEIGHT * 3f, WIDTH * 3f, 1f);
        }
    }

    private bool isStartingInspection;
    private void SelectTile(int i, int j, int iDelta, int jDelta) {
        DeselectTile();
        if (level[i, j] == null || level[i, j].GetComponent<PhysBox>() == null) {
            return;
        }
        isStartingInspection = true;
        instanceSelectedX = i;
        instanceSelectedY = j;
        instanceSelectedDeltaX = iDelta;
        instanceSelectedDeltaY = jDelta;
        inspector.SetActive(true);
        inspectorTitle.text = string.Format("{0},{1}___{2}", instanceSelectedX, instanceSelectedY, level[i, j].name);
        PhysBox physBox = level[i, j].GetComponent<PhysBox>();
        foreach (PhysBox.Property p in physBox.properties) {
            if (p.iDelta != instanceSelectedDeltaX || p.jDelta != instanceSelectedDeltaY) continue;
            switch (p.pType) {
                case PhysBox.PropertyType.Dropdown: {
                        LevelEditorDropdown element = GetDropdown();
                        element.label.text = p.name;
                        element.dropdown.value = (p as PhysBox.PropertyDropdown).pValue;
                        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                        foreach (var option in (p as PhysBox.PropertyDropdown).options) {
                            options.Add(new Dropdown.OptionData(option));
                        }
                        element.dropdown.options = options;
                        element.dropdown.value = (p as PhysBox.PropertyDropdown).pValue;
                        break;
                    }
                case PhysBox.PropertyType.Text: {
                        LevelEditorInputField element = GetInputField();
                        element.label.text = p.name;
                        element.inputField.text = (p as PhysBox.PropertyString).pValue;
                        break;
                    }
                case PhysBox.PropertyType.Number: {
                        LevelEditorInputField element = GetInputFieldNumber();
                        element.label.text = p.name;
                        if (p is PhysBox.PropertyInt) {
                            element.inputField.text = (p as PhysBox.PropertyInt).pValue.ToString();
                        }
                        else if (p is PhysBox.PropertyFloat) {
                            element.inputField.text = (p as PhysBox.PropertyFloat).pValue.ToString();
                        }
                        break;
                    }
                case PhysBox.PropertyType.Slider: {
                        LevelEditorSlider element = GetSlider();
                        if (p is PhysBox.PropertyInt) {
                            PhysBox.PropertyInt pInt = (p as PhysBox.PropertyInt);
                            element.label.text = string.Format("{0}: {1}", p.name, pInt.pValue);
                            element.slider.value = pInt.pValue;
                            element.slider.wholeNumbers = true;
                            element.slider.minValue = pInt.pMinValue;
                            element.slider.maxValue = pInt.pMaxValue;
                        }
                        else if (p is PhysBox.PropertyFloat) {
                            PhysBox.PropertyFloat pFloat = (p as PhysBox.PropertyFloat);
                            element.label.text = string.Format("{0}: {1}", p.name, pFloat.pValue);
                            element.slider.value = pFloat.pValue;
                            element.slider.wholeNumbers = false;
                            element.slider.minValue = pFloat.pMinValue;
                            element.slider.maxValue = pFloat.pMaxValue;
                        }
                        else throw new System.NotImplementedException();
                        break;
                    }
            }
        }
        isStartingInspection = false;
    }

    public void OnChangePropertyValue(LevelEditorInputField element) {
        if (isStartingInspection) return;
        PhysBox physBox = level[instanceSelectedX, instanceSelectedY].GetComponent<PhysBox>();
        if (element.inputField.text == "") physBox.SetPropertyNull(element.label.text.Split(':')[0]);
        else physBox.SetProperty(element.label.text.Split(':')[0], element.inputField.text, instanceSelectedDeltaX, instanceSelectedDeltaY);
    }

    public void OnChangePropertyValue(LevelEditorDropdown element) {
        if (isStartingInspection) return;
        PhysBox physBox = level[instanceSelectedX, instanceSelectedY].GetComponent<PhysBox>();
        physBox.SetProperty(element.label.text.Split(':')[0], element.dropdown.value, instanceSelectedDeltaX, instanceSelectedDeltaY);
    }

    public void OnChangePropertyValue(LevelEditorSlider element) {
        if (isStartingInspection) return;
        PhysBox physBox = level[instanceSelectedX, instanceSelectedY].GetComponent<PhysBox>();
        if (element.slider.wholeNumbers) {
            physBox.SetProperty(element.label.text.Split(':')[0], Mathf.RoundToInt(element.slider.value), instanceSelectedDeltaX, instanceSelectedDeltaY);
        }
        else {
            physBox.SetProperty(element.label.text.Split(':')[0], element.slider.value, instanceSelectedDeltaX, instanceSelectedDeltaY);
        }
    }

    private void DeselectTile() {
        instanceSelectedX = -1;
        instanceSelectedY = -1;
        inspector.SetActive(false);
        foreach (var element in inspectorDropdowns) element.gameObject.SetActive(false);
        foreach (var element in inspectorInputFieldNumbers) element.gameObject.SetActive(false);
        foreach (var element in inspectorInputFields) element.gameObject.SetActive(false);
        foreach (var element in inspectorSliders) element.gameObject.SetActive(false);
    }

    private LevelEditorDropdown GetDropdown() {
        foreach (var element in inspectorDropdowns) {
            if (!element.gameObject.activeSelf) {
                element.gameObject.SetActive(true);
                return element;
            }
        }
        throw new System.Exception("Cannot find suitable element");
    }

    private LevelEditorInputField GetInputField() {
        foreach (var element in inspectorInputFields) {
            if (!element.gameObject.activeSelf) {
                element.gameObject.SetActive(true);
                return element;
            }
        }
        throw new System.Exception("Cannot find suitable element");
    }

    private LevelEditorInputField GetInputFieldNumber() {
        foreach (var element in inspectorInputFieldNumbers) {
            if (!element.gameObject.activeSelf) {
                element.gameObject.SetActive(true);
                return element;
            }
        }
        throw new System.Exception("Cannot find suitable element");
    }

    private LevelEditorSlider GetSlider() {
        foreach (var element in inspectorSliders) {
            if (!element.gameObject.activeSelf) {
                element.gameObject.SetActive(true);
                return element;
            }
        }
        throw new System.Exception("Cannot find suitable element");
    }

    private static void DestroyEverything() {
        if (level != null) {
            for (int j = 0; j < HEIGHT; j++) {
                for (int i = 0; i < WIDTH; i++) {
                    if (level[i, j] != null)
                        Destroy(level[i, j]);
                }
            }
        }

        object[] all = Object.FindObjectsOfType(typeof(GameObject));
        foreach (object o in all) {
            GameObject g = (GameObject)o;
            PhysBox physObj = g.GetComponent<PhysBox>();
            if (physObj != null && physObj.transform.parent == null) {
                Destroy(g);
            }
        }
    }

    private void OnApplicationQuit() {
        File.WriteAllBytes(Application.streamingAssetsPath + "/Levels/temp.txt", Encoding.ASCII.GetBytes(GetLevelString()));
    }

    private void SetTextFileText(string fileName)
    {
        textFile.text = "Opened: " + fileName + "\n" + level.GetLength(0) + "x" + level.GetLength(1);
    }

    public static void SetPhysBoxClimbable(ClimbablePhysBox physBox, ClimbableTile c)
    {
        switch (c)
        {
            case ClimbableTile.None:
                {
                    physBox.isClimbableL = false;
                    physBox.isClimbableD = false;
                    physBox.isClimbableR = false;
                    break;
                }
            case ClimbableTile.L:
                {
                    physBox.isClimbableL = true;
                    physBox.isClimbableD = false;
                    physBox.isClimbableR = false;
                    break;
                }
            case ClimbableTile.LD:
                {
                    physBox.isClimbableL = true;
                    physBox.isClimbableD = true;
                    physBox.isClimbableR = false;
                    break;
                }
            case ClimbableTile.LR:
                {
                    physBox.isClimbableL = true;
                    physBox.isClimbableD = false;
                    physBox.isClimbableR = true;
                    break;
                }
            case ClimbableTile.R:
                {
                    physBox.isClimbableL = false;
                    physBox.isClimbableD = false;
                    physBox.isClimbableR = true;
                    break;
                }
            case ClimbableTile.RD:
                {
                    physBox.isClimbableL = false;
                    physBox.isClimbableD = true;
                    physBox.isClimbableR = true;
                    break;
                }
            case ClimbableTile.D:
                {
                    physBox.isClimbableL = false;
                    physBox.isClimbableD = true;
                    physBox.isClimbableR = false;
                    break;
                }
            case ClimbableTile.LRD:
                {
                    physBox.isClimbableL = true;
                    physBox.isClimbableD = true;
                    physBox.isClimbableR = true;
                    break;
                }
        }
    }

    public static void SetPhysBoxClimbable(ClimbablePhysBox physBox, bool isClimbableL, bool isClimbableD, bool isClimbableR)
    {
        physBox.isClimbableL = isClimbableL;
        physBox.isClimbableD = isClimbableD;
        physBox.isClimbableR = isClimbableR;
    }

    private void UpdateButtons() {
        if (categorySelected == -1) {
            for (int i = 0, len = (int)TileCategory.NumCategories; i < len; i++) {
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].GetComponentInChildren<Text>().text = "C: " + ((TileCategory)i).ToString();
                int ic = i;
                buttons[i].onClick.AddListener(delegate {
                    categorySelected = ic;
                    UpdateButtons();
                });
            }
            for (int i = 0, len = buttons.Length; i < len; i++) {
                buttons[i].gameObject.SetActive(i < (int)TileCategory.NumCategories);
            }
        }
        else {
            int buttonIndex = 0;
            buttons[buttonIndex].onClick.RemoveAllListeners();
            buttons[buttonIndex].GetComponentInChildren<Text>().text = "<-";
            buttons[buttonIndex].onClick.AddListener(delegate {
                categorySelected = -1;
                UpdateButtons();
            });
            buttonIndex++;
            List<int> currentElements = new List<int>();
            for (int i = 0, len = elements.Count; i < len; i++) {
                if (elementCategories[i] == (TileCategory)categorySelected) {
                    currentElements.Add(i);
                }
            }
            for (int i = 0, len = currentElements.Count; i < len; i++) {
                int ic = currentElements[i];
                buttons[buttonIndex].onClick.RemoveAllListeners();
                buttons[buttonIndex].onClick.AddListener(delegate {
                    elementSelected = ic;
                });
                buttons[buttonIndex].GetComponentInChildren<Text>().text = elements[ic].name + (elementClimbables.Count > ic && elementClimbables[ic] != ClimbableTile.None ? " " + elementClimbables[ic].ToString() : "");
                buttonIndex++;
            }
            for (int i = 0, len = buttons.Length; i < len; i++) {
                buttons[i].gameObject.SetActive(i < buttonIndex);
            }
        }
    }

    public static void OnBossKilled() {
        instance.musicPlayer.Stop();
    }

    public static void OnAchieveEnding() {
        instance.musicPlayer.Stop();
    }
}

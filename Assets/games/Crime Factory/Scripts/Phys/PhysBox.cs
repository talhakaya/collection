using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Games.CrimeFactory
{
	public class PhysBox : MonoBehaviour {
	    public enum Kind {
	        Static,
	        Dynamic,
	        Trigger,
	        DynamicSolid
	    }
	    public enum GameElement {
	        Wall,
	        Player,
	        EnemyLaser,
	        MovingPlatform,
	        Ladder,
	        FireEnemy,
	        FirePlayer,
	        Bomb,
	        Explosion,
	        Indestructable,
	        Door,
	        BombBox,
	        Hanger,
	        EnemyCritter,
	        EnemyJumper,
	        EnemyPistol,
	        EnemyBat,
	        EnemyDiagonal,
	        Spike,
	        BossRoot,
	        BossVulnerable,
	        SpikeSolid,
	        BossRootSpider,
	        NPC,
	        Money,
	        Barrel
	    }
	    public Kind kind;
	    public GameElement gameElement;
	    [HideInInspector] public int col;
	    [HideInInspector] public int row;
	    public PhysBox parent;
	    public PhysBody physBody;
	    public PhysNode physNode;
	    public Rectangle rect;
	    public float widthInit = 1f;
	    public float heightInit = 1f;
	    [HideInInspector] public Vector2 vel;
	    public Vector2 visualPosOffset;
	    public float visualScale = 1f;
	    protected bool updateScale;
	    public bool hasOutlines = true;
	    protected bool forceOutlineUpdate;
	    private List<SpriteRenderer> outlines;
	    private SpriteRenderer sr;
	    private AnimBehavior anim;
	    public bool isRight;
	    public float friction = 100f;
	    public float gravityMult = 1f;
	    [HideInInspector] public List<PhysBox> bossVulnerablePoints;
	    [HideInInspector] public Vector2 lastPos;
	    [HideInInspector] public PhysBox groundIfBody;
	    [HideInInspector] public char tileChar;
	    public float MaxSpeedHor = 6.5f;
	    public float MaxSpeedVer = 15f;
	    public bool simplifiedPhysics;

	    private void Awake() {
	        rect = new Rectangle(transform.position.x, transform.position.y, widthInit, heightInit);
	    }

	    private void Start() {
	        Init();
	    }

	    protected virtual void Init() {
	        rect.x = transform.position.x;
	        rect.y = transform.position.y;
	        sr = GetComponent<SpriteRenderer>();
	        anim = GetComponent<AnimBehavior>();
	        if (hasOutlines && sr != null) {
	            outlines = new List<SpriteRenderer>();
	            for (int i = 0; i < 4; i++) {
	                GameObject g = new GameObject("outline");
	                outlines.Add(g.AddComponent<SpriteRenderer>());
	                outlines[i].transform.SetParent(transform);
	                outlines[i].transform.localScale = new Vector3(1f, 1f, 1f);
	                outlines[i].transform.localEulerAngles = new Vector3(0f, 0f, 0f);
	                outlines[i].transform.localPosition = new Vector3(0f, 0f, 0.1f);
	                outlines[i].color = new Color(0f, 0f, 0f, 1f);
	                outlines[i].sprite = sr.sprite;
	            }
	            outlines[0].transform.position = transform.position + new Vector3(Platformer.OutlineDist, Platformer.OutlineDist, 0.1f);
	            outlines[1].transform.position = transform.position + new Vector3(Platformer.OutlineDist, -Platformer.OutlineDist, 0.1f);
	            outlines[2].transform.position = transform.position + new Vector3(-Platformer.OutlineDist, Platformer.OutlineDist, 0.1f);
	            outlines[3].transform.position = transform.position + new Vector3(-Platformer.OutlineDist, -Platformer.OutlineDist, 0.1f);
	        }
	    }

	    private void Update() {
	        transform.position = new Vector3(rect.x + visualPosOffset.x, rect.y + visualPosOffset.y, transform.position.z);
	        if (updateScale) {
	            transform.localScale = new Vector3(visualScale, visualScale, 1f);
	        }
	        else {
	            updateScale = (visualScale != 1f);
	        }
	    }

	    private void LateUpdate() {
	        if (hasOutlines && sr != null && (anim != null || forceOutlineUpdate)) {
	            for (int i = 0; i < 4; i++) {
	                outlines[i].sprite = sr.sprite;
	            }
	            outlines[0].transform.position = transform.position + new Vector3(Platformer.OutlineDist, Platformer.OutlineDist, 0.1f);
	            outlines[1].transform.position = transform.position + new Vector3(Platformer.OutlineDist, -Platformer.OutlineDist, 0.1f);
	            outlines[2].transform.position = transform.position + new Vector3(-Platformer.OutlineDist, Platformer.OutlineDist, 0.1f);
	            outlines[3].transform.position = transform.position + new Vector3(-Platformer.OutlineDist, -Platformer.OutlineDist, 0.1f);
	        }
	    }

	    public bool Overlaps(PhysBox physObj, float forgive = 0f) {
	        return rect.Overlaps(physObj.rect, forgive);
	    }

	    public char GetChar() {
	        return tileChar;
	    }

	    public List<PhysBox> GetChildren(List<PhysBox> all) {
	        List<PhysBox> res = new List<PhysBox>();
	        foreach (PhysBox physObj in all) {
	            if (physObj.parent == this) {
	                res.Add(physObj);
	            }
	        }
	        return res;
	    }

	    public Vector2 GetBodyVel() {
	        Vector2 bodyVel = vel;
	        PhysBox p = parent;
	        while (p != null) {
	            bodyVel += p.vel;
	            p = p.parent;
	        }
	        return bodyVel;
	    }

	    public Rectangle GetClimbRect(ClimbableTile c) {
	        Rectangle res;
	        switch (c) {
	            case ClimbableTile.L:
	                res = new Rectangle(rect.x - rect.w * 0.5f, rect.y + 0.1f, 0.3f, rect.h);
	                break;
	            case ClimbableTile.D:
	                res = new Rectangle(rect.x, rect.y - rect.h * 0.5f, rect.w - 0.2f, 0.3f);
	                break;
	            case ClimbableTile.R:
	                res = new Rectangle(rect.x + rect.w * 0.5f, rect.y + 0.1f, 0.3f, rect.h);
	                break;
	            default:
	                throw new System.EntryPointNotFoundException();
	        }
	        return res;
	    }

	    private List<Property> _properties;
	    public List<Property> properties {
	        get {
	            if (_properties == null) _properties = GetPropertyTypes();
	            return _properties;
	        }
	    }
	    public bool hasProperties {
	        get {
	            return _properties != null;
	        }
	    }

	    public class Property {
	        public string name;
	        public PropertyType pType;
	        public int iDelta;
	        public int jDelta;
	        public static Dictionary<string, Property> allTypes = new Dictionary<string, Property>() {
	            {PROP_SCALE_X, new PropertyInt() {
	                name = PROP_SCALE_X,
	                pType = PropertyType.Slider,
	                defaultValues = new List<int>() {1},
	                pValue = 1,
	                pMinValue = 1,
	                pMaxValue = 16
	            }},
	            {PROP_SCALE_Y, new PropertyInt() {
	                name = PROP_SCALE_Y,
	                pType = PropertyType.Slider,
	                defaultValues = new List<int>() {1},
	                pValue = 1,
	                pMinValue = 1,
	                pMaxValue = 16
	            }},
	            {PROP_SPEED, new PropertyFloat() {
	                name = PROP_SPEED,
	                pType = PropertyType.Slider,
	                defaultValues = new List<float>() {Simulator.MOVING_PLATFORM_SPEED},
	                pValue = 1,
	                pMinValue = 0.1f,
	                pMaxValue = 10f
	            }},
	            {PROP_DIR, new PropertyDropdown() {
	                name = PROP_DIR,
	                pType = PropertyType.Dropdown,
	                options = new List<string>() {
	                    "LEFT&RIGHT",
	                    "UP&DOWN"
	                },
	                pValue = 0,
	            }},
	            {PROP_NEXT_LEVEL, new PropertyString() {
	                name = PROP_NEXT_LEVEL,
	                pType = PropertyType.Text,
	                defaultValues = new List<string>() {
	                    ""
	                },
	                pValue = "",
	            }},
	            {PROP_SPRITE, new PropertyInt() {
	                name = PROP_SPRITE,
	                pType = PropertyType.Slider,
	                defaultValues = new List<int>() {0},
	                pValue = 0,
	                pMinValue = 0,
	                pMaxValue = 15
	            }},
	            {PROP_SPRITE_CLIMBABLE, new PropertyInt() {
	                name = PROP_SPRITE_CLIMBABLE,
	                pType = PropertyType.Slider,
	                defaultValues = new List<int>() {0},
	                pValue = 0,
	                pMinValue = 0,
	                pMaxValue = 15
	            }},
	        };
	        public static Property GetCopy(string name) {
	            Property toCopy = allTypes[name];
	            if (toCopy is PropertyDropdown) {
	                PropertyDropdown toCopyCasted = (toCopy as PropertyDropdown);
	                return new PropertyDropdown() {
	                    name = toCopyCasted.name,
	                    options = toCopyCasted.options,
	                    pType = toCopyCasted.pType,
	                    pValue = toCopyCasted.pValue
	                };
	            }
	            else if (toCopy is PropertyFloat) {
	                PropertyFloat toCopyCasted = (toCopy as PropertyFloat);
	                return new PropertyFloat() {
	                    name = toCopyCasted.name,
	                    defaultValues = toCopyCasted.defaultValues,
	                    pType = toCopyCasted.pType,
	                    pValue = toCopyCasted.pValue,
	                    pMaxValue = toCopyCasted.pMaxValue,
	                    pMinValue = toCopyCasted.pMinValue
	                };
	            }
	            else if (toCopy is PropertyInt) {
	                PropertyInt toCopyCasted = (toCopy as PropertyInt);
	                return new PropertyInt() {
	                    name = toCopyCasted.name,
	                    defaultValues = toCopyCasted.defaultValues,
	                    pType = toCopyCasted.pType,
	                    pValue = toCopyCasted.pValue,
	                    pMaxValue = toCopyCasted.pMaxValue,
	                    pMinValue = toCopyCasted.pMinValue
	                };
	            }
	            else if (toCopy is PropertyString) {
	                PropertyString toCopyCasted = (toCopy as PropertyString);
	                return new PropertyString() {
	                    name = toCopyCasted.name,
	                    defaultValues = toCopyCasted.defaultValues,
	                    pType = toCopyCasted.pType,
	                    pValue = toCopyCasted.pValue
	                };
	            }
	            else throw new System.NotImplementedException();
	        }
	    }

	    public class PropertyInt : Property {
	        public int pValue;
	        public int pMinValue = -1;//-1 means no min
	        public int pMaxValue = -1;//-1 means no max
	        public List<int> defaultValues;
	    }

	    public class PropertyFloat : Property {
	        public float pValue;
	        public float pMinValue = -1f;//-1 means no min
	        public float pMaxValue = -1f;//-1 means no max
	        public List<float> defaultValues;
	    }

	    public class PropertyString : Property {
	        public string pValue;
	        public List<string> defaultValues;
	    }

	    public class PropertyDropdown : Property {
	        public int pValue;
	        public List<string> options;
	    }

	    public enum PropertyType {
	        Text,
	        Number,
	        Slider,
	        Dropdown
	    }

	    public void SetPropertyNull(string name) {
	        foreach (var p in properties) {
	            if (p.name == name) {
	                if (p is PropertyString) (p as PropertyString).pValue = (p as PropertyString).defaultValues[0];
	                else if (p is PropertyInt) (p as PropertyInt).pValue = (p as PropertyInt).defaultValues[0];
	                else if (p is PropertyFloat) (p as PropertyFloat).pValue = (p as PropertyFloat).defaultValues[0];
	                else if (p is PropertyDropdown) (p as PropertyDropdown).pValue = 0;
	                else throw new System.NotImplementedException();
	                return;
	            }
	        }
	    }

	    public void SetPropertyGeneral(string name, string val, int iDelta = 0, int jDelta = 0) {
	        int intVal;
	        if (int.TryParse(val, out intVal)) {
	            SetProperty(name, intVal, iDelta, jDelta);
	            return;
	        }
	        float floatVal;
	        if (float.TryParse(val, out floatVal)) {
	            SetProperty(name, floatVal, iDelta, jDelta);
	            return;
	        }
	        SetProperty(name, val, iDelta, jDelta);
	    }

	    public virtual void SetProperty(string name, string val, int iDelta = 0, int jDelta = 0) {
	        foreach (var p in properties) {
	            if (p.name == name && p.iDelta == iDelta && p.jDelta == jDelta) {
	                if (p is PropertyString) (p as PropertyString).pValue = val;
	                else if (p is PropertyInt) (p as PropertyInt).pValue = int.Parse(val);
	                else if (p is PropertyFloat) (p as PropertyFloat).pValue = float.Parse(val);
	                else throw new System.ArrayTypeMismatchException();
	                return;
	            }
	        }
	    }

	    public virtual void SetProperty(string name, float val, int iDelta = 0, int jDelta = 0) {
	        foreach (var p in properties) {
	            if (p.name == name && p.iDelta == iDelta && p.jDelta == jDelta) {
	                if (p is PropertyString) (p as PropertyString).pValue = val.ToString();
	                else if (p is PropertyFloat) (p as PropertyFloat).pValue = val;
	                else if (p is PropertyInt) (p as PropertyInt).pValue = Mathf.RoundToInt(val);
	                else throw new System.ArrayTypeMismatchException();
	                return;
	            }
	        }
	    }

	    public virtual void SetProperty(string name, int val, int iDelta = 0, int jDelta = 0) {
	        foreach (var p in properties) {
	            if (p.name == name && p.iDelta == iDelta && p.jDelta == jDelta) {
	                if (p is PropertyString) (p as PropertyString).pValue = val.ToString();
	                else if (p is PropertyInt) (p as PropertyInt).pValue = val;
	                else if (p is PropertyFloat) (p as PropertyFloat).pValue = val;
	                else if (p is PropertyDropdown) (p as PropertyDropdown).pValue = val;
	                else throw new System.ArrayTypeMismatchException();
	                return;
	            }
	        }
	    }

	    protected virtual List<Property> GetPropertyTypes() {
	        return new List<Property>();
	    }

	    protected virtual void UpdateProperties() {

	    }

	    public const string PROP_SCALE_X = "Scale X";
	    public const string PROP_SCALE_Y = "Scale Y";
	    public const string PROP_SPEED = "Speed";
	    public const string PROP_DIR = "Dir";
	    public const string PROP_NEXT_LEVEL = "Next Level";
	    public const string PROP_SPRITE = "Sprite";
	    public const string PROP_SPRITE_CLIMBABLE = "SpriteClimbable";
	}
}

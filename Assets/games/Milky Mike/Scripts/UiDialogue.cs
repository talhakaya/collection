using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Games.MilkyMike
{
	public class UiDialogue : MonoBehaviour {
	    private CanvasGroup canvasGroup;
	    [SerializeField] private Image triangle;
	    [SerializeField] private Image profilePic;
	    [SerializeField] private Text textTitle;
	    [SerializeField] private Text textMain;
	    [SerializeField] private Image imageContinue;
	    private string main;
	    private float timerAlpha;
	    private float periodAlpha;
	    private float timerText;
	    private float periodText;
	    [HideInInspector] public bool isActive;
	    [HideInInspector] public bool shouldZoom;
	    private GameObject[] people;
	    private string[] mains;
	    private int dialogueIndex;
	    private bool inputBecameFalse;
	    private float triangleScaleX;
	    private float triangleRot;

	    void Awake() {
	        canvasGroup = GetComponent<CanvasGroup>();
	        canvasGroup.alpha = 0f;
	        periodAlpha = 0.35f;
	    }

	    void Update() {
	        imageContinue.enabled = false;
	        if (isActive) {
	            if (shouldZoom) {
	                triangleScaleX = 0f;
	            }
	            else {
	                Vector3 triGoal = GetCameraPos() - Camera.main.ScreenToWorldPoint(triangle.rectTransform.position);
	                triangleRot = Geometry.angleOfVector3(triGoal);
	                triangleScaleX = Geometry.lengthOfVector3(triGoal) / Camera.main.orthographicSize;// - 0.75f;
	            }

	            if (timerAlpha < periodAlpha) {
	                timerAlpha += Time.deltaTime;
	                if (timerAlpha < periodAlpha) {
	                    canvasGroup.alpha = timerAlpha / periodAlpha;
	                }
	                else {
	                    canvasGroup.alpha = 1f;
	                }
	            }
	            else if (timerText < periodText) {
	                timerText += Time.deltaTime;
	                if (timerText < periodText) {
	                    textMain.text = SubStringRichText(main, Mathf.FloorToInt(main.Length * timerText / periodText));
	                }
	                else {
	                    textMain.text = main;
	                    inputBecameFalse = false;
	                }
	            }
	            else {
	                if (!Player.instance.fireInput) {
	                    inputBecameFalse = true;
	                }
	                if (inputBecameFalse && Player.instance.fireInput) {
	                    SetDialogue(++dialogueIndex);
	                }
	                imageContinue.enabled = true;
	            }
	        }
	        else {
	            if (timerAlpha > 0f) {
	                timerAlpha -= Time.deltaTime;
	                if (timerAlpha > 0f) {
	                    canvasGroup.alpha = timerAlpha / periodAlpha;
	                }
	                else {
	                    canvasGroup.alpha = 0f;
	                }
	            }
	        }
	        triangle.transform.localEulerAngles = new Vector3(0f, 0f, triangle.transform.localEulerAngles.z + Geometry.differenceOfAnglesNegative(triangleRot, triangle.transform.localEulerAngles.z) * Game.dt * 10f);
	        triangle.transform.localScale = new Vector3(triangle.transform.localScale.x + (triangleScaleX - triangle.transform.localScale.x) * Game.dt * 10f, 0.9f + 0.2f * Game.Rhythm(), 1f);
	    }

	    private void SetDialogue(int index) {
	        if (index < people.Length) {
	            DialogueData dd = people[index].GetComponent<DialogueData>();
	            SetDialogue(dd.pp, dd.title, mains[index]);
	        }
	        else {
	            isActive = false;
	            triangleScaleX = 0f;
	        }
	    }

	    private void SetDialogue(Sprite pp, string title, string main) {
	        profilePic.sprite = pp;
	        profilePic.SetNativeSize();
	        profilePic.transform.localScale = new Vector3(1f, 1f, 1f) * 150f / Mathf.Max(profilePic.rectTransform.sizeDelta.x, profilePic.rectTransform.sizeDelta.y);
	        textTitle.text = title + ":";
	        textMain.text = "";
	        this.main = ProcessString(main);
	        timerText = 0f;
	        periodText = main.Length * 0.02f;
	    }

	    public void StartDialogue(GameObject[] people, string[] mains) {
	        dialogueIndex = 0;
	        this.people = people;
	        this.mains = mains;
	        isActive = true;
	        SetDialogue(0);
	    }

	    private static string SubStringRichText(string s, int no) {
	        return s.Substring(0, no) + "<color=#00000000>" + s.Substring(no, s.Length - no) + "</color>";
	    }

	    public Vector3 GetCameraPos() {
	        if (!isActive) throw new System.NotSupportedException();

	        DialogueData dd = people[dialogueIndex].GetComponent<DialogueData>();
	        return dd.transform.TransformPoint(dd.cameraOffset);
	    }

	    public string ProcessString(string main) {
	        string res = main;
	        int index = res.IndexOf("_name");
	        while (index != -1) {
	            res = res.Replace("_name", dialogueIndex == 0 ? people[1].GetComponent<DialogueData>().title : people[dialogueIndex - 1].GetComponent<DialogueData>().title);
	            index = res.IndexOf("_name");
	        }
	        float amount = 0f;
	        res = GetValueRemoveTag(res, "addMilk", out amount);
	        if (amount != 0f) {
	            Game.instance.milk += amount;
	        }
	        res = GetValueRemoveTag(res, "addMoney", out amount);
	        if (amount != 0f) {
	            Game.instance.moneyTotal += Mathf.RoundToInt(amount);
	        }
	        return res;
	    }

	    string GetValueRemoveTag(string main, string tag, out float amount) {
	        string res = main;
	        string startTag = "[" + tag + "=";
	        int startIndex = res.IndexOf(startTag);
	        if (startIndex == -1) {
	            amount = 0;
	            return main;
	        }
	        else {
	            string endTag = "]";
	            int endIndex = res.IndexOf(endTag, startIndex);
	            if (endIndex == -1) {
	                Debug.LogError("WTFFFFF");
	                amount = 0;
	                return main;
	            }
	            else {
	                string amountString = res.Substring((startIndex + startTag.Length), endIndex - (startIndex + startTag.Length));
	                float.TryParse(amountString, out amount);
	                return main.Remove(startIndex, endIndex + endTag.Length - startIndex);
	            }
	        }
	    }
	}
}

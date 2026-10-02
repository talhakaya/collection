using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {
    public enum State {
        None,
        WalkFirst,
        BubbleAppearing,
        LineAppearing,
        LineStanding,
        WaitTime,
        BubbleDisappearing,
        WalkLast,
    }
    public enum Char {
        None,
        Player,
        Mom,
        Girl,
        Chief,
        Cat,
        Salesperson0,
        Salesperson1,
    }
    public float inputFirst;
    public float timeFirst = 0.5f;
    public bool autoTalk;
    public bool triggerEnding;
    public float rectWidth = 4f;
    public float rectHeight = 1f;
    public Char charKey;
    public string[] lines;
    public float[] waitTimes;
    public Vector2[] input;
    public bool[] playerTalks;
    [HideInInspector] public bool dontTalkAgain;
    [HideInInspector] public State state;
    [HideInInspector] public int lineCount;
    [HideInInspector] public CharacterPhysBox physBox;
    [HideInInspector] public bool isPlayerTalking;
    private float timer;
    private bool inputNext;
    private bool inputNextOld;
    private Salesperson salesperson;

    void Start () {
		
	}

    private void OnDisable() {
        if (state != State.None) {
            TalkUI.instance.Explode();
        }
    }

    public void UpdateNPC(float dt) {
        inputNext = Input.GetButton("Jump") || Input.GetButton("Fire1") || Input.GetAxis("FireAxis") < -0.5f;

        switch (state) {
            case State.None:
                timer = 0f;
                lineCount = 0;
                break;
            case State.WalkFirst:
                timer += dt;
                if (timer < timeFirst) {
                    physBox.input = new Vector2(inputFirst, 0f);
                }
                else {
                    timer = 0f;
                    physBox.input = new Vector2(0f, 0f);
                    state = State.BubbleAppearing;
                    TalkUISizeReset();
                    isPlayerTalking = lineCount < playerTalks.Length ? playerTalks[lineCount] : false;
                }
                break;
            case State.BubbleAppearing:
                timer += dt;
                if (timer < 0.2f) {
                    TalkUI.instance.imageTail.transform.localScale = new Vector3(1f, Easing.SineEaseOut(timer, 0f, 1f, 0.2f), 1f);
                    if (timer < 0.1f) {
                        TalkUI.instance.imageBubble.transform.localScale = new Vector3(1f, 0f, 1f);
                    }
                    else {
                        TalkUI.instance.imageBubble.transform.localScale = new Vector3(1f, Easing.SineEaseOut(timer - 0.1f, 0f, 1f, 0.1f), 1f);
                    }
                    TalkUI.instance.canvasGroup.alpha = Mathf.Min(1f, timer * 4f);
                    TalkUI.instance.text.text = "";
                }
                else {
                    TalkUI.instance.imageTail.transform.localScale = new Vector3(1f, 1f, 1f);
                    TalkUI.instance.imageBubble.transform.localScale = new Vector3(1f, 1f, 1f);
                    timer = 0f;
                    state = State.LineAppearing;
                    TalkUI.instance.canvasGroup.alpha = 1f;
                    TalkUI.instance.text.text = "";
                }
                break;
            case State.LineAppearing:
                timer += inputNext ? 5f * dt : dt;
                int numChars = Localization.Get(lines[lineCount]).Length;
                float timePerChar = 0.03f;
                if (timer < timePerChar * numChars) {
                    TalkUI.instance.text.text = SubStringRichText(Localization.Get(lines[lineCount]), (int) (timer/timePerChar));
                }
                else {
                    TalkUI.instance.text.text = Localization.Get(lines[lineCount]);
                    timer = 0f;
                    state = State.LineStanding;
                    if (lineCount == lines.Length - 1) {
                        salesperson = GetComponent<Salesperson>();
                        if (salesperson != null) {
                            salesperson.StartSelling();
                            if (salesperson.state == Salesperson.State.NotEnoughMoney) {
                                TalkUI.instance.text.text += "\n" + Localization.Get("NOT_ENOUGH_MONEY");
                            }
                            else if (salesperson.state == Salesperson.State.OutOfStock) {
                                TalkUI.instance.text.text += "\n" + Localization.Get("OUT_OF_STOCK");
                            }
                        }
                    }
                }
                break;
            case State.LineStanding:
                bool moveToNextState = false;
                if (salesperson != null && salesperson.state == Salesperson.State.Selling) {
                    if (inputNext && !inputNextOld) {
                        salesperson.Buy();
                        moveToNextState = true;
                    }
                    else if (Input.GetButton("Fire2")) {
                        salesperson.Next();
                        moveToNextState = true;
                    }
                }
                else if (inputNext && !inputNextOld) {
                    if (salesperson != null) salesperson.Next();
                    moveToNextState = true;
                }
                if (moveToNextState) {
                    if (lineCount < waitTimes.Length && waitTimes[lineCount] > 0f) {
                        TalkUI.instance.text.text = "";
                        timer = 0f;
                        state = State.WaitTime;
                    }
                    else if (lineCount < lines.Length - 1) {
                        bool isPlayerTalkingNew = lineCount + 1 < playerTalks.Length ? playerTalks[lineCount + 1] : false;
                        if (isPlayerTalking == isPlayerTalkingNew) {
                            lineCount++;
                            state = State.LineAppearing;
                        }
                        else {
                            state = State.BubbleDisappearing;
                        }
                    }
                    else {
                        state = State.BubbleDisappearing;
                    }
                }
                break;
            case State.WaitTime:
                timer += dt;
                float wait = waitTimes[lineCount];
                if (timer < wait) {
                    if (lineCount < input.Length) {
                        physBox.input = input[lineCount];
                        physBox.inputJump = input[lineCount].y > 0f;
                        if (physBox.input.x != 0f) {
                            physBox.isRight = physBox.input.x > 0f;
                        }
                    }

                    float waitHalf = 0.1f;
                    if (wait < 0.2f) {
                        waitHalf = wait * 0.5f;
                    }
                    if (timer < waitHalf) {
                        TalkUI.instance.canvasGroup.alpha = 1f - (timer / waitHalf);
                        TalkUI.instance.imageTail.transform.localScale = new Vector3(1f, Easing.SineEaseOut(timer, 1f, -1f, waitHalf), 1f);
                    }
                    else if (timer > wait - waitHalf) {
                        isPlayerTalking = lineCount + 1 < playerTalks.Length ? playerTalks[lineCount + 1] : false;
                        TalkUI.instance.canvasGroup.alpha = ((timer - wait + waitHalf) / waitHalf);
                        TalkUI.instance.imageTail.transform.localScale = new Vector3(1f, Easing.SineEaseOut((timer - wait + waitHalf), 0f, 1f, waitHalf), 1f);
                        TalkUISizeReset();
                    }
                    else {
                        isPlayerTalking = lineCount + 1 < playerTalks.Length ? playerTalks[lineCount + 1] : false;
                        TalkUI.instance.canvasGroup.alpha = 0f;
                        TalkUI.instance.imageTail.transform.localScale = new Vector3(1f, 0f, 1f);
                        TalkUISizeReset();
                    }
                }
                else {
                    timer = 0f;
                    TalkUI.instance.canvasGroup.alpha = 1f;
                    TalkUI.instance.imageTail.transform.localScale = new Vector3(1f, 1f, 1f);
                    physBox.input.x = 0f;
                    physBox.input.y = 0f;
                    physBox.inputJump = false;
                    if (lineCount < lines.Length - 1) {
                        lineCount++;
                        state = State.LineAppearing;
                    }
                    else {
                        state = State.BubbleDisappearing;
                    }
                }
                break;
            case State.BubbleDisappearing:
                timer += dt;
                if (timer < 0.15f) {
                    TalkUI.instance.imageTail.transform.localScale = new Vector3(1f, 1f - Easing.SineEaseIn(timer, 0f, 1f, 0.15f), 1f);
                    if (timer > 0.05f) {
                        TalkUI.instance.imageBubble.transform.localScale = new Vector3(1f, 0f, 1f);
                    }
                    else {
                        TalkUI.instance.imageBubble.transform.localScale = new Vector3(1f, 1f - Easing.SineEaseIn(timer, 0f, 1f, 0.1f), 1f);
                    }
                    if (timer < 0.1f) {
                        TalkUI.instance.canvasGroup.alpha = 1f;
                    }
                    else {
                        TalkUI.instance.canvasGroup.alpha = 1f - (timer - 0.1f) * 20f;
                    }
                }
                else {
                    TalkUI.instance.imageTail.transform.localScale = new Vector3(1f, 0f, 1f);
                    TalkUI.instance.imageBubble.transform.localScale = new Vector3(1f, 0f, 1f);
                    TalkUI.instance.canvasGroup.alpha = 0f;
                    timer = 0f;
                    if (lineCount < lines.Length - 1) {
                        lineCount++;
                        isPlayerTalking = lineCount < playerTalks.Length ? playerTalks[lineCount] : false;
                        state = State.BubbleAppearing;
                        TalkUISizeReset();
                    }
                    else {
                        if (salesperson != null) {
                            lines = new string[1] { lines[lines.Length - 1] };
                            waitTimes = new float[0] { };
                            input = new Vector2[0] { };
                            playerTalks = new bool[1] { playerTalks[playerTalks.Length - 1] };
                        }
                        if (triggerEnding) {
                            TheEndManager.instance.Show();
                        }
                        if (inputFirst == 0f) {
                            state = State.None;
                        }
                        else {
                            state = State.WalkLast;
                        }
                    }
                }
                break;
            case State.WalkLast:
                timer += dt;
                if (timer < timeFirst) {
                    physBox.input = new Vector2(-inputFirst, 0f);
                }
                else {
                    timer = 0f;
                    physBox.input = new Vector2(0f, 0f);
                    state = State.None;
                }
                break;
        }
        inputNextOld = inputNext;
    }

    private static string SubStringRichText(string s, int no) {
        return s.Substring(0, no) + "<color=#00000000>" + s.Substring(no, s.Length - no) + "</color>";
    }

    public void Talk() {
        dontTalkAgain = autoTalk;
        if (inputFirst == 0f) {
            state = State.BubbleAppearing;
            TalkUISizeReset();
            isPlayerTalking = lineCount < playerTalks.Length ? playerTalks[lineCount] : false;
        }
        else {
            state = State.WalkFirst;
        }
        TalkUISizeReset();
    }

    public static void TalkUISizeReset() {
        TalkUI.instance.text.text = "";
        TalkUI.instance.text.SetAllDirty();
        TalkUI.instance.imageBubble.rectTransform.sizeDelta = new Vector2(TalkUI.instance.imageBubble.rectTransform.sizeDelta.x, 35f);
    }

    public Rectangle GetRect() {
        if (rectWidth == 0f || rectHeight == 0f) return physBox.rect;
        return new Rectangle(physBox.rect.x, physBox.rect.y, rectWidth, rectHeight);
    }
}

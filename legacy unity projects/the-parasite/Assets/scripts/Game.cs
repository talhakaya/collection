using UnityEngine;
using System.Collections;

public enum GameState
{
    waiting,
    mainTextAppearing,
    answer1Appearing,
    answer2Appearing,
    waitingForInput,
    evaluateAnswer
}

public class Game : MonoBehaviour {

    public static bool realSprites;
	public static float time;
	public static float dt;
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
    private int questionId;
    public static int answerId = -1;
    private GameState state;
    private string[] texts = new string[] { "", "", "" };
    public AudioSource talkSound;
    public AudioSource music;
    public AudioSource breathing;
    private float talkTimeCounter;
    private float soundTimeCounter;
    private bool fastText;
    private static float CharPerSec = 0.05f;
    public float talkSpeed = 1f;
    private static float talkPeriod = 0.2f;
    

	void Start ()
	{
        shadowVector = Vector3.down + Vector3.left;
        realSprites = false;
        RenderGrayScale.ratio = -20f;
        breathing.Play();
        music.Play();
	}
	
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
		
		dt = Time.deltaTime;
		time += dt;

		MousePosition.get = Camera.main.ScreenToWorldPoint (Input.mousePosition) + Vector3.forward;
		MousePosition.x = MousePosition.get.x;
		MousePosition.y = MousePosition.get.y;

		input = Input.GetMouseButton (0);
		inputDown = input && !inputOld;
		inputUp = !input && inputOld;

		inputOld = input;

        if (answerId != -1)
        {
            checkMouse();
        }

        switch (state)
        {
            case GameState.waiting:
                if (answerId != -1 || inputDown)
                {
                    readText();
                    talkTimeCounter = 0f;
                    state = GameState.mainTextAppearing;
                    fastText = false;
                    answerId = 0;
                    Mouth.talking = true;
                }
                break;
            case GameState.mainTextAppearing:
                if (inputDown)
                {
                    fastText = true;
                }
                talkTimeCounter += Game.dt * (fastText ? 10f : 1f);
                int noOfChars = Mathf.CeilToInt(talkTimeCounter / CharPerSec);
                if (noOfChars >= texts[0].Length)
                {
                    WriteText.texts[0] = texts[0];
                    state = GameState.answer1Appearing;
                    talkTimeCounter = 0f;
                    Mouth.talking = false;
                }
                else
                {
                    WriteText.texts[0] = texts[0].Substring(0, noOfChars);
                    soundTimeCounter += Game.dt * (fastText ? 2f : 1f);
                    if (soundTimeCounter > talkPeriod / talkSpeed)
                    {
                        soundTimeCounter -= talkPeriod / talkSpeed;
                        makeSound();
                    }
                }

                break;
            case GameState.answer1Appearing:
                if (inputDown)
                {
                    fastText = true;
                }
                talkTimeCounter += Game.dt * (fastText ? 10f : 1f);
                int noOfChars2 = Mathf.CeilToInt(talkTimeCounter / CharPerSec);
                if (noOfChars2 >= texts[1].Length)
                {
                    WriteText.texts[1] = texts[1];
                    state = GameState.answer2Appearing;
                    talkTimeCounter = 0f;
                }
                else
                {
                    WriteText.texts[1] = texts[1].Substring(0, noOfChars2);
                }
                break;
            case GameState.answer2Appearing:
                if (inputDown)
                {
                    fastText = true;
                }
                talkTimeCounter += Game.dt * (fastText ? 10f : 1f);
                int noOfChars3 = Mathf.CeilToInt(talkTimeCounter / CharPerSec);
                if (noOfChars3 >= texts[2].Length)
                {
                    WriteText.texts[2] = texts[2];
                    state = GameState.waitingForInput;
                    talkTimeCounter = 0f;
                }
                else
                {
                    WriteText.texts[2] = texts[2].Substring(0, noOfChars3);
                }
                break;
            case GameState.waitingForInput:
                checkMouse();
                if (inputDown && answerId > 0)
                {
                    state = GameState.evaluateAnswer;
                }
                break;
            case GameState.evaluateAnswer:
                evaluate();
                state = GameState.waiting;
                break;
        }
	}

    private void checkMouse()
    {
        if (MousePosition.y < -2f && MousePosition.y > -2.56f)
        {
            answerId = 1;
        }
        else if (MousePosition.y <= -2.56f)
        {
            answerId = 2;
        }
        else
        {
            answerId = 0;
        }
    }

    private void makeSound()
    {
        talkSound.pitch = Random.Range(0.5f, 1f);
        talkSound.Play();
    }

    private void readText()
    {
        WriteText.texts = new string[] { "", "", "" };
        switch (questionId)
        {
            case 0:
                RenderGrayScale.ratio = 0f;
                texts = new string[] { "Hello there.", "Hi.", "..." };
                break;
            case 1:
                RenderGrayScale.ratio = 0.3f;
                Eye.blinkingLeft = true;
                Eye.blinkingRight = true;
                texts = new string[] { "How do you feel today?", "Fine, you?", "Fine." };
                break;
            case 2:
                RenderGrayScale.ratio = 0.15f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "I'm fine, too.", "Okay.", "" };
                break;
            case 3:
                RenderGrayScale.ratio = 0.5f;
                Eye.blinkingLeft = true;
                Eye.blinkingRight = true;
                texts = new string[] { "...", "...", "How are you?" };
                break;
            case 4:
                RenderGrayScale.ratio = 0.15f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "So as you know, we're here for a reason.", "Right.", "Reason?" };
                break;
            case 5:
                RenderGrayScale.ratio = 0.3f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "Yes. I'm a very experienced person.", "Right.", "Okay." };
                break;
            case 6:
                RenderGrayScale.ratio = 0.4f;
                Eye.blinkingLeft = true;
                Eye.blinkingRight = true;
                texts = new string[] { "You can learn from me.", "Right.", "Okay." };
                break;
            case 7:
                RenderGrayScale.ratio = 0f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "Will you learn from me?", "I hope so.", "We'll see." };
                break;
            case 8:
                RenderGrayScale.ratio = 0f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "Good.", "Good.", "..." };
                break;
            case 9:
                RenderGrayScale.ratio = 0.6f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "Ha, you don't think you can learn from me?", "I don't know.", "What do you want?" };
                break;
            case 10:
                RenderGrayScale.ratio = 0.2f;
                Eye.blinkingLeft = true;
                Eye.blinkingRight = false;
                texts = new string[] { "Did you see my CV?", "No.", "Yes." };
                break;
            case 11:
                RenderGrayScale.ratio = 0.25f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "It's full of experience.", "...", "" };
                break;
            case 12:
                RenderGrayScale.ratio = 0.35f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "It's full of fancy words.", "...", "" };
                break;
            case 13:
                RenderGrayScale.ratio = -1f;
                Eye.blinkingLeft = true;
                Eye.blinkingRight = true;
                texts = new string[] { "I know what I'm doing.", "...", "" };
                break;
            case 14:
                RenderGrayScale.ratio = 0.5f;
                Eye.blinkingLeft = true;
                Eye.blinkingRight = true;
                texts = new string[] { "I want to help you.", "...", "" };
                break;
            case 15:
                RenderGrayScale.ratio = 0.2f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "So, do you have any questions for me?", "What is wrong with you?", "Can't think of any questions right now." };
                break;
            case 16:
                RenderGrayScale.ratio = -1f;
                Eye.blinkingLeft = true;
                Eye.blinkingRight = false;
                texts = new string[] { "Are you sure?", "What's up with that twitchy eye?", "Yes." };
                break;
            case 17:
                RenderGrayScale.ratio = 0.3f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "What twitchy eye?", "Your eye was twitching.", "..." };
                break;
            case 18:
                RenderGrayScale.ratio = -1f;
                Eye.blinkingLeft = true;
                Eye.blinkingRight = false;
                texts = new string[] { "There's no twitchy eye.", "...", "" };
                break;
            case 19:
                RenderGrayScale.ratio = -1f;
                Eye.blinkingLeft = true;
                Eye.blinkingRight = false;
                texts = new string[] { "You're a liar.", "...", "" };
                break;
            case 20:
                RenderGrayScale.ratio = 0.4f;
                Eye.blinkingLeft = true;
                Eye.blinkingRight = false;
                texts = new string[] { "What's wrong with me?", "...", "" };
                break;
            case 21:
                RenderGrayScale.ratio = -1f;
                Eye.blinkingLeft = true;
                Eye.blinkingRight = false;
                texts = new string[] { "WHAT'S WRONG WITH ME?", "...", "" };
                break;
            case 22:
                RenderGrayScale.ratio = -2f;
                Eye.blinkingLeft = true;
                Eye.blinkingRight = false;
                texts = new string[] { "Let ME ask you a question.", "...", "" };
                break;
            case 23:
                RenderGrayScale.ratio = -10f;
                if (!realSprites)
                {
                    breathing.Stop();
                }
                realSprites = true;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "What's wrong with YOU?", "...", "" };
                break;
            case 24:
                RenderGrayScale.ratio = 1f;
                if (!realSprites)
                {
                    breathing.Stop();
                }
                realSprites = true;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "I drove here today, because you wanted my help.", "I didn't want any help.", "" };
                break;
            case 25:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "Of course you did!", "...", "" };
                break;
            case 26:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "You wanted to use my experience.", "Okay.", "No I didn't." };
                break;
            case 27:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "I've worked in a lot of companies.", "...", "" };
                break;
            case 28:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "Did you ever run a company?", "No.", "Yes." };
                break;
            case 29:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "It's harder than anything you've done.", "...", "" };
                break;
            case 30:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = true;
                Eye.blinkingRight = true;
                texts = new string[] { "Look at you.", "...", "" };
                break;
            case 31:
                RenderGrayScale.ratio = -100f;
                realSprites = false;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "You're a little piece of shit.", "...", "?" };
                break;
            case 32:
                RenderGrayScale.ratio = -1f;
                realSprites = false;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "I had sex with more women than your age.", "...", "?" };
                break;
            case 33:
                RenderGrayScale.ratio = 1f;
                realSprites = true;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "I live in a fantastic house.", "...", "?" };
                break;
            case 34:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "I drive a fucking Porsche.", "...", "" };
                break;
            case 35:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "The kind of car that you drive in a game.", "...", "" };
                break;
            case 36:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "Did you see my amazing watch?", "No.", "Can't stop looking at it." };
                break;
            case 37:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "You need to work a thousand years to afford this watch.", "...", "" };
                break;
            case 38:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "You want to be like me, don't you?", "No I don't.", "Yes, I do." };
                break;
            case 39:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "I have a yacht.", "...", "I don't care." };
                break;
            case 40:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "I throw wild parties at my yacht.", "...", "I don't care." };
                break;
            case 41:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "Only hot chicks are allowed.", "...", "I don't give a fuck." };
                break;
            case 42:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "People look up to me.", "...", "Shut up." };
                break;
            case 43:
                RenderGrayScale.ratio = -1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "Idiots like you.", "...", "Shut the fuck up." };
                break;
            case 44:
                RenderGrayScale.ratio = -2f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "I pity you.", "...", "Shut the fuck up." };
                break;
            case 45:
                RenderGrayScale.ratio = -1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "What did you say?", "I said shut the fuck up.", "Shut up." };
                break;
            case 46:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "You know what you are?", "What the hell is wrong with you?", "What are you even talking about?" };
                break;
            case 47:
                RenderGrayScale.ratio = 0f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "You're a parasite.", "A parasite?", "Do these options even work?" };
                break;
            case 48:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "They sometimes work.", "How regularly?", "Really?" };
                break;
            case 49:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "And sometimes not.", "...", "" };
                break;
            case 50:
                RenderGrayScale.ratio = -1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "You're a parasite that's trying to suck blood.", "...", "" };
                break;
            case 51:
                RenderGrayScale.ratio = -10f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "Suck the blood of successful people like me.", "...", "" };
                break;
            case 52:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "We work hard.", "...", "OK." };
                break;
            case 53:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "You don't deserve any of this.", "...", "I don't even want it." };
                break;
            case 54:
                RenderGrayScale.ratio = 1f;
                realSprites = false;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "I'm going to go now.", "Finally.", "" };
                break;
            case 55:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "It's been nice talking to you.", "Fuck you.", "Bye." };
                break;
            case 56:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "Bye.", "...", "This was a shitty game." };
                break;
            case 57:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "Oh, you really think you're better than me?", "...", "What?" };
                break;
            case 58:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "Haha, well you'll have to try very hard.", "...", "" };
                break;
            case 59:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "And then probably fail.", "...", "" };
                break;
            case 60:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "Because you're an idiot.", "...", "" };
                break;
            case 61:
                RenderGrayScale.ratio = 1f;
                Eye.blinkingLeft = false;
                Eye.blinkingRight = false;
                texts = new string[] { "You don't know what's right or wrong.", "...", "" };
                break;
        }
    }

    private void evaluate()
    {
        WriteText.texts = new string[] { "", "", "" };
        switch (questionId)
        {
            case 0:
                questionId = 1;
                break;
            case 1:
                if (answerId == 1)
                {
                    questionId = 2;
                }
                else if (answerId == 2)
                {
                    questionId = 3;
                }
                break;
            case 2:
                questionId = 4;
                break;
            case 3:
                if (answerId == 1)
                {
                    questionId = 3;
                }
                else if (answerId == 2)
                {
                    questionId = 2;
                }
                break;
            case 4:
                questionId = 5;
                break;
            case 5:
                questionId = 6;
                break;
            case 6:
                questionId = 7;
                break;
            case 7:
                if (answerId == 1)
                {
                    questionId = 8;
                }
                else if (answerId == 2)
                {
                    questionId = 9;
                }
                break;
            case 8:
                questionId = 14;
                break;
            case 9:
                if (answerId == 1)
                {
                    questionId = 10;
                }
                else if (answerId == 2)
                {
                    questionId = 14;
                }
                break;
            case 10:
                questionId = 11;
                break;
            case 11:
                questionId = 12;
                break;
            case 12:
                questionId = 13;
                break;
            case 13:
                questionId = 14;
                break;
            case 14:
                questionId = 15;
                break;
            case 15:
                if (answerId == 1)
                {
                    questionId = 20;
                }
                else if (answerId == 2)
                {
                    questionId = 16;
                }
                break;
            case 16: 
                if (answerId == 1)
                {
                    questionId = 17;
                }
                else if (answerId == 2)
                {
                    questionId = 24;
                }
                break;
            case 17:
                questionId = 18;
                break;
            case 18:
                questionId = 19;
                break;
            case 19:
                questionId = 24;
                break;
            case 20:
                questionId = 21;
                break;
            case 21:
                questionId = 22;
                break;
            case 22:
                questionId = 23;
                break;
            case 23:
                questionId = 24;
                break;
            case 24:
                questionId = 25;
                break;
            case 25:
                questionId = 26;
                break;
            case 26:
                if (answerId == 1)
                {
                    questionId = 27;
                }
                else if (answerId == 2)
                {
                    questionId = 57;
                }
                break;
            case 57:
                questionId = 28;
                break;
            case 38:
                if (answerId == 1)
                {
                    questionId = 39;
                }
                else if (answerId == 2)
                {
                    questionId = 58;
                }
                break;
            case 61:
                questionId = 42;
                break;
            case 43:
                if (answerId == 1)
                {
                    questionId = 44;
                }
                else if (answerId == 2)
                {
                    questionId = 45;
                }
                break;
            case 44:
                if (answerId == 1)
                {
                    questionId = 46;
                }
                else if (answerId == 2)
                {
                    questionId = 45;
                }
                break;
            case 47:
                if (answerId == 1)
                {
                    questionId = 50;
                }
                else if (answerId == 2)
                {
                    questionId = 48;
                }
                break;
            case 56:
                questionId = 0;
                WriteText.texts = new string[] { "The Parasite", "By Talha Kaya", "Play with mouse, select answers" };
                answerId = -1;
                break;
            default:
                questionId++;
                break;
        }
    }
}

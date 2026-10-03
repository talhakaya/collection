using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Games.VirtualPet
{
	public enum TextState
	{
	    None,
	    LineAppearing,
	    WaitingForAnswer,
	    Answered,
	    Return,
	    Wait
	}

	public enum QuestionType
	{
	    justText,
	    yesNoQuestion,
	    textAnswer
	}

	public enum Question
	{
	    None,
	    Welcome,
	    PlayerName,
	    PetName,
	    TextBeforeWait,
	    Wait
	}

	public class StateManager : MonoBehaviour
	{
	    public static Question question;
	    public static TextState textState;
	    public Text text;
	    public string[] chatText;
	    public int chatPointer;
	    public string questionString;
	    public string answerString;
	    public bool isAnswerYes;
	    public QuestionType questionType;
	    public GameObject[] enableWhenYesNoQuestion;
	    public GameObject[] enableWhenTextQuestion;
	    private float textAppearTimer;
	    public InputField inputField;
	    private const float charsPerSec = 20f;
	    private List<AudioClip> clips;
	    private List<float> pitches;
	    public AudioClip[] sounds;
	    public AudioClip[] soundsLong;
	    private int clipIndex;
	    private const float ClipPeriod = 0.2f;
	    private float pitch = 1f;

	    private string playerName;
	    private string petName;

		void Start ()
	    {
	        question = Question.None;
	        playerName = PlayerPrefs.GetString("VirtualPet.playerName", "");
	        petName = PlayerPrefs.GetString("VirtualPet.petName", "");
		}

		void Update ()
	    {
		    switch (textState)
	        {
	            case TextState.None:
	                if (chatText == null || chatPointer >= chatText.Length - 1)
	                {
	                    getQuestion();
	                }
	                else
	                {
	                    chatPointer++;
	                    questionString = chatText[chatPointer];
	                }
	                text.text = "";
	                for (int i = 0; i < enableWhenYesNoQuestion.Length; i++)
	                {
	                    enableWhenYesNoQuestion[i].SetActive(false);
	                }
	                for (int i = 0; i < enableWhenTextQuestion.Length; i++)
	                {
	                    enableWhenTextQuestion[i].SetActive(false);
	                }
	                textAppearTimer = 0f;
	                if (question == Question.Wait)
	                {
	                    textState = TextState.Wait;
	                }
	                else
	                {
	                    setClips(questionString);
	                    textState = TextState.LineAppearing;
	                }
	                break;
	            case TextState.LineAppearing:
	                textAppearTimer += Game.dt;// * (Game.input ? 10f : 1f);
	                if (textAppearTimer > clipIndex * ClipPeriod && clipIndex < clips.Count)
	                {
	                    if (clips[clipIndex] != null)
	                    {
	                        AudioSource source = TalhaAudioSource.getSource();
	                        source.clip = clips[clipIndex];
	                        source.pitch = pitches[clipIndex];
	                        source.Play();
	                    }
	                    clipIndex++;
	                }
	                int noOfChars = Mathf.RoundToInt(textAppearTimer * charsPerSec);
	                if (noOfChars > questionString.Length)
	                {
	                    if (questionType == QuestionType.yesNoQuestion)
	                    {
	                        for (int i = 0; i < enableWhenYesNoQuestion.Length; i++)
	                        {
	                            enableWhenYesNoQuestion[i].SetActive(true);
	                        }
	                    }
	                    else if (questionType == QuestionType.textAnswer)
	                    {
	                        for (int i = 0; i < enableWhenTextQuestion.Length; i++)
	                        {
	                            enableWhenTextQuestion[i].SetActive(true);
	                        }
	                    }
	                    textState = TextState.WaitingForAnswer;
	                }
	                else
	                {
	                    text.text = questionString.Substring(0, noOfChars) + "<color=#00000000>" + questionString.Substring(noOfChars, questionString.Length - noOfChars) + "</color>";
	                }
	                break;
	            case TextState.WaitingForAnswer:
	                text.text = questionString;
	                if (questionType == QuestionType.justText)
	                {
	                    if (Game.inputDown)
	                    {
	                        textState = TextState.Answered;
	                    }
	                }
	                break;
	            case TextState.Answered:
	                text.text = "";
	                for (int i = 0; i < enableWhenYesNoQuestion.Length; i++)
	                {
	                    enableWhenYesNoQuestion[i].SetActive(false);
	                }
	                for (int i = 0; i < enableWhenTextQuestion.Length; i++)
	                {
	                    enableWhenTextQuestion[i].SetActive(false);
	                }
	                textState = TextState.Return;
	                checkAnswer();
	                break;
	            case TextState.Return:
	                textState = TextState.None;
	                break;
	            case TextState.Wait:
	                break;
	        }
		}

	    void getQuestion()
	    {
	        chatPointer = 0;
	        chatText = null;
	        changeQuestionState();
	        switch (question)
	        {
	            case Question.Welcome:
	                chat(new string[] {
	                    "welcome welcome. I'm the guy they hired to keep you company.",
	                    "feeling a bit lonely, ha, champ? that's why I'm here. don't worry.",
	                    "sometimes things can get a bit rough, but there's nothing a sip from the bleach bottle can't solve, is there?",
	                    "hehe, just joking. you better get used to my jokes. I'm a natural entertainer. Of course I am, I'm a cat!",
	                    "anyway."
	                });
	                break;
	            case Question.PlayerName:
	                questionString = "I should probably learn your name before talking about bleach. what's your name?";
	                questionType = QuestionType.textAnswer;
	                break;
	            case Question.PetName:
	                questionString = "while we're at it, tell me, what do you want to call me?";
	                questionType = QuestionType.textAnswer;
	                break;
	            case Question.TextBeforeWait:
	                chat(new string[] {
	                    petName + "?? okay. believe it or not, I've heard worse things people calling me.",
	                    "I'll just have to get used to it. eh, " + playerName + "?",
	                    "oh well. okay.",
	                    "as I said, I'm here to keep you company. make you feel less alone. so why don't you go do your thing?",
	                    "keep me open, and I'll feel it when you feel alone, or I'll just randomly make noises until you focus on me again.",
	                    "I'm a cat. I do stuff like that.",
	                    "what? it's natural. it's alright.",
	                    "okay, I'm gonna be a bit silent for now. go work on stuff, play games, watch porn, whatever.",
	                    "no judgements here, my friend. you gotta do what you gotta do.",
	                    "I know you want to. you're lonely. that's why I'm here.",
	                    "go rub one out.",
	                    "I'm not watching.",
	                    "hey, actually, can I watch?",
	                    "nah, I'm not gonna watch.",
	                    "okay, go now, leave me to my peace, bye."
	                });
	                break;
	            case Question.Wait:
	                questionString = "";
	                break;
	            //default:
	            //    questionString = "fuck bitches get money, right?";
	            //    questionType = QuestionType.yesNoQuestion;
	            //    break;
	        }
	    }

	    void chat(string[] newChatText)
	    {
	        chatText = newChatText;
	        chatPointer = 0;
	        questionString = chatText[0];
	        questionType = QuestionType.justText;
	    }

	    public void SubmitTextInput()
	    {
	        if (textState == TextState.WaitingForAnswer && questionType == QuestionType.textAnswer && inputField.text.Length > 0)
	        {
	            answerString = inputField.text;
	            textState = TextState.Answered;
	            inputField.text = "";
	        }
	    }

	    public void PressYes()
	    {
	        if (textState == TextState.WaitingForAnswer && questionType == QuestionType.yesNoQuestion)
	        {
	            isAnswerYes = true;
	            textState = TextState.Answered;
	        }
	    }

	    public void PressNo()
	    {
	        if (textState == TextState.WaitingForAnswer && questionType == QuestionType.yesNoQuestion)
	        {
	            isAnswerYes = false;
	            textState = TextState.Answered;
	        }
	    }

	    void checkAnswer()
	    {
	        switch (question)
	        {
	            case Question.PlayerName:
	                playerName = answerString;
	                PlayerPrefs.SetString("VirtualPet.playerName", playerName);
	                break;
	            case Question.PetName:
	                petName = answerString;
	                PlayerPrefs.SetString("VirtualPet.petName", petName);
	                break;
	        }
	    }

	    void changeQuestionState()
	    {
	        switch (question)
	        {
	            case Question.None:
	                question = Question.Welcome;
	                break;
	            case Question.Welcome:
	                if (playerName == "")
	                {
	                    question = Question.PlayerName;
	                }
	                else
	                {
	                    //real shit comes here
	                }
	                break;
	            case Question.PlayerName:
	                question = Question.PetName;
	                break;
	            case Question.PetName:
	                question = Question.TextBeforeWait;
	                break;
	            case Question.TextBeforeWait:
	                question = Question.Wait;
	                break;
	        }
	    }

	    void setClips(string s)
	    {
	        clipIndex = 0;
	        clips = new List<AudioClip>();
	        pitches = new List<float>();
	        char[] sentenceDelimiters = new char[] { '.', '?', '!' };
	        string[] sentences = s.Split(sentenceDelimiters, System.StringSplitOptions.None);
	        char[] wordDelimiters = new char[] { ' ', ',' };
	        for (int i = 0; i < sentences.Length; i++)
	        {
	            bool addedAnySound = false;
	            string[] words = sentences[i].Split(wordDelimiters, System.StringSplitOptions.None);
	            for (int j = 0; j < words.Length; j++)
	            {
	                bool lastWordOfSentence = (j == words.Length - 1);
	                if (words[j].Length == 0)
	                {
	                    clips.Add(null);
	                    pitches.Add(1f);
	                }
	                else if (words[j].Length <= 5)
	                {
	                    addedAnySound = true;
	                    clips.Add(getSound(words[j], lastWordOfSentence));
	                    pitches.Add(pitch);
	                }
	                else
	                {
	                    for (int k = 0; k < words[j].Length / 4f; k++)
	                    {
	                        addedAnySound = true;
	                        clips.Add(getSound(words[j].Substring(k * 4), lastWordOfSentence && (k + 1 >= words[j].Length / 4f)));
	                        pitches.Add(pitch);
	                    }
	                }
	            }
	            if (addedAnySound)
	            {
	                clips.Add(null);
	                clips.Add(null);
	                pitches.Add(1f);
	                pitches.Add(1f);
	            }
	        }

	    }

	    AudioClip getSound(string s, bool isLong)
	    {
	        int total = 0;
	        for (int i = 0; i < s.Length; i++)
	        {
	            total += (int)s[i];
	        }
	        int index = total % sounds.Length;
	        pitch = 1.1f + (total % 10) / 10f * 0.2f;
	        if (isLong)
	        {
	            return soundsLong[index];
	        }
	        else
	        {
	            return sounds[index];
	        }
	    }
	}
}

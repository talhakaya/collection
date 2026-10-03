using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.LetsNeverDoThatAgain
{
	public class Scene0 : MonoBehaviour {
	    public static int sceneCount;
	    private static int maxCount = 4;
	    // In the collection: set while one scene of the game loads the next, see Game.Awake.
	    public static bool travelling;
	    // In the collection: the scenes were loaded by their build index; these are the same five, in that order.
	    private static string[] scenes = new string[] { "scene0", "scene1", "scene1.5", "scene2", "scene3" };
	    private float timeCounter = 0f;
		void Start ()
	    {
	        if (sceneCount == 0)
	        {
	            Physics.gravity = new Vector2(0f, -9.81f);
	            Textt.updateText("Let's Never Do That Again");
	        }
	        else if (sceneCount == 1)
	        {
	            Textt.updateText("by Talha Kaya");
	        }
	        else if (sceneCount == 2)
	        {
	            Textt.updateText("and David Bowie");
	        }
	        else if (sceneCount == 3)
	        {
	            // In the collection: the line names the gamepad's button while a gamepad is the device in use.
	            Textt.updateText(usingGamepad() ? "A to jump" : "W to jump");
	        }
	        else if (sceneCount == 4)
	        {
	            Textt.updateText("");
	            Penguin.instance.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Penguin.instance.GetComponent<Rigidbody2D>().linearVelocity.x, 2f);
	        }
		}

	    void Update()
	    {
	        if (sceneCount == 0 || sceneCount == 1 || sceneCount == 2)
	        {
	            if (Penguin.instance.transform.position.x >= 6.4f)
	            {
	                nextLevel();
	            }
	        }
	        else if (sceneCount == 3)
	        {
	            // In the collection: the gamepad's A button jumps as well ("Jump" has no keyboard key).
	            if ((TaloketoInputManager.GetAxisRaw("Vertical") > 0f || TaloketoInputManager.GetButton("Jump")) && Physics2D.gravity != Vector2.zero)
	            {
	                Physics2D.gravity = Vector2.zero;
	                Penguin.instance.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Penguin.instance.GetComponent<Rigidbody2D>().linearVelocity.x, 12f);
	            }
	            else if (Physics2D.gravity == Vector2.zero)
	            {
	                Penguin.instance.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Penguin.instance.GetComponent<Rigidbody2D>().linearVelocity.x, 12f);
	                if (Penguin.instance.transform.position.y >= 5f)
	                {
	                    nextLevel();
	                }
	            }
	            else if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.kKey.wasPressedThisFrame)
	            {
	                Debug.Log(Physics2D.gravity);
	            }
	        }
	        else if (sceneCount == 4)
	        {

	            timeCounter += Game.dt;
	            if (timeCounter < 22f)
	            {

	            }
	            else if (timeCounter < 24f)
	            {
	                Physics2D.gravity = new Vector2(0f, -9.81f);
	            }
	            else
	            {
	                nextLevel();
	            }

	        }
	    }

	    private static bool usingGamepad()
	    {
	        var pad = UnityEngine.InputSystem.Gamepad.current;
	        var keyboard = UnityEngine.InputSystem.Keyboard.current;
	        return pad != null && (keyboard == null || pad.lastUpdateTime > keyboard.lastUpdateTime);
	    }

	    void nextLevel()
	    {
	        sceneCount++;
	        if (sceneCount > maxCount)
	        {
	            sceneCount = 0;
	        }
	        travelling = true;
	        UnityEngine.SceneManagement.SceneManager.LoadScene("Assets/games/Let's Never Do That Again/Scenes/" + scenes[sceneCount] + ".unity");
	    }
	}
}

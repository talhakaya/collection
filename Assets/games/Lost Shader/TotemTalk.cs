using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.LostShader
{
	public class TotemTalk : MonoBehaviour
	{
	    private float timer;
	    private float screenshakeTimer;
	    private int counter;
	    public GameObject[] texts;
	    private bool screenshake;
	    public GameObject disableKid;
	    public GameObject enableKid;
	    public GameObject destroyObject;
	    public GameObject enableObject;

		void Start ()
	    {
	        timer = 5f;
		}

		void Update ()
	    {

	        timer -= Game.dt;

	        if (timer <= 0f)
	        {
	            if (TaloketoInputManager.GetMouseButtonDown(0) || Game.anyKeyDown)
	            {
	                timer = 0.5f;
	                counter++;
	                for (int i = 0; i < texts.Length; i++)
	                {
	                    texts[i].SetActive(i < counter);
	                }
	                if (counter == texts.Length + 1)
	                {
	                    screenshake = true;
	                }
	            }
	        }

	        if (screenshake)
	        {
	            screenshakeTimer += Game.dt;
	            if (screenshakeTimer > 20f)
	            {
	                //the end
	                // In the collection: in the story mode this wins the game's artifact, after a
	                // moment to look at the kid in colour. Played by itself the game stays here,
	                // as it always did.
	                Collection.Story.StoryGames.Finish(4f);
	                Destroy(destroyObject);
	            }
	            if (screenshakeTimer > 18f)
	            {
	                //intro kid but with color
	                disableKid.SetActive(false);
	                enableKid.SetActive(true);
	            }
	            else if (screenshakeTimer > 10f)
	            {
	                enableObject.SetActive(true);
	                GetComponent<AudioSource>().Stop();
	            }
	            else
	            {
	                transform.position += new Vector3(Random.value - 0.5f, Random.value - 0.5f, 0f) * Game.dt;
	                for (int i = 0; i < texts.Length; i++)
	                {
	                    texts[i].transform.position += Vector3.down * (texts[i].transform.position.y + 10f) * Game.dt * 0.1f;
	                }
	            }
	        }
		}
	}
}

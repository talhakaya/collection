using System.Collections;
using Collection.Saving;
using UnityEngine;

namespace Collection.Story
{
    // How the desert, the story's first level, begins, and which part of that beginning a save slot is at:
    //
    //   Nothing has happened: the intro begins. The giant television is kept out of sight until the intro is
    //   over, for the character to find later.
    //
    //   The intro has been had: the sea, the character standing in it, the television in the distance.
    //
    //   The television has been met: the land is up.
    //
    // What has happened is read from the conversations kept in the slot: the intro's and the television's
    // NPCTriggers are onlyOnce. StoryDirector asks this (SetUp) as the scene starts, before it does what every
    // level's director does.
    public class DesertOpening : MonoBehaviour
    {
        public IntroSequence intro;
        [Tooltip("The intro's conversation.")]
        public NPCTrigger introTrigger;
        public TelevisionEncounter encounter;
        [Tooltip("The first meeting with the television.")]
        public NPCTrigger televisionTrigger;
        [Tooltip("The whole television, kept out of sight during the intro.")]
        public GameObject television;
        [Tooltip("Seconds after the intro's end before the television is there: until the view is back above the character and only the television's reflection can be in it.")]
        public float televisionAppearsAfter = 1f;
        [Tooltip("Seconds the television takes to appear.")]
        public float televisionFadeTime = 3f;

        // The scene as the slot has it. False while the intro is still to be had: the character is then to stay
        // where it stands in the scene, which is where the intro is set.
        public bool SetUp(StorySave story)
        {
            bool introHad = story.conversations.Contains(introTrigger.conversation);
            bool televisionMet = story.conversations.Contains(televisionTrigger.conversation);

            if (!introHad)
            {
                intro.Begin();
                television.SetActive(false);
                return false;
            }

            // In this order, and before the character is moved: each starts from what the one before left.
            intro.Skip();
            if (televisionMet)
                encounter.Skip();
            return true;
        }

        // For the intro conversation's end.
        public void IntroEnded()
        {
            StartCoroutine(ShowTelevision());
        }

        IEnumerator ShowTelevision()
        {
            yield return new WaitForSeconds(televisionAppearsAfter);

            // Not all at once: its reflection is in the picture, and would jump into it.
            MorphSphere[] shapes = television.GetComponentsInChildren<MorphSphere>(true);
            foreach (MorphSphere shape in shapes)
                shape.Visible = 0f;
            television.SetActive(true);
            for (float t = 0f; t < televisionFadeTime; t += Time.deltaTime)
            {
                foreach (MorphSphere shape in shapes)
                    shape.Visible = Mathf.SmoothStep(0f, 1f, t / televisionFadeTime);
                yield return null;
            }
            foreach (MorphSphere shape in shapes)
                shape.Visible = 1f;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Collection.Saving;
using UnityEngine;

namespace Collection.Story
{
    // Sets the story scene up as the save slot being played has it. The scene is loaded afresh on starting or
    // continuing a slot and on coming back from a game, and this decides, before anything else in it starts, which
    // part of the story it is at:
    //
    //   Nothing has happened: the intro begins. The giant television is kept out of sight until the intro is
    //   over, for the character to find later.
    //
    //   The intro has been had: the sea, the character standing in it, the television in the distance.
    //
    //   The television has been met: the land is up. The artifacts lie on it, and those already won follow the
    //   character instead.
    //
    //   All the artifacts are won: the television has something new to say (its second NPCTrigger).
    //
    // What has happened is read from the conversations kept in the slot: the intro's and the television's
    // NPCTriggers are onlyOnce. The character is put back where it was standing when the story was last left.
    [DefaultExecutionOrder(-100)]
    public class StoryDirector : MonoBehaviour
    {
        public Transform player;
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
        [Tooltip("What the television says once every artifact is won. Switched on only then.")]
        public NPCTrigger allArtifactsTrigger;
        public List<Artifact> artifacts = new List<Artifact>();
        [Tooltip("A character lower than this has fallen out of the world, and is put back on the ground (m).")]
        public float fallenBelow = -30f;

        void OnEnable()
        {
            DialogueMan.OnDialogueComplete += SavePlace;
        }

        void OnDisable()
        {
            DialogueMan.OnDialogueComplete -= SavePlace;
        }

        void Awake()
        {
            // A level with no edges set up in its scene gets the usual ones.
            if (FindFirstObjectByType<LevelBounds>(FindObjectsInactive.Include) == null)
                new GameObject("Level Bounds").AddComponent<LevelBounds>();
        }

        void Start()
        {
            StorySave story = SaveManager.Slot.story;
            bool introHad = story.conversations.Contains(introTrigger.conversation);
            bool televisionMet = story.conversations.Contains(televisionTrigger.conversation);

            if (!introHad)
            {
                intro.Begin();
                television.SetActive(false);
            }
            else
            {
                // In this order, and before the character is moved: each starts from what the one before left.
                intro.Skip();
                if (televisionMet)
                    encounter.Skip();
                PlacePlayer(story);
            }

            SetUpArtifacts(story);
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

        // Once the scene has started and the artifact has had a moment to come over.
        IEnumerator TalkToArtifact(Artifact artifact)
        {
            yield return new WaitForSeconds(1.5f);
            artifact.TalkAsWon();
        }

        void SetUpArtifacts(StorySave story)
        {
            string justWon = StoryGames.TakeJustWon();

            // Those won, in the order they were won, in a line behind the character.
            Transform leader = player;
            int won = 0;
            foreach (string id in story.artifacts)
            {
                Artifact artifact = artifacts.Find(a => a != null && a.id == id);
                if (artifact == null)
                    continue;
                artifact.Follow(leader, leader == player);
                // The one just won comes over from where it lay; the others are with the character already.
                if (id != justWon)
                    artifact.SnapBehind(player, won);
                else
                    StartCoroutine(TalkToArtifact(artifact));
                leader = artifact.transform;
                won++;
            }

            if (allArtifactsTrigger != null)
                allArtifactsTrigger.gameObject.SetActive(artifacts.Count > 0 && won >= artifacts.Count);
        }

        void PlacePlayer(StorySave story)
        {
            Vector3 place = player.position;
            float facing = player.eulerAngles.y;
            if (story.placeSaved)
            {
                place = new Vector3(story.placeX, story.placeY, story.placeZ);
                facing = story.placeFacing;
            }

            // With no place saved, on the ground where it stands in the scene: its height there is right for the
            // sea, not for land that has come up since.
            var controller = player.GetComponent<CharacterController>();
            float standing = controller.height * 0.5f - controller.center.y + controller.skinWidth;
            if (!story.placeSaved)
            {
                place.y = encounter.GroundHeight(place) + standing;
            }
            else if (place.y < encounter.GroundHeight(place) + standing - 0.5f && !SomethingUnder(place))
            {
                // A saved place that is under the ground with nothing to stand on: saved where there was sea, or
                // lower land, in an earlier version of the level. (Under the ground with something to stand on
                // is the cave.)
                place.y = encounter.GroundHeight(place) + standing;
            }

            controller.enabled = false;
            player.SetPositionAndRotation(place, Quaternion.Euler(0f, facing, 0f));
            controller.enabled = true;
        }

        // Whether there is anything to stand on within a few metres under a place. The sea's floor does not count:
        // it reaches under all of the land.
        bool SomethingUnder(Vector3 place)
        {
            Physics.SyncTransforms();
            foreach (RaycastHit hit in Physics.RaycastAll(place + Vector3.up * 0.3f, Vector3.down, 5f, ~0, QueryTriggerInteraction.Ignore))
                if (hit.collider.transform != encounter.floor && !hit.collider.transform.IsChildOf(player))
                    return true;
            return false;
        }

        // Where the character last stood outside a conversation, kept up every frame so that it is still known
        // when the scene is being taken down and the character may be gone already. Not during a conversation,
        // when the character may be somewhere only a cutscene has it.
        Vector3 lastPlace;
        float lastFacing;
        bool placeKnown;

        void LateUpdate()
        {
            // Fallen out of the world, by whatever means: back on the ground above.
            if (player.position.y < fallenBelow)
            {
                var controller = player.GetComponent<CharacterController>();
                Vector3 back = player.position;
                back.y = encounter.GroundHeight(back) + controller.height * 0.5f - controller.center.y + controller.skinWidth + 0.5f;
                bool was = controller.enabled;
                controller.enabled = false;
                player.position = back;
                controller.enabled = was;
                var movement = player.GetComponent<PlayerMovement>();
                if (movement != null)
                    movement.Velocity = Vector3.zero;
            }

            if (Main.inst.dialogue.IsTalking())
                return;
            lastPlace = player.position;
            lastFacing = player.eulerAngles.y;
            placeKnown = true;
        }

        // Keeps where the character is: on leaving the scene (for a game, or for the menu), on quitting, and
        // after every conversation.
        public void SavePlace()
        {
            if (!placeKnown)
                return;

            StorySave story = SaveManager.Slot.story;
            story.placeSaved = true;
            story.placeX = lastPlace.x;
            story.placeY = lastPlace.y;
            story.placeZ = lastPlace.z;
            story.placeFacing = lastFacing;
            SaveManager.MarkDirty();
        }

        void OnDestroy()
        {
            SavePlace();
        }

        void OnApplicationQuit()
        {
            SavePlace();
            SaveManager.Save();
        }
    }
}

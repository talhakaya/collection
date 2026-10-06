using System.Collections;
using System.Collections.Generic;
using Collection.Saving;
using UnityEngine;

namespace Collection.Story
{
    // Sets a level's scene up as the save slot being played has it. The scene is loaded afresh on starting or
    // continuing a slot, on coming back from a game, and on coming to the level from the one before; and this
    // decides, before anything else in it starts, how things stand in it. What every level has in common is
    // here:
    //
    //   The character is put back where it was standing when the story was last left, and the bicycle, if the
    //   level has one, where it was left.
    //
    //   The level's artifacts lie where they are, and those already won follow the character instead. Once all
    //   of them are won, the trigger for what comes of that is switched on (allArtifactsTrigger); at the end of
    //   that conversation the story goes on to the next level (NextLevel).
    //
    //   A character that has fallen out of the world is put back on the ground.
    //
    // What a level has of its own before any of that is its opening: the desert's is the intro in the sea and
    // the television that brings the land up (DesertOpening). A level without one simply starts.
    [DefaultExecutionOrder(-100)]
    public class StoryDirector : MonoBehaviour
    {
        public Transform player;
        [Tooltip("The desert's own beginning: the intro, the sea, the television. Empty in a level that has none.")]
        public DesertOpening desert;
        [Tooltip("What is said once every artifact is won. Switched on only then.")]
        public NPCTrigger allArtifactsTrigger;
        public List<Artifact> artifacts = new List<Artifact>();
        [Tooltip("Kept in the save where it was left. Empty: the one in the scene, if there is one.")]
        public Bicycle bicycle;
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

        Vector3 startPlace;
        PlayerControl control;
        // The level this scene was started as. The slot may be on to the next before the scene is gone.
        int level;

        void Start()
        {
            startPlace = player.position;
            control = PlayerControl.Of(player);
            level = StoryLevels.Current;
            // Whatever game was being played from the story is over: this is the story.
            StoryGames.Arrive();
            StorySave story = SaveManager.Slot.story;

            // The level's opening first, and before the character is moved: it starts from where the character
            // stands in the scene. While it is still to be had, the character stays there.
            if (desert == null || desert.SetUp(story))
                PlacePlayer(story);

            if (bicycle == null)
                bicycle = FindFirstObjectByType<Bicycle>();
            PlaceBicycle(story);

            SetUpArtifacts(story);
        }

        // For the end of the conversation that closes the level (allArtifactsTrigger's): on to the next one.
        public void NextLevel()
        {
            StoryLevels.Advance();
        }

        // Once the scene has started and the artifact has had a moment to come over. Until it speaks the
        // character stays where it is: it is not to be walked off with before it has had its say. (From then on
        // the conversation keeps the character still, as any does.)
        IEnumerator TalkToArtifact(Artifact artifact)
        {
            control.Take(this, false);
            yield return new WaitForSeconds(1.5f);
            artifact.TalkAsWon();
            control.Release(this);
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

            // With no place saved, on the ground where it stands in the scene: its height there need not be the
            // land's (the desert's is right for the sea, not for land that has come up since).
            float standing = control.Standing;
            if (!story.placeSaved)
            {
                place.y = LandUnder(ref place) + standing;
            }
            else if (place.y < LandUnder(ref place) + standing - 0.5f && !SomethingUnder(place))
            {
                // A saved place that is under the ground with nothing to stand on: saved where there was sea, or
                // lower land, in an earlier version of the level. (Under the ground with something to stand on
                // is a cave.)
                place.y = LandUnder(ref place) + standing;
            }

            control.MoveTo(place, Quaternion.Euler(0f, facing, 0f));
        }

        // The bicycle where it was left, standing. One never ridden is where the scene has it.
        void PlaceBicycle(StorySave story)
        {
            if (bicycle == null || !story.bikeSaved)
                return;
            Vector3 place = new Vector3(story.bikeX, story.bikeY, story.bikeZ);
            Physics.SyncTransforms();
            float under;
            if (Ground.Under(place + Vector3.up * 1.5f, 40f, ~0, out under, bicycle.transform, player))
                place.y = under + 0.02f;
            bicycle.PutAt(place, story.bikeFacing);
        }

        // The height of the level's ground at a place: the land's, and in the desert the sea's floor while there
        // is a sea.
        float Land(Vector3 at)
        {
            return desert != null ? desert.encounter.GroundHeight(at) : Ground.Land(at);
        }

        // The height of the ground at a place. Where there is none (off the land's edge) the place itself is
        // changed, to where the character stands in the scene.
        float LandUnder(ref Vector3 place)
        {
            float ground = Land(place);
            if (ground < -1000f)
            {
                place.x = startPlace.x;
                place.z = startPlace.z;
                ground = Land(place);
            }
            return ground;
        }

        // Whether there is anything to stand on within a few metres under a place. The sea's floor does not count:
        // it reaches under all of the land.
        bool SomethingUnder(Vector3 place)
        {
            Physics.SyncTransforms();
            float under;
            return Ground.Under(place + Vector3.up * 0.3f, 5f, ~0, out under, desert != null ? desert.encounter.floor : null, player);
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
                Vector3 back = player.position;
                back.y = LandUnder(ref back) + control.Standing + 0.5f;
                control.MoveTo(back);
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

        // Keeps where the character is, and the bicycle: on leaving the scene (for a game, or for the menu), on
        // quitting, and after every conversation.
        public void SavePlace()
        {
            if (!placeKnown)
                return;
            // Not once the slot has gone on to the next level (StoryLevels.Advance): this scene is still there
            // for a moment, and its place is not one in that level.
            if (StoryLevels.Current != level)
                return;

            StorySave story = SaveManager.Slot.story;
            story.placeSaved = true;
            story.placeX = lastPlace.x;
            story.placeY = lastPlace.y;
            story.placeZ = lastPlace.z;
            story.placeFacing = lastFacing;
            // (The bicycle may be destroyed already, with the scene; what it knew of its place is still there.)
            if (!ReferenceEquals(bicycle, null))
                bicycle.Keep(story);
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

using Collection.Saving;
using UnityEngine;

namespace Collection.Story
{
    // Something lying in the world that is won by playing one of the collection's games.
    //
    // Lying there, it is something to talk to: its NPCTrigger (on the same object) runs its conversation, and at
    // the conversation's end calls Play, which starts the game. When the game has been played to its end the
    // story scene comes back with the artifact in the save slot (StoryGames), and from then on it has nothing to
    // say and follows the character: StoryDirector calls Follow with the character, or with the artifact before it
    // in the line, to go after.
    public class Artifact : MonoBehaviour
    {
        [Tooltip("The name it is kept under in the save. Changing it loses it from saved games.")]
        public string id;
        [Tooltip("The game that wins it, by its name in the GameList.")]
        public string game;

        [Header("Look")]
        [Tooltip("The part that turns and bobs.")]
        public Transform visual;
        public float turnSpeed = 40f;
        public float bobHeight = 0.08f;
        public float bobSpeed = 1.6f;

        [Header("Following")]
        [Tooltip("How far behind what it follows it keeps (m).")]
        public float spacing = 0.9f;
        [Tooltip("How quickly it closes on its place in the line. Higher is stiffer.")]
        public float eagerness = 5f;
        [Tooltip("How high the line floats, from the character's own position, which is its middle (m).")]
        public float carryHeight = -0.2f;

        Transform leader;
        bool leaderIsCharacter;
        Vector3 visualStart;
        float phase;

        public bool Won => SaveManager.Slot.story.artifacts.Contains(id);

        void Awake()
        {
            if (visual != null)
                visualStart = visual.localPosition;
            // Not all bobbing together.
            phase = Mathf.Abs(id != null ? id.GetHashCode() % 628 : 0) * 0.01f;
        }

        // For the NPCTrigger's onConversationEnd.
        public void Play()
        {
            StoryGames.Play(game, id);
        }

        // From now on it goes after `behind`: the character, or another artifact.
        public void Follow(Transform behind, bool isCharacter)
        {
            leader = behind;
            leaderIsCharacter = isCharacter;

            // Nothing to talk to any more.
            if (TryGetComponent(out NPCTrigger trigger))
                trigger.enabled = false;
            foreach (Collider part in GetComponents<Collider>())
                part.enabled = false;
        }

        // Straight to a place in the line behind the character (0 is the first), for starting there and not
        // flying in from where it lay.
        public void SnapBehind(Transform character, int place)
        {
            transform.position = character.position + Vector3.up * carryHeight - character.forward * (spacing * (place + 1));
        }

        Vector3 LeaderPoint()
        {
            return leaderIsCharacter ? leader.position + Vector3.up * carryHeight : leader.position;
        }

        void Update()
        {
            if (visual == null)
                return;
            visual.Rotate(Vector3.up, turnSpeed * Time.deltaTime, Space.World);
            visual.localPosition = visualStart + Vector3.up * (Mathf.Sin(Time.time * bobSpeed + phase) * bobHeight);
        }

        // After the character has moved for the frame.
        void LateUpdate()
        {
            if (leader == null)
                return;

            // Pulled along like a link of a chain: it stays where it is until what it follows is further than
            // `spacing` away, then comes along the straight line between them. So the line trails out behind the
            // character along the way it walked.
            Vector3 point = LeaderPoint();
            Vector3 away = transform.position - point;
            if (away.magnitude <= spacing)
                return;
            Vector3 place = point + away.normalized * spacing;
            transform.position = Vector3.Lerp(transform.position, place, 1f - Mathf.Exp(-eagerness * Time.deltaTime));
        }
    }
}

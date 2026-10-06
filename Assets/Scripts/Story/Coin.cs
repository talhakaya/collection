using UnityEngine;

namespace Collection.Story
{
    // A coin out of a barrel or a chest: it jumps out, drops, and then flies to the character, who has it the
    // moment it arrives (CoinCounter). It is not something to be picked up or missed; the jump is for show.
    public class Coin : MonoBehaviour
    {
        [Tooltip("How fast it jumps out (m/s).")]
        public float jump = 3.5f;
        [Tooltip("Seconds before it starts for the character.")]
        public float hang = 0.55f;
        [Tooltip("How fast it gets faster on its way to the character (m/s²).")]
        public float pull = 40f;
        public float turnSpeed = 540f;

        Vector3 velocity;
        float wait;
        float age;
        float floor;
        float speed;
        static Transform character;

        // `count` coins out of `from`, one after the other, `apart` seconds between them.
        public static void Spill(Coin prefab, int count, Vector3 from, float apart)
        {
            for (int i = 0; i < count; i++)
            {
                Coin coin = Instantiate(prefab, from, Random.rotation);
                Vector2 side = Random.insideUnitCircle;
                coin.velocity = new Vector3(side.x, 0f, side.y) * (coin.jump * 0.45f) + Vector3.up * (coin.jump * Random.Range(0.8f, 1.2f));
                coin.wait = i * apart;
                coin.floor = from.y - 0.3f;
                coin.gameObject.SetActive(coin.wait <= 0f);
                if (coin.wait > 0f)
                    coin.Invoke(nameof(Appear), coin.wait);
            }
        }

        void Appear()
        {
            gameObject.SetActive(true);
        }

        void Update()
        {
            if (character == null)
            {
                PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
                if (player == null)
                    return;
                character = player.transform;
            }

            age += Time.deltaTime;
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime, Space.World);

            if (age < hang)
            {
                velocity += Vector3.down * (14f * Time.deltaTime);
                Vector3 place = transform.position + velocity * Time.deltaTime;
                if (place.y < floor && velocity.y < 0f)
                {
                    place.y = floor;
                    velocity.y *= -0.4f;
                }
                transform.position = place;
                return;
            }

            speed += pull * Time.deltaTime;
            Vector3 to = character.position - transform.position;
            float step = speed * Time.deltaTime;
            if (to.magnitude <= step + 0.3f)
            {
                CoinCounter.Add(1);
                Destroy(gameObject);
                return;
            }
            transform.position += to.normalized * step;
        }
    }
}

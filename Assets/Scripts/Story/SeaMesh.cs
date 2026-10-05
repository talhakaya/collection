using UnityEngine;
using UnityEngine.Rendering;

namespace Collection.Story
{
    // The sea's surface: one big flat grid, made here, for the Simple Water Shader.
    //
    // The shader moves the surface up and down for its waves, vertex by vertex, and its waves are a couple of metres
    // long; a mesh has to have a vertex every half metre or so for them to show. The asset's 50 m block has that,
    // but stretched to reach the horizon its vertices end up tens of metres apart and the waves are gone. This grid
    // is fine in the middle, where the game is, and coarser and coarser toward its edge, where the waves are too far
    // away to see anyway.
    //
    // Its texture coordinates go up by 1 every `blockSize` metres, which is what the asset's block had, so the
    // shader's settings (Normal Tiling, the size of the waves) mean what they mean on the asset's own water.
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter))]
    public class SeaMesh : MonoBehaviour
    {
        [Tooltip("From one edge to the other (m).")]
        public float size = 2000f;
        [Tooltip("Squares along an edge.")]
        [Range(16, 400)] public int cells = 280;
        [Tooltip("How much of the grid's fineness is spent on the middle: 1 is an even grid, lower crowds the squares toward the middle. With 280 cells over 2000 m, 0.07 gives half-metre squares in the middle.")]
        [Range(0.01f, 1f)] public float middle = 0.07f;
        [Tooltip("The texture repeats once every this many metres, as on the asset's own 50 m block.")]
        public float blockSize = 50f;
        [Tooltip("How far the shader may move the surface up (m), so the mesh is not taken to be out of sight when it is not.")]
        public float waveRoom = 2f;

        Mesh mesh;
        float builtSize, builtMiddle, builtBlock;
        int builtCells;

        void OnEnable()
        {
            Rebuild();
        }

        void OnDisable()
        {
            if (mesh != null)
            {
                if (Application.isPlaying) Destroy(mesh);
                else DestroyImmediate(mesh);
                mesh = null;
            }
        }

        void Update()
        {
            if (mesh == null || builtSize != size || builtCells != cells || builtMiddle != middle || builtBlock != blockSize)
                Rebuild();
        }

        // Where a grid line is, for u from -1 to 1: straight for an even grid, bent so that the lines near 0 are
        // closer together.
        float Place(float u)
        {
            return (middle * u + (1f - middle) * u * u * u) * size * 0.5f;
        }

        void Rebuild()
        {
            if (mesh == null)
                mesh = new Mesh { name = "Sea", hideFlags = HideFlags.HideAndDontSave };
            builtSize = size;
            builtCells = cells;
            builtMiddle = middle;
            builtBlock = blockSize;

            int n = cells;
            var vertices = new Vector3[(n + 1) * (n + 1)];
            var uvs = new Vector2[vertices.Length];
            var normals = new Vector3[vertices.Length];
            var tangents = new Vector4[vertices.Length];
            for (int z = 0; z <= n; z++)
            {
                float pz = Place(z * 2f / n - 1f);
                for (int x = 0; x <= n; x++)
                {
                    float px = Place(x * 2f / n - 1f);
                    int i = z * (n + 1) + x;
                    vertices[i] = new Vector3(px, 0f, pz);
                    uvs[i] = new Vector2(px / blockSize, pz / blockSize);
                    normals[i] = Vector3.up;
                    tangents[i] = new Vector4(1f, 0f, 0f, -1f);
                }
            }

            var triangles = new int[n * n * 6];
            int t = 0;
            for (int z = 0; z < n; z++)
            {
                for (int x = 0; x < n; x++)
                {
                    int a = z * (n + 1) + x;
                    int b = a + n + 1;
                    triangles[t++] = a;
                    triangles[t++] = b;
                    triangles[t++] = a + 1;
                    triangles[t++] = a + 1;
                    triangles[t++] = b;
                    triangles[t++] = b + 1;
                }
            }

            mesh.Clear();
            mesh.indexFormat = vertices.Length > 65000 ? IndexFormat.UInt32 : IndexFormat.UInt16;
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.tangents = tangents;
            mesh.triangles = triangles;
            mesh.bounds = new Bounds(Vector3.zero, new Vector3(size, waveRoom * 2f, size));
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }
    }
}

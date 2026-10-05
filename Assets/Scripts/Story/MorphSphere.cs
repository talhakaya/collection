using UnityEngine;

namespace Collection.Story
{
	/// <summary>
	/// A sphere that can be flattened, side by side, into the cube that just fits inside it
	/// (the Morph Sphere shader does the moving). Each of the six values goes from 0, a flat
	/// cube side, to 1, the sphere.
	///
	/// The mesh is made here: a sphere of radius 1 built as six grids, one for each side of
	/// the cube, so every side has its own square of texture coordinates (0 to 1 across, the
	/// right way up on the four sides around the middle).
	/// </summary>
	[ExecuteAlways]
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class MorphSphere : MonoBehaviour
	{
		[Range(0f, 1f)] public float right = 1f;
		[Range(0f, 1f)] public float left = 1f;
		[Range(0f, 1f)] public float up = 1f;
		[Range(0f, 1f)] public float down = 1f;
		[Range(0f, 1f)] public float front = 1f;
		[Range(0f, 1f)] public float back = 1f;

		[Tooltip("Squares along each edge of a side. More is smoother, and makes the seam between a flat side and a round one thinner.")]
		[Range(4, 128)] public int resolution = 64;

		private static readonly int RightId = Shader.PropertyToID("_Right");
		private static readonly int LeftId = Shader.PropertyToID("_Left");
		private static readonly int UpId = Shader.PropertyToID("_Up");
		private static readonly int DownId = Shader.PropertyToID("_Down");
		private static readonly int FrontId = Shader.PropertyToID("_Front");
		private static readonly int BackId = Shader.PropertyToID("_Back");

		// Each side's outward direction and which way is up on it.
		private static readonly Vector3[] Normals = { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
		private static readonly Vector3[] Ups = { Vector3.up, Vector3.up, Vector3.forward, Vector3.back, Vector3.up, Vector3.up };

		private Mesh mesh;
		private int builtResolution;
		private MeshRenderer meshRenderer;
		private MaterialPropertyBlock block;

		/// The six values in the order right, left, up, down, front, back.
		public float this[int side]
		{
			get
			{
				switch (side)
				{
					case 0: return right;
					case 1: return left;
					case 2: return up;
					case 3: return down;
					case 4: return front;
					default: return back;
				}
			}
			set
			{
				switch (side)
				{
					case 0: right = value; break;
					case 1: left = value; break;
					case 2: up = value; break;
					case 3: down = value; break;
					case 4: front = value; break;
					default: back = value; break;
				}
			}
		}

		private void OnEnable()
		{
			meshRenderer = GetComponent<MeshRenderer>();
			Rebuild();
			Apply();
		}

		private void OnDisable()
		{
			if (mesh != null)
			{
				if (Application.isPlaying) Destroy(mesh);
				else DestroyImmediate(mesh);
				mesh = null;
			}
		}

		private void LateUpdate()
		{
			if (mesh == null || builtResolution != resolution)
			{
				Rebuild();
			}

			Apply();
		}

		/// Hands the six values to the shader, for this renderer only.
		private void Apply()
		{
			if (meshRenderer == null) return;

			if (block == null) block = new MaterialPropertyBlock();
			meshRenderer.GetPropertyBlock(block);
			block.SetFloat(RightId, right);
			block.SetFloat(LeftId, left);
			block.SetFloat(UpId, up);
			block.SetFloat(DownId, down);
			block.SetFloat(FrontId, front);
			block.SetFloat(BackId, back);
			meshRenderer.SetPropertyBlock(block);
		}

		private void Rebuild()
		{
			if (mesh == null)
			{
				mesh = new Mesh { name = "Morph Sphere", hideFlags = HideFlags.HideAndDontSave };
			}

			int n = Mathf.Clamp(resolution, 4, 128);
			builtResolution = resolution;
			int perSide = (n + 1) * (n + 1);
			var vertices = new Vector3[perSide * 6];
			var uvs = new Vector2[perSide * 6];
			var triangles = new int[n * n * 6 * 6];

			int t = 0;
			for (int side = 0; side < 6; side++)
			{
				Vector3 normal = Normals[side];
				Vector3 upward = Ups[side];

				// Seen from outside, with "upward" up, this is the side's right.
				Vector3 across = Vector3.Cross(upward, -normal);
				int first = side * perSide;

				for (int y = 0; y <= n; y++)
				{
					for (int x = 0; x <= n; x++)
					{
						float u = x / (float)n;
						float v = y / (float)n;

						// Evenly spaced angles rather than evenly spaced points on the cube's
						// side, which would crowd the squares into the corners of the sphere.
						float px = Mathf.Tan((u - 0.5f) * Mathf.PI * 0.5f);
						float py = Mathf.Tan((v - 0.5f) * Mathf.PI * 0.5f);

						int i = first + y * (n + 1) + x;
						vertices[i] = (normal + across * px + upward * py).normalized;
						uvs[i] = new Vector2(u, v);
					}
				}

				for (int y = 0; y < n; y++)
				{
					for (int x = 0; x < n; x++)
					{
						int a = first + y * (n + 1) + x;
						int b = a + n + 1;
						triangles[t++] = a;
						triangles[t++] = b;
						triangles[t++] = a + 1;
						triangles[t++] = a + 1;
						triangles[t++] = b;
						triangles[t++] = b + 1;
					}
				}
			}

			mesh.Clear();
			mesh.indexFormat = vertices.Length > 65000 ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.triangles = triangles;

			// The shader never moves a vertex outside the sphere.
			mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 2f);
			GetComponent<MeshFilter>().sharedMesh = mesh;
		}
	}
}

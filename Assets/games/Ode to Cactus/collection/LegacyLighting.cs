using UnityEngine;

// In the collection: new. Feeds the scene's lights to LegacyDiffuse.shader, which lights
// the game's 3D scenes the way Unity 4's built-in renderer did (see the shader). Added by
// Game.Awake, so it is in every scene of the game without the scenes being edited.
namespace Games.OdeToCactus
{
	public class LegacyLighting : MonoBehaviour
	{
		const int MaxLights = 24;   // OTC_MAX_LIGHTS in the shader

		static readonly int lightPos = Shader.PropertyToID("_OtcLightPos");
		static readonly int lightColor = Shader.PropertyToID("_OtcLightColor");
		static readonly int lightCount = Shader.PropertyToID("_OtcLightCount");
		static readonly int ambient = Shader.PropertyToID("_OtcAmbient");

		readonly Vector4[] positions = new Vector4[MaxLights];
		readonly Vector4[] colors = new Vector4[MaxLights];

		void Awake ()
		{
			// Unity rewrote every light's intensity when it upgraded the scenes: a Unity 4
			// intensity of 1 arrives as 1.37 (2 to the power 1/2.2). They are put back, so
			// that Light.intensity means what the game's code (Father.cs) assumes it means.
			foreach (Light light in FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None))
			{
				light.intensity = Mathf.Pow(light.intensity, 2.2f) / 2f;
			}
		}

		void LateUpdate ()
		{
			int n = 0;
			foreach (Light light in FindObjectsByType<Light>(FindObjectsSortMode.None))
			{
				if (n >= MaxLights) break;
				if (!light.enabled || light.type != LightType.Point) continue;

				Vector3 p = light.transform.position;
				positions[n] = new Vector4(p.x, p.y, p.z, 1f / (light.range * light.range));
				// Light.color reads back the gamma values that were typed into the inspector.
				colors[n] = (Vector4)light.color * light.intensity;
				n++;
			}

			Shader.SetGlobalVectorArray(lightPos, positions);
			Shader.SetGlobalVectorArray(lightColor, colors);
			Shader.SetGlobalFloat(lightCount, n);
			Shader.SetGlobalVector(ambient, RenderSettings.ambientLight);
		}
	}
}

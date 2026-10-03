using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class WaveManagerPrologue : MonoBehaviour {

		private const float maxCurrentX = 160f;
		private GameObject waveSample;
		private GameObject sandSample;

		void Start ()
		{
			waveSample = Resources.Load ("Orphan/BeachRelated/WavePrologue") as GameObject;
			sandSample = Resources.Load ("Orphan/BeachRelated/SandPrologue") as GameObject;
			CreateWaves(0f, false, 0f);
			CreateWaves(2f, true, -0.1f);
			CreateWavesMiddleOfSea();
			CreateWavesMiddleOfSea();
			CreateSand();
		}

		void Update ()
		{

		}

		void CreateWaves(float currentY, bool moving, float currentZ)
		{
			float currentX = -160f;
			while (currentX < maxCurrentX)
			{
				float waveScale = Random.Range(0.5f, 1f);
				currentX += waveScale * 7f;
				if (currentX >= maxCurrentX)
				{
					break;
				}
				GameObject wave = Instantiate(waveSample, transform.position, Quaternion.identity) as GameObject;
				wave.name = "Wave";
				wave.transform.parent = transform;
				wave.transform.position += new Vector3(currentX, currentY, -0.5f + currentZ);
				wave.transform.localScale = wave.transform.localScale * waveScale;
				wave.GetComponent<WaveScriptPrologue>().moving = moving;
			}
			GameObject wave2 = Instantiate(waveSample, transform.position, Quaternion.identity) as GameObject;
			wave2.name = "Wave";
			wave2.transform.parent = transform;
			wave2.transform.position += new Vector3(maxCurrentX, currentY, -0.5f + currentZ);
			wave2.GetComponent<WaveScriptPrologue>().moving = moving;
		}

		void CreateWavesMiddleOfSea()
		{
			for (int i = 0; i < 20; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					GameObject wave = Instantiate(waveSample, transform.position + Vector3.up * 23f, Quaternion.identity) as GameObject;
					wave.name = "Wave";
					wave.transform.parent = transform;
					wave.transform.position += new Vector3(-160f + 16 * i, -15f + 10 * j, -0.5f);
					WaveScriptPrologue waveScript = wave.GetComponent<WaveScriptPrologue>();
					waveScript.moving = true;
					waveScript.randomPos = true;
				}
			}
		}

		void CreateSand()
		{
			for (int i = 0; i < 21; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					GameObject sand = Instantiate(sandSample, transform.position - Vector3.up * 13f, Quaternion.identity) as GameObject;
					sand.name = "Sand";
					sand.transform.parent = transform;
					sand.transform.position += new Vector3(Random.Range(-2f, 2f) - 170f + 16 * i, Random.Range(-2f, 2f) - 25f + 10 * j, -0.2f);
					if (Random.Range(-1f, 1f) > 0f)
					{
						sand.transform.localScale = new Vector3(-sand.transform.localScale.x, sand.transform.localScale.y, sand.transform.localScale.z);
					}
				}
			}
		}
	}
}

using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class GlassScript : MonoBehaviour {

		private DraggableObject dragScript;
		private float totalRotation = 0f;
		private const float rotateConst = -0.2f;
		private const float rotateConst2 = 2f;
		private const float randomRepositionConst = 0.1f;
		private const int noOfDropsX = 8;
		private const int noOfDropsY = 16;
		private GameObject waterDrop;

		void Start ()
		{
			dragScript = GetComponent<DraggableObject>();
			waterDrop = Resources.Load ("Orphan/ArcadeSwallowPillWaterDrop") as GameObject;
			for (int i = 0; i < noOfDropsX; i++)
			{
				for (int j = 0; j < noOfDropsY; j++)
				{
					GameObject drop = Instantiate(waterDrop, 
						new Vector3(transform.position.x -3.5f + i * 8f / noOfDropsX - randomRepositionConst + 2 * randomRepositionConst * Random.Range(0f,1f), 
						transform.position.y -7.5f + j * 16f / noOfDropsY - randomRepositionConst + 2 * randomRepositionConst * Random.Range(0f,1f), transform.position.z),
							transform.rotation) as GameObject;
					drop.transform.parent = transform;
					drop.name = "WaterDrop";
				}
			}
		}

		void Update ()
		{
			if (dragScript.hold)
			{
				transform.Rotate(Vector3.forward * rotateConst);
				totalRotation += rotateConst;
			}
			else
			{
				if (totalRotation < - rotateConst2)
				{
					transform.Rotate(Vector3.forward * rotateConst2);
					totalRotation += rotateConst2;
				}
				else if (totalRotation < 0)
				{
					transform.Rotate(- Vector3.forward * totalRotation);
					totalRotation = 0;
				}
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.gameObject.name == "WaterDrop")
			{
				other.transform.parent = transform.parent;
				other.GetComponent<WaterDropScript>().outOfGlass = true;
			}
		}
	}
}

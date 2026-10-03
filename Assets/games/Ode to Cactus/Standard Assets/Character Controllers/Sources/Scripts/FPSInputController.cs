using UnityEngine;
using Collection.Controls;

// In the collection: Unity's FPSInputController.js, translated line for line to C#.
// Unity 6 no longer compiles UnityScript. The file keeps the .js file's GUID, so the
// component on the First Person Controller is still bound to it.
namespace Games.OdeToCactus
{
	// Require a character controller to be attached to the same game object
	[RequireComponent(typeof(CharacterMotor))]
	[AddComponentMenu("Character/FPS Input Controller")]
	public class FPSInputController : MonoBehaviour
	{
		private CharacterMotor motor;

		// Use this for initialization
		void Awake ()
		{
			motor = GetComponent<CharacterMotor>();
		}

		// Update is called once per frame
		void Update ()
		{
			// Get the input vector from keyboard or analog stick
			Vector3 directionVector = new Vector3(TaloketoInputManager.GetAxis("Horizontal"), 0, TaloketoInputManager.GetAxis("Vertical"));

			if (directionVector != Vector3.zero)
			{
				// Get the length of the directon vector and then normalize it
				// Dividing by the length is cheaper than normalizing when we already have the length anyway
				float directionLength = directionVector.magnitude;
				directionVector = directionVector / directionLength;

				// Make sure the length is no bigger than 1
				directionLength = Mathf.Min(1, directionLength);

				// Make the input vector more sensitive towards the extremes and less sensitive in the middle
				// This makes it easier to control slow speeds when using analog sticks
				directionLength = directionLength * directionLength;

				// Multiply the normalized direction vector by the modified length
				directionVector = directionVector * directionLength;
			}

			// Apply the direction to the CharacterMotor
			motor.inputMoveDirection = transform.rotation * directionVector;
			motor.inputJump = TaloketoInputManager.GetButton("Jump");
		}
	}
}

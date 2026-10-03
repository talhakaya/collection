using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.MilkyMike
{
	public class Bug : Gadget {
	    public static Bug instance;
	    private Person person;

		void Awake () {
	        person = GetComponent<Person>();
	        instance = this;
	    }

	    void Update () {
	        person.SetInput(input.x, input.y, interactInput);

	        SetInput(0f, 0f);
	    }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Functions that are triggered when the fish enters the hiding place
public class RespondToHidingPlace : MonoBehaviour {

	public bool isInHiding = false;

	public void EnterHiding () {
		Debug.Log("called EnterHiding");
	}

}

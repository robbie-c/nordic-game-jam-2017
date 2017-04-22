using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingPlace : MonoBehaviour {


	void OnTriggerEnter (Collider other) {

		GameObject otherObject = other.gameObject;
		RespondToHidingPlace actionScript = otherObject.GetComponent<RespondToHidingPlace>();
		 
		// actionScript.isInHiding = true;
		actionScript.EnterHiding();

	}


}

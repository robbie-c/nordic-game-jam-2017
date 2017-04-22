using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AssemblyCSharp;

public class HidingPlace : MonoBehaviour {

	Vector3 middleOfHiding;

	void Start () {
		middleOfHiding = transform.position;
	}

	void OnTriggerEnter (Collider other) {

		GameObject otherObject = other.gameObject;
		RespondToHidingPlace actionScript = otherObject.GetComponent<RespondToHidingPlace>();
		 
		// actionScript.isInHiding = true;
		actionScript.EnterHiding(middleOfHiding);

		// Tell the server that this player is now hiding
		DummyPlayer dummyScript = otherObject.GetComponent<DummyPlayer>();
		dummyScript.frozen = true;

	}

	public void SetHidingPlaceIndex(int hidingPlaceIndex) {
		this.transform.position = Constants.kHidingPlaces [hidingPlaceIndex];
	}

	public static HidingPlace GetRoot() {
		var hidingPlaceObj = GameObject.Find("/HidingArea");
		var hidingPlace = hidingPlaceObj.GetComponent<HidingPlace> ();

		return hidingPlace;
	}
}

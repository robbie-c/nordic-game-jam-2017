using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Functions that are triggered when the fish enters the hiding place
public class RespondToHidingPlace : MonoBehaviour {

	Rigidbody playerRigidbody;

	public float hidingSpeed = 0.05f;
	public float lookRotationSpeed = 0.25f;
	public bool isInHiding = false;
	public Vector3 middleOfHiding;
	public float closeEnoughToMiddle = 0.1f;
	public float timerMoveToMiddle = 3f; // How long the fish will attemt to move to the middle before giving up

	Quaternion currentRotation;
	Vector3 hidingPlaceLookDirection = Vector3.up;
	Quaternion hidingPlaceLookRotation;

	public void EnterHiding (Vector3 middle) {
		Debug.Log("called EnterHiding at position: " + middle);
		GetComponent<PlayerMovement>().enabled = false; // Take control away from the player
		// Push the fish towards the middle of the hiding place
		middleOfHiding = middle;
		currentRotation = Quaternion.LookRotation(transform.forward);
		isInHiding = true;
	}

	void Start () {
		playerRigidbody = GetComponent<Rigidbody> ();
		hidingPlaceLookRotation = Quaternion.LookRotation(hidingPlaceLookDirection);
	}

	void FixedUpdate () {
		if (isInHiding && timerMoveToMiddle > 0f) {
			timerMoveToMiddle -= Time.deltaTime;
			MoveTowardsMiddle(middleOfHiding, closeEnoughToMiddle);	
			
		}
		if (isInHiding) RotateToFaceUp();
	}

	void MoveTowardsMiddle (Vector3 middle, float threshold) {
		float distanceToMiddle = (middle - transform.position).magnitude;
		if (distanceToMiddle > threshold) {
			playerRigidbody.MovePosition (transform.position + hidingSpeed * (middle - transform.position).normalized);
		}
	}

	void RotateToFaceUp () {
		// transform.rotation = Quaternion.Slerp(currentRotation, hidingPlaceLookRotation, Time.time * lookRotationSpeed);

        float step = lookRotationSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, Vector3.up, step, 0.0F);
        Debug.DrawRay(transform.position, newDir, Color.red);
        transform.rotation = Quaternion.LookRotation(newDir);

	}

	void MovePlayerCamera () {

	}

}

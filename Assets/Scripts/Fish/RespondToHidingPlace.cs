﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Functions that are triggered when the fish enters the hiding place
public class RespondToHidingPlace : MonoBehaviour {

	Rigidbody playerRigidbody;

	public float hidingSpeed = 0.05f;
	public float lookRotationSpeed = 1.3f;
	public bool isInHiding = false;
	public Vector3 middleOfHiding;
	public float closeEnoughToMiddle = 0.1f;
	public float timerMoveToMiddle = 3f; // How long the fish will attemt to move to the middle before giving up
	public float hidingCameraDuration = 16f;

	private float startTime;
	Vector3 hidingPlaceLookDirection = Vector3.up;
	Quaternion hidingPlaceLookRotation;
	Camera cam;
	private PlayerMovement playerMovementScript;

	private DummyPlayer dummyPlayer;

	public void EnterHiding (Vector3 middle) {
		Debug.Log(this + " called EnterHiding at position: " + middle);
		playerMovementScript = GetComponent<PlayerMovement>();
		if (playerMovementScript != null) playerMovementScript.enabled = false; // Take control away from the player
		dummyPlayer.frozen = true;
		// Push the fish towards the middle of the hiding place
		middleOfHiding = middle;
		startTime = Time.time;
		isInHiding = true;
	}

	void Start () {
		playerRigidbody = GetComponent<Rigidbody> ();
		hidingPlaceLookRotation = Quaternion.LookRotation(hidingPlaceLookDirection);
		
		cam = GameObject.Find("Camera").GetComponent<Camera>();
		dummyPlayer = GetComponent<DummyPlayer> ();

		Debug.Log("cam = " + cam);
	}

	void FixedUpdate () {
		if (isInHiding && timerMoveToMiddle > 0f) {
			timerMoveToMiddle -= Time.deltaTime;
			MoveTowardsMiddle(middleOfHiding, closeEnoughToMiddle);	
			
		}
		if (isInHiding) {
			RotateToFaceUp();
			if (playerMovementScript != null) MovePlayerCamera(middleOfHiding + Vector3.up * 10f, Vector3.down);
		}
	}

	void MoveTowardsMiddle (Vector3 middle, float threshold) {
		float distanceToMiddle = (middle - transform.position).magnitude;
		if (distanceToMiddle > threshold) {
			playerRigidbody.velocity = Vector3.zero;
			playerRigidbody.isKinematic = true;
			playerRigidbody.MovePosition (transform.position + hidingSpeed * (middle - transform.position).normalized);
		}
	}

	void RotateToFaceUp () {
        float step = lookRotationSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, Vector3.up, step, 0.0f);
        Debug.DrawRay(transform.position, newDir, Color.red);
        transform.rotation = Quaternion.LookRotation(newDir);

	}

	void MovePlayerCamera (Vector3 position, Vector3 direction) {

		// Move camera
		float fracComplete = (Time.time - startTime) / hidingCameraDuration;
		cam.transform.position = Vector3.Slerp(transform.position, position, fracComplete);


		// Rotate camera 
		float step = lookRotationSpeed * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards(cam.transform.forward, direction, step, 0.0f);
		cam.transform.rotation = Quaternion.LookRotation(newDir);

	}

}

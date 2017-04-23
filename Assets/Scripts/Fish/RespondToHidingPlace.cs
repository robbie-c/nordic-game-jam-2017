using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Functions that are triggered when the fish enters the hiding place
public class RespondToHidingPlace : MonoBehaviour {

	Rigidbody playerRigidbody;

	public float hidingSpeed = 0.05f;
	public float lookRotationSpeed = 1.3f;
	public Vector3 middleOfHiding;
	public float closeEnoughToMiddle = 2f;
	// public float timerMoveToMiddle = 3f; // How long the fish will attemt to move to the middle before giving up
	public float hidingCameraDuration = 16f;

	private float startTime;
	private Vector3 hidingPlaceLookDirection = Vector3.up;
	private Quaternion hidingPlaceLookRotation;
	private Vector3 entryPoint; // The place where the player first enters the hiding are is used as the reference for moving the camera to prevent it from shaking with the player
	private Camera cam;
	private PlayerMovement playerMovementScript;
	private GameObject sardineModel;


	private DummyPlayer dummyPlayer;

	private Quaternion originalCamRotation;
	private Vector3 originalCamPosition;
	private Transform oldParent;

	public void EnterHiding (Vector3 middle) {
		Debug.Log(this + " called EnterHiding at position: " + middle);
		playerMovementScript = GetComponent<PlayerMovement>();
		if (playerMovementScript != null) playerMovementScript.enabled = false; // Take control away from the player
		dummyPlayer.frozen = true;
		// Push the fish towards the middle of the hiding place
		middleOfHiding = middle;
		startTime = Time.time;
		dummyPlayer.frozen = true;
		entryPoint = transform.position;

		if (playerMovementScript != null) {
			oldParent = cam.transform.parent;
			cam.transform.parent = null; // Detach camera from player
		}
	}

	public void ExitHiding () {
		cam.transform.parent = oldParent;
		cam.transform.localRotation = originalCamRotation;
		cam.transform.localPosition = originalCamPosition;

		oldParent = null;
		playerRigidbody.transform.rotation = Quaternion.identity;
	}

	void Start () {
		playerRigidbody = GetComponent<Rigidbody> ();
		hidingPlaceLookRotation = Quaternion.LookRotation(hidingPlaceLookDirection);
		
		cam = GameObject.Find("/BasicFish/Camera").GetComponent<Camera>();
		// sardineModel = this.gameObject.transform.GetChild(0);
		dummyPlayer = GetComponent<DummyPlayer> ();

		Debug.Log("cam = " + cam);
		originalCamPosition = cam.transform.localPosition;
		originalCamRotation = cam.transform.localRotation;
	}

	void FixedUpdate () {
		// if (isInHiding && timerMoveToMiddle > 0f) {
		if (dummyPlayer.frozen) {
			// timerMoveToMiddle -= Time.deltaTime;
			MoveTowardsMiddle(middleOfHiding, closeEnoughToMiddle);	
			
		}
		if (dummyPlayer.frozen) {
			RotateToFaceUp();
			if (playerMovementScript != null) MovePlayerCamera(middleOfHiding + Vector3.up * 10f, Vector3.down);
		}

		if (!dummyPlayer.frozen && oldParent) {
			ExitHiding ();
		}
	}

	void MoveTowardsMiddle (Vector3 middle, float threshold) {
		float distanceToMiddle = (middle - transform.position).magnitude;
		if (distanceToMiddle > threshold) {
			playerRigidbody.velocity = Vector3.zero;
			playerRigidbody.isKinematic = false;
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
		cam.transform.position = Vector3.Slerp(entryPoint, position, fracComplete);


		// Rotate camera 
		float step = lookRotationSpeed * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards(cam.transform.forward, direction, step, 0.0f);
		cam.transform.rotation = Quaternion.LookRotation(newDir);

	}

}

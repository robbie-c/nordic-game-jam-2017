using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Keeps track of player velocity, position and state
public class PlayerMovement : MonoBehaviour {

	// Controls
	public string keyAccelerate = "w"; // This will be removed from the relase version
	public string keyTurnLeft = "a";
	public string keyTurnRight = "d";
	public string keyFastTurn = "space";

	// Player position etc
	Rigidbody playerRigidbody;

	// Player stats
	public float acceleration = 0.01f;
	public float wiggleAcceleration = 0.8f;
	public float dragCoefficient = 0.96f;
	// public float minimumSpeed = 0.01f;
	public float normalTurningSpeed = 500f;
	public float fastTurningSpeed = 1000f;
	public float wiggleOptimumButtonInterval = 0.25f;
	public float wiggleDuration = 0.05f;

	float minimumVelocity = 0.00001f;
	float turningSpeed;

	// Turning sigmoid function parameters
	// 1/(1+e^( turnDelay - turnResponsiveness * x))
	public float turnDelay = 5f;
	public float turnResponsiveness = 8f;

	// Player states
	bool wantsToTurnLeft;
	bool wantsToTurnRight;
	bool wantsToAccelerate;
	float timeAfterTurnButtonDown = 0.0f;
	float timeAfterTurnButtonUp = 0.0f;
	float timeSinceLastWiggle = 0.0f;
	// float wiggleAcceleration = 0.0f;
	// bool isWiggling = false;
	// bool turnButtonIsPressed = false;

	float startTouchPosition;
	float endTouchPosition;

	public Button yourButton;

	public void ResetMovementBools () {
		wantsToAccelerate = false;
		wantsToTurnLeft = false;
		wantsToTurnRight = false;
	}

	void Awake () {
		playerRigidbody = GetComponent<Rigidbody> ();
		turningSpeed = normalTurningSpeed;
	}

	// Use this for initialization
	void Start () { 
		timeAfterTurnButtonUp = 1f;
		timeAfterTurnButtonDown = 0f;
		playerRigidbody.velocity = Vector3.zero;
		timeSinceLastWiggle = 2f * wiggleDuration; // so that a wiggle does not happen on spawn

		bool isMobile = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;

		// for Debug purposes
		 isMobile = true;

		if (!isMobile) {
			Debug.Log ("is not mobile");
			Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
			canvas.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		ReadPlayerKeys ();

		if (Input.GetKey(keyFastTurn)) {
			turningSpeed = fastTurningSpeed;
		} else {
			turningSpeed = normalTurningSpeed;	
		}
	}

	void ReadPlayerKeys() {
		if (Input.GetKeyDown(keyTurnLeft)) {
			OnTouchDownLeft ();
		} 
		if (Input.GetKeyUp(keyTurnLeft)) {
			OnTouchUpLeft ();
		}
		if (Input.GetKeyDown (keyTurnRight)) {
			OnTouchDownRight ();
		} 
		if (Input.GetKeyUp(keyTurnRight)) {
			OnTouchUpRight ();
		}
		if (Input.GetKeyDown (keyAccelerate)) {
			OnTouchDownUp ();
		} 
		if (Input.GetKeyUp(keyAccelerate)) {
			OnTouchUpUp ();
		}
	}

	void CheckWiggle() {
		timeAfterTurnButtonDown = 0.0f;
		// Determine if a fish body wiggle is implied by the button presses
		if (timeAfterTurnButtonUp < 2f * wiggleOptimumButtonInterval) {
			// isWiggling = true;
			timeSinceLastWiggle = 0f;
		}
	}

	public void OnTouchDownRight() {
		Debug.Log ("turning right");
		timeAfterTurnButtonDown = 0.0f;
		wantsToTurnRight = true;
		CheckWiggle ();
	}

	public void OnTouchUpRight() {
		Debug.Log ("stop turning right");
		timeAfterTurnButtonUp = 0.0f;
		wantsToTurnRight = false;
	}

	public void OnTouchDownLeft() {
		Debug.Log ("turning left");
		timeAfterTurnButtonDown = 0.0f;
		wantsToTurnLeft = true;
		CheckWiggle ();
	}

	public void OnTouchUpLeft() {
		Debug.Log ("stop turning left");
		timeAfterTurnButtonUp = 0.0f;
		wantsToTurnLeft = false;
	}

	public void OnTouchDownUp() {
		Debug.Log ("turning up");
		timeAfterTurnButtonDown = 0.0f;
		wantsToAccelerate = true;
		CheckWiggle ();
	}

	public void OnTouchUpUp() {
		Debug.Log ("stop turning up");
		timeAfterTurnButtonUp = 0.0f;
		wantsToAccelerate = false;
	}

	void FixedUpdate () {
		// Apply drag
		playerRigidbody.velocity = UpdateVelocity(playerRigidbody.velocity);

		timeSinceLastWiggle += Time.deltaTime;
		
		// Update time since turn button was pressed
		if (timeAfterTurnButtonDown >= 0.0f && (wantsToTurnLeft || wantsToTurnRight)) timeAfterTurnButtonDown += Time.deltaTime;

		// Update time since turn button was released
		if (timeAfterTurnButtonUp >= 0.0f && !(wantsToTurnLeft || wantsToTurnRight)) timeAfterTurnButtonUp += Time.deltaTime;

		// Apply changes due to player input
		if (wantsToTurnRight && !wantsToTurnLeft) {
			Turn(turningSpeed * CalculateTurningSpeedFactor(timeAfterTurnButtonDown, timeAfterTurnButtonUp), true);
		}
		if (wantsToTurnLeft && !wantsToTurnRight) {
			Turn(turningSpeed * CalculateTurningSpeedFactor(timeAfterTurnButtonDown, timeAfterTurnButtonUp), false);
		}
		if (wantsToAccelerate) {
			playerRigidbody.velocity = Accelerate(playerRigidbody.velocity, acceleration);
		}
		if (timeSinceLastWiggle <= wiggleDuration) {
			playerRigidbody.velocity = Accelerate(playerRigidbody.velocity, 
				CalculateWiggleAcceleration(wiggleAcceleration, timeAfterTurnButtonUp, wiggleOptimumButtonInterval)
			);
			// isWiggling = false; 
		}
	}

	void Turn(float speed, bool right) {
		Debug.Log("Turning");
		if (right) {
			// playerRigidbody.Rotate(Vector3.right * Time.deltaTime * speed, Space.World);
			playerRigidbody.transform.Rotate(0.0f, Time.deltaTime * speed, 0.0f, Space.World);
		} else {
			// playerRigidbody.Rotate(Vector3.left * Time.deltaTime * speed, Space.World);
			playerRigidbody.transform.Rotate(0.0f, Time.deltaTime * -speed, 0.0f, Space.World);
		}
		
	}

	// Function for calculating current turning speed based on time since the turn button was pressed or released
	float CalculateTurningSpeedFactor (float t1, float t2) {
		float delta = t1 - t2;
		if (delta < 0.0f) {
			delta = 0f;
			timeAfterTurnButtonUp = 0f;
		}
		return 1f / (1f + Mathf.Exp( turnDelay - turnResponsiveness * delta ));
	}

	float CalculateWiggleAcceleration (float acceleration, float timeSinceUp, float threshold) {
		if (timeSinceUp < threshold) {
			return acceleration;
		} else {
			return acceleration * (1f - Mathf.Pow((timeSinceUp / threshold) - 1f, 2f) ); // (1-((x/0.5)-1)^2)
		}
	}

	Vector3 Accelerate (Vector3 velocity, float acceleration) {
		return velocity + playerRigidbody.transform.forward * acceleration;
	}

	Vector3 UpdateVelocity (Vector3 velocity) {
		if (velocity.magnitude > minimumVelocity) {
			return velocity * dragCoefficient;	
		} else {
			return Vector3.zero;
		}
		
	}
}

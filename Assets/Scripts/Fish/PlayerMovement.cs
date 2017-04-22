using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Keeps track of player velocity, position and state
public class PlayerMovement : MonoBehaviour {

	// Controls
	public string keyAccelerate = "w"; // This will be removed from the relase version
	public string keyTurnLeft = "a";
	public string keyTurnRight = "d";
	public string keyFastTurn = "space";

	// Player position etc
	Rigidbody playerRigidbody;
	public Vector3 velocity;

	// Player stats
	public float acceleration = 0.025f;
	public float dragCoefficient = 0.95f;
	// public float minimumSpeed = 0.01f;
	public float normalTurningSpeed = 500f;
	public float fastTurningSpeed = 1000f;
	public float wiggleOptimumTime = 0.5f;
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
	// float wiggleAcceleration = 0.0f;
	bool isWiggling = false;
	// bool turnButtonIsPressed = false;

	void Awake () {
		playerRigidbody = GetComponent<Rigidbody> ();
		turningSpeed = normalTurningSpeed;
	}

	// Use this for initialization
	void Start () {
		timeAfterTurnButtonUp = 1f;
		timeAfterTurnButtonDown = 0f;
		velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		ResetControlStates();
		// Rect to player input
		if (Input.GetKey(keyTurnLeft)) wantsToTurnLeft = true;
		if (Input.GetKey(keyTurnRight)) wantsToTurnRight = true;
		if (Input.GetKey(keyAccelerate)) wantsToAccelerate = true;
		if (Input.GetKey(keyFastTurn)) {
			turningSpeed = fastTurningSpeed;
		} else {
			turningSpeed = normalTurningSpeed;	
		}
		// Reset turn button timers
		if (Input.GetKeyDown(keyTurnRight) || Input.GetKeyDown(keyTurnLeft)) {
			timeAfterTurnButtonDown = 0.0f;
			// Determine if a fish body wiggle is implied by the button presses
			if (timeAfterTurnButtonUp < 2f * wiggleOptimumTime) {
				isWiggling = true;
			}
		}
		if (Input.GetKeyUp(keyTurnRight) || Input.GetKeyUp(keyTurnLeft)) timeAfterTurnButtonUp = 0.0f;
	}

	void FixedUpdate () {
		// Apply drag
		velocity = UpdateVelocity(velocity);
		
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
			velocity = Accelerate(velocity, acceleration);
		}
		if (isWiggling) {
			Accelerate(velocity, 
				CalculateWiggleAcceleration(acceleration, timeAfterTurnButtonUp, wiggleOptimumTime)
			);
			isWiggling = false; 
		}


		// Move the player collider
		UpdatePosition(velocity);
		
	}

	void ResetControlStates () {
		wantsToAccelerate = false;
		wantsToTurnRight = false;
		wantsToTurnLeft = false;
	}

	void Turn(float speed, bool right) {
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
		return acceleration * (1f - Mathf.Pow((timeSinceUp / threshold) - 1f, 2f) ); // (1-((x/0.5)-1)^2)
	}

	Vector3 Accelerate (Vector3 velocity, float acceleration) {
		// Debug.Log (playerRigidbody.forward);
		// playerRigidbody.position += playerRigidbody.forward *= 0.1f;
		return velocity + playerRigidbody.transform.forward * acceleration;
	}

	Vector3 UpdateVelocity (Vector3 velocity) {
		return velocity * dragCoefficient;
	}

	void UpdatePosition (Vector3 velocity) {
//		Debug.Log(velocity.magnitude);
		playerRigidbody.MovePosition (transform.position + velocity);
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Keeps track of player velocity, position and state
public class PlayerMovement : MonoBehaviour {

	// Controls
	public string keyAccelerate = "w";
	public string keyTurnLeft = "a";
	public string keyTurnRight = "d";

	// Player position etc
	public Transform playerLocation;
	public Vector3 velocity;
	Quaternion direction;

	// Player stats
	public float acceleration = 0.001f;
	public float turningSpeed = 10f;
	public float dragCoefficient = 0.1f;
	public float minimumSpeed = 0.01f;

	// Player states
	bool wantsToTurnLeft;
	bool wantsToTurnRight;
	bool wantsToAccelerate;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		ResetControlStates();
		if (Input.GetKey(keyTurnLeft)) wantsToTurnLeft = true;
		if (Input.GetKey(keyTurnRight)) {
			wantsToTurnRight = true;
		}
		if (Input.GetKey(keyAccelerate)) wantsToAccelerate = true;
	}

	void FixedUpdate () {
		velocity = UpdateVelocity(Vector3.up * 0.001f);
		UpdatePosition(velocity);
		if (wantsToTurnRight && !wantsToTurnLeft) {
			Debug.Log("Wants to turn right");
			Turn(turningSpeed, true);
		}
		if (wantsToTurnLeft && !wantsToTurnRight) {
			Turn(turningSpeed, false);
		}
		if (wantsToAccelerate) {
			Accelerate(acceleration);
		}
	}

	void ResetControlStates () {
		wantsToAccelerate = false;
		wantsToTurnRight = false;
		wantsToTurnLeft = false;
	}

	void Turn(float speed, bool right) {
		Debug.Log("called Turn");
		playerLocation.Rotate(0.0f, 0.0f, Time.deltaTime * speed);
	}

	void Accelerate (float acceleration) {

	}

	Vector3 UpdateVelocity (Vector3 acceleration) {
		return velocity = velocity * dragCoefficient;
	}

	void UpdatePosition (Vector3 velocity) {
		playerLocation.position += velocity;
	}

}

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
	Rigidbody playerRigidbody;
	public Vector3 velocity;

	// Player stats
	public float acceleration = 0.001f;
	public float turningSpeed = 100f;
	public float dragCoefficient = 0.1f;
	public float minimumSpeed = 0.01f;

	// Player states
	bool wantsToTurnLeft;
	bool wantsToTurnRight;
	bool wantsToAccelerate;

	void Awake () {
		playerRigidbody = GetComponent<Rigidbody> ();
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		ResetControlStates();
		if (Input.GetKey(keyTurnLeft)) wantsToTurnLeft = true;
		if (Input.GetKey(keyTurnRight)) wantsToTurnRight = true;
		if (Input.GetKey(keyAccelerate)) wantsToAccelerate = true;
	}

	void FixedUpdate () {
		// Apply drag
		velocity = UpdateVelocity(Vector3.up * 0.001f);
		
		// Apply changes due to player input
		if (wantsToTurnRight && !wantsToTurnLeft) {
			Turn(turningSpeed, true);
		}
		if (wantsToTurnLeft && !wantsToTurnRight) {
			Turn(turningSpeed, false);
		}
		if (wantsToAccelerate) {
			velocity = Accelerate(acceleration);
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

	Vector3 Accelerate (float acceleration) {
		// Debug.Log (playerRigidbody.forward);
		// playerRigidbody.position += playerRigidbody.forward *= 0.1f;
		return velocity += playerRigidbody.transform.forward * 0.1f;
	}

	Vector3 UpdateVelocity (Vector3 acceleration) {
		return velocity = velocity * dragCoefficient;
	}

	void UpdatePosition (Vector3 velocity) {
		playerRigidbody.MovePosition (transform.position + velocity);
	}

}

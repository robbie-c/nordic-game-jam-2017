using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Keeps track of player velocity, position and state
public class PlayerMovement : MonoBehaviour {

	public Transform playerLocation;
	public Vector2 velocity;
	public Vector2 position;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate () {
		velocity = UpdateVelocity(Vector2.up * 0.001f);
		position = UpdatePosition(velocity);
		playerLocation.position = position;
		
	}

	Vector2 UpdateVelocity (Vector2 acceleration) {
		return velocity + acceleration;
	}

	Vector2 UpdatePosition (Vector2 velocity) {
		return position + velocity;
	}
}

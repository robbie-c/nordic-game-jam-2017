using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : MonoBehaviour {

	public Vector3 desiredPosition;
	public Quaternion desiredRotation;
	public Vector3 desiredVelocity;

	public float PositionLerpSpeed = 1;
	public float RotationLerpSpeed = 1;
	public float VelocityLerpSpeed = 1;

	// Use this for initialization
	void Start () {
		var rigidBody = GetComponent<Rigidbody> ();
		desiredPosition = rigidBody.position;
		desiredRotation = rigidBody.rotation;
		desiredVelocity = rigidBody.velocity;
	}
	
	// Update is called once per frame
	void Update () {
		var rigidBody = GetComponent<Rigidbody> ();
		rigidBody.position = Vector3.Lerp (rigidBody.position, desiredPosition, Time.deltaTime * PositionLerpSpeed);
		var hackyAngles = desiredRotation.eulerAngles;
		hackyAngles.y += -90;
		var hackyRotation = Quaternion.Euler (hackyAngles);
		rigidBody.rotation = Quaternion.Lerp (rigidBody.rotation, hackyRotation, Time.deltaTime * RotationLerpSpeed);
		rigidBody.velocity = Vector3.Lerp (rigidBody.velocity, desiredVelocity, Time.deltaTime * VelocityLerpSpeed);
	}
}

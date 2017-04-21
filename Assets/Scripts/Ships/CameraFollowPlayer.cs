using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour {

	public Transform playerLocation;
	public float playerCamSmoothing = 50f;
	public bool playerCameraFollow = false;
	public float cameraSpeed = 0.4f;

	// Camera movement
	float camX;
	float camY;
	float camZ;

	Vector3 offset;

	void Start(){
		offset = transform.position - playerLocation.position;
	}

	void Update() {
		if (Input.GetKeyDown ("j"))
			playerCameraFollow = !playerCameraFollow;
	}

	void FixedUpdate() { // fixed because following a physics object
		camX = 0.0f;
		//camY = 0.0f;
		camY = 0.0f;

		if (Input.GetKey ("w")){
			playerCameraFollow = false;
			camY = camY + 1.0f*cameraSpeed;
			//print("w"+camY);
		}
		if (Input.GetKey ("a")){
			playerCameraFollow = false;
			camX = camX + -1.0f*cameraSpeed;
			//print("a"+camX);
		}
		if (Input.GetKey ("d")){
			playerCameraFollow = false;
			camX = camX + 1.0f*cameraSpeed;
			//print("d"+camX);
		}
		if (Input.GetKey ("s")){
			playerCameraFollow = false;
			camY = camY + -1.0f*cameraSpeed;
			//print("s"+camY);
		}

		if (playerCameraFollow){
			Vector3 targetCamPos = playerLocation.position + offset;
			transform.position = Vector3.Lerp (transform.position, targetCamPos, playerCamSmoothing * Time.deltaTime);
		} else {
			Vector3 targetCamPos = transform.position;
			//print(targetCamPos);
			targetCamPos.x = targetCamPos.x + camX;
			targetCamPos.y = targetCamPos.z + camY;
			//print(targetCamPos);
			transform.position = Vector3.Lerp (transform.position, targetCamPos, playerCamSmoothing * Time.deltaTime);
		}

	}
} // end class PlayerCamera

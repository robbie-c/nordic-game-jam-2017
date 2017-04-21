using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class DummyPlayer : MonoBehaviour {

	private ServerCommunication serverCommunication;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (serverCommunication == null) {
			serverCommunication = ServerCommunication.GetRoot ();
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			string text = ClientMessageCoordinator.createGameStateMessage (transform.position, transform.forward);
			serverCommunication.SendClientGameStateMessage (text);
		}

		ServerGameStateMessage serverMessage;
		if (serverCommunication.TryGetServerGameStateMessage(out serverMessage)) {
			Debug.Log("Server sent: " + serverMessage.text);
		}
	}
}

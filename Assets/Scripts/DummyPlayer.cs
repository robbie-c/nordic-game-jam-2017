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
			serverCommunication.SendClientMessage (new ClientMessage ("up"));
		}

		ServerMessage serverMessage;
		if (serverCommunication.TryGetServerMessage(out serverMessage)) {
			Debug.Log(serverMessage.text);
		}
	}
}

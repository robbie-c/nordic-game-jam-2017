using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class DummyPlayer : MonoBehaviour {

	private ServerCommunication serverCommunication;
	private bool exploded;

	// Use this for initialization
	void Start () {
		exploded = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (serverCommunication == null) {
			serverCommunication = ServerCommunication.GetRoot ();
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			serverCommunication.SendClientGameStateMessage (createGameStateMessage());
		}

		ServerGameStateMessage serverMessage;
		if (serverCommunication.TryGetServerGameStateMessage(out serverMessage)) {
			Debug.Log("Server sent: " + serverMessage.text);
		}
	}

	private string createGameStateMessage() 
	{
		ClientGameStateMessage playerPosObj = new ClientGameStateMessage(transform.position, transform.forward, exploded);
		return JsonUtility.ToJson(playerPosObj);
	}
}

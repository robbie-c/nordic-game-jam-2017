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

		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			serverCommunication.SendClientGameStateMessage (createHelloMessage());
			serverCommunication.SendClientGameStateMessage (createDiedMessage());
		}

		ServerGameStateMessage serverMessage;
		if (serverCommunication.TryGetServerGameStateMessage(out serverMessage)) {
			Debug.Log("Server sent: " + serverMessage.text);
		}
	}

	private string createGameStateMessage() 
	{
		return JsonUtility.ToJson(
			new ClientGameStateMessage(transform.position, transform.forward, exploded)
		);
	}

	private string createHelloMessage() 
	{
		return JsonUtility.ToJson(new HelloMessage());
	}

	private string createDiedMessage() 
	{
		return JsonUtility.ToJson(new DiedMessage());
	}
}

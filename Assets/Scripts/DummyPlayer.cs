using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class DummyPlayer : MonoBehaviour {

	private ServerCommunication serverCommunication;
	private bool frozen;

	// Use this for initialization
	void Start () {
		frozen = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (serverCommunication == null) {
			serverCommunication = ServerCommunication.GetRoot ();
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			serverCommunication.SendClientWebSocketMessage ("up");
			sendGameStateMessage ();
		}

		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			sendHelloMessage ();
			sendGameStateMessage ();
		}

		string serverMessage;
		if (serverCommunication.TryGetServerUdpMessage(out serverMessage)) {
			Debug.Log("Server sent UDP: " + serverMessage);
		}
		if (serverCommunication.TryGetServerWebSocketMessage(out serverMessage)) {
			Debug.Log("Server sent WS : " + serverMessage);
		}
	}

	private void sendHelloMessage() 
	{
		serverCommunication.SendClientUdpMessage (JsonUtility.ToJson(new HelloMessage()));
	}

	private void sendGameStateMessage() 
	{
		string text = JsonUtility.ToJson(
			new ClientGameStateMessage(transform.position, transform.forward, GetComponent<Rigidbody>().velocity, frozen)
		);
		serverCommunication.SendClientUdpMessage (text);
	}

	private void getGameState() 
	{
		ServerGameStateMessage message = serverCommunication.CheckForOtherClientGameStates ();
		// iterate through this and update players on screen
	}
}

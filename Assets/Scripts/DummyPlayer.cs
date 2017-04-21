using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;
using System.Collections.Generic;


public class DummyPlayer : MonoBehaviour {

	private ServerCommunication serverCommunication;
	private bool frozen;
	private int id;

	// Use this for initialization
	void Start () {
		frozen = false;

		// TODO: After hello handshake, set the id. For now, use dummy value
		id = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (serverCommunication == null) {
			serverCommunication = ServerCommunication.GetRoot ();
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			serverCommunication.SendClientWebSocketMessage ("up");
			SendGameStateMessage ();
		}

		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			SendHelloMessage ();
			SendGameStateMessage ();
		}

		string serverMessage;
		if (serverCommunication.TryGetServerUdpMessage(out serverMessage)) {
			Debug.Log("Server sent UDP: " + serverMessage);
		}
		if (serverCommunication.TryGetServerWebSocketMessage(out serverMessage)) {
			Debug.Log("Server sent WS : " + serverMessage);
		}
	}

	private void SendHelloMessage() 
	{
		serverCommunication.SendClientWebSocketMessage (JsonUtility.ToJson(new HelloMessage()));
	}

	private void SendGameStateMessage() 
	{
		string text = JsonUtility.ToJson(
			new ClientGameStateMessage(
				id,
				transform.position, 
				transform.forward, 
				GetComponent<Rigidbody>().velocity, 
				frozen
			)
		);
		serverCommunication.SendClientUdpMessage (text);
	}

	private void GetGameStateFromOtherClients() 
	{
		ServerGameStateMessage message = serverCommunication.CheckForOtherClientGameStates ();
		// iterate through this and update players on screen
		foreach (ClientGameStateMessage client in message.clients)
		{
			Vector3 position = client.playerPosition.ToVector3 ();
			Vector3 velocity = client.playerVelocity.ToVector3 ();
			Vector3 direction = client.playerDirection.ToVector3 ();
			int id = client.id;

			// TODO: do something with this info
		}

	}
}

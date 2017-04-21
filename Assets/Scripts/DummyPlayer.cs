using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;


public class DummyPlayer : MonoBehaviour {

	public GameObject prefab;
	private ServerCommunication serverCommunication;
	private bool frozen;
	private int id;
	private Dictionary<int, GameObject> otherPlayers;


	void Start () {
		frozen = false;
		id = -1;
		serverCommunication = ServerCommunication.GetRoot ();
	}
	

	void Update () {

		// Debug code
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			SendGameStateMessage ();
		}

		QueryUDPConnections ();
		QueryWebSocketConnections ();
	}

	private void QueryWebSocketConnections() {
		string serverMessage;
		if (serverCommunication.TryGetServerWebSocketMessage(out serverMessage)) {
			Debug.Log("Server sent WS : " + serverMessage);

			if (serverMessage.Contains ("ServerToClientHelloMessage")) {
				Debug.Log("Setting ID and position");
				ServerToClientHelloMessage message = JsonUtility.FromJson<ServerToClientHelloMessage> (serverMessage);
				this.id = message.id;
				this.transform.position = message.initialPosition.ToVector3 ();
			}

		}
	}

	private void SendGameStateMessage() 
	{
		if (id == -1) {
			Debug.Log ("ID not set yet!!");
			return;
		}

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

	private void QueryUDPConnections()
	{
		ServerGameStateMessage message = serverCommunication.CheckForOtherClientGameStates ();
		if (message == null) {
			return;
		}

		ProcessOtherPlayers (message);
	}

	private void ProcessOtherPlayers(ServerGameStateMessage message) {
		foreach (ClientGameStateMessage client in message.clients)
		{
			Vector3 position = client.playerPosition.ToVector3 ();
			Vector3 velocity = client.playerVelocity.ToVector3 ();
			Vector3 direction = client.playerDirection.ToVector3 ();
			int otherId = client.id;

			if (otherId == this.id) {
				// go to next iteration of loop
				continue;
			} 

			GameObject otherPlayer;
			if (otherPlayers.ContainsKey (id)) {
				otherPlayer = otherPlayers [id];
				otherPlayer.transform.position = position;
			} else {
				// create new other player and add it to the map
				otherPlayer = Instantiate(prefab, position, Quaternion.identity);
			}
			otherPlayer.GetComponent<Rigidbody>().velocity = velocity;
			otherPlayer.transform.forward = direction;
		}
	}
}

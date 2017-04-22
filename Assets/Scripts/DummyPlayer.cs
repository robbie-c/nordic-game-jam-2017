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
	private bool shouldSleep = true;

	void Start () {
		frozen = false;
		id = -1;
		otherPlayers = new Dictionary<int, GameObject> ();
		serverCommunication = ServerCommunication.GetRoot ();

		StartCoroutine("BackgroundSendGameStateToServerTask");
	}
	

	void Update () {
		QueryUDPConnections ();

		QueryWebSocketConnections ();
	}

	IEnumerator BackgroundSendGameStateToServerTask() {
		for(;;) {
			SendGameStateMessage ();
			yield return new WaitForSeconds(Constants.kGameStateUpdateTickMs / 1000.0f);
		}
	}

	private void QueryWebSocketConnections() {
		Debug.Log ("Querying for websockets");
		string serverMessage;
		if (serverCommunication.TryGetServerWebSocketMessage (out serverMessage)) {
			Debug.Log ("Server sent WS : " + serverMessage);

			if (serverMessage.Contains ("ServerToClientHelloMessage")) {
				Debug.Log ("Setting ID and position");
				ServerToClientHelloMessage message = JsonUtility.FromJson<ServerToClientHelloMessage> (serverMessage);
				this.id = message.id;
				this.transform.position = message.initialPosition.ToVector3 ();
			}
		} else {
			Debug.LogError ("Failed to get message from server");
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
		Debug.Log("sending udp");
		serverCommunication.SendClientUdpMessage (text);
	}

	private void QueryUDPConnections()
	{
		ServerGameStateMessage message = serverCommunication.CheckForOtherClientGameStates ();
		//Debug.Log (message);
		if (message == null) {
			return;
		}

		ProcessOtherPlayers (message);
	}

	private void ProcessOtherPlayers(ServerGameStateMessage message) {
		Debug.Log ("Processing other players, message: " + message);
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
				Debug.Log ("Found player with id " + id.ToString());
				otherPlayer = otherPlayers [id];
				otherPlayer.transform.position = position;
			} else {
				Debug.Log ("Creating new player with id: " + id.ToString());
				otherPlayer = Instantiate(prefab, position, Quaternion.identity);
				otherPlayers.Add (id, otherPlayer);
			}
			otherPlayer.GetComponent<Rigidbody>().velocity = velocity;
			otherPlayer.transform.forward = direction;
		}
	}
}

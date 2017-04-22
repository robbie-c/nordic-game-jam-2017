using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;
using UnityEngine.UI;


public class DummyPlayer : MonoBehaviour {

	public GameObject prefab;
	private ServerCommunication serverCommunication;
	public bool frozen;
	private int id;
	private int gameId;
	private Dictionary<int, GameObject> otherPlayers;
	private bool countingDown = false;
	private Text countdown;
	private bool waitingToStart = true;
	public int startSeconds = 3;
	public int finishSeconds = 10;
	private PlayerMovement playerMovement;
	private Image waitingForPlayersScreen;
	private Image finishScreen;
	private Image connectingScreen;
	private IEnumerator finalCountingDownCoroutine;
	private IEnumerator startCountingDownCoroutine;

	void Start () {
		frozen = false;
		id = -1;
		otherPlayers = new Dictionary<int, GameObject> ();
		serverCommunication = ServerCommunication.GetRoot ();
		countdown = GameObject.FindGameObjectWithTag ("countdown").GetComponent<Text> ();
		playerMovement = GetComponent<PlayerMovement> ();
		waitingForPlayersScreen = GameObject.FindGameObjectWithTag ("WaitingForPlayers").GetComponent<Image> ();
		finishScreen = GameObject.FindGameObjectWithTag ("finishScreen").GetComponent<Image> ();
		connectingScreen = GameObject.FindGameObjectWithTag ("Connecting").GetComponent<Image> ();
		finishScreen.enabled = false;
		waitingForPlayersScreen.enabled = false;
		connectingScreen.enabled = true;

		// TODO: in real game, this needs to be uncommented
//		playerMovement.enabled = false;

		countdown.enabled = false;

		bool isMobile = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;

		// for Debug purposes
		isMobile = true;

		if (!isMobile) {
			Debug.Log ("is not mobile");
			GameObject.FindGameObjectWithTag("rightButton").GetComponent<Button>().enabled = false;
			GameObject.FindGameObjectWithTag("leftButton").GetComponent<Button>().enabled = false;
		}

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
		string serverMessage;
		if (serverCommunication.TryGetServerWebSocketMessage (out serverMessage)) {
			Debug.Log ("Server sent WS : " + serverMessage);

			if (serverMessage.Contains ("ServerToClientHelloMessage")) {
				Debug.Log ("Setting ID and position");
				ServerToClientHelloMessage message = JsonUtility.FromJson<ServerToClientHelloMessage> (serverMessage);
				this.id = message.id;
				this.gameId = message.gameId;
				this.transform.position = message.initialPosition.ToVector3 ();
				connectingScreen.enabled = false;
				waitingForPlayersScreen.enabled = true;
				finishScreen.enabled = false;
			}

			else if (serverMessage.Contains("ServerToClientStartMessage")) {
				ServerToClientStartMessage message = JsonUtility.FromJson<ServerToClientStartMessage> (serverMessage);
				this.gameId = message.gameId;
				if (finalCountingDownCoroutine != null) {
					StopCoroutine (finalCountingDownCoroutine);
				}
				if (startCountingDownCoroutine != null) {
					return;
				}
				startCountingDownCoroutine = StartCountdown ();
				StartCoroutine(startCountingDownCoroutine);
				connectingScreen.enabled = false;
				waitingForPlayersScreen.enabled = false;
				finishScreen.enabled = false;
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
				frozen,
				gameId
			)
		);
		serverCommunication.SendClientUdpMessage (text);
	}

	private void QueryUDPConnections()
	{
		ServerGameStateMessage message = serverCommunication.CheckForOtherClientGameStates ();
		//Debug.Log (message);
		if (message == null) {
			return;
		}

		if (message.gameId == gameId) {
			ProcessOtherPlayers (message);
		}
	}

	private void ProcessOtherPlayers(ServerGameStateMessage message) {
//		Debug.Log ("Processing other players, message: " + message);
		foreach (ClientGameStateMessage client in message.clients)
		{
			Vector3 position = client.playerPosition.ToVector3 ();
			Vector3 velocity = client.playerVelocity.ToVector3 ();
			Vector3 direction = client.playerDirection.ToVector3 ();
			bool frozen = client.frozen;
			if (!countingDown && frozen) {
				countingDown = true;
				finalCountingDownCoroutine = FinalCountdown ();
				StartCoroutine(finalCountingDownCoroutine);
			}
			int otherId = client.id;

			if (otherId == this.id) {
				// go to next iteration of loop
				continue;
			} 

			GameObject otherPlayer;
			if (otherPlayers.ContainsKey (id)) {
//				Debug.Log ("Found player with id " + id.ToString());
				otherPlayer = otherPlayers [id];
				otherPlayer.transform.position = position;
			} else {
//				Debug.Log ("Creating new player with id: " + id.ToString());
				otherPlayer = Instantiate(prefab, position, Quaternion.identity);
				otherPlayers.Add (id, otherPlayer);
			}
			otherPlayer.GetComponent<Rigidbody>().velocity = velocity;
			otherPlayer.transform.forward = direction;
		}
	}

	IEnumerator FinalCountdown()
	{
		int progress = this.finishSeconds;
		this.enabled = true;

		while (progress >= 0)
		{
			countdown.text = progress.ToString();
			progress -= 1;
			yield return new WaitForSeconds(1);

		}
		yield return true;

		finalCountingDownCoroutine = null;

		FinishGame ();
	}

	IEnumerator StartCountdown()
	{
		int progress = this.startSeconds;

		while (progress >= 0)
		{
			countdown.text = progress.ToString();
			progress -= 1;
			yield return new WaitForSeconds(1);

		}
		yield return true;

		startCountingDownCoroutine = null;
		StartGame();
	}


	public void FinishGame() {
		playerMovement.enabled = false;
		finishScreen.enabled = true;
	}

	public void StartGame() {
		playerMovement.enabled = true;
	}
}

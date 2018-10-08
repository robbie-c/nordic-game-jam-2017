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
	private Text countdown;
	private bool waitingToStart = true;
	public int startSeconds = 0;
	public int finishSeconds = 10;
	private PlayerMovement playerMovement;
	private Image waitingForPlayersScreen;
	private Image winScreen;
	private Image loseScreen;
	private Image connectingScreen;
	private Image titleScreen;
	private IEnumerator finalCountingDownCoroutine;
	private IEnumerator startCountingDownCoroutine;

	private enum Status {
		titleScreen, connecting, waitingForPlayers, playing, lose, win
	}

	private Status status;

	void Start () {
		frozen = false;
		id = -1;
		gameId = -1;
		otherPlayers = new Dictionary<int, GameObject> ();
		serverCommunication = ServerCommunication.GetRoot ();
		countdown = GameObject.FindGameObjectWithTag ("countdown").GetComponent<Text> ();
		playerMovement = GetComponent<PlayerMovement> ();
		waitingForPlayersScreen = GameObject.FindGameObjectWithTag ("WaitingForPlayers").GetComponent<Image> ();
		connectingScreen = GameObject.FindGameObjectWithTag ("Connecting").GetComponent<Image> ();
		titleScreen = GameObject.FindGameObjectWithTag ("TitleScreen").GetComponent<Image> ();
		winScreen = GameObject.FindGameObjectWithTag ("WinScreen").GetComponent<Image> ();
		loseScreen = GameObject.FindGameObjectWithTag ("LoseScreen").GetComponent<Image> ();

		winScreen.enabled = false;
		loseScreen.enabled = false;
		waitingForPlayersScreen.enabled = false;
		connectingScreen.enabled = false;
		status = Status.titleScreen;

		//line below commented out for test
		// TODO: in real game, this needs to be uncommented
		//playerMovement.enabled = false;

		countdown.enabled = false;

		bool isMobile = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;

		// for Debug purposes
//		isMobile = true;

		if (!isMobile) {
			Debug.Log ("is not mobile");
			GameObject.FindGameObjectWithTag ("rightButton").SetActive(false);
			GameObject.FindGameObjectWithTag ("leftButton").SetActive(false);
		}

		StartCoroutine("BackgroundSendGameStateToServerTask");
	}

	public void EndTitleScreen() {
		titleScreen.gameObject.active = false;
		status = Status.connecting;
		//line below commented out for test
		//connectingScreen.enabled = true;
	}

	void Update () {
		ReadAndHandleReceivedMessages ();
	}

	IEnumerator BackgroundSendGameStateToServerTask() {
		for(;;) {
			SendGameStateMessage ();
			yield return new WaitForSeconds(Constants.kGameStateUpdateTickMs / 1000.0f);
		}
	}

	private void HandleServerToClientHelloMessage(ServerToClientHelloMessage message) {
		this.id = message.id;
		this.gameId = message.gameId;
		this.transform.position = message.playerPosition.ToVector3 ();
		connectingScreen.enabled = false;
		//line below commented out for test
		//waitingForPlayersScreen.enabled = true;
		winScreen.enabled = false;
		loseScreen.enabled = false;

		var hidingPlace = HidingPlace.GetRoot ();
		if (hidingPlace) {
			hidingPlace.SetHidingPlaceIndex (message.hidingPlace);
		}
		//line below commented out for test
		//status = Status.waitingForPlayers;
	}

	private void HandleServerToClientStartMessage(ServerToClientStartMessage message) {
		this.gameId = message.gameId;
		this.frozen = false;
		this.transform.position = message.playerPosition.ToVector3 ();
		if (finalCountingDownCoroutine != null) {
			StopCoroutine (finalCountingDownCoroutine);
			finalCountingDownCoroutine = null;
		}
		if (startCountingDownCoroutine != null) {
			StopCoroutine (startCountingDownCoroutine);
			startCountingDownCoroutine = null;
		}
		startCountingDownCoroutine = StartCountdown ();
		StartCoroutine(startCountingDownCoroutine);
		connectingScreen.enabled = false;
		waitingForPlayersScreen.enabled = false;
		winScreen.enabled = false;
		loseScreen.enabled = false;

		var hidingPlace = HidingPlace.GetRoot ();
		if (hidingPlace) {
			hidingPlace.SetHidingPlaceIndex (message.hidingPlace);
		}
	}

	private void ReadAndHandleReceivedMessages() {
		Message serverMessage;

		if (status == Status.titleScreen) {
			// don't process websockets
			return;
		}

		if (serverCommunication.TryGetReceivedMessage (out serverMessage)) {
			var serverToClientHelloMessage = serverMessage as ServerToClientHelloMessage;
			if (serverToClientHelloMessage != null) {
				HandleServerToClientHelloMessage (serverToClientHelloMessage);
			}

			var serverToClientStartMessage = serverMessage as ServerToClientStartMessage;
			if (serverToClientStartMessage != null) {
				HandleServerToClientStartMessage (serverToClientStartMessage);
			}

			var serverGameStateMessage = serverMessage as ServerGameStateMessage;
			if (serverGameStateMessage != null) {
				HandleServerGameStateMessage (serverGameStateMessage);
			}
		}
	}

	private void SendGameStateMessage() 
	{
		if (id == -1 || gameId == -1) {
			Debug.Log ("ID not set yet!!");
			return;
		}

		var message = new ClientGameStateMessage(
			id,
			transform.position, 
			transform.forward, 
			GetComponent<Rigidbody>().velocity, 
			frozen,
			gameId
		);
		serverCommunication.SendClientMessage (message);
	}

	private void HandleServerGameStateMessage(ServerGameStateMessage message) {
		if (message.gameId != gameId) {
			return;
		}

//		Debug.Log ("Processing other players, message: " + message);
		foreach (ClientGameStateMessage client in message.clients)
		{
			Vector3 position = client.playerPosition.ToVector3 ();
			Vector3 velocity = client.playerVelocity.ToVector3 ();
			Vector3 direction = client.playerDirection.ToVector3 ();

			if (finalCountingDownCoroutine == null && client.frozen) {
				finalCountingDownCoroutine = FinalCountdown ();
				StartCoroutine(finalCountingDownCoroutine);
			}
			int otherId = client.id;

			if (otherId == this.id) {
				// go to next iteration of loop
				continue;
			} 

			GameObject otherPlayer;
			if (otherPlayers.ContainsKey (otherId)) {
//				Debug.Log ("Found player with id " + id.ToString());
				otherPlayer = otherPlayers [otherId];
			} else {
//				Debug.Log ("Creating new player with id: " + id.ToString());
				otherPlayer = Instantiate(prefab, position, Quaternion.identity);
				otherPlayers.Add (otherId, otherPlayer);
			}
			var otherPlayerScript = otherPlayer.GetComponent<OtherPlayer> ();
			otherPlayerScript.desiredPosition = position;
			otherPlayerScript.desiredRotation = Quaternion.LookRotation(direction, Vector3.up);
			otherPlayerScript.desiredVelocity = velocity;
		}
	}

	IEnumerator FinalCountdown()
	{
		Debug.Log ("hit final countdown");
		countdown.enabled = true;
		int progress = this.finishSeconds;
		Debug.Log ("progress = " + progress.ToString());

		while (progress >= 0)
		{
			Debug.Log ("progress = " + progress.ToString());
			countdown.text = progress.ToString();
			progress -= 1;
			yield return new WaitForSeconds(1);
		}
		yield return true;
		finalCountingDownCoroutine = null;

		countdown.enabled = false;
		FinishGame ();
	}

	IEnumerator StartCountdown()
	{
		countdown.enabled = true;
		int progress = this.startSeconds;
		Debug.Log (progress);
		while (progress > 0)
		{
			countdown.text = progress.ToString();
			progress -= 1;
			yield return new WaitForSeconds(1);

		}
		yield return true;
		countdown.enabled = false;

		startCountingDownCoroutine = null;
		StartGame();
	}


	public void FinishGame() {
		playerMovement.enabled = false;

		if (frozen) {
			status = Status.win;
			winScreen.enabled = true;
		} else {
			status = Status.lose;
			loseScreen.enabled = true;
		}
	}

	public void StartGame() {
		playerMovement.enabled = true;
	}
}

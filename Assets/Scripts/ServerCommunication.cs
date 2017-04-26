using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using AssemblyCSharp;
using WebSocketSharp;

public class ServerCommunication : MonoBehaviour {

	Thread receiveThread;

	IPEndPoint udpEndpoint;
	UdpClient udpClient;
	WebSocket webSocket;

	Queue<string> receivedUdpMessageQueue = new Queue<string>();
	Queue<string> receivedWebSocketMessageQueue = new Queue<string>();

	object udpLock = new object();
	object wsLock = new object();

	static IPAddress IpV4ForHostname(string hostname) {
		IPHostEntry hostEntry;
		hostEntry = Dns.GetHostEntry(Constants.kServerHostname);

		foreach (var ip in hostEntry.AddressList) {
			if (ip.AddressFamily == AddressFamily.InterNetwork) {
				return ip;
			}
		}

		return null;
	}

	IEnumerator Start () {
		udpClient = new UdpClient(Constants.kClientPort);

		var serverIpAddr = IpV4ForHostname (Constants.kServerHostname);
		udpEndpoint = new IPEndPoint (serverIpAddr, Constants.kServerPort);

		receiveThread = new Thread(new ThreadStart(BackgroundReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();

		var builder = new UriBuilder("ws", Constants.kServerHostname, Constants.kServerPort);
		var uri = builder.Uri;
		Debug.Log (uri);
		webSocket = new WebSocket(uri);
		yield return StartCoroutine(webSocket.Connect());

		while (true)
		{
			string reply = webSocket.RecvString();
			if (reply != null)
			{
				lock (wsLock) {
					receivedWebSocketMessageQueue.Enqueue (reply);
				}
			}
			if (webSocket.error != null)
			{
				Debug.LogError ("Error: " + webSocket.error);
				break;
			}
			yield return 0;
		}
		webSocket.Close();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void BackgroundReceiveData()
	{
		while (true)
		{
			try
			{
				IPEndPoint senderEndpoint = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = udpClient.Receive(ref senderEndpoint);
				string text = Encoding.UTF8.GetString(data);

				var serverMessage = text;

				lock (udpLock) {
					receivedUdpMessageQueue.Enqueue(serverMessage);
				}
			}
			catch (Exception err)
			{
				print(err.ToString());
			}
		}
	}

	public bool TryGetServerUdpMessage(out string serverMessage) {
		lock (udpLock) {
			if (receivedUdpMessageQueue.Count > 0) {
				serverMessage = receivedUdpMessageQueue.Dequeue ();
				return true;
			} else {
				serverMessage = null;
				return false;
			}
		}
	}
		
	public void SendClientUdpMessage(string clientMessage) {
		byte[] data = Encoding.UTF8.GetBytes(clientMessage);
		udpClient.Send(data, data.Length, udpEndpoint);
	}

	public bool TryGetServerWebSocketMessage(out string serverMessage) {
		lock (wsLock) {
			if (receivedWebSocketMessageQueue.Count > 0) {
				serverMessage = receivedWebSocketMessageQueue.Dequeue ();
				return true;
			} else {
				serverMessage = null;
				return false;
			}
		}
	}

	public void SendClientWebSocketMessage(string clientMessage) {
		byte[] data = Encoding.UTF8.GetBytes(clientMessage);
		webSocket.Send (data);
	}
		
	public static ServerCommunication GetRoot() {
		var serverCommunicationObj = GameObject.Find("/ServerCommunication");
		var serverCommunication = serverCommunicationObj.GetComponent<ServerCommunication> ();
		Debug.Log (serverCommunication);

		return serverCommunication;
	}

	public ServerGameStateMessage CheckForOtherClientGameStates() 
	{
		ServerGameStateMessage jsonObj = null;
		string serverMessage;
		if (TryGetServerUdpMessage (out serverMessage)) {
//			Debug.Log ("Server sent UDP GameState: " + serverMessage);
			jsonObj = JsonUtility.FromJson<ServerGameStateMessage> (serverMessage);
		}
		return jsonObj;
	}
}

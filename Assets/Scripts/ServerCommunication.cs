using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
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

	ConcurrentQueue<string> receivedUdpMessageQueue = new ConcurrentQueue<string>();
	ConcurrentQueue<string> receivedWebSocketMessageQueue = new ConcurrentQueue<string>();

	IEnumerator Start () {
		udpClient = new UdpClient(AssemblyCSharp.Constants.kClientPort);
		var address = IPAddress.Parse(Constants.kServerAddr);
		udpEndpoint = new IPEndPoint(address, Constants.kServerPort);

		receiveThread = new Thread(new ThreadStart(BackgroundReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();

		var builder = new UriBuilder("ws", Constants.kServerAddr, Constants.kServerPort);
		var uri = builder.Uri;
		Debug.Log (uri);
		webSocket = new WebSocket(uri);
		yield return StartCoroutine(webSocket.Connect());

		while (true)
		{
			string reply = webSocket.RecvString();
			if (reply != null)
			{
				receivedWebSocketMessageQueue.Enqueue (reply);
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
				byte[] data = udpClient.Receive(ref udpEndpoint);
				string text = Encoding.UTF8.GetString(data);

				var serverMessage = text;

				receivedUdpMessageQueue.Enqueue(serverMessage);
			}
			catch (Exception err)
			{
				print(err.ToString());
			}
		}
	}

	public bool TryGetServerUdpMessage(out string serverMessage) {
		return receivedUdpMessageQueue.TryDequeue (out serverMessage);
	}
		
	public void SendClientUdpMessage(string clientMessage) {
		byte[] data = Encoding.UTF8.GetBytes(clientMessage);
		udpClient.Send(data, data.Length, udpEndpoint);
	}

	public bool TryGetServerWebSocketMessage(out string serverMessage) {
		return receivedWebSocketMessageQueue.TryDequeue (out serverMessage);
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
		if (TryGetServerUdpMessage(out serverMessage)) 
		{
			jsonObj = JsonUtility.FromJson<ServerGameStateMessage> (serverMessage);
		}
		return jsonObj;
	}
}

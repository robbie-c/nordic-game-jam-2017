using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
#if !(UNITY_WEBGL && !UNITY_EDITOR)
using System.Net;
using System.Net.Sockets;
#endif
using System.Threading;

using AssemblyCSharp;

public class ServerCommunication : MonoBehaviour {


	WebSocket webSocket;

	Queue<Message> receivedMessageQueue = new Queue<Message>();
	object receivedMessageQueueLock = new object();

	MessageParser parser = new MessageParser ();

	#if UNITY_WEBGL && !UNITY_EDITOR
	void StartNet() {
	}
	#else
	Thread receiveThread;

	IPEndPoint udpEndpoint;
	UdpClient udpClient;

	static IPAddress IpV4ForHostname(string hostname) {

		return IPAddress.Parse(Constants.kServerHostname);

		IPHostEntry hostEntry;
		hostEntry = Dns.GetHostEntry(Constants.kServerHostname);

		foreach (var ip in hostEntry.AddressList) {
			if (ip.AddressFamily == AddressFamily.InterNetwork) {
				return ip;
			}
		}

		return null;
	}

	private void BackgroundReceiveUdp()
	{
		while (true)
		{
			try
			{
				IPEndPoint senderEndpoint = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = udpClient.Receive(ref senderEndpoint);
				string text = Encoding.UTF8.GetString(data);

				var serverMessage = parser.Parse(text);
				HandleReceivedMessage(serverMessage);
			}
				catch (Exception err)
			{
				print(err.ToString());
			}
		}
	}

	private void SendClientUdpMessage(Message clientMessage) {
		string str = clientMessage.Serialize ();
		byte[] data = Encoding.UTF8.GetBytes(str);
		udpClient.Send(data, data.Length, udpEndpoint);
	}

	void StartNet () {
		udpClient = new UdpClient(Constants.kClientPort);

		var serverIpAddr = IpV4ForHostname (Constants.kServerHostname);
		udpEndpoint = new IPEndPoint (serverIpAddr, Constants.kServerPort);

		receiveThread = new Thread(new ThreadStart(BackgroundReceiveUdp));
		receiveThread.IsBackground = true;
		receiveThread.Start();
	}
	#endif

	IEnumerator Start () {
		StartNet ();

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
				var message = parser.Parse (reply);
				HandleReceivedMessage (message);
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

	private void HandleReceivedMessage(Message message) {
		lock (receivedMessageQueueLock) {
			receivedMessageQueue.Enqueue (message);
		}
	}

	public bool TryGetReceivedMessage(out Message serverMessage) {
		lock (receivedMessageQueueLock) {
			if (receivedMessageQueue.Count > 0) {
				serverMessage = receivedMessageQueue.Dequeue ();
				return true;
			} else {
				serverMessage = null;
				return false;
			}
		}
	}
		
	private void SendClientWebSocketMessage(Message clientMessage) {
		string str = clientMessage.Serialize ();
		byte[] data = Encoding.UTF8.GetBytes(str);
		webSocket.Send (data);
	}

	public void SendClientMessage(Message clientMessage) {
		#if UNITY_WEBGL && !UNITY_EDITOR
		SendClientWebSocketMessage (clientMessage);
		#else
		if (clientMessage is ClientGameStateMessage) {
			SendClientUdpMessage (clientMessage);
		} else {
			SendClientWebSocketMessage (clientMessage);
		}
		#endif
	}
		
	public static ServerCommunication GetRoot() {
		var serverCommunicationObj = GameObject.Find("/ServerCommunication");
		var serverCommunication = serverCommunicationObj.GetComponent<ServerCommunication> ();
		Debug.Log (serverCommunication);

		return serverCommunication;
	}
}

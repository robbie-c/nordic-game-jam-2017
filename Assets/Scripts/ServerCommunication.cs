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

public class ServerCommunication : MonoBehaviour {

	Thread receiveThread;

	// udpclient object
	UdpClient udpClient;
	IPEndPoint endpoint;

	ConcurrentQueue<ServerGameStateMessage> receivedMessageQueue = new ConcurrentQueue<ServerGameStateMessage>();

	// Use this for initialization
	void Start () {
		udpClient = new UdpClient(AssemblyCSharp.Constants.kClientPort);
		var address = IPAddress.Parse(Constants.kServerAddr);
		endpoint = new IPEndPoint(address, Constants.kServerPort);

		receiveThread = new Thread(new ThreadStart(BackgroundReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
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
				byte[] data = udpClient.Receive(ref endpoint);
				string text = Encoding.UTF8.GetString(data);

				var serverMessage = new ServerGameStateMessage(text);

				receivedMessageQueue.Enqueue(serverMessage);
			}
			catch (Exception err)
			{
				print(err.ToString());
			}
		}
	}

	public bool TryGetServerGameStateMessage(out ServerGameStateMessage serverMessage) {
		return receivedMessageQueue.TryDequeue (out serverMessage);
	}

//	public void SendClientGameStateMessage(ClientGameStateMessage clientMessage) {
//		byte[] data = Encoding.UTF8.GetBytes(clientMessage.text);
//		udpClient.Send(data, data.Length, endpoint);
//	}

	public void SendClientGameStateMessage(string text) {
		byte[] data = Encoding.UTF8.GetBytes(text);
		udpClient.Send(data, data.Length, endpoint);
	}

	public static ServerCommunication GetRoot() {
		var serverCommunicationObj = GameObject.Find("/ServerCommunication");
		var serverCommunication = serverCommunicationObj.GetComponent<ServerCommunication> ();
		Debug.Log (serverCommunication);

		return serverCommunication;
	}
}

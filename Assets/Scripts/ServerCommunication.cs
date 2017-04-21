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
	UdpClient client;
	IPEndPoint endpoint;

	ConcurrentQueue<ServerMessage> receivedMessageQueue = new ConcurrentQueue<ServerMessage>();

	// Use this for initialization
	void Start () {
		client = new UdpClient(AssemblyCSharp.Constants.kClientPort);
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
				byte[] data = client.Receive(ref endpoint);
				string text = Encoding.UTF8.GetString(data);

				var serverMessage = new ServerMessage(text);

				receivedMessageQueue.Enqueue(serverMessage);
			}
			catch (Exception err)
			{
				print(err.ToString());
			}
		}
	}

	public bool TryGetMessage(out ServerMessage serverMessage) {
		return receivedMessageQueue.TryDequeue (out serverMessage);
	}

	public void SendMessage(ClientMessage clientMessage) {
		byte[] data = Encoding.UTF8.GetBytes(clientMessage.text);
		client.Send(data, data.Length, endpoint);
	}
}

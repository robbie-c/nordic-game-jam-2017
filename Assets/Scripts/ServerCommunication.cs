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

public class NewBehaviourScript : MonoBehaviour {

	Thread receiveThread;

	// udpclient object
	UdpClient client; 

	ConcurrentQueue<ServerMessage> receivedMessageQueue = new ConcurrentQueue<ServerMessage>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private  void BackgroundReceiveData()
	{
		client = new UdpClient(AssemblyCSharp.Constants.kClientPort);
		while (true)
		{
			try
			{
				var address = IPAddress.Parse(Constants.kServerAddr);
				var endpoint = new IPEndPoint(address, Constants.kServerPort);
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

}

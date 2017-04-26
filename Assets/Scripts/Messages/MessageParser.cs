using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using AssemblyCSharp;

namespace AssemblyCSharp
{
	public class MessageParser
	{
		private class TypeMessage : Message {

		}

		public Message Parse(string str) {
			var typeMessage = JsonUtility.FromJson<TypeMessage> (str);
			switch (typeMessage.type) {
			case "ClientGameStateMessage":
				return JsonUtility.FromJson<ClientGameStateMessage> (str);
			case "ClientHelloMessage":
				return JsonUtility.FromJson<ClientHelloMessage> (str);
			case "ServerGameStateMessage":
				return JsonUtility.FromJson<ServerGameStateMessage> (str);
			case "ServerToClientHelloMessage":
				return JsonUtility.FromJson<ServerToClientHelloMessage> (str);
			case "ServerToClientStartMessage":
				return JsonUtility.FromJson<ServerToClientStartMessage> (str);
			default:
				Debug.LogError ("Unknown message type!");
				return null;
			}
		}
	}
}


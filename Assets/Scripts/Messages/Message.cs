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
	[Serializable]
	public abstract class Message
	{
		public string type;

		public Message() {
			type = this.GetType().Name;
		}

		public string Serialize() {
			return JsonUtility.ToJson (this);
		}

		public override string ToString() {
			return Serialize ();
		}
	}
}


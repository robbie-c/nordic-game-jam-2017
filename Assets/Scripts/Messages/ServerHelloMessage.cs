using System;

namespace AssemblyCSharp
{
	[Serializable]
	public class ServerHelloMessage
	{
		public string type;
		public int id;
		public Vector3Serialisable initialPosition;
	}
}


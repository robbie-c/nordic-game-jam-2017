using System;

namespace AssemblyCSharp
{
	[Serializable]
	public class ServerToClientHelloMessage
	{
		public string type;
		public int id;
		public Vector3Serialisable initialPosition;
		public Vector3Serialisable initialVelocity;
		public Vector3Serialisable initialDirection;
	}
}


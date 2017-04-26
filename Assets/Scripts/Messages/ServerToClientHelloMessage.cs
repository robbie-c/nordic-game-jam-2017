using System;

namespace AssemblyCSharp
{
	[Serializable]
	public class ServerToClientHelloMessage : Message
	{
		public int id;
		public int gameId;
		public Vector3Serialisable playerPosition;
		public Vector3Serialisable playerVelocity;
		public Vector3Serialisable playerDirection;
		public int hidingPlace;
	}
}


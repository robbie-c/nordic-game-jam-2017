using System;

namespace AssemblyCSharp
{
	[Serializable]
	public class ServerToClientStartMessage : Message
	{
		public int gameId;
		public int hidingPlace;
		public Vector3Serialisable playerPosition;

		public ServerToClientStartMessage (int gameId, int hidingPlace, Vector3Serialisable playerPosition)
		{
			this.gameId = gameId;
			this.hidingPlace = hidingPlace;
			this.playerPosition = playerPosition;
		}
	}
}


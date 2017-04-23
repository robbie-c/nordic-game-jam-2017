using System;

namespace AssemblyCSharp
{
	[Serializable]
	public class ServerToClientStartMessage
	{
		public int gameId;
		public string type;
		public int hidingPlace;
		public Vector3Serialisable playerPosition;

		public ServerToClientStartMessage (int gameId, int hidingPlace, Vector3Serialisable playerPosition)
		{
			this.gameId = gameId;
			type = this.GetType().Name;
			this.hidingPlace = hidingPlace;
			this.playerPosition = playerPosition;
		}
	}
}


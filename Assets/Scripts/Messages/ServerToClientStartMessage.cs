using System;

namespace AssemblyCSharp
{
	[Serializable]
	public class ServerToClientStartMessage
	{
		public int gameId;
		public string type;
		public int hidingPlace;

		public ServerToClientStartMessage (int gameId, int hidingPlace)
		{
			this.gameId = gameId;
			type = this.GetType().Name;
			this.hidingPlace = hidingPlace;
		}
	}
}


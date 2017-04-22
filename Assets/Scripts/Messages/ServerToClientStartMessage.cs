using System;

namespace AssemblyCSharp
{
	[Serializable]
	public class ServerToClientStartMessage
	{
		public int gameId;
		public string type;

		public ServerToClientStartMessage (int gameId)
		{
			this.gameId = gameId;
			type = this.GetType().Name;
		}
	}
}


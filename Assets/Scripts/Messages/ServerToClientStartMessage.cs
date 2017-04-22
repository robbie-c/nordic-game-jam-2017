using System;

namespace AssemblyCSharp
{
	[Serializable]
	public class ServerToClientStartMessage
	{
		public readonly int gameId;
		public string type;

		public ServerToClientStartMessage (int gameId)
		{
			this.gameId = gameId;
			type = this.GetType().Name;
		}
	}
}


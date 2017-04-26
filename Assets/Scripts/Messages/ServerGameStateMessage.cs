using System;
using System.Collections.Generic;


namespace AssemblyCSharp
{
	[Serializable]
	public class ServerGameStateMessage: Message
	{
		public List<ClientGameStateMessage> clients;
		public int gameId;
	}
}


using System;
using System.Collections.Generic;


namespace AssemblyCSharp
{
	[Serializable]
	public class ServerGameStateMessage
	{
		public List<ClientGameStateMessage> clients;
		public string type;
	}
}


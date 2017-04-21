using System;

namespace AssemblyCSharp
{
	public class ServerMessage : IGameMessage
	{
		public readonly string text;

		public ServerMessage (string text)
		{
			this.text = text;
		}

		public string SerializeToJson() {
			return "";
		}
	}
}


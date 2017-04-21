using System;

namespace AssemblyCSharp
{
	public class ClientMessage : IGameMessage
	{
		public readonly string text;

		public ClientMessage (string text)
		{
			this.text = text;
		}

		public string SerializeToJson() {
			return "";
		}
	}
}


using System;

namespace AssemblyCSharp
{
	public class ServerGameStateMessage
	{
		public readonly string text;

		public ServerGameStateMessage (string text)
		{
			this.text = text;
		}
	}
}


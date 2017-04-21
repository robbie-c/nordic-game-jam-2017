using System;

namespace AssemblyCSharp
{
	[Serializable]
	public class ClientHelloMessage
	{
		public string type;

		public ClientHelloMessage ()
		{
			type = this.GetType().Name;
		}
	}
}


using System;

namespace AssemblyCSharp
{
	[Serializable]
	public class HelloMessage
	{
		public string type;

		public HelloMessage ()
		{
			type = this.GetType().Name;
		}
	}
}


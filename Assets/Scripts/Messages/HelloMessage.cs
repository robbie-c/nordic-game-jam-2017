using System;

namespace AssemblyCSharp
{
	[Serializable]
	public class HelloMessage
	{
		public string type;
		public bool hello;
		public HelloMessage ()
		{
			type = this.GetType().Name;
			hello = true;
		}
	}
}


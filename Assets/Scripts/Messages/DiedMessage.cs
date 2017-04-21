using System;

namespace AssemblyCSharp
{
	[Serializable]
	public class DiedMessage
	{
		public string type;

		public DiedMessage ()
		{
			type = this.GetType ().Name;
		}
	}
}


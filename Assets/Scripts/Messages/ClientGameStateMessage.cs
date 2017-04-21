using System;
using UnityEngine;

namespace AssemblyCSharp
{
	[Serializable]
	public class ClientGameStateMessage
	{
		public float x;
		public float y;
		public float z;

		public ClientGameStateMessage (Vector3 position)
		{
			x = position.x;
			y = position.y;
			z = position.z;
		}
	}
}


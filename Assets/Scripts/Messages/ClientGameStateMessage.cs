using System;
using UnityEngine;

namespace AssemblyCSharp
{
	[Serializable]
	public class ClientGameStateMessage
	{
		public float posX;
		public float posY;
		public float posZ;
		public float forwardX;
		public float forwardY;
		public float forwardZ;

		public ClientGameStateMessage (Vector3 position, Vector3 forwardDirection)
		{
			posX = position.x;
			posY = position.y;
			posZ = position.z;
			forwardX = forwardDirection.x;
			forwardY = forwardDirection.y;
			forwardZ = forwardDirection.z;
		}
	}
}


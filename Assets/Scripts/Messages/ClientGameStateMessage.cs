using System;
using UnityEngine;


// udp client: client state
// Position
// Forward direction
// Show exploding animation
// TODO Bullet position 
// TODO Dummy position


namespace AssemblyCSharp
{
	[Serializable]
	public class ClientGameStateMessage
	{
		public string type;
		public float posX;
		public float posY;
		public float posZ;
		public float forwardX;
		public float forwardY;
		public float forwardZ;
		public Boolean exploded;

		public ClientGameStateMessage (Vector3 position, Vector3 forwardDirection, Boolean exploded)
		{
			type = this.GetType().Name;
			posX = position.x;
			posY = position.y;
			posZ = position.z;
			forwardX = forwardDirection.x;
			forwardY = forwardDirection.y;
			forwardZ = forwardDirection.z;
			this.exploded = exploded;
		}
	}
}


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
		public PlayerPosition playerPosition;
		public PlayerDirection playerDirection;
		public Boolean exploded;

		public ClientGameStateMessage (Vector3 position, Vector3 forwardDirection, Boolean exploded)
		{
			type = this.GetType().Name;
			playerPosition = new PlayerPosition (position);
			playerDirection = new PlayerDirection (forwardDirection);
			this.exploded = exploded;
		}
	}
}


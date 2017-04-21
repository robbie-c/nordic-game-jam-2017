using System;
using UnityEngine;


// udp client: client state
// Position
// Forward direction
// Show frozen animation
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
		public Boolean frozen;

		public ClientGameStateMessage (Vector3 position, Vector3 forwardDirection, Boolean frozen)
		{
			type = this.GetType().Name;
			playerPosition = new PlayerPosition (position);
			playerDirection = new PlayerDirection (forwardDirection);
			this.frozen = frozen;
		}
	}
}


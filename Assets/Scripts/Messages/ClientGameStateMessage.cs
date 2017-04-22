using System;
using UnityEngine;


// udp client: client state
// Position
// Forward direction
// Show frozen animation


namespace AssemblyCSharp
{
	[Serializable]
	public class ClientGameStateMessage
	{
		public string type;
		public int id;
		public Vector3Serialisable playerPosition;
		public Vector3Serialisable playerDirection;
		public Vector3Serialisable playerVelocity;
		public Boolean frozen;
		public int gameId;

		public ClientGameStateMessage (int id, Vector3 position, Vector3 forwardDirection, Vector3 velocity, Boolean frozen, int gameId)
		{
			this.id = id;
			this.gameId = gameId;
			type = this.GetType().Name;
			playerPosition = new Vector3Serialisable (position);
			playerDirection = new Vector3Serialisable (forwardDirection);
			playerVelocity = new Vector3Serialisable (velocity);
			this.frozen = frozen;
		}
	}
}


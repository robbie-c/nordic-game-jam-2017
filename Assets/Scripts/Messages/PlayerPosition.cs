using System;
using UnityEngine;

namespace AssemblyCSharp
{
	[Serializable]
	public class PlayerPosition
	{
		public float x;
		public float y;
		public float z;

		public PlayerPosition (Vector3 position)
		{
			x = position.x;
			y = position.y;
			z = position.z;
		}
	}
}


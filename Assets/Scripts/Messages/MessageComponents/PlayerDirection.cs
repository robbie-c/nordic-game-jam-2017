using System;
using UnityEngine;

namespace AssemblyCSharp
{
	[Serializable]
	public class PlayerDirection
	{
		public float x;
		public float y;
		public float z;

		public PlayerDirection (Vector3 position)
		{
			x = position.x;
			y = position.y;
			z = position.z;
		}
	}
}


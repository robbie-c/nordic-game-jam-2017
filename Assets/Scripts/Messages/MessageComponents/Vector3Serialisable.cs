using System;
using UnityEngine;

namespace AssemblyCSharp
{
	[Serializable]
	public class Vector3Serialisable
	{
		public float x;
		public float y;
		public float z;

		public Vector3Serialisable (Vector3 vec)
		{
			x = vec.x;
			y = vec.y;
			z = vec.z;
		}

		public Vector3 ToVector3()
		{
			return new Vector3 (x, y, z);
		}
	}
}


using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class ClientMessageCoordinator
	{
		private ClientMessageCoordinator ()
		{
		}

		public static string createPlayerPosition(Vector3 position) 
		{
			PlayerPosition playerPosObj = new PlayerPosition(position);
			return JsonUtility.ToJson(playerPosObj);
		}
	}
}


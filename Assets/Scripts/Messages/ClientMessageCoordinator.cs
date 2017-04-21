using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class ClientMessageCoordinator
	{
		private ClientMessageCoordinator ()
		{
		}

		public static string createGameStateMessage(Vector3 position, Vector3 forward) 
		{
			ClientGameStateMessage playerPosObj = new ClientGameStateMessage(position, forward);
			return JsonUtility.ToJson(playerPosObj);
		}
	}
}


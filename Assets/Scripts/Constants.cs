using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class Constants
	{
		public const string kServerHostname = "saffron.local";
		public const int kServerPort = 41234;
		public const int kClientPort = 41235;
		public const int kGameStateUpdateTickMs = 500;
		public const int kSecondsBetweenFirstHideAndRoundEnd = 10;
		public const int kSecondsBetweenRoundEndAndNextRoundStart = 5;
		public const int kNumHidingPlaces = 10;
		public static readonly Vector3[] kHidingPlaces = new [] {
			new Vector3(35, 0, 47),
			new Vector3(1, 0 -40),
			new Vector3(-16, 0, 49),
			new Vector3(35, 0, -20),
			new Vector3(-18, 0, -25),
			new Vector3(-18, 0, -11),
			new Vector3(17, 0, 6),
			new Vector3(33, 0, 9),
			new Vector3(-17, 0, 14),
			new Vector3(-26, 0, 14)
		};

		static Constants()
		{
			Debug.Assert (kHidingPlaces.Length == kNumHidingPlaces);
		}
	}
}


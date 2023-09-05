#nullable enable

using System;
using UnityEngine;
using UnityExtensions;
using Utility;

namespace GameplaySystems.Networld
{
	public class NetworldBootstrap : MonoBehaviour
	{
		private CoreRouter? coreRouter;

		[SerializeField]
		private NetworkSimulationHolder holder = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(NetworldBootstrap));
			this.coreRouter = new CoreRouter();
			this.holder.Value = new NetworkSimulationController(coreRouter);
		}

		private void Update()
		{
			coreRouter?.NetworkUpdate();
		}
	}
}
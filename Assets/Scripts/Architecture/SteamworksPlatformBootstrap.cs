#nullable enable
using UnityEngine;
using Utility;

namespace Architecture
{
	public class SteamworksPlatformBootstrap : MonoBehaviour
	{
		[SerializeField]
		private PlatformLayerHolder platformHolder = null!;

		[SerializeField]
		private int steamAppId;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(SteamworksPlatformBootstrap));
			platformHolder.Value = new SteamworksPlatformLayer();
		}
	}
}
using System;
using UnityEngine;
using Utility;

namespace GameplaySystems.Hacking
{
	public class HackingSystem : MonoBehaviour
	{
		private ExploitAsset[] exploits;
		private PayloadAsset[] payloads;
		
		[Header("Settings")]
		[SerializeField]
		private string exploitsResourcePath = "Exploits";

		[SerializeField]
		private string payloadsResourcePath = "Payloads";


		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(HackingSystem));
		}

		private void Start()
		{
			exploits = Resources.LoadAll<ExploitAsset>(exploitsResourcePath);
			payloads = Resources.LoadAll<PayloadAsset>(payloadsResourcePath);
		}
	}
}
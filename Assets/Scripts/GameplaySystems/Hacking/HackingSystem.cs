using System;
using System.Collections.Generic;
using Core;
using Core.DataManagement;
using Core.WorldData.Data;
using GameplaySystems.NonPlayerComputers;
using OS.FileSystems;
using Player;
using UnityEngine;
using Utility;

namespace GameplaySystems.Hacking
{
	public class HackingSystem : MonoBehaviour
	{
		private ExploitAsset[] exploits;
		private PayloadAsset[] payloads;
		private NonPlayerComputerEventListener npcComputers;

		[Header("Dependencies")]
		[SerializeField]
		private PlayerInstanceHolder playerInstance = null!;

		[SerializeField]
		private WorldManagerHolder world = null!;
		
		[Header("Settings")]
		[SerializeField]
		private string exploitsResourcePath = "Exploits";

		[SerializeField]
		private string payloadsResourcePath = "Payloads";

		public IEnumerable<ExploitAsset> Exploits => exploits;
		public IEnumerable<PayloadAsset> Payloads => payloads;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(HackingSystem));
			
			UnityHelpers.MustFindObjectOfType(out npcComputers);
		}

		private void Start()
		{
			exploits = Resources.LoadAll<ExploitAsset>(exploitsResourcePath);
			payloads = Resources.LoadAll<PayloadAsset>(payloadsResourcePath);

			world.Value.Callbacks.AddCreateCallback<WorldCraftedExploitData>(OnCraftedExploitCreated);
			world.Value.Callbacks.AddModifyCallback<WorldCraftedExploitData>(OnCraftedExploitModified);
			world.Value.Callbacks.AddDeleteCallback<WorldCraftedExploitData>(OnCraftedExploitDeleted);
		}

		private void OnCraftedExploitDeleted(WorldCraftedExploitData subject)
		{
		}

		private void OnCraftedExploitModified(WorldCraftedExploitData subjectprevious, WorldCraftedExploitData subjectnew)
		{
		}

		private void OnCraftedExploitCreated(WorldCraftedExploitData subject)
		{
		}

		private IFileOverrider GetFileOverrider(ObjectId computer)
		{
			if (computer == ObjectId.Invalid)
				return playerInstance.Value.FileOverrider;
			else
				return npcComputers.GetNpcFileOverrider(computer);
		}
	}
}
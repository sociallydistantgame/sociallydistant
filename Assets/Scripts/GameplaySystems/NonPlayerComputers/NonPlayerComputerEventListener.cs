#nullable enable

using System.Collections.Generic;
using Architecture;
using Core;
using Core.DataManagement;
using Core.WorldData.Data;
using OS.FileSystems;
using UnityEngine;
using Utility;

namespace GameplaySystems.NonPlayerComputers
{
	public class NonPlayerComputerEventListener : MonoBehaviour
	{
		private readonly Dictionary<ObjectId, NonPlayerComputer> instances = new Dictionary<ObjectId, NonPlayerComputer>();
		private readonly Dictionary<ObjectId, NpcFileOverrider> overriders = new Dictionary<ObjectId, NpcFileOverrider>();

		[Header("Dependencies")]
		[SerializeField]
		private WorldManagerHolder world = null!;

		[Header("Prefabs")]
		[SerializeField]
		private NonPlayerComputer computerPrefab = null!;
		
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(NonPlayerComputerEventListener));
		}

		private void Start()
		{
			InstallEvents();
		}

		private void OnDestroy()
		{
			UninstallEvents();
		}

		public bool TryGetComputer(ObjectId id, out NonPlayerComputer computer)
		{
			return this.instances.TryGetValue(id, out computer);
		}
		
		private void InstallEvents()
		{
			this.world.Value.Callbacks.AddCreateCallback<WorldComputerData>(OnCreateComputer);
			this.world.Value.Callbacks.AddModifyCallback<WorldComputerData>(OnModifyComputer);
			this.world.Value.Callbacks.AddDeleteCallback<WorldComputerData>(OnDeleteComputer);
		}

		private void UninstallEvents()
		{
			this.world.Value.Callbacks.RemoveCreateCallback<WorldComputerData>(OnCreateComputer);
			this.world.Value.Callbacks.RemoveModifyCallback<WorldComputerData>(OnModifyComputer);
			this.world.Value.Callbacks.RemoveDeleteCallback<WorldComputerData>(OnDeleteComputer);
		}
		
		private void OnDeleteComputer(WorldComputerData subject)
		{
			if (!instances.TryGetValue(subject.InstanceId, out NonPlayerComputer computer))
				return;

			instances.Remove(subject.InstanceId);
			Destroy(computer.gameObject);
			overriders.Remove(subject.InstanceId);
		}

		private void OnModifyComputer(WorldComputerData subjectprevious, WorldComputerData subjectnew)
		{
			NonPlayerComputer computer = instances[subjectnew.InstanceId];
			computer.UpdateWorldData(subjectnew);
		}

		private void OnCreateComputer(WorldComputerData subject)
		{
			var setActive = false;
			if (!overriders.TryGetValue(subject.InstanceId, out NpcFileOverrider overrider))
			{
				overrider = new NpcFileOverrider();
				overriders.Add(subject.InstanceId, overrider);
			}
			
			if (!instances.TryGetValue(subject.InstanceId, out NonPlayerComputer computer))
			{
				this.computerPrefab.gameObject.SetActive(false);
				computer = Instantiate(computerPrefab);
				computer.SetFileOverrider(overrider);
				instances.Add(subject.InstanceId, computer);
				setActive = true;
			}

			computer.UpdateWorldData(subject);
			
			if (setActive)
				computer.gameObject.SetActive(true);
		}

		public IFileOverrider GetNpcFileOverrider(ObjectId computer)
		{
			return overriders[computer];
		}
	}
}
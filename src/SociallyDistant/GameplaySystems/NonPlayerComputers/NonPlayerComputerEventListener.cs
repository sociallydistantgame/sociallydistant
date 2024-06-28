#nullable enable

using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.OS.FileSystems;

namespace SociallyDistant.GameplaySystems.NonPlayerComputers
{
	public class NonPlayerComputerEventListener
	{
		private NonPlayerComputer computerPrefab = null!;
		
		private readonly Dictionary<ObjectId, NonPlayerComputer> instances = new Dictionary<ObjectId, NonPlayerComputer>();
		private readonly Dictionary<ObjectId, NpcFileOverrider> overriders = new Dictionary<ObjectId, NpcFileOverrider>();
		
		private IWorldManager world = null!;
		
		private void Awake()
		{
			world = SociallyDistantGame.Instance.WorldManager;
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
			this.world.Callbacks.AddCreateCallback<WorldComputerData>(OnCreateComputer);
			this.world.Callbacks.AddModifyCallback<WorldComputerData>(OnModifyComputer);
			this.world.Callbacks.AddDeleteCallback<WorldComputerData>(OnDeleteComputer);
		}

		private void UninstallEvents()
		{
			this.world.Callbacks.RemoveCreateCallback<WorldComputerData>(OnCreateComputer);
			this.world.Callbacks.RemoveModifyCallback<WorldComputerData>(OnModifyComputer);
			this.world.Callbacks.RemoveDeleteCallback<WorldComputerData>(OnDeleteComputer);
		}
		
		private void OnDeleteComputer(WorldComputerData subject)
		{
			if (!instances.TryGetValue(subject.InstanceId, out NonPlayerComputer computer))
				return;

			instances.Remove(subject.InstanceId);
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
				throw new Exception("No. Not yet. I need more weed before I implement this.");
			}

			computer.UpdateWorldData(subject);
		}

		public IFileOverrider GetNpcFileOverrider(ObjectId computer)
		{
			return overriders[computer];
		}
	}
}
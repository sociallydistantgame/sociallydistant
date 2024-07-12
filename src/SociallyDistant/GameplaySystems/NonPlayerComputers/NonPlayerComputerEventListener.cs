#nullable enable

using Microsoft.Xna.Framework;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.OS.FileSystems;

namespace SociallyDistant.GameplaySystems.NonPlayerComputers
{
	public class NonPlayerComputerEventListener : GameComponent
	{
		private readonly SociallyDistantGame                     game;
		private readonly Dictionary<ObjectId, NonPlayerComputer> instances  = new Dictionary<ObjectId, NonPlayerComputer>();
		private readonly Dictionary<ObjectId, NpcFileOverrider>  overriders = new Dictionary<ObjectId, NpcFileOverrider>();

		private IWorldManager World => game.WorldManager;

		internal NonPlayerComputerEventListener(SociallyDistantGame game) : base(game)
		{
			this.game = game;
		}

		public override void Initialize()
		{
			InstallEvents();
		}

		public override void Update(GameTime gameTime)
		{
			foreach (var computer in instances.Values)
				computer.Update();
			
			base.Update(gameTime);
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposing)
				return;
			
			UninstallEvents();
		}

		public bool TryGetComputer(ObjectId id, out NonPlayerComputer computer)
		{
			return this.instances.TryGetValue(id, out computer);
		}
		
		private void InstallEvents()
		{
			this.World.Callbacks.AddCreateCallback<WorldComputerData>(OnCreateComputer);
			this.World.Callbacks.AddModifyCallback<WorldComputerData>(OnModifyComputer);
			this.World.Callbacks.AddDeleteCallback<WorldComputerData>(OnDeleteComputer);
		}

		private void UninstallEvents()
		{
			this.World.Callbacks.RemoveCreateCallback<WorldComputerData>(OnCreateComputer);
			this.World.Callbacks.RemoveModifyCallback<WorldComputerData>(OnModifyComputer);
			this.World.Callbacks.RemoveDeleteCallback<WorldComputerData>(OnDeleteComputer);
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
				computer = new NonPlayerComputer(game);
				instances.Add(subject.InstanceId, computer);
			}

			computer.UpdateWorldData(subject);
		}

		public IFileOverrider GetNpcFileOverrider(ObjectId computer)
		{
			return overriders[computer];
		}
	}
}
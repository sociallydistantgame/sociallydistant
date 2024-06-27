using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.DataManagement;
using Core.Scripting;
using Core.WorldData.Data;
using GamePlatform;
using GameplaySystems.Networld;
using GameplaySystems.NonPlayerComputers;
using Modules;
using OS.Devices;
using OS.FileSystems;
using OS.Network;
using Player;
using UI.Widgets;
using UnityEngine;
using UnityExtensions;
using Utility;

namespace GameplaySystems.Hacking
{
	public class HackingSystem : MonoBehaviour
	{
		[Header("Dependencies")]
		[SerializeField]
		private PlayerInstanceHolder playerInstance = null!;
		
		[Header("Settings")]
		[SerializeField]
		private string exploitsResourcePath = "Exploits";

		[SerializeField]
		private string payloadsResourcePath = "Payloads";
		
		private readonly Dictionary<ObjectId, IListener> hackables = new Dictionary<ObjectId, IListener>();
		private readonly Dictionary<ObjectId, CraftedExploitFile> craftedExploits = new Dictionary<ObjectId, CraftedExploitFile>();
		private GameManager gameManager;
		private ExploitAsset[] exploits;
		private PayloadAsset[] payloads;
		private NonPlayerComputerEventListener npcComputers;
		private IWorldManager world = null!;
		private ReloadHackingStuffHook reloadHackingStuffHook;

		public IEnumerable<ExploitAsset> Exploits => exploits;
		public IEnumerable<PayloadAsset> Payloads => payloads;
		
		private void Awake()
		{
			reloadHackingStuffHook = new ReloadHackingStuffHook(this);
			
			gameManager = GameManager.Instance;
			world = GameManager.Instance.WorldManager;
			
			this.AssertAllFieldsAreSerialized(typeof(HackingSystem));
			
			UnityHelpers.MustFindObjectOfType(out npcComputers);
		}

		private void OnEnable()
		{
			gameManager.ScriptSystem.RegisterHookListener(CommonScriptHooks.AfterContentReload, reloadHackingStuffHook);
			ReloadHackingStuff();
		}

		private void OnDisable()
		{
			gameManager.ScriptSystem.UnregisterHookListener(CommonScriptHooks.AfterContentReload, reloadHackingStuffHook);
		}

		private void Start()
		{
			world.Callbacks.AddCreateCallback<WorldCraftedExploitData>(OnCraftedExploitCreated);
			world.Callbacks.AddModifyCallback<WorldCraftedExploitData>(OnCraftedExploitModified);
			world.Callbacks.AddDeleteCallback<WorldCraftedExploitData>(OnCraftedExploitDeleted);
			
			world.Callbacks.AddCreateCallback<WorldHackableData>(OnHackableCreated);
		}

		private void OnHackableCreated(WorldHackableData subject)
		{
			IComputer? computer = GetComputer(subject.ComputerId);
			if (computer == null)
				return;

			if (computer.Network == null)
				return;

			if (!hackables.TryGetValue(subject.InstanceId, out IListener listener))
			{
				listener = computer.Network.Listen(subject.Port, subject.ServerType, subject.SecurityLevel);
				hackables.Add(subject.InstanceId, listener);
			}
		}

		private void OnCraftedExploitDeleted(WorldCraftedExploitData subject)
		{
			if (!craftedExploits.TryGetValue(subject.InstanceId, out CraftedExploitFile file))
				return;
			
			IFileOverrider overrider = GetFileOverrider(subject.Computer);
			string[] path = PathUtility.Split(subject.FilePath);

			overrider.RemoveFile(path, file);
			craftedExploits.Remove(subject.InstanceId);
		}

		private void OnCraftedExploitModified(WorldCraftedExploitData subjectprevious, WorldCraftedExploitData subjectnew)
		{
		}

		private void OnCraftedExploitCreated(WorldCraftedExploitData subject)
		{
			IFileOverrider overrider = GetFileOverrider(subject.Computer);
			string directoryName = PathUtility.GetDirectoryName(subject.FilePath);
			string[] directory = PathUtility.Split(directoryName);

			if (!craftedExploits.TryGetValue(subject.InstanceId, out CraftedExploitFile file))
			{
				file = new CraftedExploitFile(world, subject.InstanceId);
				craftedExploits.Add(subject.InstanceId, file);
			}

			overrider.AddFile(directory, file);
			UpdateInstance(subject, file);
		}

		private void UpdateInstance(WorldCraftedExploitData subject, CraftedExploitFile file)
		{
			string filename = PathUtility.GetFileName(subject.FilePath);

			file.Name = filename;
			file.Exploit = Exploits.First(x => x.Name == subject.Exploit);
			file.Payload = Payloads.FirstOrDefault(x => x.Name == subject.Payload);
		}

		private IComputer GetComputer(ObjectId computer)
		{
			if (computer == ObjectId.Invalid)
				return playerInstance.Value.Computer;
			else
			{
				this.npcComputers.TryGetComputer(computer, out NonPlayerComputer npc);
				return npc;
			}
		}
		
		private IFileOverrider GetFileOverrider(ObjectId computer)
		{
			if (computer == ObjectId.Invalid)
				return playerInstance.Value.FileOverrider;
			else
				return npcComputers.GetNpcFileOverrider(computer);
		}

		private void ReloadHackingStuff()
		{
			this.exploits = gameManager.ContentManager.GetContentOfType<ExploitAsset>().ToArray();
			this.payloads = gameManager.ContentManager.GetContentOfType<PayloadAsset>().ToArray();
		}
		
		private sealed class ReloadHackingStuffHook : IHookListener
		{
			private readonly HackingSystem hackingSystem;

			public ReloadHackingStuffHook(HackingSystem hackingSystem)
			{
				this.hackingSystem = hackingSystem;
			}
			
			/// <inheritdoc />
			public Task ReceiveHookAsync(IGameContext game)
			{
				hackingSystem.ReloadHackingStuff();
				return Task.CompletedTask;
			}
		}
	}
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Modding;
using OS.Devices;

namespace Core.Scripting.WorldCommands
{
	public class SpawnIspCommand : WorldCommand
	{
		/// <inheritdoc />
		protected override async Task OnExecute(IWorldManager worldManager, IScriptExecutionContext context, ITextConsole console, string name, string[] args)
		{
			if (args.Length < 2)
			{
				throw new InvalidOperationException("usage: spawnisp <narrativeID> <name>");
			}

			string narrativeId = args[0];
			string ispName = args[1];

			var isp = worldManager.World.InternetProviders.FirstOrDefault(x => x.NarrativeId == narrativeId);

			var ispIsNew = false;
			if (isp.NarrativeId != narrativeId)
			{
				ispIsNew = true;
				isp.InstanceId = worldManager.GetNextObjectId();
				isp.CidrNetwork = worldManager.GetNextIspRange();
				isp.NarrativeId = narrativeId;
			}

			isp.Name = ispName;

			if (ispIsNew)
			{
				worldManager.World.InternetProviders.Add(isp);
			}
			else
			{
				worldManager.World.InternetProviders.Modify(isp);
			}

		}
	}
}
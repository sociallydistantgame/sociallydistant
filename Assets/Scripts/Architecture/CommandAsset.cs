#nullable enable
using System.Threading.Tasks;
using OS.Devices;
using UnityEngine;

namespace Architecture
{
	[CreateAssetMenu(menuName = "ScriptableObject/Assets/Command Asset")]
	public class CommandAsset : ScriptableCommandBase
	{
		[SerializeField]
		private CommandScript commandScriptPrefab = null!;
		
		/// <inheritdoc />
		public override async Task Main(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			// Disable the prefab first so Awake is deferred.
			commandScriptPrefab.gameObject.SetActive(false);
			CommandScript instance = Instantiate(commandScriptPrefab);
			commandScriptPrefab.gameObject.SetActive(true);
			
			// Wake the script up!
			instance.gameObject.SetActive(true);
			
			// Pass everything onto the script
			await instance.Main(process, console, arguments);
		}
	}
}
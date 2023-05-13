#nullable enable
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
		public override void Main(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			// Disable the prefab first so Awake is deferred.
			commandScriptPrefab.gameObject.SetActive(false);
			CommandScript instance = Instantiate(commandScriptPrefab);
			commandScriptPrefab.gameObject.SetActive(true);
			
			// Pass everything onto the script
			instance.Main(process, console, arguments);

			// Wake the script up!
			instance.gameObject.SetActive(true);
		}
	}
}
#nullable enable
using Core;
using UnityEngine;

namespace Architecture
{
	[CreateAssetMenu(menuName = "ScriptableObject/Assets/Lifepath Description Asset")]
	public class LifepathAsset : 
		ScriptableObject,
		INamedAsset
	{
		[SerializeField]
		private string uniqueId = string.Empty;

		[SerializeField]
		private string lifepathName = string.Empty;

		[TextArea]
		[Multiline]
		[SerializeField]
		private string description = "";

		/// <inheritdoc />
		public string Name => uniqueId;

		public string LifepathName => lifepathName;

		public string Description => description;
	}
}
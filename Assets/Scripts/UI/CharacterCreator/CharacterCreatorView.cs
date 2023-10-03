using AcidicGui.Mvc;
using UnityEngine;

namespace UI.CharacterCreator
{
	public abstract class CharacterCreatorView : ViewWithData<CharacterCreatorState>
	{
		[SerializeField]
		private string title = string.Empty;

		[SerializeField]
		private string description = string.Empty;

		public string Title => title;
		public string Description => description;
	}
}
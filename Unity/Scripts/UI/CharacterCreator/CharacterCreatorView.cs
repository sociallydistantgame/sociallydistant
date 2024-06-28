﻿using System.Threading.Tasks;
using AcidicGui.Mvc;
using UnityEngine;

namespace UI.CharacterCreator
{
	public abstract class CharacterCreatorView : ViewWithData<CharacterCreatorState>
	{
		
		private string title = string.Empty;

		
		private string description = string.Empty;

		public virtual bool CanGoForward => true;
		
		public string Title => title;
		public string Description => description;

		public virtual Task<bool> ConfirmNextPage()
		{
			return Task.FromResult(true);
		}
	}
}
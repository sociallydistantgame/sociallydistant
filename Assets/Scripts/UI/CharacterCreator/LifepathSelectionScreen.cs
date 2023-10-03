using System;
using Architecture;
using UnityEngine;

namespace UI.CharacterCreator
{
	public class LifepathSelectionScreen : CharacterCreatorView
	{
		[SerializeField]
		private LifepathAsset[] lifepaths = Array.Empty<LifepathAsset>();
		
		private CharacterCreatorState state;
		
		/// <inheritdoc />
		public override void SetData(CharacterCreatorState data)
		{
			this.state = state;
		}
	}
}
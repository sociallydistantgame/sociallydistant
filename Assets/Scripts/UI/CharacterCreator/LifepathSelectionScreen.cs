namespace UI.CharacterCreator
{
	public class LifepathSelectionScreen : CharacterCreatorView
	{
		private CharacterCreatorState state;
		
		/// <inheritdoc />
		public override void SetData(CharacterCreatorState data)
		{
			this.state = state;
		}
	}
}
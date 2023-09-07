namespace UI.CharacterCreator
{
	public class WelcomeScreen : CharacterCreatorView
	{
		private CharacterCreatorState state;
		
		/// <inheritdoc />
		public override void SetData(CharacterCreatorState data)
		{
			this.state = state;
		}
	}
}
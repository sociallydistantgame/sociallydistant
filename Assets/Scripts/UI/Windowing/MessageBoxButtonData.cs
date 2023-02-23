#nullable enable
namespace UI.Windowing
{
	public struct MessageBoxButtonData
	{
		public string Text;
		public string Icon;
		
		public static implicit operator MessageBoxButtonData(string text)
		{
			return new MessageBoxButtonData
			{
				Icon = string.Empty,
				Text = text
			};
		}
	}
}
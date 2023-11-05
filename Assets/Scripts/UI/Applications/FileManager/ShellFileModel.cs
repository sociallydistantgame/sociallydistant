#nullable enable
using Shell.Common;

namespace UI.Applications.FileManager
{
	public class ShellFileModel
	{
		public string Name { get; set; } = string.Empty;
		public string Path { get; set; } = string.Empty;
		public CompositeIcon Icon { get; set; }
	}
}
#nullable enable
namespace Core.Scripting
{
	public class ShellTester
	{
		private readonly string[] expressions;

		public ShellTester(string[] expressions)
		{
			this.expressions = expressions;
		}
		
		public int Test()
		{
			return 0;
		}
	}
}
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shell.Windowing
{
	public interface ITabbedContent : IContentHolder
	{
		IReadOnlyList<IContentPanel> Tabs { get; }
		
		void NextTab();
		void PreviousTab();
		void SwitchTab(int index);
		
		IContentPanel CreateTab();
		Task<bool> RemoveTab(IContentPanel panel);
	}
}
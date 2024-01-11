using System;
using System.Threading.Tasks;
using Shell.Common;

namespace Shell.Windowing
{
	public interface IContentPanel : 
		ICloseable
	{
		IWindow Window { get; }
		string Title { get; set; }
		CompositeIcon Icon { get; set; }
		IContent? Content { get; set; }
		
		IObservable<string> TitleObservable { get; } 
	}
}
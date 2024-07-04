﻿using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.Core.Shell.Windowing
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
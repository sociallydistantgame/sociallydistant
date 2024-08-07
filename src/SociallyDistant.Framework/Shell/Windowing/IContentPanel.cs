﻿using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.Core.Shell.Windowing
{
	public interface IContentPanel : 
		ICloseable
	{
		Action<IContentPanel>? Closed { get; set; }
		
		IWindow Window { get; }
		string Title { get; set; }
		CompositeIcon Icon { get; set; }
		Widget? Content { get; set; }
	}
}
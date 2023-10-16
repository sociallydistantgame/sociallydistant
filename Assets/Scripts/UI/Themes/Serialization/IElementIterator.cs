#nullable enable
using System.Collections.Generic;

namespace UI.Themes.Serialization
{
	public interface IElementIterator
	{
		string ElementName { get; }
		
		IEnumerable<IElementSerializer> ChildElements { get; }
	}
}
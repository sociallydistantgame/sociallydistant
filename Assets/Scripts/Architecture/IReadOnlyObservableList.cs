using System.Collections.Generic;

#nullable enable
using System;

namespace Architecture
{
	public interface IReadOnlyObservableList<T> : IReadOnlyList<T>
	{
		event Action<T>? ItemAdded;
		event Action<T>? ItemRemoved;
	}
}
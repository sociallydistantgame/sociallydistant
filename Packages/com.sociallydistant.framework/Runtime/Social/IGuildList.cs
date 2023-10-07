#nullable enable
using System;
using System.Collections.Generic;

namespace Social
{
	public interface IGuildList : 
		IReadOnlyList<IGuild>,
		IDisposable
	{
		IGuildList ThatHaveMember(IProfile profile);
		
		IObservable<IGuild> ObserveGuildAdded();
		IObservable<IGuild> ObserveGuildRemoved();
	}
}
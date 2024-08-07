﻿#nullable enable
namespace SociallyDistant.Core.Social
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
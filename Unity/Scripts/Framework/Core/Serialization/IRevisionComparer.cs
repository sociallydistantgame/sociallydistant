using System;

namespace Core.Serialization
{
	public interface IRevisionComparer<TRevision> where TRevision : Enum
	{
		TRevision Earliest { get; }
		TRevision Latest { get; }
		TRevision Current { get; }

		bool IsCurrent(TRevision revision);
		bool IsCurrentOrNewer(TRevision revision);
		bool IsCurrentOrOlder(TRevision revision);
		bool IsNewer(TRevision revision);
		bool IsOlder(TRevision revision);
	}
}
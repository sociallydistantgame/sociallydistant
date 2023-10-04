using System.Collections.Generic;

namespace Social
{
	public interface IChatGroup
	{
		IEnumerable<IChatMember> Members { get; }
	}
}
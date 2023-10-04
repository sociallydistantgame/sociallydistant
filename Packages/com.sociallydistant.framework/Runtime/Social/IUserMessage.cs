using System;

namespace Social
{
	public interface IUserMessage
	{
		IProfile Author { get; }
		DateTime Date { get; }
	}
}
using Core;
using UnityEngine.Analytics;

namespace Social
{
	public interface IProfile
	{
		ObjectId ProfileId { get; }
		
		Gender Gender { get; }
		string Bio { get; }
		
		bool IsPrivate { get; }
		
		string ChatName { get; }
		string ChatUsername { get; }
	}
}
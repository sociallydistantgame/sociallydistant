#nullable enable
using System.Collections.Generic;
using Core.Serialization;

namespace Core
{
	public interface IWorldFlagCollection :
		IList<string>,
		IWorldData
	{
		
	}
}
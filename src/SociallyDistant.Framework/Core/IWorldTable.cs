#nullable enable
using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.Core
{
	/// <summary>
	///		Represents a serializable table of world data objects.
	/// </summary>
	/// <typeparam name="TDataElement">The type of object contained within the data table</typeparam>
	public interface IWorldTable<TDataElement> : 
		ISerializableDataTable<TDataElement, WorldRevision, IWorldSerializer>
		where TDataElement : struct, IWorldData, IDataWithId
	{
	}

	public interface INarrativeObjectTable<TDataElement> :
		IWorldTable<TDataElement>
		where TDataElement : struct, IWorldData, IDataWithId, INarrativeObject
	{
		TDataElement GetNarrativeObject(string narrativeId);

		bool ContainsNarrativeId(string narrativeId);
	}
}
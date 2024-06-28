namespace SociallyDistant.Core.Core.Serialization
{
	public interface IWorldData : ISerializable<WorldRevision, IWorldSerializer>
	{
		
	}

	public interface INarrativeObject
	{
		string? NarrativeId { get; set; }
	}
}
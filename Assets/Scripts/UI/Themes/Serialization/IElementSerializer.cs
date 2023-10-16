#nullable enable
namespace UI.Themes.Serialization
{
	public interface IElementSerializer : IBasicDataSerializer
	{
		bool IsReading { get; }
		string ElementName { get; }

		IElementSerializer? GetChildElement(string elementName);
		IElementIterator GetChildCollection(string elementName, string itemName);
	}
}
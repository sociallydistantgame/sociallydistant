#nullable enable
namespace UI.Themes.Serialization
{
	public interface IBasicDataSerializer :
		INumericValueSerializer,
		IValueSerializer<bool>,
		IValueSerializer<string>
	{
		
	}
}
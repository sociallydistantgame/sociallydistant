#nullable enable
namespace UI.Themes.Serialization
{
	public interface INumericValueSerializer :
		IValueSerializer<int>,
		IValueSerializer<float>
	{
		
	}
}
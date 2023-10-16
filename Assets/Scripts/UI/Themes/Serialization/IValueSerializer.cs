#nullable enable

namespace UI.Themes.Serialization
{
	public interface IValueSerializer<T>
	{
		void Serialize(ref T value, string name, T defaultValue);
	}
}
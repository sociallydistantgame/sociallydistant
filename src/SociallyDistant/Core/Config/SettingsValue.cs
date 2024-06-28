using SociallyDistant.Core.Core.Config;

namespace SociallyDistant.Core.Config
{
	public struct SettingsValue
	{
		public SettingsType Type;
		public float FloatValue;
		public int IntValue;
		public string? StringValue;
		public bool BoolValue;
	}
}
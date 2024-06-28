namespace SociallyDistant.Core.Core.WorldData.Data
{
	[Flags]
	public enum CharacterAttributes : ushort
	{
		None = 0,
		Verified = 1,
		Integral = Verified << 1,
		Scripted = Integral << 1
	}
}
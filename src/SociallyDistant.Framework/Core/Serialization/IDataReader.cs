namespace SociallyDistant.Core.Core.Serialization
{
	public interface IDataReader : IDisposable
	{
		sbyte Read_sbyte();
		byte Read_byte();
		short Read_short();
		ushort Read_ushort();
		int Read_int();
		uint Read_uint();
		long Read_long();
		ulong Read_ulong();
		bool Read_bool();
		float Read_float();
		double Read_double();
		decimal Read_decimal();
		char Read_char();
		string Read_string();
		DateTime ReadDateTime();
	}
}
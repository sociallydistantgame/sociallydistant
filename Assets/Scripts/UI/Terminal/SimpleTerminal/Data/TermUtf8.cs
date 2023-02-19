namespace UI.Terminal.SimpleTerminal.Data
{
    public static class TermUtf8
    {
        public static readonly byte[] utfbyte = new byte[EmulatorConstants.UTF_SIZ_PLUS_ONE]
            { 0x80, 0, 0xC0, 0xE0, 0xF0 };

        public static readonly byte[] utfmask = new byte[EmulatorConstants.UTF_SIZ_PLUS_ONE]
            { 0xC0, 0x80, 0xE0, 0xF0, 0xF8 };

        public static readonly uint[] utfmin = new uint[EmulatorConstants.UTF_SIZ_PLUS_ONE]
            { 0, 0, 0x80, 0x800, 0x10000 };

        public static readonly uint[] utfmax = new uint[EmulatorConstants.UTF_SIZ_PLUS_ONE]
            { 0x10FFFF, 0x7F, 0x7FF, 0xFFFF, 0x10FFFF };


        public static int utf8decode(byte[] c, int offset, out uint u, int clen)
        {
            int i, j, len = default, type = default;
            uint udecoded;

            u = EmulatorConstants.UTF_INVALID;
            if (clen == 0)
                return 0;
            udecoded = utf8decodebyte(c[offset], ref len);
            if (!BETWEEN(len, 1, EmulatorConstants.UTF_SIZ))
                return 1;
            for (i = 1, j = 1; i < clen && j < len; ++i, ++j)
            {
                udecoded = (udecoded << 6) | utf8decodebyte(c[offset + i], ref type);
                if (type != 0)
                    return j;
            }

            if (j < len)
                return 0;
            u = udecoded;
            utf8validate(ref u, len);

            return len;
        }

        public static uint utf8decodebyte(byte c, ref int i)
        {
            for (i = 0; i < utfmask.Length; ++i)
                if ((c & utfmask[i]) == utfbyte[i])
                    return (uint)(c & ~utfmask[i]);

            return 0;
        }

        public static unsafe int utf8encode(uint u, byte* c)
        {
            int len, i;

            len = utf8validate(ref u, 0);
            if (len > EmulatorConstants.UTF_SIZ)
                return 0;

            for (i = len - 1; i != 0; --i)
            {
                c[i] = utf8encodebyte(u, 0);
                u >>= 6;
            }

            c[0] = utf8encodebyte(u, len);

            return len;
        }

        public static byte utf8encodebyte(uint u, int i)
        {
            return (byte)(utfbyte[i] | (u & ~utfmask[i]));
        }

        public static int utf8validate(ref uint u, int i)
        {
            if (!BETWEEN(u, utfmin[i], utfmax[i]) || BETWEEN(u, 0xD800, 0xDFFF))
                u = EmulatorConstants.UTF_INVALID;
            for (i = 1; u > utfmax[i]; ++i)
                ;

            return i;
        }

        private static bool BETWEEN(int x, int a, int b)
        {
            return x >= a && x <= b;
        }

        private static bool BETWEEN(uint x, uint a, uint b)
        {
            return x >= a && x <= b;
        }
    }
}
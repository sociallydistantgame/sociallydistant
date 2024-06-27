using System;

namespace UI.Terminal.SimpleTerminal.Data
{
    public class STREscape
    {
        public byte type;
        public byte[] buf = Array.Empty<byte>();
        public int siz;
        public int len;
        public byte[] args = new byte[EmulatorConstants.STR_ARG_SIZ];
        public int narg;
    }
}
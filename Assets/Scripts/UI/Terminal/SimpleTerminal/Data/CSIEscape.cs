namespace UI.Terminal.SimpleTerminal.Data
{
    public class CSIEscape
    {
        public byte[] buf = new byte[EmulatorConstants.ESC_BUF_SIZ];
        public int len;
        public byte priv;
        public int[] arg = new int[EmulatorConstants.ESC_ARG_SIZ];
        public int narg;
        public byte[] mode = new byte[2];
    }
}
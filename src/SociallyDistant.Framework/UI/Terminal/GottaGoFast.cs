namespace SociallyDistant.Core.UI.Terminal;

/// <summary>
///		Really unsafe but really speedy C stuff for things that gotta go fast.
/// </summary>
public static class GottaGoFast
{
    public static unsafe long strtol(byte* str, ref byte** endchar, int b)
    {
        long i = 0;

        byte* p = str;

        byte ascii = 0;
        byte zero = 48;
        byte nine = 57;
        while ((ascii = *p) >= zero && ascii <= nine)
        {
            int num = ascii - zero;

            i = i * b + num;

            p++;
        }

        (*endchar) = p;

        return i;
    }
}
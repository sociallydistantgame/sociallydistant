using System.Reflection;
using Microsoft.Xna.Framework;

namespace AcidicGUI;

public static class ColorHelpers
{
    private static readonly Dictionary<string, Color> namedColors = new();

    private static readonly Dictionary<char, byte> nibbles = new Dictionary<char, byte>
    {
        { '0', 0 },
        { '1', 1 },
        { '2', 2 },
        { '3', 3 },
        { '4', 4 },
        { '5', 5 },
        { '6', 6 },
        { '7', 7 },
        { '8', 8 },
        { '9', 9 },
        { 'a', 10 },
        { 'b', 11 },
        { 'c', 12 },
        { 'd', 13 },
        { 'e', 14 },
        { 'f', 15 }
    };

    static ColorHelpers()
    {
        var colorType = typeof(Color);
        var properties = colorType.GetProperties(BindingFlags.Public | BindingFlags.Static);

        foreach (PropertyInfo property in properties)
        {
            if (property.PropertyType != colorType)
                break;

            string name = property.Name.ToLower();

            var color = (Color?)property.GetValue(null);
            if (!color.HasValue)
                continue;

            namedColors.Add(name, color.Value);
        }
    }
    
    public static bool ParseColor(string colorString, out Color color)
    {
        if (namedColors.TryGetValue(colorString.ToLower(), out color))
            return true;

        if (colorString.StartsWith("#"))
        {
            string hex = colorString.Substring(1).ToLower();

            switch (hex.Length)
            {
                // 12-bit color
                case 3:
                {
                    if (!ParseHex(hex[0], hex[0], out byte r) || !ParseHex(hex[1], hex[1], out byte g) ||
                        !ParseHex(hex[2], hex[2], out byte b))
                        return false;

                    color = new Color(r, g, b);
                    return true;
                }
                // 12-bit color, 4-bit alpha
                case 4:
                {
                    if (!ParseHex(hex[0], hex[0], out byte r) || !ParseHex(hex[1], hex[1], out byte g) ||
                        !ParseHex(hex[2], hex[2], out byte b) || !ParseHex(hex[3], hex[3], out byte a))
                        return false;

                    color = new Color(r, g, b, a);
                    return true;
                }
                // 24-bit color
                case 6:
                {
                    if (!ParseHex(hex[0], hex[1], out byte r) || !ParseHex(hex[2], hex[3], out byte g) ||
                        !ParseHex(hex[4], hex[5], out byte b))
                        return false;

                    color = new Color(r, g, b);
                    return true;
                }
                // 24-bit color, 8-bit alpha
                case 8:
                {
                    if (!ParseHex(hex[0], hex[1], out byte r) || !ParseHex(hex[2], hex[3], out byte g) ||
                        !ParseHex(hex[4], hex[5], out byte b) || !ParseHex(hex[6], hex[7], out byte a))
                        return false;

                    color = new Color(r, g, b, a);
                    return true;
                }
                default: return false;
            }
        }
        
        return false;
    }

    private static bool ParseHex(char a, char b, out byte value)
    {
        value = 0;

        if (!nibbles.TryGetValue(a, out byte nibbleA))
            return false;
        
        if (!nibbles.TryGetValue(b, out byte nibbleB))
            return false;


        value = (byte)((nibbleA << 4) | nibbleB);
        return true;
    }
}
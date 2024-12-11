using System;

namespace SystemWatch;

public class Utils
{
    private static readonly string[] ByteUnits = new string[] { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

    public static string GetByteUnitLabel(double value)
    {
        for (int i = 0, count = ByteUnits.Length; i < count; i++)
        {
            if (value < 1024)
            {
                return ByteUnits[i];
            }
            value /= 1024;
        }
        return "B";
    }
    
    public static string FormatByteSize(int len, double value, int type = 0)
    {
        for (int i = type, count = ByteUnits.Length; i < count; i++)
        {
            if (value < 1024)
            {
                string result = Convert.ToString(value);
                len = len > result.Length ? result.Length : len;
                return result.Substring(0, len) + ByteUnits[i];
            }
            value /= 1024;
        }
        return "0B";
    }

    public static int GetByteScale(double value)
    {
        int scale = 1;
        while (value >= 1024)
        {
            scale *= 1024;
            value /= 1024;
        }
        return scale;
    }

    public static double[] FormatByteValues(double[] values, double maxValue)
    {
        int scale = GetByteScale(maxValue);
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = Math.Round(values[i] / scale, 2);
        }
        return values;
    }
}
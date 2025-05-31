using System;

public class Converter
{
    public static int ConvertToInt(byte[] bytes, int startIndex, Func<byte[], int, int> converter)
    {
        return converter(bytes, startIndex);
    }
}
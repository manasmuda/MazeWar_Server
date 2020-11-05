using System;
using System.Text;

public class RandomStringGenerator
{
    private static readonly Random _random = new Random();
    public static string GetRandomString(int size)
    {
        var builder = new StringBuilder(size);

        char offset = 'A';
        const int lettersOffset = 26; // A...Z or a..z: length=26  

        for (var i = 0; i < size; i++)
        {
            var @char = (char)_random.Next(offset, offset + lettersOffset);
            builder.Append(@char);
        }

        return builder.ToString();
    }
}

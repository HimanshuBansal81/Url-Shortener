namespace UrlShortener.Application.Services;

internal static class Base62Encoder
{
    private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string Encode(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be greater than zero.");
        }

        Span<char> buffer = stackalloc char[11];
        var position = buffer.Length;

        while (value > 0)
        {
            buffer[--position] = Alphabet[value % Alphabet.Length];
            value /= Alphabet.Length;
        }

        return new string(buffer[position..]);
    }
}

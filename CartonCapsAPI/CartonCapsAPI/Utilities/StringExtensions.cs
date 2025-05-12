namespace CartonCapsAPI.Utilities;

public static class StringExtensions
{
    public static string GetValueOrDefault(this string? str)
    {
        return str ?? string.Empty;
    }
}

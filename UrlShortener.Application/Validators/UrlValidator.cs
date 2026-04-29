using UrlShortener.Application.Exceptions;

public static class UrlValidator
{
    public static Uri ValidateOriginalUrl(string originalUrl)
    {
        originalUrl = originalUrl.Trim();
        if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out var parsedUri) ||
            (parsedUri.Scheme != Uri.UriSchemeHttp && parsedUri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidUrlException("Original URL must be a valid HTTP or HTTPS URL.");
        }

        return parsedUri;
    }
}
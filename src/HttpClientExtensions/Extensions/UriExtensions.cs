namespace HttpClientExtensions.Extensions;

public static class UriExtensions
{
    public static string BuildUri(this string host, string endpoint)
    {
        return $"{host.TrimEnd('/')}/{endpoint.TrimStart('/')}";
    }
}

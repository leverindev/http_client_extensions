using System.Net.Http;

namespace HttpClientExtensions.Models;

public record HttpResponseResult(HttpResponseMessage? Response, string? ContentMessage)
{
    public HttpResponseMessage? Response { get; } = Response;

    public string? ContentMessage { get; } = ContentMessage;
}
